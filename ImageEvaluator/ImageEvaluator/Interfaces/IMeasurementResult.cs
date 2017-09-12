using Emgu.CV;
using Emgu.CV.Structure;

namespace ImageEvaluator.Interfaces
{
    public interface IMeasurementResult
    {
        string Name { get; set; }
        Image<Gray, float> MeanVector { get; set; }
        Image<Gray, float> StdVector { get; set; }
    }
}
