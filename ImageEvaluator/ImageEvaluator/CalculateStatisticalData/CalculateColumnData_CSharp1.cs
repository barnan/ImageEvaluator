
using Emgu.CV;
using Emgu.CV.Structure;
using System;

namespace ImageEvaluator.CalculateStatisticalData
{
    class CalculateColumnData_CSharp1 : CalculateColumnData_Base
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
            float[,,] imgData = inputImage.Data;
            float[,,] resultVector1 = meanVector.Data;
            float[,,] resultVector2 = stdVector.Data;

            for (int i = 0; i < pointArray.Length / 2; i++)
            {
                try
                {
                    if (pointArray[i, 0] > 0 && pointArray[i, 1] < inputImage.Width)
                    {
                        double sum = 0;
                        double sum2 = 0;
                        int counter = 0;

                        for (int j = 0; j < (pointArray[i, 1] - pointArray[i, 0]); j++)
                        {
                            sum += imgData[i, pointArray[i, 0] + j, 0];
                            counter++;
                        }

                        resultVector1[i, 0, 0] = (float)(sum / counter);
                        counter = 0;

                        for (int j = 0; j < (pointArray[i, 1] - pointArray[i, 0]); j++)
                        {
                            sum2 += Math.Pow(imgData[i, pointArray[i, 0] + j, 0] - resultVector1[i, 0, 0], 2);
                            counter++;
                        }


                        resultVector2[i, 0, 0] = (float)Math.Sqrt(sum2 / (counter - 1));
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error during the CalculateColumnData_CSharp1-CalculateStatistics, i:{i}, message: {ex.Message}.");
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


    }
}
