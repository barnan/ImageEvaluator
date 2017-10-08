using Emgu.CV;
using Emgu.CV.Structure;
using ImageEvaluatorInterfaces;

namespace ImageEvaluatorLib.DataSaver
{
    public class ColumnStatisticMeasurementResult : IMeasurementResult
    {
        public string Name { get; set; }

        public Image<Gray, float> MeanVector { get; set; }
        public Image<Gray, float> StdVector { get; set; }
    }
}
