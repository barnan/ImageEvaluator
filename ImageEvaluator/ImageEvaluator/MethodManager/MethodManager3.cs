using System;
using System.IO;
using ImageEvaluator.EvaluationProcessor;
using ImageEvaluatorInterfaces;
using ImageEvaluatorLib.BorderSearch;
using ImageEvaluatorLib.CalculateStatisticalData;
using ImageEvaluatorLib.DataSaver;
using ImageEvaluatorLib.FindEdgeLines;
using ImageEvaluatorLib.FitEdgeLines;
using ImageEvaluatorLib.PreProcessor;
using ImageEvaluatorLib.ReadDirectory;
using ImageEvaluatorLib.ReadImage;
using ImageEvaluatorLib.ThresholdCalculator;
using NLog;
using ImageEvaluatorInterfaces.BaseClasses;

namespace ImageEvaluator.MethodManager
{
    class MethodManager3 : MethodManagerBase
    {
        private int _width;
        private int _height;

        public MethodManager3(string[] paths, int width, int height)
            : base(paths)
        {
            _width = width;
            _height = height;
        }


        public override bool Instantiate()
        {
            try
            {
                _logger = LogManager.GetCurrentClassLogger();
                _logger?.Info("--------------------------------------------------------------------------------------------------------------------------------------");

                bool show = true;

                IImageReader imageReader = new FactorySimpleLight8BitImageReader().Factory(_logger, _width, _height, show);

                IDirectoryReader dirReader = new Factory_DirectoryReader().Factory(_logger, _inputPaths[_pathIndex], "raw", imageReader);

                int histogramRange = 4096;
                //IHistogramThresholdCalculator histcalculator = new FactoryHistogramThresholdCalculatorCSharp1().Factory(_logger, 256, 40);
                IHistogramThresholdCalculator histcalculator = new FactoryHistogramThresholdCalculatorCSharp1().Factory(_logger, 256, 30);

                BeltCoordinates beltCoords = new BeltCoordinates { LeftBeltStart = 425, LeftBeltEnd = 565, RightBeltStart = 1500, RightBeltEnd = 1640 };
                IImagePreProcessor preProcessor = new FactoryImagePreProcessor().Factory(_logger, histogramRange, _width, _height, histcalculator, show, beltCoords);

                IBorderSearcher borderSearcher = new FactoryBorderSearcherEmgu2().Factory(_logger, 10, _width, _height, show);

                IColumnDataCalculator columnDataCalculator1 = new FactoryCalculateColumnDataEmgu1().Factory(_logger, _width, _height);
                IColumnDataCalculator columnDataCalculator2 = new FactoryCalculateColumnDataEmgu3().Factory(_logger, _width, _height);

                string outputFolder = Path.Combine(_inputPaths[_pathIndex], "output");
                IResultSaver saver1 = new FactoryCsvColumnStatisticalResultSaver().Factory(outputFolder, "StatCalc", _logger);
                IResultSaver saver2 = new FactoryCsvColumnResultSaver().Factory(outputFolder, "Mean", _logger);


                IEdgeLineFinder finder = new FactoryEdgeLineFinderEmgu1().Factory(_logger, _width, _height);

                IEdgeLineFitter fitter = new Factory_EdgeLineFitter_Emgu1().Factory(_logger);

                _evaluationProcessor = new EvaluationProcessor3(_logger, dirReader, preProcessor, borderSearcher, columnDataCalculator1, columnDataCalculator2, saver1, saver2, finder);
            }
            catch (Exception ex)
            {
                _logger?.Error($"Exception during MethodManager1 Instantiation: {ex.Message}");
                return false;
            }

            return true;
        }
    }
}
