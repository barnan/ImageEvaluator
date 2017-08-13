using Emgu.CV;
using Emgu.CV.Structure;

namespace ImageEvaluator
{
    public interface IDirectoryReader : IInitalizable

    {
        Image<Gray, float> GetNextImage();

        bool Restart();
    }


    public interface IDirectoryReader_Creator
    {
        IDirectoryReader Factory(string directoryName, string extension, IDoubleLightImageReader_Creator reader, int width, int bitDepth);
    }


}
