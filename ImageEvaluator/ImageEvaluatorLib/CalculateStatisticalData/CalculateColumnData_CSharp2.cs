
using Emgu.CV;
using Emgu.CV.Structure;
using NLog;
using System;
using ImageEvaluatorInterfaces;

namespace ImageEvaluatorLib.CalculateStatisticalData
{
    class CalculateColumnDataCSharp2 : CalculateColumnDataBase
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        internal CalculateColumnDataCSharp2(ILogger logger, int width, int height)
            : base(logger, width, height)
        {
            _logger?.Info("CalculateColumnData_CSharp2 instantiated.");
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputImage"></param>
        /// <param name="maskImage"></param>
        /// <param name="pointArray"></param>
        /// <param name="meanVector"></param>
        /// <param name="stdVector"></param>
        /// <param name="resu1"></param>
        /// <param name="resu2"></param>
        /// <param name="resu3"></param>
        /// <param name="resu4"></param>
        /// <param name="resu5"></param>
        /// <param name="resu6"></param>
        public override bool Run(Image<Gray, byte> inputImage, Image<Gray, byte> maskImage, int[,] pointArray, ref Image<Gray, double> meanVector, ref Image<Gray, double> stdVector,
                                out double resu1, out double resu2, out double resu3, out double resu4, out double resu5, out double resu6)
        {
            resu1 = 0;
            resu2 = 0;
            resu3 = 0;
            resu4 = 0;
            resu5 = 0;
            resu6 = 0;
            _meanVector = new Image<Gray, double>(_height, 1);
            _stdVector = new Image<Gray, double>(_height, 1);
            _resultVector1 = _meanVector.Data;
            _resultVector2 = _stdVector.Data;

            if (!IsInitialized)
            {
                _logger.Error("CalculateColumnData_CSharp2 is not initialized.");
                return false;
            }

            if (!CheckInputData(inputImage, maskImage, pointArray, meanVector, stdVector))
                return false;

            byte[,,] imgData = inputImage.Data;

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

                    _resultVector1[0, i, 0] = (float)(sum / counter);
                    _resultVector2[0, i, 0] = (float)Math.Sqrt(sum2 / (counter - 1) - (sum * sum / Math.Pow(counter - 1, 2)));

                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error during the CalculateColumnData_CSharp2-CalculateStatistics, i:{i}, message: {ex}.");
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
        protected override bool CheckInputData(Image<Gray, byte> inputImage, Image<Gray, byte> maskImage, int[,] pointArray, Image<Gray, double> meanVector, Image<Gray, double> stdVector)
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
    public class FactoryCalculateColumnDataCSharp2 : IColumnDataCalculator_Creator
    {
        public IColumnDataCalculator Factory(ILogger logger, int width, int height)
        {
            return new CalculateColumnDataCSharp1(logger, width, height);
        }
    }


}
