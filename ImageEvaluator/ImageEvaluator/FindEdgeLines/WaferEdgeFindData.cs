using Emgu.CV.Util;
using ImageEvaluator.Interfaces;

namespace ImageEvaluator.FindEdgeLines
{
    class WaferEdgeFindData : IWaferEdgeFindData
    {
        public VectorOfPoint LeftSide { get; set; }
        public VectorOfPoint RightSide { get; set; }
        public VectorOfPoint TopSide { get; set; }
        public VectorOfPoint BottomSide { get; set; }
    }
}
