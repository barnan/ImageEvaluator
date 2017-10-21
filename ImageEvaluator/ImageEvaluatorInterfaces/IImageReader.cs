using Emgu.CV;
using Emgu.CV.Structure;
using NLog;

namespace ImageEvaluatorInterfaces
{
    public interface IImageReader : IInitalizable
    {
        bool GetImage(string fileName, ref Image<Gray, byte>[] img1);
    }


    public interface IImageReader_Creator
    {
        IImageReader Factory(ILogger logger, int width, int height, bool showImages);
    }


}
