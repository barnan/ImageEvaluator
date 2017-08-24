using System;
using System.Drawing;
using Emgu.CV;
using Emgu.CV.Structure;
using ImageEvaluator.Interfaces;
using NLog;

namespace ImageEvaluator.FindEdgeLines
{
    abstract class EdgeLineFinderBase :IEdgeLineFinder
    {
        private readonly ILogger _logger;


        protected EdgeLineFinderBase(ILogger logger)
        {
            _logger = logger;
        }


        public abstract bool FindEdgeLines(Image<Gray, float> originalImage, Image<Gray, byte> maskImage, ref IWaferEdgeFindData edgeFindData, Rectangle calcArea);


        public bool Init()
        {
            return true;
        }

    }


    internal enum SearchOrientations
    {
        TopToBottom,
        BottomToTop,
        LeftToRight,
        RightToLeft
    }
}
