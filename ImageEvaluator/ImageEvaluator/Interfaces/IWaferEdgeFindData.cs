using System;
using System.Collections.Generic;
using System.Linq;
using Emgu.CV.Util;

namespace ImageEvaluator.Interfaces
{
    interface IWaferEdgeFindData
    {

        VectorOfPointF LeftSide { get; set; }
        VectorOfPointF RightSide { get; set; }
        VectorOfPointF TopSide { get; set; }
        VectorOfPointF BottomSide { get; set; }

    }
}
