using NLog;
using System;
using System.Diagnostics;
using System.IO;
using ImageEvaluatorInterfaces;

namespace ImageEvaluatorLib.ReadImage
{
    internal class DoubleLight16BitImageReader : DoubleLightImageReader_Base
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="width"></param>
        /// <param name="showImages"></param>
        internal DoubleLight16BitImageReader(ILogger logger, int width, int height, bool showImages)
            : base(logger, width, height, showImages)
        {
            _bitNumber = 2;

            _logger?.Info($"{this.GetType().Name} instantiated.");
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override bool ReadImage()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();

            try
            {
                byte[] inputImage = File.ReadAllBytes(_fileName);
                int stride = _width * 2;

                Console.WriteLine($"{this.GetType().Name} - Image reading time: {sw.ElapsedMilliseconds}ms.");
                sw.Restart();

                byte[] dataRow1 = new byte[stride];
                byte[] dataRow2 = new byte[stride];

                // make separate emgu images:

                // to speed up:
                byte[,,] emguImage1Array = _rawImages[0].Data;
                byte[,,] emguImage2Array = _rawImages[1].Data;

                for (int i = 0; i < _height; i++)
                {
                    Buffer.BlockCopy(inputImage, 2 * i * stride, dataRow1, 0, stride);
                    Buffer.BlockCopy(inputImage, 2 * i * stride + stride, dataRow2, 0, stride);

                    for (int j = 0; j < _width; j++)
                    {
                        emguImage1Array[i, j, 0] = (byte)(dataRow1[2 * j] + (dataRow1[2 * j + 1] << 8));
                        emguImage2Array[i, j, 0] = (byte)(dataRow2[2 * j] + (dataRow2[2 * j + 1] << 8));
                    }
                }

                Console.WriteLine($"{this.GetType().Name} - Image conversion time to emgu-image: {sw.ElapsedMilliseconds}ms.");
            }
            catch (Exception ex)
            {
                _logger.Error($"Exception during file read (16 bit double light): {(string.IsNullOrEmpty(_fileName) ? string.Empty : _fileName)}. {ex}");
                return false;
            }

            return true;
        }


    }


    /// <summary>
    /// 
    /// </summary>
    public class Factory_DoubleLight16bitImageReader : IImageReader_Creator
    {
        public IImageReader Factory(ILogger logger, int width, int height, bool showImages)
        {
            logger?.Info($"{typeof(Factory_DoubleLight16bitImageReader)} factory called.");
            return new DoubleLight16BitImageReader(logger, width, height, showImages);
        }
    }


}
