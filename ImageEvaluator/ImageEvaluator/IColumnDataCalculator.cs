using Emgu.CV;
using Emgu.CV.Structure;

namespace ImageEvaluator
{
    public interface IColumnDataCalculator
    {
        void GetStatisticalData(Image<Gray, float> inputImage, Image<Gray, byte> maskimage, int[,] pointArray, Image<Gray, float> meanVector, Image<Gray, float> stdVector);

    }
}
