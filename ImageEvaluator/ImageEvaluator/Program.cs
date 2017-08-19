using ImageEvaluator.CalculateStatisticalData;
using ImageEvaluator.ManageProcess;
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


            //Logger logger = LogManager.GetLogger("ImageEvaluator.log");
            Logger logger = null;// LogManager.GetCurrentClassLogger();

            IDoubleLightImageReader imageReader = new Factory_DoubleLight16bitImageReader().Factory(logger, width, 2, true);

            IDirectoryReader dirReader = new Factory_DirectoryReader().Factory(logger, @"f:\Quantify_Image_Quality\homogenity test wins13", "raw", imageReader);

            IImagePreProcessor preProcessor = new Factory_ImagePreProcessor().Factory(logger, 4096, width, height, true);

            IBorderSearcher borderSearcher = new Factory_BorderSearcher_Emgu1().Factory(logger, 10, false);

            IColumnDataCalculator columnDataCalculator = new Factory_CalculateColumnData_Emgu1().Factory(logger, width, height);

            IMethodManager manager = new MethodManager1(logger, dirReader, preProcessor, borderSearcher, columnDataCalculator);

            manager.Run();



        }
    }
}
