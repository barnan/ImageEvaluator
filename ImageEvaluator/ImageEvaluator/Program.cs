using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using ImageEvaluator.CalculateStatisticalData;
using ImageEvaluator.DataSaver;
using ImageEvaluator.FindEdgeLines;
using ImageEvaluator.FitEdgeLines;
using ImageEvaluator.Interfaces;
using ImageEvaluator.MethodManager;
using ImageEvaluator.PreProcessor;
using ImageEvaluator.ReadDirectory;
using ImageEvaluator.ReadImage;
using ImageEvaluator.SearchContourPoints;
using NLog;

namespace ImageEvaluator
{
    class Program
    {
        static void Main(string[] args)
        {
            int width = 4096;
            int height = 4096;


            Logger logger = null;// LogManager.GetCurrentClassLogger();
            bool show = false;

            IDoubleLightImageReader imageReader = new Factory_DoubleLight16bitImageReader().Factory(logger, width, 2, show);

            string inputFolder = @"d:\_SW_Projects\Quantify_Image_Quality\homogenity test wins13\";
            string name = string.Empty;
            IDirectoryReader dirReader = new Factory_DirectoryReader().Factory(logger, inputFolder, "raw", imageReader);

            IImagePreProcessor preProcessor = new Factory_ImagePreProcessor().Factory(logger, 4096, width, height, show);

            IBorderSearcher borderSearcher = new Factory_BorderSearcher_Emgu1().Factory(logger, 10, height, show);

            IColumnDataCalculator columnDataCalculator = new Factory_CalculateColumnData_Emgu1().Factory(logger, width, height);

            string outputFolder = Path.Combine(inputFolder, "output");
            IResultSaver saver = new Factory_CsvResultSaver().Factory(outputFolder, "StatCalc", logger);


            Dictionary<SearchOrientations, Rectangle> calcareas = new Dictionary<SearchOrientations, Rectangle>();
            calcareas.Add(SearchOrientations.TopToBottom, new Rectangle(1000, 50, 2000, 450));
            calcareas.Add(SearchOrientations.LeftToRight, new Rectangle(50, 1000, 450, 2000));
            calcareas.Add(SearchOrientations.BottomToTop, new Rectangle(1000, height - 501, 2000, 500));
            calcareas.Add(SearchOrientations.RightToLeft, new Rectangle(width - 501, 1000, 500, 2000));
            IEdgeLineFinder finder = new Factory_EdgeLineFinder_CSharp1().Factory (logger, calcareas);

            IEdgeLineFitter fitter = new Factory_EdgeLineFitter_Emgu1().Factory();

            IMethodManager manager = new MethodManager1(logger, dirReader, preProcessor, borderSearcher, columnDataCalculator, saver, finder, fitter);

            manager.Run();

            Console.ReadKey();

        }
    }
}
