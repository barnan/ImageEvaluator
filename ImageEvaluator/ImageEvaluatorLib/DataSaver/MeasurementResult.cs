using System.Collections.Generic;
using Emgu.CV;
using Emgu.CV.Structure;
using ImageEvaluatorInterfaces;

namespace ImageEvaluatorLib.DataSaver
{



    public class RegionStatisticMeasurementResult : IRegionStatisticalMeasurementResult
    {
        public string Name { get; set; }

        public List<double> RegionMeanVector { get; set; }

        public List<double> RegionStdVector { get; set; }
    }



    public class ColumnStatisticalMeasurementResult : IColumnStatisticalMeasurementResult
    {
        public string Name { get; set; }
        public double MeanOfMean { get; set; }
        public double MaxOfMean { get; set; }
        public double MinOfMean { get; set; }
        public double MeanOfStd { get; set; }
        public double StdOfMean { get; set; }
        public double StdOfStd { get; set; }
        public double ColumnHomogenity1 { get; set; }
        public double ColumnHomogenity2 { get; set; }
        public double MeanOfNoiseMean { get; set; }
        public double StdOfNoiseMean { get; set; }
        public double MeanOfNoiseStd { get; set; }
        public double StdOfNoiseStd { get; set; }
        public double LeftLineSpread { get; set; }
        public double TopLineSpread { get; set; }
        public double RightLineSpread { get; set; }
        public double BottomLineSpread { get; set; }



        //public Image<Gray, double> ColumnMeanVector { get; set; }
        //public Image<Gray, double> ColumnStdVector { get; set; }

        public List<double> RegionMeanVector { get; set; }

        public List<double> RegionStdVector { get; set; }

        public List<double> RegionNoiseMeanVector { get; set; }

        public List<double> RegionNoiseStdVector { get; set; }
    }


    public class ColumnMeasurementResult : IColumnMeasurementResult
    {
        public string Name { get; set; }

        public Image<Gray, double> ColumnMeanVector { get; set; }
        public Image<Gray, double> ColumnStdVector { get; set; }
    }
}
