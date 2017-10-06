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
    public class mixer : unitop
    {
        public double mixerinitradius;

        //methods
        public mixer(int anr, double ax, double ay, int anin)
            : base(anr, ax, ay, anin, 1)
        {
            initmixer();
        }

        public mixer(baseclass baseclasscopyfrom)
            : base(0, 0, 0, ((mixer)baseclasscopyfrom).nin, 1) //The correct nin will be configured in the unitop class
        {
            initmixer();
            copyfrom(baseclasscopyfrom);
        }

        public void initmixer()
        {
            objecttype = objecttypes.Mixer;
            name = nr.ToString() + " " + objecttype.ToString();

            mixerinitradius = global.MixerInitRadiusDefault;

            updateinoutpointlocations();
        }

        public override void copyfrom(baseclass baseclasscopyfrom)
        {
            mixer mixercopyfrom = (mixer)baseclasscopyfrom;

            base.copyfrom(mixercopyfrom);

            mixerinitradius = mixercopyfrom.mixerinitradius;
        }

        public override void update(int i, bool historise)
        {

                outflow[0].massflow.v = 0;
                outflow[0].actualvolumeflow.v = 0;
                //outflow[0].mat.P.v = 0;
                outflow[0].mat.T.v = 0;
                outflow[0].mat.density.v = 0.0;
                mat.copycompositiontothisobject(inflow[0].mat);

                for (int j = 0; j < nin; j++)
                {
                    outflow[0].massflow.v += inflow[j].massflow.v;
                }
                double[] massflowtouse = new double[nin];
                double totalmassflowtouse = 0;
                for (int j = 0; j < nin; j++)
                {
                    massflowtouse[j] = (outflow[0].massflow.v == 0) ? 1 : inflow[j].massflow.v;
                    totalmassflowtouse += massflowtouse[j];
                }

                for (int j = 0; j < nin; j++)
                {
                    outflow[0].actualvolumeflow.v += inflow[j].actualvolumeflow.v;
                    //outflow[0].mat.P.v += inflow[j].mat.P.v * massflowtouse[j];
                    inflow[j].mat.P.v = outflow[0].mat.P.v;
                    outflow[0].mat.T.v += inflow[j].mat.T.v * massflowtouse[j];
                    outflow[0].mat.density.v += inflow[j].mat.density.v * massflowtouse[j];
                }

                //outflow[0].mat.P.v /= totalmassflowtouse;
                outflow[0].mat.T.v /= totalmassflowtouse;
                outflow[0].mat.density.v /= totalmassflowtouse;
                

 
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
            mixerproperties mixerprop = new mixerproperties(this, asim);
            mixerprop.Show();
        }

        public override bool mouseover(double x, double y)
        {
            return (utilities.distance(x - location.x, y - location.y) <= mixerinitradius);
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
