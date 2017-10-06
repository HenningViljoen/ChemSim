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
    public class unitop : baseprocessclass
    {


        public baseprocessclass[] inflow;
        public baseprocessclass[] outflow;
        public point[] inpoint;   //the points where the stream(s) are coming into the unitop
        public point[] outpoint; //the points where the stream(s) are going out of the unitop.
        public int nin, nout; //amount of streams in, and amount of streams out.

        public unitop(int anr, double ax, double ay, int anin, int anout)
            : base(anr, ax, ay)
        {
            initunitop(anin, anout);
        }

        public unitop(unitop unitopcopyfrom)
            : base(unitopcopyfrom.nr, unitopcopyfrom.location.x, unitopcopyfrom.location.y)
        {
            initunitop(unitopcopyfrom.nin, unitopcopyfrom.nout);
            copyfrom(unitopcopyfrom);
        }

        private void initunitop(int anin, int anout)
        {
            nin = anin;
            nout = anout;
            initinflow();
            initoutflow();
            initinpoint();
            initoutpoint();
        }

        public override void copyfrom(baseclass baseclasscopyfrom)
        {
            unitop unitopcopyfrom = (unitop)baseclasscopyfrom;

            base.copyfrom(unitopcopyfrom);

            //public baseprocessclass[] inflow;
            //public baseprocessclass[] outflow;
            //public point[] inpoint;   //the points where the stream(s) are coming into the unitop
            //public point[] outpoint; //the points where the stream(s) are going out of the unitop.
            //public int nin, nout; //amount of streams in, and amount of streams out.

            for (int i = 0; i < inflow.Length; i++) { inflow[i].copyfrom(unitopcopyfrom.inflow[i]); }
            for (int i = 0; i < outflow.Length; i++) { outflow[i].copyfrom(unitopcopyfrom.outflow[i]); }
            for (int i = 0; i < inpoint.Length; i++) { inpoint[i].copyfrom(unitopcopyfrom.inpoint[i]); }
            for (int i = 0; i < outpoint.Length; i++) { outpoint[i].copyfrom(unitopcopyfrom.outpoint[i]); }
            nin = unitopcopyfrom.nin;
            nout = unitopcopyfrom.nout;
        }

        public virtual void initinflow()
        {
            inflow = new stream[nin];
            for (int i = 0; i < nin; i++)
            {
                inflow[i] = new stream(0, location.x, location.y, location.x, location.y);
            }
            
        }

        public virtual void initoutflow()
        {
            outflow = new stream[nout];
            for (int i = 0; i < nout; i++)
            {
                outflow[i] = new stream(0, location.x, location.y, location.x, location.y);
            }
        }

        public virtual void initinpoint()
        {
            inpoint = new point[nin];  //To be changed by derived classes.
            for (int i = 0; i < nin; i++) { inpoint[i] = new point(location.x, location.y); }  //To be changed by derived classes.
        }

        public virtual void initoutpoint()
        {
            outpoint = new point[nout]; //To be changed by derived classes.
            for (int i = 0; i < nout; i++) { outpoint[i] = new point(location.x, location.y); }  //To be changed by derived classes.
        }

        public virtual void updateinoutpointlocations()
        {
            for (int i = 0; i < nin; i++)
            {
                if (inflow[i] != null) { inflow[i].points[1].copyfrom(inpoint[i]); }
            }

            for (int i = 0; i < nout; i++)
            {
                if (outflow[i] != null) { outflow[i].points[0].copyfrom(outpoint[i]); }
            }
        }

        public override void update(int i, bool historise)
        {
            base.update(i, historise);
        }

        public override void draw(Graphics G)
        {
            //Draw inpoint
            GraphicsPath[] inpointdraw = new GraphicsPath[nin];
            Pen plotPen = new Pen(Color.Black, 1);
            SolidBrush brush = new SolidBrush(Color.White);
            for (int i = 0; i < nin; i++)
            {
                inpointdraw[i] = new GraphicsPath();

                Point[] inpointarray = new Point[] 
                {new Point(global.OriginX + Convert.ToInt32(global.GScale*(inpoint[i].x)), 
                        global.OriginY + Convert.ToInt32(global.GScale*(inpoint[i].y))), 
                new Point(global.OriginX + Convert.ToInt32(global.GScale*(inpoint[i].x + global.InOutPointWidth)), 
                        global.OriginY + Convert.ToInt32(global.GScale*(inpoint[i].y - global.InOutPointHeight))), 
                new Point(global.OriginX + Convert.ToInt32(global.GScale*(inpoint[i].x + global.InOutPointWidth)), 
                        global.OriginY + Convert.ToInt32(global.GScale*(inpoint[i].y + global.InOutPointHeight)))};
                inpointdraw[i].AddPolygon(inpointarray);
                brush.Color = (inpoint[i].highlighted) ? Color.Orange : Color.White;
                plotPen.Color = (inpoint[i].highlighted) ? Color.Red : Color.Black;
                G.FillPath(brush, inpointdraw[i]);
                G.DrawPath(plotPen, inpointdraw[i]);
            }

            //Draw outpoint
            GraphicsPath[] outpointdraw = new GraphicsPath[nout];
            for (int i = 0; i < nout; i++)
            {
                outpointdraw[i] = new GraphicsPath();
                Point[] outpointarray = new Point[] 
                {new Point(global.OriginX + Convert.ToInt32(global.GScale*(outpoint[i].x)), 
                        global.OriginY + Convert.ToInt32(global.GScale*(outpoint[i].y))), 
                new Point(global.OriginX + Convert.ToInt32(global.GScale*(outpoint[i].x - global.InOutPointWidth)), 
                        global.OriginY + Convert.ToInt32(global.GScale*(outpoint[i].y + global.InOutPointHeight))), 
                new Point(global.OriginX + Convert.ToInt32(global.GScale*(outpoint[i].x - global.InOutPointWidth)), 
                        global.OriginY + Convert.ToInt32(global.GScale*(outpoint[i].y - global.InOutPointHeight)))};
                outpointdraw[i].AddPolygon(outpointarray);
                brush.Color = (outpoint[i].highlighted) ? Color.Orange : Color.White;
                plotPen.Color = (outpoint[i].highlighted) ? Color.Red : Color.Black;
                G.FillPath(brush, outpointdraw[i]);
                G.DrawPath(plotPen, outpointdraw[i]);
            }
        }

    }
}
