using System;
using System.Diagnostics;
using System.IO;
using Emgu.CV;
using Emgu.CV.Structure;
using ImageEvaluatorInterfaces;
using NLog;

namespace ImageEvaluatorLib.ReadImage
{
    internal class SimpleLight8BitImageReader : SimpleLightImageReader_Base
    {


        public SimpleLight8BitImageReader(ILogger logger, int width, bool showImages)
            : base(logger, width, showImages)
        {
            _bitNumber = 1;

            _logger?.Info("SimpleLight8bitImageReader instantiated.");
        }

        protected override bool ReadImage()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();

            try
            {
                byte[] inputImage = File.ReadAllBytes(_fileName);
                int stride = _width;

                Console.WriteLine($"       SimpleLight8bitImageReader - Image reading time: {sw.ElapsedMilliseconds}ms.");
                sw.Restart();

                //byte[] dataRow1 = new byte[stride];

                byte[,,] emguImage1Array = _img1.Data;

                for (int i = 0; i < _height; i++)
                {
                    //Buffer.BlockCopy(inputImage, i*stride, dataRow1, 0, stride);

                    for (int j = 0; j < _width; j++)
                    {
                        int index = j + i*_width;
                        //emguImage1Array[i, j, 0] = dataRow1[j];
                        emguImage1Array[i, j, 0] = inputImage[index];
                    }

                }

                Console.WriteLine($"       SimpleLight8bitImageReader - Image conversion time to emgu-image: {sw.ElapsedMilliseconds}ms.");
            }
            catch (Exception ex)
            {
                _logger.Error($"Exception during file read (8 bit simple light): {(string.IsNullOrEmpty(_fileName) ? string.Empty : _fileName)}. {ex}");
                return false;
            }

            return true;
        }

    }



    public class FactorySimpleLight8BitImageReader : ISimpleLightImageReader_Creator
    {
        public IImageReader Factory(ILogger logger, int width, bool showImages)
        {
            logger?.Info($"{typeof(FactorySimpleLight8BitImageReader)} factory called.");
            return new SimpleLight8BitImageReader(logger, width, showImages);
        }
    }


}
