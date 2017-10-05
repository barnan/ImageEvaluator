using Emgu.CV;
using Emgu.CV.Structure;

namespace ImageEvaluatorInterfaces
{
    public interface ISawmarkDeterminer : IInitalizable
    {
        void Run(Image<Gray, float> image, string name);
    }



    public interface ISawmarkDeterminer_Creator
    {
        ISawmarkDeterminer Factory(IWaferOrientationDetector detector);
    }

}
