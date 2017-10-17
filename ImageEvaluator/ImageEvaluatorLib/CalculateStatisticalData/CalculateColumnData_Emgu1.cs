using Emgu.CV;
using Emgu.CV.Structure;
using System.Drawing;
using ImageEvaluatorInterfaces;
using NLog;

namespace ImageEvaluatorLib.CalculateStatisticalData
{
    class CalculateColumnDataEmgu1 : CalculateColumnDataBaseEmgu
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        internal CalculateColumnDataEmgu1(ILogger logger, int width, int height)
            : base(logger, width, height)
        {
            InitEmguImages();

            _logger?.Info("CalculateColumnData_Emgu1 instantiated.");
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputImage"></param>
        /// <param name="maskImage"></param>
        /// <param name="pointArray"></param>
        /// <param name="meanVector"></param>
        /// <param name="stdVector"></param>
        /// <param name="resu3"></param>
        /// <param name="resu4"></param>
        /// <param name="resu1"></param>
        /// <param name="resu2"></param>
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
            int indexMin = int.MaxValue;
            int indexMax = int.MinValue;

            for (int i = 0; i < pointArray.Length / 2; i++)
            {
                if (pointArray[i, 0] > 0 && pointArray[i, 1] < imageWidth && (pointArray[i, 1] - pointArray[i, 0]) < imageWidth)
                {
                    Rectangle r = new Rectangle(pointArray[i, 0], i, pointArray[i, 1] - pointArray[i, 0], 1);

                    MCvScalar mean = new MCvScalar();
                    MCvScalar std = new MCvScalar();

                    inputImage.ROI = r;
                    CvInvoke.MeanStdDev(inputImage, ref mean, ref std);

                    _resultVector1[0, i, 0] = (float)mean.V0;
                    _resultVector2[0, i, 0] = (float)std.V0;

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

            MCvScalar meanOfMean = new MCvScalar();
            MCvScalar stdOfMean = new MCvScalar();

            MCvScalar meanOfStd = new MCvScalar();
            MCvScalar stdOfStd = new MCvScalar();

            using (Image<Gray, byte> tempMask1 = new Image<Gray, byte>(_meanVector.Width, _meanVector.Height))
            {
                tempMask1.SetValue(255);

                Rectangle rect1 = new Rectangle(0, 0, indexMin, 1);
                Rectangle rect2 = new Rectangle(indexMax, 0, _meanVector.Size.Width - indexMax -1, 1);

                tempMask1.ROI = rect1;
                tempMask1.SetValue(0);

                tempMask1.ROI = rect2;
                tempMask1.SetValue(0);
                tempMask1.ROI = new Rectangle(0, 0, _meanVector.Width, 1);

                CvInvoke.MeanStdDev(_meanVector, ref meanOfMean, ref stdOfMean, tempMask1);
                CvInvoke.MeanStdDev(_stdVector, ref meanOfStd, ref stdOfStd, tempMask1);
            }

            resu1 = meanOfMean.V0;
            resu2 = stdOfMean.V0;
            resu3 = meanOfStd.V0;
            resu4 = stdOfStd.V0;

            inputImage.ROI = _fullMask;

            return true;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputImage"></param>
        /// <param name="maskImage"></param>
        /// <param name="pointArray"></param>
        /// <param name="meanVector"></param>
        /// <param name="stdVector"></param>
        /// <returns></returns>
        protected override bool CheckInputData(Image<Gray, ushort> inputImage, Image<Gray, byte> maskImage, int[,] pointArray, Image<Gray, double> meanVector, Image<Gray, double> stdVector)
        {
            bool partResu = base.CheckInputData(inputImage, maskImage, pointArray, meanVector, stdVector);

            if (!partResu || pointArray == null || pointArray.Length < 0 || (pointArray.Length / 2) > 10000 || (pointArray.Length / 2) != inputImage.Height)
            {
                _logger?.Error("Error during CheckInputData");
                return false;
            }

            return true;
        }

    }


    /// <summary>
    /// 
    /// </summary>
    public class FactoryCalculateColumnDataEmgu1 : IColumnDataCalculator_Creator
    {
        public IColumnDataCalculator Factory(ILogger logger, int width, int height)
        {
            logger?.Info($"{typeof(FactoryCalculateColumnDataEmgu1)} factory called.");
            return new CalculateColumnDataEmgu1(logger, width, height);
        }
    }


}
