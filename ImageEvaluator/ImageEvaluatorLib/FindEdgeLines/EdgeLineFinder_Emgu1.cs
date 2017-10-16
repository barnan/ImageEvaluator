using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using ImageEvaluatorInterfaces;
using NLog;

namespace ImageEvaluatorLib.FindEdgeLines
{
    class EdgeLineFinder_Emgu1 : EdgeLineFinderBase
    {
        public EdgeLineFinder_Emgu1(ILogger logger, Dictionary<SearchOrientations, Rectangle> calcareas) 
            : base(logger, calcareas)
        {
        }

        public override bool Run(Image<Gray, ushort> originalImage, Image<Gray, byte> maskImage, ref IWaferEdgeFindData edgeFindData)
        {

            if (!CheckInputData(originalImage, maskImage, _calcAreas))
            {
                return false;
            }

            //Point[] topPoints = FindPoints(maskImage, _calcAreas[SearchOrientations.TopToBottom], SearchOrientations.TopToBottom);
            //Point[] bottomPoints = FindPoints(maskImage, _calcAreas[SearchOrientations.BottomToTop], SearchOrientations.BottomToTop);
            //Point[] leftPoints = FindPoints(maskImage, _calcAreas[SearchOrientations.LeftToRight], SearchOrientations.LeftToRight);
            //Point[] rightPoints = FindPoints(maskImage, _calcAreas[SearchOrientations.RightToLeft], SearchOrientations.RightToLeft);

            //if (topPoints?.Length < 1 || bottomPoints?.Length < 1 || leftPoints?.Length < 1 || rightPoints?.Length < 1)
            //{
            //    _logger?.Trace("The found edge segments are not proper in EdgeLineFinder_CSharp1");
            //    return false;
            //}

            //WaferEdgeFindData result = new WaferEdgeFindData
            //{
            //    TopSide = new VectorOfPoint(topPoints),
            //    BottomSide = new VectorOfPoint(bottomPoints),
            //    LeftSide = new VectorOfPoint(leftPoints),
            //    RightSide = new VectorOfPoint(rightPoints)
            //};

            //edgeFindData = result;

            return true;

        }



        protected override bool CheckInputData(Image<Gray, ushort> originalImage, Image<Gray, byte> maskImage, Dictionary<SearchOrientations, Rectangle> calcAreas)
        {
            if (originalImage == null )
            {
                _logger?.Trace("EdgeLineFinder - the input data is not proper, some of them is null.");
                return false;
            }

            return true;
        }





