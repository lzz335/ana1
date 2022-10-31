using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CombustionAnalysis
{
    internal class Node
    {
        public bool isWindow;
        public double area;
        public double L;
        public int cabinCount;
        public int[] cabinID;
        public double[] airVel;
        public double rou_d;
        public double T_d;
        public double P_d;

        public double detaP;
        public double K;
        public Node(bool isWindow,double area, double L, double detaP = 0, double K = 0.3)
        {
            this.isWindow = isWindow;
            this.area = area;
            this.L = L;
            this.detaP = detaP;
            this.K = K;
            this.airVel = new double[2] { 0, 0 };
        }
        //计算速度
        public void ComputeVel(double deta_t0,Cabin[] allCabins)
        {
            int id_i = cabinID[0];
            int id_k = cabinID[1];
            double P_i = allCabins[id_i].P;
            double P_k = allCabins[id_k].P;
            double P_g = 0;//重力引起的压强差
            double detaV_i = 0;
            double detaV_k = 0;
            if (P_i > P_k)//i舱室内压强大，气体流向k舱（或有向k舱流向的趋势）
            {
                ComputedVel(ref detaV_i, ref detaV_k, allCabins, id_i, id_k, deta_t0, P_i, P_k, P_g);
            }
            else//k舱室内压强大，气体流向i舱（或有向k舱流向的趋势）
            {
                ComputedVel(ref detaV_k, ref detaV_i, allCabins, id_k, id_i, deta_t0, P_k, P_i, P_g);
            }
            double[] detaV = new double[] { detaV_i, detaV_k };
            airVel[0] += detaV[0];
            airVel[1] += detaV[1];
        }
        //计算速度中间过程(dv)
        private void ComputedVel(ref double detaV_1, ref double detaV_2, Cabin[] allCabins, int i, int k, double deta_t0, double P_1, double P_2, double P_g)
        {
            rou_d = allCabins[i].Rou;
            T_d = allCabins[i].airT;
            P_d = allCabins[i].P;
            detaV_1 = (P_1 - P_2 + P_g + detaP + (airVel[0] > 0 ? -1 : 1) * 0.5 * K * allCabins[k].Rou * airVel[1] * airVel[1]) * deta_t0 / (L * allCabins[k].Rou);
            detaV_2 = -detaV_1;
        }
        public double ComputeDetaT(double deta_t0, Cabin[] allCabins, double[] esp)
        {
            double deta_t = deta_t0;
            int i = cabinID[0], k = cabinID[1];
            double P_i=allCabins[i].P;
            double P_k=allCabins[k].P;
            double P_g = 0;
            double detaV_i = 0;
            double detaV_k = 0;
            if (P_i > P_k)//i舱室内压强大，气体流向k舱（或有向k舱流向的趋势）
            {
                ComputeVel(ref detaV_i, ref detaV_k, allCabins, i, k, P_i, P_k, P_g);
            }
            else//k舱室内压强大，气体流向i舱（或有向k舱流向的趋势）
            {
                ComputeVel(ref detaV_k, ref detaV_i, allCabins, k, i, P_k, P_i, P_g);
            }
            double[] detaV = new double[] { detaV_i, detaV_k };
            double epson = esp[0];
            if (detaV[0]>0 && airVel[0] < -epson)
            {
                deta_t = Math.Abs(airVel[0] / detaV[0]);
            }
            else if (detaV[0] < 0 && airVel[0] > epson)
            {
                deta_t = Math.Abs(airVel[0] / detaV[0]);
            }
            else if (Math.Abs(airVel[0]) > epson)
            {
                deta_t = 0.5 * Math.Abs(airVel[0] / detaV[0]);
            }
            if (Math.Abs(detaV[1]) < epson)
            {
                deta_t = deta_t0;
            }
            return deta_t;
        }
        //计算速度中间过程(dv/dt)
        private void ComputeVel(ref double detaV_1, ref double detaV_2, Cabin[] allCabins, int i, int k, double P_1, double P_2, double P_g)
        {
            rou_d = allCabins[i].Rou;
            T_d = allCabins[i].airT;
            P_d = allCabins[i].P;
            detaV_1 = (P_1 - P_2 + P_g + detaP + (airVel[0] > 0 ? -1 : 1) * 0.5 * K * allCabins[k].Rou * airVel[1] * airVel[1]) / (L * allCabins[k].Rou);
            detaV_2 = -detaV_1;
        }
    }
}
