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


        Image<Gray, byte>[] _images;
        Image<Gray, byte>[] _masks;
        int[,] _borderPoints1;
        int[,] _borderPoints2;
        Image<Gray, double> _meanVector1;
        Image<Gray, double> _stdVector1;
        Image<Gray, double> _meanVector2;
        Image<Gray, double> _stdVector2;


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

            _logger?.Info($"{this.GetType().Name} instantiated.");

            Init();

            _logger?.Info($"{this.GetType().Name} " + (IsInitialized ? string.Empty : "NOT") + " initialized.");
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

            return IsInitialized = resu;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override bool Execute()
        {
            if (!IsInitialized)
            {
                _logger?.Error($"{this.GetType().Name} Run is not initialized yet.");
                return false;
            }

            _logger?.Info($"{this.GetType().Name} Run started.");

            while (!_dirReader.IsEndOfDirectory())
            {
                _watch1.Restart();

                string name = string.Empty;
                _dirReader.GetNextImage(_dynamicResult, ref name);

                LogElapsedTime(_watch1, $"Image reading: {Path.GetFileName(name)}");

                _preProc.Execute(_dynamicResult, name);
                //_preProc.Run(_dynamicResult, name);

                LogElapsedTime(_watch1, $"Image pre-processing: {Path.GetFileName(name)}");

                _borderSearcher.Execute(_dynamicResult, name);
                //_borderSearcher.Run(_images[1], _masks[1], ref _borderPoints2, name);

                LogElapsedTime(_watch1, $"Border search: {Path.GetFileName(name)}");


                _columnDataCalculator.Execute(_dynamicResult, string.Empty);

                LogElapsedTime(_watch1, $"Column data, statistical calculation: {Path.GetFileName(name)}");

                IColumnMeasurementResult result1 = new ColumnMeasurementResult { Name = "img1", ColumnMeanVector = _meanVector1, ColumnStdVector = _stdVector1 };
                IColumnMeasurementResult result2 = new ColumnMeasurementResult { Name = "img2", ColumnMeanVector = _meanVector2, ColumnStdVector = _stdVector2 };

                _saver.SaveResult(result1, name, "");
                _saver.SaveResult(result2, name, "");

                LogElapsedTime(_watch1, $"Result saving: {Path.GetFileName(name)}");

                IWaferEdgeFindData waferEdgeFindData1 = null;
                IWaferEdgeFindData waferEdgeFindData2 = null;

                _edgeFinder.Execute(_dynamicResult, ref waferEdgeFindData1);
                //_edgeFinder.Run(_images[1], _masks[1], ref waferEdgeFindData2);

                LogElapsedTime(_watch1, "Edge finder");

                IWaferFittingData waferEdgeFittingData1 = null;
                IWaferFittingData waferEdgeFittingData2 = null;

                _edgeFitter.Execute(waferEdgeFindData1, ref waferEdgeFittingData1);
                _edgeFitter.Execute(waferEdgeFindData2, ref waferEdgeFittingData2);

                LogElapsedTime(_watch1, "Edge fitter");

                Console.WriteLine();
            }

            _logger?.Info($"{this.GetType().Name} Run ended.");

            return true;
        }

    }
}
