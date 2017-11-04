using System;
using System.IO;
using System.Reflection;
using Emgu.CV;
using Emgu.CV.Structure;
using ImageEvaluatorInterfaces;
using NLog;
using ImageEvaluatorInterfaces.BaseClasses;
using System.Collections.Generic;

namespace ImageEvaluatorLib.DataSaver
{
    class CsvColumnResultSaver : ResultSaverBase
    {

        public CsvColumnResultSaver(string outputFolder, string prefix, ILogger logger)
            : base(outputFolder, prefix, logger)
        {
            ClassName = nameof(CsvColumnResultSaver);
            Title = ClassName;

            _logger?.InfoLog("Instantiated.", ClassName);
        }


        public override bool SaveResult(IMeasurementResult result, string inputFileName, string ext)
        {
            try
            {
                Type t = result.GetType();

                PropertyInfo[] props = t.GetProperties();
                foreach (PropertyInfo prop in props)
                {
                    object obj = prop.GetValue(result);

                    if (!(obj is Image<Gray, double>))
                        continue;

                    string fileNameBase = Path.GetFileNameWithoutExtension(inputFileName);
                    string finalOutputName = Path.Combine(OutputFolder, "LineScans", $"{fileNameBase}_{_prefix}_{ext}_{prop.Name}.csv");

                    if (!Directory.Exists(Path.GetDirectoryName(finalOutputName)))
                    {
                        Directory.CreateDirectory(Path.GetDirectoryName(finalOutputName));
                    }

                    using (StreamWriter sw = new StreamWriter(finalOutputName))
                    {
                        double[,,] data = (obj as Image<Gray, double>).Data;

                        for (int i = 0; i < data?.Length; i++)
                        {
                            sw.WriteLine(data[0, i, 0]);
                        }
                    }
                    _logger?.Trace("Image " + finalOutputName + " saved.", ClassName);
                }
            }
            catch (Exception ex)
            {
                _logger?.ErrorLog($"Exception in CsvResultSaver-SaveResult: {ex}", ClassName);
                return false;
            }

            return true;
        }

    }


    public class FactoryCsvColumnResultSaver : IResultSaver_Creator
    {
        public IResultSaver Factory(string outputFolder, string prefix, ILogger logger)
        {
            logger?.InfoLog("Factory called.", nameof(FactoryCsvColumnResultSaver));
            return new CsvColumnResultSaver(outputFolder, prefix, logger);
        }

    }


}
