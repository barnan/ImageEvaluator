using Emgu.CV.Util;
using ImageEvaluatorInterfaces;

namespace ImageEvaluatorLib.FindEdgeLines
{
    class WaferEdgeFindData : IWaferEdgeFindData
    {
        public VectorOfFloat LeftSide { get; set; }
        public VectorOfFloat RightSide { get; set; }
        public VectorOfFloat TopSide { get; set; }
        public VectorOfFloat BottomSide { get; set; }

        public float TopLineSpread { get; set; }
        public float BottomLineSpread { get; set; }
        public float LeftLineSpread { get; set; }
        public float RightLineSpread { get; set; }

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
