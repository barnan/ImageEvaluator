
using System.Collections.Generic;
using System.Drawing;
using NLog;

namespace ImageEvaluator.Interfaces
{
    public interface IEdgeLineFitter : IInitalizable
    { 
        void Run(IWaferEdgeFindData edgeFindData, ref IWaferFittingData edgeFitData);
    }


    public interface IEdgeLineFitter_Creator
    {
        IEdgeLineFitter Factory();
    }

}
