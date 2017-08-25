﻿using Emgu.CV;
using Emgu.CV.Structure;
using NLog;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using ImageEvaluator.Interfaces;

namespace ImageEvaluator.DataSaver
{
    class PngResultSaver : ResultSaver_Base
    {
        private Image<Gray, byte> _tempImage1;


        public PngResultSaver(string outputFolder, string prefix, ILogger logger)
            : base(outputFolder, prefix, logger)
        {
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        /// <param name="inputFileName"></param>
        /// <returns></returns>
        public override bool SaveResult(IMeasurementResult result, string inputFileName)
        {
            try
            {
                Type t = result.GetType();

                PropertyInfo[] props = t.GetProperties();
                var nameProp = props.Where(p => p.Name == "Name").ToArray();
                string name = (nameProp.Length > 0) ? (string) nameProp[0].GetValue(result) : string.Empty;

                foreach (PropertyInfo prop in props)
                {
                    object obj = prop.GetValue(result);

                    if (!(obj is Image<Gray, float>))
                        continue;

                    string fileNameBase = Path.GetFileNameWithoutExtension(inputFileName);
                    string finalOutputName = Path.Combine(_outputFolder, $"{fileNameBase}_{_prefix}_{prop.Name}.png");

                    _tempImage1 = (obj as Image<Gray, float>)?.Convert<Gray, byte>();
                    _tempImage1?.Save(finalOutputName);

                    _logger?.Trace("Image " + prop.Name + " " + ((obj is Image<Gray, float>) ? string.Empty : "NOT") + " saved.");
                }
            }
            catch (Exception ex)
            {
                _logger?.Error($"Exception in PngResultSaver-SaveResult: {ex.Message}");
                return false;
            }
            finally
            {
                _tempImage1?.Dispose();
            }

            return true;
        }

    }


    class Factory_PngResultSaver : IResultSaver_Creator
    {
        public IResultSaver Factory(string outputFolder, string prefix, ILogger logger)
        {
            return new PngResultSaver(outputFolder, prefix, logger);

        }
    }

}