        public void TestMethod(Image<Gray, float> inputImage)
        {
            Image<Gray, byte> workImage = inputImage.Convert<Gray, byte>();
            Image<Gray, byte> sampleMask = new Image<Gray, byte>(workImage.Size);

            int darkLimit = 40;
            Gray _white = new Gray(1);
            float CONTOUR_AREA_LIMIT_RATIO = 0.01f;
            int width = workImage.Width;
            int height = workImage.Height;

            var maskImage = workImage.ThresholdBinary(new Gray(darkLimit), _white);
            var finalWaferContour = new VectorOfVectorOfPoint();

            using (Mat hierachy = new Mat())
            {
                using (VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint())
                {
                    CvInvoke.FindContours(maskImage, contours, hierachy, RetrType.List, ChainApproxMethod.ChainApproxNone);

                    for (int i = 0; i < contours.Size; i++)
                    {
                        var currentContour = contours[i];

                        var contourArea = CvInvoke.ContourArea(currentContour);
                        if (contourArea > workImage.Height * workImage.Width * CONTOUR_AREA_LIMIT_RATIO)
                        {
                            finalWaferContour.Push(currentContour);
                        }
                    }
                }
            }

            for (int i = 0; i < finalWaferContour.Size; i++)
            {
                CvInvoke.DrawContours(sampleMask, finalWaferContour, i, new MCvScalar(1), -1);
            }

            workImage._Mul(sampleMask);
            //sampleMask.Save("mask.png");

            MCvScalar waferMean = new MCvScalar();
            MCvScalar waferStd = new MCvScalar();
            CvInvoke.MeanStdDev(workImage, ref waferMean, ref waferStd, sampleMask);

            double nSigma = 0;
            double gradLimit = waferMean.V0 - nSigma * waferStd.V0;

            Image<Gray, float> gx = workImage.Sobel(1, 0, 3);
            Image<Gray, float> gy = workImage.Sobel(0, 1, 3);

            gx = gx.AbsDiff(new Gray(0));
            gy = gy.AbsDiff(new Gray(0));

            SaveFITS(gx.Convert<Gray, double>().Data, "sobelX.fits");
            SaveFITS(gy.Convert<Gray, double>().Data, "sobelY.fits");

            CvInvoke.Transpose(gy, gy);

            SaveFITS(gy.Convert<Gray, double>().Data, "sobelYT.fits");



            float leftSlope, leftIntercept, leftLineSpread;
            float rightSlope, rightIntercept, rightLineSpread;
            float topSlope, topIntercept, topLineSpread;
            float bottomSlope, bottomIntercept, bottomLineSpread;
            VectorOfFloat leftLineData = FitEdge(gx, height / 4, 3 * height / 4, 0, width / 8, gradLimit, out leftSlope, out leftIntercept, out leftLineSpread);
            VectorOfFloat rightLineData = FitEdge(gx, height / 4, 3 * height / 4, 7 * width / 8, width, gradLimit, out rightSlope, out rightIntercept, out rightLineSpread);
            VectorOfFloat topLineData = FitEdge(gy, width / 4, 3 * width / 4, 0, height / 8, gradLimit, out topSlope, out topIntercept, out topLineSpread);
            VectorOfFloat bottomLineData = FitEdge(gy, width / 4, 3 * width / 4, 7 * height / 8, height, gradLimit, out bottomSlope, out bottomIntercept, out bottomLineSpread);

            for (int c = 0; c < width; c++)
            {
                workImage.Data[(int)(topSlope * c + topIntercept), c, 0] = 255;
                workImage.Data[(int)(bottomSlope * c + bottomIntercept), c, 0] = 255;
            }
            for (int r = 0; r < height; r++)
            {
                workImage.Data[r, (int)(leftSlope * r + leftIntercept), 0] = 255;
                workImage.Data[r, (int)(rightSlope * r + rightIntercept), 0] = 255;
            }

            workImage.Save("sidepointsFit.png");

            //return inputImage;

        }





        public VectorOfFloat FitEdge(Image<Gray, float> sobelImage, int startRow, int endRow, int startCol, int endCol, double gradLimit, out float slope, out float intercept, out float lineSpread)
        {
            List<PointF> edgePoints = new List<PointF>();
            var sobelData = sobelImage.Data;
            int stride = 1;

            List<float> fullWidthHalfMaximumVals = new List<float>();

            for (int r = startRow; r < endRow; r += stride)
            {
                float mean = 0;
                float variance = 0;
                float nData = 0;

                for (int c = startCol; c < endCol; c++)
                {
                    var currentValue = sobelData[r, c, 0];
                    if (currentValue > gradLimit)
                    {
                        mean += currentValue * c;
                        nData += currentValue;
                    }
                }

                if (nData != 0)
                {
                    mean /= nData;
                }

                nData = 0;
                int allowedRange = 30;
                //TODO: check if mean is not 0
                //Fix Belt handling
                for (int c = startCol; c < endCol; c++)
                {
                    var currentValue = sobelData[r, c, 0];
                    if (currentValue > gradLimit)
                    {
                        variance += currentValue * (float)Math.Pow(c - mean, 2);
                        nData += currentValue;
                    }
                }

                if (nData != 0)
                {
                    variance /= nData;
                }

                float fwhm = 2 * (float)Math.Sqrt(2 * Math.Log(2) * variance);
                fullWidthHalfMaximumVals.Add(fwhm);

                edgePoints.Add(new PointF(r, mean));
            }

            VectorOfPointF yvector = new VectorOfPointF();
            VectorOfFloat parameters = new VectorOfFloat();
            yvector.Push(edgePoints.ToArray());
            CvInvoke.FitLine(yvector, parameters, DistType.L12, 0, 0.01, 0.01);

            float vx = parameters[0];
            float vy = parameters[1];
            float x0 = parameters[2];
            float y0 = parameters[3];

            slope = vy / vx;
            intercept = y0 - slope * x0;


            fullWidthHalfMaximumVals.Sort();
            int length = fullWidthHalfMaximumVals.Count;
            lineSpread = 0;
            if (length % 2 == 0)
            {
                lineSpread = (fullWidthHalfMaximumVals[length / 2 - 1] + fullWidthHalfMaximumVals[length / 2]) / 2;
            }
            else
            {
                lineSpread = fullWidthHalfMaximumVals[length / 2];
            }

            //debug:
            //var m = new Image<Gray,byte>(sobelImage.Size);
            //foreach (var point in edgePoints)
            //{
            //    m.Data[(int)point.X, (int)point.Y, 0] = 255;
            //}
            //m.Save("points.png");

            return parameters;
        }



