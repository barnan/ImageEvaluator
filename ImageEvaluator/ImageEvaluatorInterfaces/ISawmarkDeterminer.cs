using Emgu.CV;
using Emgu.CV.Structure;
using NLog;

namespace ImageEvaluatorInterfaces
{
    public interface ISawmarkDeterminer : IInitalizable
    {
        void Run(Image<Gray, byte> image, string name);
    }



    public interface ISawmarkDeterminer_Creator
    {
        ISawmarkDeterminer Factory(ILogger logger, IWaferOrientationDetector detector);
    }

}
