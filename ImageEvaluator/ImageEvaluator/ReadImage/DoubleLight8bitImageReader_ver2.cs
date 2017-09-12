﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageEvaluator.Interfaces;
using ImageEvaluator.ReadImage;
using NLog;

namespace ImageEvaluator.ReadImage
{
    class DoubleLight8bitImageReader_ver2 : DoubleLightImageReader_Base
    {
        public DoubleLight8bitImageReader_ver2(ILogger logger, int width, int bitdepth, bool showImages)
            : base(logger, width, showImages)
        {
            _bitNumber = bitdepth;
            _logger?.Info("DoubleLight8bitImageReader instantiated.");
        }


        protected override bool ReadDoubleLightImage()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();

            try
            {
                byte[] inputImage = File.ReadAllBytes(_fileName);
                int wid = _width;

                Console.WriteLine($"       DoubleLight8bitImageReader_ver2 - Image reading time: {sw.ElapsedMilliseconds}ms.");
                sw.Restart();

                byte[] image1 = new byte[inputImage.Length / 2];
                byte[] image2 = new byte[inputImage.Length / 2];

                // make separate emgu images:

                for (int i = 0; i < _height; i++)
                {
                    Buffer.BlockCopy(inputImage, 2 * i * wid, image1, i * wid, wid);
                    Buffer.BlockCopy(inputImage, 2 * i * wid + wid, image2, 0, wid);
                }

                _img1.Bytes = image1;
                _img2.Bytes = image2;

                Console.WriteLine($"       DoubleLight8bitImageReader_ver2 - Image convertsion time to emgu-image: {sw.ElapsedMilliseconds}ms.");

            }
            catch (Exception ex)
            {
                _logger.Error($"Exception during file read (8 bit double light version 2): {(string.IsNullOrEmpty(_fileName) ? string.Empty : _fileName)}. {ex.Message}");
                return false;
            }

            return true;
        }
    }


    /// <summary>
    /// 
    /// </summary>
    public class Factory_DoubleLight8bitImageReader_ver2 : IDoubleLightImageReader_Creator
    {
        public IDoubleLightImageReader Factory(ILogger logger, int width, int bitDepth, bool showImages)
        {
            return new DoubleLight8bitImageReader_ver2(logger, width, bitDepth, showImages);
        }
    }


}
