using NLog;
using System;
using System.Diagnostics;
using System.IO;
using ImageEvaluator.Interfaces;

namespace ImageEvaluator.ReadImage
{
    class DoubleLight8bitImageReader : DoubleLightImageReader_Base
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="width"></param>
        /// <param name="bitdepth"></param>
        internal DoubleLight8bitImageReader(ILogger logger, int width, int bitdepth, bool showImages)
            : base(logger, width, showImages)
        {
            _bitNumber = bitdepth;

            _logger?.Info("DoubleLight8bitImageReader instantiated.");
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputFileName"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        protected override bool ReadDoubleLightImage()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();

            try
            {
                byte[] inputImage = File.ReadAllBytes(_fileName);
                int stride = _width;

                Console.WriteLine($"       DoubleLight8bitImageReader - Image reading time: {sw.ElapsedMilliseconds}ms.");
                sw.Restart();

                byte[] dataRow1 = new byte[stride];
                byte[] dataRow2 = new byte[stride];

                // make separate emgu images:

                float[,,] emguImage1_Array = _img1.Data;
                float[,,] emguImage2_Array = _img2.Data;

                for (int i = 0; i < _height; i++)
                {
                    Buffer.BlockCopy(inputImage, 2 * i * stride, dataRow1, 0, stride);
                    Buffer.BlockCopy(inputImage, 2 * i * stride + stride, dataRow2, 0, stride);

                    for (int j = 0; j < _width; j++)
                    {
                        emguImage1_Array[i, j, 0] = dataRow1[j];
                        emguImage2_Array[i, j, 0] = dataRow2[j];
                    }

                }

                Console.WriteLine($"       DoubleLight8bitImageReader - Image convertsion time to emgu-image: {sw.ElapsedMilliseconds}ms.");
            }
            catch (Exception ex)
            {
                _logger.Error($"Exception during file read (8 bit double light): {(string.IsNullOrEmpty(_fileName) ? string.Empty : _fileName)}. {ex.Message}");
                return false;
            }

            return true;
        }

    }



    /// <summary>
    /// 
    /// </summary>
    public class Factory_DoubleLight8bitImageReader : IDoubleLightImageReader_Creator
    {
        public IDoubleLightImageReader Factory(ILogger logger, int width, int bitDepth, bool showImages)
        {
            return new DoubleLight8bitImageReader(logger, width, bitDepth, showImages);
        }
    }



}
