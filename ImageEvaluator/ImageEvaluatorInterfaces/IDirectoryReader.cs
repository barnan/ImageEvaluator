using ImageEvaluatorInterfaces.BaseClasses;
using NLog;
using System.Collections.Generic;

namespace ImageEvaluatorInterfaces
{
    public interface IDirectoryReader : IInitalizable
    {
        bool GetNextImage(List<NamedData> data, ref string name);

        bool Restart();

        bool IsEndOfDirectory();
    }


    /// <summary>
    ///  Creator
    /// </summary>
    public interface IDirectoryReader_Creator
    {
        IDirectoryReader Factory(ILogger logger, string directoryName, string extension, IImageReader reader);
    }


}
