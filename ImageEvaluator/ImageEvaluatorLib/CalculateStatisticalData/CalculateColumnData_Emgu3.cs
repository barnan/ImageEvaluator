using System;
using System.Drawing;
using Emgu.CV;
using Emgu.CV.Structure;
using ImageEvaluatorInterfaces;
using NLog;
using System.Collections.Generic;
using ImageEvaluatorInterfaces.BaseClasses;

namespace ImageEvaluatorLib.CalculateStatisticalData
{
    class CalculateColumnDataEmgu3 : CalculateColumnDataBaseEmgu
    {

        Image<Gray, ushort> _lineSegment1;
        Image<Gray, ushort> _lineSegment2;


        public CalculateColumnDataEmgu3(ILogger logger, int width, int height)
            : base(logger, width, height)
        {
            ClassName = nameof(CalculateColumnDataEmgu3);
            Title = "Column Noise calculator";

            InitEmguImages();

            _logger?.InfoLog("Instantiated.", ClassName);
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
                    _logger.ErrorLog("It is not initialized.", ClassName);
                    return false;
                }

                int imageCounter = LoadNamedData(data, ref borderPointarrays, ref rawImages, ref maskImages);
                if (imageCounter == 0)
                {
                    _logger.InfoLog("No images were loaded from dynamicresult", ClassName);
                }


                double[] meanOfNoiseMean = new double[imageCounter];
                double[] stdOfNoiseMean = new double[imageCounter];
                double[] meanOfNoiseStd = new double[imageCounter];
                double[] stdOfNoiseStd = new double[imageCounter];
                double[] noiseMeanHomogeneity1 = new double[imageCounter];
                double[] noiseMeanHomogeneity2 = new double[imageCounter];
                double[] minOfNoiseMean = new double[imageCounter];
                double[] maxOfNoiseMean = new double[imageCounter];


                for (int m = 0; m < imageCounter; m++)
                {

                    int[] indexes = Iterate(rawImages[m], maskImages[m], borderPointarrays[m]);

                    if (indexes == null || indexes.Length != 2)
                    {
                        _logger.InfoLog("Problem during IterateNoise. Return indexes are not proper for further calculation.", ClassName);
                    }
                    int indexMin = indexes[0];
                    int indexMax = indexes[1];

                    if (!CalculateStatistics(indexMin, indexMax, maskImages[m]))
                    {
                        return false;
                    }


                    meanOfNoiseMean[m] = _meanOfMean.V0;
                    stdOfNoiseMean[m] = _stdOfMean.V0;
                    meanOfNoiseStd[m] = _meanOfStd.V0;
                    stdOfNoiseStd[m] = _stdOfStd.V0;
                    noiseMeanHomogeneity1[m] = Math.Max(Math.Abs(_meanOfRegion2.V0 - _meanOfRegion1.V0), Math.Abs(_meanOfRegion2.V0 - _meanOfRegion3.V0));
                    noiseMeanHomogeneity2[m] = Math.Abs(_meanOfRegion1.V0 - _meanOfRegion3.V0);
                    minOfNoiseMean[m] = _minOfMean.V0;
                    maxOfNoiseMean[m] = _maxOfMean.V0;

                }


                data.Add(new DoubleVectorNamedData(meanOfNoiseMean, "meanOfNoiseMean", nameof(meanOfNoiseMean)));
                data.Add(new DoubleVectorNamedData(stdOfNoiseMean, "stdOfNoiseMean", nameof(stdOfNoiseMean)));
                data.Add(new DoubleVectorNamedData(meanOfNoiseStd, "meanOfNoiseStd", nameof(meanOfNoiseStd)));
                data.Add(new DoubleVectorNamedData(stdOfNoiseStd, "stdOfNoiseStd", nameof(stdOfNoiseStd)));
                data.Add(new DoubleVectorNamedData(noiseMeanHomogeneity1, "noiseMeanHomogeneity1", nameof(noiseMeanHomogeneity1)));
                data.Add(new DoubleVectorNamedData(noiseMeanHomogeneity2, "noiseMeanHomogeneity2", nameof(noiseMeanHomogeneity2)));
                data.Add(new DoubleVectorNamedData(minOfNoiseMean, "minOfNoiseMean", nameof(minOfNoiseMean)));
                data.Add(new DoubleVectorNamedData(maxOfNoiseMean, "maxOfNoiseMean", nameof(maxOfNoiseMean)));


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
                if (!CheckInputData(rawImage, maskImage, pointArray, _firstVector, _secondVector))
                {
                    _logger?.InfoLog($"Input and mask data is not proper!", ClassName);
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

                        MCvScalar noiseMean = new MCvScalar();
                        MCvScalar noisestd = new MCvScalar();

                        Rectangle r1 = new Rectangle(pointArray[i, 0], i, pointArray[i, 1] - pointArray[i, 0], 1);
                        Rectangle r2 = new Rectangle(pointArray[i, 0] + 1, i, pointArray[i, 1] - pointArray[i, 0], 1);

                        rawImage.ROI = r1;
                        using (_lineSegment1 = rawImage.Copy())
                        {
                            rawImage.ROI = r2;
                            using (_lineSegment2 = rawImage.Copy())
                            {
                                using (Image<Gray, ushort> tempImage = _lineSegment1 - _lineSegment2)
                                {
                                    CvInvoke.MeanStdDev(tempImage, ref noiseMean, ref noisestd);

                                    resultVector2[0, i, 0] = (float)noisestd.V0;

                                    CvInvoke.AbsDiff(_lineSegment1, _lineSegment2, tempImage);
                                    CvInvoke.MeanStdDev(tempImage, ref noiseMean, ref noisestd);

                                    resultVector1[0, i, 0] = (float)noiseMean.V0;
                                }
                            }
                        }

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
                _logger?.ErrorLog($"Exception occured:", ClassName);
                throw;
            }
        }

    }


    /// <summary>
    /// 
    /// </summary>
    public class FactoryCalculateColumnDataEmgu3 : IColumnDataCalculator_Creator
    {
        public IColumnDataCalculator Factory(ILogger logger, int width, int height)
        {
            logger?.InfoLog($"Factory called.", nameof(FactoryCalculateColumnDataEmgu3));

            return new CalculateColumnDataEmgu3(logger, width, height);
        }
    }


}
