using System.Collections.Generic;
using Emgu.CV;
using Emgu.CV.Structure;

namespace ImageEvaluatorInterfaces
{
    public interface IColumnMeasurementResult : IMeasurementResult
    {
        Image<Gray, double> ColumnMeanVector { get; set; }
        Image<Gray, double> ColumnStdVector { get; set; }
    }


    public interface IRegionStatisticalMeasurementResult : IMeasurementResult
    {
        List<double> RegionMeanVector { get; set; }
        List<double> RegionStdVector { get; set; }
    }


    public interface IColumnStatisticalMeasurementResult : IMeasurementResult
    {
        double ColumnMeanMean { get; set; }
        double ColumnMeanStd { get; set; }
        double ColumnStdMean { get; set; }
        double ColumnStdStd { get; set; }
        double ColumnHomogenity1 { get; set; }
        double ColumnHomogenity2 { get; set; }
        double MeanOfNoise { get; set; }
        double StdOfNoise { get; set; }
        double MeanOfNoiseStd { get; set; }
        double StdOfNoiseStd { get; set; }
        double LeftLineSpread { get; set; }
        double TopLineSpread { get; set; }
        double RightLineSpread { get; set; }
        double BottomLineSpread { get; set; }


        List<double> RegionMeanVector { get; set; }
        List<double> RegionStdVector { get; set; }
        List<double> RegionNoiseMeanVector { get; set; }
        List<double> RegionNoiseStdVector { get; set; }
        //Image<Gray, double> ColumnMeanVector { get; set; }
        //Image<Gray, double> ColumnStdVector { get; set; }
    }


    public interface IMeasurementResult
    {
        string Name { get; set; }
    }



    public interface IAllMeasurementResult
    {
        string Name { get; set; }
        Image<Gray, double> RegionMeanVector { get; set; }
        Image<Gray, double> RegionStdVector { get; set; }
    }


}
