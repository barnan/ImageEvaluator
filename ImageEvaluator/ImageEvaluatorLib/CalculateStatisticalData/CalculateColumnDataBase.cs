using System;
using System.Drawing;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using ImageEvaluatorInterfaces;
using NLog;
using System.Collections.Generic;
using ImageEvaluatorInterfaces.BaseClasses;
using ImageEvaluatorLib.BaseClasses;

namespace ImageEvaluatorLib.CalculateStatisticalData
{

    internal abstract class CalculateColumnDataBase : NamedDataProvider, IColumnDataCalculator
    {
        protected int _width;
        protected int _height;

        protected ILogger _logger;
        protected Rectangle _fullROI;
        protected Rectangle _fullLineROI;
        protected string[] _enumNames;
        protected string lorum;

        protected Image<Gray, double> _firstVector;
        protected Image<Gray, double> _secondVector;


        protected MCvScalar _meanOfFirst;
        protected MCvScalar _minOfFirst;
        protected MCvScalar _maxOfFirst;
        protected MCvScalar _stdOfFirst;
        protected MCvScalar _meanOfSecond;
        protected MCvScalar _stdOfSecond;

        protected MCvScalar _meanOfRegion1OfFirst;
        protected MCvScalar _meanOfRegion2OfFirst;
        protected MCvScalar _meanOfRegion3OfFirst;

        protected MCvScalar _stdOfRegion1OfFirst;
        protected MCvScalar _stdOfRegion2OfFirst;
        protected MCvScalar _stdOfRegion3OfFirst;

        private Matrix<byte> _reducedMask;

        public string ClassName { get; protected set; }
        public string Title { get; protected set; }


        protected CalculateColumnDataBase(ILogger logger, int width, int height)
        {
            _logger = logger;
            _width = width;
            _height = height;

            ClassName = nameof(CalculateColumnDataBase);
            Title = ClassName;
        }


        public bool Init()
        {
            _enumNames = Enum.GetNames(typeof(DoubleNamingConvention));

            IsInitialized = InitEmguImages();

            _logger?.InfoLog((IsInitialized ? string.Empty : "NOT") + " Initialized.", ClassName);

            return IsInitialized;
        }


        public bool IsInitialized { get; protected set; }


        public abstract bool Execute(List<NamedData> data, string fileName);


