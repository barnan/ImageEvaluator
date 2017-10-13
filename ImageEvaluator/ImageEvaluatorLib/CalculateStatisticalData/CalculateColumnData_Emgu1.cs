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
        public override bool Run(Image<Gray, float> inputImage, Image<Gray, byte> maskImage, int[,] pointArray, ref Image<Gray, float> meanVector, ref Image<Gray, float> stdVector)
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

            for (int i = 0; i < pointArray.Length / 2; i++)
            {
                if (pointArray[i, 0] > 0 && pointArray[i, 1] < imageWidth && (pointArray[i, 1] - pointArray[i, 0]) < imageWidth)
                {
                    Rectangle r = new Rectangle(pointArray[i, 0], i, pointArray[i, 1] - pointArray[i, 0], 1);

                    MCvScalar mean = new MCvScalar();
                    MCvScalar std = new MCvScalar();

                    inputImage.ROI = r;
                    CvInvoke.MeanStdDev(inputImage, ref mean, ref std);

                    _resultVector1[i, 0, 0] = (float)mean.V0;
                    _resultVector2[i, 0, 0] = (float)std.V0;
                }
            }

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
        protected override bool CheckInputData(Image<Gray, float> inputImage, Image<Gray, byte> maskImage, int[,] pointArray, Image<Gray, float> meanVector, Image<Gray, float> stdVector)
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
