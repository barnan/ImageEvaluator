using System;
using Emgu.CV;
using ImageEvaluatorInterfaces;
using NLog;

namespace ImageEvaluatorLib.ThresholdCalculator
{

    /// <summary>
    /// Histogram quantile based threshold calculator -> TODO
    /// </summary>
    class HistogramThresholdCalculatorEmgu2 : HistogramThresholdCalculatorBase
    {

        private int _param;
        public int Param
        {
            get { return _param; }
            set
            {
                if (value < 0)
                {
                    _param = 0;
                }
                else if (value >= HistogramSize)
                {
                    _param = HistogramSize - 1;
                }
                else
                {
                    _param = value;
                }

            }
        }


        public HistogramThresholdCalculatorEmgu2(ILogger logger, int histogramSize, int param = 0)
            : base(logger, histogramSize)
        {
            Logger?.Info($"{GetType().Name} instantiated.");
        }


        public override bool Execute(DenseHistogram inputHistogram, out float pos)
        {
            pos = 0;

            if (!IsInitialized)
            {
                Logger?.Info($"Error during {GetType().Name} Execute - Not initialized yet.");
                return false;
            }


            if (inputHistogram == null)
            {
                Logger.Trace($"{GetType().Name} inputHistogramm is null!");
                return false;
            }
            try
            {
                return true;
            }
            catch (Exception ex)
            {
                Logger.Error($"Exception occured during {GetType().Name} histogram calculation: {ex}");
                return false;
            }
        }

    }



    public class FactoryHistogramThresholdCalculatorEmgu2 : IHistogramThresholdCalculator_Creator
    {
        public IHistogramThresholdCalculator Factory(ILogger logger, int range, int param = 0)
        {
            logger?.Info($"{GetType().Name} factory called.");
            return new HistogramThresholdCalculatorEmgu2(logger, range, param);
        }
    }




}
