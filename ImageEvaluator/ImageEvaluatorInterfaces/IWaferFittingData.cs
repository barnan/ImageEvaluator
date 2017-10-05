using Emgu.CV;

namespace ImageEvaluatorInterfaces
{
    public interface IWaferFittingData
    {
        OutputArray LeftSideLineData { get; set; }
        OutputArray RightSideLineData { get; set; }
        OutputArray TopSideLineData { get; set; }
        OutputArray BottomSideLineData { get; set; }

    }
}
