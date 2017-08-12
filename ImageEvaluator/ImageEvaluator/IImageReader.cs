﻿using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    interface IImageReader : IInitalizable
    {
        Image<Gray, float> ReadDoubleLightImage();


    }
}
