using Emgu.CV;
using ImageEvaluatorInterfaces;
using NLog;

namespace ImageEvaluatorLib.ThresholdCalculator
{
    internal abstract class HistogramThresholdCalculatorBase : IHistogramThresholdCalculator
    {
        protected float[] Hist { get; set; }
        protected int HistogramSize { get; }
        protected ILogger Logger { get; }

        public string ClassName { get; protected set; }
        public string Title { get; protected set; }
        public bool IsInitialized { get; protected set; }


        protected HistogramThresholdCalculatorBase(ILogger logger, int histogramSize)
        {
            ClassName = nameof(HistogramThresholdCalculatorBase);
            Title = ClassName;

            Logger = logger;
            HistogramSize = histogramSize;
        }


        public bool Init()
        {
            if (HistogramSize < 0 || HistogramSize > 100000)
            {
                return IsInitialized = false;
            }

            Hist = new float[HistogramSize];

            IsInitialized = true;

            Logger?.Info($"{typeof(HistogramThresholdCalculatorBase)}" + (IsInitialized ? string.Empty : " NOT") + " initialized.");

            return IsInitialized;
        }


        public abstract bool Execute(DenseHistogram hist, out float minPos);

    }
}
