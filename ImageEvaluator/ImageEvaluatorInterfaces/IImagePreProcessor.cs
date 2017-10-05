using Emgu.CV;
using Emgu.CV.Structure;
using NLog;

namespace ImageEvaluatorInterfaces
{
    public interface IImagePreProcessor : IInitalizable
    {
        bool Run(Image<Gray, float> inputImage, ref Image<Gray, byte> maskImage);
    }


    public interface IImagePreProcessor_Creator
    {
        IImagePreProcessor Factory(ILogger logger, int bitDepth, int width, int height, bool showImages);
    }

}
