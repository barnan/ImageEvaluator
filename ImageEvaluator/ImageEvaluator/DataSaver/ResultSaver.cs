using Emgu.CV;
using Emgu.CV.Structure;
using NLog;
using System;
using System.IO;
using System.Reflection;

namespace ImageEvaluator.DataSaver
{
    class ResultSaver : IResultSaver
    {

        private readonly string _outputFolder;
        private readonly string _prefix;
        private bool _initialized;
        private ILogger _logger;



        /// <summary>
        /// 
        /// </summary>
        /// <param name="folder"></param>
        /// <param name="logger"></param>
        public ResultSaver(string folder, string prefix, ILogger logger)
        {
            _outputFolder = folder;
            _logger = logger;
        }



        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool Init()
        {
            _initialized = CheckDir();
            _logger.Info("ResultSaver is " + (_initialized ? string.Empty : "NOT") + " initialized.");
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




        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public bool SaveResult(IMeasurementResult result)
        {
            try
            {
                Type t = result.GetType();

                PropertyInfo[] props = t.GetProperties();
                foreach (PropertyInfo prop in props)
                {
                    object obj = prop.GetValue(result);

                    (obj as Image<Gray, float>)?.Save($"{_prefix}_{prop.Name}_{DateTime.Now.ToString("MMdd_HH_mm")}.png");

                    _logger.Trace("Image " + prop.Name + " " + ((obj is Image<Gray, float>) ? string.Empty : "NOT") + " saved.");
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"Exception in ResultSaver-SaveResult: {ex.Message}");
                return false;
            }

            return true;
        }



    }
}
