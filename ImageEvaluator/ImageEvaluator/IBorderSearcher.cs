using Emgu.CV;
using Emgu.CV.Structure;
using NLog;

namespace ImageEvaluator
{
    public interface IBorderSearcher : IInitalizable
    {
        bool Run(Image<Gray, byte> maskImage, ref int[,] pointList);
    }

    interface IBorderSeracher_Creator
    {
        IBorderSearcher Factory(ILogger logger, int borderSkip, int imageHeight, bool showImages);
    }



}
