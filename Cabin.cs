using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CombustionAnalysis
{
    internal class Cabin
    {
        public int ID;//            int
        public double volume;//体积m3       double
        public double airMass;//舱内气体质量       double
        public double airT;//舱内气体温度           double
        public double P = 101325;//舱内压强           double
        public double Rou = 1.29;//舱内气体密度           double

        public static double airC = 1005;//舱内气体比热容          static double
        public static double mu = 0.0289634;//空气的摩尔质量0.0289634kg/mol         static double
        public double R = 8.31;//R为普适气体常数         static double

        public int nodeCount;//与其它舱室通风口数量          int
        public int[] nodesID;//与其它舱室通风口的id          int[]

        public int bulkheadCount; //         int
        public int[] bulkheadID; //        int[]

        public int wuziCount;//舱内存放可燃物种类 //        int
        public int[] wuziID;//可燃物id //        int[]

        public double totalE;//总热焓           double
        public double detaM = 0;//单位kg           double
        public double detaE = 0;//单位J           double

        public Cabin(int ID,double volume,double airT = -274)
        {
            this.ID = ID;
            this.volume = volume;
            if(airT != -274)
            {
                this.airT = airT;
            }
            airMass = volume * P * mu / (R * airT);
            Rou = airMass / volume;
            totalE = P * volume * mu * airC / R;
        }
        //初始化EM
        public void setEM(double detaE,double detaM)
        {
            this.detaE = detaE;
            this.detaM = detaM;
        }
        public void ComputeMTPR()
        {
            airMass += detaM;
            detaM = 0;
            totalE+=detaE;
            detaE = 0;
            this.P = R * totalE / (mu * airC * volume);
            Rou = airMass / volume;
            airT = P * volume * mu / (R * airMass);
        }
        public void ComputeDetaEM(double deta_t, Material[] allMaterial, Node[] allNodes, Bulkhead[] allBulkheads)
        {
            //燃烧产生的热量
            foreach(int index in wuziID)
            {
                detaE += allMaterial[index].detaE;
                detaM += allMaterial[index].detaM;
            }
            //空气流动造成的热量损失
            foreach(int index in nodesID)
            {
                double tempV, tempDetaM, tempGetaE;
                tempV = (allNodes[index].cabinID[0] == ID? allNodes[index].airVel[0]: allNodes[index].airVel[1]) * allNodes[index].area * deta_t;
                tempDetaM = -allNodes[index].rou_d * tempV;
                tempGetaE = -(allNodes[index].rou_d * tempV * (allNodes[index].T_d * airC) + tempV * allNodes[index].P_d);
                detaM = detaM + tempDetaM;
                detaE = detaE + tempGetaE;
            }
            foreach(int index in bulkheadID)
            {
                detaE += allBulkheads[index].cabinID[0] == ID ? allBulkheads[index].detaE[0] : allBulkheads[index].detaE[1];
            }
        }
    }
}
