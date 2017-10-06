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
    public class coolingtowerheatexchangersimple : unitop //This class is a cooling tower that is modelled on the math of the heatexchanger simple unitop
    {
        public double tuningfactor; //dimensionless.  This factor will be used to tune the simultion in terms chilling in the cooling tower.
        public double outflow0temperatureoutnew; //Kelvin
        public double doutflow0temperatureoutnewdt; //Kelvin/sec

        public double U; // W/(m^2*K) ; Overall heat exchanger coefficient for heat exchanger.  Not sure if this is going to be used now.
        public double A; // m^2 ; The contact area for the fluids with each other in the heat exchanger.  Not sure if this is going to be used now.  
        public double K; //constants used in calculating the new output temperatures of the HX.

        //Properties for stream 1
        public double strm1flowcoefficient;
        public double strm1temptau;
        public double strm1flowtau;


        //Properties for stream 2
        public double strm2flowcoefficient;
        public double strm2temptau;
        public double strm2flowtau;

        //Variables for stream 1 equations
        public double strm1massflownew; //kg/s
        public double dstrm1massflowdt; //kg/s/s
        public double strm1pressureoutnew; //Pa
        public double dstrm1pressureoutdt; //Pa/s
        public double strm1temperatureoutnew; //Kelvin
        public double dstrm1temperatureoutnewdt; //Kelvin

        //Stream 2 flow
        public double strm2massflownew; //kg/s
        public double dstrm2massflowdt; //kg/s
        public double strm2pressureoutnew; //Pa
        public double dstrm2pressureoutdt; //Pa/s
        public double strm2temperatureoutnew; //Kelvin
        public double dstrm2temperatureoutnewdt; //Kelvin

        public coolingtowerheatexchangersimple(int anr, double ax, double ay)
            : base(anr, ax, ay, global.CTHESNIn, global.CTHESNOut)
        {
            initcoolingtowerheatexchangersimple();
            update(0, true);
        }

        public coolingtowerheatexchangersimple(baseclass baseclasscopyfrom)
            : base(0, 0, 0, global.CTHESNIn, global.CTHESNOut)
        {
            initcoolingtowerheatexchangersimple();
            copyfrom(baseclasscopyfrom);
        }

        public void initcoolingtowerheatexchangersimple()
        {
            objecttype = objecttypes.CoolingTowerHeatExchangerSimple;
            name = nr.ToString() + " " + objecttype.ToString();

            controlpropthisclass.Clear();
            //controlpropthisclass.AddRange(new List<string>(new string[] {"tuningfactor"}));

            tuningfactor = global.CTHESTuningFactor;

            U = global.HeatExchangerSimpleDefaultU;
            A = global.HeatExchangerSimpleDefaultA;
            K = 0;

            strm1flowcoefficient = global.CTHESStrm1FlowCoefficient;
            strm1temptau = global.CTHESStrm1TempTau;
            strm1flowtau = global.CTHESStrm1FlowTau;

            strm2flowcoefficient = global.CTHESStrm2FlowCoefficient;
            strm2temptau = global.CTHESStrm2TempTau;
            strm2flowtau = global.CTHESStrm2FlowTau;

            strm1massflownew = global.CTHESMassFlowStrm1T0; //kg/s
            dstrm1massflowdt = 0;
            strm1pressureoutnew = global.CTHESPStrm1Outlet; //Pa
            dstrm1pressureoutdt = 0; //Pa/s
            strm1temperatureoutnew = global.CTHESTStrm1Outlet; //Kelvin
            dstrm1temperatureoutnewdt = 0;

            strm2massflownew = global.CTHESMassFlowStrm2T0; //kg/s
            dstrm2massflowdt = 0;
            strm2pressureoutnew = global.CTHESPStrm2Outlet; //Pa
            dstrm2pressureoutdt = 0; //Pa/s
            strm2temperatureoutnew = global.CTHESTStrm2Outlet; //Kelvin
            dstrm2temperatureoutnewdt = 0;
        }

        public override void copyfrom(baseclass baseclasscopyfrom)
        {
            coolingtowerheatexchangersimple coolingtowerheatexchangersimplecopyfrom = (coolingtowerheatexchangersimple)baseclasscopyfrom;

            base.copyfrom(coolingtowerheatexchangersimplecopyfrom);

            tuningfactor = coolingtowerheatexchangersimplecopyfrom.tuningfactor;
            outflow0temperatureoutnew = coolingtowerheatexchangersimplecopyfrom.outflow0temperatureoutnew; //Kelvin
            doutflow0temperatureoutnewdt = coolingtowerheatexchangersimplecopyfrom.doutflow0temperatureoutnewdt; //Kelvin/sec

            strm1flowcoefficient = coolingtowerheatexchangersimplecopyfrom.strm1flowcoefficient;
            strm1temptau = coolingtowerheatexchangersimplecopyfrom.strm1temptau;
            strm1flowtau = coolingtowerheatexchangersimplecopyfrom.strm1flowtau;
            strm2flowcoefficient = coolingtowerheatexchangersimplecopyfrom.strm2flowcoefficient;
            strm2temptau = coolingtowerheatexchangersimplecopyfrom.strm2temptau;
            strm2flowtau = coolingtowerheatexchangersimplecopyfrom.strm2flowtau;

            strm1massflownew = coolingtowerheatexchangersimplecopyfrom.strm1massflownew; //kg/s
            dstrm1massflowdt = coolingtowerheatexchangersimplecopyfrom.dstrm1massflowdt; //kg/s/s
            strm1pressureoutnew = coolingtowerheatexchangersimplecopyfrom.strm1pressureoutnew; //Pa
            dstrm1pressureoutdt = coolingtowerheatexchangersimplecopyfrom.dstrm1pressureoutdt; //Pa/s
            strm1temperatureoutnew = coolingtowerheatexchangersimplecopyfrom.strm1temperatureoutnew; //Kelvin
            dstrm1temperatureoutnewdt = coolingtowerheatexchangersimplecopyfrom.dstrm1temperatureoutnewdt; //Kelvin

            //Stream 2 flow
            strm2massflownew = coolingtowerheatexchangersimplecopyfrom.strm2massflownew; //kg/s
            dstrm2massflowdt = coolingtowerheatexchangersimplecopyfrom.dstrm2massflowdt; //kg/s
            strm2pressureoutnew = coolingtowerheatexchangersimplecopyfrom.strm2pressureoutnew; //Pa
            dstrm2pressureoutdt = coolingtowerheatexchangersimplecopyfrom.dstrm2pressureoutdt; //Pa/s
            strm2temperatureoutnew = coolingtowerheatexchangersimplecopyfrom.strm2temperatureoutnew; //Kelvin
            dstrm2temperatureoutnewdt = coolingtowerheatexchangersimplecopyfrom.dstrm2temperatureoutnewdt; //Kelvin
        }

        public void ddt(int simi)   //Differential equations
        {

            dstrm1pressureoutdt = -1 / strm1flowtau * outflow[0].mat.P.v + 1 / strm1flowtau * strm1pressureoutnew;
            dstrm2pressureoutdt = -1 / strm2flowtau * outflow[1].mat.P.v + 1 / strm2flowtau * strm2pressureoutnew;

            dstrm1temperatureoutnewdt = -1 / strm1temptau * outflow[0].mat.T.v + 1 / strm1temptau * strm1temperatureoutnew;
            dstrm2temperatureoutnewdt = -1 / strm2temptau * outflow[1].mat.T.v + 1 / strm2temptau * strm2temperatureoutnew;

        }

        public override void update(int simi, bool historise)
        {

            strm1pressureoutnew = inflow[0].mat.P.v -
                Math.Pow(inflow[0].massflow.v / strm1flowcoefficient, 2) * (inflow[0].mat.density.v + global.Epsilon);
            strm2pressureoutnew = inflow[1].mat.P.v -
                Math.Pow(inflow[1].massflow.v / strm2flowcoefficient, 2) * (inflow[1].mat.density.v + global.Epsilon);

            double f1 = inflow[0].massflow.v;
            double f2 = inflow[1].massflow.v;
            double C1 = inflow[0].mat.totalCp / inflow[0].mat.massofonemole;
            double C2 = inflow[1].mat.totalCp / inflow[1].mat.massofonemole;
            double T1in = inflow[0].mat.T.v;
            double T2in = inflow[1].mat.T.v;

            //After the flow variables have been solved we can solve the temperature variables static solutions.
            double Knew = U * A * (1 / (f1 * C1 + global.Epsilon) -
                1 / (f2 * C2 + global.Epsilon));

            if (!Double.IsNaN(Knew)) { K = Knew; }

            outflow[0].mat.copycompositiontothismat(inflow[0].mat);
            outflow[1].mat.copycompositiontothismat(inflow[1].mat);

            double strm1temperatureoutnewden = (f2 * C2 * Math.Exp(K) - f1 * C1 + global.Epsilon);
            if (Double.IsPositiveInfinity(strm1temperatureoutnewden))
            {
                strm1temperatureoutnewden = Double.MaxValue;
            }
            else if (strm1temperatureoutnewden != 0)
            {
                strm1temperatureoutnew = (f2 * C2 * (T1in + Math.Exp(K) * T2in - T2in) - f1 * C1 * T1in) / strm1temperatureoutnewden;
                double T1out = strm1temperatureoutnew;
                strm2temperatureoutnew = (f1 * C1 * (T1in - T1out) + f2 * C2 * T2in) / (f2 * C2 + global.Epsilon);
            }

            ddt(simi);

            //inflow[0].massflow.v += dstrm1massflowdt * global.SampleT;
            //inflow[0].update(simi);
            outflow[0].mat.P.v += dstrm1pressureoutdt * global.SampleT; 
            outflow[0].mat.P.v = global.Ps; //standard pressure since it is open to the atmosphere.
            outflow[0].massflow.v = inflow[0].massflow.v;
            outflow[0].mat.density.v = inflow[0].mat.density.v;

            //inflow[1].massflow.v += dstrm2massflowdt * global.SampleT;
            //inflow[1].update(simi);
            outflow[1].mat.P.v += dstrm2pressureoutdt * global.SampleT;
            outflow[1].massflow.v = inflow[1].massflow.v;
            outflow[1].mat.density.v = inflow[1].mat.density.v;

            outflow[0].mat.T.v += dstrm1temperatureoutnewdt * global.SampleT;
            outflow[1].mat.T.v += dstrm2temperatureoutnewdt * global.SampleT;

            if (outflow[0].mat.T.v < 0) { outflow[0].mat.T.v = 0; }

        }

        public override void setproperties(simulation asim) //Method that will be inherited and that will set the properties of the applicable object in a window
        {
            coolingtowerheatexchangersimpleproperties coolingtowerheatexchangersimpleprop = new coolingtowerheatexchangersimpleproperties(this, asim);
            coolingtowerheatexchangersimpleprop.Show();
        }

        public override bool mouseover(double x, double y)
        {
            return (x >= (location.x - 0.5 * global.CTHESWidth) && x <= (location.x + 0.5 * global.CTHESWidth)
                && y >=
                (location.y - 0.5 * global.CTHESHeight) && y <= (location.y + 0.5 * global.CTHESHeight));
        }

        public override void updateinoutpointlocations()
        {
            //Update in and out point locations;

            inpoint[0].x = location.x - 0.5 * global.CTSWidth + global.CTHESInPointsFraction[0] *
                global.CTSWidth;
            inpoint[0].y = location.y - 0.5 * global.CTSHeight - global.InOutPointWidth;

            inpoint[1].x = location.x - 0.5 * global.CTSWidth + global.CTHESInPointsFraction[1] *
                global.CTSWidth;
            inpoint[1].y = location.y + 0.5 * global.CTSHeight + global.InOutPointWidth;

            outpoint[0].x = location.x - 0.5 * global.CTSWidth + global.CTHESOutPointsFraction[0] *
                global.CTSWidth;
            outpoint[0].y = location.y + 0.5 * global.CTSHeight + global.InOutPointWidth;

            outpoint[1].x = location.x - 0.5 * global.CTSWidth + global.CTHESOutPointsFraction[1] *
                global.CTSWidth;
            outpoint[1].y = location.y - 0.5 * global.CTSHeight - global.InOutPointWidth;

            base.updateinoutpointlocations();
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
            {new Point(global.OriginX + Convert.ToInt32(global.GScale*(location.x - 0.5*global.CTSWidth)), 
                    global.OriginY + Convert.ToInt32(global.GScale*(location.y + 0.5*global.CTSHeight))), 
            new Point(global.OriginX + Convert.ToInt32(global.GScale*(location.x - 0.5*global.CTSWidth)), 
                    global.OriginY + Convert.ToInt32(global.GScale*(location.y - 0.5*global.CTSHeight))), 
            new Point(global.OriginX + Convert.ToInt32(global.GScale*(location.x + 0.5*global.CTSWidth)), 
                    global.OriginY + Convert.ToInt32(global.GScale*(location.y - 0.5*global.CTSHeight))), 
            new Point(global.OriginX + Convert.ToInt32(global.GScale*(location.x + 0.5*global.CTSWidth)),
                    global.OriginY + Convert.ToInt32(global.GScale*(location.y + 0.5*global.CTSHeight)))};
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
