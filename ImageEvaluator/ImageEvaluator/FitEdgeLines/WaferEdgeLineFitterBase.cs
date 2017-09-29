
using Emgu.CV;
using ImageEvaluator.Interfaces;

namespace ImageEvaluator
{
    abstract class EdgeLineFitterBase : IEdgeLineFitter
    {

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
