﻿using Emgu.CV;
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

        Image<Gray, float> _image1;
        Image<Gray, float> _image2;


        public EvaluationProcessor2(ILogger logger, IDirectoryReader dirReader, ISawmarkDeterminer sawmarkDet)
            : base(logger)
        {
            _dirReader = dirReader;
            _sawmarkDet = sawmarkDet;

            _logger?.Info("MethodManager 2 instantiated.");

            Init();
        }


        public override bool Init()
        {
            bool resu = _dirReader.Init();
            CheckInit(resu, nameof(_dirReader));

            resu = resu & _sawmarkDet.Init();
            CheckInit(resu, nameof(_sawmarkDet));

            _logger?.Info("MethodManager 2 " + (_initialized ? string.Empty : "NOT") + " initialized.");

            return _initialized = resu;
        }

        public override bool Run()
        {
            if (!_initialized)
            {
                _logger?.Error("MethodManager 2 Run is not initialized yet.");
                return false;
            }

            _logger?.Info("MethodManager 2 Run started.");
            Console.WriteLine("MethodManager 2 Run started.");

            while (!_dirReader.EndOfDirectory())
            {
                _watch1.Restart();

                string name = string.Empty;
                _dirReader.GetNextImage(ref _image1, ref _image2, ref name);

                LogElapsedTime(_watch1, $"Image reading: {Path.GetFileName(name)}");

                _sawmarkDet.Run(_image1, name);

                LogElapsedTime(_watch1, $"Determine wafer orientation: {Path.GetFileName(name)}");


                Console.WriteLine();
            }

            _logger?.Info("MethodManager 2 Run ended.");

            return true;
        }


    }
}