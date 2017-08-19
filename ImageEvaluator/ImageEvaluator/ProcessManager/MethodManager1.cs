using Emgu.CV;
using Emgu.CV.Structure;
using NLog;

namespace ImageEvaluator.ManageProcess
{

    class MethodManager1 : IMethodManager
    {
        ILogger _logger;
        IDirectoryReader _dirReader;
        IImagePreProcessor _preProc;
        IBorderSearcher _borderSearcher;
        IColumnDataCalculator _columnDataCalculator;


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
        /// <param name="logger"></param>
        /// <param name="dirReader"></param>
        /// <param name="preProc"></param>
        /// <param name="borderSearcher"></param>
        /// <param name="colummnCalculator"></param>
        public MethodManager1(ILogger logger, IDirectoryReader dirReader, IImagePreProcessor preProc, IBorderSearcher borderSearcher, IColumnDataCalculator colummnCalculator)
        {
            _dirReader = dirReader;
            _preProc = preProc;
            _borderSearcher = borderSearcher;
            _columnDataCalculator = colummnCalculator;
            _logger = logger;

            _logger?.Info("MethodManager 1 instantiated.");
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool Run()
        {
            if (_logger?.IsTraceEnabled ?? false)
                _logger?.Info("MethodManager 1 Run started.");


            string message = null;

            while (!_dirReader.EndOfDirectory())
            {
                _dirReader.GetNextImage(ref _image1, ref _image2, ref message);

                _preProc.Run(_image1, ref _mask1);
                _preProc.Run(_image2, ref _mask2);

                _borderSearcher.Run(_mask1, ref _borderPoints1, ref message);
                _borderSearcher.Run(_mask2, ref _borderPoints2, ref message);

                _columnDataCalculator.Run(_image1, _mask1, _borderPoints1, ref _meanVector1, ref _stdVector1);
                _columnDataCalculator.Run(_image2, _mask2, _borderPoints2, ref _meanVector2, ref _stdVector2);
            }

            return true;
        }


    }
}
