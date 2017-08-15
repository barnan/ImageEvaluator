using Emgu.CV;
using Emgu.CV.Structure;

namespace ImageEvaluator
{
    public interface IDirectoryReader

    {
        bool GetNextImage(ref Image<Gray, float> img1, ref Image<Gray, float> img2, ref string message);

        bool Restart();

        bool EndOfDirectory();
    }


    public interface IDirectoryReader_Creator
    {
        IDirectoryReader Factory(string directoryName, string extension, IDoubleLightImageReader_Creator reader, int width, int bitDepth);
    }


}
