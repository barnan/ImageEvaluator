
using Emgu.CV;
using ImageEvaluator.Interfaces;

namespace ImageEvaluator
{
    abstract class EdgeLineFitterBase :IEdgeLineFitter
    {

        public bool Init()
        {
            return true;
        }

        public void Run(IWaferEdgeFindData edgeFindData, ref IWaferFittingData edgeFitData)
        {
            
        }



        protected abstract OutputArray FitEdge(IWaferEdgeFindData edgepoints);

    }
}
