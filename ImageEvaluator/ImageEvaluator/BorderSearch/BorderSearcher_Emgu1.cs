using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using System;
using System.Drawing;

namespace ImageEvaluator.SearchContourPoints
{
    class BorderSearcher_Emgu1 : IBorderSearcher
    {
        int[,] _borderPoints;
        int _borderSkipSize;
        bool _showImages;


        public BorderSearcher_Emgu1(int border, bool show)
        {
            _borderSkipSize = border;
            _showImages = show;
        }


        public int[,] GetBorderPoints(Image<Gray, byte> maskImage, ref string outMessage)
        {
            try
            {
                if (!CheckInputImage(maskImage))
                {
                    outMessage = "Invalid input image.";

                    return null;
                }

                Init(maskImage.Height);

                CalculatePoints(maskImage);

            }
            catch (Exception ex)
            {
                outMessage = $"Exception during border points calculation: {ex.Message}";
                return null;
            }

            return _borderPoints;
        }


        private void CalculatePoints(Image<Gray, byte> maskImage)
        {
            using (Mat hierarchy = new Mat())
            {
                using (VectorOfVectorOfPoint contour = new VectorOfVectorOfPoint())
                {
                    //image1
                    CvInvoke.FindContours(maskImage, contour, hierarchy, RetrType.List, ChainApproxMethod.ChainApproxSimple);
                    //List<Point> pointList = new List<Point>();

                    int verticalCenterLine = maskImage.Width / 2;
                    int magicNumber1 = 2000;

                    for (int i = 0; i < contour.Size; i++)
                    {
                        Point[] coordinateList = contour[i].ToArray();

                        for (int j = 0; j < contour[i].Size - 1; j++)
                        {
                            if ((coordinateList[j].Y != coordinateList[j + 1].Y) && (Math.Abs(coordinateList[j].Y - coordinateList[j + 1].Y) < magicNumber1))
                            {
                                LineSegment2D contourLineSegment = new LineSegment2D(new Point(coordinateList[j].X, coordinateList[j].Y),
                                    new Point(coordinateList[j + 1].X, coordinateList[j + 1].Y));

                                LineSegment2D horizontalLine;
                                for (int k = 0; k < Math.Abs(coordinateList[j + 1].Y - coordinateList[j].Y); k++)
                                {
                                    int difference = coordinateList[j + 1].Y - coordinateList[j].Y;
                                    int yCoord = coordinateList[j].Y + k * (difference / Math.Abs(difference));

                                    horizontalLine = new LineSegment2D(new Point(0, yCoord),
                                        new Point(4095, yCoord));

                                    var resu = GetIntersection(horizontalLine, contourLineSegment);

                                    if (resu.X < verticalCenterLine)
                                    {
                                        if (_borderPoints[yCoord, 0] < resu.X)
                                        {
                                            _borderPoints[yCoord, 0] = resu.X;
                                        }
                                    }
                                    else
                                    {
                                        if (_borderPoints[yCoord, 1] > resu.X)
                                        {
                                            _borderPoints[yCoord, 1] = resu.X;
                                        }
                                    }


                                }
                            }

                        }
                    }


                }
            }
        }

        private bool Init(int height)
        {
            _borderPoints = new int[height, 2];

            return true;
        }

        private bool CheckInputImage(Image<Gray, byte> maskImage)
        {
            if (maskImage == null || maskImage.Height < 0 || maskImage.Height > 10000 || maskImage.Width < 0 || maskImage.Width > 10000)
            {
                return false;
            }
            return true;
        }

        private Point GetIntersection(LineSegment2D line1, LineSegment2D line2)
        {

            if (line1.P1.X == line1.P2.X)
            {
                return new Point((int)line1.P2.X, line2.P2.Y);
            }
            double a1 = (line1.P1.Y - line1.P2.Y) / (double)(line1.P1.X - line1.P2.X);
            double b1 = line1.P1.Y - a1 * line1.P1.X;


            if (line2.P1.X == line2.P2.X)
            {
                return new Point((int)line2.P2.X, line1.P2.Y);
            }
            double a2 = (line2.P1.Y - line2.P2.Y) / (double)(line2.P1.X - line2.P2.X);
            double b2 = line2.P1.Y - a2 * line2.P1.X;


            if (Math.Abs(a1 - a2) < Double.Epsilon)
                throw new InvalidOperationException();

            double x = (b2 - b1) / (a1 - a2);
            double y = a1 * x + b1;
            return new Point((int)(x + 0.5), (int)(y + 0.5));
        }

    }
}
