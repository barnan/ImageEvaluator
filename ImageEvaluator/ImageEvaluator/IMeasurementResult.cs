using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageEvaluator
{
    interface IMeasurementResult
    {
        Image<Gray, float> MeanVector { get; set; }
        Image<Gray, float> StdVector { get; set; }
    }
}
