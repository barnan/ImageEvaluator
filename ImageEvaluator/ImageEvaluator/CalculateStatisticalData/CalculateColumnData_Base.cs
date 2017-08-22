using System;
using Emgu.CV;
using Emgu.CV.Structure;
using NLog;

namespace ImageEvaluator.CalculateStatisticalData
{
    abstract class CalculateColumnData_Base : IColumnDataCalculator
    {

        protected bool _initialized;
        protected int _width;
        protected int _height;
        protected Image<Gray, float> _meanVector;
        protected Image<Gray, float> _stdVector;
        protected float[,,] _resultVector1;
        protected float[,,] _resultVector2;
        protected ILogger _logger;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        internal CalculateColumnData_Base(ILogger logger, int width, int height)
        {
            _logger = logger;
            _width = width;
            _height = height;
        }


        public bool Init()
        {
            _initialized = InitEmguImages();

            _logger.Info("CalculateColumnData_Base " + (_initialized ? string.Empty : "NOT") + " Initialized.");

            return _initialized;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputImage"></param>
        /// <param name="maskImage"></param>
        /// <param name="meanVector"></param>
        /// <param name="stdVector"></param>
        /// <returns></returns>
        public abstract bool Run(Image<Gray, float> inputImage, Image<Gray, byte> maskImage, int[,] pointArray, ref Image<Gray, float> meanVector, ref Image<Gray, float> stdVector);



        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputImage"></param>
        /// <param name="maskImage"></param>
        /// <returns></returns>
        protected virtual bool CheckInputData(Image<Gray, float> inputImage, Image<Gray, byte> maskImage, int[,] pointArray, Image<Gray, float> meanVector, Image<Gray, float> stdVector)
        {
            try
            {
                if (inputImage == null || inputImage.Height != _height || inputImage.Width != _width)
                {
                    _logger.Error($"Error in the input image size. Predefined width: {_width}, Predefined height: {_height}, image width: {inputImage?.Width}, image height: {inputImage.Height}");
                    return false;
                }
                if (meanVector == null || stdVector == null || meanVector.Height != inputImage.Height || stdVector.Height != inputImage.Height)
                {
                    _logger?.Error($"Error in the meanVector and stdVector length. meanVector height:{meanVector.Height} stdVector height:{stdVector.Height} meanVector height:{inputImage.Height}.");
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"Error during CalculateColumnData CheckInputImage: {ex.Message}");
            }

            return true;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected virtual bool InitEmguImages()
        {
            if (_initialized)
                return true;

            try
            {
                _meanVector = new Image<Gray, float>(_width, _height);
                _stdVector = new Image<Gray, float>(_width, _height);

                _resultVector1 = _meanVector.Data;
                _resultVector2 = _stdVector.Data;

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during CalculcateColumnData - Init: {ex.Message}.");
                return false;
            }
        }


        protected virtual bool ClearEmguImages()
        {
            _meanVector?.Dispose();
            _stdVector?.Dispose();

            _initialized = false;

            return true;
        }


    }
}
