
using Emgu.CV;
using Emgu.CV.Structure;
using NLog;
using System;
using ImageEvaluatorInterfaces;

namespace ImageEvaluatorLib.CalculateStatisticalData
{
    class CalculateColumnDataCSharp1 : CalculateColumnDataBase
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        internal CalculateColumnDataCSharp1(ILogger logger, int width, int height)
            : base(logger, width, height)
        {
            _logger?.Info("CalculateColumnData_CSharp1 instantiated.");
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputImage"></param>
        /// <param name="maskImage"></param>
        /// <param name="pointArray"></param>
        /// <param name="meanVector"></param>
        /// <param name="stdVector"></param>
        public override bool Run(Image<Gray, float> inputImage, Image<Gray, byte> maskImage, int[,] pointArray, ref Image<Gray, float> meanVector, ref Image<Gray, float> stdVector)
        {
            if (!IsInitialized)
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
                        counter++;
                    }

                    _resultVector1[i, 0, 0] = (float)(sum / counter);
                    counter = 0;

                    for (int j = 0; j < (pointArray[i, 1] - pointArray[i, 0]); j++)
                    {
                        sum2 += Math.Pow(imgData[i, pointArray[i, 0] + j, 0] - _resultVector1[i, 0, 0], 2);
                        counter++;
                    }

                    _resultVector2[i, 0, 0] = (float)Math.Sqrt(sum2 / (counter - 1));
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error during the CalculateColumnData_CSharp1-CalculateStatistics, message: {ex}.");
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
        /// <param name="meanVector"></param>
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
    public class FactoryCalculateColumnDataCSharp1 : IColumnDataCalculator_Creator
    {
        public IColumnDataCalculator Factory(ILogger logger, int width, int height)
        {
            return new CalculateColumnDataCSharp1(logger, width, height);
        }
    }


}
