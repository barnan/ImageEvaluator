using Emgu.CV;
using Emgu.CV.Structure;
using NLog;
using System;
using ImageEvaluator.Interfaces;

namespace ImageEvaluator.SearchContourPoints
{
    class BorderSearcher_CSharp1 : BorderSearcherBase
    {

        internal BorderSearcher_CSharp1(ILogger logger, int border, int imageHeight)
            : base(logger, imageHeight)
        {
            _borderSkipSize = border;

            _logger?.Info("BorderSearcher_CSharp1 instantiated.");
        }


        protected override void CalculatePoints(Image<Gray, byte> maskImage)
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

            }
            catch (Exception ex)
            {
                _logger?.Error($"Exception caught in BorderSearcher_CSharp1-CalculatePoints: {ex.Message}.");
            }

        }
    }




    /// <summary>
    /// 
    /// </summary>
    class Factory_BorderSearcher_CSharp1 : IBorderSeracher_Creator
    {
        public IBorderSearcher Factory(ILogger logger, int border, int imageHeight, bool showImages)
        {
            return new BorderSearcher_CSharp1(logger, border, imageHeight);
        }
    }
}

