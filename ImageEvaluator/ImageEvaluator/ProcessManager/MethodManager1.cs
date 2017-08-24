﻿using Emgu.CV;
using Emgu.CV.Structure;
using ImageEvaluator.DataSaver;
using NLog;

namespace ImageEvaluator.ManageProcess
{

    class MethodManager1 : IMethodManager
    {
        readonly ILogger _logger;
        readonly IDirectoryReader _dirReader;
        readonly IImagePreProcessor _preProc;
        readonly IBorderSearcher _borderSearcher;
        readonly IColumnDataCalculator _columnDataCalculator;
        private readonly IResultSaver _saver;
        private bool _initialized;


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
        public MethodManager1(ILogger logger, IDirectoryReader dirReader, IImagePreProcessor preProc, IBorderSearcher borderSearcher, IColumnDataCalculator colummnCalculator, IResultSaver saver)
        {
            _dirReader = dirReader;
            _preProc = preProc;
            _borderSearcher = borderSearcher;
            _columnDataCalculator = colummnCalculator;
            _saver = saver;
            _logger = logger;

            _logger?.Info("MethodManager 1 instantiated.");

            Init();
            _logger?.Info("MethodManager1 " + (_initialized ? string.Empty : "NOT") + " initialized.");

        }



        public bool Init()
        {
            bool resu = true;

            resu = resu && _dirReader.Init();
            CheckInit(resu, nameof(_dirReader));

            resu = resu && _preProc.Init();
            CheckInit(resu, nameof(_preProc));

            resu = resu && _borderSearcher.Init();
            CheckInit(resu, nameof(_borderSearcher));

            resu = resu && _columnDataCalculator.Init();
            CheckInit(resu, nameof(_columnDataCalculator));

            resu = resu && _saver.Init();
            CheckInit(resu, nameof(_saver));

            return _initialized = resu;
        }


        private void CheckInit(bool resu, string dirreader)
        {
            _logger?.Info(resu ? $"Initialization SUCCED: {dirreader}" : $"Initialization FAILED: {dirreader}");
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool Run()
        {
            if (!_initialized)
            {
                _logger?.Error("MethodManager Run is not initialized yet.");
                return false;
            }

            _logger?.Info("MethodManager 1 Run started.");

            while (!_dirReader.EndOfDirectory())
            {
                _dirReader.GetNextImage(ref _image1, ref _image2);

                _preProc.Run(_image1, ref _mask1);
                _preProc.Run(_image2, ref _mask2);

                _borderSearcher.Run(_mask1, ref _borderPoints1);
                _borderSearcher.Run(_mask2, ref _borderPoints2);

                _columnDataCalculator.Run(_image1, _mask1, _borderPoints1, ref _meanVector1, ref _stdVector1);
                _columnDataCalculator.Run(_image2, _mask2, _borderPoints2, ref _meanVector2, ref _stdVector2);

                IMeasurementResult result1 = new MeasurementResult {Name = "img1", MeanVector = _meanVector1, StdVector = _stdVector1};
                IMeasurementResult result2 = new MeasurementResult {Name = "img2", MeanVector = _meanVector2, StdVector = _stdVector2};

                _saver.SaveResult(result1);
                _saver.SaveResult(result2);
            }

            _logger?.Info("MethodManager 1 Run started.");

            return true;
        }


    }
}
