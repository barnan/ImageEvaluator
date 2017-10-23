﻿using ImageEvaluatorInterfaces.BaseClasses;
using NLog;
using System.Collections.Generic;

namespace ImageEvaluatorInterfaces
{
    public interface IBorderSearcher : IInitalizable
    {
        bool Run(List<NamedData> data, string name);

    }

    public interface IBorderSeracher_Creator
    {
        IBorderSearcher Factory(ILogger logger, int borderSkip, int imageWidth, int imageHeight, bool showImages);
    }



}
