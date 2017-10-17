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

namespace ImageEvaluatorLib.DataSaver
{
    internal class CsvColumnStatisticalResultSaver : ResultSaverBase
    {

        public CsvColumnStatisticalResultSaver(string outputFolder, string prefix, ILogger logger)
            : base(outputFolder, prefix, logger)
        {
            _logger?.Info($"{typeof(CsvColumnStatisticalResultSaver)} instantiated.");
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
                string fileNameBase = Path.GetFileNameWithoutExtension(inputFileName);
                string finalStatOutputName = Path.Combine(OutputFolder, $"{_prefix}.csv");

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
                                sw.Write(prop.Name + ",");
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
                                    sw.Write(prop.Name + ",");
                                }
                            }
                        }
                    }

                }

                //foreach (PropertyInfo prop in propList)
                //{
                //    object obj = prop.GetValue(result);

                //    if (obj is Image<Gray, double>)
                //    {
                //        string finalOutputName = Path.Combine(OutputFolder, $"{fileNameBase}_{_prefix}_{prop.Name}.csv");

                //        using (StreamWriter sw = new StreamWriter(finalOutputName))
                //        {
                //            double[,,] data = (obj as Image<Gray, double>).Data;

                //            for (int i = 0; i < data?.Length; i++)
                //            {
                //                sw.WriteLine(data[i, 0, 0]);
                //            }
                //        }
                //        _logger?.Trace("Image " + finalOutputName + " saved.");
                //    }
                //}


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
                                sw.Write((obj ?? string.Empty) + ",");
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                _logger?.Error($"Exception in CsvResultSaver-SaveResult: {ex}");
                return false;
            }

            return true;
        }
    }


    public class FactoryCsvColumnStatisticalResultSaver : IResultSaver_Creator
    {
        public IResultSaver Factory(string outputFolder, string prefix, ILogger logger)
        {
            logger?.Info($"{typeof(FactoryCsvColumnStatisticalResultSaver)} factory called.");
            return new CsvColumnStatisticalResultSaver(outputFolder, prefix, logger);
        }
    }

}
