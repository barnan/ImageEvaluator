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

            _logger?.Info($"{this.GetType().Name} instantiated.");
        }


        public override bool Run(List<NamedData> data, string fileName)
        {

            Image<Gray, byte>[] rawImages = null;
            Image<Gray, byte>[] maskImages = null;
            BorderPointArrays borderPointarrays;



            try
            {
                if (!IsInitialized)
                {
                    _logger.Error($"{this.GetType().Name} is not initialized.");
                    return false;
                }

                rawImages = GetEmguByteImages("_rawImages", data);
                int imageCounterRaw = rawImages?.Length ?? 0;

                maskImages = GetEmguByteImages("maskImages", data);
                int imageCounterMask = maskImages?.Length ?? 0;

                borderPointarrays = GetBorderPointArrays("borderPointArrayList", data);


                if ((imageCounterMask != imageCounterRaw) || (imageCounterRaw == 0) || (imageCounterRaw != borderPointarrays.Count))
                {
                    _logger.Info($"{this.GetType()} input and mask image number is not the same!");
                    return false;
                }

                double[] meanOfMean = new double[imageCounterRaw];
                double[] stdOfMean = new double[imageCounterRaw];
                double[] meanOfStd = new double[imageCounterRaw];
                double[] stdOfStd = new double[imageCounterRaw];
                double[] homogeneity1 = new double[imageCounterRaw];
                double[] homogeneity2 = new double[imageCounterRaw];
                double[] minOfMean = new double[imageCounterRaw];
                double[] maxOfMean = new double[imageCounterRaw];

                for (int m = 0; m < imageCounterRaw; m++)
                {
                    if (!CheckInputData(rawImages[m], maskImages[m], borderPointarrays[m], _meanVector, _stdVector))
                    {
                        _logger.Info($"{this.GetType()} input and mask data is not proper!");
                        continue;
                    }

                    ReAllocateEmgu();
                    _resultVector1 = _meanVector.Data;
                    _resultVector2 = _stdVector.Data;

                    int imageWidth = rawImages[m].Width;
                    int indexMin = int.MaxValue;
                    int indexMax = int.MinValue;

                    for (int i = 0; i < borderPointarrays[m].Length / 2; i++)
                    {
                        if (borderPointarrays[m][i, 0] > 0 && borderPointarrays[m][i, 1] < imageWidth && (borderPointarrays[m][i, 1] - borderPointarrays[m][i, 0]) < imageWidth)
                        {
                            Rectangle r = new Rectangle(borderPointarrays[m][i, 0], i, borderPointarrays[m][i, 1] - borderPointarrays[m][i, 0], 1);

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
                        _logger.Info($"Problem during statistics calculation: {m}");
                        continue;
                    }

                    meanOfMean[m] = _meanOfMean.V0;
                    stdOfMean[m] = _stdOfMean.V0;
                    meanOfStd[m] = _meanOfStd.V0;
                    stdOfStd[m] = _stdOfStd.V0;
                    homogeneity1[m] = Math.Max(Math.Abs(_meanOfRegion2.V0 - _meanOfRegion1.V0), Math.Abs(_meanOfRegion2.V0 - _meanOfRegion3.V0));
                    homogeneity2[m] = Math.Abs(_meanOfRegion1.V0 - _meanOfRegion3.V0);
                    minOfMean[m] = _minOfMean.V0;
                    maxOfMean[m] = _maxOfMean.V0;

                }

                data.Add(new DoubleVectorNamedData(meanOfMean, "meanOfMean", nameof(meanOfMean)));
                data.Add(new DoubleVectorNamedData(stdOfMean, "stdOfMean", nameof(stdOfMean)));
                data.Add(new DoubleVectorNamedData(meanOfStd, "meanOfStd", nameof(meanOfStd)));
                data.Add(new DoubleVectorNamedData(stdOfStd, "stdOfStd", nameof(stdOfStd)));
                data.Add(new DoubleVectorNamedData(homogeneity1, "homogeneity1", nameof(homogeneity1)));
                data.Add(new DoubleVectorNamedData(homogeneity2, "homogeneity2", nameof(homogeneity2)));
                data.Add(new DoubleVectorNamedData(minOfMean, "minOfMean", nameof(minOfMean)));
                data.Add(new DoubleVectorNamedData(maxOfMean, "maxOfMean", nameof(maxOfMean)));

                return true;
            }
            catch (Exception ex)
            {
                _logger?.Error($"Exception occured during {this.GetType().Name} - Run: {ex}");
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
