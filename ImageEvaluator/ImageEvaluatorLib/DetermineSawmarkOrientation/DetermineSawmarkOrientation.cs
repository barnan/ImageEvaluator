using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Threading;
using Emgu.CV;
using Emgu.CV.Structure;
using ImageEvaluatorInterfaces;

namespace ImageEvaluatorLib.DetermineSawmarkOrientation
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
