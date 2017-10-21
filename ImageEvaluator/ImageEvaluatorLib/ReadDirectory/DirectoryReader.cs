using System;
using Emgu.CV;
using Emgu.CV.Structure;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using ImageEvaluatorInterfaces;
using NLog;

namespace ImageEvaluatorLib.ReadDirectory
{
    class DirectoryReader : IDirectoryReader
    {
        private string _directoryName;
        private string _extension;
        private int _currentImageNumber;
        private string[] _fileList;
        private IImageReader _reader;
        private ILogger _logger;



        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="directoryName"></param>
        /// <param name="extension"></param>
        /// <param name="reader"></param>
        internal DirectoryReader(ILogger logger, string directoryName, string extension, IImageReader reader)
        {
            _logger = LogManager.GetCurrentClassLogger();
            _directoryName = directoryName;
            _extension = extension;
            _reader = reader;

            _logger?.Info($"{this.GetType().Name} instantiated.");
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
                    return IsInitialized = false;
                }

                List<string> allList = new List<string>(Directory.GetFiles(_directoryName));
                _fileList = allList.Where(p => Path.GetExtension(p) == $".{_extension}").ToArray();

                if (_fileList == null || _fileList.Length < 0)
                {
                    _logger?.Error($"The directory ({_directoryName}) contains NO files with the given extension: ({ _extension}) -> _initialized=false.");
                    return IsInitialized = false;
                }
            }
            catch (Exception ex)
            {
                _logger?.Error($"Exception during DirReader-CheckDir: {ex.Message}");
                return IsInitialized = false;
            }

            return true;
        }



        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool Init()
        {
            IsInitialized = (CheckDir() && _reader.Init());
            _logger?.Trace("DirectoryReader " + (IsInitialized ? string.Empty : "NOT") + " initialized.");

            return IsInitialized;
        }

        public bool IsInitialized { get; protected set; }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool GetNextImage(ref Image<Gray, byte>[] img, ref string name)
        {
            if (!IsInitialized)
            {
                _logger?.Error("The directory reader is not initialized yet.");
                return false;
            }

            int maxLength = _fileList.Length;

            if (_currentImageNumber >= maxLength)
                return false;

            try
            {
                bool resu = _reader.GetImage(_fileList[_currentImageNumber], ref img);

                if (resu)
                {
                    name = _fileList[_currentImageNumber];
                    _currentImageNumber++;
                    _logger?.Trace($"Image arrived. CurrentImageNumber: {_currentImageNumber}");
                    return true;
                }

                _logger?.Trace($"Image couldn't get. CurrentImageNumber: {_currentImageNumber}");
                return false;
            }
            catch (Exception ex)
            {
                _logger?.Trace($"Exception in GetNextImage: {ex}");
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
        public bool IsEndOfDirectory()
        {
            if (!IsInitialized)
                return true;

            int maxLength = _fileList.Length;

            return _currentImageNumber >= (maxLength) ? true : false;
        }


    }




    /// <summary>
    /// 
    /// </summary>
    public class Factory_DirectoryReader : IDirectoryReader_Creator
    {
        public IDirectoryReader Factory(ILogger logger, string directoryName, string extension, IImageReader reader)
        {
            logger?.Info($"{typeof(Factory_DirectoryReader).ToString()} factory called.");
            var dirReader = new DirectoryReader(logger, directoryName, extension, reader);
            //dirReader.Init();

            return dirReader;
        }
    }





}
