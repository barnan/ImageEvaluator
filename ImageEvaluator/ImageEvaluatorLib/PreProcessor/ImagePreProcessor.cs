﻿using System;
using System.Drawing;
using System.IO;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using NLog;
using Emgu.CV.UI;
using ImageEvaluatorInterfaces;
using ImageEvaluatorLib.ThresholdCalculator;
using ImageEvaluatorInterfaces.BaseClasses;
using System.Collections.Generic;
using ImageEvaluatorLib.BaseClasses;

namespace ImageEvaluatorLib.PreProcessor
{
    class ImagePreProcessor : NamedDataProvider, IImagePreProcessor
    {
        private IHistogramThresholdCalculator _thresholdcalculator;
        private DenseHistogram _hist;
        private int _intensityRange;
        Image<Gray, byte> _thresholdedImage;
        Image<Gray, byte> _tempimage;
        Image<Gray, byte> _dilatedImage;
        private Matrix<byte> _reducedMask;
        protected bool _showImages;
        private int _width;
        private int _height;
        private ILogger _logger;
        private BeltCoordinates _beltCoordinates;

        public string ClassName { get; protected set; }
        public string Title { get; protected set; }
        public bool IsInitialized { get; protected set; }


        internal ImagePreProcessor(ILogger logger, int intensityRange, int width, int height, IHistogramThresholdCalculator histcalculator, bool showImages, BeltCoordinates beltcoords)
        {
            ClassName = nameof(ImagePreProcessor);
            Title = ClassName;

            _intensityRange = intensityRange;
            _showImages = showImages;
            _width = width;
            _height = height;
            _logger = logger;
            _thresholdcalculator = histcalculator;
            _beltCoordinates = beltcoords;
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

            _logger?.InfoLog((IsInitialized ? string.Empty : "NOT") + " initialized.", ClassName);

            return IsInitialized;
        }


        /// <summary>
        /// Makes image transpose and creates mask image
        /// </summary>
        /// <param name="inputImage"></param>
        /// <param name="maskImage"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool Execute(List<NamedData> data, string name)
        {
            try
            {
                Image<Gray, ushort>[] inputImages = GetEmguUShortImages("RawImages", data);
                int imageCounter = inputImages?.Length ?? 0;

                if (imageCounter == 0)
                {
                    _logger?.InfoLog("No raw image was found dynamicresult!", ClassName);
                    return false;
                }

                Image<Gray, byte>[] maskImages = new Image<Gray, byte>[imageCounter];

                for (int m = 0; m < imageCounter; m++)
                {

                    CvInvoke.Transpose(inputImages[m], inputImages[m]);

                    // calculate historamm for binarythreshold
                    _hist.Calculate<ushort>(new[] { inputImages[0] }, false, null);

                    float thresh;
                    _thresholdcalculator.Execute(_hist, out thresh);


                    if (_showImages)
                    {
                        GeneralImageHandling.SaveHistogram(name, "PreProcessor", "Histogram", _hist, _logger);
                    }

                    double maskValue = 255.0;
                    if (thresh > 255)
                        thresh /= 16;

                    // tempimage for thresholding:
                    _tempimage = inputImages[m].Convert<Gray, byte>();
                    CvInvoke.Threshold(_tempimage, _thresholdedImage, thresh, maskValue, ThresholdType.Binary);
                    _tempimage?.Dispose();

                    CvInvoke.Erode(_thresholdedImage, _dilatedImage, null, new Point(-1, -1), 3, BorderType.Default, new MCvScalar(0));
                    CvInvoke.Dilate(_dilatedImage, _thresholdedImage, null, new Point(-1, -1), 4, BorderType.Default, new MCvScalar(0));

                    maskImages[m] = _thresholdedImage;

                    maskImages[m].Reduce(_reducedMask, ReduceDimension.SingleCol, ReduceType.ReduceAvg);

                    // this section tries to mask out the belts:
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
                    if (count < maskImages[m].Height * (1 - MagicThreshold2))
                    {
                        Rectangle fullRoi = new Rectangle(0, 0, maskImages[m].Width, maskImages[m].Height);
                        Rectangle rectLeft = new Rectangle(0, _beltCoordinates.LeftBeltStart, maskImages[m].Width, _beltCoordinates.LeftBeltEnd - _beltCoordinates.LeftBeltStart);
                        Rectangle rectRight = new Rectangle(0, _beltCoordinates.RightBeltStart, maskImages[m].Width, _beltCoordinates.RightBeltEnd - _beltCoordinates.RightBeltStart);

                        maskImages[m].ROI = rectLeft;
                        maskImages[m].SetValue(0.0);

                        maskImages[m].ROI = rectRight;
                        maskImages[m].SetValue(0.0);

                        maskImages[m].ROI = fullRoi; // new Rectangle(0, 0, maskImage.Width, maskImage.Height);
                    }


                    if (_showImages)
                    {
                        ImageViewer.Show(inputImages[m], "ImagePreProcessor - transposed image");
                        ImageViewer.Show(maskImages[m], "ImagePreProcessor - maskImage");

                        //SaveMaskImage(name, maskImages[m], "MaskImage");
                        GeneralImageHandling.SaveImage(name, "ImagePreProcessor", "MaskImage", maskImages[m], _logger);
                    }
                }

                data.Add(new EmguByteNamedData(maskImages, "Maskimages", "MaskImages"));

                return true;
            }
            catch (Exception ex)
            {
                _logger?.ErrorLog($"Exception occured: {ex}", ClassName);
                return false;
            }
        }





        private bool CheckWidthData()
        {
            if (_width > 10000 || _width < 0)
            {
                _logger?.ErrorLog($"Image width is not proper: {_width}", ClassName);
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
                _tempimage = new Image<Gray, byte>(_width, _height);
                _dilatedImage = new Image<Gray, byte>(_width, _height);
                _reducedMask = new Matrix<byte>(_height, 1);
                _hist = new DenseHistogram(_intensityRange, new RangeF(0, _intensityRange - 1));

                return true;
            }
            catch (Exception ex)
            {
                _logger?.ErrorLog($"Exception occured: {ex}", ClassName);
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
            _tempimage?.Dispose();
            _dilatedImage?.Dispose();
            _reducedMask?.Dispose();

            IsInitialized = false;

            return true;
        }
    }



    public class FactoryImagePreProcessor : IImagePreProcessorCreator
    {
        public IImagePreProcessor Factory(ILogger logger, int intensityRange, int width, int height, IHistogramThresholdCalculator histcalculator, bool showImages, BeltCoordinates beltcoords)
        {
            logger?.InfoLog("Factory called.", nameof(FactoryImagePreProcessor));

            return new ImagePreProcessor(logger, intensityRange, width, height, histcalculator, showImages, beltcoords);
        }
    }

}
