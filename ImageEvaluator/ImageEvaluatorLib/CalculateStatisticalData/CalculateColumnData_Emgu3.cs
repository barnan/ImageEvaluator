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

            lorum = "Noise";

            _logger?.InfoLog("Instantiated.", ClassName);
        }


        protected override int[] Iterate(Image<Gray, ushort> rawImage, Image<Gray, byte> maskImage, int[,] pointArray)
        {
            try
            {
                ReAllocateEmgu();
                double[,,] resultVector1 = _firstVector.Data;
                double[,,] resultVector2 = _secondVector.Data;

                if (!CheckInputData(rawImage, maskImage, pointArray, _firstVector, _secondVector))
                {
                    _logger?.InfoLog($"Input and mask data is not proper!", ClassName);
                    return null;
                }

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


    public class FactoryCalculateColumnDataEmgu3 : IColumnDataCalculator_Creator
    {
        public IColumnDataCalculator Factory(ILogger logger, int width, int height)
        {
            logger?.InfoLog($"Factory called.", nameof(FactoryCalculateColumnDataEmgu3));

            return new CalculateColumnDataEmgu3(logger, width, height);
        }
    }


}
