using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;

namespace chemsim
{
    [Serializable]
    public class baseclass
    {
        public int nr; //Index of this object in the collection that it is part of.
        public string name; //Name of the piece of equipment / stream that is under scope.
        //public bool on; //Is the equipment/unitop/stream/controller, on, or off.
        public objecttypes objecttype; //What kind of unit op is a particular one.
        public bool highlighted;
        public point location;
        public List<string> controlproperties;
        public List<string> controlpropthisclass;
        public int nrcontrolpropinherited;

        public point[] points; //Points used for the item on the PFD.  Expressed in meters.

        public baseclass(int anr, double ax, double ay)
        {
            nr = anr;
            name = "";
            //on = true;

            controlproperties = new List<string>();
            controlpropthisclass = new List<string>();
            nrcontrolpropinherited = 0;

            highlighted = false;

            location = new point(ax, ay);
        }

        public virtual void copyfrom(baseclass baseclasscopyfrom)
        {
            nr = baseclasscopyfrom.nr;
            name = baseclasscopyfrom.name;
            //on = baseclasscopyfrom.on;
            objecttype = baseclasscopyfrom.objecttype;
            highlighted = baseclasscopyfrom.highlighted;
            location.copyfrom(baseclasscopyfrom.location);
            controlproperties = new List<string>(baseclasscopyfrom.controlproperties);
            controlpropthisclass = new List<string>(baseclasscopyfrom.controlpropthisclass);
            nrcontrolpropinherited = baseclasscopyfrom.nrcontrolpropinherited;
        }

        public virtual controlvar selectedproperty(int selection)
        {
            return null;
        }

        public virtual void update(int i, bool historise) //index for where in the simvectors the update is to be stored, 
                                                            //boolean for whether or not history should be kept for the simulation
                                                            //at this time.
        {
        }

        public virtual void updatepoint(int i, double x, double y)
        {
        }

        public virtual bool mouseover(double x, double y) //This function will indicate whether the mouse is over a particular unitop or stream at any moment in time.
        {
            return false;
        }

        public virtual void setproperties(simulation asim) //Method that will be inherited and that will set the properties of the applicable object in a window
        {

        }

        public virtual void showtrenddetail(simulation asim, List<Form> detailtrendslist) //Virtual method that will set up the trend detail window for the 
        //                                                     //applicable object.
        {
        }

        public virtual void draw(Graphics G)
        {
        }

    }
}
