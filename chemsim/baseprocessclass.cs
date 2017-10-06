using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace chemsim
{
    [Serializable]
    public class baseprocessclass : baseclass
    {

        public bool hasmaterial; //True if the stream/unitop/baseclass has material inside that can flow/pump or be processed.

        public material mat;

        public controlvar actualvolumeflow; //m3/s  non-standard conditions.
        public controlvar standardvolumeflow; //m3/s  standard conditions (later to be descriminated between gases and liquids)
        public controlvar massflow; //kg/second
        
        public controlvar molarflow; //molar flow per second

        public baseprocessclass(int anr, double ax, double ay)
            : base(anr, ax, ay)
        {

            actualvolumeflow = new controlvar();
            standardvolumeflow = new controlvar(); 
            molarflow = new controlvar();
            massflow = new controlvar();
            hasmaterial = true;

            //mat = new material(global.baseprocessclassInitVolume);
            //public material(string componentname, double aTemp, double aV, double aP, double af) //second constructor
            mat = new material(global.fluidpackage, global.baseprocessclassInitTemperature, global.baseprocessclassInitVolume, 
                global.baseprocessclassInitPressure, 0);
            massflow.v = global.baseprocessclassInitMassFlow;
            
            calcactualvolumeflowfrommassflow();
            calcmolarflowfrommassflow();
            calcstandardflowfrommoleflow();

            //pressuresimvector = new double[global.SimIterations];

            controlpropthisclass.Clear();
            controlpropthisclass.AddRange(new List<string>(new string[] {"pressure",
                                                                        "volume",
                                                                        "density",
                                                                        "temperature",
                                                                        "mass",
                                                                        "n",
                                                                        "actualvolumeflow",
                                                                        "standardvolumeflow",
                                                                        "massflow",
                                                                        "molarflow"}));
            controlproperties.AddRange(controlpropthisclass);
        }

        public override void copyfrom(baseclass baseclasscopyfrom)
        {
            baseprocessclass baseprocessclasscopyfrom = (baseprocessclass)baseclasscopyfrom;

            base.copyfrom(baseprocessclasscopyfrom);

            hasmaterial = baseprocessclasscopyfrom.hasmaterial; //True if the stream/unitop/baseclass has material inside that can flow/pump or be processed.

            mat.copyfrom(baseprocessclasscopyfrom.mat);

            actualvolumeflow.v = baseprocessclasscopyfrom.actualvolumeflow.v; //m3/s  non-standard conditions.
            standardvolumeflow.v = baseprocessclasscopyfrom.standardvolumeflow.v; //m3/s  standard conditions (later to be descriminated between gases and liquids)
            massflow.copyfrom(baseprocessclasscopyfrom.massflow); //kg/second  //At this stage we use copyfrom for this one as we need to copy the excel
                                                                  //source in particular as well for the mass flow.
            molarflow.v = baseprocessclasscopyfrom.molarflow.v; //molar flow per second

        }

        public override controlvar selectedproperty(int selection)
        {

            switch (selection)
            {
                case 0:
                    return mat.P;
                case 1:
                    return mat.V;
                case 2:
                    return mat.density;
                case 3:
                    return mat.T;
                case 4:
                    return mat.mass;
                case 5:
                    return mat.n;
                case 6:
                    return actualvolumeflow;
                case 7:
                    return standardvolumeflow;
                case 8:
                    return massflow;
                case 9:
                    return molarflow;
                default:
                    return null;

            }
        }

        //public void calcmassflowfromactualvolflow()
        //{
        //    massflow = actualvolumeflow * density;
        //}

        public void calcactualvolumeflowfrommassflow()
        {
            actualvolumeflow.v = massflow.v / (mat.density.v + 0.001);
        }

        public void calcstandardflowfrommoleflow()
        {
            standardvolumeflow.v = utilities.dndt2fps(molarflow.v);
        }



        public void calcmolarflowfrommassflow()
        {
            molarflow.v = 0;
            molarflow.v = massflow.v / (mat.massofonemole + 0.001);
        }

        //public void calcmassflowfromstandardflow()
        //{
        //    molarflow = utilities.fps2dndt(standardvolumeflow);
        //    massflow = molarflow * mat.massofonemole;
        //} 

        public void calcmassflowfrommolarflow()
        {
            massflow.v = 0;
            for (int i = 0; i < mat.composition.Count; i++)
            {
                massflow.v += mat.composition[i].n / mat.n.v * molarflow.v * 
                    mat.composition[i].m.molarmass;
            }
        }

        public virtual void update(int i, bool historise) //index for where in the simvectors the update is to be stored.
        {
            mat.update(i, historise);
        }

        //public virtual void updatepoint(int i, double x, double y)
        //{
        //}

        //public virtual bool mouseover(double x, double y) //This function will indicate whether the mouse is over a particular unitop or stream at any moment in time.
        //{
        //    return false;
        //}

        //public virtual void setproperties(simulation asim) //Method that will be inherited and that will set the properties of the applicable object in a window
        //{

        //}

        //public virtual void draw(Graphics G)
        //{
        //}





    }
}
