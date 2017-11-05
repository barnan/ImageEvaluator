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

        internal CalculateColumnDataEmgu1(ILogger logger, int width, int height)
            : base(logger, width, height)
        {
            ClassName = nameof(CalculateColumnDataEmgu1);
            Title = "Column Mean and Std calculator";

            InitEmguImages();

            _logger?.InfoLog($"Instantiated.", ClassName);
        }


        public override bool Execute(List<NamedData> data, string fileName)
        {
            Image<Gray, ushort>[] rawImages = null;
            Image<Gray, byte>[] maskImages = null;
            BorderPointArrays borderPointarrays = null;

            try
            {
                if (!IsInitialized)
                {
                    _logger?.ErrorLog($"It is not initialized.", ClassName);
                    return false;
                }

                int imageCounter = LoadNamedData(data, ref borderPointarrays, ref rawImages, ref maskImages);
                if (imageCounter == 0)
                {
                    _logger?.InfoLog("No images were loaded from dynamicresult", ClassName);
                }

                double[] meanOfBrightnessMean = new double[imageCounter];
                double[] stdOfBrightnessMean = new double[imageCounter];
                double[] meanOfBrightnessStd = new double[imageCounter];
                double[] stdOfBrightnessStd = new double[imageCounter];
                double[] brightnessHomogeneity1 = new double[imageCounter];
                double[] brightnessHomogeneity2 = new double[imageCounter];
                double[] minOfBrightnessMean = new double[imageCounter];
                double[] maxOfBrightnessMean = new double[imageCounter];

                for (int m = 0; m < imageCounter; m++)
                {
                    int[] indexes = Iterate(rawImages[m], maskImages[m], borderPointarrays[m]);

                    if (indexes == null || indexes.Length != 2)
                    {
                        _logger?.InfoLog($"Problem during Iterate. Return indexes are not proper for further calculation. index:{m}", ClassName);
                    }
                    int indexMin = indexes[0];
                    int indexMax = indexes[1];

                    if (!CalculateStatistics(indexMin, indexMax, maskImages[m]))
                    {
                        _logger?.InfoLog($"Problem during statistics calculation: {m}", ClassName);
                        continue;
                    }

                    meanOfBrightnessMean[m] = _meanOfFirst.V0;
                    stdOfBrightnessMean[m] = _stdOfFirst.V0;
                    meanOfBrightnessStd[m] = _meanOfSecond.V0;
                    stdOfBrightnessStd[m] = _stdOfSecond.V0;
                    brightnessHomogeneity1[m] = Math.Max(Math.Abs(_meanOfRegion2OfFirst.V0 - _meanOfRegion1OfFirst.V0), Math.Abs(_meanOfRegion2OfFirst.V0 - _meanOfRegion3OfFirst.V0));
                    brightnessHomogeneity2[m] = Math.Abs(_meanOfRegion1OfFirst.V0 - _meanOfRegion3OfFirst.V0);
                    minOfBrightnessMean[m] = _minOfFirst.V0;
                    maxOfBrightnessMean[m] = _maxOfFirst.V0;

                }

                data.Add(new DoubleVectorNamedData(meanOfBrightnessMean, "MeanOfBrightnessMean", "MeanOfBrightnessMean"));
                data.Add(new DoubleVectorNamedData(stdOfBrightnessMean, "StdOfMean", "StdOfBrightnessMean"));
                data.Add(new DoubleVectorNamedData(meanOfBrightnessStd, "MeanOfStd", "MeanOfBrightnessStd"));
                data.Add(new DoubleVectorNamedData(stdOfBrightnessStd, "StdOfStd", "StdOfBrightnessStd"));
                data.Add(new DoubleVectorNamedData(brightnessHomogeneity1, "BrightnessHomogeneity1", "BrightnessHomogeneity1"));
                data.Add(new DoubleVectorNamedData(brightnessHomogeneity2, "BrightnessHomogeneity2", "BrightnessHomogeneity2"));
                data.Add(new DoubleVectorNamedData(minOfBrightnessMean, "MinOfMean", "MinOfBrightnessMean"));
                data.Add(new DoubleVectorNamedData(maxOfBrightnessMean, "MaxOfMean", "MAxOfBrightnessMean"));

                return true;
            }
            catch (Exception ex)
            {
                _logger?.ErrorLog($"Exception occured: {ex}", ClassName);
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



        protected int[] Iterate(Image<Gray, ushort> rawImage, Image<Gray, byte> maskImage, int[,] pointArray)
        {
            try
            {
                ReAllocateEmgu();
                double[,,] resultVector1 = _firstVector.Data;
                double[,,] resultVector2 = _secondVector.Data;

                if (!CheckInputData(rawImage, maskImage, pointArray, _firstVector, _secondVector))
                {
                    _logger?.InfoLog("Input and mask data is not proper!", ClassName);
                    return null;
                }

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
            catch (Exception ex)
            {
                _logger?.ErrorLog($"Exception occured: {ex}", ClassName);
                throw;
            }
        }


    }


    public class FactoryCalculateColumnDataEmgu1 : IColumnDataCalculator_Creator
    {
        public IColumnDataCalculator Factory(ILogger logger, int width, int height)
        {
            logger?.InfoLog($"Factory called.", nameof(FactoryCalculateColumnDataEmgu1));

            return new CalculateColumnDataEmgu1(logger, width, height);
        }
    }


}
