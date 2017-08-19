using Emgu.CV;
using Emgu.CV.Structure;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageEvaluator
{
    public interface IDoubleLightImageReader
    {
        bool GetImage(string fileName, ref Image<Gray, float> img1, ref Image<Gray, float> img2, ref string outmessage);
    }



    public interface IDoubleLightImageReader_Creator
    {
        IDoubleLightImageReader Factory(ILogger logger, int width, int bitDepth, bool showImages);
    }



}
