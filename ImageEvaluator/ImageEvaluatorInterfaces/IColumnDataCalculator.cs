using Emgu.CV;
using Emgu.CV.Structure;
using NLog;

namespace ImageEvaluatorInterfaces
{
    public interface IColumnDataCalculator : IInitalizable
    {
        //bool GetStatisticalData(Image<Gray, float> inputImage, Image<Gray, byte> maskimage, int[,] pointArray, ref Image<Gray, float> meanVector, ref Image<Gray, float> stdVector);

        bool Run(Image<Gray, byte> inputImage, Image<Gray, byte> maskImage, int[,] pointArray, ref Image<Gray, double> meanVector, ref Image<Gray, double> stdVector,
                out double resu1, out double resu2, out double resu3, out double resu4, out double resu5, out double resu6, out double resu7, out double resu8,
                out double resu9, out double resu10);
    }



    public interface IColumnDataCalculator_Creator
    {
        IColumnDataCalculator Factory(ILogger logger, int width, int height);
    }


}
