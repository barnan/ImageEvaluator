using ImageEvaluator.CalculateStatisticalData;
using ImageEvaluator.ManageProcess;
using ImageEvaluator.PreProcessor;
using ImageEvaluator.ReadDirectory;
using ImageEvaluator.ReadImage;
using ImageEvaluator.SearchContourPoints;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageEvaluator
{
    class Program
    {
        static void Main(string[] args)
        {

            IProcessManager manager = new ProcessManager(new Factory_DirectoryReader(), new Factory_DoubleLight16bitImageReader(),
                                                        new Factory_ImagePreProcessor(), new Factory_BorderSearcher_Emgu1(), new Factory_CalculateColumnData_Emgu1());

            manager.Init(@"f:\Quantify_Image_Quality\homogenity test wins13", "raw", 4096, 4096, 2, 4096, 10, false);


            manager.Run();

        }
    }
}
