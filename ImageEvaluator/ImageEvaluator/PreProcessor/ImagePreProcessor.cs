using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV;
using Emgu.CV.Structure;
using NLog;

namespace ImageEvaluator.PreProcessor
{
    class ImagePreProcessor : IImagePreProcessor
    {
        DenseHistogram _hist;
        int _intensityRange;
        Image<Gray, byte> _maskImage;
        Image<Gray, float> _rotatedImage;
        bool _emguInitialized;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="intensityRange"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        internal ImagePreProcessor(ILogger logger, int intensityRange, int width, int height)
        {
            _intensityRange = intensityRange;

            InitEmguImages(width, height);
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
                _hist.Calculate<float>(new[] { _rotatedImage }, false, null);

                float thresh = CalculateThresholdRegardingMoment(_hist);

                // create mask image:
                double maskValue = 255.0;
                var thresholdedImage1 = _rotatedImage.ThresholdBinary(new Gray(thresh), new Gray(maskValue));

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in ImagePreProcessor - InitEmguImages. {ex.Message}");
                return false;
            }
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


        /// <summary>
        /// 
        /// </summary>
        /// <param name="intensityRange"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        private bool InitEmguImages(int width, int height)
        {
            if (_emguInitialized)
                return true;

            try
            {
                _maskImage = new Image<Gray, byte>(width, height);
                _rotatedImage = new Image<Gray, float>(width, height);
                _hist = new DenseHistogram(_intensityRange, new RangeF(0, _intensityRange - 1));

                return _emguInitialized = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in ImagePreProcessor - InitEmguImages. {ex.Message}");
                return _emguInitialized = false;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private bool ClearEmguImages()
        {
            _maskImage?.Dispose();
            _rotatedImage?.Dispose();

            _emguInitialized = false;

            return true;
        }
    }




    class Factory_ImagePreProcessor : IImagePreProcessor_Creator
    {
        public IImagePreProcessor Factory(ILogger logger, int intensityRange, int width, int height)
        {
            return new ImagePreProcessor(logger, intensityRange, width, height);
        }
    }

}
