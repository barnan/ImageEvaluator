using Emgu.CV;
using Emgu.CV.Structure;
using NLog;

namespace ImageEvaluatorInterfaces
{
    public interface IImagePreProcessor : IInitalizable
    {
        bool Run(Image<Gray, float> inputImage, ref Image<Gray, byte> maskImage);
    }


    public interface IImagePreProcessorCreator
    {
        IImagePreProcessor Factory(ILogger logger, int bitDepth, int width, int height, IHistogramThresholdCalculator histcalculator, bool showImages);
    }

}
