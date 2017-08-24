
using NLog;

namespace ImageEvaluator.CalculateStatisticalData
{
    abstract class CalculateColumnDataBase_Emgu : CalculateColumnDataBase
    {

        protected int _imageWidth;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        protected CalculateColumnDataBase_Emgu(ILogger logger, int width, int height)
            : base(logger, width, height)
        {
        }



    }
}
