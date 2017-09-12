using NLog;

namespace ImageEvaluator.Interfaces
{
    public interface IResultSaver : IInitalizable
    {
        bool SaveResult(IMeasurementResult result, string inputfilename);
    }


    public interface IResultSaver_Creator
    {
        IResultSaver Factory(string outputFolder, string prefix, ILogger logger);
    }
}
