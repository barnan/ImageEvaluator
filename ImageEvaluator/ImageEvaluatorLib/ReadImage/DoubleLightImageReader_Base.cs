using NLog;

namespace ImageEvaluatorLib.ReadImage
{
    internal abstract class DoubleLightImageReader_Base : ImageReaderBase
    {

        protected DoubleLightImageReader_Base(ILogger logger, int width, int height, bool showImages)
            : base(width, height, logger, showImages, 2)
        {
            ClassName = nameof(DoubleLight16BitImageReader);
            Title = ClassName;
        }


    }
}
