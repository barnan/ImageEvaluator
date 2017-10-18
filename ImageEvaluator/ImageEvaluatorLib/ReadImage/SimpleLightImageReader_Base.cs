using System;
using System.Drawing;
using System.IO;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.UI;
using NLog;

namespace ImageEvaluatorLib.ReadImage
{
    internal abstract class SimpleLightImageReader_Base : ImageReaderBase
    {
        protected string _fileName;
        protected Image<Gray, byte> _img1;
        protected Rectangle _fullROI;


        protected SimpleLightImageReader_Base(ILogger logger, int width, bool showImages)
            : base(width, width, logger, showImages)
        {
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputfileName"></param>
        /// <param name="img1"></param>
        /// <param name="img2">it is not used</param>
        /// <returns></returns>
        public override bool GetImage(string inputfileName, ref Image<Gray, byte> img1, ref Image<Gray, byte> img2)
        {

            if (!IsInitialized)
            {
                _logger?.Trace("SimpleLightImageReader is not initialized yet.");
                return false;
            }

            _logger?.Trace($"SimpleLightImageReader_Base GetImage. inputFileName: {inputfileName}");

            _fileName = inputfileName;

            if (!CheckFileName(inputfileName))
            {
                _logger?.Error("The file name is invalid. It does not exists or the width, height are invalid.");
                return false;
            }

            try
            {
                ResetImageROI();

                img1 = _img1;
            }
            catch (Exception ex)
            {
                _logger?.Error($"Error during emgu image allocation: {ex}");
                return false;
            }

            try
            {
                ReadImage();

                if (_showImages)
                {
                    ImageViewer.Show(_img1, "SimpleLightImageReader_Base - input image 1");
                }

                _logger?.Trace($"{_fileName} readed.");

                return true;
            }
            catch (Exception ex)
            {
                ClearEmguImages();
                _logger?.Error($"Error during double light image reading. {ex}");
                return false;
            }

        }


        protected override bool ClearEmguImages()
        {
            _img1?.Dispose();

            return true;
        }


        protected override bool ResetImageROI()
        {
            if (IsInitialized)
            {
                if (_img1.IsROISet)
                    _img1.ROI = _fullROI;

                _logger.Info("Image ROIs were reseted.");
                return true;
            }

            _logger.Info("Image ROIs could be reseted because Reader is not initialized yet");
            return false;
        }


        protected override bool InitEmguImages()
        {
            if (IsInitialized)
            {
                return true;
            }

            try
            {
                _img1 = new Image<Gray, byte>(_width, _height);
                _fullROI = new Rectangle(0, 0, _width, _height);

                return true;
            }
            catch (Exception ex)
            {
                _logger?.Error($"SimpleLightImageReader_Base - InitEmguImages. Error during emgu initialization. {ex}");
                return false;
            }
        }


        protected override bool CheckWidthData()
        {
            if (_width > 10000 || _width < 0)
            {
                _logger?.Error($"Image width is not proper: {_width}");
                return false;
            }
            return true;
        }


        public override bool Init()
        {
            IsInitialized = (CheckWidthData() && InitEmguImages());

            _logger?.Info("SimpleLightImageReaderBase " + (IsInitialized ? string.Empty : "NOT") + " initialized.");

            return IsInitialized;
        }


        protected override bool CheckFileName(string inputfileName)
        {
            try
            {
                if (!File.Exists(inputfileName))
                    return false;

                FileInfo fi = new FileInfo(inputfileName);
                if (fi.Length != (_width * _height * _bitNumber))
                    return false;
            }
            catch (Exception ex)
            {
                _logger?.Error($"Exception in DoubleLightImageReader_Base-CheckFileName: {ex}");
                return false;
            }

            return true;
        }


    }
}