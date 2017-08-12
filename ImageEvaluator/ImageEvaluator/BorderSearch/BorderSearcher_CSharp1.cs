using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageEvaluator.SearchContourPoints
{
    class BorderSearcher_CSharp1 : IBorderSearcher
    {
        int[,] _borderPoints;
        int _borderSkipSize;


        public BorderSearcher_CSharp1(int border)
        {
            _borderSkipSize = border;
        }


        public int[,] GetBorderPoints(Image<Gray, byte> maskImage, ref string outMessage)
        {
            try
            {
                if (!CheckInputImage(maskImage))
                {
                    outMessage = "invalid input image.";

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


    }
}
