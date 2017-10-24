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

        Image<Gray, byte> _lineSegment1;
        Image<Gray, byte> _lineSegment2;


        public CalculateColumnDataEmgu3(ILogger logger, int width, int height)
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
                        _logger.Info($"{this.GetType().Name} - problem during IterateNoise. Return indexes are not proper for further calculation.");
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
                                using (Image<Gray, byte> tempImage = _lineSegment1 - _lineSegment2)
                                {
                                    CvInvoke.MeanStdDev(tempImage, ref noiseMean, ref noisestd);

                                    resultVector1[0, i, 0] = (float)noiseMean.V0;
                                    resultVector2[0, i, 0] = (float)noisestd.V0;
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
                _logger.Error($"Exception during {this.GetType().Name} - IterateNoise.");
                throw;
            }
        }




        //private bool CalculateStatistics(int indexMin, int indexMax)
        //{
        //    _meanOfMean = new MCvScalar();
        //    _stdOfMean = new MCvScalar();
        //    _meanOfStd = new MCvScalar();
        //    _stdOfStd = new MCvScalar();

        //    _meanOfNoiseRegion1 = new MCvScalar();
        //    _meanOfNoiseRegion2 = new MCvScalar();
        //    _meanOfNoiseRegion3 = new MCvScalar();

        //    MCvScalar stdOfRegion1 = new MCvScalar();
        //    MCvScalar stdOfRegion2 = new MCvScalar();
        //    MCvScalar stdOfRegion3 = new MCvScalar();

        //    try
        //    {
        //        Rectangle rect1 = new Rectangle(indexMin, 0, indexMax - indexMin, 1);
        //        Rectangle fullRoi = new Rectangle(0, 0, _meanVector.Width, 1);

        //        _meanVector.ROI = rect1;
        //        _stdVector.ROI = rect1;

        //        CvInvoke.MeanStdDev(_meanVector, ref _meanOfMean, ref _stdOfMean);
        //        CvInvoke.MeanStdDev(_stdVector, ref _meanOfStd, ref _stdOfStd);

        //        int regionWidth = (indexMax - indexMin) / 5;
        //        Rectangle rect3 = new Rectangle(indexMin, 0, regionWidth, 1);
        //        Rectangle rect4 = new Rectangle(indexMin + 2 * regionWidth, 0, regionWidth, 1);
        //        Rectangle rect5 = new Rectangle(indexMin + 4 * regionWidth, 0, regionWidth, 1);

        //        _meanVector.ROI = rect3;
        //        CvInvoke.MeanStdDev(_stdVector, ref _meanOfNoiseRegion1, ref stdOfRegion1);

        //        _meanVector.ROI = rect4;
        //        CvInvoke.MeanStdDev(_stdVector, ref _meanOfNoiseRegion2, ref stdOfRegion2);

        //        _meanVector.ROI = rect5;
        //        CvInvoke.MeanStdDev(_stdVector, ref _meanOfNoiseRegion3, ref stdOfRegion3);

        //        _meanVector.ROI = fullRoi;
        //        _stdVector.ROI = fullRoi;

        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger?.Error($"Exception occured during CalculateColumnDataEmgu1 - CalculateStatistics: {ex}");
        //        return false;
        //    }
        //}


    }


    /// <summary>
    /// 
    /// </summary>
    public class FactoryCalculateColumnDataEmgu3 : IColumnDataCalculator_Creator
    {
        public IColumnDataCalculator Factory(ILogger logger, int width, int height)
        {
            logger?.Info($"{typeof(FactoryCalculateColumnDataEmgu3)} factory called.");
            return new CalculateColumnDataEmgu3(logger, width, height);
        }
    }


}
