
namespace ImageEvaluator.Interfaces
{
    interface IEdgeLineFitter : IInitalizable
    {

        void Run(IWaferEdgeFindData edgeFindData, ref IWaferFittingData edgeFitData);






    }
}
