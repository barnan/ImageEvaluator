using System;
using Emgu.CV;
using Emgu.CV.Structure;
using ImageEvaluatorInterfaces;
using NLog;

namespace ImageEvaluatorLib.BorderSearch
{
    internal abstract class BorderSearcherBase : IBorderSearcher
    {

        protected int[,] _borderPoints;
        protected int _borderSkipSize;
        protected bool _showImages;
        protected int _imageHeight;
        protected int _imageWidth;
        //private Image<Gray, byte> _maskImage;
        protected ILogger _logger;


        protected BorderSearcherBase(ILogger logger, int imageWidth, int imageHeight, int border)
        {
            _imageWidth = imageWidth;
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
            IsInitialized = InitArrays();
            _logger?.Info($"{typeof(BorderSearcherBase)}" + (IsInitialized ? string.Empty : " NOT") + " initialized.");
            return IsInitialized;
        }

        public bool IsInitialized { get; protected set; }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="maskImage"></param>
        /// <param name="pointList"></param>
        /// <returns></returns>
        public bool Run(Image<Gray, byte> maskImage, ref int[,] pointList)
        {
            if (!IsInitialized)
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
                _logger?.Error($"Exception during border points calculation: {ex}");
                return false;
            }

            return true;
        }



        protected abstract void CalculatePoints(Image<Gray, byte> maskImage);


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected bool InitArrays()
        {
            if (IsInitialized)
                return IsInitialized;

            _borderPoints = new int[_imageHeight, 2];

            return true;
        }



        protected bool ResetPointList()
        {
            if (!IsInitialized)
                return false;

            for (int i = 0; i < _borderPoints.Length / 2; i++)
            {
                _borderPoints[i, 0] = 0;
                _borderPoints[i, 1] = _imageWidth;
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
