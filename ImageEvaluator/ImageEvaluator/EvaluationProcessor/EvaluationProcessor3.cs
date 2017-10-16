using System;
using System.IO;
using Emgu.CV;
using Emgu.CV.Structure;
using ImageEvaluatorInterfaces;
using ImageEvaluatorLib.DataSaver;
using NLog;

namespace ImageEvaluator.EvaluationProcessor
{
    class EvaluationProcessor3 : EvaluationProcessorBase
    {

        readonly IDirectoryReader _dirReader;
        readonly IImagePreProcessor _preProc;
        readonly IBorderSearcher _borderSearcher;
        readonly IColumnDataCalculator _columnDataCalculator;
        readonly IResultSaver _saver;
        private readonly IEdgeLineFinder _edgeFinder;
        bool _initialized;


        Image<Gray, ushort> _image1;
        Image<Gray, ushort> _image2;
        Image<Gray, byte> _mask1;
        Image<Gray, byte> _mask2;
        int[,] _borderPoints1;
        int[,] _borderPoints2;
        Image<Gray, float> _meanVector1;
        Image<Gray, float> _stdVector1;
        Image<Gray, float> _meanVector2;
        Image<Gray, float> _stdVector2;


        public EvaluationProcessor3(ILogger logger, IDirectoryReader dirReader, IImagePreProcessor preProc, IBorderSearcher borderSearcher, IColumnDataCalculator colummnCalculator, IResultSaver saver, IEdgeLineFinder edgeFinder)
            : base(logger)
        {
            _dirReader = dirReader;
            _preProc = preProc;
            _borderSearcher = borderSearcher;
            _columnDataCalculator = colummnCalculator;
            _edgeFinder = edgeFinder;
            _saver = saver;

            _logger?.Info("EvaluationProcessor3 instantiated.");

            Init();

            _logger?.Info("EvaluationProcessor3 " + (_initialized ? string.Empty : "NOT") + " initialized.");
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

            //resu = resu && _edgeFinder.Init();
            //CheckInit(resu, nameof(_edgeFinder));

            return _initialized = resu;
        }


        public override bool Run()
        {

            if (!_initialized)
            {
                _logger?.Error("EvaluationProcessor3 Run is not initialized yet.");
                return false;
            }

            _logger?.Info("EvaluationProcessor3 Run started.");

            while (!_dirReader.EndOfDirectory())
            {
                _watch1.Restart();

                string name = string.Empty;
                _dirReader.GetNextImage(ref _image1, ref _image2, ref name);

                LogElapsedTime(_watch1, $"Image reading: {Path.GetFileName(name)}");

                _preProc.Run(_image1, ref _mask1, name);

                LogElapsedTime(_watch1, $"Image pre-processing: {Path.GetFileName(name)}");

                _borderSearcher.Run(_mask1, ref _borderPoints1);

                LogElapsedTime(_watch1, $"Border search: {Path.GetFileName(name)}");

                _columnDataCalculator.Run(_image1, _mask1, _borderPoints1, ref _meanVector1, ref _stdVector1);

                LogElapsedTime(_watch1, $"Column data, statistical calculation: {Path.GetFileName(name)}");

                IColumnMeasurementResult result1 = new ColumnMeasurementResult {Name = "img1", ColumnMeanVector = _meanVector1, ColumnStdVector = _stdVector1};

                _saver.SaveResult(result1, name);

                LogElapsedTime(_watch1, $"Result saving: {Path.GetFileName(name)}");

                IWaferEdgeFindData waferEdgeFindData1 = null;

                _edgeFinder.Run(_image1, _mask1, ref waferEdgeFindData1);

                LogElapsedTime(_watch1, "Edge finder");
            }

            _logger?.Info("EvaluationProcessor3 Run ended.");

            return true;

        }



    }
}
