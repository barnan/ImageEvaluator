using Emgu.CV;
using Emgu.CV.Structure;
using ImageEvaluatorInterfaces.BaseClasses;
using NLog;
using System.Collections.Generic;

namespace ImageEvaluatorInterfaces
{
    public interface IColumnDataCalculator : IInitalizable, IElement
    {
        bool Execute(List<NamedData> data, string fileName);
    }



    public interface IColumnDataCalculator_Creator
    {
        IColumnDataCalculator Factory(ILogger logger, int width, int height);
    }


}
