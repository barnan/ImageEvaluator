using ImageEvaluatorInterfaces;
using ImageEvaluatorInterfaces.BaseClasses;
using NLog;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace ImageEvaluator.EvaluationProcessor
{
    internal abstract class EvaluationProcessorBase : IEvaluationProcessor
    {
        protected readonly ILogger _logger;
        protected Stopwatch _watch1;
        protected bool _isInitialized;
        protected List<NamedData> _dynamicResult;


        public abstract bool Execute();
        public abstract bool Init();

        public bool IsInitialized { get; protected set; }

        protected EvaluationProcessorBase(ILogger logger)
        {
            _logger = logger;
            _watch1 = new Stopwatch();
        }


        protected void CheckInit(bool resu, string message)
        {
            _logger?.Info(resu ? $"Initialization SUCCED: {message}" : $"Initialization FAILED: {message}");
        }

        protected void LogElapsedTime(Stopwatch watch1, string outermessage = null)
        {
            if (watch1 == null)
                return;

            string message = $"{outermessage ?? string.Empty}. Elapsed time: {watch1.ElapsedMilliseconds}";
            _logger?.Trace(message);
            Console.WriteLine(message);

            watch1.Restart();
        }



    }
}
