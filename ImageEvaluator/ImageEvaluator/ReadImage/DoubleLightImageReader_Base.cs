using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.IO;

namespace ImageEvaluator.ReadImage
{
    abstract class DoubleLightImageReader_Base : IDoubleLightImageReader
    {
        protected string _fileName;
        protected int _width;
        protected int _height;
        protected int _bitNumber;
        private int width;
        protected Image<Gray, float> _img1;
        protected Image<Gray, float> _img2;
        private bool _initialized;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="width"></param>
        public DoubleLightImageReader_Base(int width)
        {
            if (width > 10000 || width < 0)
                return;

            this.width = width;
            this._height = width * 2;

            InitEmguImages();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputfileName"></param>
        /// <param name="img1"></param>
        /// <param name="immg2"></param>
        public bool GetImage(string inputfileName, ref Image<Gray, float> img1, ref Image<Gray, float> img2, ref string outmessage)
        {
            if (!CheckFileName(inputfileName))
            {
                outmessage = $"The file name is invalid. It does not exists or the width, height are invalid. ";
                return false;
            }

            try
            {
                InitEmguImages();

                img1 = _img1;
                img2 = _img2;
            }
            catch (Exception ex)
            {
                outmessage = $"Error during emgu image allocation. {ex.Message}";
                return false;
            }

            try
            {
                ReadDoubleLightImage();

                return true;
            }
            catch (Exception ex)
            {
                ClearEmguImages();

                outmessage = $"Error during double light image reading. {ex.Message}";

                return false;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected abstract void ReadDoubleLightImage();


        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputfileName"></param>
        /// <returns></returns>
        protected bool CheckFileName(string inputfileName)
        {
            if (!File.Exists(inputfileName))
                return false;

            FileInfo fi = new FileInfo(inputfileName);
            if (fi.Length != (_width * _height * _bitNumber))
                return false;

            return true;
        }


        /// <summary>
        /// 
        /// </summary>
        private bool InitEmguImages()
        {
            if (_initialized)
                return true;

            try
            {
                _img1 = new Image<Gray, float>(_width, _height);
                _img2 = new Image<Gray, float>(_width, _height);

                return _initialized = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"DoubleLightImageReader_Base - InitEmguImages. Error during emgu initialization. {ex.Message}");
                return _initialized = false;
            }

        }


        /// <summary>
        /// 
        /// </summary>
        private bool ClearEmguImages()
        {
            _img1?.Dispose();
            _img2?.Dispose();

            return !(_initialized = false);
        }


    }
}
