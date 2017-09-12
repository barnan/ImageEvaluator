using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV;
using ImageEvaluator.Interfaces;

namespace ImageEvaluator.FitEdgeLines
{
    class EdgeLineFitter_Emgu1 : EdgeLineFitterBase
    {
        protected override OutputArray FitEdge(IWaferEdgeFindData edgepoints)
        {
            throw new NotImplementedException();
        }
    }
}
