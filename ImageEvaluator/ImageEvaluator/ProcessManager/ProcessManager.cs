using Emgu.CV;
using Emgu.CV.Structure;

namespace ImageEvaluator.ManageProcess
{

    class ProcessManager : IProcessManager
    {
        IDirectoryReader _dirReader;
        IImagePreProcessor _preProc;
        IBorderSearcher _borderSearcher;
        IColumnDataCalculator _calcColumnData;


        IDirectoryReader_Creator _dirReaderFactory;
        IDoubleLightImageReader_Creator _imageReaderFactory;
        IImagePreProcessor_Creator _preProcFactory;
        IBorderSeracher_Creator _borderFactory;
        IColumnDataCalculator_Creator _colummnCalculatorFactory;


        Image<Gray, float> _image1;
        Image<Gray, float> _image2;
        Image<Gray, byte> _mask1;
        Image<Gray, byte> _mask2;
        int[,] _borderPoints1;
        int[,] _borderPoints2;
        Image<Gray, float> _meanVector1;
        Image<Gray, float> _stdVector1;
        Image<Gray, float> _meanVector2;
        Image<Gray, float> _stdVector2;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="dirReaderFactory"></param>
        /// <param name="imageReaderFactory"></param>
        /// <param name="preProcFactory"></param>
        /// <param name="borderFactory"></param>
        /// <param name="colummnCalculatorFactory"></param>
        public ProcessManager(IDirectoryReader_Creator dirReaderFactory, IDoubleLightImageReader_Creator imageReaderFactory,
                                    IImagePreProcessor_Creator preProcFactory, IBorderSeracher_Creator borderFactory, IColumnDataCalculator_Creator colummnCalculatorFactory)
        {
            _dirReaderFactory = dirReaderFactory;
            _imageReaderFactory = imageReaderFactory;
            _preProcFactory = preProcFactory;
            _borderFactory = borderFactory;
            _colummnCalculatorFactory = colummnCalculatorFactory;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="diretoryName"></param>
        /// <param name="fileExtension"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="bitDepth"></param>
        /// <param name="borderSkip"></param>
        /// <param name="showImages"></param>
        /// <returns></returns>
        public bool Init(string diretoryName, string fileExtension, int width, int height, int bitNumber, int intensityRange, int borderSkip, bool showImages)
        {
            _dirReader = _dirReaderFactory.Factory(diretoryName, fileExtension, _imageReaderFactory, width, bitNumber);

            _preProc = _preProcFactory.Factory(intensityRange, width, height);

            _borderSearcher = _borderFactory.Factory(borderSkip, showImages);

            _calcColumnData = _colummnCalculatorFactory.Factory(width, height);

            return true;
        }


        public bool Run()
        {

            string message = null;

            while (!_dirReader.EndOfDirectory())
            {
                _dirReader.GetNextImage(ref _image1, ref _image2, ref message);

                _preProc.PreProcessImage(_image1, ref _mask1);
                _preProc.PreProcessImage(_image2, ref _mask2);

                _borderSearcher.GetBorderPoints(_mask1, ref _borderPoints1, ref message);
                _borderSearcher.GetBorderPoints(_mask2, ref _borderPoints2, ref message);

                _calcColumnData.CalculateStatistics(_image1, _mask1, _borderPoints1, ref _meanVector1, ref _stdVector2);
                _calcColumnData.CalculateStatistics(_image2, _mask2, _borderPoints2, ref _meanVector1, ref _stdVector2);
            }

            return true;
        }


    }
}
