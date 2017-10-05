using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV;
using ImageEvaluatorLib.CalculateStatisticalData;
using ImageEvaluatorInterfaces;
using NLog;

namespace ImageEvaluatorLib.FitEdgeLines
{
    class EdgeLineFitter_Emgu1 : EdgeLineFitterBase
    {
        protected override OutputArray FitEdge(IWaferEdgeFindData edgepoints)
        {
            throw new NotImplementedException();
        }

    }



    /// <summary>
    /// 
    /// </summary>
    public class Factory_EdgeLineFitter_Emgu1 : IEdgeLineFitter_Creator
    {
        public IEdgeLineFitter Factory()
        {
            return new EdgeLineFitter_Emgu1();
        }
    }

}
