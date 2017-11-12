
using Emgu.CV;
using Emgu.CV.Structure;
using ImageEvaluatorInterfaces.BaseClasses;
using NLog;
using System;
using System.Collections.Generic;

namespace ImageEvaluatorLib.CalculateStatisticalData
{
    abstract class CalculateColumnDataBaseEmgu : CalculateColumnDataBase
    {
        protected int _imageWidth;

        protected CalculateColumnDataBaseEmgu(ILogger logger, int width, int height)
            : base(logger, width, height)
        {
            ClassName = nameof(CalculateColumnDataBase);
            Title = ClassName;
        }


        public override bool Execute(List<NamedData> data, string fileName)
        {
            Image<Gray, ushort>[] rawImages = null;
            Image<Gray, byte>[] maskImages = null;
            BorderPointArrays borderPointarrays = null;
            string additionalExtensionName = string.Empty;

            try
            {
                if (!IsInitialized)
                {
                    _logger?.ErrorLog($"It is not initialized.", ClassName);
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


        protected abstract int[] Iterate(Image<Gray, ushort> rawImages, Image<Gray, byte> maskImages, int[,] borderPointarrays);

    }
}
