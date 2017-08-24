
using ImageEvaluator.Interfaces;

namespace ImageEvaluator
{
    abstract class EdgeLineFitter :IEdgeLineFitter
    {

        public abstract IWaferFittingData FitWaferEdges(IWaferEdgeFindData data);



        public bool Init()
        {
            return true;
        }
    }
}
