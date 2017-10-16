using Emgu.CV;
using ImageEvaluatorInterfaces;
using NLog;

namespace ImageEvaluatorLib.ThresholdCalculator
{
    class HistogramThresholdCalculatorCSharp1 : HistogramThresholdCalculatorBase
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

        public HistogramThresholdCalculatorCSharp1(ILogger logger, int histogramSize, int param) 
            : base(logger, histogramSize)
        {
            Param = param;
        }
        

        public override bool Run(DenseHistogram hist, out float minPos)
        {
            minPos = Param;

            return true;
        }
    }


    public class FactoryHistogramThresholdCalculatorCSharp1 : IHistogramThresholdCalculator_Creator
    {
        public IHistogramThresholdCalculator Factory(ILogger logger, int range, int param = 0)
        {
            return new HistogramThresholdCalculatorCSharp1(logger, range, param);
        }
    }
}
