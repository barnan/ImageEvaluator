
namespace ImageEvaluator.CalculateStatisticalData
{
    class CalculateColumnData_Base_Emgu : CalculateColumnData_Base
    {

        protected int _imageWidth;
        protected float[,,] _resultVector1;
        protected float[,,] _resultVector2;


        protected virtual void InitEmguImages(int width, int height)
        { }

        protected virtual void ClearEmguImages()
        { }


    }
}
