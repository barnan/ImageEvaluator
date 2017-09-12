
using Emgu.CV;
using Emgu.CV.Structure;
using NLog;
using System;
using ImageEvaluator.Interfaces;

namespace ImageEvaluator.CalculateStatisticalData
{
    class CalculateColumnData_CSharp2 : CalculateColumnDataBase
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        internal CalculateColumnData_CSharp2(ILogger logger, int width, int height)
            : base(logger, width, height)
        {
            _logger?.Info("CalculateColumnData_CSharp2 instantiated.");
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputImage"></param>
        /// <param name="pointArray"></param>
        /// <param name="meanVector"></param>
        /// <param name="stdVector"></param>
        public override bool Run(Image<Gray, float> inputImage, Image<Gray, byte> maskImage, int[,] pointArray, ref Image<Gray, float> meanVector, ref Image<Gray, float> stdVector)
        {
            if (!_initialized)
            {
                _logger.Error("CalculateColumnData_CSharp2 is not initialized.");
                return false;
            }

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
            bool partResu = base.CheckInputData(inputImage, maskImage, pointArray, meanVector, stdVector);

            if (!partResu || pointArray == null || pointArray.Length < 0 || (pointArray.Length / 2) > 10000 || (pointArray.Length / 2) != inputImage.Height)
            {
                _logger.Error("Error during CheckInputData");
                return false;
            }

            return true;
        }

    }



    /// <summary>
    /// 
    /// </summary>
    public class Factory_CalculateColumnData_CSharp2 : IColumnDataCalculator_Creator
    {
        public IColumnDataCalculator Factory(ILogger logger, int width, int height)
        {
            return new CalculateColumnData_CSharp1(logger, width, height);
        }
    }


}
