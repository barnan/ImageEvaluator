
using Emgu.CV;
using ImageEvaluatorInterfaces;

namespace ImageEvaluatorLib.FitEdgeLines
{
    class WaferFittingData : IWaferFittingData
    {
        public OutputArray LeftSideLineData { get; set; }
        public OutputArray RightSideLineData { get; set; }
        public OutputArray TopSideLineData { get; set; }
        public OutputArray BottomSideLineData { get; set; }
    }
}
