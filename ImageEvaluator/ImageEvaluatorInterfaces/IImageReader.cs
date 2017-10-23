using NLog;
using System.Collections.Generic;
using ImageEvaluatorInterfaces.BaseClasses;

namespace ImageEvaluatorInterfaces
{
    public interface IImageReader : IInitalizable
    {
        bool GetImage(string fileName, List<NamedData> data);
    }


    public interface IImageReader_Creator
    {
        IImageReader Factory(ILogger logger, int width, int height, bool showImages);
    }


}
