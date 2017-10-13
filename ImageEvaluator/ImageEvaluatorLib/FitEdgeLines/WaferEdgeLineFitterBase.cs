
using Emgu.CV;
using ImageEvaluatorInterfaces;
using NLog;

namespace ImageEvaluatorLib.FitEdgeLines
{
    internal abstract class EdgeLineFitterBase : IEdgeLineFitter
    {
        protected ILogger _logger;


        protected EdgeLineFitterBase(ILogger logger)
        {
            _logger = logger;
        }



        public bool Init()
        {
            return IsInitialized = true;
        }

        public bool IsInitialized { get; protected set; }


        public bool Run(IWaferEdgeFindData edgeFindData, ref IWaferFittingData edgeFitData)
        {

            return true;
        }



        protected abstract OutputArray FitEdge(IWaferEdgeFindData edgepoints);

    }
}
