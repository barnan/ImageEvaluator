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
        private string _directoryName;
        private string _extension;
        private int _currentImageNumber;
        private string[] _fileList;
        private IDoubleLightImageReader _reader;
        private ILogger _logger;
        private bool _initialized;



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
            try
            {
                if (!Directory.Exists(_directoryName))
                {
                    _logger?.Error($"The directory ({ _directoryName}) does not exist -> _initialized=false.");
                    return _initialized = false;
                }

                List<string> allList = new List<string>(Directory.GetFiles(_directoryName));
                _fileList = allList.Where(p => Path.GetExtension(p) == $".{_extension}").ToArray();

                if (_fileList == null || _fileList.Length < 0)
                {
                    _logger?.Error($"The directory ({_directoryName}) contains NO files with the given extension: ({ _extension}) -> _initialized=false.");
                    return _initialized = false;
                }
            }
            catch (Exception ex)
            {
                _logger?.Error($"Exception during DirReader-CheckDir: {ex.Message}");
                return _initialized = false;
            }

            return true;
        }



        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool Init()
        {
            _initialized = (CheckDir() && _reader.Init());
            _logger?.Trace("DirectoryReader " + (_initialized ? string.Empty : "NOT") + " initializated.");

            return _initialized;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool GetNextImage(ref Image<Gray, float> img1, ref Image<Gray, float> img2)
        {
            if (!_initialized)
            {
                _logger?.Error($"The directory reader is not initialized yet.");
                return false;
            }

            int maxLength = _fileList.Length;

            if (_currentImageNumber >= maxLength - 1)
                return false;

            try
            {
                bool resu = _reader.GetImage(_fileList[_currentImageNumber], ref img1, ref img2);

                if (resu)
                {
                    _currentImageNumber++;
                    _logger?.Trace($"Image arrived. CurrentImageNumber: {_currentImageNumber}");
                    return true;
                }
                else
                {
                    _logger?.Trace($"Image couldn't get. CurrentImageNumber: {_currentImageNumber}");
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.Trace($"Exception in GetNextImage: {ex.Message}");
                return false;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool Restart()
        {
            bool resu = Init();

            _currentImageNumber = 0;
            return resu;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool EndOfDirectory()
        {
            if (!_initialized)
                return true;

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

            var dirReader = new DirectoryReader(logger, directoryName, extension, reader);
            dirReader.Init();

            return dirReader;
        }
    }





}
