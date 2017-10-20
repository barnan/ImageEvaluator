﻿using System;
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
        protected Rectangle _fullROI;
        protected Rectangle _fullLineROI;


        protected MCvScalar _meanOfMean;
        protected MCvScalar _minOfMean;
        protected MCvScalar _maxOfMean;
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
        /// <param name="resu5"></param>
        /// <param name="resu6"></param>
        /// <param name="resu7"></param>
        /// <param name="resu8"></param>
        /// <param name="resu9"></param>
        /// <param name="resu10"></param>
        /// <returns></returns>
        public abstract bool Run(Image<Gray, byte> inputImage, Image<Gray, byte> maskImage, int[,] pointArray, ref Image<Gray, double> meanVector, ref Image<Gray, double> stdVector,
                                out double resu1, out double resu2, out double resu3, out double resu4, out double resu5, out double resu6, out double resu7, out double resu8,
                                out double resu9, out double resu10);


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
                _fullROI = new Rectangle(0, 0, _width, _height);
                _fullLineROI = new Rectangle(0, 0, _width, 1);
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

                int thresh = 150; // 150 means -> 1204 pixels in case of H2048, 2409 pixels in ase of  H4096, 4818 in case of H8192  (H->Height)

                for (int i = 0; i < _reducedMask.Height / 2; i++)
                {
                    tempReducedMask.Data[0, i, 0] = 0;
                    if (_reducedMask[i, 0] > thresh)  
                    {
                        indexMin = i;
                        break;
                    }
                }
                for (int i = _reducedMask.Height - 1; i > _reducedMask.Height / 2; i--)
                {
                    tempReducedMask.Data[0, i, 0] = 0;
                    if (_reducedMask[i, 0] > thresh)
                    {
                        indexMax = i;
                        break;
                    }
                }

                int value1 = _reducedMask.Height;
                int value2 = _reducedMask.Height;
                for (int i = indexMin; i < indexMax; i++)
                {
                    if (_reducedMask[i, 0] < thresh)
                    {
                        value1 = i - indexMin;
                        break;
                    }
                }
                for (int i = indexMax; i > indexMin; i--)
                {
                    if (_reducedMask[i, 0] < thresh)
                    {
                        value2 = indexMax - i;
                        break;
                    }
                }

                for (int i = 0; i < _reducedMask.Height; i++)
                {
                    if (_reducedMask[i, 0] < thresh)
                    {
                        tempReducedMask.Data[0, i, 0] = 0;
                    }
                    else
                    {
                        tempReducedMask.Data[0, i, 0] = 255;
                    }
                }

                CvInvoke.MeanStdDev(_meanVector, ref _meanOfMean, ref _stdOfMean, tempReducedMask);
                CvInvoke.MeanStdDev(_stdVector, ref _meanOfStd, ref _stdOfStd, tempReducedMask);

                int regionWidth = (indexMax - indexMin)/5;
                Rectangle rect3 = new Rectangle(indexMin, 0, Math.Min(regionWidth, value1), 1);
                Rectangle rect4 = new Rectangle(indexMin + 2*regionWidth, 0, regionWidth, 1);
                Rectangle rect5 = new Rectangle(indexMax - Math.Min(regionWidth, value2), 0, Math.Min(regionWidth, value2), 1);

                _meanVector.ROI = rect3;
                CvInvoke.MeanStdDev(_meanVector, ref _meanOfRegion1, ref stdOfRegion1);

                _meanVector.ROI = rect4;
                CvInvoke.MeanStdDev(_meanVector, ref _meanOfRegion2, ref stdOfRegion2);

                _meanVector.ROI = rect5;
                CvInvoke.MeanStdDev(_meanVector, ref _meanOfRegion3, ref stdOfRegion3);

                double maxVal = 0.0;
                double minVal = 0.0;
                Point maxPos = new Point();
                Point minPos = new Point();

                CvInvoke.MinMaxLoc(_meanVector, ref minVal, ref maxVal, ref minPos, ref maxPos, tempReducedMask);
                _minOfMean = new MCvScalar(minVal);
                _maxOfMean = new MCvScalar(maxVal);

                return true;
            }
            catch (Exception ex)
            {
                _logger?.Error($"Exception occured during CalculateColumnDataEmgu1 - CalculateStatistics: {ex}");
                return false;
            }
            finally
            {
                _meanVector.ROI = _fullROI;
                _stdVector.ROI = _fullLineROI;
            }
        }


    }
}
