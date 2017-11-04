
using NLog;

namespace ImageEvaluatorLib.CalculateStatisticalData
{
    abstract class CalculateColumnDataBaseEmgu : CalculateColumnDataBase
    {

        protected int _imageWidth;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        protected CalculateColumnDataBaseEmgu(ILogger logger, int width, int height)
            : base(logger, width, height)
        {
            ClassName = nameof(CalculateColumnDataBase);
            Title = ClassName;
        }



    }
}
