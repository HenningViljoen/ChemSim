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
    //Types of valve will later be added here as an enum.
    [Serializable]
    public class pump : unitop
    {
        //variables

        public controlvar on; //0 for off, 1 for on.
        public controlvar deltapressure; //Pa
        public controlvar pumpspeed; //rev per second
        public double pumpspeeddynamic; //will trail the main pumpspeed set point which is the controlvar above.
        public double dpumpspeeddt;
        public double speedtau;
        //public double[] deltapressuresimvector; //Pa; history sim vector.
        public double maxdeltapressure; //Pa
        public double mindeltapressure; //Pa
        public controlvar maxactualvolumeflow;     //

        public double pumpcurvem;
        public double pumpcurvec;

        public double pumpcurvea0;
        public double pumpcurvea1;
        public double pumpcurvea2;

        public controlvar pumppower; //W
        public double newpumppower; //W; The future steady state fan power.
        public double pumppowerstatespacex1;
        public double pumppowerstatespacex2;
        public double ddtpumppowerstatespacex1;
        public double ddtpumppowerstatespacex2;


        public point outletlocation;
        public calculationmethod calcmethod;

        //methods
        public pump(int anr, double ax, double ay, double amaxdeltapressure, double amindeltapressure, 
            double amaxactualflow, double anactualvolumeflow, double aon)
            : base(anr, ax, ay, 1, 1)
        {
            initpump(amaxdeltapressure, amindeltapressure, amaxactualflow, anactualvolumeflow, aon);
        }

        public pump(baseclass baseclasscopyfrom) : base(0,0,0,1,1)
        {
            initpump(0, 0, 0, 0, 0);
            copyfrom(baseclasscopyfrom);
        }

        public void initpump(double amaxdeltapressure, double amindeltapressure,
            double amaxactualflow, double anactualvolumeflow, double aon)
        {
            objecttype = objecttypes.Pump;

            

            name = nr.ToString() + " " + objecttype.ToString();


            controlpropthisclass.Clear();
            controlpropthisclass.AddRange(new List<string>(new string[] {"on","deltapressure",
                                                                        "maxactualvolumeflow","pumpspeed","pumppower"}));
            nrcontrolpropinherited = controlproperties.Count;
            controlproperties.AddRange(controlpropthisclass);

            on = new controlvar(1, true);
            deltapressure = new controlvar();
            pumpspeed = new controlvar(global.PumpCurveSpeedT0);
            pumpspeeddynamic = global.PumpCurveSpeedT0;
            dpumpspeeddt = 0;
            speedtau = global.PumpSpeedTau;
            maxactualvolumeflow = new controlvar();

            maxdeltapressure = amaxdeltapressure; //Pa
            mindeltapressure = amindeltapressure;
            deltapressure = new controlvar(0);
            //deltapressuresimvector = new double[global.SimVectorLength];
            maxactualvolumeflow = new controlvar(amaxactualflow);     //actual m3/s
            actualvolumeflow.v = anactualvolumeflow;  //actual m3/s
            actualvolumeflow.simvector = new double[global.SimVectorLength];
            actualvolumeflow.v = 0;
            on.v = aon; //on or off
            calcmethod = calculationmethod.DetermineFlow;

            calcpumpcurve();

            pumppower = new controlvar(0.0);
            newpumppower = 0.0; //W; The future steady state fan power.
            pumppowerstatespacex1 = 0.0;
            pumppowerstatespacex2 = 0.0;
            ddtpumppowerstatespacex1 = 0.0;
            ddtpumppowerstatespacex2 = 0.0;

            outletlocation = new point(0, 0);
            updateinoutpointlocations();
        }

        public override void copyfrom(baseclass baseclasscopyfrom)
        {
            pump pumpcopyfrom = (pump)baseclasscopyfrom;

            base.copyfrom(pumpcopyfrom);

            on.v = pumpcopyfrom.on.v;
            deltapressure.v = pumpcopyfrom.deltapressure.v; //Pa
            pumpspeed.v = pumpcopyfrom.pumpspeed.v;
            pumpspeeddynamic = pumpcopyfrom.pumpspeeddynamic;
            dpumpspeeddt = pumpcopyfrom.dpumpspeeddt;
            speedtau = pumpcopyfrom.speedtau;
            on = pumpcopyfrom.on; //on or off
            maxdeltapressure = pumpcopyfrom.maxdeltapressure; //Pa
            mindeltapressure = pumpcopyfrom.mindeltapressure; //Pa
            maxactualvolumeflow.v = pumpcopyfrom.maxactualvolumeflow.v;     //
            pumpcurvem = pumpcopyfrom.pumpcurvem;
            pumpcurvec = pumpcopyfrom.pumpcurvec;
            pumpcurvea0 = pumpcopyfrom.pumpcurvea0;
            pumpcurvea1 = pumpcopyfrom.pumpcurvea1;
            pumpcurvea2 = pumpcopyfrom.pumpcurvea2;

            pumppower.v = pumpcopyfrom.pumppower.v;
            newpumppower = pumpcopyfrom.newpumppower; //W; The future steady state fan power.
            pumppowerstatespacex1 = pumpcopyfrom.pumppowerstatespacex1;
            pumppowerstatespacex2 = pumpcopyfrom.pumppowerstatespacex2;
            ddtpumppowerstatespacex1 = pumpcopyfrom.ddtpumppowerstatespacex1;
            ddtpumppowerstatespacex2 = pumpcopyfrom.ddtpumppowerstatespacex2;

            outletlocation.copyfrom(pumpcopyfrom.outletlocation);
            calcmethod = pumpcopyfrom.calcmethod;
        }

        public override controlvar selectedproperty(int selection)
        {
            if (selection >= nrcontrolpropinherited)
            {
                switch (selection - nrcontrolpropinherited)
                {
                    case 0:
                        return on;
                    case 1:
                        return deltapressure;
                    case 2:
                        return maxactualvolumeflow;
                    case 3:
                        return pumpspeed;
                    case 4:
                        return pumppower;
                    default:
                        return null;
                }
            }
            else { return base.selectedproperty(selection); };
        }

        public void calcpumpcurve()
        {
            pumpcurvem = -maxdeltapressure / maxactualvolumeflow.v;
            pumpcurvec = maxdeltapressure;

            maxactualvolumeflow.v = global.PumpCurvef2;
            double p1 = global.PumpCurvep1;
            double f1 = global.PumpCurvef1;
            double f2 = global.PumpCurvef2;
            pumpcurvea0 = global.PumpCurveYAxis / Math.Pow(pumpspeed.v, 2);
            pumpcurvea2 = (pumpspeed.v * f2 * p1 - f2 * pumpcurvea0 * Math.Pow(pumpspeed.v, 3) + pumpcurvea0 * Math.Pow(pumpspeed.v, 3) * f1) /
                (-Math.Pow(f2, 2) * pumpspeed.v * f1 + Math.Pow(f1, 2) * pumpspeed.v * f2);
            pumpcurvea1 = (-pumpcurvea0 * Math.Pow(pumpspeed.v, 2) - pumpcurvea2 * Math.Pow(f2, 2)) / (pumpspeed.v * f2);
        }

        public double calcdeltapressurequadratic(double actualflow)
        {
            return pumpcurvea0 * Math.Pow(pumpspeeddynamic, 2) + pumpcurvea1 * pumpspeeddynamic * actualflow +
                    pumpcurvea2 * Math.Pow(actualflow, 2);
        }

        public double calcpumppower(double volumeflow, double pressure)
        {
            return volumeflow * pressure;

        }   

        private void calcactualvolumeflowquadratic()
        {
            double xfinal = 0;
            double ison = (on.v >= 0.5) ? 1 : 0;
            double a = pumpcurvea2;
            double b = pumpcurvea1 * pumpspeeddynamic;
            double c = pumpcurvea0 * Math.Pow(pumpspeeddynamic, 2) - deltapressure.v;
            double sqrtarg = b * b - 4 * a * c;
            if (sqrtarg >= 0)
            {
                double x1 = (-b + Math.Sqrt(sqrtarg)) / (2 * a);
                double x2 = (-b - Math.Sqrt(sqrtarg)) / (2 * a);
                xfinal = (x1 >= x2) ? x1 : x2;
            }
            else { xfinal = global.PumpMinActualFlow; }
            actualvolumeflow.v = ison * xfinal;
        }

        private void ddt()
        {
            pumppowerstatespacex1 = pumppower.v;
            ddtpumppowerstatespacex1 = pumppowerstatespacex2;
            ddtpumppowerstatespacex2 = -global.Rotatinga0 * pumppowerstatespacex1 - global.Rotatinga1 * pumppowerstatespacex2 + 
                global.Rotatingb0 * newpumppower;

            dpumpspeeddt = -1 / global.PumpSpeedTau * pumpspeeddynamic + 1 / global.PumpSpeedTau * pumpspeed.v;
        }

        public override void update(int simi, bool historise)
        {
            if (inflow[0] != null && outflow[0] != null)
            {
                deltapressure.v = outflow[0].mat.P.v - inflow[0].mat.P.v;
                
                if (deltapressure.v > maxdeltapressure) { deltapressure.v = maxdeltapressure; }
                else if (deltapressure.v < mindeltapressure) { deltapressure.v = mindeltapressure; }

                if (inflow[0].hasmaterial)
                {
                    if (calcmethod == calculationmethod.DetermineFlow)
                    {
                        calcactualvolumeflowquadratic();
                        //actualvolumeflow.v = ison*(deltapressure.v - pumpcurvec) / pumpcurvem; //whether this is actual or standard flow needs to be checked and updated.
                    }
                    else
                    {
                        deltapressure.v = calcdeltapressurequadratic(actualvolumeflow.v);
                        //deltapressure.v = actualvolumeflow.v * pumpcurvem + pumpcurvec;
                        outflow[0].mat.P.v = inflow[0].mat.P.v + deltapressure.v;
                    }
                }
                else
                {
                    actualvolumeflow.v = 0;
                }

                newpumppower = calcpumppower(actualvolumeflow.v, deltapressure.v);

                ddt();
                pumppowerstatespacex1 += ddtpumppowerstatespacex1 * global.SampleT;
                pumppowerstatespacex2 += ddtpumppowerstatespacex2 * global.SampleT;
                pumpspeeddynamic += dpumpspeeddt * global.SampleT;

                pumppower.v = pumppowerstatespacex1;
                if (pumppower.v < 0) { pumppower.v = 0; }

                inflow[0].massflow.v = actualvolumeflow.v * inflow[0].mat.density.v;
                outflow[0].massflow.v = actualvolumeflow.v * inflow[0].mat.density.v;
                outflow[0].mat.copycompositiontothismat(inflow[0].mat);
                outflow[0].mat.T.v = inflow[0].mat.T.v;

                if (historise && (simi % global.SimVectorUpdatePeriod == 0))
                {
                    if (deltapressure.simvector != null) { deltapressure.simvector[simi / global.SimVectorUpdatePeriod] = deltapressure.v; }

                    if (actualvolumeflow.simvector != null) { actualvolumeflow.simvector[simi / global.SimVectorUpdatePeriod] = actualvolumeflow.v; }

                    if (pumpspeed.simvector != null) { pumpspeed.simvector[simi / global.SimVectorUpdatePeriod] = pumpspeed.v; }

                    if (pumppower.simvector != null) { pumppower.simvector[simi / global.SimVectorUpdatePeriod] = pumppower.v; }

                    if (on.simvector != null) { on.simvector[simi / global.SimVectorUpdatePeriod] = on.v; }
                    
                }
            }
        }

        public override bool mouseover(double x, double y)
        {
            return (utilities.distance(x - location.x, y - location.y) <= global.PumpInitRadius);
        }

        public override void updateinoutpointlocations()
        {
            outletlocation.setxy(location.x - global.PumpInitOutletLength / 2, location.y - global.PumpInitRadius);

            //Update in and out point locations;
            inpoint[0].x = location.x + global.PumpInitRadius + global.InOutPointWidth;
            inpoint[0].y = location.y;
            outpoint[0].x = location.x - global.PumpInitOutletLength - global.InOutPointWidth;
            outpoint[0].y = outletlocation.y;

            base.updateinoutpointlocations();
        }

        public override void setproperties(simulation asim)
        {
            //update(asim.simi, false);
            pumpproperties pumpprop = new pumpproperties(this, asim);
            pumpprop.Show();
        }

        public override void showtrenddetail(simulation asim, List<Form> detailtrendslist)
        {
            detailtrendslist.Add(new pumpdetail(this, asim));
            detailtrendslist[detailtrendslist.Count - 1].Show();
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
            {new Point(global.OriginX + Convert.ToInt32(global.GScale*(outletlocation.x - global.PumpInitOutletLength / 2)), 
                    global.OriginY + Convert.ToInt32(global.GScale*(outletlocation.y + global.PumpInitOutletRadius))), 
            new Point(global.OriginX + Convert.ToInt32(global.GScale*(outletlocation.x - global.PumpInitOutletLength / 2)), 
                    global.OriginY + Convert.ToInt32(global.GScale*(outletlocation.y - global.PumpInitOutletRadius))), 
            new Point(global.OriginX + Convert.ToInt32(global.GScale*(outletlocation.x + global.PumpInitOutletLength / 2)), 
                    global.OriginY + Convert.ToInt32(global.GScale*(outletlocation.y - global.PumpInitOutletRadius))), 
            new Point(global.OriginX + Convert.ToInt32(global.GScale*(outletlocation.x + global.PumpInitOutletLength / 2)),
                    global.OriginY + Convert.ToInt32(global.GScale*(outletlocation.y + global.PumpInitOutletRadius)))};

            plot1.AddPolygon(myArray);

            plot1.AddEllipse(global.OriginX + Convert.ToInt32(global.GScale * (location.x - global.PumpInitRadius)),
                            global.OriginY + Convert.ToInt32(global.GScale * (location.y - global.PumpInitRadius)),
                            Convert.ToInt32(global.GScale * (global.PumpInitRadius * 2)),
                            Convert.ToInt32(global.GScale * (global.PumpInitRadius * 2)));

            plotPen.Color = Color.Black;

            SolidBrush brush = new SolidBrush(Color.White);
            if (highlighted) { brush.Color = Color.Orange; }
            G.FillPath(brush, plot1);
            G.DrawPath(plotPen, plot1);

            base.draw(G);
        }

    }
}
