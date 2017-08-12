using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageEvaluator
{
    public interface IImageReader
    {
        Image<Gray, float> ReadDoubleLightImage();


    }
}
