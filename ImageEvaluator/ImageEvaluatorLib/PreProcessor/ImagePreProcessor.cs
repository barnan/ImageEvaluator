using System;
using Emgu.CV;
using Emgu.CV.Structure;
using NLog;
using Emgu.CV.UI;
using ImageEvaluatorInterfaces;
using ImageEvaluatorLib.ThresholdCalculator;

namespace ImageEvaluatorLib.PreProcessor
{
    class ImagePreProcessor : IImagePreProcessor
    {
        private IHistogramThresholdCalculator _thresholdcalculator;
        private DenseHistogram _hist;
        private int _intensityRange;
        Image<Gray, float> _thresholdedImage;
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
        /// <param name="showImages"></param>
        internal ImagePreProcessor(ILogger logger, int intensityRange, int width, int height, IHistogramThresholdCalculator histcalculator, bool showImages)
        {
            _intensityRange = intensityRange;
            _showImages = showImages;
            _width = width;
            _height = height;
            _logger = logger;
            _thresholdcalculator = histcalculator;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public virtual bool Init()
        {
            IsInitialized = (CheckWidthData() && InitEmguImages());

            _logger?.Info("ImagePreProcessor " + (IsInitialized ? string.Empty : "NOT") + " initialized.");

            return IsInitialized;
        }

        public bool IsInitialized { get; protected set; }


        /// <summary>
        /// Makes image transpose and creates mask image
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

                float thresh;
                _thresholdcalculator.Run(_hist, out thresh);

                // create mask image:
                double maskValue = 255.0;
                _thresholdedImage = inputImage.ThresholdBinary(new Gray(thresh), new Gray(maskValue));
                maskImage = _thresholdedImage.Convert<Gray, byte>();

                if (_showImages)
                {
                    ImageViewer.Show(inputImage, "ImagePreProcessor - input image");
                    ImageViewer.Show(maskImage, "ImagePreProcessor - maskImage");
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger?.Error($"Error in ImagePreProcessor - InitEmguImages. {ex}");
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
        /// <returns></returns>
        private bool InitEmguImages()
        {
            if (IsInitialized)
                return true;

            try
            {
                _thresholdedImage = new Image<Gray, float>(_width, _height);
                _hist = new DenseHistogram(_intensityRange, new RangeF(0, _intensityRange - 1));

                return true;
            }
            catch (Exception ex)
            {
                _logger?.Error($"Error in ImagePreProcessor - InitEmguImages. {ex}");
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

            IsInitialized = false;

            return true;
        }
    }


    public class FactoryImagePreProcessor : IImagePreProcessorCreator
    {
        public IImagePreProcessor Factory(ILogger logger, int intensityRange, int width, int height, IHistogramThresholdCalculator histcalculator, bool showImages)
        {
            logger?.Info($"{typeof(FactoryImagePreProcessor)} factory called.");
            return new ImagePreProcessor(logger, intensityRange, width, height, histcalculator, showImages);
        }
    }

}
