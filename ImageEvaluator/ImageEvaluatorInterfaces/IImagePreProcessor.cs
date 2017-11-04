using ImageEvaluatorInterfaces.BaseClasses;
using NLog;
using System.Collections.Generic;

namespace ImageEvaluatorInterfaces
{
    public interface IImagePreProcessor : IInitalizable, IElement
    {
        bool Execute(List<NamedData> data, string name);
    }


    public interface IImagePreProcessorCreator
    {
        IImagePreProcessor Factory(ILogger logger, int bitDepth, int width, int height, IHistogramThresholdCalculator histcalculator, bool showImages, BeltCoordinates beltcoords);
    }

}
