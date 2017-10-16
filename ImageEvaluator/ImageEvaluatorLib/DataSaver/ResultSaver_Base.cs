using System;
using System.IO;
using ImageEvaluatorInterfaces;
using NLog;

namespace ImageEvaluatorLib.DataSaver
{
    internal abstract class ResultSaver_Base : IResultSaver
    {
        protected readonly string _outputFolder;
        protected readonly string _prefix;
        protected ILogger _logger;


        protected ResultSaver_Base(string outputFolder, string prefix, ILogger logger)
        {
            _outputFolder = outputFolder;
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


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private bool CheckDir()
        {
            if (!Directory.Exists(_outputFolder))
            {
                _logger?.Trace($"Directory {_outputFolder} doesn't exist -> it will be created.");

                try
                {
                    Directory.CreateDirectory(_outputFolder);
                }
                catch (Exception ex)
                {
                    _logger?.Error($"Exception during ResultSaver-CheckDir: {ex}");
                    return IsInitialized = false;
                }

            }

            return IsInitialized = true;
        }


        public abstract bool SaveResult(IColumnMeasurementResult result, string inputfilename);
        public abstract bool SaveResult(IColumnStatisticalMeasurementResult result, string inputfilename);
        public abstract bool SaveResult(IRegionStatisticalMeasurementResult result, string inputfilename);



    }
}
