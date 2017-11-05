using System;
using System.IO;
using ImageEvaluatorInterfaces;
using NLog;
using ImageEvaluatorInterfaces.BaseClasses;
using System.Collections.Generic;
using System.Linq;

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
                if (item.DataType == typeof(double) || item.DataType == typeof(int) || item.DataType == typeof(byte) || item.DataType == typeof(long) || item.DataType == typeof(bool))
                {
                    fileHeader.Add(item.Name);
                }
            }

            if (fileHeader.Count == 0)
            {
                return false;
            }

            int counter = 0;
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(filename);
            string finalFileName = Path.Combine(OutputFolder, fileNameExtension + $"_Result_{counter.ToString("00")}.csv");
            bool compareResu;
            //
            for (int i = 0; i < 10; i++)
            {
                try
                {
                    using (FileStream fs = new FileStream(finalFileName, FileMode.Append, FileAccess.ReadWrite))
                    using (StreamReader sr = new StreamReader(fs))
                    {
                        firstLineElements = new List<string>(sr.ReadLine().Split(','));
                        compareResu = fileHeader.SequenceEqual(firstLineElements);

                        if (!compareResu)
                        {
                            finalFileName = Path.Combine(OutputFolder, fileNameExtension + $"_Result_{(++counter).ToString("00")}.csv");
                        }
                        else
                        {
                            SaveData(data, finalFileName);
                        }
                    }
                }
                catch (Exception)
                {
                    finalFileName = Path.Combine(OutputFolder, fileNameExtension + $"_Result_{(++counter).ToString("00")}.csv");
                }
            }



            return true;
        }



        private bool SaveData(List<NamedData> data, string finalFileName)
        {


        }
    }
}
