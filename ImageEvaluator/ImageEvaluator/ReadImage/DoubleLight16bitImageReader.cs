using NLog;
using System;
using System.IO;

namespace ImageEvaluator.ReadImage
{
    class DoubleLight16bitImageReader : DoubleLightImageReader_Base
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="width"></param>
        /// <param name="bitnumber"></param>
        internal DoubleLight16bitImageReader(ILogger logger, int width, int bitnumber, bool showImages)
            : base(logger, width, showImages)
        {
            _bitNumber = bitnumber;

            _logger?.Info("DoubleLight16bitImageReader instantiated.");
        }




        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputFileName"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        protected override void ReadDoubleLightImage()
        {
            try
            {
                byte[] inputImage = File.ReadAllBytes(_fileName);
                int stride = _width * 2;

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

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during fileread (16 bit double light): {(string.IsNullOrEmpty(_fileName) ? string.Empty : _fileName)}. {ex.Message}");
            }
        }


    }


    /// <summary>
    /// 
    /// </summary>
    class Factory_DoubleLight16bitImageReader : IDoubleLightImageReader_Creator
    {
        public IDoubleLightImageReader Factory(ILogger logger, int width, int bitnumber, bool showImages)
        {
            return new DoubleLight16bitImageReader(logger, width, bitnumber, showImages);
        }
    }


}
