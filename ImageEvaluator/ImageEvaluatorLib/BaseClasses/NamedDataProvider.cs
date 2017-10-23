using Emgu.CV;
using Emgu.CV.Structure;
using ImageEvaluatorInterfaces.BaseClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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



    }
}
