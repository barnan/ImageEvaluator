using Emgu.CV;
using Emgu.CV.Structure;
using ImageEvaluator.Interfaces;

namespace ImageEvaluator.DataSaver
{
    class MeasurementResult : IMeasurementResult
    {
        public string Name { get; set; }

        public Image<Gray, float> MeanVector { get; set; }
        public Image<Gray, float> StdVector { get; set; }
    }
}
