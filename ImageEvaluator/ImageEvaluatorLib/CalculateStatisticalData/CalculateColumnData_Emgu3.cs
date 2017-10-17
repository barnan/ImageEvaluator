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
        private Image<Gray, ushort> _lineSegment1;
        private Image<Gray, ushort> _lineSegment2;

        public CalculateColumnDataEmgu3(ILogger logger, int width, int height) 
            : base(logger, width, height)
        {
            InitEmguImages();

            _logger?.Info("CalculateColumnData_Emgu3 instantiated.");
        }

        public override bool Run(Image<Gray, ushort> inputImage, Image<Gray, byte> maskImage, int[,] pointArray, ref Image<Gray, double> meanVector, ref Image<Gray, double> stdVector,
                                out double resu1, out double resu2, out double resu3, out double resu4)
        {
            resu1 = 0;
            resu2 = 0;
            resu3 = 0;
            resu4 = 0;

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

            for (int i = 0; i < pointArray.Length / 2; i++)
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
                            using (Image<Gray, ushort> tempImage = _lineSegment1 - _lineSegment2)
                            {
                                CvInvoke.MeanStdDev(tempImage, ref noiseMean, ref noisestd);

                                _resultVector1[0, i, 0] = (float) noiseMean.V0;
                                _resultVector2[0, i, 0] = (float) noisestd.V0;
                            }
                        }
                    }

                }
                else
                {
                    _resultVector1[0, i, 0] = 0.0f;
                    _resultVector2[0, i, 0] = 0.0f;
                }
            }

            inputImage.ROI = _fullMask;
            return true;

        }

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
