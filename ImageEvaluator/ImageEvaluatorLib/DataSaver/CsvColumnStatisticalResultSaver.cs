using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using Emgu.CV;
using Emgu.CV.Structure;
using ImageEvaluatorInterfaces;
using NLog;
using ImageEvaluatorInterfaces.BaseClasses;

namespace ImageEvaluatorLib.DataSaver
{
    internal class CsvColumnStatisticalResultSaver : ResultSaverBase
    {

        public CsvColumnStatisticalResultSaver(string outputFolder, string prefix, ILogger logger)
            : base(outputFolder, prefix, logger)
        {
            ClassName = nameof(CsvColumnStatisticalResultSaver);
            Title = ClassName;

            _logger?.InfoLog("Instantiated.", ClassName);
        }


        public override bool SaveResult(IMeasurementResult result, string inputFileName, string ext)
        {
            try
            {
                string fileNameBase = Path.GetFileNameWithoutExtension(inputFileName);
                string finalStatOutputName = Path.Combine(OutputFolder, $"{_prefix}_{ext}.csv");

                Type t = result.GetType();
                List<PropertyInfo> propLis = t.GetProperties().ToList();
                List<PropertyInfo> propList = new List<PropertyInfo>();

                foreach (var prop in propLis)
                {
                    object obj = prop.GetValue(result);
                    if (!(obj is Image<Gray, double>))
                    {
                        propList.Add(prop);
                    }
                }
                List<string> propString = propList.Select(a => a.Name).ToList();


                if (!File.Exists(finalStatOutputName))
                {
                    using (FileStream fs = new FileStream(finalStatOutputName, FileMode.Create))
                    {
                        using (StreamWriter sw = new StreamWriter(fs))
                        {
                            foreach (PropertyInfo prop in propList)
                            {
                                sw.Write(prop.Name);
                                if (propList.IndexOf(prop) != propList.Count - 1)
                                {
                                    sw.Write(",");
                                }
                            }
                        }
                    }
                }
                else
                {
                    List<string> existingProps;

                    using (FileStream fs = new FileStream(finalStatOutputName, FileMode.Open))
                    {
                        using (StreamReader sr = new StreamReader(fs))
                        {
                            string text = sr.ReadLine();
                            existingProps = text?.Split(',').ToList() ?? new List<string>();
                        }
                    }

                    if (!propString.SequenceEqual(existingProps))
                    {
                        using (FileStream fs = new FileStream(finalStatOutputName, FileMode.Append))
                        {
                            using (StreamWriter sw = new StreamWriter(fs))
                            {
                                sw.Write(Environment.NewLine);
                                foreach (PropertyInfo prop in propList)
                                {
                                    sw.Write(prop.Name);

                                    if (propList.IndexOf(prop) != propList.Count - 1)
                                    {
                                        sw.Write(",");
                                    }
                                }
                            }
                        }
                    }

                }


                using (FileStream fs = new FileStream(finalStatOutputName, FileMode.Append))
                {
                    using (StreamWriter sw = new StreamWriter(fs))
                    {
                        sw.Write(Environment.NewLine);

                        foreach (PropertyInfo prop in propList)
                        {
                            object obj = prop.GetValue(result);
                            if (!(obj is Image<Gray, double>))
                            {
                                sw.Write((obj ?? string.Empty));

                                if (propList.IndexOf(prop) != propList.Count - 1)
                                {
                                    sw.Write(",");
                                }
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                _logger?.ErrorLog($"Exception occured: {ex}", ClassName);
                return false;
            }

            return true;
        }
    }


    public class FactoryCsvColumnStatisticalResultSaver : IResultSaver_Creator
    {
        public IResultSaver Factory(string outputFolder, string prefix, ILogger logger)
        {
            logger?.InfoLog("Factory called.", nameof(FactoryCsvColumnStatisticalResultSaver));

            return new CsvColumnStatisticalResultSaver(outputFolder, prefix, logger);
        }
    }

}
