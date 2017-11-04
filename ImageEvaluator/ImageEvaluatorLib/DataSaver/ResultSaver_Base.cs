using System;
using System.IO;
using ImageEvaluatorInterfaces;
using NLog;
using ImageEvaluatorInterfaces.BaseClasses;
using System.Collections.Generic;

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


        public abstract bool SaveResult(IMeasurementResult result, string inputfilename, string ext);


        public bool SaveResult(List<NamedData> result, string inputfilename, string ext)
        {





            return true;
        }



    }
}
