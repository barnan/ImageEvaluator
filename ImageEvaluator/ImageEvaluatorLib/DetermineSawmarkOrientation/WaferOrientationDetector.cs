using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Threading;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using ImageEvaluatorInterfaces;
using NLog;

namespace ImageEvaluatorLib.DetermineSawmarkOrientation
{

    public class WaferOrientationDetector : IWaferOrientationDetector
    {
        private readonly ILogger _logger;
        private readonly object _detectorLock = new object();
        private int _originalImageWidth = 4096; //default values:
        private int _originalImageHeight = 4096;
        private int _linescanStartInPixel = 1024;
        private int _linescanEndInPixel = 3072;
        private int _sectionWidthInPixel = 10;
        private int _lowerFreqLimitIn1PerN = 60;
        private int _upperFreqLimitIn1PerN = 800;
        private bool _initialized;

        // emgu images:
        private Image<Gray, float> _inputSectionVertical;
        private Image<Gray, float> _inputSectionHorizontal;
        private Image<Gray, float> _inputSectionHorizontalTransposed;
        private Image<Gray, byte> _byteInputSectionHorizontalTransposed;
        private byte[] _rawInputSectionHorizontalTransposed;
        private Image<Gray, float> _blurredInputSectionVertical;
        private Image<Gray, float> _blurredInputSectionHorizontal;
        private Image<Gray, float> _lineScanVertical;
        private Image<Gray, float> _lineScanHorizontal;

        // emgu images for fourier transformation:
        private Matrix<float> _matLineScanVertical;
        private Matrix<float> _dftInBlankVertical;
        private Matrix<float> _dftInVertical;
        private Matrix<float> _dftOutVertical;

        private Matrix<float> _matLineScanHorizontal;
        private Matrix<float> _dftInBlankHorizontal;
        private Matrix<float> _dftInHorizontal;
        private Matrix<float> _dftOutHorizontal;

        private Matrix<float> _outRealVertical;
        private Matrix<float> _outRealVerticalPow;
        private Matrix<float> _outImagVertical;
        private Matrix<float> _outImagVerticalPow;

        private Matrix<float> _outRealHorizontal;
        private Matrix<float> _outRealHorizontalPow;
        private Matrix<float> _outImagHorizontal;
        private Matrix<float> _outImagHorizontalPow;

        private Matrix<float> _powerSpectrumVertical;
        private Matrix<float> _powerSpectrumHorizontal;

        private Image<Gray, byte> _freqRangeMask;



        #region properties

        public int OriginalImageWidth
        {
            get { return _originalImageWidth; }
            set
            {
                if (value < 10000 && value >= 0)
                {
                    _originalImageWidth = value;
                }
            }
        }

        public int OriginalImageHeight
        {
            get { return _originalImageHeight; }
            set
            {
                if (value < 10000 && value >= 0)
                {
                    _originalImageHeight = value;
                }
            }
        }

        public int LinescanStartInPixel
        {
            get { return _linescanStartInPixel; }
            set
            {
                if (value <= OriginalImageHeight && value >= 0)
                {
                    _linescanStartInPixel = value;
                }
            }
        }

        public int LinescanEndInPixel
        {
            get { return _linescanEndInPixel; }
            set
            {
                if (value <= OriginalImageHeight && value >= 0)
                {
                    _linescanEndInPixel = value;
                }
            }
        }

        public int SectionWidthInPixel
        {
            get { return _sectionWidthInPixel; }
            set
            {
                if (value < 50 && value >= 0)
                {
                    _sectionWidthInPixel = value;
                }
            }
        }

        public int LowerFreqLimitIn1PerN
        {
            get { return _lowerFreqLimitIn1PerN; }
            set
            {
                if (value < OriginalImageHeight / 2 && value >= 0)
                {
                    _lowerFreqLimitIn1PerN = value;
                }
            }
        }

        public int UpperFreqLimitIn1PerN
        {
            get { return _upperFreqLimitIn1PerN; }
            set
            {
                if (value < OriginalImageHeight / 2 || value >= 0)
                {
                    _upperFreqLimitIn1PerN = value;
                }
            }
        }



        #endregion


