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
        private Image<Gray, byte> _lineSegment1;
        private Image<Gray, byte> _lineSegment2;



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

                    for (int i = 0; i < pointArray.Length / 2; i++)
                    {
                        if (pointArray[i, 0] > 0 && pointArray[i, 1] < imageWidth && (pointArray[i, 1] - pointArray[i, 0]) < imageWidth)
                        {

                            MCvScalar noiseMean = new MCvScalar();
                            MCvScalar noisestd = new MCvScalar();

                            Rectangle r1 = new Rectangle(pointArray[i, 0], i, pointArray[i, 1] - pointArray[i, 0], 1);
                            Rectangle r2 = new Rectangle(pointArray[i, 0] + 1, i, pointArray[i, 1] - pointArray[i, 0], 1);

                            rawImages[m].ROI = r1;
                            using (_lineSegment1 = rawImages[m].Copy())
                            {
                                rawImages[m].ROI = r2;
                                using (_lineSegment2 = rawImages[m].Copy())
                                {
                                    using (Image<Gray, byte> tempImage = _lineSegment1 - _lineSegment2)
                                    {
                                        CvInvoke.MeanStdDev(tempImage, ref noiseMean, ref noisestd);

                                        _resultVector1[0, i, 0] = (float)noiseMean.V0;
                                        _resultVector2[0, i, 0] = (float)noisestd.V0;
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
                            _resultVector1[0, i, 0] = 0.0f;
                            _resultVector2[0, i, 0] = 0.0f;
                        }
                    }

                    if (!CalculateStatistics(indexMin, indexMax, maskImages[m]))
                    {
                        return false;
                    }

                    resu1 = _meanOfMean.V0;
                    resu2 = _stdOfMean.V0;
                    resu3 = _meanOfStd.V0;
                    resu4 = _stdOfStd.V0;

                }

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
