using System;
using ImageEvaluatorInterfaces;
using NLog;
using ImageEvaluatorLib.ReadDirectory;
using ImageEvaluatorLib.DetermineSawmarkOrientation;
using ImageEvaluator.EvaluationProcessor;
using ImageEvaluatorLib.ReadImage;

namespace ImageEvaluator.MethodManager
{
    class MethodManager2 : MethodManagerBase
    {
        private int _width;
        private int _height;

        public MethodManager2(string[] paths, int width, int height)
            : base(paths)
        {
            _width = width;
            _height = height;
        }


        public override bool Instantiate()
        {
            try
            {
                const int width = 4096;

                _logger = LogManager.GetCurrentClassLogger();
                _logger?.Info("--------------------------------------------------------------------------------------------------------------------------------------");

                bool show = false;

                IImageReader imageReader = new FactoryDoubleLight8BitImageReader().Factory(_logger, width, show);

                //string inputFolder = @"d:\WaferOrientationCheck\MCI_Images\Diamond_Mono_0degree_U5\";
                if (_inputPaths == null)
                    return false;

                IDirectoryReader dirReader = new Factory_DirectoryReader().Factory(_logger, _inputPaths[_pathIndex], "raw", imageReader);

                IWaferOrientationDetector det = new WaferOrientationDetector(null, 4096, 4096, 1024, 3072);

                ISawmarkDeterminer sawmarkDeterminer = new Factory_DetermineSawmarkOrientation().Factory(_logger, det);

                _evaluationProcessor = new EvaluationProcessor2(_logger, dirReader, sawmarkDeterminer);
            }
            catch (Exception ex)
            {
                _logger?.Error($"Exception during MethodManager2 Instantiation: {ex.Message}");
                return false;
            }

            return true;
        }

    }
}
