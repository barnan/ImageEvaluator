using Emgu.CV;
using Emgu.CV.Structure;
using NLog;

namespace ImageEvaluatorInterfaces
{
    public interface IDirectoryReader : IInitalizable
    {
        bool GetNextImage(ref Image<Gray, byte> img1, ref Image<Gray, byte> img2, ref string name);

        bool Restart();

        bool EndOfDirectory();
    }


    /// <summary>
    ///  Creator
    /// </summary>
    public interface IDirectoryReader_Creator
    {
        IDirectoryReader Factory(ILogger logger, string directoryName, string extension, IImageReader reader);
    }


}
