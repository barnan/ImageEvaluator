using System;
using Emgu.CV;
using ImageEvaluatorInterfaces;
using NLog;

namespace ImageEvaluatorLib.ThresholdCalculator
{
    class HistogramThresholdCalculatorEmgu2 : HistogramThresholdCalculatorBase
    {

        public HistogramThresholdCalculatorEmgu2(ILogger logger, int histogramSize, int param = 0)
            : base(logger, histogramSize)
        {
            Logger?.Info($"{typeof(HistogramThresholdCalculatorEmgu2)} instantiated.");
        }


        public override bool Run(DenseHistogram inputHistogram, out float pos)
        {
            pos = 0;

            if (!IsInitialized)
            {
                return false;
            }


            if (inputHistogram == null)
            {
                Logger.Trace("ThresholdCalculator_Emgu2 inputHistogramm is null!");
                return false;
            }
            try
            {
                return true;
            }
            catch (Exception ex)
            {
                Logger.Error($"Exception occured during HistogramThresholdCalculator_Emgu1 histogram calculation: {ex}");
                return false;
            }
        }
        
    }



    public class FactoryHistogramThresholdCalculatorEmgu2 : IHistogramThresholdCalculator_Creator
    {
        public IHistogramThresholdCalculator Factory(ILogger logger, int range, int param = 0)
        {
            return new HistogramThresholdCalculatorEmgu2(logger, range, param);
        }
    }




}
