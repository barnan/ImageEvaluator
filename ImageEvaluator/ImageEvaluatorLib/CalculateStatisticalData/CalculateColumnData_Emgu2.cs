
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using System.Drawing;
using System;
using System.CodeDom;
using ImageEvaluatorInterfaces;
using NLog;
using System.Collections.Generic;
using ImageEvaluatorInterfaces.BaseClasses;

namespace ImageEvaluatorLib.CalculateStatisticalData
{
    class CalculateColumnDataEmgu2 : CalculateColumnDataBaseEmgu
    {

        Image<Gray, byte> _complementaryMask;
        Image<Gray, float> _squareImage;
        Image<Gray, float> _squaremean;
        Image<Gray, float> _mean;
        Image<Gray, float> _counter;
        Image<Gray, float> _convertedMask;
        Image<Gray, float> _sq;
        Image<Gray, float> _resu;
        Image<Gray, float> _sqrt;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        internal CalculateColumnDataEmgu2(ILogger logger, int width, int height)
            : base(logger, width, height)
        {
            _logger?.Info("CalculateColumnData_Emgu2 instantiated.");
        }


        public override bool Run(List<NamedData> data, int[,] pointArray, ref Image<Gray, double> meanVector, ref Image<Gray, double> stdVector,
            out double resu1, out double resu2, out double resu3, out double resu4, out double resu5, out double resu6, out double resu7, out double resu8,
            out double resu9, out double resu10)
        {
            Image<Gray, byte>[] rawImages = null;
            Image<Gray, byte>[] maskImages = null;

            resu1 = 0;
            resu2 = 0;
            resu3 = 0;
            resu4 = 0;
            resu5 = 0;
            resu6 = 0;
            resu7 = 0;
            resu8 = 0;
            resu9 = 0;
            resu10 = 0;

            _firstVector = new Image<Gray, double>(_height, 1);
            _secondVector = new Image<Gray, double>(_height, 1);
            _resultVector1 = _firstVector.Data;
            _resultVector2 = _secondVector.Data;

            if (!IsInitialized)
            {
                _logger.Error("CalculateColumnData_CSharp2 is not initialized.");
                return false;
            }

            rawImages = GetEmguByteImages("_rawImages", data);
            int imageCounterRaw = rawImages?.Length ?? 0;

            maskImages = GetEmguByteImages("maskImages", data);
            int imageCounterMask = maskImages?.Length ?? 0;

            if (imageCounterMask != imageCounterRaw)
            {
                _logger.Info($"{this.GetType()} input and mask image number is not the same!");
                return false;
            }

            for (int m = 0; m < imageCounterRaw; m++)
            {

                if (!CheckInputData(rawImages[m], maskImages[m], pointArray, meanVector, stdVector))
                {
                    _logger.Info($"{this.GetType()} input and mask data is not proper!");
                    continue;
                }

                meanVector = _firstVector;
                stdVector = _secondVector;

                // mask the outer area to zero
                Gray zero = new Gray(0.0);
                using (_complementaryMask = maskImages[m].Not())
                {
                    rawImages[m].SetValue(zero, _complementaryMask);
                }

                using (Image<Gray, ushort> tempImage = rawImages[m].Convert<Gray, ushort>())
                {
                    using (_convertedMask = maskImages[m].Convert<Gray, float>())
                    {
                        //calculate square image
                        //_squareImage = tempImage.Pow(2);
                        CvInvoke.Pow(tempImage, 2, _squareImage);

                        // reduce the sum2 (sum)
                        //_squaremean = new Image<Gray, float>(new Size(1, inputImage.Height));
                        CvInvoke.Reduce(_squareImage, _squaremean, ReduceDimension.Auto, ReduceType.ReduceSum);

                        // reduce the sum image (sum)
                        //_mean = new Image<Gray, float>(new Size(1, inputImage.Height));
                        CvInvoke.Reduce(rawImages[m], _mean, ReduceDimension.Auto, ReduceType.ReduceSum);

                        // reduce the sum image (sum)
                        //_counter = new Image<Gray, float>(new Size(1, inputImage.Height));

                        CvInvoke.Reduce(_convertedMask, _counter, ReduceDimension.Auto, ReduceType.ReduceSum);

                        //Image<Gray, float> counterReciprok = counter.Pow(-1);

                        CvInvoke.Divide(_squaremean, _counter, _squaremean);
                        CvInvoke.Divide(_mean, _counter, _mean);

                        //squaremean._Mul(counterReciprok);
                        //mean._Mul(counterReciprok);

                        // calculate   Math.Sqrt(sum2 / (counter-1) - (sum * sum / Math.Pow(counter-1,2)));
                        CvInvoke.Pow(_mean, 2, _sq);
                        CvInvoke.Subtract(_squaremean, _sq, _resu);
                        CvInvoke.Pow(_resu, 0.5, _sqrt);
                        //_sq = _mean.Pow(2);
                        //_resu = _squaremean - _sq;
                        //_sqrt = _resu.Pow(0.5);

                        //ClearEmguImages();
                    }
                }

                float[,,] meanData = _mean.Data;
                float[,,] resuData = _sqrt.Data;

                for (int i = 0; i < _mean.Height; i++)
                {
                    _resultVector1[0, i, 0] = meanData[i, 0, 0];
                    _resultVector2[0, i, 0] = resuData[i, 0, 0];
                }

            }

            return true;

        }


        protected override bool CheckInputData(Image<Gray, byte> inputImage, Image<Gray, byte> maskImage, int[,] pointArray, Image<Gray, double> meanVector, Image<Gray, double> stdVector)
        {
            base.CheckInputData(inputImage, maskImage, pointArray, meanVector, stdVector);

            if (maskImage == null || maskImage.Height < 0 || maskImage.Height > 10000 || maskImage.Width < 0 || maskImage.Width > 10000)
            {
                return false;
            }
            if (inputImage.Height != maskImage.Height || inputImage.Width != maskImage.Width)
            {
                return false;
            }

            return true;
        }


        protected override bool InitEmguImages()
        {
            if (IsInitialized)
                return true;

            try
            {
                //_complementaryMask = new Image<Gray, byte>(_width, _height);
                _squareImage = new Image<Gray, float>(_width, _height);
                _squaremean = new Image<Gray, float>(1, _height);
                _mean = new Image<Gray, float>(1, _height);
                _counter = new Image<Gray, float>(1, _height);
                //_convertedMask = new Image<Gray, float>(_width, _height);
                _sq = new Image<Gray, float>(_width, _height);
                _resu = new Image<Gray, float>(_width, _height);
                _sqrt = new Image<Gray, float>(_width, _height);

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during InitEmguImages: {ex}");
                return IsInitialized = false;
            }

            return IsInitialized = true;
        }


        protected override bool ClearEmguImages()
        {
            _complementaryMask?.Dispose();
            _squareImage?.Dispose();
            _squaremean?.Dispose();
            _mean?.Dispose();
            _counter?.Dispose();
            _convertedMask?.Dispose();
            _sq?.Dispose();
            _resu?.Dispose();
            _sqrt?.Dispose();

            return IsInitialized = false;
        }

    }



    /// <summary>
    /// 
    /// </summary>
    public class FactoryCalculateColumnDataEmgu2 : IColumnDataCalculator_Creator
    {
        public IColumnDataCalculator Factory(ILogger logger, int width, int height)
        {
            return new CalculateColumnDataEmgu2(logger, width, height);
        }
    }

}
