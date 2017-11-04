
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
            ClassName = nameof(CalculateColumnDataEmgu2);
            Title = "Column Mean and Std calculator";

            _logger?.InfoLog("Instantiated.", ClassName);
        }


        public override bool Execute(List<NamedData> data, string fileName)
        {
            Image<Gray, ushort>[] rawImages = null;
            Image<Gray, byte>[] maskImages = null;
            BorderPointArrays borderPointarrays = null;

            try
            {
                if (!IsInitialized)
                {
                    _logger?.ErrorLog("It is not initialized.", ClassName);
                    return false;
                }

                int imageCounter = LoadNamedData(data, ref borderPointarrays, ref rawImages, ref maskImages);
                if (imageCounter == 0)
                {
                    _logger?.InfoLog("No images were loaded from dynamicresult", ClassName);
                }

                double[,,] resultVector1 = _firstVector.Data;
                double[,,] resultVector2 = _secondVector.Data;

                for (int m = 0; m < imageCounter; m++)
                {

                    if (!CheckInputData(rawImages[m], maskImages[m], borderPointarrays[m], _firstVector, _secondVector))
                    {
                        _logger?.InfoLog($"Input and mask data is not proper!", ClassName);
                        continue;
                    }

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
                        resultVector1[0, i, 0] = meanData[i, 0, 0];
                        resultVector2[0, i, 0] = resuData[i, 0, 0];
                    }

                }

                return true;
            }
            catch (Exception ex)
            {
                _logger?.ErrorLog($"Exception occured: {ex}", ClassName);
                return false;
            }
            finally
            {
                foreach (var item in rawImages)
                {
                    if (item != null)
                    {
                        item.ROI = _fullROI;
                    }
                }
                foreach (var item in maskImages)
                {
                    if (item != null)
                    {
                        item.ROI = _fullROI;
                    }
                }
            }


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
                _logger.InfoLog($"Error occured: {ex}", ClassName);
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
            logger?.InfoLog($"Factory called.", nameof(FactoryCalculateColumnDataEmgu2));

            return new CalculateColumnDataEmgu2(logger, width, height);
        }
    }

}
