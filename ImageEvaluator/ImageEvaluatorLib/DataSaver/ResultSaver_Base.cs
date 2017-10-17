using System;
using System.IO;
using ImageEvaluatorInterfaces;
using NLog;

namespace ImageEvaluatorLib.DataSaver
{
    internal abstract class ResultSaverBase : IResultSaver
    {
        protected readonly string _prefix;
        protected ILogger _logger;
        private string _outputFoler;


        protected ResultSaverBase(string outputFolder, string prefix, ILogger logger)
        {
            OutputFolder = outputFolder;
            _prefix = prefix;
            _logger = logger;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool Init()
        {
            IsInitialized = CheckDir();
            _logger?.Info("ResultSaver is " + (IsInitialized ? string.Empty : "NOT") + " initialized.");
            return IsInitialized;
        }

        public bool IsInitialized { get; protected set; }

        
        public string OutputFolder { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private bool CheckDir()
        {
            if (!Directory.Exists(OutputFolder))
            {
                _logger?.Trace($"Directory {OutputFolder} doesn't exist -> it will be created.");

                try
                {
                    Directory.CreateDirectory(OutputFolder);
                }
                catch (Exception ex)
                {
                    _logger?.Error($"Exception during ResultSaver-CheckDir: {ex}");
                    return IsInitialized = false;
                }

            }

            return IsInitialized = true;
        }


        public abstract bool SaveResult(IMeasurementResult result, string inputfilename);


    }
}
