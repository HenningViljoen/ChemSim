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
    public class distillationcolumn : unitop
    {
        public int ntrays; //Number of trays in the column.
        public List<material> inv; //Inventory.  The inventory of the column.
        public List<double> L; //mol/second.  Molar liquid stream leaving stage i in the column.  List of all these streams.
        public List<double> V; //mol/second.  Molar vapour stream leaving stage i in the column.  List of all these streams.


        public distillationcolumn(int anr, double ax, double ay)
            : base(anr, ax, ay, global.DistillationColumnNIn, global.DistillationColumnNOut)
        {
            initdistillationcolumn();
        }

        public distillationcolumn(baseclass baseclasscopyfrom) : base(0,0,0,global.DistillationColumnNIn, global.DistillationColumnNOut)
        {
            initdistillationcolumn();
            copyfrom(baseclasscopyfrom);
        }

        public void initdistillationcolumn()
        {
            objecttype = objecttypes.DistillationColumn;

            ntrays = global.NTrays;
            inv = new List<material>();
            for (int i = 0; i < ntrays; i++)
            {
                inv.Add(new material(global.InitialDCTrayVolume));

                inv[i].uvflash();
            }
        }

        public override void update(int simi, bool historise)
        {

        }

        public override bool mouseover(double x, double y)
        {
            return (x >= (location.x - global.DistillationColumnRadius) && x <= (location.x + global.DistillationColumnRadius)
                && y >=
                (location.y - 0.5 * global.DistillationColumnHeight) && y <= (location.y + 0.5 * global.DistillationColumnHeight));
        }

        public override void updateinoutpointlocations()
        {
            //Update in and out point locations;
            for (int i = 0; i < global.DistillationColumnNIn; i++)
            {
                inpoint[i].x = location.x - global.DistillationColumnRadius - global.InOutPointWidth;
                inpoint[i].y = location.y - global.DistillationColumnHeight * 0.5 + global.DistillationColumnInPointsFraction[i] *
                    global.DistillationColumnHeight;
            }

            for (int i = 0; i < global.DistillationColumnNOut; i++)
            {
                outpoint[i].x = location.x + global.DistillationColumnRadius + global.InOutPointWidth;
                outpoint[i].y = location.y - global.DistillationColumnHeight * 0.5 + global.DistillationColumnOutPointsFraction[i] *
                    global.DistillationColumnHeight;
            }
        }

        public override void setproperties(simulation asim)
        {
            distillationcolumnproperties distcolmnprop = new distillationcolumnproperties(this, asim);
            distcolmnprop.Show();
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
            {new Point(global.OriginX + Convert.ToInt32(global.GScale*(location.x - global.DistillationColumnRadius)), 
                    global.OriginY + Convert.ToInt32(global.GScale*(location.y + 0.5*global.DistillationColumnHeight))), 
            new Point(global.OriginX + Convert.ToInt32(global.GScale*(location.x - global.DistillationColumnRadius)), 
                    global.OriginY + Convert.ToInt32(global.GScale*(location.y - 0.5*global.DistillationColumnHeight))), 
            new Point(global.OriginX + Convert.ToInt32(global.GScale*(location.x + global.DistillationColumnRadius)), 
                    global.OriginY + Convert.ToInt32(global.GScale*(location.y - 0.5*global.DistillationColumnHeight))), 
            new Point(global.OriginX + Convert.ToInt32(global.GScale*(location.x + global.DistillationColumnRadius)),
                    global.OriginY + Convert.ToInt32(global.GScale*(location.y + 0.5*global.DistillationColumnHeight)))};
            tankmain.AddPolygon(myArray);
            plotPen.Color = Color.Black;
            SolidBrush brush = new SolidBrush(Color.White);
            brush.Color = (highlighted) ? Color.Orange : Color.White;
            G.FillPath(brush, tankmain);
            G.DrawPath(plotPen, tankmain);

            //Draw inpoint
            base.draw(G);

        }


    }
}