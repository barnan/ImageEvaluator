using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageEvaluator
{
    interface IImagePreProcessor
    {

        bool PreProcessImage(Image<Gray, float> inputImage, ref Image<Gray, byte> maskImage);

    }
}
