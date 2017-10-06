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
    public class valve : unitop
    {
        //variables
        public controlvar deltapressure; //Pa
        //public double[] deltapressuresimvector; //Pa; history sim vector.
        public double Cv; //Valve coeficient
        public controlvar op; //Fraction : valve opening as a fraction
        //public double[] opsimvector;

        public double deltapressurenew; //Pa
        public double ddeltapressuredt; //Pa/s

        //methods
        public valve(int anr, double ax, double ay, double aCv, double aop)
            : base(anr, ax, ay, 1, 1)
        {
            initvalve(anr, ax, ay, aCv, aop);
        }

        public valve(baseclass baseclasscopyfrom)
            : base(0, 0, 0, 1, 1) //these numbes do not matter much since they will be sorted anyway by the copymethods down the 
                                    //hierarchy
        {
            initvalve(0, 0, 0, 0, 0);
            copyfrom(baseclasscopyfrom);
        }

        public void initvalve(int anr, double ax, double ay, double aCv, double aop)
        {
            objecttype = objecttypes.Valve;

            deltapressure = new controlvar();
            op = new controlvar();

            name = nr.ToString() + " " + objecttype.ToString();

            controlpropthisclass.Clear();
            controlpropthisclass.AddRange(new List<string>(new string[] {"deltapressure",
                                                                        "op"}));
            nrcontrolpropinherited = controlproperties.Count;
            controlproperties.AddRange(controlpropthisclass);

            Cv = aCv;
            op.v = aop;
            deltapressure.v = 0;
            //deltapressuresimvector = new double[global.SimVectorLength];
            //opsimvector = new double[global.SimVectorLength];

            actualvolumeflow.v = 0;

            deltapressurenew = 0; //Pa
            ddeltapressuredt = 0;

            updateinoutpointlocations();

            update(0, false);
        }

        public override void copyfrom(baseclass baseclasscopyfrom)
        {
            valve valvecopyfrom = (valve)baseclasscopyfrom;

            base.copyfrom(valvecopyfrom);

            deltapressure.v = valvecopyfrom.deltapressure.v; //Pa
            Cv = valvecopyfrom.Cv; //Valve coeficient
            op.v = valvecopyfrom.op.v; //Fraction : valve opening as a fraction

            deltapressurenew = valvecopyfrom.deltapressurenew; //Pa
            ddeltapressuredt = valvecopyfrom.ddeltapressuredt;
        }

        public override controlvar selectedproperty(int selection)
        {
            if (selection >= nrcontrolpropinherited)
            {
                switch (selection - nrcontrolpropinherited)
                {
                    case 0:
                        return deltapressure;
                    case 1:
                        return op;
                    default:
                        return null;
                }
            }
            else { return base.selectedproperty(selection); };
        }

        public void ddt(int simi)
        {

            ddeltapressuredt = -1 / global.ValveHydraulicTau * deltapressure.v + 1 / global.ValveHydraulicTau * deltapressurenew;
        }

        public override void update(int simi, bool historise)
        {
            
            if (inflow[0] != null && outflow[0] != null)
            {
                mat.copycompositiontothisobject(inflow[0].mat);
                mat.density.v = inflow[0].mat.density.v;
                mat.T.v = inflow[0].mat.T.v;
                massflow.v = inflow[0].massflow.v;
                actualvolumeflow.v = massflow.v/mat.density.v;
                deltapressurenew = Math.Pow(actualvolumeflow.v / (Cv * Math.Pow(global.ValveEqualPercR, op.v - 1)), 2);

                ddt(simi);

                deltapressure.v += ddeltapressuredt * global.SampleT;
                    
                inflow[0].mat.P.v = outflow[0].mat.P.v + deltapressure.v;
                
                calcmolarflowfrommassflow();
                calcstandardflowfrommoleflow();

                outflow[0].mat.copycompositiontothisobject(mat);
                outflow[0].massflow.v = massflow.v;
                outflow[0].mat.density.v = mat.density.v;
                outflow[0].mat.T.v = mat.T.v;
            }

            //if (op.v > 1) { op.v = 1; }
            if (op.v < 0.00) { op.v = 0.00; }

            if (historise && (simi % global.SimVectorUpdatePeriod == 0))
            {
                if (deltapressure.simvector != null) { deltapressure.simvector[simi / global.SimVectorUpdatePeriod] = deltapressure.v; }

                if (op.simvector != null) { op.simvector[simi / global.SimVectorUpdatePeriod] = op.v; }

                if (actualvolumeflow.simvector != null) { actualvolumeflow.simvector[simi / global.SimVectorUpdatePeriod] = actualvolumeflow.v; }

                if (standardvolumeflow.simvector != null) { standardvolumeflow.simvector[simi / global.SimVectorUpdatePeriod] = standardvolumeflow.v; }

                if (massflow.simvector != null) { massflow.simvector[simi / global.SimVectorUpdatePeriod] = massflow.v; }

                if (molarflow.simvector != null) { molarflow.simvector[simi / global.SimVectorUpdatePeriod] = molarflow.v; }                
            }
        }

        //public void sizevalvefromstandardflow()
        //{
        //    calcmassflowfromstandardflow();
        //    calcactualvolumeflowfrommassflow();
        //    if (Math.Abs(deltapressure) > 0) { Cv = actualvolumeflow / (op * Math.Sqrt(Math.Abs(deltapressure))); }
        //}

        public void sizevalvefromactualvolumeflow()
        {
            if (Math.Abs(deltapressure.v) > 0) { Cv = actualvolumeflow.v / (op.v * Math.Sqrt(Math.Abs(deltapressure.v))); }
        }

        public override void showtrenddetail(simulation asim, List<Form> detailtrendslist)
        {
            detailtrendslist.Add(new valvedetail(this, asim));
            detailtrendslist[detailtrendslist.Count - 1].Show();
        }

        public override bool mouseover(double x, double y)
        {
            return (utilities.distance(x - location.x, y - location.y) <= global.ValveLength);
        }

        public override void updateinoutpointlocations()
        {
            inpoint[0].x = location.x - global.ValveLength / 2 - global.InOutPointWidth;
            inpoint[0].y = location.y;
            outpoint[0].x = location.x + global.ValveLength / 2 + global.InOutPointWidth;
            outpoint[0].y = location.y;

            base.updateinoutpointlocations();
        }

        public override void setproperties(simulation asim)
        {
            update(asim.simi, false);
            valveproperties valveprop = new valveproperties(this, asim);
            valveprop.Show();
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
            {new Point(global.OriginX + Convert.ToInt32(global.GScale*(location.x - global.ValveLength/2)), 
                    global.OriginY + Convert.ToInt32(global.GScale*(location.y - global.ValveWidth/2))), 
            new Point(global.OriginX + Convert.ToInt32(global.GScale*(location.x + global.ValveLength / 2)), 
                    global.OriginY + Convert.ToInt32(global.GScale*(location.y + global.ValveWidth/2))), 
            new Point(global.OriginX + Convert.ToInt32(global.GScale*(location.x + global.ValveLength / 2)), 
                    global.OriginY + Convert.ToInt32(global.GScale*(location.y - global.ValveWidth/2))), 
            new Point(global.OriginX + Convert.ToInt32(global.GScale*(location.x - global.ValveLength / 2)),
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
