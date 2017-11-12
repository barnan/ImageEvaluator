using System;
using System.IO;
using ImageEvaluatorInterfaces;
using NLog;
using ImageEvaluatorInterfaces.BaseClasses;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using System.Text;

namespace ImageEvaluatorLib.DataSaver
{
    internal abstract class ResultSaverBase : IResultSaver, INamedDataResultSaver
    {
        protected readonly string _prefix;
        protected ILogger _logger;
        private string _outputFoler;

        public string ClassName { get; protected set; }
        public string Title { get; protected set; }

        public bool IsInitialized { get; protected set; }
        public string OutputFolder { get; set; }


        protected ResultSaverBase(string outputFolder, string prefix, ILogger logger)
        {
            ClassName = nameof(ResultSaverBase);
            Title = ClassName;

            OutputFolder = outputFolder;
            _prefix = prefix;
            _logger = logger;
        }


        public bool Init()
        {
            IsInitialized = CheckDir();
            _logger?.InfoLog((IsInitialized ? string.Empty : "NOT") + " initialized.", ClassName);
            return IsInitialized;
        }


        private bool CheckDir()
        {
            if (!Directory.Exists(OutputFolder))
            {
                _logger?.TraceLog($"Directory {OutputFolder} doesn't exist -> it will be created.", ClassName);

                try
                {
                    Directory.CreateDirectory(OutputFolder);
                }
                catch (Exception ex)
                {
                    _logger?.ErrorLog($"Exception occured: {ex}", ClassName);
                    return IsInitialized = false;
                }

            }

            return IsInitialized = true;
        }


        public abstract bool SaveResult(IMeasurementResult result, string inputfilename, string fileNameExtension);


        public bool SaveResult(List<NamedData> data, string filename, string fileNameExtension)
        {
            // Determine header:
            List<string> firstLineElements = null;
            List<string> fileHeader = new List<string>();
            foreach (var item in data)
            {
                if (item.DataType == typeof(double) || item.DataType == typeof(int) || item.DataType == typeof(float) || item.DataType == typeof(byte) || item.DataType == typeof(long) || item.DataType == typeof(bool))
                {
                    fileHeader.Add(item.Name);
                }
            }

            if (fileHeader.Count == 0)
            {
                _logger?.InfoLog("Empty header from dynamic result list.", ClassName);
                return false;
            }

            int counter = 0;
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(filename);
            string finalFileName = Path.Combine(OutputFolder, fileNameExtension + $"_{counter.ToString("00")}.csv");
            bool compareResu;
            bool writeHeader = false;
            //
            for (int i = 0; i < 10; i++)
            {
                try
                {
                    using (FileStream fs = new FileStream(finalFileName, FileMode.OpenOrCreate, FileAccess.ReadWrite))
                    using (StreamReader sr = new StreamReader(fs))
                    {
                        string line = sr.ReadLine();

                        if (line == null)
                        {
                            writeHeader = true;
                        }
                        else
                        {
                            firstLineElements = new List<string>(line.Split(','));
                            compareResu = fileHeader.SequenceEqual(firstLineElements);

                            if (!compareResu)
                            {
                                writeHeader = true;
                                finalFileName = Path.Combine(OutputFolder, fileNameExtension + $"_Result_{(++counter).ToString("00")}.csv");
                                continue;
                            }
                        }
                    }

                    SaveData(data, finalFileName, writeHeader, fileHeader);
                    break;
                }
                catch (Exception)
                {
                    _logger?.ErrorLog($"{finalFileName} can not be openned.", ClassName);
                    finalFileName = Path.Combine(OutputFolder, fileNameExtension + $"_Result_{(++counter).ToString("00")}.csv");
                }
            }

            return true;
        }



        private bool SaveData(List<NamedData> data, string finalFileName, bool writeHeader, List<string> fileHeader)
        {

            CultureInfo cultInfo = CultureInfo.InvariantCulture;
            StringBuilder sbHeader = new StringBuilder(fileHeader.Count);
            StringBuilder sbData = new StringBuilder(fileHeader.Count);

            for (int i = 0; i < fileHeader.Count; i++)
            {
                sbHeader.Append(fileHeader[i]);
                AddComa(sbHeader, i, fileHeader.Count);
            }

            for (int i = 0; i < data.Count; i++)
            {
                if (data[i] is DoubleNamedData)
                {
                    sbData.Append((data[i] as DoubleNamedData).Value.ToString(cultInfo));
                    AddComa(sbData, i, data.Count);
                }
                if (data[i] is IntNamedData)
                {
                    sbData.Append((data[i] as IntNamedData).Value);
                    AddComa(sbData, i, data.Count);
                }

                if (data[i] is FloatNamedData)
                {
                    sbData.Append((data[i] as FloatNamedData).Value.ToString(cultInfo));
                    AddComa(sbData, i, data.Count);
                }

                if (data[i] is ByteNamedData)
                {
                    sbData.Append((data[i] as ByteNamedData).Value.ToString(cultInfo));
                    AddComa(sbData, i, data.Count);
                }

                if (data[i] is BooleanNamedData)
                {
                    sbData.Append((data[i] as BooleanNamedData).Value.ToString(cultInfo));
                    AddComa(sbData, i, data.Count);
                }

                if (data[i] is LongNamedData)
                {
                    sbData.Append((data[i] as LongNamedData).Value.ToString(cultInfo));
                    AddComa(sbData, i, data.Count);
                }

            }

            using (FileStream fs = new FileStream(finalFileName, FileMode.Append, FileAccess.Write))
            using (StreamWriter sw = new StreamWriter(fs))
            {
                if (writeHeader)
                {
                    sw.WriteLine(sbHeader.ToString());
                }
                sw.WriteLine(sbData.ToString());
            }

            return true;
        }



        private void AddComa(StringBuilder sb, int i, int count)
        {
            if (i != (count - 1))
            {
                sb.Append(',');
            }
        }


    }
}
