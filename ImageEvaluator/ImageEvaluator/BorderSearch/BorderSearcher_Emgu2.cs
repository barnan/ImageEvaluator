using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using ImageEvaluator.SearchContourPoints;
using NLog;

namespace ImageEvaluator.BorderSearch
{
    class BorderSearcher_Emgu2 : BorderSearcherBase
    {
        public BorderSearcher_Emgu2(ILogger logger, int border, bool show, int imageHeight)
            : base(logger, imageHeight, border)
        {

        }

        protected override void CalculatePoints(Image<Gray, byte> maskImage)
        {
            using (Mat hierarchy = new Mat())
            {
                using (VectorOfVectorOfPoint contour = new VectorOfVectorOfPoint())
                {
                    try
                    {
                        CvInvoke.FindContours(maskImage, contour, hierarchy, RetrType.List, ChainApproxMethod.ChainApproxNone);

                        int verticalCenterLine = maskImage.Width / 2;
                        int magicNumber1 = 2000;

                        for (int i = 0; i < contour.Size; i++)
                        {
                           
                            





                        }




                    }
                    catch (Exception)
                    {
                        throw;
                    }




                }
            }





        }
    }


}
