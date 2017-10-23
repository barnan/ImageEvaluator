using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageEvaluatorInterfaces.BaseClasses
{
    public class BorderPointArrays
    {
        private int[][,] borderPointArrays;

        public BorderPointArrays(int[][,] borderPointArrayList)
        {
            this.borderPointArrays = borderPointArrayList;
        }


        public int[,] this[int index]
        {
            get { return borderPointArrays[index]; }
            set { borderPointArrays[index] = value; }
        }



        public int Count
        {
            get
            {
                if (borderPointArrays != null)
                {
                    return borderPointArrays.Length;
                }
                else
                {
                    return 0;
                }
            }
        }

        // TODO : cloning for BorderPointArrays
        //public BorderPointArrays Clone(BorderPointArrays input)
        //{

        //}


    }
}
