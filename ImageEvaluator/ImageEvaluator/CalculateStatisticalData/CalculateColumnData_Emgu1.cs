using Emgu.CV;
using Emgu.CV.Structure;
using System.Drawing;
using System;

namespace ImageEvaluator.CalculateStatisticalData
{
    class CalculateColumnData_Emgu1 : CalculateColumnData_Base_Emgu
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputImage"></param>
        /// <param name="pointArray"></param>
        /// <param name="meanVector"></param>
        /// <param name="stdVector"></param>
        protected override void CalculateStatistics(Image<Gray, float> inputImage, int[,] pointArray, Image<Gray, float> meanVector, Image<Gray, float> stdVector)
        {
            int imageWidth = inputImage.Width;
            float[,,] resultVector1 = meanVector.Data;
            float[,,] resultVector2 = stdVector.Data;


            for (int i = 0; i < pointArray.Length / 2; i++)
            {
                if (pointArray[i, 0] > 0 && pointArray[i, 1] < imageWidth && (pointArray[i, 1] - pointArray[i, 0]) < imageWidth)
                {
                    Rectangle r = new Rectangle(pointArray[i, 0], i, pointArray[i, 1] - pointArray[i, 0], 1);

                    MCvScalar mean = new MCvScalar();
                    MCvScalar std = new MCvScalar();

                    inputImage.ROI = r;
                    CvInvoke.MeanStdDev(inputImage, ref mean, ref std, null);

                    resultVector1[i, 0, 0] = (float)mean.V0;
                    resultVector2[i, 0, 0] = (float)std.V0;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputImage"></param>
        /// <param name="maskImage"></param>
        /// <param name="pointArray"></param>
        /// <returns></returns>
        protected override bool CheckInputData(Image<Gray, float> inputImage, Image<Gray, byte> maskImage, int[,] pointArray, Image<Gray, float> meanVector, Image<Gray, float> stdVector)
        {
            base.CheckInputData(inputImage, maskImage, pointArray, meanVector, stdVector);

            if (pointArray == null || pointArray.Length < 0 || (pointArray.Length / 2) > 10000 || (pointArray.Length / 2) != inputImage.Height)
                return false;


            return true;
        }


        protected override bool Init(int width, int height)
        {
            return true;
        }


        protected override void InitEmguImages(int width, int height)
        {
        }

        protected override void ClearEmguImages()
        {
        }
    }
}
