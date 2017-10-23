using System;
using Emgu.CV;
using Emgu.CV.Structure;
using System.Drawing;
using System.Drawing.Drawing2D;
using Emgu.CV.CvEnum;
using Emgu.CV.UI;
using ImageEvaluatorInterfaces;
using NLog;
using System.Collections.Generic;
using ImageEvaluatorInterfaces.BaseClasses;

namespace ImageEvaluatorLib.CalculateStatisticalData
{
    class CalculateColumnDataEmgu1 : CalculateColumnDataBaseEmgu
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        internal CalculateColumnDataEmgu1(ILogger logger, int width, int height)
            : base(logger, width, height)
        {
            InitEmguImages();

            _logger?.Info("CalculateColumnData_Emgu1 instantiated.");
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
        /// <param name="resu7"></param>
        /// <param name="resu8"></param>
        /// <param name="resu9"></param>
        /// <param name="resu10"></param>
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

            _meanVector = new Image<Gray, double>(_height, 1);
            _stdVector = new Image<Gray, double>(_height, 1);
            _resultVector1 = _meanVector.Data;
            _resultVector2 = _stdVector.Data;

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
                    meanVector = _meanVector;
                    stdVector = _stdVector;

                    if (!CheckInputData(rawImages[m], maskImages[m], pointArray, meanVector, stdVector))
                    {
                        _logger.Info($"{this.GetType()} input and mask data is not proper!");
                        continue;
                    }

                    int imageWidth = rawImages[m].Width;
                    int indexMin = int.MaxValue;
                    int indexMax = int.MinValue;

                    for (int i = 0; i < pointArray.Length / 2; i++)
                    {
                        if (pointArray[i, 0] > 0 && pointArray[i, 1] < imageWidth && (pointArray[i, 1] - pointArray[i, 0]) < imageWidth)
                        {
                            Rectangle r = new Rectangle(pointArray[i, 0], i, pointArray[i, 1] - pointArray[i, 0], 1);

                            MCvScalar mean = new MCvScalar();
                            MCvScalar std = new MCvScalar();

                            rawImages[m].ROI = r;
                            CvInvoke.MeanStdDev(rawImages[m], ref mean, ref std);

                            _resultVector1[0, i, 0] = (float)mean.V0;
                            _resultVector2[0, i, 0] = (float)std.V0;

                            if (i < indexMin)
                            {
                                indexMin = i;
                            }

                            if (i > indexMax)
                            {
                                indexMax = i;
                            }
                        }
                        else
                        {
                            _resultVector1[0, i, 0] = 0.0f;
                            _resultVector2[0, i, 0] = 0.0f;
                        }
                    }

                    if (!CalculateStatistics(indexMin, indexMax, maskImages[m]))
                    {
                        return false;
                    }

                }

                resu1 = _meanOfMean.V0;
                resu2 = _stdOfMean.V0;
                resu3 = _meanOfStd.V0;
                resu4 = _stdOfStd.V0;
                resu5 = Math.Max(Math.Abs(_meanOfRegion2.V0 - _meanOfRegion1.V0), Math.Abs(_meanOfRegion2.V0 - _meanOfRegion3.V0));
                resu6 = Math.Abs(_meanOfRegion1.V0 - _meanOfRegion3.V0);
                resu7 = _minOfMean.V0;
                resu8 = _maxOfMean.V0;

                return true;
            }
            catch (Exception ex)
            {
                _logger?.Error($"Exception occured during CalculateColumnDataEmgu1 - Run: {ex}");
                return false;
            }
            finally
            {
                foreach (var item in rawImages)
                {
                    if (item != null)
                    {
                        item.ROI = _fullROI;
                    }
                }
                foreach (var item in maskImages)
                {
                    if (item != null)
                    {
                        item.ROI = _fullROI;
                    }
                }
            }
        }




        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputImage"></param>
        /// <param name="maskImage"></param>
        /// <param name="pointArray"></param>
        /// <param name="meanVector"></param>
        /// <param name="stdVector"></param>
        /// <returns></returns>
        protected override bool CheckInputData(Image<Gray, byte> inputImage, Image<Gray, byte> maskImage, int[,] pointArray, Image<Gray, double> meanVector, Image<Gray, double> stdVector)
        {
            bool partResu = base.CheckInputData(inputImage, maskImage, pointArray, meanVector, stdVector);

            if (!partResu || pointArray == null || pointArray.Length < 0 || (pointArray.Length / 2) > 10000 || (pointArray.Length / 2) != inputImage.Height)
            {
                _logger?.Error("Error during CheckInputData");
                return false;
            }

            return true;
        }


    }


    /// <summary>
    /// 
    /// </summary>
    public class FactoryCalculateColumnDataEmgu1 : IColumnDataCalculator_Creator
    {
        public IColumnDataCalculator Factory(ILogger logger, int width, int height)
        {
            logger?.Info($"{typeof(FactoryCalculateColumnDataEmgu1)} factory called.");
            return new CalculateColumnDataEmgu1(logger, width, height);
        }
    }


}
