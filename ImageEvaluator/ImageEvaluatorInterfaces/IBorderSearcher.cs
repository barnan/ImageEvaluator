using Emgu.CV;
using Emgu.CV.Structure;
using NLog;

namespace ImageEvaluatorInterfaces
{
    public interface IBorderSearcher : IInitalizable
    {
        bool Run(Image<Gray, byte> maskImage, ref int[,] pointList);

    }

    public interface IBorderSeracher_Creator
    {
        IBorderSearcher Factory(ILogger logger, int borderSkip, int imageHeight, bool showImages);
    }



}
