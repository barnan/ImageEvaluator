using System;
using ImageEvaluatorInterfaces;
using NLog;
using System.IO;

namespace ImageEvaluator.MethodManager
{
    internal abstract class MethodManagerBase : IMethodManager
    {
        protected Logger _logger;
        protected IEvaluationProcessor _evaluationProcessor;
        protected string[] _inputPaths;
        protected int _pathIndex;


        public MethodManagerBase(string[] paths)
        {
            _inputPaths = paths;
        }


        private int? CheckInput()
        {
            if (_inputPaths == null)
                return null;

            for (int i = 0; i < _inputPaths.Length; i++)
            {
                if (Directory.Exists(_inputPaths[i]))
                {
                    _logger?.Trace($"InputDirectory checking: {_inputPaths[i]}{Environment.NewLine}Directory is existing.");
                    return i;
                }
            }

            _logger?.Trace("InputDirectory array did not contain valid directory path.");
            return null;
        }


        public bool Init()
        {
            int? resu = CheckInput();
            if (resu != null)
            {
                _pathIndex = (int)resu;
                Instantiate();
            }
            else
            {
                return IsInitialized = false;
            }

            return IsInitialized = true;
        }

        public bool IsInitialized { get; protected set; }


        public bool Execute()
        {
            if (!IsInitialized)
            {
                _logger?.Info("Methodmanager is not initialized properly!!!");
                return false;
            }

            return _evaluationProcessor.Execute();
        }


        public abstract bool Instantiate();

    }
}
