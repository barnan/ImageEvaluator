using Emgu.CV;
using Emgu.CV.Structure;
using ImageEvaluatorInterfaces;
using NLog;
using System;
using System.IO;

namespace ImageEvaluator.EvaluationProcessor
{
    class EvaluationProcessor2 : EvaluationProcessorBase
    {

        private readonly IDirectoryReader _dirReader;
        private readonly ISawmarkDeterminer _sawmarkDet;
        bool _initialized;

        Image<Gray, byte>[] _images;
        Image<Gray, byte> _image2;


        public EvaluationProcessor2(ILogger logger, IDirectoryReader dirReader, ISawmarkDeterminer sawmarkDet)
            : base(logger)
        {
            _dirReader = dirReader;
            _sawmarkDet = sawmarkDet;

            _logger?.Info("EvaluationProcessor2 instantiated.");

            Init();
        }


        public override bool Init()
        {
            bool resu = _dirReader.Init();
            CheckInit(resu, nameof(_dirReader));

            resu = resu & _sawmarkDet.Init();
            CheckInit(resu, nameof(_sawmarkDet));

            _logger?.Info("EvaluationProcessor2 " + (_initialized ? string.Empty : "NOT") + " initialized.");

            return _initialized = resu;
        }

        public override bool Run()
        {
            if (!_initialized)
            {
                _logger?.Error("EvaluationProcessor2 Run is not initialized yet.");
                return false;
            }

            _logger?.Info("EvaluationProcessor2 Run started.");

            while (!_dirReader.IsEndOfDirectory())
            {
                _watch1.Restart();

                string name = string.Empty;
                _dirReader.GetNextImage(ref _images, ref name);

                LogElapsedTime(_watch1, $"Image reading: {Path.GetFileName(name)}");

                _sawmarkDet.Run(_images[0], name);

                LogElapsedTime(_watch1, $"Determine wafer orientation: {Path.GetFileName(name)}");


                Console.WriteLine();
            }

            _logger?.Info("EvaluationProcessor2 Run ended.");

            return true;
        }


    }
}
