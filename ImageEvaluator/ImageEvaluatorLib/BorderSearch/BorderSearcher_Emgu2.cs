using System;
using System.Drawing;
using System.IO;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using ImageEvaluatorInterfaces;
using NLog;
using Emgu.CV.UI;

namespace ImageEvaluatorLib.BorderSearch
{

    /// <summary>
    /// it is using ChainApproxNone-t hence the line fitting can be eliminated
    /// </summary>
    class BorderSearcherEmgu2 : BorderSearcherBase
    {
        public BorderSearcherEmgu2(ILogger logger, int border, bool show, int imageWidth, int imageHeight)
            : base(logger, imageWidth, imageHeight, border)
        {
            _showImages = show;
        }

        protected override void CalculatePoints(Image<Gray, byte> origImage, Image<Gray, byte> maskImage, string name)
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

                        if (coordinateList.Length > 300)
                        {
                            if (_showImages)
                            {
                                using (var tempImage = new Image<Gray, byte>(maskImage.Size))
                                {
                                    maskImage.SetValue(255.0, maskImage);
                                    maskImage.CopyTo(tempImage);
                                    tempImage.Draw(coordinateList, new Gray(100.0), 2);
                                    ImageViewer.Show(tempImage, "BorderSearcher_Emgu1 - contour points");

                                    SaveMaskImage(name, tempImage, "orig");
                                }

                                using (var tempImage = new Image<Gray, byte>(origImage.Size))
                                {
                                    origImage.CopyTo(tempImage);
                                    tempImage.Draw(coordinateList, new Gray(200.0), 2);
                                    ImageViewer.Show(tempImage, "BorderSearcher_Emgu1 - contour points");

                                    SaveMaskImage(name, tempImage, "orig");
                                }
                            }

                            for (int j = 0; j < contour[i].Size - 1; j++)
                            {
                                if ((coordinateList[j].Y != coordinateList[j + 1].Y) && (Math.Abs(coordinateList[j].Y - coordinateList[j + 1].Y) < magicNumber1))
                                {
                                    if (coordinateList[j].X < verticalCenterLine)
                                    {
                                        if (_borderPoints[coordinateList[j].Y, 0] < coordinateList[j].X + _borderSkipSize)
                                        {
                                            _borderPoints[coordinateList[j].Y, 0] = coordinateList[j].X + _borderSkipSize;
                                        }
                                    }
                                    else
                                    {
                                        if (_borderPoints[coordinateList[j].Y, 1] > coordinateList[j].X - _borderSkipSize)
                                        {
                                            _borderPoints[coordinateList[j].Y, 1] = coordinateList[j].X - _borderSkipSize;
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


    public class FactoryBorderSearcherEmgu2 : IBorderSeracher_Creator
    {
        public IBorderSearcher Factory(ILogger logger, int border, int imageWidth, int imageHeight, bool showImages)
        {
            return new BorderSearcherEmgu2(logger, border, showImages, imageWidth, imageHeight);
        }
    }


}
