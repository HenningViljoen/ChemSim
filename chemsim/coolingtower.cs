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
    public class coolingtower : unitop //This class will endevour to take the latest research on cooling tower modelling into 
                                       //account and build on that.
    {
        public int nrstages;
        public double h; //step size for integration

        public controlvar on; //0 for off, 1 for on.
        public valve linkedsprayvalve;  //Every coolingtower will now have feeding spray valve that is linked to it.

        public controlvar fanspeed; //revolusions per second.

        public controlvar fanpower; //W
        public double newfanpower; //W; The future steady state fan power.
        public double fanpowerstatespacex1;
        public double fanpowerstatespacex2;
        public double ddtfanpowerstatespacex1;
        public double ddtfanpowerstatespacex2;

        public double deltapressure; //Pa.  The pressure drop over the fan.

        public controlvar watervolumefraction; //fraction of total volume in tower occupied by water.  This is from Marques article -> 0.01.
        public double CTAirVolumeFraction;
        public double CTTotalWaterVolume;
        public double CTNrDroplets;
        public double CTDefaultSegmentContactArea; //m^2  interfacial contact area total per segment.
        public double a; //m^2/m^3 CTDefaultContactAreaPerVolume. interfacial contact area of water droplets per unit volume of the tower.


        public double tuningfactor; //dimensionless.  This factor will be used to tune the simultion in terms chilling in the cooling tower.
        public double outflow0temperatureoutnew; //Kelvin
        public double doutflow0temperatureoutnewdt; //Kelvin/sec

        public double U; // W/(m^2*K) ; Overall heat exchanger coefficient for heat exchanger.  Not sure if this is going to be used now.
        public double A; // m^2 ; The contact area for the fluids with each other in the heat exchanger.  Not sure if this is going to be used now.  
        //public double K; //constants used in calculating the new output temperatures of the HX.


        public controlvar masstransfercoefair; //kg/(s*m^2)  
        public controlvar heattransfercoefwater; //W/(m^2*K)
        public controlvar heattransfercoefair; //W/(m^2*K)
        public double wetbulbtemperature; //Kelvin

        public double massfluxwater; //kg/(m^2s*) : Volume flow of water in the CT.
        public double massfluxair; //kg/(m^2s*) : Volume flow of air in the CT.

        public List<double> segmentvolume; //m^3 total volume per water segment : includes packing, water, and air in each segment.
        public List<double> segmentvolumewater; //m^3 volume per water segment
        public List<double> segmentvolumeair; //m^3 volume per air segment

        public List<material> watersegment;
        public List<material> airsegment;
        public List<double> watersegmentflowout; //kg/s. The mass flow out of the particular water segment.
        public List<double> airsegmentdryairflowout; //The mass flow out of dry air of the particular air segment.

        public List<double> interfacetemperature; //Kelvin.  Temperature at the interface for the segment.
        public List<double[]> interfacetemperaturesimvector;
        public List<double> interfaceabshumidity; //kg mass water / kg dry air
        public List<double[]> interfaceabshumiditysimvector;
        public List<double> airabshumidity; //kg mass water / kg dry air
        public List<double[]> airabshumiditysimvector;
        public List<double> massevap; //kg/s.  Mass evaporated per second, per segment.
        public List<double> segmenttransferarea; //m2.  The surface area used to calculate the vapour mass transfer flow.
        public List<double> segmentcontactarea; //m^2  interfacial contact area of water droplets for each segment of the tower.

        public List<double[]> dTwaterdt; //K/s Let the first column be the normal derivative, and then column 1 and on will be the different k values in
                                        // in RK4.
        public List<double[]> dTairdt; //K/s
        public List<double[]> dhumidityairdt; //K/s

        //Properties for stream 0
        public double strm0valveop; //Fraction.  Spray nozzle modelled as a valve.
        public double strm0cv; //The upstream pressure will be calculated by modelling the spray nozzle as a valve.
        public double strm0temptau;
        public double strm0flowtau;


        //Properties for stream 1
        public double strm1flowcoefficient;
        public double strm1temptau;
        public double strm1flowtau;

        //Variables for stream 0 equations .  In the case of the cooling tower, this will be the hot water stream.
        public double strm0massflownew; //kg/s
        public double dstrm0massflowdt; //kg/s/s
        public double strm0pressureinnew; //Pa
        public double dstrm0pressureindt; //Pa/s
        public double strm0temperatureoutnew; //Kelvin
        public double dstrm0temperatureoutnewdt; //Kelvin

        //Stream 1 flow
        public double strm1massflownew; //kg/s
        public double dstrm1massflowdt; //kg/s
        public double strm1pressureoutnew; //Pa
        public double dstrm1pressureoutdt; //Pa/s
        public double strm1temperatureoutnew; //Kelvin
        public double dstrm1temperatureoutnewdt; //Kelvin

        public coolingtower(int anr, double ax, double ay)
            : base(anr, ax, ay, global.CTHESNIn, global.CTHESNOut)
        {
            initcoolingtower();
            update(0, true);
        }

        public coolingtower(baseclass baseclasscopyfrom)
            : base(0, 0, 0, global.CTHESNIn, global.CTHESNOut)
        {
            initcoolingtower();
            copyfrom(baseclasscopyfrom);
        }

        public void initcoolingtower()
        {
            objecttype = objecttypes.CoolingTower;
            name = nr.ToString() + " " + objecttype.ToString();

            controlpropthisclass.Clear();
            controlpropthisclass.AddRange(new List<string>(new string[] { "on", "fanspeed", "fanpower", "ctwatervolumefraction", "masstransfercoefair", 
                "heattransfercoefwater", "heattransfercoefair" }));

            nrcontrolpropinherited = controlproperties.Count;
            controlproperties.AddRange(controlpropthisclass);

            nrstages = global.CTDefaultNrStages;
            h = global.SampleT;

            on = new controlvar(1, true);
            fanspeed = new controlvar(global.CTFanSpeedT0);

            fanpower = new controlvar(0.0);
            newfanpower = 0.0; //W; The future steady state fan power.
            fanpowerstatespacex1 = 0.0;
            fanpowerstatespacex2 = 0.0;
            ddtfanpowerstatespacex1 = 0.0;
            ddtfanpowerstatespacex2 = 0.0;

            deltapressure = global.CTDefaultFanDP;

            watervolumefraction = new controlvar(global.CTWaterVolumeFraction); //fraction of total volume in tower occupied by water.  This is from Marques article -> 0.01.
            CTAirVolumeFraction = 1 - watervolumefraction.v - global.CTPackingVolumeFraction;
            CTTotalWaterVolume = global.CTTotalVolume * watervolumefraction.v;
            CTNrDroplets = CTTotalWaterVolume / global.CTDropletVolume;
            CTDefaultSegmentContactArea = CTNrDroplets * global.CTDropletSurfaceArea / nrstages; //m^2  interfacial contact area total per segment.
            a = CTNrDroplets * global.CTDropletSurfaceArea / (global.CTTotalVolume); 
               //m^2/m^3  interfacial contact area of water droplets per unit volume of the tower.

            tuningfactor = global.CTTuningFactor;

            //U = global.HeatExchangerSimpleDefaultU;
            //A = global.HeatExchangerSimpleDefaultA;
            //K = 0;

            masstransfercoefair = new controlvar(global.CTDefaultMassTransferCoefficientAir);
            masstransfercoefair.simvector = new double[global.SimVectorLength];
            heattransfercoefwater = new controlvar(global.CTDefaultHeatTransferCoefficientWater);
            heattransfercoefwater.simvector = new double[global.SimVectorLength];
            heattransfercoefair = new controlvar(global.CTDefaultHeatTransferCoefficientAir);
            heattransfercoefair.simvector = new double[global.SimVectorLength];

            calcwetbulbtemp(); //Kelvin

            massfluxwater = 0; //m3/s : Mass flux flow of water in the CT.
            massfluxair = 0; //m3/s : Mass flux flow of air in the CT.

            segmentvolume = new List<double>(0);
            for (int i = 0; i < nrstages; i++)
            {
                segmentvolume.Add(global.CTWidth * global.CTLength * global.CTHeight / nrstages);
            }

            segmentvolumewater = new List<double>(0);
            for (int i = 0; i < nrstages; i++)
            {
                segmentvolumewater.Add(global.CTWidth * global.CTLength * global.CTHeight * watervolumefraction.v / nrstages); //times 0.5 since half the volume is for water, 
                                                                                                  // and half for air.
            }

            segmentvolumeair = new List<double>(0);
            for (int i = 0; i < nrstages; i++)
            {
                segmentvolumeair.Add(global.CTWidth * global.CTLength * global.CTHeight * CTAirVolumeFraction/nrstages); //times 0.5 since half the volume is for air, 
                // and half for air.
            }

            watersegment = new List<material>(0);
            for (int i = 0; i < nrstages; i++)
            {
                watersegment.Add(new material(global.fluidpackage, global.CTTStrm0Inlet, global.baseprocessclassInitVolume,
                    global.CTPStrm0Inlet, 0)); //should maybe again be initialised with water only.
            }

            airsegment = new List<material>(0);
            for (int i = 0; i < nrstages; i++)
            {
                airsegment.Add(new material(global.fluidpackage, global.CTTStrm1Inlet, segmentvolumeair[i],
                    global.CTPStrm1Inlet, 1)); //Should maybe again be initialised with Air only.
                for (int j = 0; j < airsegment[i].composition.Count; j++)
                {
                    if (j == (int)components.Air) { airsegment[i].composition[j].molefraction = 1; }
                    else { airsegment[i].composition[j].molefraction = 0; }

                }
                airsegment[i].PTfVflash(airsegment[i].T.v, airsegment[i].V.v, airsegment[i].P.v, airsegment[i].f.v);
            }

            watersegmentflowout = new List<double>(0); //kg/s. The mass flow out of the particular water segment.
            for (int i = 0; i < nrstages; i++)
            {
                watersegmentflowout.Add(0.0);
            }

            airsegmentdryairflowout = new List<double>(0); //The mass flow out of the particular air segment.
            for (int i = 0; i < nrstages; i++)
            {
                airsegmentdryairflowout.Add(0.0);
            }

            interfacetemperature = new List<double>(0);
            interfacetemperaturesimvector = new List<double[]>(0);
            for (int i = 0; i < nrstages; i++)
            {
                interfacetemperature.Add(global.CTTInterfaceT0);
                interfacetemperaturesimvector.Add(new double[global.SimVectorLength]);
            }

            interfaceabshumidity = new List<double>(0);
            interfaceabshumiditysimvector = new List<double[]>(0);
            for (int i = 0; i < nrstages; i++) 
            {
                interfaceabshumidity.Add(calcabshumidity(calcwatervapourpressuresaturation(interfacetemperature[i])));
                interfaceabshumiditysimvector.Add(new double[global.SimVectorLength]);
            }

            airabshumidity = new List<double>(0);
            airabshumiditysimvector = new List<double[]>(0);
            for (int i = 0; i < nrstages; i++) 
            { 
                airabshumidity.Add(calcabshumidity(calcwatervapourpressure(airsegment[i].T.v)));
                airabshumiditysimvector.Add(new double[global.SimVectorLength]);
            }

            massevap = new List<double>(0);
            for (int i = 0; i < nrstages; i++) { massevap.Add(0.0); }

            segmenttransferarea = new List<double>(0);
            for (int i = 0; i < nrstages; i++) { segmenttransferarea.Add(global.CTTotalInterfaceArea); }

            segmentcontactarea = new List<double>(0);
            for (int i = 0; i < nrstages; i++) { segmentcontactarea.Add(CTDefaultSegmentContactArea); }
            
            
            dTwaterdt = new List<double[]>(0); //K/s
            for (int i = 0; i < nrstages; i++) { dTwaterdt.Add(new double[global.CTRK4ArraySize]); }

            dTairdt = new List<double[]>(0); //K/s
            for (int i = 0; i < nrstages; i++) { dTairdt.Add(new double[global.CTRK4ArraySize]); }

            dhumidityairdt = new List<double[]>(0); //K/s
            for (int i = 0; i < nrstages; i++) { dhumidityairdt.Add(new double[global.CTRK4ArraySize]); }

            strm0valveop = global.CTStrm0ValveOpeningDefault;
            strm0cv = global.CTStrm0Cv;
            strm0temptau = global.CTStrm0TempTau;
            strm0flowtau = global.CTStrm0FlowTau;

            strm1flowcoefficient = global.CTStrm1FlowCoefficient;
            strm1temptau = global.CTStrm1TempTau;
            strm1flowtau = global.CTStrm1FlowTau;

            strm0massflownew = global.CTMassFlowStrm0T0; //kg/s
            dstrm0massflowdt = 0;
            strm0pressureinnew = global.CTPStrm0Inlet; //Pa
            dstrm0pressureindt = 0; //Pa/s
            strm0temperatureoutnew = global.CTTStrm0Outlet; //Kelvin
            dstrm0temperatureoutnewdt = 0;

            strm1massflownew = global.CTMassFlowStrm1T0; //kg/s
            dstrm1massflowdt = 0;
            strm1pressureoutnew = global.CTPStrm1Outlet; //Pa
            dstrm1pressureoutdt = 0; //Pa/s
            strm1temperatureoutnew = global.CTTStrm1Outlet; //Kelvin
            dstrm1temperatureoutnewdt = 0;
        }

        public override void copyfrom(baseclass baseclasscopyfrom)
        {
            coolingtower coolingtowercopyfrom = (coolingtower)baseclasscopyfrom;

            base.copyfrom(coolingtowercopyfrom);

            on.v = coolingtowercopyfrom.on.v;
            nrstages = coolingtowercopyfrom.nrstages;

            fanspeed.v = coolingtowercopyfrom.fanspeed.v;

            fanpower.v = coolingtowercopyfrom.fanpower.v;
            newfanpower = coolingtowercopyfrom.newfanpower; //W; The future steady state fan power.
            fanpowerstatespacex1 = coolingtowercopyfrom.fanpowerstatespacex1;
            fanpowerstatespacex2 = coolingtowercopyfrom.fanpowerstatespacex2;
            ddtfanpowerstatespacex1 = coolingtowercopyfrom.ddtfanpowerstatespacex1;
            ddtfanpowerstatespacex2 = coolingtowercopyfrom.ddtfanpowerstatespacex2;

            deltapressure = coolingtowercopyfrom.deltapressure;

            tuningfactor = coolingtowercopyfrom.tuningfactor;

            U = coolingtowercopyfrom.U;
            A = coolingtowercopyfrom.A;
            //K = coolingtowercopyfrom.K;

            masstransfercoefair = coolingtowercopyfrom.masstransfercoefair;
            heattransfercoefwater = coolingtowercopyfrom.heattransfercoefwater;
            heattransfercoefair = coolingtowercopyfrom.heattransfercoefair;

            wetbulbtemperature = coolingtowercopyfrom.wetbulbtemperature; //Kelvin

            massfluxwater = coolingtowercopyfrom.massfluxwater; //m3/s : Volume flow of water in the CT
            massfluxair = coolingtowercopyfrom.massfluxair; //m3/s : Volume flow of air in the CT.

            for (int i = 0; i < nrstages; i++) { segmentvolume[i] = coolingtowercopyfrom.segmentvolume[i]; }

            for (int i = 0; i < nrstages; i++) {segmentvolumewater[i] = coolingtowercopyfrom.segmentvolumewater[i];}

            for (int i = 0; i < nrstages; i++) {segmentvolumeair[i] = coolingtowercopyfrom.segmentvolumeair[i]; }

            for (int i = 0; i < nrstages; i++) { watersegment[i].copyfrom(coolingtowercopyfrom.watersegment[i]);  }

            for (int i = 0; i < nrstages; i++) { airsegment[i].copyfrom(coolingtowercopyfrom.airsegment[i]); }

            for (int i = 0; i < nrstages; i++) { watersegmentflowout[i] = coolingtowercopyfrom.watersegmentflowout[i]; }

            for (int i = 0; i < nrstages; i++) { airsegmentdryairflowout[i] = coolingtowercopyfrom.airsegmentdryairflowout[i]; }

            for (int i = 0; i < nrstages; i++) { interfacetemperature[i] = coolingtowercopyfrom.interfacetemperature[i]; }

            for (int i = 0; i < nrstages; i++) { interfaceabshumidity[i] = coolingtowercopyfrom.interfaceabshumidity[i]; }

            for (int i = 0; i < nrstages; i++) { airabshumidity[i] = coolingtowercopyfrom.airabshumidity[i]; }

            for (int i = 0; i < nrstages; i++) { massevap[i] = coolingtowercopyfrom.massevap[i]; }

            for (int i = 0; i < nrstages; i++) { segmenttransferarea[i] = coolingtowercopyfrom.segmenttransferarea[i]; }

            for (int i = 0; i < nrstages; i++) { segmentcontactarea[i] = coolingtowercopyfrom.segmentcontactarea[i]; }

            for (int i = 0; i < nrstages; i++) { dTwaterdt[i] = coolingtowercopyfrom.dTwaterdt[i]; }

            for (int i = 0; i < nrstages; i++) { dTairdt[i] = coolingtowercopyfrom.dTairdt[i]; }

            for (int i = 0; i < nrstages; i++) { dhumidityairdt[i] = coolingtowercopyfrom.dhumidityairdt[i]; }

            outflow0temperatureoutnew = coolingtowercopyfrom.outflow0temperatureoutnew; //Kelvin
            doutflow0temperatureoutnewdt = coolingtowercopyfrom.doutflow0temperatureoutnewdt; //Kelvin/sec

            strm0valveop = coolingtowercopyfrom.strm0valveop;
            strm0cv = coolingtowercopyfrom.strm0cv;
            strm0temptau = coolingtowercopyfrom.strm0temptau;
            strm0flowtau = coolingtowercopyfrom.strm0flowtau;
            strm1flowcoefficient = coolingtowercopyfrom.strm1flowcoefficient;
            strm1temptau = coolingtowercopyfrom.strm1temptau;
            strm1flowtau = coolingtowercopyfrom.strm1flowtau;

            strm0massflownew = coolingtowercopyfrom.strm0massflownew; //kg/s
            dstrm0massflowdt = coolingtowercopyfrom.dstrm0massflowdt; //kg/s/s
            strm0pressureinnew = coolingtowercopyfrom.strm0pressureinnew; //Pa
            dstrm0pressureindt = coolingtowercopyfrom.dstrm0pressureindt; //Pa/s
            strm0temperatureoutnew = coolingtowercopyfrom.strm0temperatureoutnew; //Kelvin
            dstrm0temperatureoutnewdt = coolingtowercopyfrom.dstrm0temperatureoutnewdt; //Kelvin

            //Stream 2 flow
            strm1massflownew = coolingtowercopyfrom.strm1massflownew; //kg/s
            dstrm1massflowdt = coolingtowercopyfrom.dstrm1massflowdt; //kg/s
            strm1pressureoutnew = coolingtowercopyfrom.strm1pressureoutnew; //Pa
            dstrm1pressureoutdt = coolingtowercopyfrom.dstrm1pressureoutdt; //Pa/s
            strm1temperatureoutnew = coolingtowercopyfrom.strm1temperatureoutnew; //Kelvin
            dstrm1temperatureoutnewdt = coolingtowercopyfrom.dstrm1temperatureoutnewdt; //Kelvin
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
                        return fanspeed;
                    case 2:
                        return fanpower;
                    case 3:
                        return watervolumefraction;
                    case 4:
                        return masstransfercoefair;
                    case 5:
                        return heattransfercoefwater;
                    case 6:
                        return heattransfercoefair;

                    default:
                        return null;
                }
            }
            else { return base.selectedproperty(selection); };
        }

        public void calcwetbulbtemp()
        {
            double Ta = global.AmbientTemperature;
            double RH = global.RelativeHumidity;
            wetbulbtemperature = Ta * Math.Atan(0.151977 * Math.Pow(RH + 8.313659, 0.5)) + Math.Atan(Ta + RH) -
                Math.Atan(RH - 1.676331) + 0.00391838 * Math.Pow(RH, 1.5) * Math.Atan(0.023101 * RH) - 4.686035;
            wetbulbtemperature = 290; //TEST
        }

        private double calcwatervapourpressure(double temperature) //From the Antoine equation
        {
            double temp = utilities.kelvin2celcius(temperature);
            return global.ConvertmmHgtoPa * Math.Pow(10, global.AAntoineWater - global.BAntoineWater / (global.CAntoineWater + temp));
        }

        private double calcwatervapourpressurefromrelativehumidty(double relativehumidity, double temperature) 
            //Water vap pressure in Pa, from rel.humidity as a percentage
            //and from the temperature in Kelvin
        {
            double vapourpressureatsaturation = calcwatervapourpressuresaturation(temperature);
            return relativehumidity/100.0 * vapourpressureatsaturation;
        }

        private double calcwatervapourpressuresaturation(double temperature) //From the Buck equation
        {
            double temp = utilities.kelvin2celcius(temperature);
            return global.BuckC1 * Math.Exp((global.BuckC2 - temp / global.BuckC3) * (temp / (global.BuckC4 + temp)));
        }

        private double calcabshumidity(double watervappressure)  //Specific Humidity from Peries 8th edition
        {
            return 0.622 * watervappressure / (global.Ps - watervappressure); //kg/kg
        }

        public void calcairheattransfercoef1() //This is if we want to use the Lewis factor, which we might actually not want to use.
        {
            heattransfercoefair = global.CTLewisFactor * masstransfercoefair * global.CTCpAir / CTDefaultSegmentContactArea;
        }

        public void calcairheattransfercoef2() //This is from McCabe's chapther on humidification processes and operations.
        {
            heattransfercoefair = global.CTLewisFactor * masstransfercoefair * global.CTCpAir;
        }

        public void updatefrompropertydialogue() //When property dialogue is read, some properties need to be updated.
        {
            calcairheattransfercoef2();
            //if (linkedsprayvalve != null) 
            //{ 
            //    linkedsprayvalve.op.v = on.v; 
            //}

        }

        public void ddt(int simi, double scaleslope)   //Differential equations.  scaleslope is for RukgaKutta4
        {
            fanpowerstatespacex1 = fanpower.v;
            ddtfanpowerstatespacex1 = fanpowerstatespacex2;
            //ddtfanpowerstatespacex2 = -9 * fanpowerstatespacex1 - 2 * fanpowerstatespacex2 + 9 * newfanpower;
            ddtfanpowerstatespacex2 = -global.Rotatinga0 * fanpowerstatespacex1 - global.Rotatinga1 * fanpowerstatespacex2 + global.Rotatingb0 * newfanpower;

            dstrm0pressureindt = -1 / strm0flowtau * inflow[0].mat.P.v + 1 / strm0flowtau * strm0pressureinnew;
            dstrm1pressureoutdt = -1 / strm1flowtau * outflow[1].mat.P.v + 1 / strm1flowtau * strm1pressureoutnew;

            double incomingwaterflow;
            double incomingairflow;
            double incomingT;
            double incominghumidity;
            for (int i = 0; i < nrstages; i++) //segments in tower are numbered from zero at the bottom of the tower, to nrstages - 1 at the top.
            {
                

                incomingwaterflow = (i == nrstages - 1) ? inflow[0].massflow.v : watersegmentflowout[i + 1];
                watersegmentflowout[i] = incomingwaterflow - massevap[i];

                incominghumidity = (i == 0) ? calcabshumidity(calcwatervapourpressurefromrelativehumidty(inflow[1].mat.relativehumidity.v,
                    inflow[1].mat.T.v)) :
                    (airabshumidity[i - 1] + scaleslope*dhumidityairdt[i - 1][0]);

                incomingairflow = (i == 0) ? inflow[1].massflow.v/(1 + incominghumidity) : airsegmentdryairflowout[i - 1];
                airsegmentdryairflowout[i] = incomingairflow;
                //airsegmentflowout[i] = incomingairflow + massevap[i]; Since we are only interested in dry air flow, which will not change, in the tower
                                                                        //there is no need for this line anymore.

                incomingT = (i == nrstages - 1) ? inflow[0].mat.T.v : (watersegment[i + 1].T.v + scaleslope*dTwaterdt[i + 1][0]);
                dTwaterdt[i][0] = 1 / (watersegment[i].density.v*segmentvolumewater[i]) *
                    (incomingwaterflow * incomingT - watersegmentflowout[i] * (watersegment[i].T.v + scaleslope * dTwaterdt[i][0])) -
                    heattransfercoefwater.v * segmentcontactarea[i] * ((watersegment[i].T.v + scaleslope * dTwaterdt[i][0]) - interfacetemperature[i]) / 
                    (watersegment[i].totalCp/watersegment[i].massofonemole * watersegment[i].density.v * segmentvolumewater[i]);

                incomingT = (i == 0) ? inflow[1].mat.T.v : (airsegment[i - 1].T.v + scaleslope*dTairdt[i - 1][0]);
                dTairdt[i][0] = 1 / (airsegment[i].density.v*segmentvolumeair[i]) * 
                    (incomingairflow*incomingT - airsegmentdryairflowout[i]*(airsegment[i].T.v + scaleslope*dTairdt[i][0])) -
                    heattransfercoefair.v * segmentcontactarea[i] * ((airsegment[i].T.v + scaleslope * dTairdt[i][0]) - interfacetemperature[i]) / 
                    (airsegment[i].totalCp/airsegment[i].massofonemole * airsegment[i].density.v * segmentvolumeair[i]);

                
                dhumidityairdt[i][0] = (incomingairflow*incominghumidity + massevap[i] - 
                    (airabshumidity[i] + scaleslope*dhumidityairdt[i][0])*airsegmentdryairflowout[i])/(airsegment[i].density.v * segmentvolumeair[i]); 
                    //(Total water mass in, minus total water mass out) / (total mass air in segment)

            }
        }

        public override void update(int simi, bool historise)
        {
            calcairheattransfercoef2(); //Assuming now that the heat transfer coef for air is linked to the mass transfer coeff.

            if (linkedsprayvalve != null)
            {
                //linkedsprayvalve.op.v = (on.v >= 0.5) ? 1:0;
                
            }
            double hybridfanspeed = (on.v < 0.5) ? hybridfanspeed = global.CTFanShutdownSpeed : hybridfanspeed = fanspeed.v;
            inflow[1].massflow.v = global.CTMassFlowStrm1T0 * hybridfanspeed / global.CTFanSpeedT0;  //This line is commented out when CT fitting is done.
            deltapressure = global.CTDefaultFanDP * Math.Pow(hybridfanspeed / global.CTFanSpeedT0, 2);
            newfanpower = global.CTFanPowerT0 * Math.Pow(hybridfanspeed / global.CTFanSpeedT0, 3);

            double strm0pressureinold = strm0pressureinnew;

            strm0pressureinnew = outflow[0].mat.P.v +
                Math.Pow(inflow[0].massflow.v / (strm0cv * (strm0valveop + global.Epsilon)), 2) * (inflow[0].mat.density.v);
            if (strm0pressureinnew / (strm0pressureinold + global.Epsilon) > global.CTPMaxFactorIncreaseperSampleT) //Dynamic protection:
               //if pressure changes too fast the diff. equations cannot solve.
            {
                strm0pressureinnew = strm0pressureinold * global.CTPMaxFactorIncreaseperSampleT;
            }

            strm1pressureoutnew = inflow[1].mat.P.v + deltapressure;
            //strm1pressureoutnew = inflow[1].mat.P.v -
            //    Math.Pow(inflow[1].massflow.v / strm1flowcoefficient, 2) * (inflow[1].mat.density.v + global.Epsilon);

            massfluxwater = inflow[0].massflow.v / (global.CTTotalInterfaceArea * watervolumefraction.v); //The volume flow of water.
            massfluxair = inflow[1].massflow.v / (global.CTTotalInterfaceArea *CTAirVolumeFraction); ; //The volume flow of air.

            //calcairheattransfercoef(); //Update the air heat transfer coeff in case the water mass transfer coeff has changed.           

            for (int i = 0; i < nrstages; i++) //Static equations that can be solved without integration.
            {
                interfacetemperature[i] = -((masstransfercoefair.v * global.DeltaHWater * (interfaceabshumidity[i] - airabshumidity[i]) - 
                    heattransfercoefwater.v*watersegment[i].T.v - heattransfercoefair.v*airsegment[i].T.v)/
                    (heattransfercoefair.v + heattransfercoefwater.v));
                interfaceabshumidity[i] = calcabshumidity(calcwatervapourpressuresaturation(interfacetemperature[i]));

                massevap[i] = masstransfercoefair.v * segmentcontactarea[i] * (interfaceabshumidity[i] - airabshumidity[i]);

                

                //watersegment[i].Cp[0] = watersegment[i].composition[0].m.calcCp(watersegment[i].T.v);
                //airsegment[i].Cp[0] = airsegment[i].composition[0].m.calcCp(airsegment[i].T.v);
            }
            
           

            ddt(simi, 0);
            for (int i = 0; i < nrstages; i++)
            {
                dTwaterdt[i][1] = dTwaterdt[i][0];
                dTairdt[i][1] = dTairdt[i][0];
                dhumidityairdt[i][1] = dhumidityairdt[i][0];
            }

            ddt(simi, h/2);
            for (int i = 0; i < nrstages; i++)
            {
                dTwaterdt[i][2] = dTwaterdt[i][0];
                dTairdt[i][2] = dTairdt[i][0];
                dhumidityairdt[i][2] = dhumidityairdt[i][0];
            }

            ddt(simi, h / 2);
            for (int i = 0; i < nrstages; i++)
            {
                dTwaterdt[i][3] = dTwaterdt[i][0];
                dTairdt[i][3] = dTairdt[i][0];
                dhumidityairdt[i][3] = dhumidityairdt[i][0];
            }

            ddt(simi, h);
            for (int i = 0; i < nrstages; i++)
            {
                dTwaterdt[i][4] = dTwaterdt[i][0];
                dTairdt[i][4] = dTairdt[i][0];
                dhumidityairdt[i][4] = dhumidityairdt[i][0];
            }

            for (int i = 0; i < nrstages; i++)
                {
                    watersegment[i].T.v += h / 6 * (dTwaterdt[i][1] + 2 * dTwaterdt[i][2] + 2 * dTwaterdt[i][3] + dTwaterdt[i][4]);
                    airsegment[i].T.v += h / 6 * (dTairdt[i][1] + 2 * dTairdt[i][2] + 2 * dTairdt[i][3] + dTairdt[i][4]);
                    airabshumidity[i] += h / 6 * (dhumidityairdt[i][1] + 2 * dhumidityairdt[i][2] + 2 * dhumidityairdt[i][3] + dhumidityairdt[i][4]); 
                }


            fanpowerstatespacex1 += ddtfanpowerstatespacex1 * global.SampleT;
            fanpowerstatespacex2 += ddtfanpowerstatespacex2 * global.SampleT;

            fanpower.v = fanpowerstatespacex1;
            if (fanpower.v < 0) { fanpower.v = 0; }
            

            outflow[0].mat.T.v = watersegment[0].T.v;
            outflow[1].mat.T.v = airsegment[nrstages - 1].T.v;

            outflow[0].mat.copycompositiontothismat(inflow[0].mat);
            outflow[1].mat.copycompositiontothismat(inflow[1].mat);

            inflow[0].mat.P.v += dstrm0pressureindt * global.SampleT; 
            //outflow[0].mat.P.v = global.Ps; //standard pressure since it is open to the atmosphere.
            outflow[0].massflow.v = watersegmentflowout[0];
            outflow[0].mat.density.v = watersegment[0].density.v; 

            outflow[1].mat.P.v += dstrm1pressureoutdt * global.SampleT;
            outflow[1].massflow.v = airsegmentdryairflowout[nrstages - 1]*(1 + airabshumidity[nrstages - 1]);
            outflow[1].mat.density.v = airsegment[nrstages - 1].density.v; 

            if (outflow[0].mat.T.v < 0) { outflow[0].mat.T.v = 0; }

            if (historise && (simi % global.SimVectorUpdatePeriod == 0))
            {
                for (int i = 0; i < nrstages; i++)
                {
                    interfaceabshumiditysimvector[i][simi / global.SimVectorUpdatePeriod] = interfaceabshumidity[i];
                    airabshumiditysimvector[i][simi / global.SimVectorUpdatePeriod] = airabshumidity[i];
                    if (watersegment[i].T.simvector != null) { watersegment[i].T.simvector[simi / global.SimVectorUpdatePeriod] = watersegment[i].T.v; }
                    if (interfacetemperaturesimvector[i] != null) { interfacetemperaturesimvector[i][simi / global.SimVectorUpdatePeriod] = interfacetemperature[i]; }
                }

                if (fanpower.simvector != null) { fanpower.simvector[simi / global.SimVectorUpdatePeriod] = fanpower.v; }


                if (fanspeed.simvector != null) { fanspeed.simvector[simi / global.SimVectorUpdatePeriod] = hybridfanspeed; }

                if (on.simvector != null) { on.simvector[simi / global.SimVectorUpdatePeriod] = on.v; }

                if (masstransfercoefair.simvector != null) { masstransfercoefair.simvector[simi / global.SimVectorUpdatePeriod] = masstransfercoefair.v; }

                if (heattransfercoefwater.simvector != null) { heattransfercoefwater.simvector[simi / global.SimVectorUpdatePeriod] = heattransfercoefwater.v; }
                
            }

        }

        public override void setproperties(simulation asim) //Method that will be inherited and that will set the properties of the applicable object in a window
        {
            coolingtowerproperties coolingtowerheatexchangersimpleprop = new coolingtowerproperties(this, asim);
            coolingtowerheatexchangersimpleprop.Show();
        }

        public override void showtrenddetail(simulation asim, List<Form> detailtrendslist)
        {
            detailtrendslist.Add(new coolingtowerdetail(this, asim));
            detailtrendslist[detailtrendslist.Count - 1].Show();
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
            {new Point(global.OriginX + Convert.ToInt32(global.GScale*(location.x - 0.5*global.CTWidthDraw)), 
                    global.OriginY + Convert.ToInt32(global.GScale*(location.y + 0.5*global.CTHeightDraw))), 
            new Point(global.OriginX + Convert.ToInt32(global.GScale*(location.x - 0.5*global.CTWidthDraw)), 
                    global.OriginY + Convert.ToInt32(global.GScale*(location.y - 0.5*global.CTHeightDraw))), 
            new Point(global.OriginX + Convert.ToInt32(global.GScale*(location.x + 0.5*global.CTWidthDraw)), 
                    global.OriginY + Convert.ToInt32(global.GScale*(location.y - 0.5*global.CTHeightDraw))), 
            new Point(global.OriginX + Convert.ToInt32(global.GScale*(location.x + 0.5*global.CTWidthDraw)),
                    global.OriginY + Convert.ToInt32(global.GScale*(location.y + 0.5*global.CTHeightDraw)))};
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