        public WaferOrientationDetector(ILogger logger, int originalImageWidth, int originalImageHeight, int linescanStartInPixel, int linescanEndInPixel)
        {
            _logger = logger;
            OriginalImageWidth = originalImageWidth;
            OriginalImageHeight = originalImageHeight;
            LinescanStartInPixel = linescanStartInPixel;
            LinescanEndInPixel = linescanEndInPixel;

            _logger?.Info("WaferOrientationDetector instantiated.");
        }



        public bool Init()
        {
            _initialized = CreateEmguImages();

            _logger?.Info("WaferOrientationDetector " + (_initialized ? string.Empty : "NOT") + " Initialized.");

            return _initialized;
        }


        private bool CreateEmguImages()
        {
            Monitor.Enter(_detectorLock);
            try
            {
                _inputSectionVertical = new Image<Gray, float>(_sectionWidthInPixel, _linescanEndInPixel - _linescanStartInPixel);
                _inputSectionHorizontal = new Image<Gray, float>(_sectionWidthInPixel, _linescanEndInPixel - _linescanStartInPixel);
                //_inputSectionHorizontalTransposed = new Image<Gray, float>(_linescanEnd - _linescanStart, _lineScanWidth);
                _byteInputSectionHorizontalTransposed = new Image<Gray, byte>(_linescanEndInPixel - _linescanStartInPixel, _sectionWidthInPixel);
                _rawInputSectionHorizontalTransposed = new byte[_sectionWidthInPixel * (_linescanEndInPixel - _linescanStartInPixel)];

                _blurredInputSectionVertical = new Image<Gray, float>(_inputSectionVertical.Width, _inputSectionVertical.Height);
                _blurredInputSectionHorizontal = new Image<Gray, float>(_inputSectionHorizontal.Width, _inputSectionHorizontal.Height);

                _lineScanVertical = new Image<Gray, float>(1, _blurredInputSectionVertical.Height);
                _lineScanHorizontal = new Image<Gray, float>(1, _blurredInputSectionHorizontal.Height);

                _matLineScanVertical = new Matrix<float>(CvInvoke.GetOptimalDFTSize(_lineScanVertical.Rows), CvInvoke.GetOptimalDFTSize(_lineScanVertical.Cols));
                _dftInBlankVertical = _matLineScanVertical.CopyBlank();
                _dftInVertical = new Matrix<float>(_matLineScanVertical.Rows, _matLineScanVertical.Cols, 2);
                _dftOutVertical = new Matrix<float>(_matLineScanVertical.Rows, _matLineScanVertical.Cols, 2);

                _matLineScanHorizontal = new Matrix<float>(CvInvoke.GetOptimalDFTSize(_lineScanHorizontal.Rows), CvInvoke.GetOptimalDFTSize(_lineScanHorizontal.Cols));
                _dftInBlankHorizontal = _matLineScanHorizontal.CopyBlank();
                _dftInHorizontal = new Matrix<float>(_matLineScanHorizontal.Rows, _matLineScanHorizontal.Cols, 2);
                _dftOutHorizontal = new Matrix<float>(_matLineScanHorizontal.Rows, _matLineScanHorizontal.Cols, 2);

                _outRealVertical = new Matrix<float>(_matLineScanVertical.Size);
                _outRealVerticalPow = new Matrix<float>(_matLineScanVertical.Size);
                _outImagVertical = new Matrix<float>(_matLineScanVertical.Size);
                _outImagVerticalPow = new Matrix<float>(_matLineScanVertical.Size);

                _outRealHorizontal = new Matrix<float>(_matLineScanHorizontal.Size);
                _outRealHorizontalPow = new Matrix<float>(_matLineScanHorizontal.Size);
                _outImagHorizontal = new Matrix<float>(_matLineScanHorizontal.Size);
                _outImagHorizontalPow = new Matrix<float>(_matLineScanHorizontal.Size);

                // create freequency mask:
                _freqRangeMask = new Image<Gray, byte>(_lineScanVertical.Width, _lineScanVertical.Height);
                _freqRangeMask.SetValue(0);
                _freqRangeMask.ROI = new Rectangle(0, _lowerFreqLimitIn1PerN, 1, _upperFreqLimitIn1PerN - _lowerFreqLimitIn1PerN);
                _freqRangeMask.SetValue(255);
                _freqRangeMask.ROI = Rectangle.Empty;

                _logger?.Info("WaferOrientationDetector - emgu images created.");
            }
            catch (Exception ex)
            {
                _logger?.Error($"Exception in WaferOrientationDetector-CreateEmguImages: {ex.Message}.");
                return _initialized = false;
            }
            finally
            {
                Monitor.Exit(_detectorLock);
            }

            return _initialized = true;
        }


