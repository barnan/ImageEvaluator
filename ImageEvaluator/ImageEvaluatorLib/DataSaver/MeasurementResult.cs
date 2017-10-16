using System;
using System.Collections.Generic;
using Emgu.CV;
using Emgu.CV.Structure;
using ImageEvaluatorInterfaces;

namespace ImageEvaluatorLib.DataSaver
{



    public class RegionStatisticMeasurementResult : IRegionStatisticalMeasurementResult
    {
        public string Name { get; set; }

        public List<float> RegionMeanVector { get; set; }

        public List<float> RegionStdVector { get; set; }
    }



    public class ColumnStatisticalMeasurementResult : IColumnStatisticalMeasurementResult
    {
        public string Name { get; set; }

        public float ColumnMeanMeanVector { get; set; }

        public float ColumnMeanStdVector { get; set; }

        public float ColumnStdMeanVector { get; set; }

        public float ColumnStdStdVector { get; set; }

        public float RegionNoiseMeanVector { get; set; }

        public float RegionNoiseStdVector { get; set; }
    }


    public class ColumnMeasurementResult : IColumnMeasurementResult
    {
        public string Name { get; set; }

        public Image<Gray, float> ColumnMeanVector { get; set; }
        public Image<Gray, float> ColumnStdVector { get; set; }
    }
}
