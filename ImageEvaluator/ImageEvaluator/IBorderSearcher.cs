using Emgu.CV;
using Emgu.CV.Structure;
using NLog;

namespace ImageEvaluator
{
    public interface IBorderSearcher
    {
        bool Run(Image<Gray, byte> maskImage, ref int[,] pointList, ref string message);
    }

    interface IBorderSeracher_Creator
    {
        IBorderSearcher Factory(ILogger logger, int borderSkip, bool showImages);
    }



}
