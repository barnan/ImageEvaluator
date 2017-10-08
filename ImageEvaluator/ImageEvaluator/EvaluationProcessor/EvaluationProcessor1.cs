using Emgu.CV;
using Emgu.CV.Structure;
using ImageEvaluatorInterfaces;
using ImageEvaluatorLib.DataSaver;
using NLog;
using System;
using System.IO;

namespace ImageEvaluator.EvaluationProcessor
{
    class EvaluationProcessor1 : EvaluationProcessorBase
    {
        readonly IDirectoryReader _dirReader;
        readonly IImagePreProcessor _preProc;
        readonly IBorderSearcher _borderSearcher;
        readonly IColumnDataCalculator _columnDataCalculator;
        readonly IResultSaver _saver;
        private readonly IEdgeLineFinder _edgeFinder;
        private readonly IEdgeLineFitter _edgeFitter;
        bool _initialized;


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
        /// <param name="saver"></param>
        /// <param name="edgeFinder"></param>
        /// <param name="edgeFitter"></param>
        public EvaluationProcessor1(ILogger logger, IDirectoryReader dirReader, IImagePreProcessor preProc, IBorderSearcher borderSearcher, IColumnDataCalculator colummnCalculator, IResultSaver saver, IEdgeLineFinder edgeFinder, IEdgeLineFitter edgeFitter)
            : base(logger)
        {
            _dirReader = dirReader;
            _preProc = preProc;
            _borderSearcher = borderSearcher;
            _columnDataCalculator = colummnCalculator;
            _saver = saver;
            _edgeFinder = edgeFinder;
            _edgeFitter = edgeFitter;

            _logger?.Info("MethodManager 1 instantiated.");

            Init();

            _logger?.Info("MethodManager 1 " + (_initialized ? string.Empty : "NOT") + " initialized.");

        }


        public override bool Init()
        {
            bool resu = _dirReader.Init();
            CheckInit(resu, nameof(_dirReader));

            resu = resu && _preProc.Init();
            CheckInit(resu, nameof(_preProc));

            resu = resu && _borderSearcher.Init();
            CheckInit(resu, nameof(_borderSearcher));

            resu = resu && _columnDataCalculator.Init();
            CheckInit(resu, nameof(_columnDataCalculator));

            resu = resu && _saver.Init();
            CheckInit(resu, nameof(_saver));

            resu = resu && _edgeFinder.Init();
            CheckInit(resu, nameof(_edgeFinder));

            resu = resu && _edgeFitter.Init();
            CheckInit(resu, nameof(_edgeFitter));

            return _initialized = resu;
        }



        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override bool Run()
        {
            if (!_initialized)
            {
                _logger?.Error("MethodManager 1 Run is not initialized yet.");
                return false;
            }

            _logger?.Info("MethodManager 1 Run started.");
            Console.WriteLine("MethodManager 1 Run started.");

            while (!_dirReader.EndOfDirectory())
            {
                _watch1.Restart();

                string name = string.Empty;
                _dirReader.GetNextImage(ref _image1, ref _image2, ref name);

                LogElapsedTime(_watch1, $"Image reading: {Path.GetFileName(name)}");

                _preProc.Run(_image1, ref _mask1);
                _preProc.Run(_image2, ref _mask2);

                LogElapsedTime(_watch1, $"Image pre-processing: {Path.GetFileName(name)}");

                _borderSearcher.Run(_mask1, ref _borderPoints1);
                _borderSearcher.Run(_mask2, ref _borderPoints2);

                LogElapsedTime(_watch1, $"Border search: {Path.GetFileName(name)}");

                _columnDataCalculator.Run(_image1, _mask1, _borderPoints1, ref _meanVector1, ref _stdVector1);
                _columnDataCalculator.Run(_image2, _mask2, _borderPoints2, ref _meanVector2, ref _stdVector2);

                LogElapsedTime(_watch1, $"Column data, statistical calculation: {Path.GetFileName(name)}");

                IMeasurementResult result1 = new ColumnStatisticMeasurementResult { Name = "img1", MeanVector = _meanVector1, StdVector = _stdVector1 };
                IMeasurementResult result2 = new ColumnStatisticMeasurementResult { Name = "img2", MeanVector = _meanVector2, StdVector = _stdVector2 };

                _saver.SaveResult(result1, name);
                _saver.SaveResult(result2, name);

                LogElapsedTime(_watch1, $"Result saving: {Path.GetFileName(name)}");

                IWaferEdgeFindData waferEdgeFindData1 = null;
                IWaferEdgeFindData waferEdgeFindData2 = null;

                _edgeFinder.Run(_image1, _mask1, ref waferEdgeFindData1);
                _edgeFinder.Run(_image2, _mask2, ref waferEdgeFindData2);

                LogElapsedTime(_watch1, $"Edge finder");

                IWaferFittingData waferEdgeFittingData1 = null;
                IWaferFittingData waferEdgeFittingData2 = null;

                _edgeFitter.Run(waferEdgeFindData1, ref waferEdgeFittingData1);
                _edgeFitter.Run(waferEdgeFindData2, ref waferEdgeFittingData2);

                LogElapsedTime(_watch1, $"Edge fitter");

                Console.WriteLine();
            }

            _logger?.Info("MethodManager 1 Run ended.");

            return true;
        }

    }
}
