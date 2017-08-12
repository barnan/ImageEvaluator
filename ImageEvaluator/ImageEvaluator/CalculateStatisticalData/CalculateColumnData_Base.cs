using System;
using Emgu.CV;
using Emgu.CV.Structure;

namespace ImageEvaluator.CalculateStatisticalData
{
    class CalculateColumnData_Base : IColumnDataCalculator
    {

        public void GetStatisticalData(Image<Gray, float> inputImage, Image<Gray, byte> maskimage,
                                        int[,] pointArray, Image<Gray, float> meanVector, Image<Gray, float> stdVector)
        {

        }


        protected virtual void CalculateStatistics(Image<Gray, float> inputImage, Image<Gray, byte> maskImage, Image<Gray, float> meanVector, Image<Gray, float> stdVector)
        { }

        protected virtual void CalculateStatistics(Image<Gray, float> inputImage, int[,] pointList, Image<Gray, float> meanVector, Image<Gray, float> stdVector)
        { }


        protected bool Init(int height)
        {


            return true;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputImage"></param>
        /// <param name="maskImage"></param>
        /// <returns></returns>
        protected virtual bool CheckInputData(Image<Gray, float> inputImage, Image<Gray, byte> maskImage, int[,] pointArray, Image<Gray, float> meanVector, Image<Gray, float> stdVector)
        {
            if (inputImage == null || inputImage.Height < 0 || inputImage.Height > 10000 || inputImage.Width < 0 || inputImage.Width > 10000)
            {
                return false;
            }
            if (meanVector == null || stdVector == null || meanVector.Height != inputImage.Height || stdVector.Height != inputImage.Height)
            {
                return false;
            }

            return true;
        }
    }


}
