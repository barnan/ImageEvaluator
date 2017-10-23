using ImageEvaluatorInterfaces.BaseClasses;
using NLog;
using System.Collections.Generic;

namespace ImageEvaluatorInterfaces
{
    public interface ISawmarkDeterminer : IInitalizable
    {
        bool Run(List<NamedData> data, string name);
    }



    public interface ISawmarkDeterminer_Creator
    {
        ISawmarkDeterminer Factory(ILogger logger, IWaferOrientationDetector detector);
    }

}
