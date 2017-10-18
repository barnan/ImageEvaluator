using System;
using System.Collections.Generic;
using System.Linq;
using Emgu.CV.Util;

namespace ImageEvaluatorInterfaces
{
    public interface IWaferEdgeFindData
    {
        VectorOfFloat LeftSide { get; set; }
        VectorOfFloat RightSide { get; set; }
        VectorOfFloat TopSide { get; set; }
        VectorOfFloat BottomSide { get; set; }

        float TopLineSpread { get; set; }
        float BottomLineSpread { get; set; }
        float LeftLineSpread { get; set; }
        float RightLineSpread { get; set; }
    }



    public interface IWaferEdgeFit
    {
        float Slope { get; set; }
        float Intercept { get; set; }
        bool InvertedRepresentation { get; set; }
        float LineSpread { get; set; }

        VectorOfFloat FitParams { get; set; }
    }


}
