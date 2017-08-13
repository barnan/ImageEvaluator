using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV;
using Emgu.CV.Structure;

namespace ImageEvaluator.SearchContourPoints
{
    abstract class BorderSearcher_Base : IBorderSearcher
    {
        protected int[,] _borderPoints;
        protected int _borderSkipSize;
        protected bool _showImages;


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

        protected abstract void CalculatePoints(Image<Gray, byte> maskImage);

        protected bool Init(int height)
        {
            _borderPoints = new int[height, 2];

            return true;
        }

        protected bool CheckInputImage(Image<Gray, byte> maskImage)
        {
            if (maskImage == null || maskImage.Height < 0 || maskImage.Height > 10000 || maskImage.Width < 0 || maskImage.Width > 10000)
            {
                return false;
            }
            return true;
        }


    }
}
