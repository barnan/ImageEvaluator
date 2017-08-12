
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using System.Drawing;

namespace ImageEvaluator.CalculateStatisticalData
{
    class CalculateColumnData_Emgu2 : CalculateColumnData_Base_Emgu
    {

        Image<Gray, byte> _complementaryMask;
        Image<Gray, float> _squareImage;
        Image<Gray, float> _squaremean;
        Image<Gray, float> _mean;
        Image<Gray, float> _counter;
        Image<Gray, float> _convertedMask;
        Image<Gray, float> _sq;
        Image<Gray, float> _resu;
        Image<Gray, float> _sqrt;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputImage"></param>
        /// <param name="maskImage"></param>
        /// <param name="meanVector"></param>
        /// <param name="stdVector"></param>
        protected override void CalculateStatistics(Image<Gray, float> inputImage, Image<Gray, byte> maskImage, Image<Gray, float> meanVector, Image<Gray, float> stdVector)
        {
            float[,,] resultVector1 = meanVector.Data;
            float[,,] resultVector2 = stdVector.Data;

            // mask the outer area to zero
            Gray zero = new Gray(0.0);
            _complementaryMask = maskImage.Not();
            inputImage.SetValue(zero, _complementaryMask);

            //calculate square image
            _squareImage = inputImage.Pow(2);

            // reduce the sum2 (sum)
            _squaremean = new Image<Gray, float>(new Size(1, inputImage.Height));
            CvInvoke.Reduce(_squareImage, _squaremean, ReduceDimension.Auto, ReduceType.ReduceSum);

            // reduce the sum image (sum)
            _mean = new Image<Gray, float>(new Size(1, inputImage.Height));
            CvInvoke.Reduce(inputImage, _mean, ReduceDimension.Auto, ReduceType.ReduceSum);

            // reduce the sum image (sum)
            _counter = new Image<Gray, float>(new Size(1, inputImage.Height));
            _convertedMask = maskImage.Convert<Gray, float>();
            CvInvoke.Reduce(_convertedMask, _counter, ReduceDimension.Auto, ReduceType.ReduceSum);

            //Image<Gray, float> counterReciprok = counter.Pow(-1);

            CvInvoke.Divide(_squaremean, _counter, _squaremean);
            CvInvoke.Divide(_mean, _counter, _mean);

            //squaremean._Mul(counterReciprok);
            //mean._Mul(counterReciprok);

            // calculate   Math.Sqrt(sum2 / (counter-1) - (sum * sum / Math.Pow(counter-1,2)));
            _sq = _mean.Pow(2);
            _resu = _squaremean - _sq;
            _sqrt = _resu.Pow(0.5);


            float[,,] meanData = _mean.Data;
            float[,,] resuData = _sqrt.Data;

            for (int i = 0; i < _mean.Height; i++)
            {
                resultVector1[i, 0, 0] = meanData[i, 0, 0];
                resultVector2[i, 0, 0] = resuData[i, 0, 0];
            }
        }


        protected override bool CheckInputData(Image<Gray, float> inputImage, Image<Gray, byte> maskImage, int[,] pointArray, Image<Gray, float> meanVector, Image<Gray, float> stdVector)
        {
            base.CheckInputData(inputImage, maskImage, pointArray, meanVector, stdVector);

            if (maskImage == null || maskImage.Height < 0 || maskImage.Height > 10000 || maskImage.Width < 0 || maskImage.Width > 10000)
            {
                return false;
            }
            if (inputImage.Height != maskImage.Height || inputImage.Width != maskImage.Width)
            {
                return false;
            }

            return true;
        }


        protected override void InitEmguImages(int width, int height)
        {
            _complementaryMask = new Image<Gray, byte>(width, height);
            _squareImage = new Image<Gray, float>(width, height);
            _squaremean = new Image<Gray, float>(width, height);
            _mean = new Image<Gray, float>(width, height);
            _counter = new Image<Gray, float>(width, height);
            _convertedMask = new Image<Gray, float>(width, height);
            _sq = new Image<Gray, float>(width, height);
            _resu = new Image<Gray, float>(width, height);
            _sqrt = new Image<Gray, float>(width, height);
        }


        protected override void ClearEmguImages()
        {
            _complementaryMask?.Dispose();
            _squareImage?.Dispose();
            _squaremean?.Dispose();
            _mean?.Dispose();
            _counter?.Dispose();
            _convertedMask?.Dispose();
            _sq?.Dispose();
            _resu?.Dispose();
            _sqrt?.Dispose();
        }

    }
}
