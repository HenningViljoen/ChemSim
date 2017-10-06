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
    public class flange : unitop
    {
        public flange(int anr, double ax, double ay)
            : base(anr, ax, ay, 1, 1)
        {
            initflange();
            update(0, true);
        }

        public flange(baseclass baseclasscopyfrom) : base(0,0,0,1,1)
        {
            initflange();
            copyfrom(baseclasscopyfrom);
        }

        public void initflange()
        {
            objecttype = objecttypes.Flange;

            name = nr.ToString() + " " + objecttype.ToString();
        }

        public override void copyfrom(baseclass baseclasscopyfrom)
        {
            flange flangecopyfrom = (flange)baseclasscopyfrom;

            base.copyfrom(flangecopyfrom);
        }

        public override void update(int simi, bool historise)
        {

                mat.copycompositiontothisobject(inflow[0].mat);
                mat.density = inflow[0].mat.density;
                mat.T = inflow[0].mat.T;
                massflow = inflow[0].massflow;
                actualvolumeflow = massflow / mat.density;

                outflow[0].mat.P.v = inflow[0].mat.P.v;

                calcmolarflowfrommassflow();
                calcstandardflowfrommoleflow();

                outflow[0].mat.copycompositiontothisobject(mat);
                outflow[0].massflow = massflow;
                outflow[0].mat.density = mat.density;
                outflow[0].mat.T = mat.T;

        }

        public override bool mouseover(double x, double y)
        {
            return (utilities.distance(x - location.x, y - location.y) <= global.ValveLength);
        }

        public override void updateinoutpointlocations()
        {
            inpoint[0].x = location.x - global.FlangeLength / 2 - global.InOutPointWidth;
            inpoint[0].y = location.y;
            outpoint[0].x = location.x + global.FlangeLength / 2 + global.InOutPointWidth;
            outpoint[0].y = location.y;

            base.updateinoutpointlocations();
        }

        public override void draw(Graphics G)
        {
            updateinoutpointlocations();

            GraphicsPath plot1;
            Pen plotPen;
            float width = 1;

            plot1 = new GraphicsPath();
            plotPen = new Pen(Color.Black, width);

            Point[] myArray = new Point[] 
            {new Point(global.OriginX + Convert.ToInt32(global.GScale*(location.x - global.FlangeLength/2)), 
                    global.OriginY + Convert.ToInt32(global.GScale*(location.y - global.ValveWidth/2))), 
            new Point(global.OriginX + Convert.ToInt32(global.GScale*(location.x + global.FlangeLength / 2)), 
                    global.OriginY + Convert.ToInt32(global.GScale*(location.y + global.ValveWidth/2))), 
            new Point(global.OriginX + Convert.ToInt32(global.GScale*(location.x + global.FlangeLength / 2)), 
                    global.OriginY + Convert.ToInt32(global.GScale*(location.y - global.ValveWidth/2))), 
            new Point(global.OriginX + Convert.ToInt32(global.GScale*(location.x - global.FlangeLength / 2)),
                    global.OriginY + Convert.ToInt32(global.GScale*(location.y + global.ValveWidth/2)))};

            plot1.AddPolygon(myArray);

            plotPen.Color = Color.Black;

            SolidBrush brush = new SolidBrush(Color.White);
            if (highlighted) { brush.Color = Color.Orange; }
            G.FillPath(brush, plot1);
            G.DrawPath(plotPen, plot1);

            base.draw(G);
        }

    }
}
