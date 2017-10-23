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
        internal DoubleLight8BitImageReader(ILogger logger, int width, int height, bool showImages)
            : base(logger, width, height, showImages)
        {
            _bitNumber = 1;

            _logger?.Info($"{this.GetType().Name} instantiated.");
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

                Console.WriteLine($"{this.GetType().Name} - Image reading time: {sw.ElapsedMilliseconds}ms.");
                sw.Restart();

                byte[] dataRow1 = new byte[stride];
                byte[] dataRow2 = new byte[stride];

                // make separate emgu images:

                byte[,,] emguImage1Array = _rawImages[0].Data;
                byte[,,] emguImage2Array = _rawImages[1].Data;

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

                Console.WriteLine($"{this.GetType().Name} - Image conversion time to emgu-image: {sw.ElapsedMilliseconds}ms.");
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
    public class FactoryDoubleLight8BitImageReader : IImageReader_Creator
    {
        public IImageReader Factory(ILogger logger, int width, int height, bool showImages)
        {
            logger?.Info($"{typeof(FactoryDoubleLight8BitImageReader)} factory called.");
            return new DoubleLight8BitImageReader(logger, width, height, showImages);
        }
    }



}
