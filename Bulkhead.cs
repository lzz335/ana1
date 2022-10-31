using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CombustionAnalysis
{
    internal class Bulkhead
    {
        public int ID;
        public int cabinCount;
        public int[] cabinID;
        public double[] T_ij = new double[] { 273.15 + 20, 273.15 + 20 };
        public double[] detaE = new double[] { 0, 0 };

        public double area;
        public double mass;
        public double thick = 0.02;

        public double T;
        public static double C;
        public static double lamt = 10;

        public Bulkhead(int id,double area)
        {
            ID = id;
            this.area = area;
        }
        public void ComputeDetaE(double deta_t0,Cabin[] allCabins)
        {
            int i = cabinID[0];
            T_ij[0] = allCabins[i].airT;
            if (cabinCount > 1)
            {
                i = cabinID[1];
                T_ij[1] = allCabins[i].airT;
            }
            else
            {
                T_ij[1] = 273.15 + 20;
            }
            double absDetaE = Math.Abs(T_ij[0] - T_ij[1]) * lamt / thick * area * deta_t0;
            if (T_ij[0] > T_ij[1])
            {
                detaE[0] = -absDetaE;
                detaE[1] = absDetaE;
            }
            else
            {
                detaE[0] = absDetaE;
                detaE[1] = -absDetaE;
            }
        }
    }
}
