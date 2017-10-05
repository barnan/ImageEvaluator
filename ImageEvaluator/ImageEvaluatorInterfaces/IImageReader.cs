using Emgu.CV;
using Emgu.CV.Structure;
using NLog;

namespace ImageEvaluatorInterfaces
{
    public interface IDoubleLightImageReader : IInitalizable
    {
        bool GetImage(string fileName, ref Image<Gray, float> img1, ref Image<Gray, float> img2);
    }



    public interface IDoubleLightImageReader_Creator
    {
        IDoubleLightImageReader Factory(ILogger logger, int width, bool showImages);
    }



}
