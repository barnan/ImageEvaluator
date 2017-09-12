using System;
using System.Collections.Generic;
using System.Linq;
using Emgu.CV.Util;

namespace ImageEvaluator.Interfaces
{
    public interface IWaferEdgeFindData
    {

        VectorOfPoint LeftSide { get; set; }
        VectorOfPoint RightSide { get; set; }
        VectorOfPoint TopSide { get; set; }
        VectorOfPoint BottomSide { get; set; }

    }
}
