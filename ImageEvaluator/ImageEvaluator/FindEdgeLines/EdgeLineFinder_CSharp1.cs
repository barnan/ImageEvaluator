using System.Drawing;
using Emgu.CV;
using Emgu.CV.Structure;
using ImageEvaluator.Interfaces;
using NLog;

namespace ImageEvaluator.FindEdgeLines
{
    class EdgeLineFinder_CSharp1 : EdgeLineFinderBase
    {
        public EdgeLineFinder_CSharp1(ILogger logger)
            :base(logger)
        {
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="originalImage"></param>
        /// <param name="maskImage"></param>
        /// <param name="edgeFindData"></param>
        /// <param name="calcArea"></param>
        /// <returns></returns>
        public override bool FindEdgeLines(Image<Gray, float> originalImage, Image<Gray, byte> maskImage, ref IWaferEdgeFindData edgeFindData, Rectangle calcArea)
        {
            if (!CheckInputData(originalImage, maskImage, edgeFindData, calcArea))
            {
                return false;
            }



            return true;

        }



        private bool CheckInputData(Image<Gray, float> originalImage, Image<Gray, byte> maskImage, IWaferEdgeFindData edgeFindData, Rectangle calcArea)
        {



            return true;
        }



        private Point[] Findpoints(Image<Gray, byte> maskImage, Rectangle calcRectangle,  SearchOrientations orientation, IWaferEdgeFindData edgeFindData)
        {
            int startX = 0;
            int startY = 0;
            int stopX = 0;
            int stopY = 0;
            int stepX = 0;
            int stepY = 0;

            byte[,,] maskData = maskImage.Data;
            

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
                    startY = calcRectangle.Y + calcRectangle.Height;
                    stopX = calcRectangle.X;
                    stopY = calcRectangle.Y;
                    stepY = 1;
                    stepX = -1;
                    break;
            }


            switch (orientation)
            {
                case SearchOrientations.TopToBottom:
                case SearchOrientations.BottomToTop:
                    for (int i = startX; i < stopX; i += stepX )
                    {
                        for (int j = startY; j < stopY; j += stepY)
                        {
                            if (maskData[j,i,0] > 0)
                                edgeFindData.TopSide.Push(new PointF[] {new PointF(j,i) });
                        }
                    }
                    break;

                case SearchOrientations.LeftToRight:
                case SearchOrientations.RightToLeft:
                    for (int j = startY; j < stopY; j += stepY)
                    {
                        for (int i = startX; i < stopX; i += stepX)
                        {
                            if (maskData[j, i, 0] > 0)
                                edgeFindData.TopSide.Push(new PointF[] { new PointF(j, i) });
                        }
                    }
                    break;
            }

            
            return new Point[] {new Point(0,0) };
        }


        
    }
}
