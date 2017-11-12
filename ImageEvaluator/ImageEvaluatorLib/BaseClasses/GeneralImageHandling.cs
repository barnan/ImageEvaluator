using Emgu.CV;
using ImageEvaluatorInterfaces.BaseClasses;
using NLog;
using System;
using System.Globalization;
using System.IO;

namespace ImageEvaluatorLib.BaseClasses
{
    static class GeneralImageHandling
    {
        static string ClassName { get; }

        static GeneralImageHandling()
        {
            ClassName = nameof(GeneralImageHandling);

        }


        public static bool SaveImage<TColor, TDepth>(string fileName, string folderExt, string fileNameExt, Image<TColor, TDepth> image, ILogger logger)
            where TColor : struct, IColor
            where TDepth : new()
        {
            try
            {
                string finalOutputName = CheckOutputDirectoryOfImageSaving(fileName, folderExt, fileNameExt, ".png");

                if (finalOutputName != null)
                {
                    image.Save(finalOutputName);
                }

                return true;
            }
            catch (Exception ex)
            {
                logger?.ErrorLog($"Exception occured: {ex}", ClassName);
                return false;
            }
        }


        public static string CheckOutputDirectoryOfImageSaving(string fileName, string folderExt, string fileNameExt, string extension)
        {
            try
            {
                string fileNameBase = Path.GetFileNameWithoutExtension(fileName);
                string path = Path.GetDirectoryName(fileName);
                string finalOutputName = Path.Combine(path ?? string.Empty, "_" + folderExt, fileNameBase + "_" + fileNameExt + extension);

                string directory = Path.GetDirectoryName(finalOutputName);
                if (directory != null && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                return finalOutputName;
            }
            catch (Exception)
            {
                throw;
            }
        }



        public static bool CheckImages<TColor1, TDepth1, TColor2, TDepth2>(Image<TColor1, TDepth1> image1, Image<TColor2, TDepth2> image2, int width, int height, ILogger logger)
            where TColor1 : struct, IColor
            where TDepth1 : new()
            where TColor2 : struct, IColor
            where TDepth2 : new()
        {
            try
            {
                if (image1 == null || image1.Height != height || image1.Width != width)
                {
                    logger?.ErrorLog($"Error in the image1 size. Predefined width: {width}, Predefined height: {height}, image1 width: {image1?.Width}, image1 height: {image1?.Height}", ClassName);
                    return false;
                }
                if (image2 == null || image2.Height != height || image2.Width != width)
                {
                    logger?.ErrorLog($"Error in the image2 size. Predefined width: {width}, Predefined height: {height}, image2 width: {image2?.Width}, image2 height: {image2?.Height}", ClassName);
                    return false;
                }
            }
            catch (Exception ex)
            {
                logger?.ErrorLog($"Exception occured: {ex}", ClassName);
            }
            return true;
        }



        public static bool Save1DArrayToCsv<T>(string name, string folderExt, string fileNameExt, string extension, T[] data, ILogger logger)
            where T : struct
        {
            try
            {
                CultureInfo cultInfo = CultureInfo.InvariantCulture;

                string finalOutputName = CheckOutputDirectoryOfImageSaving(name, folderExt, fileNameExt, ".csv");

                using (StreamWriter sw = new StreamWriter(finalOutputName))
                {
                    for (int i = 0; i < data.Length; i++)
                    {
                        sw.WriteLine($"{i},{data[i]}");
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                logger?.ErrorLog($"Exception occured: {ex}", ClassName);
                return false;
            }
        }



        public static bool SaveHistogram(string name, string folderExt, string fileNameExt, DenseHistogram hist, ILogger logger)
        {
            try
            {
                float[] Hist = hist.GetBinValues();
                Save1DArrayToCsv<float>(name, folderExt, fileNameExt, ".csv", Hist, logger);
                return true;
            }
            catch (Exception ex)
            {
                logger?.ErrorLog($"Exception occured: {ex}", ClassName);
                return false;
            }
        }



    }
}
