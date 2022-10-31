using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace CombustionAnalysis
{
    internal class Material
    {
        public string name;
        public double mass0;
        public double mass;
        public double T;
        public double burning;
        public int cabinID;

        public double massRate = 0;
        public double detaHr = 18000000;//铝会在构造方法重置
        public double HRR_m2;
        public double HRR;
        public double T0;//燃点温度，仅有其他材料有

        public double detaE;
        public double detaM;
        public double maxT = 50;
        //构造方法
        public Material(string material_name,string name,double mass,int cabinID,double T_0)
        {
            if(material_name == "Al")
            {
                setMaterial_Al(name, mass, cabinID);
            }
            else
            {
                setMaterial_normal(name, mass, cabinID, T_0);
            }
        }
        //Al材料
        public void setMaterial_Al(string name, double mass, int cabinID)
        {
            this.name = name;
            this.mass = mass > 0 ? mass : 0;
            mass0 = mass;
            this.cabinID = cabinID;
            massRate = 1.0/30.0;
            detaHr = 8400000;//重新赋值铝的热值
        }
        //一般材料
        public void setMaterial_normal(string name, double mass, int cabinID, double T_0)
        {
            this.name = name;
            this.mass = mass > 0 ? mass : 0;
            mass0 = mass;
            this.cabinID = cabinID;

            this.burning = 0;
            this.T0=T_0;

            this.detaM = 0.0;
            this.detaE = 0.0;
        }
        //计算detaT时间内金属铝产生的热量和气体质量
        public void computeAlEM(double detaT)
        {
            double detaM0 = massRate * detaT * mass0;
            if (mass < detaM0)
            {
                detaM0 = mass * (mass > 0 ? 1 : 0);
            }
            double detaE0 = detaM0 * detaHr;
            mass = mass - detaM0;
            mass=mass<0?0:mass;
            detaM = detaM0;
            detaE = detaE0;
        }
        //一般材料的方程
        //一般材料的质量燃烧速率方程
        public void ComputeMassRate()
        {
            double maxT = 50;
            massRate = (burning > 0) ? MathematicalAlgorithm.WeibullDistribution(burning / (maxT / 0.5), 1, 1.5) : 0;
        }
        //计算detaT时间内产生的热量和气体质量
        public void ComputeEM(double deta_t,Cabin[] allCabins)
        {
            //计算燃烧速率
            T = allCabins[cabinID].airT;
            if(burning < 0 && T > T0)
            {
                //依据温度判断是否开始燃烧
                burning = 0.5 * deta_t;
            }
            maxT = 50;
            massRate = burning > 0 ? MathematicalAlgorithm.WeibullDistribution(burning / (maxT / 0.5), 1, 1.5) : 0;
            double detaM0 = massRate * deta_t * mass0;
            if (mass < detaM0)
            {
                detaM0 = mass * (mass > 0 ? 1 : 0);
            }
            double detaE0 = detaM0 * detaHr;
            if (mass < 0)
            {
                mass = 0;
            }
            detaM = detaM0;
            detaE = detaE0;
        }
    }
}