        protected virtual bool CheckInputData(Image<Gray, ushort> inputImage, Image<Gray, byte> maskImage, int[,] pointArray, Image<Gray, double> meanVector, Image<Gray, double> stdVector)
        {
            try
            {
                if (!GeneralImageHandling.CheckImages(inputImage, maskImage, _width, _height, _logger))
                {
                    return false;
                }

                if (meanVector == null || stdVector == null || meanVector.Width != inputImage.Height || stdVector.Width != inputImage.Height)
                {
                    _logger?.ErrorLog($"Error in the meanVector and stdVector length. meanVector height:{meanVector?.Height} stdVector height:{stdVector?.Height} meanVector height:{inputImage.Height}.", ClassName);
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger?.ErrorLog($"Exception occured: {ex}", ClassName);
            }

            return true;
        }


        protected virtual bool InitEmguImages()
        {
            if (IsInitialized)
                return true;

            try
            {
                _fullROI = new Rectangle(0, 0, _width, _height);
                _fullLineROI = new Rectangle(0, 0, _width, 1);
                _reducedMask = new Matrix<byte>(_height, 1);

                return true;
            }
            catch (Exception ex)
            {
                _logger?.ErrorLog($"Exception occured: {ex}", ClassName);
                return false;
            }
        }


        protected virtual bool ReAllocateEmgu()
        {
            if (!IsInitialized)
                return false;

            _firstVector = new Image<Gray, double>(_height, 1);
            _secondVector = new Image<Gray, double>(_height, 1);

            return true;
        }


        protected virtual bool ClearEmguImages()
        {
            _firstVector?.Dispose();
            _secondVector?.Dispose();
            _reducedMask?.Dispose();

            IsInitialized = false;

            return true;
        }


        protected virtual int LoadNamedData(List<NamedData> data, ref BorderPointArrays borderPoints, ref Image<Gray, ushort>[] rawImages, ref Image<Gray, byte>[] maskImages)
        {
            rawImages = GetEmguUShortImages("RawImages", data);
            int imageCounterRaw = rawImages?.Length ?? 0;

            maskImages = GetEmguByteImages("MaskImages", data);
            int imageCounterMask = maskImages?.Length ?? 0;

            borderPoints = GetBorderPointArrays("BorderPointArrayList", data);


            if ((imageCounterMask != imageCounterRaw) || (imageCounterRaw == 0) || (imageCounterRaw != borderPoints.Count))
            {
                _logger?.InfoLog($"Input and mask image number is not the same!", ClassName);
                return 0;
            }

            return imageCounterRaw;
        }



        protected bool CalculateStatistics(int indexMin, int indexMax, Image<Gray, byte> maskImage)
        {
            try
            {
                _meanOfFirst = new MCvScalar();
                _stdOfFirst = new MCvScalar();
                _meanOfSecond = new MCvScalar();
                _stdOfSecond = new MCvScalar();
                _minOfFirst = new MCvScalar();
                _maxOfFirst = new MCvScalar();

                _meanOfRegion1OfFirst = new MCvScalar();
                _meanOfRegion2OfFirst = new MCvScalar();
                _meanOfRegion3OfFirst = new MCvScalar();

                _stdOfRegion1OfFirst = new MCvScalar();
                _stdOfRegion2OfFirst = new MCvScalar();
                _stdOfRegion3OfFirst = new MCvScalar();


                maskImage.Reduce(_reducedMask, ReduceDimension.SingleCol, ReduceType.ReduceAvg);
                Image<Gray, byte> tempReducedMask = new Image<Gray, byte>(_reducedMask.Height, 1);

                int thresh = 150; // 150 means -> 1204 pixels in case of H2048, 2409 pixels in ase of  H4096, 4818 in case of H8192  (H->Height)

                for (int i = 0; i < _reducedMask.Height; i++)
                {
                    if (_reducedMask[i, 0] < thresh && _firstVector.Data[0, i, 0] < double.Epsilon)
                    {
                        tempReducedMask.Data[0, i, 0] = 0;
                    }
                    else
                    {
                        tempReducedMask.Data[0, i, 0] = 255;
                    }
                }

                for (int i = 0; i < _reducedMask.Height / 2; i++)
                {
                    tempReducedMask.Data[0, i, 0] = 0;
                    if (_reducedMask[i, 0] > thresh && _firstVector.Data[0, i, 0] > double.Epsilon)
                    {
                        indexMin = i;
                        break;
                    }
                }
                for (int i = _reducedMask.Height - 1; i > _reducedMask.Height / 2; i--)
                {
                    tempReducedMask.Data[0, i, 0] = 0;
                    if (_reducedMask[i, 0] > thresh && _firstVector.Data[0, i, 0] > double.Epsilon)
                    {
                        indexMax = i;
                        break;
                    }
                }

                int value1 = _reducedMask.Height;
                int value2 = _reducedMask.Height;
                for (int i = indexMin; i < indexMax; i++)
                {
                    if (_reducedMask[i, 0] < thresh && _firstVector.Data[0, i, 0] < double.Epsilon)
                    {
                        value1 = i - indexMin;
                        break;
                    }
                }
                for (int i = indexMax; i > indexMin; i--)
                {
                    if (_reducedMask[i, 0] < thresh && _firstVector.Data[0, i, 0] < double.Epsilon)
                    {
                        value2 = indexMax - i;
                        break;
                    }
                }


                CvInvoke.MeanStdDev(_firstVector, ref _meanOfFirst, ref _stdOfFirst, tempReducedMask);
                CvInvoke.MeanStdDev(_secondVector, ref _meanOfSecond, ref _stdOfSecond, tempReducedMask);

                double maxVal = 0.0;
                double minVal = 0.0;
                Point maxPos = new Point();
                Point minPos = new Point();

                CvInvoke.MinMaxLoc(_firstVector, ref minVal, ref maxVal, ref minPos, ref maxPos, tempReducedMask);
                _minOfFirst = new MCvScalar(minVal);
                _maxOfFirst = new MCvScalar(maxVal);

                int regionWidth = (indexMax - indexMin) / 5;
                Rectangle rect3 = new Rectangle(indexMin, 0, Math.Min(regionWidth, value1), 1);
                Rectangle rect4 = new Rectangle(indexMin + 2 * regionWidth, 0, regionWidth, 1);
                Rectangle rect5 = new Rectangle(indexMax - Math.Min(regionWidth, value2), 0, Math.Min(regionWidth, value2), 1);

                _firstVector.ROI = rect3;
                CvInvoke.MeanStdDev(_firstVector, ref _meanOfRegion1OfFirst, ref _stdOfRegion1OfFirst);

                _firstVector.ROI = rect4;
                CvInvoke.MeanStdDev(_firstVector, ref _meanOfRegion2OfFirst, ref _stdOfRegion2OfFirst);

                _firstVector.ROI = rect5;
                CvInvoke.MeanStdDev(_firstVector, ref _meanOfRegion3OfFirst, ref _stdOfRegion3OfFirst);

                return true;
            }
            catch (Exception ex)
            {
                _logger?.ErrorLog($"Exception occured: {ex}", ClassName);
                return false;
            }
            finally
            {
                if (_firstVector != null && _fullLineROI != null)
                    _firstVector.ROI = _fullLineROI;

                if (_secondVector != null && _fullLineROI != null)
                    _secondVector.ROI = _fullLineROI;
            }
        }


    }
}
