using System;
using System.IO;
using Emgu.CV;
using Emgu.CV.Structure;
using ImageEvaluatorInterfaces;
using NLog;
using System.Collections.Generic;
using ImageEvaluatorInterfaces.BaseClasses;
using ImageEvaluatorLib.BaseClasses;
using System.Globalization;
using System.Drawing;

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
        protected static string _ownFolderNameForSaving = "BorderSearch";

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

            _logger?.Info((IsInitialized ? string.Empty : " NOT") + " initialized.", ClassName);

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
                string[] enumNames = Enum.GetNames(typeof(DoubleNamingConvention));
                string additionalExtensionName = string.Empty;

                Image<Gray, ushort>[] rawImages = GetEmguUShortImages("RawImages", data);
                int imageCounterRaw = rawImages?.Length ?? 0;

                Image<Gray, byte>[] maskImages = GetEmguByteImages("MaskImages", data);
                int imageCounterMask = maskImages?.Length ?? 0;

                if (imageCounterMask != imageCounterRaw || imageCounterRaw > (enumNames?.Length ?? 0))
                {
                    _logger?.InfoLog("Input and mask image number OR enumNames number is not the same!", ClassName);
                    return false;
                }

                int[][,] borderPointArrayLists = new int[imageCounterRaw][,];

                for (int m = 0; m < imageCounterRaw; m++)
                {
                    if (!(GeneralImageHandling.CheckImages(rawImages[m], maskImages[m], _imageWidth, _imageHeight, _logger)))
                    {
                        continue;
                    }

                    if (!(AllocArrays() && ResetPointList()))
                    {
                        _logger?.InfoLog("Borderpoints array re-allocation failed.", ClassName);
                        continue;
                    }

                    if (enumNames.Length == imageCounterRaw)
                    {
                        additionalExtensionName = "_" + enumNames[m];
                    }

                    CalculatePoints(rawImages[m], maskImages[m], name, additionalExtensionName);

                    borderPointArrayLists[m] = _borderPoints;
                }

                data.Add(new BorderPointArraysNamedData(new BorderPointArrays(borderPointArrayLists), "contains the found border points", "BorderPointArrayList"));

                if (_showImages)
                {
                    SavePointLists(borderPointArrayLists, name, "PointList");
                }
            }
            catch (Exception ex)
            {
                _logger?.ErrorLog($"Exception during border points calculation: {ex}", ClassName);
                return false;
            }

            return true;
        }


        protected abstract bool CalculatePoints(Image<Gray, ushort> origImage, Image<Gray, byte> maskImage, string name, string enumName);


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


        protected bool SavePointLists(int[][,] pointLists, string name, string ext)
        {
            try
            {
                string[] enumNames = Enum.GetNames(typeof(DoubleNamingConvention));
                string additionalExtension = string.Empty;
                CultureInfo cultInfo = CultureInfo.InvariantCulture;

                // check:
                if (name == null)
                {
                    return false;
                }
                if (enumNames.Length < pointLists.Length)
                {
                    _logger?.ErrorLog($"Error during poinlist saving. The number of pointlists is longer the possible enum names", ClassName);
                    return false;
                }


                for (int m = 0; m < pointLists.Length; m++)
                {
                    if (enumNames.Length == pointLists.Length)
                    {
                        additionalExtension = "_" + enumNames[m];
                    }

                    string finalOutputName = GeneralImageHandling.CheckOutputDirectoryOfImageSaving(name, "BorderSearch", ext + additionalExtension, ".csv");

                    using (StreamWriter sw = new StreamWriter(finalOutputName))
                    {
                        for (int i = 0; i < _borderPoints.Length / 2; i++)
                        {
                            sw.WriteLine($"{i},{_borderPoints[i, 0].ToString(cultInfo)},{_borderPoints[i, 1].ToString(cultInfo)}");
                        }
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger?.ErrorLog($"Exception occured: {ex}", ClassName);
                return false;
            }
        }


    }
}
