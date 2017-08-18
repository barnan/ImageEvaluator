using NLog;
using System;
using System.IO;

namespace ImageEvaluator.ReadImage
{
    class DoubleLight8bitImageReader : DoubleLightImageReader_Base
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="width"></param>
        /// <param name="bitdepth"></param>
        internal DoubleLight8bitImageReader(ILogger logger, int width, int bitdepth) : base(logger, width)
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
        protected override void ReadDoubleLightImage()
        {
            try
            {
                byte[] inputImage = File.ReadAllBytes(_fileName);
                int stride = _width;

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
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during fileread (8 bit double light): {(string.IsNullOrEmpty(_fileName) ? string.Empty : _fileName)}. {ex.Message}");
            }
        }


    }





    /// <summary>
    /// 
    /// </summary>
    class Factory_DoubleLight8bitImageReader : IDoubleLightImageReader_Creator
    {
        public IDoubleLightImageReader Factory(ILogger logger, int width, int bitDepth)
        {
            return new DoubleLight8bitImageReader(logger, width, bitDepth);
        }
    }



}
