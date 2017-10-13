using ImageEvaluatorInterfaces;

namespace ImageEvaluatorLib.FindCorners
{
    class CornerFinder : ICornerfinder
    {

        public bool Init()
        {
            return true;
        }

        public bool IsInitialized { get; protected set; }
    }
}
