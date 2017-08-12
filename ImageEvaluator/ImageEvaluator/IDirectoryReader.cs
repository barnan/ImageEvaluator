using Emgu.CV;
using Emgu.CV.Structure;

namespace ConsoleApplication1
{
    interface IDirectoryReader : IInitalizable
    {

        Image<Gray, float> GetNextImage();


    }
}
