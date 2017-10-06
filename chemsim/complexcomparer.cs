using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chemsim
{
    public class complexcomparer : IComparer<complex>
    {
        public int Compare(complex x, complex y)
        {
            if (Math.Abs(x.b) < global.ZeroImaginary && Math.Abs(y.b) >= global.ZeroImaginary)
            {
                return -1;
            }
            else if (Math.Abs(x.b) >= global.ZeroImaginary && Math.Abs(y.b) < global.ZeroImaginary)
            {
                return 1;
            }
            else if (Math.Abs(x.b) < global.ZeroImaginary && Math.Abs(y.b) < global.ZeroImaginary)
            {
                if (x.a < y.a) { return -1; }
                else if (x.a > y.a) { return 1; }
                else { return 0; }
            }
            else
            {
                return 0;
            }

        }
    }
}
