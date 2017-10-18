using System;
using NLog;
using ImageEvaluator.MethodManager;
using ImageEvaluatorInterfaces;

namespace ImageEvaluator
{
    class Program
    {
        static void Main(string[] args)
        {
            string[] inputDirs = new string[]
            {
                @"d:\_SW_Projects\Quantify_Image_Quality\WSI_160214016_Cont\",
                @"d:\_SW_Projects\Quantify_Image_Quality\homogenity test wins13\",
            };

            //IMethodManager mm1 = new MethodManager1(inputDirs);
            //IMethodManager mm2 = new MethodManager2(inputDirs);

            IMethodManager mm3 = new MethodManager3(inputDirs, 2048, 2048);
            //IMethodManager mm3 = new MethodManager3(inputDirs, 4096, 4096);

            mm3.Init();
            mm3.Run();
            

            Console.ReadKey();

        }
    }
}
