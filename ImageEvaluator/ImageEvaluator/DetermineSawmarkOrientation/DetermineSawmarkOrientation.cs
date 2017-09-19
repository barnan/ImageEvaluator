using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Threading;
using Emgu.CV;
using Emgu.CV.Structure;
using ImageEvaluator.Interfaces;

namespace ImageEvaluator.DetermineSawmarkOrientation
{
    public class DetermineSawmarkOrientation : ISawmarkDeterminer
    {
        private IWaferOrientationDetector det;

        public DetermineSawmarkOrientation(IWaferOrientationDetector det)
        {
            this.det = det;
        }



        public bool Init()
        {
            return det.Init();
        }



        public void Run(Image<Gray, float> image, string name)
        {

            byte[] input = image.Bytes; //ReadDoubleLightImage(name);

            CancellationToken token = new CancellationToken();

            det.Run(input, token, 1.3, 10, 4, 50);


            //Image<Gray, float>[] inputVectors = ReadDoubleLightImage(name);
            //Image<Gray, float> inputVector = inputVectors[0];
            //Image<Gray, float> inputVector2 = inputVectors[1];

            //Stopwatch ora = new Stopwatch();
            //ora.Start();

            //Console.WriteLine($"Futás kezdete: {ora.ElapsedMilliseconds}");

            //int param_LowerFreqLimit = 60; //82;
            //int param_UpperFreqLimit = 800; //410;


            //MCvScalar pre_mean = CvInvoke.Mean(inputVector);
            ////MCvScalar pre_mean2 = CvInvoke.Mean(inputVector);

            //inputVector._Mul(1000);
            //inputVector2._Mul(1000);
            //Image<Gray, float> blurredInputVector = new Image<Gray, float>(inputVector.Width, inputVector.Height);
            //Image<Gray, float> blurredInputVector2 = new Image<Gray, float>(inputVector.Width, inputVector.Height);
            //CvInvoke.GaussianBlur(inputVector, blurredInputVector, new Size(5, 5), 1, 1, BorderType.Reflect);
            //CvInvoke.GaussianBlur(inputVector2, blurredInputVector2, new Size(5, 5), 1, 1, BorderType.Reflect);


            ////ImageViewer.Show(inputVector);
            ////ImageViewer.Show(blurredInputVector);

            //DenseHistogram inputHist = new DenseHistogram(256, new RangeF(0f, 255000f));
            //DenseHistogram inputHist2 = new DenseHistogram(256, new RangeF(0f, 255000f));
            //inputHist.Calculate<float>(new Image<Gray, float>[] { blurredInputVector }, false, null);
            //inputHist2.Calculate<float>(new Image<Gray, float>[] { blurredInputVector2 }, false, null);

            //Image<Gray, float> Hist = inputHist.ToImage<Gray, float>();
            //Image<Gray, float> Hist2 = inputHist.ToImage<Gray, float>();

            //MCvScalar histMean = CvInvoke.Mean(blurredInputVector);
            //MCvScalar histMean2 = CvInvoke.Mean(blurredInputVector2);

            //int minHist = 0;
            //int maxHist = 0;
            //float[,,] histData = Hist.Data;
            //for (int i = 0; i < Hist.Height; i++)
            //{
            //    if (histData[i, 0, 0] > 5)
            //    {
            //        minHist = i;
            //        break;
            //    }
            //}

            //for (int i = Hist.Height - 1; i >= 0; i--)
            //{
            //    if (histData[i, 0, 0] > 5)
            //    {
            //        maxHist = i;
            //        break;
            //    }
            //}

            //int minHist2 = 0;
            //int maxHist2 = 0;
            //float[,,] histData2 = Hist2.Data;
            //for (int i = 0; i < Hist2.Height; i++)
            //{
            //    if (histData2[i, 0, 0] > 5)
            //    {
            //        minHist2 = i;
            //        break;
            //    }
            //}

            //for (int i = Hist2.Height - 1; i >= 0; i--)
            //{
            //    if (histData2[i, 0, 0] > 5)
            //    {
            //        maxHist2 = i;
            //        break;
            //    }
            //}


            //float histWidthRatio = (maxHist - minHist) / (float)histMean.V0;
            //float histWidthRatio2 = (maxHist2 - minHist2) / (float)histMean2.V0;

            //Image<Gray, float> lineScan = new Image<Gray, float>(1, inputVector.Height);
            //Image<Gray, float> lineScan2 = new Image<Gray, float>(1, inputVector2.Height);
            //CvInvoke.Reduce(blurredInputVector, lineScan, ReduceDimension.SingleCol, ReduceType.ReduceAvg);
            //CvInvoke.Reduce(blurredInputVector2, lineScan2, ReduceDimension.SingleCol, ReduceType.ReduceAvg);

            //using (StreamWriter sw = new StreamWriter("linescan.csv", false))
            //{
            //    for (int i = 0; i < lineScan.Height; i++)
            //    {
            //        sw.WriteLine(lineScan.Data[i, 0, 0]);
            //    }
            //}
            //using (StreamWriter sw = new StreamWriter("linescan2.csv", false))
            //{
            //    for (int i = 0; i < lineScan2.Height; i++)
            //    {
            //        sw.WriteLine(lineScan2.Data[i, 0, 0]);
            //    }
            //}

            //Matrix<float> matLineScan = new Matrix<float>(CvInvoke.GetOptimalDFTSize(lineScan.Rows), CvInvoke.GetOptimalDFTSize(lineScan.Cols));
            //Matrix<float> matLineScan2 = new Matrix<float>(CvInvoke.GetOptimalDFTSize(lineScan2.Rows), CvInvoke.GetOptimalDFTSize(lineScan2.Cols));

            //CvInvoke.CopyMakeBorder(lineScan.Mat, matLineScan, 0, matLineScan.Height - lineScan.Height, 0, matLineScan.Width - lineScan.Width, BorderType.Constant, new MCvScalar());
            //CvInvoke.CopyMakeBorder(lineScan2.Mat, matLineScan2, 0, matLineScan2.Height - lineScan2.Height, 0, matLineScan2.Width - lineScan2.Width, BorderType.Constant, new MCvScalar());

            //Matrix<float> dftIn = new Matrix<float>(matLineScan.Rows, matLineScan.Cols, 2);
            //Matrix<float> dftOut = new Matrix<float>(matLineScan.Rows, matLineScan.Cols, 2);
            //Matrix<float> dftInBlank = matLineScan.CopyBlank();

            //Matrix<float> dftIn2 = new Matrix<float>(matLineScan2.Rows, matLineScan2.Cols, 2);
            //Matrix<float> dftOut2 = new Matrix<float>(matLineScan2.Rows, matLineScan2.Cols, 2);
            //Matrix<float> dftInBlank2 = matLineScan2.CopyBlank();

            //using (VectorOfMat mv = new VectorOfMat(new Mat[] { matLineScan.Mat, dftInBlank.Mat }))
            //{
            //    CvInvoke.Merge(mv, dftIn);
            //}

            //using (VectorOfMat mv2 = new VectorOfMat(new Mat[] { matLineScan2.Mat, dftInBlank2.Mat }))
            //{
            //    CvInvoke.Merge(mv2, dftIn2);
            //}

            ////var im = lineScan.Resize(200, lineScan.Height, Inter.Linear, false);
            ////ImageViewer.Show(im, "kep");

            //CvInvoke.Dft(dftIn, dftOut, DxtType.Forward, 0);
            //CvInvoke.Dft(dftIn2, dftOut2, DxtType.Forward, 0);

            //Matrix<float> outReal = new Matrix<float>(matLineScan.Size);
            //Matrix<float> outRealPow = new Matrix<float>(matLineScan.Size);
            //Matrix<float> outImag = new Matrix<float>(matLineScan.Size);
            //Matrix<float> outImagPow = new Matrix<float>(matLineScan.Size);

            //Matrix<float> outReal2 = new Matrix<float>(matLineScan2.Size);
            //Matrix<float> outRealPow2 = new Matrix<float>(matLineScan2.Size);
            //Matrix<float> outImag2 = new Matrix<float>(matLineScan2.Size);
            //Matrix<float> outImagPow2 = new Matrix<float>(matLineScan2.Size);

            //using (VectorOfMat vm = new VectorOfMat())
            //{
            //    vm.Push(outReal.Mat);
            //    vm.Push(outImag.Mat);
            //    CvInvoke.Split(dftOut, vm);
            //}
            //using (VectorOfMat vm2 = new VectorOfMat())
            //{
            //    vm2.Push(outReal2.Mat);
            //    vm2.Push(outImag2.Mat);
            //    CvInvoke.Split(dftOut2, vm2);
            //}

            //CvInvoke.Pow(outReal, 2, outRealPow);
            //CvInvoke.Pow(outImag, 2, outImagPow);
            //CvInvoke.Pow(outReal2, 2, outRealPow2);
            //CvInvoke.Pow(outImag2, 2, outImagPow2);

            //Matrix<float> powerSpectrum = outRealPow + outImagPow;
            //Matrix<float> powerSpectrum2 = outRealPow2 + outImagPow2;

            //CvInvoke.Sqrt(powerSpectrum, powerSpectrum);
            //CvInvoke.Sqrt(powerSpectrum2, powerSpectrum2);


            //Image<Gray, byte> freqRangeMask = new Image<Gray, byte>(lineScan.Width, lineScan.Height);
            //freqRangeMask.SetValue(0);
            //freqRangeMask.ROI = new Rectangle(0, param_LowerFreqLimit, 1, param_UpperFreqLimit - param_LowerFreqLimit);
            //freqRangeMask.SetValue(255);
            //freqRangeMask.ROI = Rectangle.Empty;


            //MCvScalar meanResult = CvInvoke.Mean(powerSpectrum, freqRangeMask);
            //MCvScalar origMeanResult = CvInvoke.Mean(inputVector);


            //MCvScalar meanResult2 = CvInvoke.Mean(powerSpectrum2, freqRangeMask);
            //MCvScalar origMeanResult2 = CvInvoke.Mean(inputVector2);

            ////var im2 = flineScan.ToImage<Gray, float>().Resize(200, lineScan.Height/2, Inter.Linear, false);
            ////ImageViewer.Show(im2, "kep2");

            //Console.WriteLine($"Futás vége: {ora.ElapsedMilliseconds}");

            //using (StreamWriter sw = new StreamWriter("DFT_results.csv", true))
            //{
            //    sw.WriteLine(histWidthRatio + "," + meanResult.V0 + "," + origMeanResult.V0 + "," + histWidthRatio2 + "," + meanResult2.V0 + "," + origMeanResult2.V0);
            //}

        }

        
    }


    public class Factory_DetermineSawmarkOrientation : ISawmarkDeterminer_Creator
    {
        public ISawmarkDeterminer Factory(IWaferOrientationDetector detector)
        {
            return new DetermineSawmarkOrientation(detector);
        }
    }
}
