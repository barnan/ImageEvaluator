using System;
using System.Drawing;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using ImageEvaluatorInterfaces;
using NLog;

namespace ImageEvaluatorLib.CalculateStatisticalData
{
    internal abstract class CalculateColumnDataBase : IColumnDataCalculator
    {
        protected int _width;
        protected int _height;
        protected Image<Gray, double> _meanVector;
        protected Image<Gray, double> _stdVector;
        protected double[,,] _resultVector1;
        protected double[,,] _resultVector2;
        protected ILogger _logger;
        protected Rectangle _fullMask;


        protected MCvScalar _meanOfMean;
        protected MCvScalar _stdOfMean;
        protected MCvScalar _meanOfStd;
        protected MCvScalar _stdOfStd;

        protected MCvScalar _meanOfRegion1;
        protected MCvScalar _meanOfRegion2;
        protected MCvScalar _meanOfRegion3;

        private Matrix<byte> _reducedMask;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        protected CalculateColumnDataBase(ILogger logger, int width, int height)
        {
            _logger = logger;
            _width = width;
            _height = height;
        }


        public bool Init()
        {
            IsInitialized = InitEmguImages();

            _logger?.Info("CalculateColumnData_Base " + (IsInitialized ? string.Empty : "NOT") + " Initialized.");

            return IsInitialized;
        }

        public bool IsInitialized { get; protected set; }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputImage"></param>
        /// <param name="maskImage"></param>
        /// <param name="pointArray"></param>
        /// <param name="meanVector"></param>
        /// <param name="stdVector"></param>
        /// <param name="resu3"></param>
        /// <param name="resu4"></param>
        /// <param name="resu1"></param>
        /// <param name="resu2"></param>
        /// <returns></returns>
        public abstract bool Run(Image<Gray, byte> inputImage, Image<Gray, byte> maskImage, int[,] pointArray, ref Image<Gray, double> meanVector, ref Image<Gray, double> stdVector,
                                out double resu1, out double resu2, out double resu3, out double resu4, out double resu5, out double resu6);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputImage"></param>
        /// <param name="maskImage"></param>
        /// <param name="pointArray"></param>
        /// <param name="meanVector"></param>
        /// <param name="stdVector"></param>
        /// <returns></returns>
        protected virtual bool CheckInputData(Image<Gray, byte> inputImage, Image<Gray, byte> maskImage, int[,] pointArray, Image<Gray, double> meanVector, Image<Gray, double> stdVector)
        {
            try
            {
                if (inputImage == null || inputImage.Height != _height || inputImage.Width != _width)
                {
                    _logger?.Error($"Error in the input image size. Predefined width: {_width}, Predefined height: {_height}, image width: {inputImage?.Width}, image height: {inputImage?.Height}");
                    return false;
                }
                if (meanVector == null || stdVector == null || meanVector.Width != inputImage.Height || stdVector.Width != inputImage.Height)
                {
                    _logger?.Error($"Error in the meanVector and stdVector length. meanVector height:{meanVector?.Height} stdVector height:{stdVector?.Height} meanVector height:{inputImage.Height}.");
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger?.Error($"Error during CalculateColumnData CheckInputImage: {ex}");
            }

            return true;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected virtual bool InitEmguImages()
        {
            if (IsInitialized)
                return true;

            try
            {
                //_meanVector = new Image<Gray, double>(_height, 1);
                //_stdVector = new Image<Gray, double>(_height, 1);
                _fullMask = new Rectangle(0, 0, _width, _height);
                _reducedMask = new Matrix<byte>(_height, 1);

                return true;
            }
            catch (Exception ex)
            {
                _logger?.Error($"Error during CalculcateColumnData - Init: {ex}.");
                return false;
            }
        }


        protected virtual bool ClearEmguImages()
        {
            _meanVector?.Dispose();
            _stdVector?.Dispose();
            _reducedMask?.Dispose();

            IsInitialized = false;

            return true;
        }


        protected bool CalculateStatistics(int indexMin, int indexMax, Image<Gray, byte> maskImage)
        {
            _meanOfMean = new MCvScalar();
            _stdOfMean = new MCvScalar();
            _meanOfStd = new MCvScalar();
            _stdOfStd = new MCvScalar();

            _meanOfRegion1 = new MCvScalar();
            _meanOfRegion2 = new MCvScalar();
            _meanOfRegion3 = new MCvScalar();

            MCvScalar stdOfRegion1 = new MCvScalar();
            MCvScalar stdOfRegion2 = new MCvScalar();
            MCvScalar stdOfRegion3 = new MCvScalar();

            try
            {
                maskImage.Reduce(_reducedMask, ReduceDimension.SingleCol, ReduceType.ReduceAvg);
                Image<Gray, byte> tempReducedMask = new Image<Gray, byte>(_reducedMask.Height, 1);

                for (int i = 0; i < _reducedMask.Height; i++)
                {
                    if (_reducedMask[i, 0] == 0)
                    {
                        tempReducedMask.Data[0, i, 0] = 0;
                    }
                    else
                    {
                        tempReducedMask.Data[0, i, 0] = 255;
                    }
                }


                int value1 = _reducedMask.Rows;
                int value2 = 0;
                for (int i = indexMin; i < indexMax; i++)
                {
                    if (_reducedMask[i, 0] == 0)
                    {
                        value1 = i - indexMin;
                        break;
                    }
                }
                for (int i = indexMax; i > indexMin; i--)
                {
                    if (_reducedMask[i, 0] == 0)
                    {
                        value2 = indexMax - i;
                        break;
                    }
                }

                Rectangle rect1 = new Rectangle(indexMin, 0, indexMax - indexMin, 1);
                Rectangle fullRoi = new Rectangle(0, 0, _meanVector.Width, 1);

                //_meanVector.ROI = rect1;
                //_stdVector.ROI = rect1;

                CvInvoke.MeanStdDev(_meanVector, ref _meanOfMean, ref _stdOfMean, tempReducedMask);
                CvInvoke.MeanStdDev(_stdVector, ref _meanOfStd, ref _stdOfStd, tempReducedMask);

                int regionWidth = (indexMax - indexMin) / 5;
                Rectangle rect3 = new Rectangle(indexMin, 0, Math.Min(regionWidth, value1), 1);
                Rectangle rect4 = new Rectangle(indexMin + 2 * regionWidth, 0, regionWidth, 1);
                Rectangle rect5 = new Rectangle(indexMax - Math.Min(regionWidth, value2), 0, Math.Min(regionWidth, value2), 1);

                _meanVector.ROI = rect3;
                CvInvoke.MeanStdDev(_meanVector, ref _meanOfRegion1, ref stdOfRegion1);

                _meanVector.ROI = rect4;
                CvInvoke.MeanStdDev(_meanVector, ref _meanOfRegion2, ref stdOfRegion2);

                _meanVector.ROI = rect5;
                CvInvoke.MeanStdDev(_meanVector, ref _meanOfRegion3, ref stdOfRegion3);

                _meanVector.ROI = fullRoi;
                _stdVector.ROI = fullRoi;

                return true;
            }
            catch (Exception ex)
            {
                _logger?.Error($"Exception occured during CalculateColumnDataEmgu1 - CalculateStatistics: {ex}");
                return false;
            }
        }



    }
}
