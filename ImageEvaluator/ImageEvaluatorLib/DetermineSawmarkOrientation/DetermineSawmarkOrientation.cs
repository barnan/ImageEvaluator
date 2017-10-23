using System.Threading;
using Emgu.CV;
using Emgu.CV.Structure;
using ImageEvaluatorInterfaces;
using NLog;
using System.Collections.Generic;
using ImageEvaluatorInterfaces.BaseClasses;
using ImageEvaluatorLib.BaseClasses;

namespace ImageEvaluatorLib.DetermineSawmarkOrientation
{
    internal class DetermineSawmarkOrientation : NamedDataProvider, ISawmarkDeterminer
    {
        private IWaferOrientationDetector det;
        private ILogger _logger;

        public DetermineSawmarkOrientation(ILogger logger, IWaferOrientationDetector det)
        {
            this.det = det;
            this._logger = logger;
        }


        public bool Init()
        {
            det.Init();
            return IsInitialized = det.IsInitialized;
        }


        public bool IsInitialized { get; protected set; }


        public bool Run(List<NamedData> data, string name)
        {
            Image<Gray, byte>[] rawImages = null;

            if (!IsInitialized)
            {
                _logger.Error($"{this.GetType().Name} is not initialized.");
                return false;
            }

            rawImages = GetEmguByteImages("_rawImages", data);
            int imageCounterRaw = rawImages?.Length ?? 0;


            for (int m = 0; m < imageCounterRaw; m++)
            {
                byte[] input = rawImages[m].Bytes; //ReadDoubleLightImage(name);

                CancellationToken token = new CancellationToken();

                det.Run(input, token, 1.3, 10, 4, 50);
            }

            return true;
        }
    }


    public class Factory_DetermineSawmarkOrientation : ISawmarkDeterminer_Creator
    {
        public ISawmarkDeterminer Factory(ILogger logger, IWaferOrientationDetector detector)
        {
            logger?.Info($"{typeof(Factory_DetermineSawmarkOrientation).ToString()} factory called.");
            return new DetermineSawmarkOrientation(logger, detector);
        }
    }
}
