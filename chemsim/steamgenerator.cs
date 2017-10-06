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
    public class steamgenerator : unitop
    {
        public List<material> gassegments; //Gas inventory.
        public List<material> watersegments; //Water/steam inventory.

        //Variables for gas equations
        public double[] Wg; //moles/s.  Gas mass flow at the region boundaries.
        public double[,] Wgcomp; //moles/s.  Gas mass flow at the region boundaries, per component.  
        public double[][] Wgsimvector;
        public double[] gasmeanvelocity; // m/s
        public double[] gasdynviscosity; //Pa s
        public double[] gasreanoldsnumber;
        public double[] gasfrictionfactor;
        public double[] gasfrictionforce; //Newton
        public double[] gasfrictionenergylosspsecond; //Joule

        public double[] dngasdt; //ddt of moles in each gas segment.
        public double[,] dngascompdt; //ddt of moles in each gas segment per component.
        public double[] dUgasdt; //ddt of internal energy in each gas segment.

        //Declaration of variables for METAL HEAT equation

        public double[] Qgm; //Joule/second (W); Heat transfer per region from the gas to the metal.  This var has to be in the 
        //flash calc for the gas as well.
        public double[][] Qgmsimvector;

        public double[] Cpm; //J/(kg*K)  Specific heat capacity of the metal of the tube per region.  
        //It will be a function of the region metal temperature.
        public double[][] Cpmsimvector;

        public double[] Tmave; //Kelvin; metal temperature per region.
        public double[] Tmavetemp; //WILL PROBABLY NOT NEED THIS VAR IN THE FUTURE. temp var for integration.
        public double[] dTmavedt;
        public double[][] dTmavedtk;
        public double[][] Tmavesimvector;

        public double[] Qms; //Joule/second (W); heat transfer between metal and steam per region.
        public double[][] Qmssimvector;

        //HEAT EXCHANGE equation
        public double[] heattransferarea; //Heat transfer areas between the different boundary areas.

        //Steam flow
        public double[] Ws; //moles/s  Steam mass flow at the region border.
        public double[][] Wssimvector;
        public double[] watermeanvelocity; // m/s
        public double[] dynviscosity; //Pa s
        public double[] waterreanoldsnumber;
        public double[] waterfrictionfactor;
        public double[] waterfrictionforce; //Newton
        public double[] waterfrictionenergylosspsecond; //Joule

        public double[] dnwaterdt; //ddt of moles in each water segment.
        public double[] dUwaterdt; //ddt of internal energy in each water segment.
        //MOMENTUM equation



        public steamgenerator(int anr, double ax, double ay)
            : base(anr, ax, ay, global.SteamGeneratorNIn, global.SteamGeneratorNOut)
        {
            //Initialising the input streams
            //inflow[0] = new stream(0, ax, ay, ax, ay);   //later this steam will have to be supplied by the user.
            //inflow[0].mat.init("Helium", global.TGasT0[2], global.SteamGeneratorGasSegmentVolume, global.SGPGasInlet, 0);
            //inflow[0].molarflow.v = global.WgasT0 * global.NSteamCoils / inflow[0].mat.composition[0].m.molarmass;
            //inflow[1] = new stream(1, ax, ay, ax, ay);
            //inflow[1].mat.init("Water", global.TWaterT0[0], global.SteamGeneratorWaterSegmentVolume, global.SGPWaterInlet, 0);
            //outflow[0] = new stream(0, ax, ay, ax, ay);   //later this steam will have to be supplied by the user.
            //outflow[0].mat.init("Helium", global.TGasT0[0], global.SteamGeneratorGasSegmentVolume, global.SGPGasOutlet, 0);
            //outflow[0].molarflow.v = global.WgasT0 * global.NSteamCoils / inflow[0].mat.composition[0].m.molarmass;
            //outflow[1] = new stream(1, ax, ay, ax, ay);
            //outflow[1].mat.init("Water", global.TWaterT0[0], global.SteamGeneratorWaterSegmentVolume, global.SGPWaterOutlet, 0);

            ////Material class initialisation for the steam lines and the gas lines.
            //gassegments = new List<material>();
            //watersegments = new List<material>();
            //for (int i = 0; i < global.SteamGeneratorNSegments; i++)
            //{
            //    gassegments.Add(new material("Helium", global.TGasT0[i], global.SteamGeneratorGasSegmentVolume,
            //        global.PGasT0[i], 0));
            //    watersegments.Add(new material("Water", global.TWaterT0[i], global.SteamGeneratorWaterSegmentVolume,
            //        global.PWaterT0[i], 0));
            //}

            //Initialise variables for gas molar flow variables
            Wg = new double[global.SteamGeneratorNNodes];
            Wgcomp = new double[global.SteamGeneratorNNodes, gassegments[0].composition.Count];
            Wgsimvector = new double[global.SteamGeneratorNNodes][];
            for (int i = 0; i < global.SteamGeneratorNNodes; i++)
            {
                Wg[i] = global.WgT0[i] / gassegments[0].composition[0].m.molarmass; //initial molar flow at the boundary moles/s.
                for (int j = 0; j < gassegments[0].composition.Count; j++) //These references to gassegment[0] will need to change to become a lot more dynamic later on.
                {
                    material mat = (i == global.SteamGeneratorNNodes - 1) ? inflow[0].mat : gassegments[i];
                    Wgcomp[i, j] = Wg[i] * mat.z[j];
                }
                Wgsimvector[i] = new double[global.SimIterations];
            }
            gasmeanvelocity = new double[global.SteamGeneratorNNodes]; // m/s
            gasdynviscosity = new double[global.SteamGeneratorNNodes];
            gasreanoldsnumber = new double[global.SteamGeneratorNNodes];
            gasfrictionfactor = new double[global.SteamGeneratorNNodes];
            gasfrictionforce = new double[global.SteamGeneratorNNodes];
            gasfrictionenergylosspsecond = new double[global.SteamGeneratorNNodes];

            dngasdt = new double[global.SteamGeneratorNSegments];
            dngascompdt = new double[global.SteamGeneratorNSegments, inflow[0].mat.composition.Count];
            dUgasdt = new double[global.SteamGeneratorNSegments];

            Qgm = new double[global.SteamGeneratorNSegments];
            Qgmsimvector = new double[global.SteamGeneratorNSegments][];
            for (int i = 0; i < global.SteamGeneratorNSegments; i++)
            {
                Qgm[i] = global.ThermalPower / global.NSteamCoils / global.SteamGeneratorNSegments; // global.QgmT0[i];
                Qgmsimvector[i] = new double[global.SimIterations];
            }

            //Initialise variables for the METAL HEAT EQUATION
            Tmave = new double[global.SteamGeneratorNSegments];
            Tmavetemp = new double[global.SteamGeneratorNSegments];
            dTmavedt = new double[global.SteamGeneratorNSegments];
            dTmavedtk = new double[global.SteamGeneratorNSegments][];
            Tmavesimvector = new double[global.SteamGeneratorNSegments][];
            for (int i = 0; i < global.SteamGeneratorNSegments; i++)
            {
                Tmave[i] = 0.5 * (global.TGasT0[i] + global.TWaterT0[i]);
                Tmavetemp[i] = Tmave[i];
                dTmavedt[i] = 0;
                dTmavedtk[i] = new double[global.RungaKuta];
                Tmavesimvector[i] = new double[global.SimIterations];
            }

            Cpm = new double[global.SteamGeneratorNSegments];
            Cpmsimvector = new double[global.SteamGeneratorNSegments][];
            for (int i = 0; i < global.SteamGeneratorNSegments; i++)
            {
                calcCpm(i);
                Cpmsimvector[i] = new double[global.SimIterations];
            }

            Qms = new double[global.SteamGeneratorNSegments];
            Qmssimvector = new double[global.SteamGeneratorNSegments][];
            for (int i = 0; i < global.SteamGeneratorNSegments; i++)
            {
                Qms[i] = global.ThermalPower / global.NSteamCoils / global.SteamGeneratorNSegments; // global.QmsT0[i];
                Qmssimvector[i] = new double[global.SimIterations];
            }

            //HEAT TRANSFER equation
            heattransferarea = new double[global.SteamGeneratorNSegments];

            //STEAM FLOW
            Ws = new double[global.SteamGeneratorNNodes];
            Wssimvector = new double[global.SteamGeneratorNNodes][];
            for (int i = 0; i < global.SteamGeneratorNNodes; i++)
            {
                Ws[i] = global.MolFlowWaterT0; //convert from mass to molar flow. Simple for ONE
                //component.
                Wssimvector[i] = new double[global.SimIterations];
            }
            watermeanvelocity = new double[global.SteamGeneratorNNodes]; // m/s
            dynviscosity = new double[global.SteamGeneratorNNodes];
            waterreanoldsnumber = new double[global.SteamGeneratorNNodes];
            waterfrictionfactor = new double[global.SteamGeneratorNNodes];
            waterfrictionforce = new double[global.SteamGeneratorNNodes];
            waterfrictionenergylosspsecond = new double[global.SteamGeneratorNNodes];

            dnwaterdt = new double[global.SteamGeneratorNSegments];
            dUwaterdt = new double[global.SteamGeneratorNSegments];
        }

        private void calcCpm(int i)
        {
            Cpm[i] = 128.1 * Math.Log(Tmave[i]) - 264.11;  //Equation is from Excel sheet.
        }

        public void ddt(int simi)   //Differential equations
        //At this point it is assumed that the flow rates of all gas and liquid phases 
        //are equal to each other at a particular boundary.
        {
            //Equations for GAS flow side
            Wg[global.SteamGeneratorNNodes - 1] =
                utilities.flowsqrt((inflow[0].mat.P.v - gassegments[global.SteamGeneratorNSegments - 1].P.v) / global.SGGasDeltaPK,
                Wg[global.SteamGeneratorNNodes - 1]);
            for (int j = 0; j < inflow[0].mat.composition.Count; j++)
            {
                Wgcomp[global.SteamGeneratorNNodes - 1, j] = Wg[global.SteamGeneratorNNodes - 1] * inflow[0].mat.z[j];
            }
            inflow[0].molarflow.v = Wg[global.SteamGeneratorNNodes - 1] * global.NSteamCoils;
            for (int i = global.SteamGeneratorNNodes - 2; i > 0; i--)
            {
                Wg[i] = utilities.flowsqrt((gassegments[i].P.v - gassegments[i - 1].P.v) / global.SGGasDeltaPK,
                    Wg[i]);
                for (int j = 0; j < gassegments[i].composition.Count; j++)
                {
                    Wgcomp[i, j] = Wg[i] * gassegments[i].z[j];
                }
                Wgsimvector[i][simi] = Wg[i];
            }
            Wg[0] = Wg[1];
            //outflow[0].molarflow = Wg[0] * global.NSteamCoils;

            for (int i = global.SteamGeneratorNSegments - 1; i >= 0; i--)
            {
                dngasdt[i] = Wg[i + 1] - Wg[i];
                for (int j = 0; j < gassegments[i].composition.Count; j++)
                {
                    dngascompdt[i, j] = Wgcomp[i + 1, j] - Wgcomp[i, j];
                }
                gasmeanvelocity[i] = Wg[i] * gassegments[i].vmolar.v / global.Ag; // m/s
                //calcdynviscosity(i);
                //waterreanoldsnumber[i] = watersegments[i].density * watermeanvelocity[i] * global.TubeDiameter /
                //    dynviscosity[i];
                gasfrictionfactor[i] = 20;
                gasfrictionforce[i] = 0.5 * gasfrictionfactor[i] * gassegments[i].density.v * Math.Pow(gasmeanvelocity[i], 2) *
                    global.EffGasTubeCircInside * global.AveLengthPerSegment;
                gasfrictionenergylosspsecond[i] = global.SteamGenGasAddFriction * gasfrictionforce[i] * gasmeanvelocity[i];
            }
            dUgasdt[global.SteamGeneratorNSegments - 1] = Wg[global.SteamGeneratorNNodes - 1] * inflow[0].mat.umolar.v -
                Wg[global.SteamGeneratorNNodes - 2] * gassegments[global.SteamGeneratorNSegments - 1].umolar.v -
                Qgm[global.SteamGeneratorNSegments - 1] - gasfrictionenergylosspsecond[global.SteamGeneratorNSegments - 1];
            for (int i = global.SteamGeneratorNSegments - 2; i >= 0; i--)
            {
                dUgasdt[i] = Wg[i + 1] * gassegments[i + 1].umolar.v - Wg[i] * gassegments[i].umolar.v - Qgm[i] -
                    gasfrictionenergylosspsecond[i];
            }

            //Equations for heat transfer
            for (int i = 0; i < global.SteamGeneratorNSegments; i++)
            {
                //This line can 
                //be moved to Global.
                Qgm[i] = global.HeatTransferArea * (gassegments[i].T.v - Tmave[i]) /
                    (global.Kgm[i] * Math.Exp(-Wg[i + 1] * gassegments[0].composition[0].m.molarmass * global.Cgi) +
                    global.Ri * 0.5);
                Qgmsimvector[i][simi] = Qgm[i];
                Qms[i] = global.HeatTransferArea * (Tmave[i] - watersegments[i].T.v) /
                    (global.Kms[i] * Math.Exp(-Ws[i] * watersegments[0].composition[0].m.molarmass * global.Csi) +
                    global.Ri * 0.5);
                Qmssimvector[i][simi] = Qms[i];

                calcCpm(i);
                dTmavedt[i] = (Qgm[i] - Qms[i]) / (global.M * Cpm[i] * global.AveLengthPerTube / global.SteamGeneratorNSegments);
            }

            //Equations for WATER flow side
            //Ws[0] = utilities.flowsqrt((inflow[1].mat.P - watersegments[0].P) / global.SGWaterDeltaPK, Ws[0]);
            Ws[0] = global.MolFlowWaterT0;
            for (int i = 1; i < global.SteamGeneratorNNodes - 1; i++)
            {
                //Ws[i] = utilities.flowsqrt((watersegments[i - 1].P - watersegments[i].P) / global.SGWaterDeltaPK, Ws[i]); //This equation will later be changed to include a hydraulic equation.
                Ws[i] = Ws[i - 1];
                Wssimvector[i][simi] = Ws[i];
            }
            Ws[global.SteamGeneratorNNodes - 1] = Ws[global.SteamGeneratorNNodes - 2];

            for (int i = 0; i < global.SteamGeneratorNSegments; i++)
            {
                dnwaterdt[i] = Ws[i] - Ws[i + 1];
                watermeanvelocity[i] = Ws[i] * watersegments[i].vmolar.v / global.As; // m/s
                calcdynviscosity(i);
                waterreanoldsnumber[i] = watersegments[i].density.v * watermeanvelocity[i] * global.TubeDiameter /
                    dynviscosity[i];
                waterfrictionfactor[i] = (watermeanvelocity[i] > 0) ? 16 / waterreanoldsnumber[i] : 0;
                waterfrictionforce[i] = 0.5 * waterfrictionfactor[i] * watersegments[i].density.v * Math.Pow(watermeanvelocity[i], 2) *
                    global.TubeCircInside * global.AveLengthPerSegment;
                waterfrictionenergylosspsecond[i] = global.SteamGenAddFriction * waterfrictionforce[i] * watermeanvelocity[i];
            }
            dUwaterdt[0] = Ws[0] * inflow[1].mat.umolar.v - Ws[1] * watersegments[0].umolar.v + Qms[0] -
                    waterfrictionenergylosspsecond[0];
            for (int i = 1; i < global.SteamGeneratorNSegments; i++)
            {
                dUwaterdt[i] = Ws[i] * watersegments[i - 1].umolar.v - Ws[i + 1] * watersegments[i].umolar.v + Qms[i] -
                    waterfrictionenergylosspsecond[i];
            }
        }

        public override void update(int simi, bool historise)
        {
            ddt(simi);
            inflow[0].update(simi, false);
            for (int i = global.SteamGeneratorNSegments - 1; i >= 0; i--)
            {
                gassegments[i].n.v += dngasdt[i] * global.SampleT;
                for (int j = 0; j < gassegments[i].composition.Count; j++)
                {
                    gassegments[i].composition[j].n += dngascompdt[i,j] * global.SampleT;
                }
                gassegments[i].U.v += dUgasdt[i] * global.SampleT;
                gassegments[i].uvflash();
                gassegments[i].update(simi, historise);
            }
            outflow[0].mat = gassegments[0]; //This pointer assignment might not be needed each iteration.

            for (int i = 0; i < global.SteamGeneratorNSegments; i++)
            {
                Tmave[i] += dTmavedt[i] * global.SampleT;
                Tmavesimvector[i][simi] = Tmave[i];
            }
            for (int i = 0; i < global.SteamGeneratorNSegments; i++)
            {
                watersegments[i].n.v += dnwaterdt[i] * global.SampleT;
                watersegments[i].U.v += dUwaterdt[i] * global.SampleT;
                watersegments[i].uvflash();
                watersegments[i].update(simi, historise);

            }
            //Ws[0] = utilities.flowsqrt((inflow[1].mat.P - watersegments[0].P) / global.SteamGenDeltaPK);
            //for (int i = 1; i < global.SteamGeneratorNNodes - 1; i++)
            //{
            //    Ws[i] = utilities.flowsqrt((watersegments[i - 1].P - watersegments[i].P) / global.SteamGenDeltaPK);
            //}
            //Ws[global.SteamGeneratorNNodes - 1] =
            //    utilities.flowsqrt((watersegments[global.SteamGeneratorNSegments - 1].P - outflow[1].mat.P) / global.SteamGenDeltaPK);
        }

        private void calcdynviscosity(int i) //Dynamic Viscosity in watersegment[i]
        {
            dynviscosity[i] = global.DynViscA * Math.Exp(global.DynViscB * watersegments[i].T.v);
        }

        //public override void showtrenddetail(simulation asim)
        //{
        //    detailtrends = new steamgeneratordetail(this, asim);
        //    detailtrends.Show();
        //}

        public override void showtrenddetail(simulation asim, List<Form> detailtrendslist)
        {
            detailtrendslist.Add(new steamgeneratordetail(this, asim));
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
            for (int i = 0; i < global.SteamGeneratorNIn; i++)
            {
                inpoint[i].x = location.x - global.SteamGeneratorRadius - global.InOutPointWidth;
                inpoint[i].y = location.y - global.SteamGeneratorHeight * 0.5 + global.SteamGeneratorInPointsFraction[i] *
                    global.SteamGeneratorHeight;
            }

            for (int i = 0; i < global.SteamGeneratorNOut; i++)
            {
                outpoint[i].x = location.x + global.SteamGeneratorRadius + global.InOutPointWidth;
                outpoint[i].y = location.y - global.SteamGeneratorHeight * 0.5 + global.SteamGeneratorOutPointsFraction[i] *
                    global.SteamGeneratorHeight;
            }
        }

        public override void setproperties(simulation asim)
        {
            steamgeneratorproperties distcolmnprop = new steamgeneratorproperties(this, asim);
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
            {new Point(global.OriginX + Convert.ToInt32(global.GScale*(location.x - global.SteamGeneratorRadius)), 
                    global.OriginY + Convert.ToInt32(global.GScale*(location.y + 0.5*global.SteamGeneratorHeight))), 
            new Point(global.OriginX + Convert.ToInt32(global.GScale*(location.x - global.SteamGeneratorRadius)), 
                    global.OriginY + Convert.ToInt32(global.GScale*(location.y - 0.5*global.SteamGeneratorHeight))), 
            new Point(global.OriginX + Convert.ToInt32(global.GScale*(location.x + global.SteamGeneratorRadius)), 
                    global.OriginY + Convert.ToInt32(global.GScale*(location.y - 0.5*global.SteamGeneratorHeight))), 
            new Point(global.OriginX + Convert.ToInt32(global.GScale*(location.x + global.SteamGeneratorRadius)),
                    global.OriginY + Convert.ToInt32(global.GScale*(location.y + 0.5*global.SteamGeneratorHeight)))};
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
