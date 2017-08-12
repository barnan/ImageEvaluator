using Emgu.CV;
using Emgu.CV.Structure;

namespace ImageEvaluator
{
    public interface IDirectoryReader
    {

        Image<Gray, float> GetNextImage();


    }
}
