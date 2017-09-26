using System.Threading;
using ImageEvaluator.DetermineSawmarkOrientation;
using NLog;

namespace ImageEvaluator.Interfaces
{
    public interface IWaferOrientationDetector : IInitalizable
    {
        WaferOrientation? Run(byte[] inputRawImage, CancellationToken cancelToken, double orientationThresholdInAdu, int sectionWidthInPixel, int lowerSpatialLimitInPixel,
            int upperSpatialLimitInPixel);
    }


    public interface IWaferOrientationDetector_Creator
    {
        IWaferOrientationDetector Factory(ILogger logger, int originalImageWidth, int originalImageHeight, int linescanHeightStartInPixel, int linescanHeightEndInPixel);
    }

}

