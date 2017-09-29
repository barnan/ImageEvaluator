using System;
using System.Drawing;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using ImageEvaluator.Interfaces;
using ImageEvaluator.SearchContourPoints;
using NLog;
using System.Threading;
using Emgu.CV.UI;

namespace ImageEvaluator.SearchContourPoints
{

    /// <summary>
    /// it is using ChainApproxNone-t hence the line fitting can be eliminated
    /// </summary>
    class BorderSearcher_Emgu2 : BorderSearcherBase
    {
        public BorderSearcher_Emgu2(ILogger logger, int border, bool show, int imageHeight)
            : base(logger, imageHeight, border)
        {
            _showImages = show;
        }

        protected override void CalculatePoints(Image<Gray, byte> maskImage)
        {
            using (VectorOfVectorOfPoint contour = new VectorOfVectorOfPoint())
            {
                try
                {
                    CvInvoke.FindContours(maskImage, contour, null, RetrType.List, ChainApproxMethod.ChainApproxNone);

                    int verticalCenterLine = maskImage.Width / 2;
                    int magicNumber1 = 2000;

                    for (int i = 0; i < contour.Size; i++)
                    {
                        Point[] coordinateList = contour[i].ToArray();

                        if (_showImages)
                        {
                            maskImage.SetValue(new Gray(100.0), maskImage);

                            maskImage.Draw(coordinateList, new Gray(200.0), 2);
                            ImageViewer.Show(maskImage, "BorderSearcher_Emgu2 - contour points");
                        }

                        for (int j = 0; j < contour[i].Size - 1; j++)
                        {
                            if ((coordinateList[j].Y != coordinateList[j + 1].Y) && (Math.Abs(coordinateList[j].Y - coordinateList[j + 1].Y) < magicNumber1))
                            {
                                if (coordinateList[j].X < verticalCenterLine)
                                {
                                    if (_borderPoints[coordinateList[j].Y, 0] < coordinateList[j].X)
                                    {
                                        _borderPoints[coordinateList[j].Y, 0] = coordinateList[j].X;
                                    }
                                }
                                else
                                {
                                    if (_borderPoints[coordinateList[j].Y, 1] > coordinateList[j].X)
                                    {
                                        _borderPoints[coordinateList[j].Y, 1] = coordinateList[j].X;
                                    }
                                }


                            }
                        }

                    }
                }
                catch (Exception ex)
                {
                    _logger?.Error($"Exception caught in BorderSearcher_Emgu1-CalculatePoints: {ex.Message}.");
                }

            }

        }
    }


    public class Factory_BorderSearcher_Emgu2 : IBorderSeracher_Creator
    {
        public IBorderSearcher Factory(ILogger logger, int border, int imageHeight, bool showImages)
        {
            return new BorderSearcher_Emgu2(logger, border, showImages, imageHeight);
        }
    }


}
