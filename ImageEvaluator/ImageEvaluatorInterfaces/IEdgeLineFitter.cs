
using System.Collections.Generic;
using System.Drawing;
using NLog;

namespace ImageEvaluatorInterfaces
{
    public interface IEdgeLineFitter : IInitalizable
    {
        bool Run(IWaferEdgeFindData edgeFindData, ref IWaferFittingData edgeFitData);
    }


    public interface IEdgeLineFitter_Creator
    {
        IEdgeLineFitter Factory();
    }

}
