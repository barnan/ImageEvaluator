using Emgu.CV;
using Emgu.CV.Structure;
using NLog;

namespace ImageEvaluatorInterfaces
{
    public interface IColumnDataCalculator : IInitalizable
    {
        //bool GetStatisticalData(Image<Gray, float> inputImage, Image<Gray, byte> maskimage, int[,] pointArray, ref Image<Gray, float> meanVector, ref Image<Gray, float> stdVector);

        bool Run(Image<Gray, ushort> inputImage, Image<Gray, byte> maskImage, int[,] pointArray, ref Image<Gray, float> meanVector, ref Image<Gray, float> stdVector);
    }



    public interface IColumnDataCalculator_Creator
    {
        IColumnDataCalculator Factory(ILogger logger, int width, int height);
    }


}
