using Emgu.CV;

namespace ImageEvaluator.Interfaces
{
    interface IWaferFittingData
    {
        OutputArray LeftSideLineData { get; set; }
        OutputArray RightSideLineData { get; set; }
        OutputArray TopSideLineData { get; set; }
        OutputArray BottomSideLineData { get; set; }

    }
}
