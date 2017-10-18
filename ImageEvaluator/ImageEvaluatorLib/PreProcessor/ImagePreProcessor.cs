using System;
using System.Drawing;
using System.IO;
using Emgu.CV;
using Emgu.CV.CvEnum;
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
        Image<Gray, byte> _thresholdedImage;
        Image<Gray, byte> _dilatedImage;
        private Matrix<byte> _reducedMask;
        protected bool _showImages;
        private int _width;
        private int _height;
        private ILogger _logger;
        private BeltCoordinates _beltCoordinates;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="intensityRange"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="histcalculator"></param>
        /// <param name="showImages"></param>
        /// <param name="beltLeftStart"></param>
        /// <param name="beltLeftEnd"></param>
        /// <param name="beltRightStart"></param>
        /// <param name="beltRightEnd"></param>
        internal ImagePreProcessor(ILogger logger, int intensityRange, int width, int height, 
                                    IHistogramThresholdCalculator histcalculator, 
                                    bool showImages, 
                                    int beltLeftStart,
                                    int beltLeftEnd,
                                    int beltRightStart,
                                    int beltRightEnd)
        {
            _intensityRange = intensityRange;
            _showImages = showImages;
            _width = width;
            _height = height;
            _logger = logger;
            _thresholdcalculator = histcalculator;
            _beltCoordinates = new BeltCoordinates
            {
                LeftStart = beltLeftStart,
                LeftEnd = beltLeftEnd,
                RightStart = beltRightStart,
                RightEnd = beltRightEnd
            };
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public virtual bool Init()
        {
            IsInitialized = CheckWidthData() 
                            && InitEmguImages() 
                            && _thresholdcalculator.Init();

            _logger?.Info("ImagePreProcessor " + (IsInitialized ? string.Empty : "NOT") + " initialized.");

            return IsInitialized;
        }

        public bool IsInitialized { get; protected set; }


        /// <summary>
        /// Makes image transpose and creates mask image
        /// </summary>
        /// <param name="inputImage"></param>
        /// <param name="maskImage"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool Run(Image<Gray, byte> inputImage, ref Image<Gray, byte> maskImage, string name)
        {
            try
            {
                CvInvoke.Transpose(inputImage, inputImage);

                // calculate historamm for binarythreshold
                _hist.Calculate<byte>(new[] {inputImage}, false, null);

                float thresh;
                _thresholdcalculator.Run(_hist, out thresh);

                if (_showImages)
                {
                    SaveHistogram(name);
                }

                // create mask image:
                double maskValue = 255.0;
                CvInvoke.Threshold(inputImage, _thresholdedImage, thresh, maskValue, ThresholdType.Binary);

                CvInvoke.Erode(_thresholdedImage, _dilatedImage, null, new Point(-1, -1), 3, BorderType.Default, new MCvScalar(0));
                CvInvoke.Dilate(_dilatedImage, _thresholdedImage, null, new Point(-1, -1), 4, BorderType.Default, new MCvScalar(0));

                maskImage = _thresholdedImage;

                maskImage.Reduce(_reducedMask, ReduceDimension.SingleCol, ReduceType.ReduceAvg);

                int count = 0;
                double MagicThreshold1 = 0.4;   // 40%
                double MagicThreshold2 = 0.18;   // 15%
                for (int i = 0; i < _reducedMask.Height; i++)
                {
                    if (_reducedMask[i, 0] > (1 - MagicThreshold1) * maskValue)
                    {
                        count++;
                    }
                }
                if (count < maskImage.Height * (1- MagicThreshold2))
                {
                    Rectangle fullRoi = new Rectangle(0, 0, maskImage.Width, maskImage.Height);
                    Rectangle rectLeft = new Rectangle(0, _beltCoordinates.LeftStart, maskImage.Width, _beltCoordinates.LeftEnd- _beltCoordinates.LeftStart);
                    Rectangle rectRight = new Rectangle(0, _beltCoordinates.RightStart, maskImage.Width, _beltCoordinates.RightEnd - _beltCoordinates.RightStart);

                    maskImage.ROI = rectLeft;
                    maskImage.SetValue(0.0);

                    maskImage.ROI = rectRight;
                    maskImage.SetValue(0.0);

                    maskImage.ROI = fullRoi; // new Rectangle(0, 0, maskImage.Width, maskImage.Height);
                }


                if (_showImages)
                {
                    ImageViewer.Show(inputImage, "ImagePreProcessor - transposed image");
                    ImageViewer.Show(maskImage, "ImagePreProcessor - maskImage");

                    SaveMaskImage(name, maskImage);
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger?.Error($"Error in ImagePreProcessor - InitEmguImages. {ex}");
                return false;
            }
        }

        private void SaveHistogram(string name)
        {
            string fileNameBase = Path.GetFileNameWithoutExtension(name);
            string path = Path.GetDirectoryName(name);
            string finalOutputName = Path.Combine(path ?? string.Empty, "Histogram", $"{fileNameBase}.csv");

            string directory = Path.GetDirectoryName(finalOutputName);
            if (directory != null && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            float[] Hist = _hist.GetBinValues();
            using (StreamWriter sw = new StreamWriter(finalOutputName))
            {
                for (int i = 0; i < Hist.Length; i++)
                {
                    sw.WriteLine($"{i},{Hist[i]}");
                }
            }
        }


        private void SaveMaskImage(string name, Image<Gray, byte> maskImage)
        {
            string fileNameBase = Path.GetFileNameWithoutExtension(name);
            string path = Path.GetDirectoryName(name);
            string finalOutputName = Path.Combine(path ?? string.Empty, "MaskImage_PreProcessor", $"{fileNameBase}.png");

            string directory = Path.GetDirectoryName(finalOutputName);
            if (directory != null && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            
            maskImage.Save(finalOutputName);
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
                _thresholdedImage = new Image<Gray, byte>(_width, _height);
                _dilatedImage = new Image<Gray, byte>(_width, _height);
                _reducedMask = new Matrix<byte>(_height, 1);
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
            _dilatedImage?.Dispose();
            _reducedMask?.Dispose();

            IsInitialized = false;

            return true;
        }
    }


    internal class BeltCoordinates
    {
        public int LeftStart { get; set; }
        public int LeftEnd { get; set; }
        public int RightStart { get; set; }
        public int RightEnd { get; set; }
    }

    public class FactoryImagePreProcessor : IImagePreProcessorCreator
    {
        public IImagePreProcessor Factory(ILogger logger, int intensityRange, int width, int height, 
                                            IHistogramThresholdCalculator histcalculator, 
                                            bool showImages,
                                            int beltLeftStart,
                                            int beltLeftEnd,
                                            int beltRightStart,
                                            int beltRightEnd)
        {
            logger?.Info($"{typeof(FactoryImagePreProcessor)} factory called.");
            return new ImagePreProcessor(logger, intensityRange, width, height, histcalculator, showImages, beltLeftStart, beltLeftEnd, beltRightStart, beltRightEnd);
        }
    }

}
