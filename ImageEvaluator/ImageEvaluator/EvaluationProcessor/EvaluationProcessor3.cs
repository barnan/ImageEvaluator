using System;
using System.IO;
using Emgu.CV;
using Emgu.CV.Structure;
using ImageEvaluatorInterfaces;
using ImageEvaluatorInterfaces.BaseClasses;
using ImageEvaluatorLib.DataSaver;
using NLog;
using System.Collections.Generic;

namespace ImageEvaluator.EvaluationProcessor
{
    class EvaluationProcessor3 : EvaluationProcessorBase
    {

        readonly IDirectoryReader _dirReader;
        readonly IImagePreProcessor _preProc;
        readonly IBorderSearcher _borderSearcher;
        readonly IColumnDataCalculator _columnDataCalculator1;
        readonly IColumnDataCalculator _columnDataCalculator2;
        readonly IResultSaver _saver1;
        readonly IResultSaver _saver2;
        private readonly IEdgeLineFinder _edgeFinder;


        //Image<Gray, byte>[] _images;
        //Image<Gray, byte>[] _masks;
        int[,] _borderPoints1;
        Image<Gray, double> _meanVector1;
        Image<Gray, double> _stdVector1;


        public EvaluationProcessor3(ILogger logger, IDirectoryReader dirReader, IImagePreProcessor preProc, IBorderSearcher borderSearcher, IColumnDataCalculator colummnCalculator1, IColumnDataCalculator colummnCalculator2, IResultSaver saver1, IResultSaver saver2, IEdgeLineFinder edgeFinder)
            : base(logger)
        {
            _dirReader = dirReader;
            _preProc = preProc;
            _borderSearcher = borderSearcher;
            _columnDataCalculator1 = colummnCalculator1;
            _columnDataCalculator2 = colummnCalculator2;
            _edgeFinder = edgeFinder;
            _saver1 = saver1;
            _saver2 = saver2;

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

            resu = resu && _columnDataCalculator1.Init();
            CheckInit(resu, nameof(_columnDataCalculator1));

            resu = resu && _columnDataCalculator2.Init();
            CheckInit(resu, nameof(_columnDataCalculator2));

            resu = resu && _saver1.Init();
            CheckInit(resu, nameof(_saver1));

            resu = resu && _saver2.Init();
            CheckInit(resu, nameof(_saver2));

            resu = resu && _edgeFinder.Init();
            CheckInit(resu, nameof(_edgeFinder));

            return IsInitialized = resu;
        }


        public override bool Run()
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

                string path = Path.Combine(_saver1.OutputFolder, Path.GetFileName(name) ?? string.Empty);

                LogElapsedTime(_watch1, $"Image reading: {name}");

                _preProc.Run(_dynamicResult, path);

                LogElapsedTime(_watch1, $"Image pre-processing: {name}");

                _borderSearcher.Run(_dynamicResult, path);

                LogElapsedTime(_watch1, $"Border search: {Path.GetFileName(name)}");

                double resu1;
                double resu2;
                double resu3;
                double resu4;
                double resu5;
                double resu6;
                double resu7;
                double resu8;
                double resu9;
                double resu10;
                //_columnDataCalculator1.Run(_dynamicResult, _borderPoints1, ref _meanVector1, ref _stdVector1, out resu1, out resu2, out resu3, out resu4, out resu5, out resu6, out resu7, out resu8, out resu9, out resu10);
                _columnDataCalculator1.Run(_dynamicResult, path);

                IColumnStatisticalMeasurementResult result1 = new ColumnStatisticalMeasurementResult
                {
                    Name = name,
                    MeanOfMean = resu1,
                    StdOfMean = resu2,
                    MeanOfStd = resu3,
                    StdOfStd = resu4,
                    ColumnHomogenity1 = resu5,
                    ColumnHomogenity2 = resu6,
                    MinOfMean = resu7,
                    MaxOfMean = resu8
                };

                IColumnMeasurementResult result2 = new ColumnMeasurementResult
                {
                    Name = name,
                    ColumnMeanVector = _meanVector1,
                    ColumnStdVector = _stdVector1
                };
                _saver2.SaveResult(result2 as IMeasurementResult, name, "MeanAndStd");


                LogElapsedTime(_watch1, $"Column data, statistical calculation 1: {Path.GetFileName(name)}");

                _columnDataCalculator2.Run(_dynamicResult, _borderPoints1, ref _meanVector1, ref _stdVector1, out resu1, out resu2, out resu3, out resu4, out resu5, out resu6, out resu7, out resu8, out resu9, out resu10);
                result1.MeanOfNoiseMean = resu1;
                result1.StdOfNoiseMean = resu2;
                result1.MeanOfNoiseStd = resu3;
                result1.StdOfNoiseStd = resu4;

                IColumnMeasurementResult result3 = new ColumnMeasurementResult
                {
                    Name = name,
                    ColumnMeanVector = _meanVector1,
                    ColumnStdVector = _stdVector1
                };
                _saver2.SaveResult(result3 as IMeasurementResult, name, "Noise");


                LogElapsedTime(_watch1, $"Column data, statistical calculation 2: {Path.GetFileName(name)}");

                IWaferEdgeFindData waferEdgeFindData1 = null;

                _edgeFinder.Run(_dynamicResult, ref waferEdgeFindData1);
                result1.LeftLineSpread = waferEdgeFindData1.LeftLineSpread;
                result1.RightLineSpread = waferEdgeFindData1.RightLineSpread;
                result1.TopLineSpread = waferEdgeFindData1.TopLineSpread;
                result1.BottomLineSpread = waferEdgeFindData1.BottomLineSpread;

                LogElapsedTime(_watch1, "Edge finder");

                _saver1.SaveResult(result1 as IMeasurementResult, name, "");

                LogElapsedTime(_watch1, $"Result saving: {Path.GetFileName(name)}");
            }

            _logger?.Info($"{this.GetType().Name} Run ended.");

            return true;

        }



    }
}
