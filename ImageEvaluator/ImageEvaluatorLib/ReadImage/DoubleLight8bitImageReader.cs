using NLog;
using System;
using System.Diagnostics;
using System.IO;
using ImageEvaluatorInterfaces;

namespace ImageEvaluatorLib.ReadImage
{
    internal class DoubleLight8BitImageReader : DoubleLightImageReader_Base
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="width"></param>
        /// <param name="showImages"></param>
        /// <param name="logger"></param>
        internal DoubleLight8BitImageReader(ILogger logger, int width, bool showImages)
            : base(logger, width, showImages)
        {
            _bitNumber = 1;

            _logger?.Info("DoubleLight8bitImageReader instantiated.");
        }



        /// <summary>
        /// 
        /// </summary>
        protected override bool ReadImage()
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

                ushort[,,] emguImage1Array = _img1.Data;
                ushort[,,] emguImage2Array = _img2.Data;

                for (int i = 0; i < _height; i++)
                {
                    Buffer.BlockCopy(inputImage, 2 * i * stride, dataRow1, 0, stride);
                    Buffer.BlockCopy(inputImage, 2 * i * stride + stride, dataRow2, 0, stride);

                    for (int j = 0; j < _width; j++)
                    {
                        emguImage1Array[i, j, 0] = dataRow1[j];
                        emguImage2Array[i, j, 0] = dataRow2[j];
                    }

                }

                Console.WriteLine($"       DoubleLight8bitImageReader - Image conversion time to emgu-image: {sw.ElapsedMilliseconds}ms.");
            }
            catch (Exception ex)
            {
                _logger.Error($"Exception during file read (8 bit double light): {(string.IsNullOrEmpty(_fileName) ? string.Empty : _fileName)}. {ex}");
                return false;
            }

            return true;
        }

    }



    /// <summary>
    /// 
    /// </summary>
    public class FactoryDoubleLight8BitImageReader : IDoubleLightImageReader_Creator
    {
        public IImageReader Factory(ILogger logger, int width, bool showImages)
        {
            logger?.Info($"{typeof(FactoryDoubleLight8BitImageReader)} factory called.");
            return new DoubleLight8BitImageReader(logger, width, showImages);
        }
    }



}
