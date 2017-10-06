using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chemsim
{
    [Serializable]
    public class molecule
    {
        public string abreviation;
        public string name;
        public double molarmass; //kg / mole
        public double dynamicviscosity; //Pa·s  We need the dynamic viscosity here since it is used to calculate the 
        //Renoulds number in liquid flow applications.  Later temperature dependance will be added.  Standard conditions for now.
        public double density;          //kg/m3.  Also used in Re nr calc.  Later temp dependance will be added.  Standard conditions for now.
        //public double defaultmolefraction; //fraction of this molecule that is the default fraction for each stream.
        public double Tc; //Kelvin; Critical temperature.
        public double Pc; //Pascal; Critical pressure.
        public double omega; //unitless; Acentric factor.
        public double CpA; //coeficient in equation to calculate Cp is a function of T
        public double CpB; //coeficient in equation to calculate Cp is a function of T
        public double CpC; //coeficient in equation to calculate Cp is a function of T
        public double CpD; //coeficient in equation to calculate Cp is a function of T

        public molecule(string anabreviation, string aname, double amolarmass, double adynamicviscosity, double adensity, double aTc = 500, double aPc = 50*100000,
            double aomega = 0.3, double aCpA = -4.224, double aCpB = 0.3063, double aCpC = -1.586e-04, double aCpD = 3.215e-08)
        //Propane is the default Cp coefficients that have been inserted here.  The Cp arguments here are heat capacity coefficients for the molecule.
        {
            abreviation = anabreviation;
            name = aname;
            molarmass = amolarmass;
            dynamicviscosity = adynamicviscosity;
            density = adensity;

            Tc = aTc;
            Pc = aPc;
            omega = aomega;
            CpA = aCpA; //coeficient in equation to calculate Cp is a function of T
            CpB = aCpB; //coeficient in equation to calculate Cp is a function of T
            CpC = aCpC; //coeficient in equation to calculate Cp is a function of T
            CpD = aCpD; //coeficient in equation to calculate Cp is a function of T
        }

        public double calcCp(double T) //given the Temperature, calculate the Cp for the molecule.
        {
            return CpA + CpB * T + CpC * Math.Pow(T, 2.0) + CpD * Math.Pow(T, 3.0);
        }

    }
}
