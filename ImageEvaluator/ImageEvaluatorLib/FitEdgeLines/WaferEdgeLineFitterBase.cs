
using Emgu.CV;
using ImageEvaluatorInterfaces;
using NLog;

namespace ImageEvaluatorLib.FitEdgeLines
{
    abstract class EdgeLineFitterBase : IEdgeLineFitter
    {
        protected ILogger _logger;



        public EdgeLineFitterBase(ILogger logger)
        {
            _logger = logger;
        }



        public bool Init()
        {
            return true;
        }

        public bool Run(IWaferEdgeFindData edgeFindData, ref IWaferFittingData edgeFitData)
        {

            return true;
        }



        protected abstract OutputArray FitEdge(IWaferEdgeFindData edgepoints);

    }
}
