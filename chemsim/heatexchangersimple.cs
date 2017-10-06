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
    public class heatexchangersimple : unitop //This class is a simplied heat exchanger.  Static solution assuming no phase change is calculated.  First order dynamics added to that.
    {
        public controlvar U; // W/(m^2*K) ; Overall heat exchanger coefficient for heat exchanger.  Not sure if this is going to be used now.
        
        public controlvar A; // m^2 ; The contact area for the fluids with each other in the heat exchanger.  Not sure if this is going to be used now.  
        
        public double K; //constants used in calculating the new output temperatures of teh HX.
        
        //This simple heatexchanger will not be modelled with hundreds of small diameter pipes going through the exchanger.  Total flow in will be referenced
        //only, and total flow out.  No sections will be modelled in the exchanger.  Only flow, temperature, and pressure in, and flow and T and P out.

        //For the first application, namely the cooling water circuit, flow through the exchangers will be calculated based on the delta pressure over it.
        
        //The heat exchanger will be modelled as 2 streams flowing and exchanging heat.  Will be accronymed with str1 and str2.  Strm1 will be shell side, Strm2 tube side.
        //For the sake of equation signs of terms, it is assumed that str1 is the warmer stream.  But this does not need to be the case, in the case of 
        //str2 being the warmer stream, the math should still work with heat terms just being negative.
        //For the analogy with the steam generator unitop, str1 is the gas stream, and str2 is the water/steam stream.

        //Properties for stream 1
        public double strm1flowcoefficient;
        public controlvar strm1temptau;
        
        public double strm1flowtau;


        //Properties for stream 2
        public controlvar strm2flowcoefficient;
        public controlvar strm2temptau;
        
        public double strm2flowtau;

        //Variables for stream 1 equations
        public double strm1massflownew; //kg/s
        public double dstrm1massflowdt; //kg/s/s
        public double strm1pressureinnew; //Pa
        public double dstrm1pressureindt; //Pa/s
        public double strm1temperatureoutnew; //Kelvin
        public double dstrm1temperatureoutnewdt; //Kelvin

        //Stream 2 flow
        public double strm2massflownew; //kg/s
        public double dstrm2massflowdt; //kg/s
        public double strm2pressureinnew; //Pa
        public double dstrm2pressureindt; //Pa/s
        public double strm2temperatureoutnew; //Kelvin
        public double dstrm2temperatureoutnewdt; //Kelvin

        
        public heatexchangersimple(int anr, double ax, double ay)
            : base(anr, ax, ay, global.HeatExchangerNIn, global.HeatExchangerNOut)
        {
            initheatexchangersimple();
        }

        public heatexchangersimple(baseclass baseclasscopyfrom) :
            base(0, 0, 0, global.HeatExchangerNIn, global.HeatExchangerNOut)
        {
            initheatexchangersimple();
            copyfrom(baseclasscopyfrom);
        }

        public void initheatexchangersimple()
        {
            objecttype = objecttypes.HeatExchangerSimple;

            U = new controlvar(global.HeatExchangerSimpleDefaultU);
            U.simvector = new double[global.SimVectorLength];
            A = new controlvar(global.HeatExchangerSimpleDefaultA);
            A.simvector = new double[global.SimVectorLength];
            K = 0;

            name = objecttype.ToString() + " " + nr.ToString();

            controlpropthisclass.Clear();
            controlpropthisclass.AddRange(new List<string>(new string[] {"U",
                                                                        "A",
                                                                        "strm1temptau",
                                                                        "strm2temptau",
                                                                        "strm2flowcoefficient"}));
            nrcontrolpropinherited = controlproperties.Count;
            controlproperties.AddRange(controlpropthisclass);

            strm1flowcoefficient = global.HESStrm1FlowCoefficient;
            strm1temptau = new controlvar(global.HESStrm1TempTau);
            strm1temptau.simvector = new double[global.SimVectorLength];
            strm1flowtau = global.HESStrm1FlowTau;

            strm2flowcoefficient = new controlvar(global.HESStrm2FlowCoefficient);
            strm2temptau = new controlvar(global.HESStrm2TempTau);
            strm2temptau.simvector = new double[global.SimVectorLength];
            strm2flowtau = global.HESStrm2FlowTau;

            strm1massflownew = global.HESMassFlowStrm1T0; //kg/s
            dstrm1massflowdt = 0;
            strm1pressureinnew = global.HEPStrm1Inlet; //Pa
            dstrm1pressureindt = 0; //Pa/s
            strm1temperatureoutnew = global.HETStrm1Outlet; //Kelvin
            dstrm1temperatureoutnewdt = 0;

            strm2massflownew = global.HESMassFlowStrm2T0; //kg/s
            dstrm2massflowdt = 0;
            strm2pressureinnew = global.HEPStrm2Inlet; //Pa
            dstrm2pressureindt = 0; //Pa/s
            strm2temperatureoutnew = global.HETStrm2Outlet; //Kelvin
            dstrm2temperatureoutnewdt = 0;
        }

        public override void copyfrom(baseclass baseclasscopyfrom)
        {
            heatexchangersimple heatexchangersimplecopyfrom = (heatexchangersimple)baseclasscopyfrom;

            base.copyfrom(heatexchangersimplecopyfrom);

            U.v = heatexchangersimplecopyfrom.U.v; 
            A.v = heatexchangersimplecopyfrom.A.v;   
            K = heatexchangersimplecopyfrom.K;

            strm1flowcoefficient = heatexchangersimplecopyfrom.strm1flowcoefficient;
            strm1temptau.v = heatexchangersimplecopyfrom.strm1temptau.v;
            strm1flowtau = heatexchangersimplecopyfrom.strm1flowtau;
            strm2flowcoefficient = heatexchangersimplecopyfrom.strm2flowcoefficient;
            strm2temptau.v = heatexchangersimplecopyfrom.strm2temptau.v;
            strm2flowtau = heatexchangersimplecopyfrom.strm2flowtau;

            strm1massflownew = heatexchangersimplecopyfrom.strm1massflownew; //kg/s
            dstrm1massflowdt = heatexchangersimplecopyfrom.dstrm1massflowdt; //kg/s/s
            strm1pressureinnew = heatexchangersimplecopyfrom.strm1pressureinnew; //Pa
            dstrm1pressureindt = heatexchangersimplecopyfrom.dstrm1pressureindt; //Pa/s
            strm1temperatureoutnew = heatexchangersimplecopyfrom.strm1temperatureoutnew; //Kelvin
            dstrm1temperatureoutnewdt = heatexchangersimplecopyfrom.dstrm1temperatureoutnewdt; //Kelvin

            //Stream 2 flow
            strm2massflownew = heatexchangersimplecopyfrom.strm2massflownew; //kg/s
            dstrm2massflowdt = heatexchangersimplecopyfrom.dstrm2massflowdt; //kg/s
            strm2pressureinnew = heatexchangersimplecopyfrom.strm2pressureinnew; //Pa
            dstrm2pressureindt = heatexchangersimplecopyfrom.dstrm2pressureindt; //Pa/s
            strm2temperatureoutnew = heatexchangersimplecopyfrom.strm2temperatureoutnew; //Kelvin
            dstrm2temperatureoutnewdt = heatexchangersimplecopyfrom.dstrm2temperatureoutnewdt; //Kelvin
        }

        public override controlvar selectedproperty(int selection)
        {
            if (selection >= nrcontrolpropinherited)
            {
                switch (selection - nrcontrolpropinherited)
                {
                    case 0:
                        return U;
                    case 1:
                        return A;
                    case 2:
                        return strm1temptau;
                    case 3:
                        return strm2temptau;
                    case 4:
                        return strm2flowcoefficient;
                    default:
                        return null;
                }
            }
            else { return base.selectedproperty(selection); };
        }

        //private void calcCpm(int i) //I think this method will not be needed anymore.
        //{
        //    Cpm[i] = 128.1 * Math.Log(Tmave[i]) - 264.11;  //Equation is from Excel sheet.
        //}

        public void ddt(int simi)   //Differential equations
        {

            dstrm1pressureindt = -1 / strm1flowtau * inflow[0].mat.P.v + 1 / strm1flowtau * strm1pressureinnew;
            dstrm2pressureindt = -1 / strm2flowtau * inflow[1].mat.P.v + 1 / strm2flowtau * strm2pressureinnew;

            dstrm1temperatureoutnewdt = -1 / strm1temptau.v * outflow[0].mat.T.v + 1 / strm1temptau.v * strm1temperatureoutnew;
            dstrm2temperatureoutnewdt = -1 / strm2temptau.v * outflow[1].mat.T.v + 1 / strm2temptau.v * strm2temperatureoutnew;

        }

        public override void update(int simi, bool historise)
        {
            
            

            //strm1massflownew = global.HESStrm1FlowCoefficient * Math.Sqrt((inflow[0].mat.P.v - outflow[0].mat.P.v + global.Epsilon) * inflow[0].mat.density.v);
            //strm2massflownew = global.HESStrm2FlowCoefficient * Math.Sqrt((inflow[1].mat.P.v - outflow[1].mat.P.v + global.Epsilon) * inflow[1].mat.density.v);

            strm1pressureinnew = outflow[0].mat.P.v + 
                Math.Pow(inflow[0].massflow.v / strm1flowcoefficient, 2) * (inflow[0].mat.density.v + global.Epsilon);
            strm2pressureinnew = outflow[1].mat.P.v + 
                Math.Pow(inflow[1].massflow.v / strm2flowcoefficient.v, 2) * (inflow[1].mat.density.v + global.Epsilon);

            double f1 = inflow[0].massflow.v;
            double f2 = inflow[1].massflow.v;
            double C1 = inflow[0].mat.totalCp / inflow[0].mat.massofonemole;
            double C2 = inflow[1].mat.totalCp / inflow[1].mat.massofonemole;
            double T1in = inflow[0].mat.T.v;
            double T2in = inflow[1].mat.T.v;

            //After the flow variables have been solved we can solve the temperature variables static solutions.
            double Knew = U.v * A.v * (1 / (f1*C1 + global.Epsilon) - 
                1/(f2*C2 + global.Epsilon));

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
                strm2temperatureoutnew = (f1*C1*(T1in - T1out) + f2*C2*T2in)/(f2*C2 + global.Epsilon);
            }

            ddt(simi);

            //inflow[0].massflow.v += dstrm1massflowdt * global.SampleT;
            //inflow[0].update(simi);
            inflow[0].mat.P.v += dstrm1pressureindt * global.SampleT;
            outflow[0].massflow.v = inflow[0].massflow.v;
            outflow[0].mat.density.v = inflow[0].mat.density.v;

            //inflow[1].massflow.v += dstrm2massflowdt * global.SampleT;
            //inflow[1].update(simi);
            inflow[1].mat.P.v += dstrm2pressureindt * global.SampleT;
            outflow[1].massflow.v = inflow[1].massflow.v;
            outflow[1].mat.density.v = inflow[1].mat.density.v;

            outflow[0].mat.T.v += dstrm1temperatureoutnewdt * global.SampleT;
            outflow[1].mat.T.v += dstrm2temperatureoutnewdt * global.SampleT;

            //outflow[0].update(simi);  I do not think we need to update these here, since they will be updated in the simulation class sim method.
            //outflow[1].update(simi);

            if (historise && (simi % global.SimVectorUpdatePeriod == 0))
            {
                if (U.simvector != null) { U.simvector[simi / global.SimVectorUpdatePeriod] = U.v; }
                
                if (A.simvector != null) { A.simvector[simi / global.SimVectorUpdatePeriod] = A.v; }
                
                if (strm1temptau.simvector != null) { strm1temptau.simvector[simi / global.SimVectorUpdatePeriod] = strm1temptau.v; }
                
                if (strm2temptau.simvector != null) { strm2temptau.simvector[simi / global.SimVectorUpdatePeriod] = strm2temptau.v; }
                
                if (strm2flowcoefficient.simvector != null) { strm2flowcoefficient.simvector[simi / global.SimVectorUpdatePeriod] = strm2flowcoefficient.v; }
                                
            }

        }

        //private void calcdynviscosity(int i) //Dynamic Viscosity in watersegment[i]
        //{
        //    strm2dynviscosity[i] = global.DynViscA * Math.Exp(global.DynViscB * strm2segments[i].T.v);
        //}

        public override void showtrenddetail(simulation asim, List<Form> detailtrendslist)
        {
            detailtrendslist.Add(new heatexchangersimpledetail(this, asim));
            detailtrendslist[detailtrendslist.Count - 1].Show();
        }

        //public override void updatetrenddetail(simulation asim)
        //{
        //    if (detailtrends != null && detailtrends.Visible) { detailtrends.Invalidate(); }
        //}

        public override bool mouseover(double x, double y)
        {
            return (x >= (location.x - global.HeatExchangerWidth / 2) && x <= (location.x + global.HeatExchangerWidth / 2)
                && y >=
                (location.y - global.HeatExchangerRadius) && y <= (location.y + global.HeatExchangerRadius));
        }

        public override void updateinoutpointlocations()
        {
            //Update in and out point locations;

            inpoint[0].x = location.x - 0.5*global.HeatExchangerWidth + global.HeatExchangerInPointsFraction[0] *
                global.HeatExchangerWidth;
            inpoint[0].y = location.y - global.HeatExchangerRadius - global.InOutPointWidth;

            inpoint[1].x = location.x - 0.5 * global.HeatExchangerWidth + global.HeatExchangerInPointsFraction[1] *
                global.HeatExchangerWidth;
            inpoint[1].y = location.y + global.HeatExchangerRadius + global.InOutPointWidth;

            outpoint[0].x = location.x - 0.5 * global.HeatExchangerWidth + global.HeatExchangerInPointsFraction[0] *
                global.HeatExchangerWidth;
            outpoint[0].y = location.y + global.HeatExchangerRadius + global.InOutPointWidth;

            outpoint[1].x = location.x - 0.5 * global.HeatExchangerWidth + global.HeatExchangerInPointsFraction[1] *
                global.HeatExchangerWidth;
            outpoint[1].y = location.y - global.HeatExchangerRadius - global.InOutPointWidth;

            base.updateinoutpointlocations();
        }

        public override void setproperties(simulation asim)
        {
            heatexchangersimpleproperties heatexchangersimpleprop = new heatexchangersimpleproperties(this, asim);
            heatexchangersimpleprop.Show();
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
            {new Point(global.OriginX + Convert.ToInt32(global.GScale*(location.x - 0.5*global.HeatExchangerWidth)), 
                    global.OriginY + Convert.ToInt32(global.GScale*(location.y + global.HeatExchangerRadius))), 
            new Point(global.OriginX + Convert.ToInt32(global.GScale*(location.x - 0.5*global.HeatExchangerWidth)), 
                    global.OriginY + Convert.ToInt32(global.GScale*(location.y - global.HeatExchangerRadius))), 
            new Point(global.OriginX + Convert.ToInt32(global.GScale*(location.x + 0.5*global.HeatExchangerWidth)), 
                    global.OriginY + Convert.ToInt32(global.GScale*(location.y - global.HeatExchangerRadius))), 
            new Point(global.OriginX + Convert.ToInt32(global.GScale*(location.x + 0.5*global.HeatExchangerWidth)),
                    global.OriginY + Convert.ToInt32(global.GScale*(location.y + global.HeatExchangerRadius)))};
            tankmain.AddPolygon(myArray);
            plotPen.Color = Color.Black;
            SolidBrush brush = new SolidBrush(Color.White);
            brush.Color = (highlighted) ? Color.Orange : Color.White;
            G.FillPath(brush, tankmain);
            G.DrawPath(plotPen, tankmain);

            //The writing of the name of the unitop in the unitop.
            GraphicsPath unitopname = new GraphicsPath();
            StringFormat format = StringFormat.GenericDefault;
            FontFamily family = new FontFamily("Arial");
            int myfontStyle = (int)FontStyle.Bold;
            int emSize = 10;
            PointF namepoint = new PointF(global.OriginX + Convert.ToInt32(global.GScale*(location.x) - name.Length*emSize/2/2),
                global.OriginY + Convert.ToInt32(global.GScale*(location.y)));
            unitopname.AddString(name, family, myfontStyle, emSize, namepoint, format);
            G.FillPath(Brushes.Black, unitopname);

            //Draw inpoint
            base.draw(G);
        }
    }
}
