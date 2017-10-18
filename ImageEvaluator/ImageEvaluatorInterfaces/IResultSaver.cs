using NLog;

namespace ImageEvaluatorInterfaces
{
    public interface IResultSaver : IInitalizable
    {
        bool SaveResult(IMeasurementResult result, string inputfilename, string ext);

        string OutputFolder { get; set; }

    }


    public interface IResultSaver_Creator
    {
        IResultSaver Factory(string outputFolder, string prefix, ILogger logger);
    }
}
