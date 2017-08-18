
using NLog;

namespace ImageEvaluator.CalculateStatisticalData
{
    abstract class CalculateColumnData_Base_Emgu : CalculateColumnData_Base
    {

        protected int _imageWidth;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public CalculateColumnData_Base_Emgu(ILogger logger, int width, int height)
            : base(logger, width, height)
        {
        }



        //protected abstract void InitEmguImages(int width, int height);



        //protected abstract void ClearEmguImages();

    }
}
