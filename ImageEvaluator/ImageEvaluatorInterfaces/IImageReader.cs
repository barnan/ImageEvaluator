using Emgu.CV;
using Emgu.CV.Structure;
using NLog;

namespace ImageEvaluatorInterfaces
{
    public interface IImageReader : IInitalizable
    {
        bool GetImage(string fileName, ref Image<Gray, ushort> img1, ref Image<Gray, ushort> img2);
    }


    public interface IDoubleLightImageReader_Creator
    {
        IImageReader Factory(ILogger logger, int width, bool showImages);
    }


    public interface ISimpleLightImageReader_Creator
    {
        IImageReader Factory(ILogger logger, int width, bool showImages);
    }


}
