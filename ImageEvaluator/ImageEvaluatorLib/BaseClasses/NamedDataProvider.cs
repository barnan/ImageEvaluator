using Emgu.CV;
using Emgu.CV.Structure;
using ImageEvaluatorInterfaces.BaseClasses;
using System.Collections.Generic;

namespace ImageEvaluatorLib.BaseClasses
{
    internal class NamedDataProvider
    {

        public Image<Gray, byte>[] GetEmguByteImages(string name, List<NamedData> data)
        {
            Image<Gray, byte>[] images = null;
            foreach (NamedData item in data)
            {
                if (item.AsEmguByteNamedData() != null)
                {
                    if (item.AsEmguByteNamedData().Name == name)
                    {
                        images = item?.AsEmguByteNamedData();
                    }
                }
            }
            return images;
        }


        public BorderPointArrays GetBorderPointArrays(string name, List<NamedData> data)
        {
            BorderPointArrays arrays = null;
            foreach (NamedData item in data)
            {
                if (item.AsBorderPointArraysNamedData() != null)
                {
                    if (item.AsBorderPointArraysNamedData().Name == name)
                    {
                        arrays = item?.AsBorderPointArraysNamedData();
                    }
                }
            }
            return arrays;
        }



    }
}
