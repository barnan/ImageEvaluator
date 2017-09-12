using System;
using Emgu.CV;
using Emgu.CV.Structure;
using NLog;
using Emgu.CV.UI;
using ImageEvaluator.Interfaces;

namespace ImageEvaluator.PreProcessor
{
    class ImagePreProcessor : IImagePreProcessor
    {
        private DenseHistogram _hist;
        private int _intensityRange;
        Image<Gray, float> _thresholdedImage;
        private bool _initialized;
        protected bool _showImages;
        private int _width;
        private int _height;
        private ILogger _logger;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="intensityRange"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        internal ImagePreProcessor(ILogger logger, int intensityRange, int width, int height, bool showImages)
        {
            _intensityRange = intensityRange;
            _showImages = showImages;
            _width = width;
            _height = height;
            _logger = logger;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public virtual bool Init()
        {
            _initialized = (CheckWidthData() && InitEmguImages());

            _logger?.Info("ImagePreProcessor " + (_initialized ? string.Empty : "NOT") + " initialized.");

            return _initialized;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputImage"></param>
        /// <param name="maskImage"></param>
        /// <returns></returns>
        public bool Run(Image<Gray, float> inputImage, ref Image<Gray, byte> maskImage)
        {
            try
            {
                CvInvoke.Transpose(inputImage, inputImage);

                // calculate historamm for binarythreshold
                _hist.Calculate<float>(new[] { inputImage }, false, null);

                float thresh = CalculateThresholdRegardingMoment(_hist);

                // create mask image:
                double maskValue = 255.0;
                _thresholdedImage = inputImage.ThresholdBinary(new Gray(thresh), new Gray(maskValue));
                maskImage = _thresholdedImage.Convert<Gray, byte>();

                if (_showImages)
                {
                    ImageViewer.Show(inputImage, "rotated image");
                    ImageViewer.Show(maskImage, "mask image");
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger?.Error($"Error in ImagePreProcessor - InitEmguImages. {ex.Message}");
                return false;
            }
        }


        private bool CheckWidthData()
        {
            if (_width > 10000 || _width < 0)
            {
                _logger?.Error($"Image width is not proper: {_width}");
                return false;
            }
            return true;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="intensityRange"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        private bool InitEmguImages()
        {
            if (_initialized)
                return true;

            try
            {
                _thresholdedImage = new Image<Gray, float>(_width, _height);
                _hist = new DenseHistogram(_intensityRange, new RangeF(0, _intensityRange - 1));

                return true;
            }
            catch (Exception ex)
            {
                _logger?.Error($"Error in ImagePreProcessor - InitEmguImages. {ex.Message}");
                return false;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private bool ClearEmguImages()
        {
            _thresholdedImage?.Dispose();

            _initialized = false;

            return true;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputHistogram"></param>
        /// <returns></returns>
        private float CalculateThresholdRegardingMoment(DenseHistogram inputHistogram)
        {
            float[] Hist;
            Hist = inputHistogram.GetBinValues();

            float[,] mean = new float[4096, 2];
            float[,] deviation = new float[4096, 2];
            float[,] num = new float[256, 2];
            float min = 100000;
            float minpos = 0;
            for (int nt = 5; nt < 256 - 5; nt++)
            {
                for (int n = 0; n < 256; n++)
                {
                    if (n < nt)
                    {
                        num[nt, 0] += Hist[n];
                        mean[nt, 0] += n * Hist[n];
                        deviation[nt, 0] += n * n * Hist[n];
                    }
                    else
                    {
                        num[nt, 1] += Hist[n];
                        mean[nt, 1] += n * Hist[n];
                        deviation[nt, 1] += n * n * Hist[n];
                    }
                }
                mean[nt, 0] = mean[nt, 0] / num[nt, 0];
                deviation[nt, 0] = (float)Math.Sqrt(deviation[nt, 0] / num[nt, 0] - mean[nt, 0] * mean[nt, 0]);
                mean[nt, 1] = mean[nt, 1] / num[nt, 1];
                deviation[nt, 1] = (float)Math.Sqrt(deviation[nt, 1] / num[nt, 1] - mean[nt, 1] * mean[nt, 1]);

                if (min > deviation[nt, 0] + deviation[nt, 1])
                {
                    min = deviation[nt, 0] + deviation[nt, 1];
                    minpos = nt;
                }
            }

            return minpos;
        }


    }




    public class Factory_ImagePreProcessor : IImagePreProcessor_Creator
    {
        public IImagePreProcessor Factory(ILogger logger, int intensityRange, int width, int height, bool showImages)
        {
            return new ImagePreProcessor(logger, intensityRange, width, height, showImages);
        }
    }

}
