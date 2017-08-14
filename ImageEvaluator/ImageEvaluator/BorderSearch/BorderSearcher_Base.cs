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
        bool _initialized;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="maskImage"></param>
        /// <param name="pointList"></param>
        /// <param name="outMessage"></param>
        /// <returns></returns>
        public bool GetBorderPoints(Image<Gray, byte> maskImage, ref int[,] pointList, ref string outMessage)
        {
            try
            {
                if (!CheckInputImage(maskImage))
                {
                    outMessage = "GetBorderPoints - Invalid input image.";
                    return false;
                }

                Init(maskImage.Height);

                ResetPointList();

                pointList = _borderPoints;

                CalculatePoints(maskImage);

            }
            catch (Exception ex)
            {
                outMessage = $"Exception during border points calculation: {ex.Message}";
                return false;
            }

            return true;
        }

        protected abstract void CalculatePoints(Image<Gray, byte> maskImage);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="height"></param>
        /// <returns></returns>
        protected bool Init(int height)
        {
            if (_initialized)
                return true;

            _borderPoints = new int[height, 2];

            return _initialized = true;
        }



        protected bool ResetPointList()
        {
            for (int i = 0; i < _borderPoints.Length / 2; i++)
            {
                _borderPoints[i, 0] = 0;
                _borderPoints[i, 1] = 4096;
            }

            return true;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="maskImage"></param>
        /// <returns></returns>
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
