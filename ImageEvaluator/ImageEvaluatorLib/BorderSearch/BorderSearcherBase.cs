using System;
using System.IO;
using Emgu.CV;
using Emgu.CV.Structure;
using ImageEvaluatorInterfaces;
using NLog;
using System.Collections.Generic;
using ImageEvaluatorInterfaces.BaseClasses;
using ImageEvaluatorLib.BaseClasses;

namespace ImageEvaluatorLib.BorderSearch
{
    internal abstract class BorderSearcherBase : NamedDataProvider, IBorderSearcher, IElement
    {

        protected int[,] _borderPoints;
        protected int _borderSkipSize;
        protected bool _showImages;
        protected int _imageHeight;
        protected int _imageWidth;
        //private Image<Gray, byte> _maskImage;
        protected ILogger _logger;

        public string ClassName { get; protected set; }
        public string Title { get; protected set; }


        protected BorderSearcherBase(ILogger logger, int imageWidth, int imageHeight, int border)
        {
            _imageWidth = imageWidth;
            _imageHeight = imageHeight;
            _logger = logger;
            _borderSkipSize = border;

            ClassName = nameof(BorderSearcherBase);
            Title = ClassName;
        }


        public bool IsInitialized { get; protected set; }

        public bool Init()
        {
            AllocArrays();
            ResetPointList();

            _logger?.Info($"{typeof(BorderSearcherBase)}" + (IsInitialized ? string.Empty : " NOT") + " initialized.");

            return IsInitialized = true;
        }


        public bool Execute(List<NamedData> data, string name)
        {
            if (!IsInitialized)
            {
                _logger?.ErrorLog("Not initialized yet.", ClassName);
                return false;
            }

            try
            {
                Image<Gray, ushort>[] rawImages = GetEmguUShortImages("RawImages", data);
                int imageCounterRaw = rawImages?.Length ?? 0;

                Image<Gray, byte>[] maskImages = GetEmguByteImages("MaskImages", data);
                int imageCounterMask = maskImages?.Length ?? 0;

                if (imageCounterMask != imageCounterRaw)
                {
                    _logger?.InfoLog("Input and mask image number is not the same!", ClassName);
                    return false;
                }

                int[][,] borderPointArrayList = new int[imageCounterRaw][,];

                for (int m = 0; m < imageCounterRaw; m++)
                {
                    if (!CheckInputImage(maskImages[m]))
                    {
                        _logger?.InfoLog("Invalid input mask image.", ClassName);
                        continue;
                    }

                    if (!(AllocArrays() && ResetPointList()))
                    {
                        _logger?.InfoLog("Array re-allocation failed.", ClassName);
                        continue;
                    }

                    CalculatePoints(rawImages[m], maskImages[m], name);

                    borderPointArrayList[m] = _borderPoints;
                }

                data.Add(new NamedData<BorderPointArrays>(new BorderPointArrays(borderPointArrayList), "borderPointArrayList", "contains the found border points"));
            }
            catch (Exception ex)
            {
                _logger?.ErrorLog($"Exception during border points calculation: {ex}", ClassName);
                return false;
            }

            return true;
        }



        protected abstract bool CalculatePoints(Image<Gray, ushort> origImage, Image<Gray, byte> maskImage, string name);


        protected bool AllocArrays()
        {
            _borderPoints = new int[_imageHeight, 2];

            return true;
        }



        protected bool ResetPointList()
        {
            if (!IsInitialized)
                return false;

            for (int i = 0; i < _borderPoints.Length / 2; i++)
            {
                _borderPoints[i, 0] = 0;
                _borderPoints[i, 1] = _imageWidth;
            }

            return true;
        }


        protected bool CheckInputImage(Image<Gray, ushort> image)
        {
            if (image == null || image.Height != _imageHeight || image.Width != _imageHeight)
            {
                return false;
            }
            return true;
        }


        protected bool CheckInputImage(Image<Gray, byte> image)
        {
            if (image == null || image.Height != _imageHeight || image.Width != _imageHeight)
            {
                return false;
            }
            return true;
        }

        protected void SaveMaskImage(string name, Image<Gray, byte> image, string ext)
        {
            string finalOutputName = CheckOutputDirectoryOfImageSaving(name, ext, ".png");

            if (finalOutputName != null)
            {
                image.Save(finalOutputName);
            }
        }


        protected void SaveMaskImage(string name, Image<Gray, ushort> image, string ext)
        {
            string finalOutputName = CheckOutputDirectoryOfImageSaving(name, ext, ".png");

            if (finalOutputName != null)
            {
                image.Save(finalOutputName);
            }
        }


        private string CheckOutputDirectoryOfImageSaving(string name, string ext, string extension)
        {
            try
            {
                string fileNameBase = Path.GetFileNameWithoutExtension(name);
                string path = Path.GetDirectoryName(name);
                string finalOutputName = Path.Combine(path ?? string.Empty, ext + "_BorderSearch", $"{fileNameBase}{extension}");

                string directory = Path.GetDirectoryName(finalOutputName);
                if (directory != null && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                return finalOutputName;
            }
            catch (Exception ex)
            {
                _logger?.ErrorLog($"Exception during directory determination: {ex}", ClassName);
                return null;
            }
        }


        protected void SavePointList(string name, string ext)
        {
            string finalOutputName = CheckOutputDirectoryOfImageSaving(name, ext, ".csv");

            if (finalOutputName == null)
            {
                return;
            }

            using (StreamWriter sw = new StreamWriter(finalOutputName))
            {
                for (int i = 0; i < _borderPoints.Length / 2; i++)
                {
                    sw.WriteLine($"{i},{_borderPoints[i, 0]},{_borderPoints[i, 1]}");
                }
            }
        }


    }
}
