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
    internal abstract class BorderSearcherBase : NamedDataProvider, IBorderSearcher
    {

        protected int[,] _borderPoints;
        protected int _borderSkipSize;
        protected bool _showImages;
        protected int _imageHeight;
        protected int _imageWidth;
        //private Image<Gray, byte> _maskImage;
        protected ILogger _logger;


        protected BorderSearcherBase(ILogger logger, int imageWidth, int imageHeight, int border)
        {
            _imageWidth = imageWidth;
            _imageHeight = imageHeight;
            _logger = logger;
            _borderSkipSize = border;
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
                _logger?.Error("BorderSearch is not initialized yet.");
                return false;
            }

            try
            {
                Image<Gray, byte>[] rawImages = GetEmguByteImages("RawImages", data);
                int imageCounterRaw = rawImages?.Length ?? 0;

                Image<Gray, byte>[] maskImages = GetEmguByteImages("MaskImages", data);
                int imageCounterMask = maskImages?.Length ?? 0;

                if (imageCounterMask != imageCounterRaw)
                {
                    _logger.Info($"{this.GetType()} input and mask image number is not the same!");
                    return false;
                }

                int[][,] borderPointArrayList = new int[imageCounterRaw][,];

                for (int m = 0; m < imageCounterRaw; m++)
                {
                    if (!CheckInputImage(maskImages[m]))
                    {
                        _logger?.Info("GetBorderPoints - Invalid input mask image.");
                        continue;
                    }

                    if (!(AllocArrays() && ResetPointList()))
                    {
                        _logger.Info($"{this.GetType().Name} array re-allocation failed.");
                        continue;
                    }

                    CalculatePoints(rawImages[m], maskImages[m], name);

                    borderPointArrayList[m] = _borderPoints;
                }

                data.Add(new NamedData<BorderPointArrays>(new BorderPointArrays(borderPointArrayList), "borderPointArrayList", "contains the found border points"));
            }
            catch (Exception ex)
            {
                _logger?.Error($"Exception during border points calculation: {ex}");
                return false;
            }

            return true;
        }



        protected abstract bool CalculatePoints(Image<Gray, byte> origImage, Image<Gray, byte> maskImage, string name);


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


        protected bool CheckInputImage(Image<Gray, byte> maskImage)
        {
            if (maskImage == null || maskImage.Height != _imageHeight || maskImage.Width != _imageHeight)
            {
                return false;
            }
            return true;
        }


        protected void SaveMaskImage(string name, Image<Gray, byte> maskImage, string ext)
        {
            string fileNameBase = Path.GetFileNameWithoutExtension(name);
            string path = Path.GetDirectoryName(name);
            string finalOutputName = Path.Combine(path ?? string.Empty, ext + "_BorderSearch", $"{fileNameBase}.png");

            string directory = Path.GetDirectoryName(finalOutputName);
            if (directory != null && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            maskImage.Save(finalOutputName);
        }



        protected void SavePointList(string name)
        {
            string fileNameBase = Path.GetFileNameWithoutExtension(name);
            string path = Path.GetDirectoryName(name);
            string finalOutputName = Path.Combine(path ?? string.Empty, "PointList", $"{fileNameBase}.csv");

            string directory = Path.GetDirectoryName(finalOutputName);
            if (directory != null && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
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
