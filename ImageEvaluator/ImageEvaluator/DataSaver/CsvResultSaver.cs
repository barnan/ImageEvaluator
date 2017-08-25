using System;
using System.IO;
using System.Reflection;
using Emgu.CV;
using Emgu.CV.Structure;
using ImageEvaluator.Interfaces;
using NLog;

namespace ImageEvaluator.DataSaver
{
    class CsvResultSaver : ResultSaver_Base
    {

        public CsvResultSaver(string outputFolder, string prefix, ILogger logger)
            : base(outputFolder, prefix, logger)
        {
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        /// <param name="inputFileName"></param>
        /// <returns></returns>
        public override bool SaveResult(IMeasurementResult result, string inputFileName)
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
                            sw.WriteLine(data[i,0,0]);
                        }
                    }
                    _logger?.Trace("Image " + finalOutputName + " saved.");
                }
            }
            catch (Exception ex)
            {
                _logger?.Error($"Exception in CsvResultSaver-SaveResult: {ex.Message}");
                return false;
            }

            return true;
        }

    }


    class Factory_CsvResultSaver : IResultSaver_Creator
    {
        public IResultSaver Factory(string outputFolder, string prefix, ILogger logger)
        {
            return new CsvResultSaver(outputFolder, prefix, logger);

        }

    }


}
