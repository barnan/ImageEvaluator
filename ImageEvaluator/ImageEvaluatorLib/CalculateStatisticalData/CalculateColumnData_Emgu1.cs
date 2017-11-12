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

            lorum = "Brightness";

            _logger?.InfoLog($"Instantiated.", ClassName);
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
