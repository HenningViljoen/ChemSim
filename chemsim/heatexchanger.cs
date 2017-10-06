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
    public class heatexchanger : unitop
    {
        //The pressure drop over the exchanger will determine the flow later on
        
        //The heat exchanger will be modelled as 2 streams flowing and exchanging heat.  Will be accronymed with str1 and str2.  Strm1 will be shell side, Strm2 tube side.
        //For the sake of equation signs of terms, it is assumed that str1 is the warmer stream.  But this does not need to be the case, in the case of 
        //str2 being the warmer stream, the math should still work with heat terms just being negative.
        //For the analogy with the steam generator unitop, str1 is the gas stream, and str2 is the water/steam stream.

        public List<material> strm1segments; //Stream1 inventory.
        public List<material> strm2segments; //Stream2 inventory.

        //Variables for stream 1 equations
        public double[] dnstrm1boundarydt; //moles/s.  Stream 1 mass flow at the region boundaries.
        public double[,] dnstrm1compboundarydt; //moles/s.  Gas mass flow at the region boundaries, per component.
        public double[][] dnstrm1boundarydtsimvector;
        public double[] strm1meanvelocity; // m/s
        public double[] strm1dynviscosity; //Pa s
        public double[] strm1reanoldsnumber;
        public double[] strm1frictionfactor;
        public double[] strm1frictionforce; //Newton
        public double[] strm1frictionenergylosspsecond; //Joule

        public double[] dnstrm1segmentdt; //ddt of moles in each stream1 segment.
        public double[,] dnstrm1compsegmentdt; //ddt of moles in each gas segment per component.
        public double[] dUstrm1dt; //ddt of internal energy in each stream1 segment.

        //Declaration of variables for METAL HEAT equation

        public double[] Qstrm1m; //Joule/second (W); Heat transfer per region from stream1 to the metal.  This var has to be in the 
        //flash calc for the stream1 as well.
        public double[][] Qstrm1msimvector;

        public double[] Cpm; //J/(kg*K)  Specific heat capacity of the metal of the tube per region.  
        //It will be a function of the region metal temperature.
        public double[][] Cpmsimvector;

        public double[] Tmave; //Kelvin; metal temperature per region.
        public double[] Tmavetemp; //WILL PROBABLY NOT NEED THIS VAR IN THE FUTURE. temp var for integration.
        public double[] dTmavedt;
        public double[][] dTmavedtk;
        public double[][] Tmavesimvector;

        public double[] Qmstrm2; //Joule/second (W); heat transfer between metal and stream2 per region.
        public double[][] Qmstrm2simvector;

        //HEAT EXCHANGE equation
        public double[] heattransferarea; //Heat transfer areas between the different boundary areas.

        //Stream 2 flow
        public double[] dnstrm2boundarydt; //moles/s  Steam mass flow at the region border.
        public double[][] dnstrm2boundarydtsimvector;
        public double[] strm2meanvelocity; // m/s
        public double[] strm2dynviscosity; //Pa s
        public double[] strm2reanoldsnumber;
        public double[] strm2frictionfactor;
        public double[] strm2frictionforce; //Newton
        public double[] strm2frictionenergylosspsecond; //Joule

        public double[] dnstrm2segmentdt; //ddt of moles in each strm2 segment.
        public double[] dUstrm2dt; //ddt of internal energy in each water segment.
        //MOMENTUM equation



        public heatexchanger(int anr, double ax, double ay)
            : base(anr, ax, ay, global.HeatExchangerNIn, global.HeatExchangerNOut)
        {
            //U = global.HeatExchangerDefaultU; //These will have to be used later, will first model it as per the steam generator.
            //A = global.HeatExchangerDefaultA; //These will have to be used later, will first model it as per the steam generator.

            //Initialising the input streams
            //inflow[0] = new stream(0, ax, ay, ax, ay);   //later this steam will have to be supplied by the user.   `
            //inflow[0].mat.init("Carbon Dioxide", global.HETStrm1T0[2], global.HEStrm1SegmentVolume, global.HEPStrm1Inlet, 1);
            //inflow[0].molarflow.v = global.HEMassFlowStrm1T0 * global.NStrm2Coils / inflow[0].mat.composition[0].m.molarmass;
            //inflow[1] = new stream(1, ax, ay, ax, ay);
            //inflow[1].mat.init("Water", global.HETStrm2T0[0], global.HEStrm2SegmentVolume, global.HEPStrm2Inlet, 0);
            //outflow[0] = new stream(0, ax, ay, ax, ay);   //later this steam will have to be supplied by the user.
            //outflow[0].mat.init("Carbon Dioxide", global.HETStrm1T0[0], global.HEStrm1SegmentVolume, global.HEPStrm1Outlet, 1);
            //outflow[0].molarflow.v = global.HEMassFlowStrm1T0 * global.NStrm2Coils / inflow[0].mat.composition[0].m.molarmass;
            //outflow[1] = new stream(1, ax, ay, ax, ay);
            //outflow[1].mat.init("Water", global.HETStrm2T0[0], global.HEStrm2SegmentVolume, global.HEPStrm2Outlet, 0);

            //Material class initialisation for the steam lines and the gas lines.
            strm1segments = new List<material>();
            strm2segments = new List<material>();
            for (int i = 0; i < global.HENSegments; i++)
            {
                //strm1segments.Add(new material("Carbon Dioxide", global.HETStrm1T0[i], global.HEStrm1SegmentVolume,
                //    global.HEPStrm1T0[i], 1));
                //strm2segments.Add(new material("Water", global.HETStrm2T0[i], global.HEStrm2SegmentVolume,
                //    global.HEPStrm2T0[i], 0));
            }

            //Initialise variables for gas molar flow variables
            dnstrm1boundarydt = new double[global.HENNodes];
            dnstrm1compboundarydt = new double[global.HENNodes, strm1segments[0].composition.Count];
            dnstrm1boundarydtsimvector = new double[global.HENNodes][];
            for (int i = 0; i < global.HENNodes; i++)
            {
                dnstrm1boundarydt[i] = global.HEMassFlowArrayStrm1T0[i] / strm1segments[0].composition[0].m.molarmass; //initial molar flow at the boundary moles/s.
                for (int j = 0; j < strm1segments[0].composition.Count; j++) //These references to gassegment[0] will need to change to become a lot more dynamic later on.
                {
                    material mat = (i == global.HENNodes - 1) ? inflow[0].mat : strm1segments[i];
                    dnstrm1compboundarydt[i, j] = dnstrm1boundarydt[i] * mat.z[j];
                }
                dnstrm1boundarydtsimvector[i] = new double[global.SimIterations];
            }
            strm1meanvelocity = new double[global.HENNodes]; // m/s
            strm1dynviscosity = new double[global.HENNodes];
            strm1reanoldsnumber = new double[global.HENNodes];
            strm1frictionfactor = new double[global.HENNodes];
            strm1frictionforce = new double[global.HENNodes];
            strm1frictionenergylosspsecond = new double[global.HENNodes];

            dnstrm1segmentdt = new double[global.HENSegments];
            dnstrm1compsegmentdt = new double[global.HENSegments, inflow[0].mat.composition.Count];
            dUstrm1dt = new double[global.HENSegments];

            Qstrm1m = new double[global.HENSegments];
            Qstrm1msimvector = new double[global.HENSegments][];
            for (int i = 0; i < global.HENSegments; i++)
            {
                Qstrm1m[i] = global.HEThermalPower / global.NStrm2Coils / global.HENSegments; // global.QgmT0[i];
                Qstrm1msimvector[i] = new double[global.SimIterations];
            }

            //Initialise variables for the METAL HEAT EQUATION
            Tmave = new double[global.HENSegments];
            Tmavetemp = new double[global.HENSegments];
            dTmavedt = new double[global.HENSegments];
            dTmavedtk = new double[global.HENSegments][];
            Tmavesimvector = new double[global.HENSegments][];
            for (int i = 0; i < global.HENSegments; i++)
            {
                //Tmave[i] = 0.5 * (global.HETStrm1T0[i] + global.HETStrm2T0[i]);
                Tmave[i] = global.HETStrm2T0[i];
                Tmavetemp[i] = Tmave[i];
                dTmavedt[i] = 0;
                dTmavedtk[i] = new double[global.RungaKuta];
                Tmavesimvector[i] = new double[global.SimIterations];
            }

            Cpm = new double[global.HENSegments];
            Cpmsimvector = new double[global.HENSegments][];
            for (int i = 0; i < global.HENSegments; i++)
            {
                calcCpm(i);
                Cpmsimvector[i] = new double[global.SimIterations];
            }

            Qmstrm2 = new double[global.HENSegments];
            Qmstrm2simvector = new double[global.HENSegments][];
            for (int i = 0; i < global.HENSegments; i++)
            {
                Qmstrm2[i] = global.HEThermalPower / global.NStrm2Coils / global.HENSegments; // global.QmsT0[i];
                Qmstrm2simvector[i] = new double[global.SimIterations];
            }

            //HEAT TRANSFER equation
            heattransferarea = new double[global.HENSegments];

            //STRJ2 FLOW
            dnstrm2boundarydt = new double[global.HENNodes];
            dnstrm2boundarydtsimvector = new double[global.HENNodes][];
            for (int i = 0; i < global.HENNodes; i++)
            {
                dnstrm2boundarydt[i] = global.MolFlowStrm2T0; //convert from mass to molar flow. Simple for ONE
                //component.
                dnstrm2boundarydtsimvector[i] = new double[global.SimIterations];
            }
            strm2meanvelocity = new double[global.HENNodes]; // m/s
            strm2dynviscosity = new double[global.HENNodes];
            strm2reanoldsnumber = new double[global.HENNodes];
            strm2frictionfactor = new double[global.HENNodes];
            strm2frictionforce = new double[global.HENNodes];
            strm2frictionenergylosspsecond = new double[global.HENNodes];

            dnstrm2segmentdt = new double[global.HENSegments];
            dUstrm2dt = new double[global.HENSegments];


        }

        private void calcCpm(int i) //I think this method will not be needed anymore.
        {
            Cpm[i] = 128.1 * Math.Log(Tmave[i]) - 264.11;  //Equation is from Excel sheet.
        }

        public void ddt(int simi)   //Differential equations
        //At this point it is assumed that the flow rates of all gas and liquid phases 
        //are equal to each other at a particular boundary.
        {
            //Equations for GAS flow side
            dnstrm1boundarydt[global.HENNodes - 1] =
                utilities.flowsqrt((inflow[0].mat.P.v - strm1segments[global.HENSegments - 1].P.v) / global.HEStrm1DeltaPK[global.HENNodes - 1],
                dnstrm1boundarydt[global.HENNodes - 1]);
            for (int j = 0; j < inflow[0].mat.composition.Count; j++)
            {
                dnstrm1compboundarydt[global.HENNodes - 1, j] = dnstrm1boundarydt[global.HENNodes - 1] * inflow[0].mat.z[j];
            }
            inflow[0].molarflow.v = dnstrm1boundarydt[global.HENNodes - 1] * global.NStrm2Coils;
            for (int i = global.SteamGeneratorNNodes - 2; i > 0; i--)
            {
                dnstrm1boundarydt[i] = utilities.flowsqrt((strm1segments[i].P.v - strm1segments[i - 1].P.v) / global.HEStrm1DeltaPK[i],
                    dnstrm1boundarydt[i]);
                for (int j = 0; j < strm1segments[i].composition.Count; j++)
                {
                    dnstrm1compboundarydt[i, j] = dnstrm1boundarydt[i] * strm1segments[i].z[j];
                }
                dnstrm1boundarydtsimvector[i][simi] = dnstrm1boundarydt[i];
            }
            dnstrm1boundarydt[0] = dnstrm1boundarydt[1];
            //outflow[0].molarflow = Wg[0] * global.NSteamCoils;

            for (int i = global.HENSegments - 1; i >= 0; i--)
            {
                dnstrm1segmentdt[i] = dnstrm1boundarydt[i + 1] - dnstrm1boundarydt[i];
                for (int j = 0; j < strm1segments[i].composition.Count; j++)
                {
                    dnstrm1compsegmentdt[i, j] = dnstrm1compboundarydt[i + 1, j] - dnstrm1compboundarydt[i, j];
                }
                strm1meanvelocity[i] = dnstrm1boundarydt[i] * strm1segments[i].vmolar.v / global.HEAStrm1; // m/s
                //calcdynviscosity(i);
                //waterreanoldsnumber[i] = watersegments[i].density * watermeanvelocity[i] * global.TubeDiameter /
                //    dynviscosity[i];
                strm1frictionfactor[i] = 20;
                strm1frictionforce[i] = 0.5 * strm1frictionfactor[i] * strm1segments[i].density.v * Math.Pow(strm1meanvelocity[i], 2) *
                    global.HEEffGasTubeCircInside * global.HEAveLengthPerSegment;
                strm1frictionenergylosspsecond[i] = global.HEStrm1AddFriction * strm1frictionforce[i] * strm1meanvelocity[i];
            }
            dUstrm1dt[global.HENSegments - 1] = dnstrm1boundarydt[global.HENNodes - 1] * inflow[0].mat.umolar.v -
                dnstrm1boundarydt[global.HENNodes - 2] * strm1segments[global.HENSegments - 1].umolar.v -
                Qstrm1m[global.HENSegments - 1] - strm1frictionenergylosspsecond[global.HENSegments - 1];
            for (int i = global.HENSegments - 2; i >= 0; i--)
            {
                dUstrm1dt[i] = dnstrm1boundarydt[i + 1] * strm1segments[i + 1].umolar.v - dnstrm1boundarydt[i] * strm1segments[i].umolar.v - Qstrm1m[i] -
                    strm1frictionenergylosspsecond[i];
            }

            //Equations for heat transfer
            for (int i = 0; i < global.HENSegments; i++)
            {
                //This line can 
                //be moved to Global.
                Qstrm1m[i] = global.HEHeatTransferArea/global.HENSegments * (strm1segments[i].T.v - Tmave[i]) /
                    (global.HEKgm[i] * Math.Pow(dnstrm1boundarydt[i + 1] * strm1segments[0].composition[0].m.molarmass,-global.HECgi) +
                    global.HERi * 0.5);
                Qstrm1msimvector[i][simi] = Qstrm1m[i];
                Qmstrm2[i] = global.HEHeatTransferArea / global.HENSegments * (Tmave[i] - strm2segments[i].T.v) /
                    (global.HEKms[i] * Math.Pow(dnstrm2boundarydt[i] * strm2segments[0].composition[0].m.molarmass, -global.HECsi) +
                    global.HERi * 0.5);
                Qmstrm2simvector[i][simi] = Qmstrm2[i];

                calcCpm(i);
                dTmavedt[i] = (Qstrm1m[i] - Qmstrm2[i]) / (global.HEM * Cpm[i] * global.HEAveLengthPerTube / global.HENSegments);
            }

            //Equations for strm2 flow side
            dnstrm2boundarydt[0] = utilities.flowsqrt((inflow[1].mat.P.v - strm2segments[0].P.v) / global.HEStrm2DeltaPK[0], dnstrm2boundarydt[0]); //THERE IS A PROBLEM WITH THIS EQ.
            //dnstrm2boundarydt[0] = global.MolFlowStrm2T0;
            dnstrm2boundarydtsimvector[0][simi] = dnstrm2boundarydt[0];
            for (int i = 1; i < global.HENNodes - 1; i++)
            {
                //Ws[i] = utilities.flowsqrt((watersegments[i - 1].P - watersegments[i].P) / global.SGWaterDeltaPK, Ws[i]); //This equation will later be changed to include a hydraulic equation.
                //dnstrm2boundarydt[i] = dnstrm2boundarydt[i - 1];
                dnstrm2boundarydt[i] = utilities.flowsqrt((strm2segments[i - 1].P.v - strm2segments[i].P.v) / global.HEStrm2DeltaPK[i], dnstrm2boundarydt[i]); //THIS EQ has a problem.
                dnstrm2boundarydtsimvector[i][simi] = dnstrm2boundarydt[i];
            }
            dnstrm2boundarydt[global.HENNodes - 1] = dnstrm2boundarydt[global.HENNodes - 2];

            for (int i = 0; i < global.HENSegments; i++)
            {
                dnstrm2segmentdt[i] = dnstrm2boundarydt[i] - dnstrm2boundarydt[i + 1];
                strm2meanvelocity[i] = dnstrm2boundarydt[i] * strm2segments[i].vmolar.v / global.HEAStrm2; // m/s
                calcdynviscosity(i);
                strm2reanoldsnumber[i] = strm2segments[i].density.v * strm2meanvelocity[i] * global.HEInsideDiameterTube /
                    strm2dynviscosity[i];
                strm2frictionfactor[i] = (strm2meanvelocity[i] > 0) ? 16 / strm2reanoldsnumber[i] : 0;
                strm2frictionforce[i] = 0.5 * strm2frictionfactor[i] * strm2segments[i].density.v * Math.Pow(strm2meanvelocity[i], 2) *
                    global.HETubeCircInside * global.HEAveLengthPerSegment;
                strm2frictionenergylosspsecond[i] = global.HEStrm2AddFriction * strm2meanvelocity[i] *strm2frictionforce[i]; 
            }
            dUstrm2dt[0] = dnstrm2boundarydt[0] * inflow[1].mat.umolar.v - dnstrm2boundarydt[1] * strm2segments[0].umolar.v + Qmstrm2[0] - 
                    strm2frictionenergylosspsecond[0]; //THIS WILL NEED TO BE ADDED BACK LATER, TAKEN OUT HERE FOR DEBUGGING AS U IN SEGMENT 0 KEPT DROPPING.
            for (int i = 1; i < global.HENSegments; i++)
            {
                dUstrm2dt[i] = dnstrm2boundarydt[i] * strm2segments[i - 1].umolar.v - dnstrm2boundarydt[i + 1] * strm2segments[i].umolar.v + Qmstrm2[i] -
                    strm2frictionenergylosspsecond[i];
            }
        }

        public override void update(int simi, bool historise)
        {
            ddt(simi);
            inflow[0].update(simi, false);
            for (int i = global.HENSegments - 1; i >= 0; i--)
            {
                strm1segments[i].n.v += dnstrm1segmentdt[i] * global.SampleT;
                for (int j = 0; j < strm1segments[i].composition.Count; j++)
                {
                    strm1segments[i].composition[j].n += dnstrm1compsegmentdt[i, j] * global.SampleT;
                }
                strm1segments[i].U.v += dUstrm1dt[i] * global.SampleT;
                strm1segments[i].uvflash();
                strm1segments[i].update(simi, historise);
            }
            outflow[0].mat = strm1segments[0]; //This pointer assignment might not be needed each iteration.

            for (int i = 0; i < global.HENSegments; i++)
            {
                Tmave[i] += dTmavedt[i] * global.SampleT;
                Tmavesimvector[i][simi] = Tmave[i];
            }
            for (int i = 0; i < global.HENSegments; i++)
            {
                strm2segments[i].n.v += dnstrm2segmentdt[i] * global.SampleT;
                strm2segments[i].U.v += dUstrm2dt[i] * global.SampleT;
                strm2segments[i].uvflash();
                strm2segments[i].update(simi, historise);

            }

        }

        private void calcdynviscosity(int i) //Dynamic Viscosity in watersegment[i]
        {
            strm2dynviscosity[i] = global.DynViscA * Math.Exp(global.DynViscB * strm2segments[i].T.v);
        }

        public override void showtrenddetail(simulation asim, List<Form> detailtrendslist)
        {
            detailtrendslist.Add(new heatexchangerdetail(this, asim));
            detailtrendslist[detailtrendslist.Count - 1].Show();
        }

        //public override void updatetrenddetail(simulation asim)
        //{
        //    if (detailtrends != null && detailtrends.Visible) { detailtrends.Invalidate(); }
        //}

        public override bool mouseover(double x, double y)
        {
            return (x >= (location.x - global.SteamGeneratorRadius) && x <= (location.x + global.SteamGeneratorRadius)
                && y >=
                (location.y - 0.5 * global.SteamGeneratorHeight) && y <= (location.y + 0.5 * global.SteamGeneratorHeight));
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
        }

        public override void setproperties(simulation asim)
        {
            heatexchangerproperties distcolmnprop = new heatexchangerproperties(this, asim);
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

            //Draw inpoint
            base.draw(G);
        }

    }
}
