using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageEvaluator
{
    public interface IColumnDataCalculator
    {
        void GetStatisticalData(Image<Gray, float> inputImage, Image<Gray, byte> maskimage, int[,] pointArray, Image<Gray, float> meanVector, Image<Gray, float> stdVector);


    }
}
