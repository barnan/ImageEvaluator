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

namespace ImageEvaluator.MethodManager
{
    class MethodManager3 : MethodManagerBase
    {
        public MethodManager3(string[] paths)
            : base(paths)
        {
        }


        public override bool Instantiate()
        {
            try
            {
                const int width = 8192;
                const int height = 8192;

                _logger = LogManager.GetCurrentClassLogger();
                _logger?.Info("--------------------------------------------------------------------------------------------------------------------------------------");

                bool show = true;

                IImageReader imageReader = new FactorySimpleLight8BitImageReader().Factory(_logger, width, show);

                IDirectoryReader dirReader = new Factory_DirectoryReader().Factory(_logger, _inputPaths[_pathIndex], "raw", imageReader);

                int histogramRange = 4096;
                IHistogramThresholdCalculator histcalculator = new FactoryHistogramThresholdCalculatorCSharp1().Factory(_logger, histogramRange, 40);

                IImagePreProcessor preProcessor = new FactoryImagePreProcessor().Factory(_logger, histogramRange, width, height, histcalculator, show);

                IBorderSearcher borderSearcher = new FactoryBorderSearcherEmgu2().Factory(_logger, 10, width, height, show);

                IColumnDataCalculator columnDataCalculator = new FactoryCalculateColumnDataEmgu1().Factory(_logger, width, height);

                string outputFolder = Path.Combine(_inputPaths[_pathIndex], "output");
                IResultSaver saver = new FactoryCsvResultSaver().Factory(outputFolder, "StatCalc", _logger);


                IEdgeLineFinder finder = new FactoryEdgeLineFinderEmgu1().Factory(_logger);

                IEdgeLineFitter fitter = new Factory_EdgeLineFitter_Emgu1().Factory(_logger);

                _evaluationProcessor = new EvaluationProcessor3(_logger, dirReader, preProcessor, borderSearcher, columnDataCalculator, saver, finder);
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
