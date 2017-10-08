using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageEvaluator.MethodManager
{
    class MethodManager3 : MethodManagerBase
    {
        public MethodManager3(string[] paths)
            : base(paths)
        {
        }


        public override bool Instantiate()
        {
            try
            {

            }
            catch (Exception ex)
            {

                throw;
            }



            return true;
        }
    }
}
