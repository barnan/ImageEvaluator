
using Emgu.CV;
using Emgu.CV.Structure;
using System;

namespace ImageEvaluator.CalculateStatisticalData
{
    class CalculateColumnData_CSharp2 : CalculateColumnData_Base
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        internal CalculateColumnData_CSharp2(int width, int height)
            : base(width, height)
        {
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputImage"></param>
        /// <param name="pointArray"></param>
        /// <param name="meanVector"></param>
        /// <param name="stdVector"></param>
        public override bool CalculateStatistics(Image<Gray, float> inputImage, Image<Gray, byte> maskImage, int[,] pointArray, ref Image<Gray, float> meanVector, ref Image<Gray, float> stdVector)
        {
            if (!CheckInputData(inputImage, maskImage, pointArray, meanVector, stdVector))
                return false;

            float[,,] imgData = inputImage.Data;

            meanVector = _meanVector;
            stdVector = _stdVector;

            for (int i = 0; i < pointArray.Length / 2; i++)
            {
                try
                {
                    double sum = 0;
                    double sum2 = 0;
                    int counter = 0;

                    for (int j = 0; j < (pointArray[i, 1] - pointArray[i, 0]); j++)
                    {
                        sum += imgData[i, pointArray[i, 0] + j, 0];
                        sum2 += (imgData[i, pointArray[i, 0] + j, 0] * imgData[i, pointArray[i, 0] + j, 0]);
                        counter++;
                    }

                    _resultVector1[i, 0, 0] = (float)(sum / counter);
                    _resultVector2[i, 0, 0] = (float)Math.Sqrt(sum2 / (counter - 1) - (sum * sum / Math.Pow(counter - 1, 2)));

                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error during the CalculateColumnData_CSharp2-CalculateStatistics, i:{i}, message: {ex.Message}.");
                    return false;
                }
            }

            return true;
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

    }



    /// <summary>
    /// 
    /// </summary>
    class Factory_CalculateColumnData_CSharp2 : IColumnDataCalculator_Creator
    {
        public IColumnDataCalculator Factory(int width, int height)
        {
            return new CalculateColumnData_CSharp1(width, height);
        }
    }


}
