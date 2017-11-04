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


        public SimpleLight8BitImageReader(ILogger logger, int width, int height, bool showImages)
            : base(logger, width, height, showImages)
        {
            ClassName = nameof(DoubleLight16BitImageReader);
            Title = ClassName;

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
                int stride = _width;

                Console.WriteLine($"{this.GetType().Name} - Image reading time: {sw.ElapsedMilliseconds}ms.");
                sw.Restart();

                //_rawImages[0].Bytes = inputImage;

                ushort[,,] emguImage1Array = _rawImages[0].Data;

                for (int i = 0; i < _height; i++)
                {
                    int index = i * _width;

                    for (int j = 0; j < _width; j++)
                    {

                        emguImage1Array[i, j, 0] = inputImage[index + j];
                    }

                }

                Console.WriteLine($"{this.GetType().Name} - Image conversion time to emgu-image: {sw.ElapsedMilliseconds}ms.");
            }
            catch (Exception ex)
            {
                _logger.Error($"Exception during file read (8 bit simple light): {(string.IsNullOrEmpty(_fileName) ? string.Empty : _fileName)}. {ex}");
                return false;
            }

            return true;
        }

    }



    public class FactorySimpleLight8BitImageReader : IImageReader_Creator
    {
        public IImageReader Factory(ILogger logger, int width, int height, bool showImages)
        {
            logger?.Info($"{typeof(FactorySimpleLight8BitImageReader)} factory called.");
            return new SimpleLight8BitImageReader(logger, width, height, showImages);
        }
    }


}
