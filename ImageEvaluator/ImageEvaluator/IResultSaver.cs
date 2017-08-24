using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;

namespace ImageEvaluator
{
    interface IResultSaver : IInitalizable
    {
        bool SaveResult(IMeasurementResult result);
    }


    interface IResultSaver_Creator
    {
        IResultSaver Factory(string outputFolder, string prefix, ILogger logger);
    }
}
