using NLog;
using System;
using System.Diagnostics;
using System.IO;
using ImageEvaluatorInterfaces;

namespace ImageEvaluatorLib.ReadImage
{
    class DoubleLight16bitImageReader : DoubleLightImageReader_Base
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="width"></param>
        /// <param name="bitnumber"></param>
        internal DoubleLight16bitImageReader(ILogger logger, int width, bool showImages)
            : base(logger, width, showImages)
        {
            _bitNumber = 2;

            _logger?.Info("DoubleLight16bitImageReader instantiated.");
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override bool ReadDoubleLightImage()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();

            try
            {
                byte[] inputImage = File.ReadAllBytes(_fileName);
                int stride = _width * 2;

                Console.WriteLine($"       DoubleLight16bitImageReader - Image reading time: {sw.ElapsedMilliseconds}ms.");
                sw.Restart();

                byte[] dataRow1 = new byte[stride];
                byte[] dataRow2 = new byte[stride];

                // make separate emgu images:

                // to speed up:
                float[,,] emguImage1_Array = _img1.Data;
                float[,,] emguImage2_Array = _img2.Data;

                for (int i = 0; i < _height; i++)
                {
                    Buffer.BlockCopy(inputImage, 2 * i * stride, dataRow1, 0, stride);
                    Buffer.BlockCopy(inputImage, 2 * i * stride + stride, dataRow2, 0, stride);

                    for (int j = 0; j < _width; j++)
                    {
                        emguImage1_Array[i, j, 0] = dataRow1[2 * j] + (dataRow1[2 * j + 1] << 8);
                        emguImage2_Array[i, j, 0] = dataRow2[2 * j] + (dataRow2[2 * j + 1] << 8);
                    }
                }

                Console.WriteLine($"       DoubleLight16bitImageReader - Image convertsion time to emgu-image: {sw.ElapsedMilliseconds}ms.");
            }
            catch (Exception ex)
            {
                _logger.Error($"Exception during file read (16 bit double light): {(string.IsNullOrEmpty(_fileName) ? string.Empty : _fileName)}. {ex.Message}");
                return false;
            }

            return true;
        }


    }


    /// <summary>
    /// 
    /// </summary>
    public class Factory_DoubleLight16bitImageReader : IDoubleLightImageReader_Creator
    {
        public IDoubleLightImageReader Factory(ILogger logger, int width, bool showImages)
        {
            return new DoubleLight16bitImageReader(logger, width, showImages);
        }
    }


}
