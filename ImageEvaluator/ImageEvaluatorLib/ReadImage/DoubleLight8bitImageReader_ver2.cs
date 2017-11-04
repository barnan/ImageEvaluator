using System;
using System.Diagnostics;
using System.IO;
using ImageEvaluatorInterfaces;
using NLog;
using ImageEvaluatorInterfaces.BaseClasses;

namespace ImageEvaluatorLib.ReadImage
{
    internal class DoubleLight8BitImageReaderVer2 : DoubleLightImageReader_Base
    {
        [Obsolete]
        public DoubleLight8BitImageReaderVer2(ILogger logger, int width, int height, bool showImages)
            : base(logger, width, height, showImages)
        {
            ClassName = nameof(DoubleLight16BitImageReader);
            Title = ClassName;

            _bitNumber = 1;

            _logger?.Info($"Instantiated.", ClassName);
        }

        protected override bool ReadImage()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();

            try
            {
                byte[] inputImage = File.ReadAllBytes(_fileName);
                int wid = _width;

                _logger.TraceLog($"Image reading time: {sw.ElapsedMilliseconds}ms.", ClassName);
                sw.Restart();

                //byte[] image1 = new byte[inputImage.Length / 2];
                //byte[] image2 = new byte[inputImage.Length / 2];

                // make separate emgu images:
                ushort[,,] emguImage1Array = _rawImages[0].Data;
                ushort[,,] emguImage2Array = _rawImages[1].Data;

                for (int i = 0; i < _height; i++)
                {
                    //Buffer.BlockCopy(inputImage, 2 * i * wid, image1, i * wid, wid);
                    //Buffer.BlockCopy(inputImage, 2 * i * wid + wid, image2, 0, wid);

                    int index = 2 * i * wid;

                    for (int j = 0; j < _width; j++)
                    {
                        emguImage1Array[i, j, 0] = inputImage[index + j];
                        emguImage2Array[i, j, 0] = inputImage[index + j];
                    }
                }

                //_rawImages[0].Bytes = image1;
                //_rawImages[1].Bytes = image2;

                _logger.TraceLog($"Conversion time to emgu-image: {sw.ElapsedMilliseconds}ms.", ClassName);

            }
            catch (Exception ex)
            {
                _logger.ErrorLog($"Exception occured: {(string.IsNullOrEmpty(_fileName) ? string.Empty : _fileName)}. {ex.Message}", ClassName);
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
            logger?.InfoLog($"Factory called.", nameof(FactoryDoubleLight8BitImageReaderVer2));

            return new DoubleLight8BitImageReaderVer2(logger, width, height, showImages);
        }
    }


}
