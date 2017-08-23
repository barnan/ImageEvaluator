﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageEvaluator
{
    interface IResultSaver : IInitalizable
    {

        bool SaveResult(IMeasurementResult result);


    }
}
