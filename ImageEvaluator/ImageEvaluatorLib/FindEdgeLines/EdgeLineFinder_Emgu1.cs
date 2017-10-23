using System;
using System.Collections.Generic;
using System.Drawing;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using ImageEvaluatorInterfaces;
using NLog;
using ImageEvaluatorInterfaces.BaseClasses;

namespace ImageEvaluatorLib.FindEdgeLines
{

    class WaferEdgeFit
    {
        public float Slope { get; set; }
        public float Intercept { get; set; }
        public bool InvertedRepresentation { get; set; }
        public float LineSpread { get; set; }

        public VectorOfFloat FitParams { get; set; }
    }


    class EdgeLineFinderEmgu1 : EdgeLineFinderBase
    {
        private byte[] _pixelbytes;

        public EdgeLineFinderEmgu1(ILogger logger, int width, int height, Dictionary<SearchOrientations, Rectangle> calcareas)
            : base(logger, width, height, calcareas)
        {
            _logger?.Info($"{this.GetType().Name} instantiated.");
        }

        public override bool Run(List<NamedData> data, ref IWaferEdgeFindData edgeFindData)
        {

            Image<Gray, byte>[] rawImages = null;
            Image<Gray, byte>[] maskImages = null;

            try
            {
                if (!IsInitialized)
                {
                    _logger.Error($"{this.GetType().Name} is not initialized.");
                    return false;
                }

                rawImages = GetEmguByteImages("_rawImages", data);
                int imageCounterRaw = rawImages?.Length ?? 0;

                maskImages = GetEmguByteImages("maskImages", data);
                int imageCounterMask = maskImages?.Length ?? 0;

                if (imageCounterMask != imageCounterRaw)
                {
                    _logger.Info($"{this.GetType()} input and mask image number is not the same!");
                    return false;
                }


                for (int m = 0; m < imageCounterRaw; m++)
                {

                    if (!CheckInputData(rawImages[m], maskImages[m], _calcAreas))
                    {
                        _logger.Info($"{this.GetType()} input and mask data is not proper!");
                        continue;
                    }

                    WaferEdgeFit leftLineData;
                    WaferEdgeFit rightLineData;
                    WaferEdgeFit topLineData;
                    WaferEdgeFit bottomLineData;

                    LineSpreadFunction(rawImages[m], out leftLineData, out rightLineData, out topLineData, out bottomLineData);

                    edgeFindData = new WaferEdgeFindData
                    {
                        TopLineSpread = topLineData.LineSpread,
                        LeftLineSpread = leftLineData.LineSpread,
                        BottomLineSpread = bottomLineData.LineSpread,
                        RightLineSpread = rightLineData.LineSpread,

                        TopSide = topLineData.FitParams,
                        LeftSide = leftLineData.FitParams,
                        BottomSide = bottomLineData.FitParams,
                        RightSide = rightLineData.FitParams
                    };
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger?.Error($"Exception occured during CalculateColumnDataEmgu1 - Run: {ex}");
                return false;
            }

        }



        protected override bool CheckInputData(Image<Gray, byte> originalImage, Image<Gray, byte> maskImage, Dictionary<SearchOrientations, Rectangle> calcAreas)
        {
            if (originalImage == null)
            {
                _logger?.Trace("EdgeLineFinder - the input data is not proper, some of them is null.");
                return false;
            }

            return true;
        }


        public override bool Init()
        {
            int stride = _width * 8;

            _pixelbytes = new byte[_height * stride];

            return true;
        }


        public static void LineSpreadFunction(Image<Gray, byte> workImage, out WaferEdgeFit leftLineData, out WaferEdgeFit rightLineData, out WaferEdgeFit topLineData, out WaferEdgeFit bottomLineData)
        {
            int width = workImage.Width;
            int height = workImage.Height;

            //Recipe? Waferareapercent?
            int darkLimit = 20;
            CvInvoke.Threshold(workImage, workImage, darkLimit, 0, ThresholdType.ToZero);


            FitEdge(workImage, height / 3, 2 * height / 3, 0, width / 8, false, out leftLineData);
            FitEdge(workImage, height / 3, 2 * height / 3, 7 * width / 8, width, false, out rightLineData);
            FitEdge(workImage, 0, height / 8, width / 3, 2 * width / 3, true, out topLineData);
            FitEdge(workImage, 7 * height / 8, height, width / 3, 2 * width / 3, true, out bottomLineData);
        }

        public static void FitEdge(Image<Gray, byte> inputImage, int startRow, int endRow, int startCol, int endCol, bool isTopBottom, out WaferEdgeFit edge)
        {
            edge = new WaferEdgeFit();

            Rectangle origRoi = inputImage.ROI;
            Rectangle sideRoi = new Rectangle(startCol, startRow, endCol - startCol, endRow - startRow);

            bool startFromRight = !isTopBottom && startCol > origRoi.Width / 2 || isTopBottom && startRow > origRoi.Height / 2;

            inputImage.ROI = sideRoi;

            int workingWidth = isTopBottom ? sideRoi.Height : sideRoi.Width;
            int workingHeight = isTopBottom ? sideRoi.Width : sideRoi.Height;
            using (Image<Gray, float> workImage = new Image<Gray, float>(workingWidth, workingHeight))
            {

                double gradientLimit;

                using (Image<Gray, float> sobelImage =
                    isTopBottom ? inputImage.Sobel(0, 1, 3) : inputImage.Sobel(1, 0, 3))
                {
                    using (Image<Gray, float> nullImage = new Image<Gray, float>(sobelImage.Size))
                    {
                        CvInvoke.AbsDiff(sobelImage, nullImage, sobelImage);
                    }

                    using (Image<Gray, byte> mask = new Image<Gray, byte>(sideRoi.Width, sideRoi.Height))
                    {
                        CvInvoke.Threshold(inputImage, mask, 0, 1, ThresholdType.Binary);
                        MCvScalar gradientMean = new MCvScalar();
                        MCvScalar gradientStd = new MCvScalar();
                        CvInvoke.MeanStdDev(sobelImage, ref gradientMean, ref gradientStd, mask);
                        double nSigma = 5;
                        gradientLimit = gradientMean.V0 + nSigma * gradientStd.V0;
                    }

                    if (isTopBottom)
                    {
                        CvInvoke.Transpose(sobelImage, workImage);
                    }
                    else
                    {
                        sobelImage.CopyTo(workImage);
                    }
                }

                inputImage.ROI = origRoi;

                List<PointF> edgePoints = new List<PointF>();
                List<float> fullWidthHalfMaximumVals = new List<float>();
                var sobelData = workImage.Data;
                int stride = 1;

                for (int r = 0; r < workingHeight; r += stride)
                {
                    int approxEdgeCol = 0;
                    if (!startFromRight)
                    {
                        for (int c = 0; c < workingWidth; c++)
                        {
                            var currentValue = sobelData[r, c, 0];
                            if (currentValue > gradientLimit)
                            {
                                approxEdgeCol = c;
                                break;
                            }
                        }
                    }
                    else
                    {
                        for (int c = workingWidth - 1; c > 0; c--)
                        {
                            var currentValue = sobelData[r, c, 0];
                            if (currentValue > gradientLimit)
                            {
                                approxEdgeCol = c;
                                break;
                            }
                        }
                    }

                    int meanEdgeCol = 0;
                    float maxValue = 0;
                    var currentStartCol = Math.Max(approxEdgeCol - 5, 1);
                    var currentEndCol = Math.Min(approxEdgeCol + 5 + 1, workingWidth - 1);
                    for (int c = currentStartCol; c < currentEndCol; c++)
                    {
                        if (sobelData[r, c, 0] > maxValue)
                        {
                            maxValue = sobelData[r, c, 0];
                            meanEdgeCol = c;
                        }
                    }

                    if (!(maxValue > 0))
                    {
                        continue;
                    }

                    float halfMaxLeftValue = maxValue;
                    float halfMaxRightValue = maxValue;
                    int halfMaxLeftCol = meanEdgeCol;
                    int halfMaxRightCol = meanEdgeCol;

                    while (halfMaxLeftValue > maxValue / 2 && halfMaxLeftCol >= 0)
                    {
                        halfMaxLeftCol--;
                        halfMaxLeftValue = sobelData[r, halfMaxLeftCol, 0];
                    }
                    while (halfMaxRightValue > maxValue / 2 && halfMaxRightCol < workingWidth)
                    {
                        halfMaxRightCol++;
                        halfMaxRightValue = sobelData[r, halfMaxRightCol, 0];
                    }

                    float fwhm = halfMaxRightCol - halfMaxLeftCol;

                    //Interpolation
                    float dPixel = (maxValue / 2 - halfMaxLeftValue) /
                                   (sobelData[r, halfMaxLeftCol + 1, 0] - halfMaxLeftValue);
                    fwhm -= dPixel;
                    dPixel = (maxValue / 2 - halfMaxRightValue) /
                             (sobelData[r, halfMaxRightCol - 1, 0] - halfMaxRightValue);
                    fwhm -= dPixel;

                    fullWidthHalfMaximumVals.Add(fwhm);


                    edgePoints.Add(isTopBottom
                        ? new PointF(r + sideRoi.X, meanEdgeCol + sideRoi.Y)
                        : new PointF(r + sideRoi.Y, meanEdgeCol + sideRoi.X));
                }

                VectorOfPointF yvector = new VectorOfPointF();
                VectorOfFloat parameters = new VectorOfFloat();
                yvector.Push(edgePoints.ToArray());
                CvInvoke.FitLine(yvector, parameters, DistType.L12, 0, 0.01, 0.01);

                float vx = parameters[0];
                float vy = parameters[1];
                float x0 = parameters[2];
                float y0 = parameters[3];

                edge.FitParams = parameters;
                edge.Slope = vy / vx;
                edge.Intercept = y0 - edge.Slope * x0;

                fullWidthHalfMaximumVals.Sort();
                int length = fullWidthHalfMaximumVals.Count;

                if (length % 2 == 0)
                {
                    edge.LineSpread = (fullWidthHalfMaximumVals[length / 2 - 1] + fullWidthHalfMaximumVals[length / 2]) / 2;
                }
                else
                {
                    edge.LineSpread = fullWidthHalfMaximumVals[length / 2];
                }

                if (!isTopBottom)
                {
                    edge.InvertedRepresentation = true;
                }
            }
        }



        public void SaveFits(double[,,] pixels, string filename)
        {
            int width = pixels.GetLength(1);
            int height = pixels.GetLength(0);
            int stride = width * 8;

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
                    Buffer.BlockCopy(fpixels, 0, _pixelbytes, y * stride, stride);
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

                        _pixelbytes[cnt++] = bytes[7];
                        _pixelbytes[cnt++] = bytes[6];
                        _pixelbytes[cnt++] = bytes[5];
                        _pixelbytes[cnt++] = bytes[4];
                        _pixelbytes[cnt++] = bytes[3];
                        _pixelbytes[cnt++] = bytes[2];
                        _pixelbytes[cnt++] = bytes[1];
                        _pixelbytes[cnt++] = bytes[0];
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
            string headerCard = String.Format("{0,-8}= {1,20} / {2,-47}", "SIMPLE", "T", "file conforms to FITS standard");
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

            stream.Write(_pixelbytes, 0, height * width * 8);

            stream.Close();
        }


    }





    public class FactoryEdgeLineFinderEmgu1 : IEdgeLineFinder_Creator
    {
        public IEdgeLineFinder Factory(ILogger logger, int width, int height, Dictionary<SearchOrientations, Rectangle> calcAreas = null)
        {
            logger?.Info($"{typeof(FactoryEdgeLineFinderEmgu1)} factory called.");
            return new EdgeLineFinderEmgu1(logger, width, height, calcAreas);
        }
    }



}
