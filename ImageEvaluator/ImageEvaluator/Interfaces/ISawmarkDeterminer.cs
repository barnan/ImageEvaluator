using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV;
using Emgu.CV.Structure;

namespace ImageEvaluator.Interfaces
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
