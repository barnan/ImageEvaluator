
using Emgu.CV;
using Emgu.CV.Structure;
using NLog;
using System;
using ImageEvaluatorInterfaces;
using System.Collections.Generic;
using ImageEvaluatorInterfaces.BaseClasses;

namespace ImageEvaluatorLib.CalculateStatisticalData
{

    class CalculateColumnDataCSharp2 : CalculateColumnDataBase
    {

        internal CalculateColumnDataCSharp2(ILogger logger, int width, int height)
            : base(logger, width, height)
        {
            ClassName = nameof(CalculateColumnDataCSharp1);
            Title = ClassName;

            lorum = "Brightness";

            _logger?.InfoLog($"Instantiated.", ClassName);

        }


        public override bool Execute(List<NamedData> data, string fileName)
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
                    _logger.ErrorLog($"It is not initialized.", ClassName);
                    return false;
                }

                int imageCounter = LoadNamedData(data, ref borderPointarrays, ref rawImages, ref maskImages);
                if (imageCounter == 0)
                {
                    _logger?.Info($"No images were loaded from dynamicresult", ClassName);
                }

                for (int m = 0; m < imageCounter; m++)
                {
                    if (!CheckInputData(rawImages[m], maskImages[m], borderPointarrays[m], _firstVector, _secondVector))
                        return false;

                    ushort[,,] imgData = rawImages[m].Data;

                    for (int i = 0; i < borderPointarrays[m].Length / 2; i++)
                    {
                        try
                        {
                            double sum = 0;
                            double sum2 = 0;
                            int counter = 0;

                            for (int j = 0; j < (borderPointarrays[m][i, 1] - borderPointarrays[m][i, 0]); j++)
                            {
                                sum += imgData[i, borderPointarrays[m][i, 0] + j, 0];
                                sum2 += (imgData[i, borderPointarrays[m][i, 0] + j, 0] * imgData[i, borderPointarrays[m][i, 0] + j, 0]);
                                counter++;
                            }

                            resultVector1[0, i, 0] = (float)(sum / counter);
                            resultVector2[0, i, 0] = (float)Math.Sqrt(sum2 / (counter - 1) - (sum * sum / Math.Pow(counter - 1, 2)));

                        }
                        catch (Exception ex)
                        {
                            _logger?.ErrorLog($"Error during the calculation, i:{i}, message: {ex}.", ClassName);
                            return false;
                        }
                    }
                }

                return true;
            }
            catch (Exception)
            {

                throw;
            }
        }

    }



    /// <summary>
    /// 
    /// </summary>
    public class FactoryCalculateColumnDataCSharp2 : IColumnDataCalculator_Creator
    {
        public IColumnDataCalculator Factory(ILogger logger, int width, int height)
        {
            logger?.Info($"Factory called.", nameof(FactoryCalculateColumnDataEmgu1));

            return new CalculateColumnDataCSharp2(logger, width, height);
        }
    }


}
