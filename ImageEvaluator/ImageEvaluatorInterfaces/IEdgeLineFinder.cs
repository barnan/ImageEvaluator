using System.Collections.Generic;
using System.Drawing;
using Emgu.CV;
using Emgu.CV.Structure;
using NLog;
using ImageEvaluatorInterfaces.BaseClasses;

namespace ImageEvaluatorInterfaces
{
    public interface IEdgeLineFinder : IInitalizable
    {
        bool Execute(List<NamedData> data, ref IWaferEdgeFindData edgeFindData);
    }


    public enum SearchOrientations
    {
        TopToBottom,
        BottomToTop,
        LeftToRight,
        RightToLeft
    }

    public interface IEdgeLineFinder_Creator
    {
        IEdgeLineFinder Factory(ILogger logger, int width, int height, Dictionary<SearchOrientations, Rectangle> calcAreas);
    }


}
