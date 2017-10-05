using System.Threading;
using NLog;

namespace ImageEvaluatorInterfaces
{

    public enum WaferOrientation
    {
        NormalWaferOrientation = 0,
        RotatedWafer
    }


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

