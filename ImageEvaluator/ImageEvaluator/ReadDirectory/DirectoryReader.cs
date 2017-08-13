
using Emgu.CV;
using Emgu.CV.Structure;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace ImageEvaluator.ReadDirectory
{
    class DirectoryReader : IDirectoryReader
    {
        string _directoryName;
        string _extension;
        int _currentImageNumber;
        string[] _fileList;
        IDoubleLightImageReader _reader;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        public DirectoryReader(string directoryName, string extension, IDoubleLightImageReader_Creator reader, int width, int bitDepth)
        {
            _directoryName = directoryName;
            _extension = extension;
            _reader = reader.Factory(width, bitDepth);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool Init()
        {
            if (!CheckDir())
            {
                return false;
            }

            return true;
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
        public Image<Gray, float> GetNextImage()
        {
            int maxLength = _fileList.Length;




        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool Restart()
        {
            _currentImageNumber = 0;
            return true;
        }
    }








}
