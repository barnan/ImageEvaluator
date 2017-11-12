
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
        internal CalculateColumnDataCSharp1(ILogger logger, int width, int height)
            : base(logger, width, height)
        {
            ClassName = nameof(CalculateColumnDataCSharp1);
            Title = ClassName;

            lorum = "Brightness";

            _logger?.InfoLog($"Instantiated.", ClassName);
        }


        public override bool Execute(List<NamedData> data, string path)
        {
            Image<Gray, ushort>[] rawImages = null;
            Image<Gray, byte>[] maskImages = null;
            BorderPointArrays borderPointarrays = null;
            string additionalExtensionName = string.Empty;

            try
            {
                if (!IsInitialized)
                {
                    _logger?.ErrorLog($"It is not initialized", ClassName);
                    return false;
                }

                int imageCounter = LoadNamedData(data, ref borderPointarrays, ref rawImages, ref maskImages);
                if (imageCounter == 0 || imageCounter > _enumNames.Length)
                {
                    _logger?.InfoLog("No images were loaded from dynamicresult OR more images arrived than enumNames!", ClassName);
                }

                for (int m = 0; m < imageCounter; m++)
                {
                    if (_enumNames.Length == imageCounter)
                    {
                        additionalExtensionName = _enumNames[m];
                    }

                    int[] indexes = Iterate(rawImages[m], maskImages[m], borderPointarrays[m]);

                    if (indexes == null || indexes.Length != 2)
                    {
                        _logger?.InfoLog($"Problem during Iterate. Return indexes are not proper for further calculation. index:{m}", ClassName);
                    }

                    if (!CalculateStatistics(indexes[0], indexes[1], maskImages[m]))
                    {
                        _logger?.InfoLog($"Problem during statistics calculation: {m}", ClassName);
                        continue;
                    }

                    double homogeneity1 = Math.Max(Math.Abs(_meanOfRegion2OfFirst.V0 - _meanOfRegion1OfFirst.V0), Math.Abs(_meanOfRegion2OfFirst.V0 - _meanOfRegion3OfFirst.V0));
                    double homogeneity2 = Math.Abs(_meanOfRegion1OfFirst.V0 - _meanOfRegion3OfFirst.V0);

                    string lorum = "Brightness";

                    data.Add(new DoubleNamedData(_meanOfFirst.V0, "MeanOf" + lorum + "Mean", "MeanOf" + lorum + "Mean"));
                    data.Add(new DoubleNamedData(_stdOfFirst.V0, "StdOf" + lorum + "Mean", "StdOf" + lorum + "Mean"));
                    data.Add(new DoubleNamedData(_meanOfSecond.V0, "MeanOf" + lorum + "Std", "MeanOf" + lorum + "Std"));
                    data.Add(new DoubleNamedData(_stdOfSecond.V0, "StdOf" + lorum + "Std", "StdOf" + lorum + "Std"));
                    data.Add(new DoubleNamedData(homogeneity1, lorum + "MeanHomogeneity1", lorum + "MeanHomogeneity1"));
                    data.Add(new DoubleNamedData(homogeneity2, lorum + "MeanHomogeneity2", lorum + "MeanHomogeneity2"));
                    data.Add(new DoubleNamedData(_minOfFirst.V0, "MinOf" + lorum + "Mean", "MinOf" + lorum + "Mean"));
                    data.Add(new DoubleNamedData(_maxOfFirst.V0, "MaxOf" + lorum + "Mean", "MaxOf" + lorum + "Mean"));

                }

                return true;
            }
            catch (Exception ex)
            {
                _logger?.ErrorLog($"Exception occured: {ex}.", ClassName);
                return false;
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
                    _logger?.InfoLog($"Input and mask data is not proper!", ClassName);
                }

                int imageWidth = rawImage.Width;
                int indexMin = int.MaxValue;
                int indexMax = int.MinValue;
                ushort[,,] imgData = rawImage.Data;

                for (int i = 0; i < pointArray.Length / 2; i++)
                {

                    if (pointArray[i, 0] > 0 && pointArray[i, 1] < imageWidth && (pointArray[i, 1] - pointArray[i, 0]) < imageWidth)
                    {
                        double sum = 0;
                        double sum2 = 0;
                        int counter = 0;

                        for (int j = 0; j < (pointArray[i, 1] - pointArray[i, 0]); j++)
                        {
                            sum += imgData[i, pointArray[i, 0] + j, 0];
                            counter++;
                        }

                        resultVector1[0, i, 0] = (float)(sum / counter);
                        counter = 0;

                        for (int j = 0; j < (pointArray[i, 1] - pointArray[i, 0]); j++)
                        {
                            sum2 += Math.Pow(imgData[i, pointArray[i, 0] + j, 0] - resultVector1[0, i, 0], 2);
                            counter++;
                        }

                        resultVector2[i, 0, 0] = (float)Math.Sqrt(sum2 / (counter - 1));

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



    public class FactoryCalculateColumnDataCSharp1 : IColumnDataCalculator_Creator
    {
        public IColumnDataCalculator Factory(ILogger logger, int width, int height)
        {
            logger?.InfoLog($"Factory called.", nameof(FactoryCalculateColumnDataEmgu1));

            return new CalculateColumnDataCSharp1(logger, width, height);
        }
    }


}
