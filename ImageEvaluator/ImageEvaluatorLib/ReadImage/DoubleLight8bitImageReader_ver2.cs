using System;
using System.Diagnostics;
using System.IO;
using ImageEvaluatorInterfaces;
using NLog;

namespace ImageEvaluatorLib.ReadImage
{
    internal class DoubleLight8BitImageReaderVer2 : DoubleLightImageReader_Base
    {
        public DoubleLight8BitImageReaderVer2(ILogger logger, int width, int height, bool showImages)
            : base(logger, width, height, showImages)
        {
            _bitNumber = 1;
            _logger?.Info($"{this.GetType().Name} instantiated.");
        }


        protected override bool ReadImage()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();

            try
            {
                byte[] inputImage = File.ReadAllBytes(_fileName);
                int wid = _width;

                Console.WriteLine($"{this.GetType().Name} - Image reading time: {sw.ElapsedMilliseconds}ms.");
                sw.Restart();

                byte[] image1 = new byte[inputImage.Length / 2];
                byte[] image2 = new byte[inputImage.Length / 2];

                // make separate emgu images:

                for (int i = 0; i < _height; i++)
                {
                    Buffer.BlockCopy(inputImage, 2 * i * wid, image1, i * wid, wid);
                    Buffer.BlockCopy(inputImage, 2 * i * wid + wid, image2, 0, wid);
                }

                _rawImages[0].Bytes = image1;
                _rawImages[1].Bytes = image2;

                Console.WriteLine($"{this.GetType().Name} - Image conversion time to emgu-image: {sw.ElapsedMilliseconds}ms.");

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
    public class FactoryDoubleLight8BitImageReaderVer2 : IImageReader_Creator
    {
        public IImageReader Factory(ILogger logger, int width, int height, bool showImages)
        {
            logger?.Info($"{typeof(FactoryDoubleLight8BitImageReaderVer2)} factory called.");
            return new DoubleLight8BitImageReaderVer2(logger, width, height, showImages);
        }
    }


}
