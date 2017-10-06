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
    public class pidcontroller : baseclass
    {
        public double K, I, D;
        public double bias;
        public double pvspan; //Engineering Unit range over which the PV, Set point, will vary;
        public double opspan; //Engineering Unit range over which the OP will vary.
        public double integral;
        public bool dointegral; //If in wind-up, will go to false.
        public double sp, err;
        public controlvar pv;
        public controlvar op;
        public piddirection direction; // As controldirection
        public double maxpv, minpv;
        public double maxop, minop;
        public string pvname;
        public string opname;


        public pidcontroller(int anr, double ax, double ay, double aK, double aI, double aD, double asp, double aminpv, double amaxpv,
            double aminop, double amaxop)
            : base(anr, ax, ay)
        {
            pidcontrollerinit(aK, aI, aD, asp, aminpv, amaxpv, aminop, amaxop);
        }

        public pidcontroller(pidcontroller pidcontrollercopyfrom)
            : base(pidcontrollercopyfrom.nr, pidcontrollercopyfrom.location.x, pidcontrollercopyfrom.location.y)
        {
            pidcontrollerinit(pidcontrollercopyfrom.K, pidcontrollercopyfrom.I, pidcontrollercopyfrom.D, pidcontrollercopyfrom.sp,
                pidcontrollercopyfrom.minpv, pidcontrollercopyfrom.maxpv, pidcontrollercopyfrom.minop, pidcontrollercopyfrom.maxop);
            copyfrom(pidcontrollercopyfrom);
        }

        private void pidcontrollerinit(double aK, double aI, double aD, double asp, double aminpv, double amaxpv,
            double aminop, double amaxop)
        {
            objecttype = objecttypes.PIDController;

            name = nr.ToString() + " " + objecttype.ToString();

            K = aK;
            I = aI;
            D = aD;
            integral = 0;
            dointegral = true;
            bias = 0;
            sp = asp;

            pv = new controlvar();
            op = new controlvar();

            err = 0;
            direction = piddirection.Direct; // for now
            maxpv = amaxpv;
            minpv = aminpv;
            maxop = amaxop;
            minop = aminop;

            highlighted = false;
            calcspan();
        }


        public override void copyfrom(baseclass baseclasscopyfrom)
        {
            pidcontroller pidcontrollercopyfrom = (pidcontroller)baseclasscopyfrom;

            base.copyfrom(pidcontrollercopyfrom);

            K = pidcontrollercopyfrom.K;
            I = pidcontrollercopyfrom.I;
            D = pidcontrollercopyfrom.D;
            bias = pidcontrollercopyfrom.bias;
            pvspan = pidcontrollercopyfrom.pvspan; //Engineering Unit range over which the PV, Set point, will vary;
            opspan = pidcontrollercopyfrom.opspan; //Engineering Unit range over which the OP will vary.
            integral = pidcontrollercopyfrom.integral;
            dointegral = pidcontrollercopyfrom.dointegral; //If in wind-up, will go to false.
            sp = pidcontrollercopyfrom.sp;
            err = pidcontrollercopyfrom.err;
            pv.copyfrom(pidcontrollercopyfrom.pv);
            op.copyfrom(pidcontrollercopyfrom.op);
            direction = pidcontrollercopyfrom.direction; // As controldirection
            maxpv = pidcontrollercopyfrom.maxpv; 
            minpv = pidcontrollercopyfrom.minpv;
            maxop = pidcontrollercopyfrom.maxop;
            minop = pidcontrollercopyfrom.minop;
            pvname = pidcontrollercopyfrom.pvname;
            opname = pidcontrollercopyfrom.opname;
        }

        public void calcspan()
        {
            pvspan = maxpv - minpv;
            opspan = maxop - minop;
        }

        public void calcerr()
        {
            err = (int)direction * (sp - pv.v) / pvspan;
        }

        public void calcintegral()
        {
            if (dointegral) { integral += err * global.SampleT; }
        }

        public void calcop()
        {
            op.v = K * (err + 1 / I * integral)*opspan + bias;
            if (op.v > maxop)
            {
                op.v = maxop;
                dointegral = false;
            }
            else if (op.v < minop)
            {
                op.v = minop;
                dointegral = false;
            }
            else { dointegral = true; }
        }

        public void init()
        {
            calcspan();
            bias = 0.0;

            calcerr();
            double oldop = op.v;
            calcop();
            bias = oldop - op.v;

        }

        public override void update(int simi, bool historise)
        {
            calcerr();
            calcintegral();
            calcop();
        }

        public override void setproperties(simulation asim)
        {
            //update(asim.simi); These comments might need to be put back in again at some point.
            pidcontrollerproperties pidprop = new pidcontrollerproperties(this, asim);
            pidprop.Show();
        }

        public override bool mouseover(double x, double y)
        {
            return (utilities.distance(x - location.x, y - location.y) <= global.PIDControllerInitRadius);
        }

        public override void draw(Graphics G)
        {
            //updateinoutpointlocations();

            GraphicsPath plot1;
            Pen plotPen;
            float width = 1;

            plot1 = new GraphicsPath();
            plotPen = new Pen(Color.Black, width);

            plot1.AddEllipse(global.OriginX + Convert.ToInt32(global.GScale * (location.x - global.PIDControllerInitRadius)),
                            global.OriginY + Convert.ToInt32(global.GScale * (location.y - global.PIDControllerInitRadius)),
                            Convert.ToInt32(global.GScale * (global.PIDControllerInitRadius * 2)),
                            Convert.ToInt32(global.GScale * (global.PIDControllerInitRadius * 2)));

            plotPen.Color = Color.Black;

            SolidBrush brush = new SolidBrush(Color.White);
            if (highlighted) { brush.Color = Color.Orange; }
            G.FillPath(brush, plot1);
            G.DrawPath(plotPen, plot1);

            base.draw(G);
        }


    }
}
