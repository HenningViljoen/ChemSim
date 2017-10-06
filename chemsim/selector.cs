using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace chemsim
{
    [Serializable]
    public class selector : block
    {
        public typesofselector selectortype;

        //methods
        public selector(int anr, double ax, double ay, int anin, typesofselector atype)
            : base(anr, ax, ay, anin, 1)
        {
            initselector(atype);
        }

        public selector(baseclass baseclasscopyfrom)
            : base(0, 0, 0, ((mixer)baseclasscopyfrom).nin, 1) //The correct nin will be configured in the unitop class
        {
            initselector();
            copyfrom(baseclasscopyfrom);
        }

        public void initselector(typesofselector atype = typesofselector.LowSelector)
        {
            objecttype = objecttypes.Selector;
            name = nr.ToString() + " " + objecttype.ToString();

            selectortype = atype;

            updateinoutpointlocations();
        }

        public override void copyfrom(baseclass baseclasscopyfrom)
        {
            selector selectorcopyfrom = (selector)baseclasscopyfrom;

            base.copyfrom(selectorcopyfrom);

            selectortype = selectorcopyfrom.selectortype;
        }

        public override void update(int i, bool historise)
        {

                

                for (int j = 0; j < nin; j++)
                {
                   
                }
                
                for (int j = 0; j < nin; j++)
                {
                    
                }

                for (int j = 0; j < nin; j++)
                {
                }

                //outflow[0].mat.P.v /= totalmassflowtouse;
                

 
        }



        public override void updateinoutpointlocations()
        {
            //Update in and out point locations;
            outpoint[0].x = location.x + global.MixerLength / 2;
            outpoint[0].y = location.y;
            for (int i = 0; i < nin; i++)
            {
                inpoint[i].x = location.x - global.MixerLength / 2;
                inpoint[i].y = location.y - (nin - 1) / 2.0 * global.MixerDistanceBetweenBranches +
                    i * global.MixerDistanceBetweenBranches;
            }

            base.updateinoutpointlocations();
        }

        public override void setproperties(simulation asim)
        {
            //mixerproperties mixerprop = new mixerproperties(this, asim);
            //mixerprop.Show();
        }

        public override bool mouseover(double x, double y)
        {
            return true;
            //return (utilities.distance(x - location.x, y - location.y) <= mixerinitradius);
        }

        public override void draw(Graphics G)
        {
            updateinoutpointlocations();

            GraphicsPath plot1;
            Pen plotPen;
            float width = 1;

            plot1 = new GraphicsPath();
            plotPen = new Pen(Color.Black, width);


            Point[] output = new Point[] 
            {new Point(global.OriginX + Convert.ToInt32(global.GScale*(outpoint[0].x)), 
                    global.OriginY + Convert.ToInt32(global.GScale*(outpoint[0].y - global.MixerBranchThickness/2))), 
            new Point(global.OriginX + Convert.ToInt32(global.GScale*(outpoint[0].x - global.MixerLength/2)), 
                    global.OriginY + Convert.ToInt32(global.GScale*(outpoint[0].y - global.MixerBranchThickness/2))), 
            new Point(global.OriginX + Convert.ToInt32(global.GScale*(outpoint[0].x - global.MixerLength/2)), 
                    global.OriginY + Convert.ToInt32(global.GScale*(outpoint[0].y + global.MixerBranchThickness/2))), 
            new Point(global.OriginX + Convert.ToInt32(global.GScale*(outpoint[0].x)),
                    global.OriginY + Convert.ToInt32(global.GScale*(outpoint[0].y + global.MixerBranchThickness/2)))};

            plot1.AddPolygon(output);

            Rectangle upright = new Rectangle(
                global.OriginX + Convert.ToInt32(global.GScale * (location.x - global.MixerBranchThickness / 2)),
                global.OriginY + Convert.ToInt32(global.GScale * (location.y - (nin - 1) / 2.0 * global.MixerDistanceBetweenBranches)),
                Convert.ToInt32(global.GScale * (global.MixerBranchThickness)),
                Convert.ToInt32(global.GScale * ((nin - 1) * global.MixerDistanceBetweenBranches)));
            plot1.AddRectangle(upright);

            Rectangle[] branches = new Rectangle[nin];
            for (int i = 0; i < nin; i++)
            {
                branches[i] = new Rectangle(
                    global.OriginX + Convert.ToInt32(global.GScale * (location.x - global.MixerLength / 2)),
                    global.OriginY + Convert.ToInt32(global.GScale * (location.y - (nin - 1) / 2.0 * global.MixerDistanceBetweenBranches +
                    i * global.MixerDistanceBetweenBranches)),
                    Convert.ToInt32(global.GScale * (global.MixerLength / 2)),
                    Convert.ToInt32(global.GScale * (global.MixerBranchThickness)));
                plot1.AddRectangle(branches[i]);
            }

            plotPen.Color = Color.Black;

            SolidBrush brush = new SolidBrush(Color.Black);
            if (highlighted) { brush.Color = Color.Orange; }
            G.FillPath(brush, plot1);
            G.DrawPath(plotPen, plot1);

            base.draw(G);
        }

    }
}
