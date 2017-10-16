using System.Collections.Generic;
using Emgu.CV;
using Emgu.CV.Structure;

namespace ImageEvaluatorInterfaces
{
    public interface IColumnMeasurementResult
    {
        string Name { get; set; }
        Image<Gray, float> ColumnMeanVector { get; set; }
        Image<Gray, float> ColumnStdVector { get; set; }
    }


    public interface IRegionStatisticalMeasurementResult
    {
        string Name { get; set; }
        List<float> RegionMeanVector { get; set; }
        List<float> RegionStdVector { get; set; }
    }


    public interface IColumnStatisticalMeasurementResult
    {
        string Name { get; set; }
        float ColumnMeanMeanVector { get; set; }
        float ColumnMeanStdVector { get; set; }
        float ColumnStdMeanVector { get; set; }
        float ColumnStdStdVector { get; set; }
        float RegionNoiseMeanVector { get; set; }
        float RegionNoiseStdVector { get; set; }

    }


    public interface IAllMeasurementResult
    {
        string Name { get; set; }
        Image<Gray, float> RegionMeanVector { get; set; }
        Image<Gray, float> RegionStdVector { get; set; }
    }


}
