using NLog;

namespace ImageEvaluator.Interfaces
{
    interface IResultSaver : IInitalizable
    {
        bool SaveResult(IMeasurementResult result, string inputfilename);
    }


    interface IResultSaver_Creator
    {
        IResultSaver Factory(string outputFolder, string prefix, ILogger logger);
    }
}
