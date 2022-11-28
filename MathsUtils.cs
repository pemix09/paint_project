using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace paint_project
{
    public static class MathsUtils
    {
        public static int nCk(int n, int k)
        {
            if (k > n)
                return 0;
            if (k == 0 || k == n)
                return 1;

            return nCk(n - 1, k - 1)
                + nCk(n - 1, k);
        }
    }
}
