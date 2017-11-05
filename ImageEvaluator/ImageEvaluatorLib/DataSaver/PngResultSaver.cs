using Emgu.CV;
using Emgu.CV.Structure;
using NLog;
using System;
using System.IO;
using System.Reflection;
using ImageEvaluatorInterfaces;
using ImageEvaluatorInterfaces.BaseClasses;
using System.Collections.Generic;

namespace ImageEvaluatorLib.DataSaver
{
    class PngResultSaver : ResultSaverBase
    {
        private Image<Gray, byte> _tempImage1;


        public PngResultSaver(string outputFolder, string prefix, ILogger logger)
            : base(outputFolder, prefix, logger)
        {
            ClassName = nameof(PngResultSaver);
            Title = ClassName;

            _logger?.InfoLog("Instantiated.", ClassName);
        }


        public override bool SaveResult(IMeasurementResult result, string inputFileName, string ext)
        {
            try
            {
                Type t = result.GetType();

                PropertyInfo[] props = t.GetProperties();

                foreach (PropertyInfo prop in props)
                {
                    object obj = prop.GetValue(result);

                    if (!(obj is Image<Gray, float>))
                        continue;

                    string fileNameBase = Path.GetFileNameWithoutExtension(inputFileName);
                    string finalOutputName = Path.Combine(OutputFolder, $"{fileNameBase}_{_prefix}_{prop.Name}.png");

                    _tempImage1 = (obj as Image<Gray, float>).Convert<Gray, byte>();
                    _tempImage1?.Save(finalOutputName);

                    _logger?.TraceLog("Image " + prop.Name + " " + " is saved", ClassName);
                }
            }
            catch (Exception ex)
            {
                _logger?.ErrorLog($"Exception occured: {ex}", ClassName);
                return false;
            }
            finally
            {
                _tempImage1?.Dispose();
            }

            return true;
        }

    }


    public class FactoryPngResultSaver : IResultSaver_Creator
    {
        public IResultSaver Factory(string outputFolder, string prefix, ILogger logger)
        {
            logger?.Info("Factory called.", nameof(FactoryPngResultSaver));

            return new PngResultSaver(outputFolder, prefix, logger);
        }
    }

}
