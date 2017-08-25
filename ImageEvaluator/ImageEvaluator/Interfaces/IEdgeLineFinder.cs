using System.Collections.Generic;
using System.Drawing;
using Emgu.CV;
using Emgu.CV.Structure;
using NLog;

namespace ImageEvaluator.Interfaces
{
    interface IEdgeLineFinder : IInitalizable
    {
        bool FindEdgeLines(Image<Gray, float> originalImage, Image<Gray, byte> maskImage, ref IWaferEdgeFindData edgeFindData);
    }


    internal enum SearchOrientations
    {
        TopToBottom,
        BottomToTop,
        LeftToRight,
        RightToLeft
    }

    interface IEdgeLineFinder_Creator
    {
        IEdgeLineFinder Factory(ILogger logger, Dictionary<SearchOrientations, Rectangle> calcAreas);
    }


}
