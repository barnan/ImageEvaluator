using Emgu.CV;
using Emgu.CV.Structure;

namespace ImageEvaluator
{
    public interface IBorderSearcher
    {

        int[,] GetBorderPoints(Image<Gray, byte> maskImage, ref string message);

    }
}
