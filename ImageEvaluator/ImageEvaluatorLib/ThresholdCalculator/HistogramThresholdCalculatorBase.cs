using Emgu.CV;
using ImageEvaluatorInterfaces;
using NLog;

namespace ImageEvaluatorLib.ThresholdCalculator
{
    internal abstract class HistogramThresholdCalculatorBase : IHistogramThresholdCalculator
    {
        public bool IsInitialized { get; protected set; }
        protected float[] Hist { get; set; }
        protected int HistogramSize { get; }
        protected ILogger Logger { get; }


        protected HistogramThresholdCalculatorBase(ILogger logger, int histogramSize)
        {
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
