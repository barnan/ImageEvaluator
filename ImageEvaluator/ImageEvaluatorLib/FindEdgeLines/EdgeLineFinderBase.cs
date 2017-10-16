using System.Collections.Generic;
using System.Drawing;
using Emgu.CV;
using Emgu.CV.Structure;
using ImageEvaluatorInterfaces;
using NLog;

namespace ImageEvaluatorLib.FindEdgeLines
{
    abstract class EdgeLineFinderBase : IEdgeLineFinder
    {
        protected readonly ILogger _logger;
        protected readonly Dictionary<SearchOrientations, Rectangle> _calcAreas;


        protected EdgeLineFinderBase(ILogger logger, Dictionary<SearchOrientations, Rectangle> calcareas)
        {
            _logger = logger;
            _calcAreas = calcareas;
        }


        public abstract bool Run(Image<Gray, ushort> originalImage, Image<Gray, byte> maskImage, ref IWaferEdgeFindData edgeFindData);


        public bool Init()
        {
            return IsInitialized = true;
        }

        public bool IsInitialized { get; protected set; }


        protected virtual bool CheckInputData(Image<Gray, ushort> originalImage, Image<Gray, byte> maskImage, Dictionary<SearchOrientations, Rectangle> calcAreas)
        {
            if (originalImage == null || maskImage == null || calcAreas == null)
            {
                _logger?.Trace("EdgeLineFinder - the input data is not proper, some of them is null.");
                return false;
            }

            if (originalImage?.Width != maskImage?.Width || originalImage?.Height != maskImage?.Height)
            {
                _logger?.Trace("EdgeLineFinder - the side length of input images is not proper.");
                return false;
            }

            foreach (var calcarea in _calcAreas)
            {
                if (calcarea.Value.X < 0 || calcarea.Value.Y < 0 || calcarea.Value.Width < 0 || calcarea.Value.Height < 0
                     || calcarea.Value.X + calcarea.Value.Width > originalImage.Width
                     || calcarea.Value.Y + calcarea.Value.Height > originalImage.Height)
                {
                    _logger?.Trace("EdgeLineFinder - the calculation area is not proper.");
                    return false;
                }
            }

            return true;
        }


    }



}
