using Emgu.CV;
using Emgu.CV.Structure;
using NLog;

namespace ImageEvaluatorInterfaces
{
    public interface IImagePreProcessor : IInitalizable
    {
        bool Run(Image<Gray, byte> inputImage, ref Image<Gray, byte> maskImage, string name);
    }


    public interface IImagePreProcessorCreator
    {
        IImagePreProcessor Factory(ILogger logger, int bitDepth, int width, int height, IHistogramThresholdCalculator histcalculator, bool showImages,
                                    int beltLeftStart,
                                    int beltLeftEnd,
                                    int beltRightStart,
                                    int beltRightEnd);
    }

}
