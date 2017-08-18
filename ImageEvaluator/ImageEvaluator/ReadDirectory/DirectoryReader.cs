using System;
using Emgu.CV;
using Emgu.CV.Structure;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using NLog;

namespace ImageEvaluator.ReadDirectory
{
    class DirectoryReader : IDirectoryReader
    {
        string _directoryName;
        string _extension;
        int _currentImageNumber;
        string[] _fileList;
        IDoubleLightImageReader _reader;
        ILogger _logger;



        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        internal DirectoryReader(ILogger logger, string directoryName, string extension, IDoubleLightImageReader reader)
        {
            _logger = logger;
            _directoryName = directoryName;
            _extension = extension;
            _reader = reader;

            _logger?.Info("DirectoryReader instantiated.");

        }



        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private bool CheckDir()
        {
            if (!Directory.Exists(_directoryName))
            {
                return false;
            }

            var allList = new List<string>(Directory.GetFiles(_directoryName));
            _fileList = allList.Where(p => Path.GetExtension(p) == $".{_extension}").ToArray();

            if (_fileList == null || _fileList.Length < 0)
            {
                return false;
            }

            return true;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool GetNextImage(ref Image<Gray, float> img1, ref Image<Gray, float> img2, ref string message)
        {
            if (!CheckDir())
            {
                message = $"the directory ({_directoryName}) does not exist or contains no files with the given extension: ({_extension})";
                return false;
            }

            int maxLength = _fileList.Length;

            if (_currentImageNumber >= maxLength - 1)
                return false;

            try
            {
                bool resu = _reader.GetImage(_fileList[_currentImageNumber], ref img1, ref img2, ref message);
                _currentImageNumber++;

                if (resu)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetImage: {ex.Message}");
                return false;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool Restart()
        {
            CheckDir();

            _currentImageNumber = 0;
            return true;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool EndOfDirectory()
        {
            CheckDir();

            int maxLength = _fileList.Length;

            return _currentImageNumber >= (maxLength - 1) ? true : false;
        }
    }



    /// <summary>
    /// 
    /// </summary>
    public class Factory_DirectoryReader : IDirectoryReader_Creator
    {
        public IDirectoryReader Factory(ILogger logger, string directoryName, string extension, IDoubleLightImageReader reader)
        {
            return new DirectoryReader(logger, directoryName, extension, reader);
        }
    }





}
