
namespace ImageEvaluator.CalculateStatisticalData
{
    abstract class CalculateColumnData_Base_Emgu : CalculateColumnData_Base
    {

        protected int _imageWidth;
        protected float[,,] _resultVector1;
        protected float[,,] _resultVector2;


        protected abstract void InitEmguImages(int width, int height);

        protected abstract void ClearEmguImages();

        protected abstract bool Init(int width, int height);
    }
}
