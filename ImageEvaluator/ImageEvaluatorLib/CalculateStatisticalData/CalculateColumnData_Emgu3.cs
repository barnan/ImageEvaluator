using System;
using System.Drawing;
using Emgu.CV;
using Emgu.CV.Structure;
using ImageEvaluatorInterfaces;
using NLog;

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

            _logger?.Info("CalculateColumnData_Emgu3 instantiated.");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputImage"></param>
        /// <param name="maskImage"></param>
        /// <param name="pointArray"></param>
        /// <param name="meanVector"></param>
        /// <param name="stdVector"></param>
        /// <param name="resu1"></param>
        /// <param name="resu2"></param>
        /// <param name="resu3"></param>
        /// <param name="resu4"></param>
        /// <param name="resu5"></param>
        /// <param name="resu6"></param>
        /// <returns></returns>
        public override bool Run(Image<Gray, byte> inputImage, Image<Gray, byte> maskImage, int[,] pointArray, ref Image<Gray, double> meanVector, ref Image<Gray, double> stdVector,
            out double resu1, out double resu2, out double resu3, out double resu4, out double resu5, out double resu6, out double resu7, out double resu8,
            out double resu9, out double resu10)
        {
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

                meanVector = _meanVector;
                stdVector = _stdVector;

                if (!CheckInputData(inputImage, maskImage, pointArray, meanVector, stdVector))
                {
                    return false;
                }

                int imageWidth = inputImage.Width;
                int indexMin = int.MaxValue;
                int indexMax = int.MinValue;

                for (int i = 0; i < pointArray.Length/2; i++)
                {
                    if (pointArray[i, 0] > 0 && pointArray[i, 1] < imageWidth && (pointArray[i, 1] - pointArray[i, 0]) < imageWidth)
                    {

                        MCvScalar noiseMean = new MCvScalar();
                        MCvScalar noisestd = new MCvScalar();

                        Rectangle r1 = new Rectangle(pointArray[i, 0], i, pointArray[i, 1] - pointArray[i, 0], 1);
                        Rectangle r2 = new Rectangle(pointArray[i, 0] + 1, i, pointArray[i, 1] - pointArray[i, 0], 1);

                        inputImage.ROI = r1;
                        using (_lineSegment1 = inputImage.Copy())
                        {
                            inputImage.ROI = r2;
                            using (_lineSegment2 = inputImage.Copy())
                            {
                                using (Image<Gray, byte> tempImage = _lineSegment1 - _lineSegment2)
                                {
                                    CvInvoke.MeanStdDev(tempImage, ref noiseMean, ref noisestd);

                                    _resultVector1[0, i, 0] = (float) noiseMean.V0;
                                    _resultVector2[0, i, 0] = (float) noisestd.V0;
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

                if (!CalculateStatistics(indexMin, indexMax, maskImage))
                {
                    return false;
                }

                resu1 = _meanOfMean.V0;
                resu2 = _stdOfMean.V0;
                resu3 = _meanOfStd.V0;
                resu4 = _stdOfStd.V0;
                
                return true;
            }
            catch (Exception ex)
            {
                _logger?.Error($"Exception occured during CalculateColumnDataEmgu1 - Run: {ex}");
                return false;
            }
            finally
            {
                inputImage.ROI = _fullROI;
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
