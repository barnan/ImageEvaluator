using System;
using System.IO;
using NLog;

namespace ImageEvaluator.DataSaver
{
    abstract class ResultSaver_Base : IResultSaver
    {

        protected readonly string _outputFolder;
        protected readonly string _prefix;
        protected bool _initialized;
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
            _initialized = CheckDir();
            _logger?.Info("ResultSaver is " + (_initialized ? string.Empty : "NOT") + " initialized.");
            return _initialized;
        }

        
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
                    _logger?.Error($"Exception during ResultSaver-CheckDir: {ex.Message}");
                    return _initialized = false;
                }

            }

            return _initialized = true;
        }


        public abstract bool SaveResult(IMeasurementResult result, string inputfilename);



    }
}
