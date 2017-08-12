using Emgu.CV;
using Emgu.CV.Structure;

namespace ImageEvaluator
{
    public interface IDirectoryReader : IInitalizable
    {

        Image<Gray, float> GetNextImage();


    }
}
