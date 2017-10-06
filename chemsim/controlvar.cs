using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chemsim
{
    [Serializable]
    public class controlvar   //This objective of this class is to serve as a type of pointer, replacing the pointer that was used,
    //since that did not really work.  C# does not really support pointers as they claim.
    {
        public double v; //The variable in this object that will have the PV or OP to be controlled in the simulation. 
        public bool isbool; //Is this a boolean variable?  Could then be part of a hybrid system.
        public double[] simvector;

        public exceldataset excelsource; // for the case that data will be drawn in from an Excel file.
        public datasourceforvar datasource; //The source of data for the variable in the model.

        public controlvar(double av = 0, bool aisbool = false)
        {
            v = av;
            isbool = aisbool;
            //simvector = new double[global.SimIterations];
            excelsource = null;
            datasource = datasourceforvar.Simulation;
        }

        public controlvar(controlvar copyfrom)
        {
            v = copyfrom.v;
            excelsource = copyfrom.excelsource;
            datasource = copyfrom.datasource;
        }

        public void copyfrom(controlvar copyfrom)
        {
            v = copyfrom.v;
            excelsource = copyfrom.excelsource;
            datasource = copyfrom.datasource;
        }

        public string ToString(string format)
        {
            return v.ToString(format);
        }

        public override string ToString()
        {
            return v.ToString();
        }

        public static controlvar operator -(controlvar c, double d)
        {
            return new controlvar(c.v - d);
        }

        public static controlvar operator -(controlvar c)
        {
            return new controlvar(-c.v);
        }

        public static bool operator <(controlvar c, double d)
        {
            return c.v < d;
        }

        public static bool operator >(controlvar c, double d)
        {
            return c.v > d;
        }

        public static controlvar operator /(controlvar c, double d)
        {
            return new controlvar(c.v / d);
        }

        public static controlvar operator *(controlvar c, double d)
        {
            return new controlvar(c.v * d);
        }

        public static controlvar operator +(controlvar c, double d)
        {
            return new controlvar(c.v + d);
        }

        public static controlvar operator *(double d, controlvar c)
        {
            return new controlvar(d * c.v);
        }

        public static controlvar operator /(double d, controlvar c)
        {
            return new controlvar(d / c.v);
        }

        public static controlvar operator *(controlvar c1, controlvar c2)
        {
            return new controlvar(c1.v * c2.v);
        }

        public static controlvar operator /(controlvar c1, controlvar c2)
        {
            return new controlvar(c1.v/c2.v);
        }

        public static controlvar operator +(controlvar c1, controlvar c2)
        {
            return new controlvar(c1.v + c2.v);
        }

        public static controlvar operator -(controlvar v1, controlvar v2)
        {
            return new controlvar(v1.v - v2.v);
        }

        public static controlvar Pow(controlvar c, double y)
        {
            return new controlvar(Math.Pow(c.v, y));
        }


    }
}
