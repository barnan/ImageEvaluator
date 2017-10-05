using System;
using System.Diagnostics;
using ImageEvaluatorInterfaces;
using NLog;

namespace ImageEvaluator.MethodManager
{
    public abstract class MethodManagerBase : IMethodManager
    {
        protected readonly ILogger _logger;
        protected Stopwatch _watch1;

        public abstract bool Run();

        public abstract bool Init();


        protected MethodManagerBase(ILogger logger)
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
