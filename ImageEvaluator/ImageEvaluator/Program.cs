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
                @"f:\Quantify_Image_Quality\WSI_160214016_Chip\",
                @"d:\_SW_Projects\Quantify_Image_Quality\WSI_170214004_Chip\",
                @"d:\_SW_Projects\Quantify_Image_Quality\homogenity test wins13\",
            };

            //IMethodManager mm1 = new MethodManager1(inputDirs);
            //IMethodManager mm2 = new MethodManager2(inputDirs);

            //IMethodManager mm3 = new MethodManager3(inputDirs, 2048, 2048);
            IMethodManager mm3 = new MethodManager3(inputDirs, 4096, 4096);
            //IMethodManager mm3 = new MethodManager3(inputDirs, 8192, 8192);

            mm3.Init();
            mm3.Execute();


            Console.ReadKey();

        }
    }
}
