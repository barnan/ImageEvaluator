using Emgu.CV;
using Emgu.CV.Structure;
using NLog;
using System;
using ImageEvaluatorInterfaces;
using ImageEvaluatorInterfaces.BaseClasses;

namespace ImageEvaluatorLib.BorderSearch
{
    class BorderSearcherCSharp1 : BorderSearcherBase
    {

        internal BorderSearcherCSharp1(ILogger logger, int border, int imageWidth, int imageHeight)
            : base(logger, imageWidth, imageHeight, border)
        {
            ClassName = nameof(BorderSearcherCSharp1);
            Title = ClassName;

            _logger?.InfoLog("Instantiated.", ClassName);
        }


        protected override bool CalculatePoints(Image<Gray, ushort> origImage, Image<Gray, byte> maskImage, string name)
        {
            try
            {
                byte[,,] imgArray = maskImage.Data;

                for (int i = 0; i < maskImage.Height; i++)
                {
                    for (int j = 0; j < maskImage.Width / 4; j++)
                    {
                        if (imgArray[i, j, 0] > 0)
                        {
                            _borderPoints[i, 0] = j + _borderSkipSize;
                            break;
                        }
                    }
                }

                for (int i = 0; i < maskImage.Height; i++)
                {
                    for (int j = maskImage.Width - 1; j > maskImage.Width * 3 / 4; j--)
                    {
                        if (imgArray[i, j, 0] > 0)
                        {
                            _borderPoints[i, 1] = j - _borderSkipSize;
                            break;
                        }
                    }
                }

                if (_showImages)
                {
                    SavePointList(name, "PointList");
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger?.ErrorLog($"Exception occured: {ex}", ClassName);
                return false;
            }

        }
    }



    /// <summary>
    /// 
    /// </summary>
    public class FactoryBorderSearcherCSharp1 : IBorderSeracher_Creator
    {
        public IBorderSearcher Factory(ILogger logger, int border, int imageWidth, int imageHeight, bool showImages)
        {
            logger?.InfoLog("Factory called.", nameof(FactoryBorderSearcherCSharp1));

            return new BorderSearcherCSharp1(logger, border, imageWidth, imageHeight);
        }
    }
}

