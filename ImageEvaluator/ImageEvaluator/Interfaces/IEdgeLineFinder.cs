using System.Collections.Generic;
using System.Drawing;
using Emgu.CV;
using Emgu.CV.Structure;
using NLog;

namespace ImageEvaluator.Interfaces
{
    public interface IEdgeLineFinder : IInitalizable
    {
        bool Run(Image<Gray, float> originalImage, Image<Gray, byte> maskImage, ref IWaferEdgeFindData edgeFindData);
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
        IEdgeLineFinder Factory(ILogger logger, Dictionary<SearchOrientations, Rectangle> calcAreas);
    }


}
