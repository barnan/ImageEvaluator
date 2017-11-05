using System;
using Emgu.CV;
using ImageEvaluatorInterfaces;
using NLog;
using ImageEvaluatorInterfaces.BaseClasses;

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
            ClassName = nameof(HistogramThresholdCalculatorEmgu2);
            Title = ClassName;

            Logger?.InfoLog("Instantiated.", ClassName);
        }


        public override bool Execute(DenseHistogram inputHistogram, out float pos)
        {
            pos = 0;

            if (!IsInitialized)
            {
                Logger?.InfoLog("It is not initialized yet.", ClassName);
                return false;
            }


            if (inputHistogram == null)
            {
                Logger.TraceLog("InputHistogramm is null!", ClassName);
                return false;
            }
            try
            {
                return true;
            }
            catch (Exception ex)
            {
                Logger.ErrorLog($"Exception occured: {ex}", ClassName);
                return false;
            }
        }

    }



    public class FactoryHistogramThresholdCalculatorEmgu2 : IHistogramThresholdCalculator_Creator
    {
        public IHistogramThresholdCalculator Factory(ILogger logger, int range, int param = 0)
        {
            logger?.InfoLog("Factory called.", nameof(FactoryHistogramThresholdCalculatorEmgu2));

            return new HistogramThresholdCalculatorEmgu2(logger, range, param);
        }
    }




}
