using Emgu.CV.Util;
using ImageEvaluator.Interfaces;

namespace ImageEvaluator.FindEdgeLines
{
    class WaferEdgeFindData : IWaferEdgeFindData
    {
        public VectorOfPointF LeftSide { get; set; }
        public VectorOfPointF RightSide { get; set; }
        public VectorOfPointF TopSide { get; set; }
        public VectorOfPointF BottomSide { get; set; }
    }
}
