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
using ImageEvaluatorInterfaces.BaseClasses;
using ImageEvaluatorLib.BaseClasses;

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

            ClassName = nameof(BorderSearcherEmgu2);
            Title = ClassName;

            _logger?.InfoLog("Instantiated.", ClassName);
        }

        protected override bool CalculatePoints(Image<Gray, ushort> origImage, Image<Gray, byte> maskImage, string name, string enumName)
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
                        Point[] _coordinateList = contour[i].ToArray();

                        if (_coordinateList.Length > 300)
                        {
                            maskImage.SetValue(255.0, maskImage);

                            if (_showImages)
                            {

                                using (var tempImage = new Image<Gray, byte>(maskImage.Size))
                                {
                                    maskImage.CopyTo(tempImage);
                                    tempImage.Draw(_coordinateList, new Gray(100.0), 2);
                                    ImageViewer.Show(tempImage, "BorderSearcher_Emgu2 - contour points");

                                    GeneralImageHandling.SaveImage(name, _ownFolderNameForSaving, "MaskImage" + enumName, tempImage, _logger);
                                }

                                using (var tempImage = new Image<Gray, ushort>(origImage.Size))
                                {
                                    origImage.CopyTo(tempImage);
                                    tempImage.Draw(_coordinateList, new Gray(200.0), 2);
                                    ImageViewer.Show(tempImage, "BorderSearcher_Emgu2 - contour points");

                                    GeneralImageHandling.SaveImage(name, _ownFolderNameForSaving, "OrigImage" + enumName, tempImage, _logger);
                                }
                            }

                            for (int j = 0; j < contour[i].Size - 1; j++)
                            {
                                if ((_coordinateList[j].Y != _coordinateList[j + 1].Y) && (Math.Abs(_coordinateList[j].Y - _coordinateList[j + 1].Y) < magicNumber1))
                                {
                                    if (_coordinateList[j].X + _borderSkipSize < verticalCenterLine)
                                    {
                                        if (_borderPoints[_coordinateList[j].Y, 0] < _coordinateList[j].X + _borderSkipSize)
                                        {
                                            _borderPoints[_coordinateList[j].Y, 0] = _coordinateList[j].X + _borderSkipSize;
                                        }
                                    }
                                    else if (_coordinateList[j].X - _borderSkipSize > verticalCenterLine)
                                    {
                                        if (_borderPoints[_coordinateList[j].Y, 1] > _coordinateList[j].X - _borderSkipSize)
                                        {
                                            _borderPoints[_coordinateList[j].Y, 1] = _coordinateList[j].X - _borderSkipSize;
                                        }
                                    }

                                }
                            }
                        }

                    }

                    return true;
                }
                catch (Exception ex)
                {
                    _logger?.ErrorLog($"Exception occured: {ex}.", ClassName);
                    return false;
                }

            }

        }

    }


    public class FactoryBorderSearcherEmgu2 : IBorderSeracher_Creator
    {
        public IBorderSearcher Factory(ILogger logger, int border, int imageWidth, int imageHeight, bool showImages)
        {
            logger?.InfoLog("Factory called.", nameof(FactoryBorderSearcherEmgu2));

            return new BorderSearcherEmgu2(logger, border, showImages, imageWidth, imageHeight);
        }
    }


}