        public bool DestroyAllEmguImages()
        {
            Monitor.Enter(_detectorLock);
            try
            {
                _inputSectionVertical?.Dispose();
                _inputSectionHorizontal?.Dispose();
                _inputSectionHorizontalTransposed?.Dispose();

                _blurredInputSectionVertical?.Dispose();
                _blurredInputSectionHorizontal?.Dispose();

                _lineScanVertical?.Dispose();
                _lineScanHorizontal?.Dispose();

                _matLineScanVertical?.Dispose();
                _dftInBlankVertical?.Dispose();
                _dftInVertical?.Dispose();
                _dftOutVertical?.Dispose();

                _matLineScanHorizontal?.Dispose();
                _dftInBlankHorizontal?.Dispose();
                _dftInHorizontal?.Dispose();
                _dftOutHorizontal?.Dispose();

                _outRealVertical?.Dispose();
                _outRealVerticalPow?.Dispose();
                _outImagVertical?.Dispose();
                _outImagVerticalPow?.Dispose();

                _outRealHorizontal?.Dispose();
                _outRealHorizontalPow?.Dispose();
                _outImagHorizontal?.Dispose();
                _outImagHorizontalPow?.Dispose();

                _freqRangeMask?.Dispose();

                _initialized = false;

                _logger?.Info("WaferOrientationDetector - emgu images destroyed.");

            }
            catch (Exception ex)
            {
                _logger?.Error($"Exception occured in DestroyAllEmguImages: {ex.Message}");
                return false;
            }
            finally
            {
                Monitor.Exit(_detectorLock);
            }

            return true;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputRawImage">4Kx8Kx8bit input RAW double light image</param>
        /// <param name="cancelToken"></param>
        /// <param name="orientationThresholdInAdu"></param>
        /// <param name="sectionWidthInPixel"></param>
        /// <param name="lowerSpatialLimitInPixel"></param>
        /// <param name="upperSpatialLimitInPixel"></param>
        /// <returns>WaferOrientation.NormalWaferOrientation OR WaferOrientation.RotatedWafer</returns>
        public WaferOrientation? Run(byte[] inputRawImage, CancellationToken cancelToken, double orientationThresholdInAdu, int sectionWidthInPixel, int lowerSpatialLimitInPixel, int upperSpatialLimitInPixel)
        {
            WaferOrientation? orienatation = null;

            Monitor.Enter(_detectorLock);
            try
            {
                Stopwatch swatch = new Stopwatch();
                swatch.Start();

                // Variable initlization (and check):  ---------------------------------------------------------------------------------------------
                if (!_initialized)
                    return null;

                _logger?.Info("WaferOrientationDetection started.");
                if (cancelToken.IsCancellationRequested)
                {
                    _logger?.Info("Runing of WaferOrientationDetector was stopped by cancellationtoken.");

                }

                // frequency calculation:
                SectionWidthInPixel = sectionWidthInPixel;
                LowerFreqLimitIn1PerN = (LinescanEndInPixel - LinescanStartInPixel) / upperSpatialLimitInPixel;
                UpperFreqLimitIn1PerN = (LinescanEndInPixel - LinescanStartInPixel) / lowerSpatialLimitInPixel;

                bool splitResult = SplitDoubleLightImage(inputRawImage);
                if (!splitResult || _inputSectionVertical == null || _inputSectionHorizontal == null)
                {
                    return null;
                }


                // Prepare the input images:  ------------------------------------------------------------------------------------------------------
                _inputSectionVertical._Mul(1000);
                _inputSectionHorizontal._Mul(1000);

                double gaussianSigmaSize = 1.0;     // MAGIC kernel size !!!!
                int gaussianKernelSize = (int)Math.Round(5 * gaussianSigmaSize, MidpointRounding.AwayFromZero);
                CvInvoke.GaussianBlur(_inputSectionVertical, _blurredInputSectionVertical, new Size(gaussianKernelSize, gaussianKernelSize), gaussianSigmaSize, gaussianSigmaSize, BorderType.Reflect);
                CvInvoke.GaussianBlur(_inputSectionHorizontal, _blurredInputSectionHorizontal, new Size(gaussianKernelSize, gaussianKernelSize), gaussianSigmaSize, gaussianSigmaSize, BorderType.Reflect);


                //create a single linescan from the image sections
                CvInvoke.Reduce(_blurredInputSectionVertical, _lineScanVertical, ReduceDimension.SingleCol, ReduceType.ReduceAvg);
                CvInvoke.Reduce(_blurredInputSectionHorizontal, _lineScanHorizontal, ReduceDimension.SingleCol, ReduceType.ReduceAvg);


                LogElapsedTime(swatch, "WaferOrientationDetector - linescan creation");
                if (cancelToken.IsCancellationRequested)
                {
                    _logger?.Info("Runing of WaferOrientationDetector was stopped by cancellationtoken.");
                    return null;
                }


                //create data-structures for dft ----------------------------------------------------------------------------------------------------
                _dftInVertical = new Matrix<float>(_matLineScanVertical.Rows, _matLineScanVertical.Cols, 2);
                _dftInHorizontal = new Matrix<float>(_matLineScanHorizontal.Rows, _matLineScanHorizontal.Cols, 2);

                CvInvoke.CopyMakeBorder(_lineScanVertical.Mat, _matLineScanVertical, 0, _matLineScanVertical.Height - _lineScanVertical.Height, 0,
                    _matLineScanVertical.Width - _lineScanVertical.Width, BorderType.Constant);
                CvInvoke.CopyMakeBorder(_lineScanHorizontal.Mat, _matLineScanHorizontal, 0, _matLineScanHorizontal.Height - _lineScanHorizontal.Height, 0,
                    _matLineScanHorizontal.Width - _lineScanHorizontal.Width, BorderType.Constant);

                using (VectorOfMat mvVertical = new VectorOfMat(_matLineScanVertical.Mat, _dftInBlankVertical.Mat))
                {
                    CvInvoke.Merge(mvVertical, _dftInVertical);
                }

                using (VectorOfMat mvHorizontal = new VectorOfMat(_matLineScanHorizontal.Mat, _dftInBlankHorizontal.Mat))
                {
                    CvInvoke.Merge(mvHorizontal, _dftInHorizontal);
                }

                // perform dft:
                CvInvoke.Dft(_dftInVertical, _dftOutVertical, DxtType.Forward, 0);
                CvInvoke.Dft(_dftInHorizontal, _dftOutHorizontal, DxtType.Forward, 0);


                LogElapsedTime(swatch, "WaferOrientationDetector - dft peformed");
                if (cancelToken.IsCancellationRequested)
                {
                    _logger?.Info("Runing of WaferOrientationDetector was stopped by cancellationtoken.");
                    return null;
                }

                //get data from dft-result:  ---------------------------------------------------------------------------------------------------------
                using (VectorOfMat vm = new VectorOfMat())
                {
                    vm.Push(_outRealVertical.Mat);
                    vm.Push(_outImagVertical.Mat);
                    CvInvoke.Split(_dftOutVertical, vm);
                }
                using (VectorOfMat vm2 = new VectorOfMat())
                {
                    vm2.Push(_outRealHorizontal.Mat);
                    vm2.Push(_outImagHorizontal.Mat);
                    CvInvoke.Split(_dftOutHorizontal, vm2);
                }

                // create power spectrum: ------------------------------------------------------------------------------------------------------------
                CvInvoke.Pow(_outRealVertical, 2, _outRealVerticalPow);
                CvInvoke.Pow(_outImagVertical, 2, _outImagVerticalPow);
                CvInvoke.Pow(_outRealHorizontal, 2, _outRealHorizontalPow);
                CvInvoke.Pow(_outImagHorizontal, 2, _outImagHorizontalPow);

                _powerSpectrumVertical = _outRealVerticalPow + _outImagVerticalPow;
                _powerSpectrumHorizontal = _outRealHorizontalPow + _outImagHorizontalPow;

                CvInvoke.Sqrt(_powerSpectrumVertical, _powerSpectrumVertical);
                CvInvoke.Sqrt(_powerSpectrumHorizontal, _powerSpectrumHorizontal);

                MCvScalar meanPowerVertical = CvInvoke.Mean(_powerSpectrumVertical, _freqRangeMask);
                MCvScalar meanPowerHorizontal = CvInvoke.Mean(_powerSpectrumHorizontal, _freqRangeMask);


                // decision point: --------------------------------------------------------------------------------------------------------------------
                orienatation = (meanPowerVertical.V0 / meanPowerHorizontal.V0) > orientationThresholdInAdu ? WaferOrientation.NormalWaferOrientation : WaferOrientation.RotatedWafer;

                LogElapsedTime(swatch, "WaferOrientationDetector calculation finished");
                //Console.WriteLine(swatch.ElapsedMilliseconds);


                //using (StreamWriter sw = new StreamWriter("DFT_results.csv", true))
                //{
                //    sw.WriteLine(meanPowerVertical.V0 + "," + meanPowerHorizontal.V0);
                //}
            }
            catch (Exception ex)
            {
                _logger?.Error($"Exception during WaferOrientation detection: {ex.Message}");
            }
            finally
            {
                Monitor.Exit(_detectorLock);
            }

            return orienatation;
        }


        /// <summary>
        /// Crops two linescans from the input RAW double light image (4Kx8Kx8bit.
        /// </summary>
        /// <param name="inputImage">input image that contains the souble light image</param>
        /// <returns></returns>
        private bool SplitDoubleLightImage(byte[] inputImage)
        {
            try
            {
                int verticalColumnCoord = _originalImageWidth / 2;
                int horizontalRowCoord = _originalImageHeight / 2;

                float[,,] outVerticalScanData = _inputSectionVertical.Data;

                //crop the vertical linescan ( ~ w10 x h2048):
                int scanHalf = SectionWidthInPixel / 2;
                for (int i = 0; i < (LinescanEndInPixel - LinescanStartInPixel); i++)
                {
                    int verticalIndex = (i + LinescanStartInPixel) * OriginalImageWidth * 2 + verticalColumnCoord;

                    for (int j = -scanHalf; j < scanHalf; j++)
                        outVerticalScanData[i, j + scanHalf, 0] = inputImage[verticalIndex + j];
                }

                // crop the horizontal linescan ( ~ w2048 x h10)
                for (int i = 0; i < SectionWidthInPixel; i++)
                {
                    int verticalIndex = (horizontalRowCoord - scanHalf + i) * OriginalImageWidth * 2 + LinescanStartInPixel;
                    Array.Copy(inputImage, verticalIndex, _rawInputSectionHorizontalTransposed, i * (LinescanEndInPixel - LinescanStartInPixel), (LinescanEndInPixel - LinescanStartInPixel));
                }
                _byteInputSectionHorizontalTransposed.Bytes = _rawInputSectionHorizontalTransposed;
                _inputSectionHorizontalTransposed = _byteInputSectionHorizontalTransposed.Convert<Gray, float>();
                CvInvoke.Transpose(_inputSectionHorizontalTransposed, _inputSectionHorizontal);
            }
            catch (Exception ex)
            {
                _logger.Error($"Exception occured in WaferOrientationDetector-SplitDoubleLightImage: {ex.Message}.");
                return false;
            }
            return true;
        }


        private void LogElapsedTime(Stopwatch watch1, string outermessage = null)
        {
            if (watch1 == null)
                return;

            string message = $"{outermessage ?? string.Empty}. Elapsed time: {watch1.ElapsedMilliseconds}";
            _logger?.Trace(message);

            watch1.Restart();
        }


    }


    public class Factory_WaferOrientationDetector : IWaferOrientationDetector_Creator
    {
        public IWaferOrientationDetector Factory(ILogger logger, int originalImageWidth, int originalImageHeight, int linescanHeightStartInPixel, int linescanHeightEndInPixel)
        {
            return new WaferOrientationDetector(logger, originalImageWidth, originalImageHeight, linescanHeightStartInPixel, linescanHeightEndInPixel);
        }
    }





}

