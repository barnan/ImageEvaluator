using System;
using System.Collections.Generic;
using System.Drawing;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using ImageEvaluatorInterfaces;
using NLog;
using ImageEvaluatorInterfaces.BaseClasses;

namespace ImageEvaluatorLib.FindEdgeLines
{
    internal class EdgeLineFinderCSharp1 : EdgeLineFinderBase
    {
        public EdgeLineFinderCSharp1(ILogger logger, int width, int height, Dictionary<SearchOrientations, Rectangle> calcareas)
            : base(logger, width, height, calcareas)
        {
            _logger?.Info($"{this.GetType().Name} instantiated.");
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="originalImage"></param>
        /// <param name="maskImage"></param>
        /// <param name="edgeFindData"></param>
        /// <returns></returns>
        public override bool Execute(List<NamedData> data, ref IWaferEdgeFindData edgeFindData)
        {

            Image<Gray, byte>[] rawImages = null;
            Image<Gray, byte>[] maskImages = null;


            if (!IsInitialized)
            {
                _logger.Error($"{this.GetType().Name} is not initialized.");
                return false;
            }

            rawImages = GetEmguByteImages("RawImages", data);
            int imageCounterRaw = rawImages?.Length ?? 0;

            maskImages = GetEmguByteImages("MaskImages", data);
            int imageCounterMask = maskImages?.Length ?? 0;

            if (imageCounterMask != imageCounterRaw)
            {
                _logger.Info($"{this.GetType()} input and mask image number is not the same!");
                return false;
            }


            for (int m = 0; m < imageCounterRaw; m++)
            {
                if (!CheckInputData(rawImages[m], maskImages[m], _calcAreas))
                {
                    _logger.Info($"{this.GetType()} input and mask data is not proper!");
                    continue;
                }

                Point[] topPoints = FindPoints(maskImages[m], _calcAreas[SearchOrientations.TopToBottom], SearchOrientations.TopToBottom);
                Point[] bottomPoints = FindPoints(maskImages[m], _calcAreas[SearchOrientations.BottomToTop], SearchOrientations.BottomToTop);
                Point[] leftPoints = FindPoints(maskImages[m], _calcAreas[SearchOrientations.LeftToRight], SearchOrientations.LeftToRight);
                Point[] rightPoints = FindPoints(maskImages[m], _calcAreas[SearchOrientations.RightToLeft], SearchOrientations.RightToLeft);

                if (topPoints?.Length < 1 || bottomPoints?.Length < 1 || leftPoints?.Length < 1 || rightPoints?.Length < 1)
                {
                    _logger?.Trace("The found edge segments are not proper in EdgeLineFinder_CSharp1");
                    return false;
                }

                //WaferEdgeFindData result = new WaferEdgeFindData
                //{
                //    TopSide = new VectorOfPoint(topPoints),
                //    BottomSide = new VectorOfPoint(bottomPoints),
                //    LeftSide = new VectorOfPoint(leftPoints),
                //    RightSide = new VectorOfPoint(rightPoints)
                //};

                //edgeFindData = result;
            }

            return true;
        }


        private Point[] FindPoints(Image<Gray, byte> maskImage, Rectangle calcRectangle, SearchOrientations orientation)
        {
            _logger?.Trace($"FindPoints started. orientation: {orientation}");

            byte[,,] maskData = maskImage.Data;
            Point[] result = null;

            MyCalculationRectangle rect = DetermineMyRectangle(calcRectangle, orientation);

            try
            {
                switch (orientation)
                {
                    case SearchOrientations.TopToBottom:
                    case SearchOrientations.BottomToTop:
                        result = new Point[calcRectangle.Width];

                        for (int i = rect.StartX; i < rect.StopX; i += rect.StepX)
                        {
                            for (int j = 0; j < Math.Abs(rect.StopY - rect.StartY); j++)
                            {
                                if (maskData[rect.StartY + j * rect.StepY, i, 0] <= 0) continue;

                                result[i - rect.StartX] = new Point(j, i);
                                break;
                            }
                        }
                        break;

                    case SearchOrientations.LeftToRight:
                    case SearchOrientations.RightToLeft:
                        result = new Point[calcRectangle.Height];

                        for (int j = rect.StartY; j < rect.StopY; j += rect.StepY)
                        {
                            for (int i = 0; i < Math.Abs(rect.StopX - rect.StartX); i++)
                            {
                                if (maskData[j, rect.StartX + i * rect.StepX, 0] <= 0) continue;

                                result[j - rect.StartY] = new Point(j, i);
                                break;
                            }
                        }
                        break;
                }
            }
            catch (Exception)
            {
                _logger?.Error($"Exception during FindPoints, orientation {orientation.ToString()}");
            }


            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="calcRectangle"></param>
        /// <param name="orientation"></param>
        /// <returns></returns>
        private MyCalculationRectangle DetermineMyRectangle(Rectangle calcRectangle, SearchOrientations orientation)
        {
            MyCalculationRectangle myrectangle = new MyCalculationRectangle();

            switch (orientation)
            {
                case SearchOrientations.TopToBottom:
                    myrectangle.StartX = calcRectangle.X;
                    myrectangle.StartY = calcRectangle.Y;
                    myrectangle.StopX = calcRectangle.X + calcRectangle.Width;
                    myrectangle.StopY = calcRectangle.Y + calcRectangle.Height;
                    myrectangle.StepY = 1;
                    myrectangle.StepX = 1;
                    break;
                case SearchOrientations.BottomToTop:
                    myrectangle.StartX = calcRectangle.X;
                    myrectangle.StartY = calcRectangle.Y + calcRectangle.Height;
                    myrectangle.StopX = calcRectangle.X + calcRectangle.Width;
                    myrectangle.StopY = calcRectangle.Y;
                    myrectangle.StepY = -1;
                    myrectangle.StepX = 1;
                    break;
                case SearchOrientations.LeftToRight:
                    myrectangle.StartX = calcRectangle.X;
                    myrectangle.StartY = calcRectangle.Y;
                    myrectangle.StopX = calcRectangle.X + calcRectangle.Width;
                    myrectangle.StopY = calcRectangle.Y + calcRectangle.Height;
                    myrectangle.StepY = 1;
                    myrectangle.StepX = 1;
                    break;
                case SearchOrientations.RightToLeft:
                    myrectangle.StartX = calcRectangle.X + calcRectangle.Width;
                    myrectangle.StartY = calcRectangle.Y;
                    myrectangle.StopX = calcRectangle.X;
                    myrectangle.StopY = calcRectangle.Y + calcRectangle.Height;
                    myrectangle.StepY = 1;
                    myrectangle.StepX = -1;
                    break;
            }

            return myrectangle;
        }
    }


    /// <summary>
    /// 
    /// </summary>
    public class FactoryEdgeLineFinderCSharp1 : IEdgeLineFinder_Creator
    {
        public IEdgeLineFinder Factory(ILogger logger, int width, int height, Dictionary<SearchOrientations, Rectangle> calcAreas = null)
        {
            logger?.Info($"{typeof(FactoryEdgeLineFinderCSharp1)} factory called.");
            return new EdgeLineFinderCSharp1(logger, width, height, calcAreas);
        }
    }




}
