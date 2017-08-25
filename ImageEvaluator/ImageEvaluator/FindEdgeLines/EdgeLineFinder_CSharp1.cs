using System;
using System.Collections.Generic;
using System.Drawing;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using ImageEvaluator.Interfaces;
using NLog;

namespace ImageEvaluator.FindEdgeLines
{
    class EdgeLineFinder_CSharp1 : EdgeLineFinderBase
    {
        public EdgeLineFinder_CSharp1(ILogger logger, Dictionary<SearchOrientations, Rectangle> calcareas)
            : base(logger, calcareas)
        {
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="originalImage"></param>
        /// <param name="maskImage"></param>
        /// <param name="edgeFindData"></param>
        /// <param name="calcAreas"></param>
        /// <returns></returns>
        public override bool FindEdgeLines(Image<Gray, float> originalImage, Image<Gray, byte> maskImage, ref IWaferEdgeFindData edgeFindData)
        {
            if (!CheckInputData(originalImage, maskImage, _calcAreas))
            {
                return false;
            }

            Point[] topPoints = FindPoints(maskImage, _calcAreas[SearchOrientations.TopToBottom], SearchOrientations.TopToBottom);
            Point[] bottomPoints = FindPoints(maskImage, _calcAreas[SearchOrientations.BottomToTop], SearchOrientations.BottomToTop);
            Point[] leftPoints = FindPoints(maskImage, _calcAreas[SearchOrientations.LeftToRight], SearchOrientations.LeftToRight);
            Point[] rightPoints = FindPoints(maskImage, _calcAreas[SearchOrientations.RightToLeft], SearchOrientations.RightToLeft);

            if (topPoints?.Length < 1 || bottomPoints?.Length < 1 || leftPoints?.Length < 1 || rightPoints?.Length < 1)
            {
                _logger?.Trace("The found edge segments are not proper in EdgeLineFinder_CSharp1");
                return false;
            }

            WaferEdgeFindData result = new WaferEdgeFindData
            {
                TopSide = new VectorOfPoint(topPoints),
                BottomSide = new VectorOfPoint(bottomPoints),
                LeftSide = new VectorOfPoint(leftPoints),
                RightSide = new VectorOfPoint(rightPoints)
            };

            edgeFindData = result;

            return true;
        }





        private Point[] FindPoints(Image<Gray, byte> maskImage, Rectangle calcRectangle, SearchOrientations orientation)
        {
            _logger?.Trace($"FindPoints started. orientation: {orientation}");

            int startX = 0;
            int startY = 0;
            int stopX = 0;
            int stopY = 0;
            int stepX = 0;
            int stepY = 0;

            byte[,,] maskData = maskImage.Data;
            Point[] result = null;

            switch (orientation)
            {
                case SearchOrientations.TopToBottom:
                    startX = calcRectangle.X;
                    startY = calcRectangle.Y;
                    stopX = calcRectangle.X + calcRectangle.Width;
                    stopY = calcRectangle.Y + calcRectangle.Height;
                    stepY = 1;
                    stepX = 1;
                    break;
                case SearchOrientations.BottomToTop:
                    startX = calcRectangle.X;
                    startY = calcRectangle.Y + calcRectangle.Height;
                    stopX = calcRectangle.X + calcRectangle.Width;
                    stopY = calcRectangle.Y;
                    stepY = -1;
                    stepX = 1;
                    break;
                case SearchOrientations.LeftToRight:
                    startX = calcRectangle.X;
                    startY = calcRectangle.Y;
                    stopX = calcRectangle.X + calcRectangle.Width;
                    stopY = calcRectangle.Y + calcRectangle.Height;
                    stepY = 1;
                    stepX = 1;
                    break;
                case SearchOrientations.RightToLeft:
                    startX = calcRectangle.X + calcRectangle.Width;
                    startY = calcRectangle.Y;
                    stopX = calcRectangle.X;
                    stopY = calcRectangle.Y + calcRectangle.Height;
                    stepY = 1;
                    stepX = -1;
                    break;
            }

            try
            {
                switch (orientation)
                {
                    case SearchOrientations.TopToBottom:
                    case SearchOrientations.BottomToTop:
                        result = new Point[calcRectangle.Width];

                        for (int i = startX; i < stopX; i += stepX)
                        {
                            for (int j = 0; j < Math.Abs(stopY - startY); j++)
                            {
                                if (maskData[startY + j*stepY, i, 0] <= 0) continue;

                                result[i - startX] = new Point(j, i);
                                break;
                            }
                        }
                        break;

                    case SearchOrientations.LeftToRight:
                    case SearchOrientations.RightToLeft:
                        result = new Point[calcRectangle.Height];

                        for (int j = startY; j < stopY; j += stepY)
                        {
                            for (int i = 0; i < Math.Abs(stopX - startX); i++)
                            {
                                if (maskData[j, startX + i*stepX, 0] <= 0) continue;

                                result[j - startY] = new Point(j, i);
                                break;
                            }
                        }
                        break;
                }
            }
            catch (Exception)
            {

                throw;
            }


            return result;
        }

    }


    class Factory_EdgeLineFinder_CSharp1 : IEdgeLineFinder_Creator
    {
        public IEdgeLineFinder Factory(ILogger logger, Dictionary<SearchOrientations, Rectangle> calcAreas)
        {
            return new EdgeLineFinder_CSharp1(logger, calcAreas);
        }
    }

    


}
