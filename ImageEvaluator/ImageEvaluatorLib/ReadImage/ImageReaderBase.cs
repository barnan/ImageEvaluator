using Emgu.CV;
using Emgu.CV.Structure;
using ImageEvaluatorInterfaces;
using NLog;

namespace ImageEvaluatorLib.ReadImage
{
    internal abstract class ImageReaderBase : IImageReader
    {

        protected int _width;
        protected int _height;
        protected int _bitNumber;
        protected ILogger _logger;
        protected bool _showImages;


        protected ImageReaderBase(int width, int height, ILogger logger, bool showImages)
        {
            _width = width;
            _height = height;
            _logger = logger;
            _showImages = showImages;
        }


        protected abstract bool ClearEmguImages();

        protected abstract bool ResetImageROI();

        protected abstract bool InitEmguImages();

        protected abstract bool CheckWidthData();

        public abstract bool Init();


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected abstract bool ReadImage();


        public bool IsInitialized { get; protected set; }


        public abstract bool GetImage(string inputfileName, ref Image<Gray, ushort> img1, ref Image<Gray, ushort> img2);


        protected abstract bool CheckFileName(string inputfileName);


    }
}
