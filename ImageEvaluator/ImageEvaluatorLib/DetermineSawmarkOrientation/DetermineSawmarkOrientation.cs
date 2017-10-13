using System.Threading;
using Emgu.CV;
using Emgu.CV.Structure;
using ImageEvaluatorInterfaces;
using NLog;

namespace ImageEvaluatorLib.DetermineSawmarkOrientation
{
    public class DetermineSawmarkOrientation : ISawmarkDeterminer
    {
        private IWaferOrientationDetector det;
        private bool _isInitialized;

        public DetermineSawmarkOrientation(IWaferOrientationDetector det)
        {
            this.det = det;
        }


        public bool Init()
        {
            det.Init();
            return _isInitialized = det.IsInitialized;
        }


        public bool IsInitialized => _isInitialized;


        public void Run(Image<Gray, float> image, string name)
        {
            byte[] input = image.Bytes; //ReadDoubleLightImage(name);

            CancellationToken token = new CancellationToken();

            det.Run(input, token, 1.3, 10, 4, 50);
        }
    }


    public class Factory_DetermineSawmarkOrientation : ISawmarkDeterminer_Creator
    {
        public ISawmarkDeterminer Factory(ILogger logger, IWaferOrientationDetector detector)
        {
            logger?.Info($"{typeof(Factory_DetermineSawmarkOrientation).ToString()} factory called.");
            return new DetermineSawmarkOrientation(detector);
        }
    }
}
