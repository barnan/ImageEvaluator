using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using ImageEvaluatorInterfaces;
using NLog;
using ImageEvaluatorInterfaces.BaseClasses;

namespace ImageEvaluatorLib.ReadDirectory
{

    class DirectoryReader : IDirectoryReader, IElement
    {
        private string _directoryName;
        private string _extension;
        private int _currentImageNumber;
        private string[] _fileList;
        private IImageReader _reader;
        private ILogger _logger;



        internal DirectoryReader(ILogger logger, string directoryName, string extension, IImageReader reader)
        {
            _logger = LogManager.GetCurrentClassLogger();
            _directoryName = directoryName;
            _extension = extension;
            _reader = reader;

            ClassName = nameof(DirectoryReader);
            Title = ClassName;

            _logger?.InfoLog($"Instantiated.", ClassName);
        }



        private bool CheckDir()
        {
            try
            {
                if (!Directory.Exists(_directoryName))
                {
                    _logger?.ErrorLog($"The directory ({ _directoryName}) does not exist -> _initialized = false.", ClassName);
                    return IsInitialized = false;
                }

                List<string> allList = new List<string>(Directory.GetFiles(_directoryName));
                _fileList = allList.Where(p => Path.GetExtension(p) == $".{_extension}").ToArray();

                if (_fileList == null || _fileList.Length < 0)
                {
                    _logger?.ErrorLog($"The directory ({_directoryName}) contains NO files with the given extension: ({ _extension}) -> _initialized=false.", ClassName);
                    return IsInitialized = false;
                }
            }
            catch (Exception ex)
            {
                _logger?.ErrorLog($"Exception occured: {ex}", ClassName);
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
            _logger?.TraceLog((IsInitialized ? string.Empty : "NOT") + " initialized.", ClassName);

            return IsInitialized;
        }

        public bool IsInitialized { get; protected set; }

        public string ClassName { get; }

        public string Title { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool GetNextImage(List<NamedData> data, ref string name)
        {
            if (!IsInitialized)
            {
                _logger?.ErrorLog($"It is not initialized yet.", ClassName);
                return false;
            }

            int maxLength = _fileList.Length;

            if (_currentImageNumber >= maxLength)
            {
                _logger?.InfoLog($"All images are read from the given input folder: {_directoryName}", ClassName);
                return false;
            }

            try
            {
                bool resu = _reader.GetImage(_fileList[_currentImageNumber], data);

                if (resu)
                {
                    name = _fileList[_currentImageNumber];
                    _currentImageNumber++;
                    _logger?.TraceLog($"Image arrived. CurrentImageNumber: {_currentImageNumber}", ClassName);
                    return true;
                }

                _logger?.TraceLog($"Image couldn't get. CurrentImageNumber: {_currentImageNumber}", ClassName);
                return false;
            }
            catch (Exception ex)
            {
                _logger?.TraceLog($"Exception occured: {ex}", ClassName);
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
            logger?.InfoLog($"Factory called.", nameof(Factory_DirectoryReader));

            var dirReader = new DirectoryReader(logger, directoryName, extension, reader);
            //dirReader.Init();

            return dirReader;
        }
    }





}
