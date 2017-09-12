using System;
using Emgu.CV;
using Emgu.CV.Structure;
using ImageEvaluator.Interfaces;
using NLog;

namespace ImageEvaluator.MethodManager
{
    class MethodManager2 : MethodManagerBase
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


        public MethodManager2(ILogger logger, IDirectoryReader dirReader, IImagePreProcessor preProc, IBorderSearcher borderSearcher, IColumnDataCalculator colummnCalculator, IResultSaver saver, IEdgeLineFinder edgeFinder, IEdgeLineFitter edgeFitter)
            :base (logger)
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

            _logger?.Info("MethodManager1 " + (_initialized? string.Empty : "NOT") + " initialized.");

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

        public override bool Run()
        {
            throw new NotImplementedException();
        }

    }
}
