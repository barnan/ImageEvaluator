using System;
using System.IO;
using ImageEvaluatorInterfaces;
using ImageEvaluatorLib.DataSaver;
using ImageEvaluatorLib.ReadImage;
using ImageEvaluatorLib.ReadDirectory;
using ImageEvaluatorLib.PreProcessor;
using ImageEvaluatorLib.CalculateStatisticalData;
using System.Collections.Generic;
using System.Drawing;
using ImageEvaluatorLib.FindEdgeLines;
using ImageEvaluatorLib.FitEdgeLines;
using ImageEvaluator.EvaluationProcessor;
using ImageEvaluatorLib.BorderSearch;
using ImageEvaluatorLib.ThresholdCalculator;
using NLog;
using ImageEvaluatorInterfaces.BaseClasses;

namespace ImageEvaluator.MethodManager
{

    internal class MethodManager1 : MethodManagerBase
    {
        private int _width;
        private int _height;

        public MethodManager1(string[] paths, int width, int height)
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

                bool show = false;

                IImageReader imageReader = new Factory_DoubleLight16bitImageReader().Factory(_logger, _width, _height, show);

                IDirectoryReader dirReader = new Factory_DirectoryReader().Factory(_logger, _inputPaths[_pathIndex], "raw", imageReader);

                int histogramRange = 4096;
                IHistogramThresholdCalculator histcalculator = new FactoryHistogramThresholdCalculatorEmgu1().Factory(_logger, histogramRange);

                BeltCoordinates beltCoords = new BeltCoordinates { LeftBeltStart = 425, LeftBeltEnd = 565, RightBeltStart = 1500, RightBeltEnd = 1640 };
                IImagePreProcessor preProcessor = new FactoryImagePreProcessor().Factory(_logger, histogramRange, _width, _height, histcalculator, show, beltCoords);

                IBorderSearcher borderSearcher = new FactoryBorderSearcherEmgu1().Factory(_logger, 10, _width, _height, show);

                IColumnDataCalculator columnDataCalculator = new FactoryCalculateColumnDataEmgu1().Factory(_logger, _width, _height);

                string outputFolder = Path.Combine(_inputPaths[_pathIndex], "output");
                IResultSaver saver = new FactoryCsvColumnResultSaver().Factory(outputFolder, "StatCalc", _logger);


                Dictionary<SearchOrientations, Rectangle> calcareas = new Dictionary<SearchOrientations, Rectangle>();
                calcareas.Add(SearchOrientations.TopToBottom, new Rectangle(1000, 50, 2000, 450));
                calcareas.Add(SearchOrientations.LeftToRight, new Rectangle(50, 1000, 450, 2000));
                calcareas.Add(SearchOrientations.BottomToTop, new Rectangle(1000, _height - 501, 2000, 500));
                calcareas.Add(SearchOrientations.RightToLeft, new Rectangle(_width - 501, 1000, 500, 2000));
                IEdgeLineFinder finder = new FactoryEdgeLineFinderCSharp1().Factory(_logger, _width, _height, calcareas);

                IEdgeLineFitter fitter = new Factory_EdgeLineFitter_Emgu1().Factory(_logger);

                _evaluationProcessor = new EvaluationProcessor1(_logger, dirReader, preProcessor, borderSearcher, columnDataCalculator, saver, finder, fitter);
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
