using NLog;

namespace ImageEvaluatorInterfaces
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
