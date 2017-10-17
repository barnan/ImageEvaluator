using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using NLog;
using System;
using System.Drawing;
using System.IO;
using ImageEvaluatorInterfaces;
using Emgu.CV.UI;

namespace ImageEvaluatorLib.BorderSearch
{
    class BorderSearcherEmgu1 : BorderSearcherBase
    {
        internal BorderSearcherEmgu1(ILogger logger, int border, bool show, int imageWidth, int imageHeight)
            : base(logger, imageWidth, imageHeight, border)
        {
            _showImages = show;

            _logger?.Info("BorderSearcher_Emgu1 instantiated.");
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="maskImage"></param>
        protected override void CalculatePoints(Image<Gray, byte> maskImage, string name)
        {
            using (Mat hierarchy = new Mat())
            {
                using (VectorOfVectorOfPoint contour = new VectorOfVectorOfPoint())
                {
                    try
                    {
                        CvInvoke.FindContours(maskImage, contour, hierarchy, RetrType.List, ChainApproxMethod.ChainApproxSimple);

                        int verticalCenterLine = maskImage.Width / 2;
                        int magicNumber1 = 2000;

                        for (int i = 0; i < contour.Size; i++)
                        {
                            Point[] coordinateList = contour[i].ToArray();

                            if (coordinateList.Length > 100)
                            {
                                if (_showImages)
                                {
                                    maskImage.SetValue(new Gray(100.0), maskImage);

                                    maskImage.Draw(coordinateList, new Gray(200.0), 2);
                                    ImageViewer.Show(maskImage, "BorderSearcher_Emgu1 - contour points");

                                    SaveMaskImage(name, maskImage);
                                }

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
                                            int yCoord = coordinateList[j].Y + k*(difference/Math.Abs(difference));

                                            horizontalLine = new LineSegment2D(new Point(0, yCoord), new Point(_imageWidth - 1, yCoord));

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

                        if (_showImages)
                        {
                            SavePointList(name);
                        }

                    }
                    catch (Exception ex)
                    {
                        _logger?.Error($"Exception caught in BorderSearcher_Emgu1-CalculatePoints: {ex}.");
                    }
                }
            }
        }

        /// <summary>
        ///  Calculates the intersection of the two input linesegments
        /// </summary>
        /// <param name="line1">linesegment 1</param>
        /// <param name="line2">linesegment 2</param>
        /// <returns>intersection Point</returns>
        protected Point GetIntersection(LineSegment2D line1, LineSegment2D line2)
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



    /// <summary>
    /// 
    /// </summary>
    public class FactoryBorderSearcherEmgu1 : IBorderSeracher_Creator
    {
        public IBorderSearcher Factory(ILogger logger, int border, int imageWidth, int imageHeight, bool showImages)
        {
            logger?.Info($"{typeof(FactoryBorderSearcherEmgu1)} factory called.");
            return new BorderSearcherEmgu1(logger, border, showImages, imageWidth, imageHeight);
        }
    }


}
