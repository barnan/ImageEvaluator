﻿using System;
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
            BorderPointArrays borderPointarrays = null;

            try
            {
                if (!IsInitialized)
                {
                    _logger.Error($"{this.GetType().Name} is not initialized.");
                    return false;
                }

                int imageCounter = LoadNamedData(data, ref borderPointarrays, ref rawImages, ref maskImages);
                if (imageCounter == 0)
                {
                    _logger.Info($"{this.GetType().Name} - No images were loaded from dynamicresult");
                }

                double[] meanOfMean = new double[imageCounter];
                double[] stdOfMean = new double[imageCounter];
                double[] meanOfStd = new double[imageCounter];
                double[] stdOfStd = new double[imageCounter];
                double[] homogeneity1 = new double[imageCounter];
                double[] homogeneity2 = new double[imageCounter];
                double[] minOfMean = new double[imageCounter];
                double[] maxOfMean = new double[imageCounter];

                for (int m = 0; m < imageCounter; m++)
                {
                    int[] indexes = Iterate(rawImages[m], maskImages[m], borderPointarrays[m]);
                    if (indexes == null || indexes.Length != 2)
                    {
                        _logger.Info($"{this.GetType().Name} - problem during Iterate. Return indexes are not proper for further calculation.");
                    }
                    int indexMin = indexes[0];
                    int indexMax = indexes[1];

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



        protected int[] Iterate(Image<Gray, byte> rawImage, Image<Gray, byte> maskImage, int[,] pointArray)
        {
            try
            {
                if (!CheckInputData(rawImage, maskImage, pointArray, _firstVector, _secondVector))
                {
                    _logger.Info($"{this.GetType()} input and mask data is not proper!");
                    return null;
                }

                ReAllocateEmgu();
                double[,,] resultVector1 = _firstVector.Data;
                double[,,] resultVector2 = _secondVector.Data;

                int imageWidth = rawImage.Width;
                int indexMin = int.MaxValue;
                int indexMax = int.MinValue;

                for (int i = 0; i < pointArray.Length / 2; i++)
                {
                    if (pointArray[i, 0] > 0 && pointArray[i, 1] < imageWidth && (pointArray[i, 1] - pointArray[i, 0]) < imageWidth)
                    {
                        Rectangle r = new Rectangle(pointArray[i, 0], i, pointArray[i, 1] - pointArray[i, 0], 1);

                        MCvScalar mean = new MCvScalar();
                        MCvScalar std = new MCvScalar();

                        rawImage.ROI = r;
                        CvInvoke.MeanStdDev(rawImage, ref mean, ref std);

                        resultVector1[0, i, 0] = mean.V0;
                        resultVector2[0, i, 0] = std.V0;

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
                        resultVector1[0, i, 0] = 0.0f;
                        resultVector2[0, i, 0] = 0.0f;
                    }
                }

                return new int[] { indexMin, indexMax };
            }
            catch (Exception)
            {
                _logger.Error($"Exception during {this.GetType().Name} - Iterate.");
                throw;
            }
        }


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


    public class FactoryCalculateColumnDataEmgu1 : IColumnDataCalculator_Creator
    {
        public IColumnDataCalculator Factory(ILogger logger, int width, int height)
        {
            logger?.Info($"{typeof(FactoryCalculateColumnDataEmgu1)} factory called.");
            return new CalculateColumnDataEmgu1(logger, width, height);
        }
    }


}
