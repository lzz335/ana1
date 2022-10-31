using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CombustionAnalysis
{
    internal class MathematicalAlgorithm
    {
        public static double WeibullDistribution(double x,double a,double b)
        {
            if (x <= 0)
            {
                return 0;
            }
            return (b/a)*Math.Pow(x/a,b-1)*Math.Exp(-Math.Pow(x/a,b));
        }


    }
}
