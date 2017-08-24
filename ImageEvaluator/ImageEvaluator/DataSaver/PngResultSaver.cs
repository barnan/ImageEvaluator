using Emgu.CV;
using Emgu.CV.Structure;
using NLog;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace ImageEvaluator.DataSaver
{
    class PngResultSaver : ResultSaver_Base
    {

        public PngResultSaver(string outputFolder, string prefix, ILogger logger)
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
                var nameProp = props.Where(p => p.Name == "Name").ToArray();
                string name = (nameProp != null && nameProp.Length> 0) ? (string) nameProp[0].GetValue(result) : string.Empty ;

                foreach (PropertyInfo prop in props)
                {
                    object obj = prop.GetValue(result);

                    string finalOutputName = Path.Combine(_outputFolder, $"{_prefix}_{name}_{prop.Name}_{DateTime.Now.ToString("MMdd_HH_mm")}.png");
                    (obj as Image<Gray, float>)?.Save(finalOutputName);

                    _logger?.Trace("Image " + prop.Name + " " + ((obj is Image<Gray, float>) ? string.Empty : "NOT") + " saved.");
                }
            }
            catch (Exception ex)
            {
                _logger?.Error($"Exception in PngResultSaver-SaveResult: {ex.Message}");
                return false;
            }

            return true;
        }

    }


    class Factory_PngResultSaver : IResultSaver_Creator
    {
        public IResultSaver Factory(string outputFolder, string prefix, ILogger logger)
        {
            return new PngResultSaver(outputFolder, prefix, logger);

        }
    }

}
