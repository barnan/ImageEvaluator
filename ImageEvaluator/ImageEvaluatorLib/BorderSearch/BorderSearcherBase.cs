using System;
using Emgu.CV;
using Emgu.CV.Structure;
using ImageEvaluatorInterfaces;
using NLog;

namespace ImageEvaluatorLib.SearchContourPoints
{
    abstract class BorderSearcherBase : IBorderSearcher
    {

        protected int[,] _borderPoints;
        protected int _borderSkipSize;
        protected bool _showImages;
        private bool _initialized;
        private int _imageHeight;
        private Image<Gray, byte> _maskImage;
        protected ILogger _logger;


        public BorderSearcherBase(ILogger logger, int imageHeight, int border)
        {
            _imageHeight = imageHeight;
            _logger = logger;
            _borderSkipSize = border;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool Init()
        {
            _initialized = InitArrays();
            _logger?.Info("BorderSearcher_Base " + (_initialized ? string.Empty : "NOT") + " initialized.");
            return _initialized;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="maskImage"></param>
        /// <param name="pointList"></param>
        /// <returns></returns>
        public bool Run(Image<Gray, byte> maskImage, ref int[,] pointList)
        {
            if (!_initialized)
            {
                _logger?.Error("BorderSearch is not initialized yet.");
                return false;
            }

            try
            {
                if (!CheckInputImage(maskImage))
                {
                    _logger?.Error("GetBorderPoints - Invalid input image.");
                    return false;
                }

                ResetPointList();
                pointList = _borderPoints;

                CalculatePoints(maskImage);
            }
            catch (Exception ex)
            {
                _logger?.Error($"Exception during border points calculation: {ex.Message}");
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
        protected bool InitArrays()
        {
            if (_initialized)
                return _initialized;

            _borderPoints = new int[_imageHeight, 2];

            return true;
        }



        protected bool ResetPointList()
        {
            if (!_initialized)
                return false;

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
            if (maskImage == null || maskImage.Height != _imageHeight || maskImage.Width != _imageHeight)
            {
                return false;
            }
            return true;
        }


    }
}
