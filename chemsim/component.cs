using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chemsim
{
    [Serializable]
    public class component
    {
        public molecule m;
        public double n; //Amount of moles of this molecule/compound that is in this material.
        public double molefraction; //fraction of total moles in the material that is from this component.
        public double massfraction; //mass fraction of this component of the material in a particular unitop or stream.
        //public double molefraction;

        public component(molecule am = null, double amolefraction = 0.0, double an = 0.0)
        {
            molefraction = amolefraction;
            n = an;
            m = am;
            //massfraction = amassfraction;
        }

        public void copytothisobject(component c)
        {
            m = c.m; //Do not make a copy of the c.m object, just point to the address of c.m, since c.m should never change between unit ops - it is part
            // of the fluid package.
            molefraction = c.molefraction;
            n = c.n;
            massfraction = c.massfraction;
            //molefraction = c.molefraction;
        }
    }
}
