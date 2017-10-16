using NLog;

namespace ImageEvaluatorInterfaces
{
    public interface IResultSaver : IInitalizable
    {
        bool SaveResult(IColumnMeasurementResult result, string inputfilename);
    }


    public interface IResultSaver_Creator
    {
        IResultSaver Factory(string outputFolder, string prefix, ILogger logger);
    }
}
