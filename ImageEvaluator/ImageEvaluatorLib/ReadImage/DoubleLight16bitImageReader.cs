using NLog;
using System;
using System.Diagnostics;
using System.IO;
using ImageEvaluatorInterfaces;
using ImageEvaluatorInterfaces.BaseClasses;

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
            ClassName = nameof(DoubleLight16BitImageReader);
            Title = ClassName;

            _bitNumber = 2;

            _logger?.Info($"Instantiated.", ClassName);
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

                _logger?.TraceLog($"Image reading time: {sw.ElapsedMilliseconds}ms.", ClassName);
                sw.Restart();

                byte[] dataRow1 = new byte[stride];
                byte[] dataRow2 = new byte[stride];

                // make separate emgu images:

                // to speed up:
                ushort[,,] emguImage1Array = _rawImages[0].Data;
                ushort[,,] emguImage2Array = _rawImages[1].Data;

                for (int i = 0; i < _height; i++)
                {
                    Buffer.BlockCopy(inputImage, 2 * i * stride, dataRow1, 0, stride);
                    Buffer.BlockCopy(inputImage, 2 * i * stride + stride, dataRow2, 0, stride);

                    for (int j = 0; j < _width; j++)
                    {
                        emguImage1Array[i, j, 0] = (ushort)(dataRow1[2 * j] + (dataRow1[2 * j + 1] << 8));
                        emguImage2Array[i, j, 0] = (ushort)(dataRow2[2 * j] + (dataRow2[2 * j + 1] << 8));
                    }
                }

                _logger?.TraceLog($"Conversion time to emgu-image: {sw.ElapsedMilliseconds}ms.", ClassName);
            }
            catch (Exception ex)
            {
                _logger?.ErrorLog($"Exception occured: {(string.IsNullOrEmpty(_fileName) ? string.Empty : _fileName)}: {ex}", ClassName);
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
            logger?.InfoLog($"Factory called.", nameof(Factory_DoubleLight16bitImageReader));
            return new DoubleLight16BitImageReader(logger, width, height, showImages);
        }
    }


}
