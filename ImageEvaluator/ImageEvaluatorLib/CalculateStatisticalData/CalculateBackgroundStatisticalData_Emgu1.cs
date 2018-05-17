using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV;
using Emgu.CV.Structure;
using NLog;
using ImageEvaluatorInterfaces;
using ImageEvaluatorInterfaces.BaseClasses;

namespace ImageEvaluatorLib.CalculateStatisticalData
{
    class CalculateBackgroundStatisticalData_Emgu1 : CalculateColumnDataBaseEmgu
    {

        public CalculateBackgroundStatisticalData_Emgu1(ILogger logger, int width, int height)
            : base(logger, width, height)
        {
            ClassName = nameof(CalculateBackgroundStatisticalData_Emgu1);
            Title = "Column Mean and Std calculator";

            InitEmguImages();

            lorum = "Backgound";

            _logger?.InfoLog($"Instantiated.", ClassName);
        }



        protected override int[] Iterate(Image<Gray, ushort> rawImages, Image<Gray, byte> maskImages, int[,] borderPointarrays)
        {
            throw new NotImplementedException();
        }



    }



    public class Factory_CalculateBackgroundStatisticalData_Emgu1 : IColumnDataCalculator_Creator
    {
        public IColumnDataCalculator Factory(ILogger logger, int width, int height)
        {
            logger?.InfoLog($"Factory called.", nameof(Factory_CalculateBackgroundStatisticalData_Emgu1));

            return new CalculateBackgroundStatisticalData_Emgu1(logger, width, height);
        }
    }



}