        public void SaveFITS(double[,,] pixels, string filename)
        {
            int width = pixels.GetLength(1);
            int height = pixels.GetLength(0);
            int stride = width * 8;
            byte[] pixelbytes = new byte[height * stride];
            string headerCard;
            System.Text.UTF8Encoding enc = new System.Text.UTF8Encoding();
            int cardCounter = 0;

            // Create a vector representation of the data
            if (!BitConverter.IsLittleEndian)
            {
                double[] fpixels = new double[width];
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        fpixels[x] = pixels[y, x, 0];
                    }
                    Buffer.BlockCopy(fpixels, 0, pixelbytes, y * stride, stride);
                }
            }
            else
            {
                int cnt = 0;
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        byte[] bytes = BitConverter.GetBytes(pixels[y, x, 0]);

                        pixelbytes[cnt++] = bytes[7];
                        pixelbytes[cnt++] = bytes[6];
                        pixelbytes[cnt++] = bytes[5];
                        pixelbytes[cnt++] = bytes[4];
                        pixelbytes[cnt++] = bytes[3];
                        pixelbytes[cnt++] = bytes[2];
                        pixelbytes[cnt++] = bytes[1];
                        pixelbytes[cnt++] = bytes[0];
                    }
                }
            }

            System.IO.FileStream stream = new System.IO.FileStream(filename, System.IO.FileMode.Create);

            /*This is to verify the header format in FITS, which is very strict
            Console.WriteLine("");
            Console.Write("         1");
            Console.Write("         2");
            Console.Write("         3");
            Console.Write("         4");
            Console.Write("         5");
            Console.Write("         6");
            Console.Write("         7");
            Console.WriteLine("         ");
            Console.Write("1234567890");
            Console.Write("1234567890");
            Console.Write("1234567890");
            Console.Write("1234567890");
            Console.Write("1234567890");
            Console.Write("1234567890");
            Console.Write("1234567890");
            Console.WriteLine("123456789");
            // */

            // SIMPLE = T
            headerCard = String.Format("{0,-8}= {1,20} / {2,-47}", "SIMPLE", "T", "file conforms to FITS standard");
            stream.Write(enc.GetBytes(headerCard), 0, 80);
            cardCounter++;

            // BITPIX = 16 as we ONLY deal with 16 bit data, FITS allows many values
            headerCard = String.Format("{0,-8}= {1,20} / {2,-47}", "BITPIX", -64, "number of bits per data pixel");
            stream.Write(enc.GetBytes(headerCard), 0, 80);
            cardCounter++;

            // NAXIS = 2 as we only deal with 2D images, FITS allows any dimension
            headerCard = String.Format("{0,-8}= {1,20} / {2,-47}", "NAXIS", 2, "number of data axes");
            stream.Write(enc.GetBytes(headerCard), 0, 80);
            cardCounter++;

            // NAXIS1 = width
            headerCard = String.Format("{0,-8}= {1,20} / {2,-47}", "NAXIS1", width, "length of data axis 1");
            stream.Write(enc.GetBytes(headerCard), 0, 80);
            cardCounter++;

            // NAXIS = 2 as we only deal with 2D images, FITS allows any dimension
            headerCard = String.Format("{0,-8}= {1,20} / {2,-47}", "NAXIS2", height, "length of data axis 2");
            stream.Write(enc.GetBytes(headerCard), 0, 80);
            cardCounter++;

            headerCard = String.Format("{0,-80}", "END");
            stream.Write(enc.GetBytes(headerCard), 0, 80);
            cardCounter++;

            headerCard = String.Format("{0,-80}", "");

            while (0 != (cardCounter % 36))
            {
                stream.Write(enc.GetBytes(headerCard), 0, 80);
                cardCounter++;
            }

            stream.Write(pixelbytes, 0, height * width * 8);

            stream.Close();
        }


    }





    public class FactoryEdgeLineFinderEmgu1 : IEdgeLineFinder_Creator
    {
        public IEdgeLineFinder Factory(ILogger logger, Dictionary<SearchOrientations, Rectangle> calcAreas = null)
        {
            logger?.Info($"{typeof(FactoryEdgeLineFinderEmgu1)} factory called.");
            return new EdgeLineFinder_Emgu1(logger, calcAreas);
        }
    }



}
