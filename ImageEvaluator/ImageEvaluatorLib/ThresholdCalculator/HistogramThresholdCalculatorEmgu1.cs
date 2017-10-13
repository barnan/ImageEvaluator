using System;
using Emgu.CV;
using ImageEvaluatorInterfaces;
using NLog;

namespace ImageEvaluatorLib.ThresholdCalculator
{
    class HistogramThresholdCalculatorEmgu1 : HistogramThresholdCalculatorBase
    {

        public HistogramThresholdCalculatorEmgu1(ILogger logger, int histogramSize, int param = 0)
            : base(logger, histogramSize)
        {
            Logger?.Info($"{typeof(HistogramThresholdCalculatorEmgu1)} instantiated.");
        }


        public override bool Run(DenseHistogram inputHistogram, out float minPos)
        {
            minPos = 0;

            if (!IsInitialized)
            {
                return false;
            }

            if (inputHistogram == null)
            {
                Logger.Trace("ThresholdCalculator_Emgu2 inputHistogramm is null!");
                return false;
            }

            Hist = inputHistogram.GetBinValues();

            try
            {
                float[,] mean = new float[4096, 2];
                float[,] deviation = new float[4096, 2];
                float[,] num = new float[256, 2];
                float min = 100000;
                for (int nt = 5; nt < 256 - 5; nt++)
                {
                    for (int n = 0; n < 256; n++)
                    {
                        if (n < nt)
                        {
                            num[nt, 0] += Hist[n];
                            mean[nt, 0] += n*Hist[n];
                            deviation[nt, 0] += n*n*Hist[n];
                        }
                        else
                        {
                            num[nt, 1] += Hist[n];
                            mean[nt, 1] += n*Hist[n];
                            deviation[nt, 1] += n*n*Hist[n];
                        }
                    }
                    mean[nt, 0] = mean[nt, 0]/num[nt, 0];
                    deviation[nt, 0] = (float) Math.Sqrt(deviation[nt, 0]/num[nt, 0] - mean[nt, 0]*mean[nt, 0]);
                    mean[nt, 1] = mean[nt, 1]/num[nt, 1];
                    deviation[nt, 1] = (float) Math.Sqrt(deviation[nt, 1]/num[nt, 1] - mean[nt, 1]*mean[nt, 1]);

                    if (min > deviation[nt, 0] + deviation[nt, 1])
                    {
                        min = deviation[nt, 0] + deviation[nt, 1];
                        minPos = nt;
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                Logger?.Error($"Exception occured during HistogramThresholdCalculator_Emgu2 histogramcalculation: {ex}");
                return false;
            }
        }

    }


    public class FactoryHistogramThresholdCalculatorEmgu1 : IHistogramThresholdCalculator_Creator
    {
        public IHistogramThresholdCalculator Factory(ILogger logger, int range, int param = 0)
        {
            logger?.Info($"{typeof(FactoryHistogramThresholdCalculatorEmgu1)} factory called.");
            return new HistogramThresholdCalculatorEmgu1(logger, range, param);
        }
    }



}
