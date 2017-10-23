using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.UI;
using ImageEvaluatorInterfaces;
using ImageEvaluatorInterfaces.BaseClasses;
using NLog;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace ImageEvaluatorLib.ReadImage
{
    internal abstract class ImageReaderBase : IImageReader
    {
        protected string _fileName;
        protected int _width;
        protected int _height;
        protected int _bitNumber;
        protected ILogger _logger;
        protected bool _showImages;
        protected int _imageNum;
        protected Image<Gray, byte>[] _rawImages;
        protected Rectangle _fullROI;



        protected ImageReaderBase(int width, int height, ILogger logger, bool showImages, int imageNum)
        {
            _width = width;
            _height = height;
            _logger = logger;
            _showImages = showImages;
            _imageNum = imageNum;
        }

        public bool IsInitialized { get; protected set; }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected virtual bool CheckWidthData()
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
        public virtual bool Init()
        {
            IsInitialized = (CheckWidthData() && InitEmguImages());

            _logger?.Info($"{this.GetType().Name} " + (IsInitialized ? string.Empty : "NOT") + " initialized.");

            return IsInitialized;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputfileName"></param>
        /// <returns></returns>
        protected virtual bool CheckFile(string inputfileName)
        {
            try
            {
                if (!File.Exists(inputfileName))
                    return false;

                FileInfo fi = new FileInfo(inputfileName);
                if (fi.Length != (_width * _height * _imageNum * _bitNumber))
                    return false;
            }
            catch (Exception ex)
            {
                _logger?.Error($"Exception in DoubleLightImageReader_Base-CheckFileName: {ex}");
                return false;
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
            {
                return true;
            }

            try
            {
                _rawImages = new Image<Gray, byte>[_imageNum];
                for (int i = 0; i < _imageNum; i++)
                {
                    _rawImages[0] = new Image<Gray, byte>(_width, _height);
                }

                _fullROI = new Rectangle(0, 0, _width, _height);

                return true;
            }
            catch (Exception ex)
            {
                _logger?.Error($"DoubleLightImageReader_Base - InitEmguImages. Error during emgu initialization. {ex}");
                return false;
            }

        }

        protected virtual bool ResetImageROI()
        {
            if (IsInitialized)
            {

                for (int i = 0; i < _imageNum; i++)
                {
                    if (_rawImages[i].IsROISet)
                    {
                        _rawImages[i].ROI = _fullROI;
                    }
                }

                _logger.Info("Image ROIs were reseted.");
                return true;
            }

            _logger.Info("Image ROIs could be reseted because Reader is not initialized yet");
            return false;
        }


        /// <summary>
        /// 
        /// </summary>
        protected virtual bool ClearEmguImages()
        {
            for (int i = 0; i < _imageNum; i++)
            {
                _rawImages[i]?.Dispose();
            }

            return true;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputfileName"></param>
        /// <param name="img1"></param>
        /// <param name="img2"></param>
        /// <returns></returns>
        public virtual bool GetImage(string inputfileName, List<NamedData> data)
        {
            if (!IsInitialized)
            {
                _logger?.Trace("DoubleLightImageReader is not initialized yet.");
                return false;
            }

            _logger?.Trace($"{this.GetType().Name} GetImage. inputFileName: {inputfileName}");

            _fileName = inputfileName;

            if (!CheckFile(inputfileName))
            {
                _logger?.Error("The file name is invalid. It does not exists or the width, height are invalid.");
                return false;
            }

            try
            {
                ResetImageROI();

                data.Add(new EmguByteNamedData(_rawImages, "Contains the raw input images", nameof(_rawImages)));

            }
            catch (Exception ex)
            {
                _logger?.Error($"Error during image handling: {ex}");
                return false;
            }

            try
            {
                ReadImage();

                if (_showImages)
                {
                    for (int i = 0; i < _imageNum; i++)
                    {
                        ImageViewer.Show(_rawImages[i], $"{this.GetType().Name} - input image 1");
                    }
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



        protected abstract bool ReadImage();

    }
}
