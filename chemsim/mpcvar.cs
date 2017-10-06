using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chemsim
{
    [Serializable]
    public class mpcvar
    {
        public controlvar var;
        public string name;
        public controlvar target; //This will be the target in the objective function of the NMPC/MPC controller that will use this variable.
                                //The target will be a control var, since it might be a signal that changes (moving target), which might come from the 
                                //historian, or from a data file.
        public double weight; //This will be the weight in the objective function for the NMPC/MPC controller that will use this variable.
        public double min; //Each MV and CV will have a high and low limit.  This will be taken into account in the NMPC algorithm as 
                                //constraints.
        public double max;

        public mpcvar(controlvar avar, string aname, double atarget, double aweight, double amin = 0, double amax = 0)
        {
            var = avar;
            name = aname;
            target = new controlvar(atarget);
            weight = aweight;
            min = amin;
            max = amax;
        }

        public mpcvar(mpcvar mpcvarcopyfrom)
        {
            var = new controlvar(mpcvarcopyfrom.var);
            copyfrom(mpcvarcopyfrom);
        }

        public void copyfrom(mpcvar mpcvarcopyfrom)
        {
            var.copyfrom(mpcvarcopyfrom.var);
            name = mpcvarcopyfrom.name;
            target = mpcvarcopyfrom.target;
            weight = mpcvarcopyfrom.weight;
            min = mpcvarcopyfrom.min;
            max = mpcvarcopyfrom.max;
        }

        public double fracofrange() //returns the fraction of range of the Engineering Unit MV or CV
        {
            return (var.v - min) / (max - min + global.Epsilon);
        }

        public double rangetoeu(double frac) //converts a fraction value for the variable to its Engineering Unit form.
        {
            return frac * (max - min) + min;
        }
    }
}
