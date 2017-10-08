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
                @"f:\Quantify_Image_Quality\homogenity test wins13\"
            };

            IMethodManager mm1 = new MethodManager1(inputDirs);
            IMethodManager mm2 = new MethodManager2(inputDirs);


            mm1.Init();



            Console.ReadKey();

        }
    }
}
