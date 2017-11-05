
using Emgu.CV;
using Emgu.CV.Structure;
using NLog;
using System;
using ImageEvaluatorInterfaces;
using System.Collections.Generic;
using ImageEvaluatorInterfaces.BaseClasses;

namespace ImageEvaluatorLib.CalculateStatisticalData
{
    class CalculateColumnDataCSharp1 : CalculateColumnDataBase
    {


        internal CalculateColumnDataCSharp1(ILogger logger, int width, int height)
            : base(logger, width, height)
        {
            ClassName = nameof(CalculateColumnDataCSharp1);
            Title = ClassName;

            _logger?.InfoLog($"Instantiated.", ClassName);
        }


        public override bool Execute(List<NamedData> data, string path)
        {
            Image<Gray, ushort>[] rawImages = null;
            Image<Gray, byte>[] maskImages = null;
            BorderPointArrays borderPointarrays = null;


            double[,,] resultVector1 = _firstVector.Data;
            double[,,] resultVector2 = _secondVector.Data;

            try
            {
                if (!IsInitialized)
                {
                    _logger?.ErrorLog($"It is not initialized", ClassName);
                    return false;
                }

                int imageCounter = LoadNamedData(data, ref borderPointarrays, ref rawImages, ref maskImages);
                if (imageCounter == 0)
                {
                    _logger?.InfoLog($"No images were loaded from dynamicresult", ClassName);
                }

                for (int m = 0; m < imageCounter; m++)
                {

                    if (!CheckInputData(rawImages[m], maskImages[m], borderPointarrays[m], _firstVector, _secondVector))
                    {
                        _logger?.InfoLog($"Input and mask data is not proper!", ClassName);
                        continue;
                    }

                    ushort[,,] imgData = rawImages[m].Data;

                    for (int i = 0; i < borderPointarrays[m].Length / 2; i++)
                    {

                        double sum = 0;
                        double sum2 = 0;
                        int counter = 0;

                        for (int j = 0; j < (borderPointarrays[m][i, 1] - borderPointarrays[m][i, 0]); j++)
                        {
                            sum += imgData[i, borderPointarrays[m][i, 0] + j, 0];
                            counter++;
                        }

                        resultVector1[0, i, 0] = (float)(sum / counter);
                        counter = 0;

                        for (int j = 0; j < (borderPointarrays[m][i, 1] - borderPointarrays[m][i, 0]); j++)
                        {
                            sum2 += Math.Pow(imgData[i, borderPointarrays[m][i, 0] + j, 0] - resultVector1[0, i, 0], 2);
                            counter++;
                        }

                        resultVector2[i, 0, 0] = (float)Math.Sqrt(sum2 / (counter - 1));

                    }
                }

                return true;

            }
            catch (Exception ex)
            {
                _logger?.ErrorLog($"Exception occured: {ex}.", ClassName);
                return false;
            }
        }

    }




    /// <summary>
    /// 
    /// </summary>
    public class FactoryCalculateColumnDataCSharp1 : IColumnDataCalculator_Creator
    {
        public IColumnDataCalculator Factory(ILogger logger, int width, int height)
        {
            logger?.InfoLog($"Factory called.", nameof(FactoryCalculateColumnDataEmgu1));

            return new CalculateColumnDataCSharp1(logger, width, height);
        }
    }


}
