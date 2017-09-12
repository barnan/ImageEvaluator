using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.UI;
using ImageEvaluator.Interfaces;

namespace ImageEvaluator.DetermineSawmarkOrientation
{
    public class DetermineSawmarkOrientation : ISawmarkDeterminer
    {

        public bool Init()
        {
            return true;
        }



        public void Run(Image<Gray, float> image)
        {
            Image<Gray, float> imageSection = new Image<Gray, float>(20, image.Height);
            Image<Gray, float> lineScan = new Image<Gray, float>(1, image.Height);
            //Image<Gray, float> flineScan = new Image<Gray, float>(1, image.Height);
            Mat flineScan = new Mat(new Size(1, image.Height), DepthType.Cv32F, 1);
            Rectangle rect = new Rectangle(3500, 0, 20, 4096);

            image.ROI = rect;
            imageSection = image.Copy();
            
            CvInvoke.Reduce(imageSection, lineScan, ReduceDimension.SingleCol, ReduceType.ReduceAvg);

            var im = lineScan.Resize(200, lineScan.Height, Inter.Linear, false);
            ImageViewer.Show(im, "kep");


            Matrix<float> dftMatrix = new Matrix<float>(image.Rows, image.Cols, 2);
            CvInvoke.Dft(lineScan.Mat, flineScan, DxtType.Forward, 0);


            var im2 = flineScan.ToImage<Gray, float>().Resize(200, lineScan.Height, Inter.Linear, false);
            ImageViewer.Show(im2, "kep2");

            im2.Convert<Gray, byte>().Save("fft.png");

        }

    }


    public class Factory_DetermineSawmarkOrientation : ISawmarkDeterminer_Creator
    {
        public ISawmarkDeterminer Factory()
        {
            return new DetermineSawmarkOrientation();
        }
    }
}
