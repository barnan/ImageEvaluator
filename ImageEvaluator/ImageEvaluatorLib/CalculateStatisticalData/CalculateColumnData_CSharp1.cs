﻿
using Emgu.CV;
using Emgu.CV.Structure;
using NLog;
using System;
using ImageEvaluatorInterfaces;
using System.Collections.Generic;
using ImageEvaluatorInterfaces.BaseClasses;

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
        /// <param name="resu3"></param>
        /// <param name="resu4"></param>
        /// <param name="resu1"></param>
        /// <param name="resu2"></param>
        /// <param name="resu5"></param>
        /// <param name="resu6"></param>
        public override bool Run(List<NamedData> data, int[,] pointArray, ref Image<Gray, double> meanVector, ref Image<Gray, double> stdVector,
                                out double resu1, out double resu2, out double resu3, out double resu4, out double resu5, out double resu6, out double resu7, out double resu8,
                                out double resu9, out double resu10)
        {
            Image<Gray, byte>[] rawImages = null;
            Image<Gray, byte>[] maskImages = null;


            resu1 = 0;
            resu2 = 0;
            resu3 = 0;
            resu4 = 0;
            resu5 = 0;
            resu6 = 0;
            resu7 = 0;
            resu8 = 0;
            resu9 = 0;
            resu10 = 0;

            _firstVector = new Image<Gray, double>(_height, 1);
            _secondVector = new Image<Gray, double>(_height, 1);
            _resultVector1 = _firstVector.Data;
            _resultVector2 = _secondVector.Data;

            try
            {
                if (!IsInitialized)
                {
                    _logger.Error("CalculateColumnData_CSharp2 is not initialized.");
                    return false;
                }


                rawImages = GetEmguByteImages("_rawImages", data);
                int imageCounterRaw = rawImages?.Length ?? 0;

                maskImages = GetEmguByteImages("maskImages", data);
                int imageCounterMask = maskImages?.Length ?? 0;

                if (imageCounterMask != imageCounterRaw)
                {
                    _logger.Info($"{this.GetType()} input and mask image number is not the same!");
                    return false;
                }

                for (int m = 0; m < imageCounterRaw; m++)
                {
                    meanVector = _firstVector;
                    stdVector = _secondVector;

                    if (!CheckInputData(rawImages[m], maskImages[m], pointArray, meanVector, stdVector))
                    {
                        _logger.Info($"{this.GetType()} input and mask data is not proper!");
                        continue;
                    }

                    byte[,,] imgData = rawImages[m].Data;

                    for (int i = 0; i < pointArray.Length / 2; i++)
                    {

                        double sum = 0;
                        double sum2 = 0;
                        int counter = 0;

                        for (int j = 0; j < (pointArray[i, 1] - pointArray[i, 0]); j++)
                        {
                            sum += imgData[i, pointArray[i, 0] + j, 0];
                            counter++;
                        }

                        _resultVector1[0, i, 0] = (float)(sum / counter);
                        counter = 0;

                        for (int j = 0; j < (pointArray[i, 1] - pointArray[i, 0]); j++)
                        {
                            sum2 += Math.Pow(imgData[i, pointArray[i, 0] + j, 0] - _resultVector1[0, i, 0], 2);
                            counter++;
                        }

                        _resultVector2[i, 0, 0] = (float)Math.Sqrt(sum2 / (counter - 1));

                    }
                }

                return true;

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during the CalculateColumnData_CSharp1-CalculateStatistics, message: {ex}.");
                return false;
            }
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
    public class FactoryCalculateColumnDataCSharp1 : IColumnDataCalculator_Creator
    {
        public IColumnDataCalculator Factory(ILogger logger, int width, int height)
        {
            return new CalculateColumnDataCSharp1(logger, width, height);
        }
    }


}
