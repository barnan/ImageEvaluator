using Emgu.CV.Util;
using ImageEvaluatorInterfaces;

namespace ImageEvaluatorLib.FindEdgeLines
{
    class WaferEdgeFindData : IWaferEdgeFindData
    {
        public VectorOfPoint LeftSide { get; set; }
        public VectorOfPoint RightSide { get; set; }
        public VectorOfPoint TopSide { get; set; }
        public VectorOfPoint BottomSide { get; set; }
    }



    class MyCalculationRectangle
    {
        public int StartX { get; set; } = 0;
        public int StartY { get; set; } = 0;
        public int StopX { get; set; } = 0;
        public int StopY { get; set; } = 0;
        public int StepX { get; set; } = 0;
        public int StepY { get; set; } = 0;
    }


}
