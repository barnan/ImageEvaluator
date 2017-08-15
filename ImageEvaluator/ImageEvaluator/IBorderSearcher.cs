using Emgu.CV;
using Emgu.CV.Structure;

namespace ImageEvaluator
{
    public interface IBorderSearcher
    {
        bool GetBorderPoints(Image<Gray, byte> maskImage, ref int[,] pointList, ref string message);
    }

    interface IBorderSeracher_Creator
    {
        IBorderSearcher Factory(int borderSkip, bool showImages);
    }



}
