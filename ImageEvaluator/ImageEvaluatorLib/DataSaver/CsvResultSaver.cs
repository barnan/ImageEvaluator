using System;
using System.IO;
using System.Reflection;
using Emgu.CV;
using Emgu.CV.Structure;
using ImageEvaluatorInterfaces;
using NLog;

namespace ImageEvaluatorLib.DataSaver
{
    class CsvResultSaver : ResultSaver_Base
    {

        public CsvResultSaver(string outputFolder, string prefix, ILogger logger)
            : base(outputFolder, prefix, logger)
        {
            _logger?.Info($"{typeof(CsvResultSaver)} instantiated.");
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        /// <param name="inputFileName"></param>
        /// <returns></returns>
        public override bool SaveResult(IColumnMeasurementResult result, string inputFileName)
        {
            try
            {
                Type t = result.GetType();

                PropertyInfo[] props = t.GetProperties();
                foreach (PropertyInfo prop in props)
                {
                    object obj = prop.GetValue(result);

                    if (!(obj is Image<Gray, float>))
                        continue;

                    string fileNameBase = Path.GetFileNameWithoutExtension(inputFileName);
                    string finalOutputName = Path.Combine(_outputFolder, $"{fileNameBase}_{_prefix}_{prop.Name}.csv");


                    using (StreamWriter sw = new StreamWriter(finalOutputName))
                    {
                        float[,,] data = (obj as Image<Gray, float>)?.Data;

                        for (int i = 0; i < data?.Length; i++)
                        {
                            sw.WriteLine(data[i, 0, 0]);
                        }
                    }
                    _logger?.Trace("Image " + finalOutputName + " saved.");
                }
            }
            catch (Exception ex)
            {
                _logger?.Error($"Exception in CsvResultSaver-SaveResult: {ex}");
                return false;
            }

            return true;
        }

        public override bool SaveResult(IColumnStatisticalMeasurementResult result, string inputfilename)
        {
            throw new NotImplementedException();
        }

        public override bool SaveResult(IRegionStatisticalMeasurementResult result, string inputfilename)
        {
            throw new NotImplementedException();
        }
    }


    public class FactoryCsvResultSaver : IResultSaver_Creator
    {
        public IResultSaver Factory(string outputFolder, string prefix, ILogger logger)
        {
            logger?.Info($"{typeof(FactoryCsvResultSaver).ToString()} factory called.");
            return new CsvResultSaver(outputFolder, prefix, logger);

        }

    }


}
