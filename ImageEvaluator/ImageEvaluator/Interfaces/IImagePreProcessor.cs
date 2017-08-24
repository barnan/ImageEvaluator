using Emgu.CV;
using Emgu.CV.Structure;
using NLog;

namespace ImageEvaluator.Interfaces
{
    interface IImagePreProcessor : IInitalizable
    {
        bool Run(Image<Gray, float> inputImage, ref Image<Gray, byte> maskImage);
    }


    interface IImagePreProcessor_Creator
    {
        IImagePreProcessor Factory(ILogger logger, int bitDepth, int width, int height, bool showImages);
    }

}
