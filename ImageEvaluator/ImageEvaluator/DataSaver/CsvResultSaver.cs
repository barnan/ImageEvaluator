using System;
using System.IO;
using System.Reflection;
using Emgu.CV;
using Emgu.CV.Structure;
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
        /// <returns></returns>
        public override bool SaveResult(IMeasurementResult result)
        {
            try
            {
                Type t = result.GetType();

                PropertyInfo[] props = t.GetProperties();
                foreach (PropertyInfo prop in props)
                {
                    object obj = prop.GetValue(result);

                    (obj as Image<Gray, float>)?.Save(Path.Combine(_outputFolder, $"{_prefix}_{prop.Name}_{DateTime.Now.ToString("MMdd_HH_mm")}.png"));

                    _logger?.Trace("Image " + prop.Name + " " + ((obj is Image<Gray, float>) ? string.Empty : "NOT") + " saved.");
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
