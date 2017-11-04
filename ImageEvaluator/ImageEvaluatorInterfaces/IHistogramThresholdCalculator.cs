using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV;
using NLog;

namespace ImageEvaluatorInterfaces
{
    public interface IHistogramThresholdCalculator : IInitalizable, IElement
    {
        bool Execute(DenseHistogram hist, out float pos);

    }


    public interface IHistogramThresholdCalculator_Creator
    {
        IHistogramThresholdCalculator Factory(ILogger logger, int range, int param1);
    }


}
