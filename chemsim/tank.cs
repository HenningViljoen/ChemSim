using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace chemsim
{
    [Serializable]
    public class tank : unitop
    {
      

        public double maxvolume; //m^3
        public double radius;
        public double height;
        public controlvar massinventory; //kg
        public controlvar actualvolumeinventory; //m3
        public controlvar fracinventory; //fraction
        public controlvar inventoryheight; //m
        public controlvar pressureatbottom; //Pa
        


        //Methods
        public tank(int anr, double ax, double ay, double afracinventory, double aradius, double aheight)
            : base(anr, ax, ay, 1, 1)
        {
            inittank(afracinventory, aradius, aheight);
            
        }

        public tank(baseclass baseclasscopyfrom)
            : base(baseclasscopyfrom.nr, baseclasscopyfrom.location.x, baseclasscopyfrom.location.y, 1, 1)
        {
            inittank(global.TankInitFracInventory, global.TankRadiusDraw, global.TankInitHeight);
            copyfrom(baseclasscopyfrom);
        }

        public void inittank(double afracinventory, double aradius, double aheight)
        {
            objecttype = objecttypes.Tank;
            name = nr.ToString() + " " + objecttype.ToString();
            
            massinventory = new controlvar(); //kg
            actualvolumeinventory = new controlvar(); //m3
            fracinventory = new controlvar(); //fraction
            inventoryheight = new controlvar(); //m
            pressureatbottom = new controlvar(); //Pa

            

            //G = aG;
            radius = aradius;
            height = aheight;

            fracinventory.v = afracinventory;
            calcmaxvolume();
            calcmassinventoryfromfracinventory();

            updateinoutpointlocations();
            update(0, false);

            controlpropthisclass.Clear();
            controlpropthisclass.AddRange(new List<string>(new string[] {
                                            "massinventory",
                                            "actualvolumeinventory",
                                            "fracinventory",
                                            "inventoryheight",
                                            "pressureatbottom"}));
            nrcontrolpropinherited = controlproperties.Count;
            controlproperties.AddRange(controlpropthisclass);

        }

        public override void copyfrom(baseclass baseclasscopyfrom)
        {
            tank tankcopyfrom = (tank)baseclasscopyfrom;

            base.copyfrom(tankcopyfrom);

            maxvolume = tankcopyfrom.maxvolume; //m^3
            radius = tankcopyfrom.radius;
            height = tankcopyfrom.height;
            massinventory.copyfrom(tankcopyfrom.massinventory); //kg
            actualvolumeinventory.copyfrom(tankcopyfrom.actualvolumeinventory); //m3
            fracinventory.copyfrom(tankcopyfrom.fracinventory); //fraction
            inventoryheight.copyfrom(tankcopyfrom.inventoryheight); //m
            pressureatbottom.copyfrom(tankcopyfrom.pressureatbottom); //Pa
        }

        public override controlvar selectedproperty(int selection)
        {
            if (selection >= nrcontrolpropinherited)
            {
                switch (selection - nrcontrolpropinherited)
                {
                    case 0:
                        return massinventory;
                    case 1:
                        return actualvolumeinventory;
                    case 2:
                        return fracinventory;
                    case 3:
                        return inventoryheight;
                    case 4:
                        return pressureatbottom;
                    default:
                        return null;

                }
            }
            else { return base.selectedproperty(selection); };
        }

        public void calcmassinventoryfromfracinventory()
        {
            actualvolumeinventory = fracinventory * maxvolume;
            massinventory = actualvolumeinventory * mat.density;
        }

        public void calcmaxvolume()
        {
            maxvolume = Math.PI * Math.Pow(radius, 2) * height;
        }

        public void inventorycalcs()
        {
            calcmaxvolume();
            actualvolumeinventory = massinventory / mat.density;
            fracinventory.v = actualvolumeinventory.v / (maxvolume + global.Epsilon);
            inventoryheight = actualvolumeinventory / (Math.PI * Math.Pow(radius, 2));
        }

        public override void update(int simi, bool historise)
        {
            if (inflow[0] != null)
            {
                mat.T.v = (massinventory.v * mat.T.v + inflow[0].massflow.v * global.SampleT * inflow[0].mat.T.v) / 
                    (massinventory.v + inflow[0].massflow.v * global.SampleT);
                massinventory += inflow[0].massflow * global.SampleT;

            }

            if (outflow[0] != null)
            {
                massinventory += -outflow[0].massflow * global.SampleT;
            }

            inventorycalcs();
            pressureatbottom.v = mat.density.v * global.g * height*fracinventory.v + global.Ps;


            if (outflow[0] != null)
            {
                outflow[0].mat.P.v = pressureatbottom.v;
                outflow[0].mat.T.v = mat.T.v;
            }
            
            if (fracinventory.v < global.TankMinFracInventory)
            {
                if (outflow[0] != null)
                {
                    outflow[0].hasmaterial = false;
                }
            }
            else if (outflow[0] != null)
            {
                outflow[0].hasmaterial = true;
            }

            if (historise && (simi % global.SimVectorUpdatePeriod == 0))
            {
                if (fracinventory.simvector != null) { fracinventory.simvector[simi / global.SimVectorUpdatePeriod] = fracinventory.v; }

                if (pressureatbottom.simvector != null) { pressureatbottom.simvector[simi / global.SimVectorUpdatePeriod] = pressureatbottom.v; }
                
            }
            
        }

        public override void showtrenddetail(simulation asim, List<Form> detailtrendslist)
        {
            detailtrendslist.Add(new tankdetail(this, asim));
            detailtrendslist[detailtrendslist.Count - 1].Show();
        }

        public override bool mouseover(double x, double y)
        {
            return (x >= (location.x - radius) && x <= (location.x + radius) && y >= (location.y - 0.5 * height) && y <= (location.y + 0.5 * height));
        }

        public override void updateinoutpointlocations()
        {
            //Update in and out point locations;
            inpoint[0].x = location.x - global.TankRadiusDraw - global.InOutPointWidth;
            inpoint[0].y = location.y - global.TankInitInOutletDistanceFraction / 2 * global.TankHeightDraw;
            outpoint[0].x = location.x + global.TankRadiusDraw + global.InOutPointWidth;
            outpoint[0].y = location.y + global.TankInitInOutletDistanceFraction / 2 * global.TankHeightDraw;
        }

        public override void setproperties(simulation asim)
        {
            tankproperties tankprop = new tankproperties(this, asim);
            tankprop.Show();
        }

        public override void draw(Graphics G) //public virtual void draw(Graphics G)
        {
            updateinoutpointlocations();

            //Draw main tank
            GraphicsPath tankmain;
            Pen plotPen;
            float width = 1;

            tankmain = new GraphicsPath();
            plotPen = new Pen(Color.Black, width);

            Point[] myArray = new Point[] 
            {new Point(global.OriginX + Convert.ToInt32(global.GScale*(location.x - global.TankRadiusDraw)), 
                    global.OriginY + Convert.ToInt32(global.GScale*(location.y + 0.5*global.TankHeightDraw))), 
            new Point(global.OriginX + Convert.ToInt32(global.GScale*(location.x - global.TankRadiusDraw)), 
                    global.OriginY + Convert.ToInt32(global.GScale*(location.y - 0.5*global.TankHeightDraw))), 
            new Point(global.OriginX + Convert.ToInt32(global.GScale*(location.x + global.TankRadiusDraw)), 
                    global.OriginY + Convert.ToInt32(global.GScale*(location.y - 0.5*global.TankHeightDraw))), 
            new Point(global.OriginX + Convert.ToInt32(global.GScale*(location.x + global.TankRadiusDraw)),
                    global.OriginY + Convert.ToInt32(global.GScale*(location.y + 0.5*global.TankHeightDraw)))};
            tankmain.AddPolygon(myArray);
            plotPen.Color = Color.Black;
            SolidBrush brush = new SolidBrush(Color.White);
            brush.Color = (highlighted) ? Color.Orange : Color.White;
            G.FillPath(brush, tankmain);
            G.DrawPath(plotPen, tankmain);


            //Draw level in the tank (might later be changed for an embedded trend)
            GraphicsPath tanklevel;

            tanklevel = new GraphicsPath();

            Point[] tanklevelarray = new Point[] 
            {new Point(global.OriginX + Convert.ToInt32(global.GScale*(location.x - global.TankRadiusDraw)), 
                    global.OriginY + Convert.ToInt32(global.GScale*(location.y + 0.5*global.TankHeightDraw))), 
            new Point(global.OriginX + Convert.ToInt32(global.GScale*(location.x - global.TankRadiusDraw)), 
                    global.OriginY + Convert.ToInt32(global.GScale*(location.y + 0.5*global.TankHeightDraw - fracinventory.v*global.TankHeightDraw))), 
            new Point(global.OriginX + Convert.ToInt32(global.GScale*(location.x + global.TankRadiusDraw)), 
                    global.OriginY + Convert.ToInt32(global.GScale*(location.y + 0.5*global.TankHeightDraw - fracinventory.v*global.TankHeightDraw))), 
            new Point(global.OriginX + Convert.ToInt32(global.GScale*(location.x + global.TankRadiusDraw)),
                    global.OriginY + Convert.ToInt32(global.GScale*(location.y + 0.5*global.TankHeightDraw)))};
            tanklevel.AddPolygon(tanklevelarray);
            plotPen.Color = Color.Black;
            brush.Color = (highlighted) ? Color.Blue : Color.Green;
            G.FillPath(brush, tanklevel);
            G.DrawPath(plotPen, tanklevel);


            //Draw inpoint
            base.draw(G);

        }

    }
}
