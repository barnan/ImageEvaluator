using System;
using Emgu.CV;
using ImageEvaluatorInterfaces;
using NLog;

namespace ImageEvaluatorLib.FitEdgeLines
{
    class EdgeLineFitter_Emgu1 : EdgeLineFitterBase
    {

        public EdgeLineFitter_Emgu1(ILogger logger)
            : base(logger)
        {
            _logger?.Info($"{typeof(EdgeLineFitter_Emgu1)} instantiated.");
        }





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
        public IEdgeLineFitter Factory(ILogger logger)
        {
            logger?.Info($"{typeof(Factory_EdgeLineFitter_Emgu1).ToString()} factory called.");
            return new EdgeLineFitter_Emgu1(logger);
        }
    }

}
