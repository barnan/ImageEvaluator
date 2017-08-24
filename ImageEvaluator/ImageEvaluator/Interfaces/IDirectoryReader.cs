using Emgu.CV;
using Emgu.CV.Structure;
using NLog;

namespace ImageEvaluator.Interfaces
{
    public interface IDirectoryReader : IInitalizable
    {
        bool GetNextImage(ref Image<Gray, float> img1, ref Image<Gray, float> img2, ref string name);

        bool Restart();

        bool EndOfDirectory();
    }


    /// <summary>
    ///  Creator
    /// </summary>
    public interface IDirectoryReader_Creator
    {
        IDirectoryReader Factory(ILogger logger, string directoryName, string extension, IDoubleLightImageReader reader);
    }


}
