using NLog;

namespace ImageEvaluatorLib.ReadImage
{

    internal abstract class SimpleLightImageReader_Base : ImageReaderBase
    {

        protected SimpleLightImageReader_Base(ILogger logger, int width, int height, bool showImages)
            : base(width, height, logger, showImages, 1)
        {
        }


    }
}