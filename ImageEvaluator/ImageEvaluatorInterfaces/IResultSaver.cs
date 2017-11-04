using ImageEvaluatorInterfaces.BaseClasses;
using NLog;
using System.Collections.Generic;

namespace ImageEvaluatorInterfaces
{
    public interface IResultSaver : IInitalizable, IElement
    {
        bool SaveResult(IMeasurementResult result, string inputfilename, string ext);

        string OutputFolder { get; set; }

    }


    public interface INamedDataResultSaver : IInitalizable, IElement
    {
        bool SaveResult(List<NamedData> result, string inputfilename, string ext);

        string OutputFolder { get; set; }

    }


    public interface IResultSaver_Creator
    {
        IResultSaver Factory(string outputFolder, string prefix, ILogger logger);
    }
}
