using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chemsim
{

    //public static enum components { Wax, Condensate };

    public enum objecttypes { FTReactor, GasPipe, LiquidPipe, Pump, Tank, Valve, Tee, Mixer, Stream, PIDController, HX,
        HeatExchangerSimple, SteamGenerator, Flange, NMPC, CoolingTower, CoolingTowerSimple, CoolingTowerHeatExchangerSimple, DistillationColumn, Signal,
        Selector, ControlMVSignalSplitter };

    public enum liquidpipeflowreference { PipeEntrance, PipeEnd };

    public enum materialphase { Solid, Liquid, Gas };

    public enum piddirection { Direct = -1, Reverse = 1 };

    public enum baseclasstypeenum { UnitOp, Stream, Block };

    public enum calculationmethod { DetermineFlow, DeterminePressure };

    public enum nmpcalgorithm { UnconstrainedLineSearch, InteriorPoint1, ActiveSet1, GeneticAlgorithm1, ParticleSwarmOptimisation1 };

    public enum datasourceforvar { Simulation, Exceldata };

    public enum typesofselector { LowSelector, HighSelector };

    public enum components { Naphtha = 0, Air = 1 };

    [Serializable]
    public static class global
    {
        //Timing constants ---------------------------------------------------------------------------------------------------------------------------
        public static int TimerInterval = 1; // micro seconds
        public static double SpeedUpFactor = 200; //50, CT and normal sim: 200; //factor  Heat exchangers: 30000
        public static double SampleT = calcSampleT(); //Function returns a value since then reader can see which function it is.
        public static double SimVectorUpdateT = 1.0; //CT: 1.0; //seconds; HX: 30s; normal sim with cooling tower: 1s.
        public static int SimVectorUpdatePeriod = Convert.ToInt32(Math.Round(SimVectorUpdateT / SampleT)); //Nr. samples between saving in vect.
        public static double TrendUpdateT = 1.0; //seconds.  The simulation period of updating trends in simulation.  
                                                 //; HX: 30s; normal sim with cooling tower: 1s.
        public static int TrendUpdateIterPeriod = Convert.ToInt32(Math.Round(TrendUpdateT / SampleT)); //Nr. of samples between update trend.
        public static double SimTime = 3600.0*4; // Valve stepping: 3600.0*1; Normal full model: 3600.0*4 ; CT alone:  64830; //seconds 3600*1;//3600*24;  135: ES004 53340 ; for 172EP004: 34440;for CT fitting: 12 hours/3hours.
        public static int SimIterations = calcSimIterations(); //Nr of iterations of the simulation.
        public static int SimVectorLength = calcSimVectorLength();
        public static double PlantDataSampleT = 30.0; //30 seconds for CT;  The sample frequency of the IP.21 data used for fitting.


        //Scientific constants -----------------------SOME OF THESE WILL LATER BE ABLE TO BE MOVED TO THE FLUID PACKAGE-----------------------------------------------------------------------------
        public static double g = 9.81; //m/s^2
        public static double R = 8.314; // J/Kelvin/molecule
        public static double Ps = 100000; //Pa
        public static double Ts = 273.15; //Calvin
        //Water constants
        public static double WaterDensity = 1000; //kg/m^3
        public static double DeltaHWater = 2257*1000; //Joule / kg
        //Air constants
        public static double AirDensity = 1.225; //kg/m^3

        //Differential constants ----------------------------------------------------------------------------------------------------------------------
        public static double limithm = 0.00001; //0.00001; //h in the limit to zero as a multiplier for derivative calculation.
        public static double limitjacnmpc = 0.00001;  //0.00001  h in the limit to zero as a multiplier for derivative calculation for nmpc.
        public static double limitjacnmpcadd = 0.0001;  //h in the limit to zero as an added infinitesimal constant for derivative calculation for nmpc.
        public static int RungaKuta = 4; //Number of runga kuta iteration calculations for integration of the diff equations.

        //Calculation constants
        public static double Epsilon = 0.00000001; //small number that is added to a denominator in order to divide by it safely.
        public static double ConvergeDiffFrac = 0.001; //Fraction difference or less that will be treated as convergance.

        //Timing variables
        public static double[] simtimevector;

        //Screen constants
        public static double GScale = 1600 / 100; //pixels per m
        public static int OriginX = 10; //pixels
        public static int OriginY = 60; //pixels
        public static double DefaultLocationX = 0;
        public static double DefaultLocationY = 0;
        public static double MinDistanceFromPoint = 0.5; //m : Minimum Distance from each point for it to be selected
        
        public static double MinDistanceFromGasPipe = 0.5; //m : Minimum distance from each gas pipe for it to be selected

        //Simulation-wide constants
        public static double RelativeHumidity = 80; //%.  Average RH for Doha through the year.  Might need to make this an instantaneous figure later
                                                    //    on.  wwww.qatar.climatemps.com/humidity.php
        public static double AmbientTemperature = 25 + 273; //K.  To be converted to degC for Stull equation.  This is the average daily mean
                                                            //through out the year.  To be changed later based on when the simulation is run.
                                                            //Currently from en.wikipedia.org/wiki/Doha#Climate
        public static double HeCircuitFlowT0 = 85.0; //kg/s
        public static double SGPGasInlet = 65 * 100000; //Pa
        public static double SGPGasOutlet = 58.5 * 100000; //Pa
        public static double PBPGasInlet = 6.9e6; //Pa
        public static double PBPGasOutlet = 6.5e6; //Pa
        public static double PBTGasInlet = 259.1 + 273; //Kelvin.
        public static double PBTGasOutlet = 700 + 273; //Kelvin.

        //Material properties
        public static double AirMolarMass = 0.02897; //kg/mol
        public static double CO2MolarMass = 0.018; //kg/mol
        public static double H2OMolarMass = 0.0180153; //kg/mol
        public static double HeMolarMass = 4.002602e-3; //kg/mol
        

        //in and out point constants
        public static double InOutPointWidth = 0.2; //m
        public static double InOutPointHeight = 0.08; //m

        //baseprocessclass class default properties
        public static double baseprocessclassInitMass = 10; //kg
        public static double baseprocessclassInitMassFlow = 0; //kg/h
        public static double baseprocessclassInitPressure = 1 * Ps; //Pa    210*Ps
        public static double baseprocessclassInitTemperature = Ts + 25; //Kelvin.  25 degC   Ts + 170
        public static double baseprocessclassInitVolume = 1; //m3

        //chromosome class constants ------------------------------------------------------------------------------------------------------------------
        public static int DefaultMaxValueforMVs = 100; //the top end of the frac/perc range of each MV.
        public static int MaxBinaryLengthChromosome = 7;

        //complex class constants ----------------------------------------------------------------------------------------------------------------------
        public static double ZeroImaginary = 0.0001;


        //coolingtower class constants (CT) ---------This class is initially modelled on the Swedish paper, Marques later added-------------------------------------------------------------------------

        public static int CTRK4ArraySize = 5;

        public static double CTHeight = 14.1;  //m
        public static double CTWidth = 14.4; //m
        public static double CTLength = 14.4; //m  
        public static int CTDefaultNrStages = 10;  //The number of discretisations of the water, interface and air streams

        public static double CTDefaultFanDP = 211.9; //Pa.  Calculated on model Excel sheet.
        public static double CTDefaultFanTotalEfficiency = 0.866; //Fraction.  From CT datasheet.
        public static double CTFanSpeedT0 = 120.1 / 60.0; //revolutions per second; from data sheet for fan.
        public static double CTFanPowerT0 = 137000; //W; as per CT datasheet.
        public static double CTFanShutdownSpeed = 0.1; //rps.  Speed at shutdown to keep simulating the air that does exchange heat with the water when the fan is turned off.
        
        //Below are the model paramaters for the second order power transient for the fans and pumps
        public static double RotatingPercOS = 50; //% percentage overshoot
        public static double RotatingTsettle = 15; //seconds; settling time.
        public static double RotatingZeta = -Math.Log(RotatingPercOS/100)/(Math.Sqrt(Math.Pow(Math.PI,2) + Math.Pow(Math.Log(RotatingPercOS/100),2)));
        //public static double RotatingOmegaN = 4 / (RotatingZeta * RotatingTsettle);
        public static double RotatingOmegaN = -Math.Log(0.02 * Math.Sqrt(1 - Math.Pow(RotatingZeta, 2))) / (RotatingZeta * RotatingTsettle);
        //public static double RotatingZeta = 0.4;
        //public static double RotatingOmegaN = 1 / 8;
        
        public static double Rotatingb0 = Math.Pow(RotatingOmegaN, 2);
        public static double Rotatinga0 = Math.Pow(RotatingOmegaN, 2);
        public static double Rotatinga1 = 2 * RotatingZeta * RotatingOmegaN;
        public static double Rotatinga2 = 1;

        public static double CTTotalInterfaceArea = CTWidth * CTHeight;
        public static double CTTotalHorizontalArea = CTWidth * CTLength;
        public static double CTTotalVolume = CTWidth * CTLength * CTHeight;
        public static double CTFillVolume = 1161; //m^3;  as per CT data sheet.
        public static double CTDefaultSegmentVolume = CTWidth * CTLength * CTHeight / CTDefaultNrStages;

        public static double CTWaterVolumeFraction = 0.1;
        public static double CTPackingVolumeFraction = CTFillVolume / CTTotalVolume;
        public static double CTAirVolumeFraction = 1 - CTWaterVolumeFraction - CTPackingVolumeFraction;
        
        public static double CTDropletRadius = 0.001; //m; Assumption  0.001
        public static double CTDropletVolume = 4 / 3 * Math.PI * Math.Pow(CTDropletRadius, 3);
        
        
        public static double CTDropletSurfaceArea = 4 * Math.PI * Math.Pow(CTDropletRadius, 2);
        //
        

        public static double CTLewisFactor = 1; //This is from Lewis' work.
        public static double CTCpAir = 1013; //J/kgK; At 400K.

        public static double CTTransferCoefCoef = 1 / CTDefaultSegmentVolume; //Multiplier to scale all the 
        public static double CTDefaultMassTransferCoefficientAir = 0.000657; // kg/(s*m^2) 0.0001; CTTransferCoefCoef*2.71E-14; //; fitted value . 9.2688E-07
        public static double CTDefaultHeatTransferCoefficientWater = 64.395; //W/(m^2*K) . 1.0; fitted value .CTTransferCoefCoef*14.814
        public static double CTDefaultHeatTransferCoefficientAir = 0.6658;  //W/(m^2*K) . 1.0; fitted value .CTTransferCoefCoef*5.729;
        
        
        public static int CTNIn = 2;  //One flow in is water (strm1), the other is air (strm2).
        public static int CTNOut = 2;

        public static double AAntoineWater = 8.07131; //Antoine equation coefficients for water vapour pressure.  This is for the equation yielding mmHg
        public static double BAntoineWater = 1730.63;
        public static double CAntoineWater = 233.426;
        public static double AbsHumidityConst = 2.16679 / 1000.0; //kg*K/J
        public static double ConvertmmHgtoPa = 133.3223; //Pa per mmHg

        public static double WaterSatPressC1 = -7.85951;
        public static double WaterSatPressC2 = 1.844;
        public static double WaterSatPressC3 = -11.786;
        public static double WaterSatPressC4 = 22.68;
        public static double WaterSatPressC5 = -15.96;
        public static double WaterSatPressC6 = 1.801;

        public static double BuckC1 = 611.21;
        public static double BuckC2 = 18.678;
        public static double BuckC3 = 234.5;
        public static double BuckC4 = 257.14;

        

        public static double CTTuningFactor = 0.9; //factor to throttle the amount of cooling for tuning the total model purposes (to be removed later).

        public static double CTHeightDraw = 5.0; //meter
        public static double CTWidthDraw = 5.0; //meter
        public static double[] CTInPointsFraction = new double[] { 0.1, 0.9 }; //input 1: Cooling water return
        public static double[] CTOutPointsFraction = new double[] { 0.10, 0.9 }; //Output 1: Cooling water supply

        public static double CTTemperatureTau = 15 * 60; //seconds.  From Muller Craig article.

        //Strm1 is normally the warm stream, and Strm2 the cold stream.  So for the CT strm1 will then be the water that is coming in, and strm2 the air.
        public static double CTDefaultU = 497; //from datasheet    330 * 1000000 / 3600; //W/(m^2*K);  Taken from the Muller/Craig article and converted to SI units.
        public static double CTDefaultA = 287; //m^2 ; From the Muller/Craig article this figure would have been 100.

        public static double CTMassFlowStrm0T0 = 6169960 / 3600.0; //kg/s , based on Flows to Equipment sheet.
        public static double CTMolFlowStrm0T0 = CTMassFlowStrm0T0 / H2OMolarMass; //MOLAR MASS here is for the water flow then back from the plant.
        
        public static double CTMassFlowStrm1T0 = 2340671/3600.0; //kg/s , from fitted data per cooling tower.
        public static double CTMolFlowStrm1T0 = CTMassFlowStrm1T0 / AirMolarMass; //MOLAR MASS TO BE CHANGED HERE TO BE GENERIC
        

        public static double CTPStrm0Inlet = 2.0 * Ps + Ps; //Pa  2.0 barg
        public static double CTPStrm0Outlet = Ps;
        public static double CTPStrm1Inlet = Ps;
        public static double CTPStrm1Outlet = CTPStrm1Inlet - 0.1 * Ps; //THE FAN MODEL WILL NEED TO BE ADDED HERE LATER TO MAKE THIS MORE ACCURATE.

        public static double CTTStrm0Inlet = 273 + 45; //Kelvin (water)
        public static double CTTStrm0Outlet = 273 + 35; //Kelvin (water)
        public static double CTTStrm1Inlet = AmbientTemperature; //Kelvin (air)
        public static double CTTStrm1Outlet = 273 + 39.6; //Kelvin (air)

        public static double CTTInterfaceT0 = 0.5 * (CTTStrm0Inlet + CTTStrm1Inlet);

        public static double CTStrm0ValveOpeningDefault = 1.0; //fraction
        public static double CTStrm0Cv = CTMassFlowStrm0T0 / CTStrm0ValveOpeningDefault / 
            Math.Sqrt((CTPStrm0Inlet - CTPStrm0Outlet) / WaterDensity);

        public static double CTPMaxFactorIncreaseperSampleT = 2.0;

        public static double CTStrm1FlowCoefficient = CTMassFlowStrm1T0 / Math.Sqrt((CTPStrm1Inlet - CTPStrm1Outlet) / AirDensity);
        public static double CTStrm0TempTau = CTTemperatureTau; //seconds.  
        public static double CTStrm1TempTau = CTTemperatureTau; //seconds.  
        public static double CTStrm0FlowTau = 60; //seconds.  Based on Muller-Craig.
        public static double CTStrm1FlowTau = 60; //seconds.  Based on Muller-Craig.



        //coolingtowerheatexchangersimple class constants (CTHES) -------------------------------------------------------------------------------------

        public static int CTHESNIn = 2;  //One flow in is water (strm1), the other is air (strm2).
        public static int CTHESNOut = 2;

        public static double CTHESTuningFactor = 0.9; //factor to throttle the amount of cooling for tuning the total model purposes (to be removed later).

        public static double CTHESHeight = 5.0; //meter
        public static double CTHESWidth = 5.0; //meter
        public static double[] CTHESInPointsFraction = new double[] { 0.1, 0.9 }; //input 1: Cooling water return
        public static double[] CTHESOutPointsFraction = new double[] { 0.10, 0.9}; //Output 1: Cooling water supply

        public static double CTHESTemperatureTau = 15 * 60; //seconds.  From Muller Craig article.

        //Strm1 is normally the warm stream, and Strm2 the cold stream.  So for the CT strm1 will then be the water that is coming in, and strm2 the air.
        public static double CTHESDefaultU = 497; //from datasheet    330 * 1000000 / 3600; //W/(m^2*K);  Taken from the Muller/Craig article and converted to SI units.
        public static double CTHESDefaultA = 287; //m^2 ; From the Muller/Craig article this figure would have been 100.

        public static double CTHESMassFlowStrm1T0 = 6169960 / 3600.0; //kg/s , based on Flows to Equipment sheet.
        public static double CTHESMolFlowStrm1T0 = CTHESMassFlowStrm1T0 / H2OMolarMass; //MOLAR MASS here is for the water flow then back from the plant.
        public static double CTHESMassFlowStrm2T0 = CTHESMassFlowStrm1T0; //kg/s , FOR NOW. TO BE CHANGED LATER.
        public static double CTHESMolFlowStrm2T0 = CTHESMassFlowStrm2T0 / AirMolarMass; //MOLAR MASS TO BE CHANGED HERE TO BE GENERIC

        public static double CTHESPStrm1Inlet = 3.7 * Ps + Ps; //Pa
        public static double CTHESPStrm1Outlet = Ps;
        public static double CTHESPStrm2Inlet = Ps;
        public static double CTHESPStrm2Outlet = CTHESPStrm2Inlet - 0.1*Ps; //THE FAN MODEL WILL NEED TO BE ADDED HERE LATER TO MAKE THIS MORE ACCURATE.

        public static double CTHESTStrm1Outlet = 373 + 35; //Kelvin (water)
        public static double CTHESTStrm2Outlet = 373 + 39.6; //Kelvin (air)

        public static double CTHESStrm1FlowCoefficient = CTHESMassFlowStrm1T0 / Math.Sqrt((CTHESPStrm1Inlet - CTHESPStrm1Outlet) / WaterDensity);

        public static double CTHESStrm2FlowCoefficient = CTHESMassFlowStrm2T0 / Math.Sqrt((CTHESPStrm2Inlet - CTHESPStrm2Outlet) / AirDensity);
        public static double CTHESStrm1TempTau = CTHESTemperatureTau; //seconds.  
        public static double CTHESStrm2TempTau = CTHESTemperatureTau; //seconds.  
        public static double CTHESStrm1FlowTau = 60; //seconds.  Based on Muller-Craig.
        public static double CTHESStrm2FlowTau = 60; //seconds.  Based on Muller-Craig.


        //coolingtowersimple class constants (CTS) ---------------------------------------------------------------------------------------------------
        public static double CTSApproach = 4.5;  //K or degC (different in Temps so either unit applies).  From Muller&Craig article.
        public static int CoolingTowerSimpleNIn = 1;  //For now this will just be the main process flow in and out for simplicity,  can be changed again
        //later to add more complexity.
        public static int CoolingTowerSimpleNOut = 1;
        public static double CTSFlowMakeUp = 144.0 * 1000 / 3600; //kg/s  Taken from ORYX GTL plant data for the cooling tower make-up.
        public static double CTSVaporisationFraction = 0.00153; //fraction per degree C
        public static double CTSTuningFactor = 0.1; //factor to throttle the amount of cooling for tuning the total model purposes (to be removed later).

        public static double CTSHeight = 5.0; //meter
        public static double CTSWidth = 5.0; //meter
        public static double[] CTSInPointsFraction = new double[] { 0.1 }; //input 1: Cooling water return
        public static double[] CTSOutPointsFraction = new double[] { 0.50 }; //Output 1: Cooling water supply

        public static double CTSTemperatureTau = 5 * 60; //seconds.  From Muller Craig article.


        //distillation column default properties -------------------------------------------------------------------------------------------------------
        public static double DistillationColumnRadius = 2.4; //m
        public static double DistillationColumnHeight = 22.1; //m
        public static int NTrays = 1;
        public static int DistillationColumnNIn = 1;
        public static int DistillationColumnNOut = 2;
        public static double InitialDCTrayVolume = 0.61 * Math.Pow(0.6, 2) * Math.PI; //Some dimentions from p 471 
        //Chemical Process Equipment - Selection and Design
        public static double InitialDCTrayU = 100; //Joule.  VROOM guess.
        public static double InitialDCTrayn = 100000; //Moles.  VROOM guess.

        public static double[] DistillationColumnInPointsFraction = new double[] { 0.50 }; //input 1: Gas feed, Input 2: Catalyst slurry.
        public static double[] DistillationColumnOutPointsFraction = new double[] { 0.05, 0.95 }; //Output 1: Products, Output 2: Catalyst take-out.


        //embeddedtrend class constants
        public static double EmbeddedTrendWidth = 10; //meters
        public static double EmbeddedTrendHeight = 7.5; //meters


        //flange class constants -----------------------------------------------------------------------------------------------------------------------
        public static double FlangeLength = 0.2; //m


        //ftreactor class constants
        public static double FTReactorRadius = 4.8; //m
        public static double FTReactorHeight = 44.2; //m
        public static int FTReactorNIn = 2;
        public static int FTReactorNOut = 2;
        public static double[] FTReactorInPointsFraction = new double[] { 0.60, 0.95 }; //input 1: Gas feed, Input 2: Catalyst slurry.
        public static double[] FTReactorOutPointsFraction = new double[] { 0.50, 0.95 }; //Output 1: Products, Output 2: Catalyst take-out.

        public static double FTReactorMaxVolume = 10; //m3 - Arbritrary number at this stage.  
        public static double FTReactorInitInventory = 50.0; //%
        public static int OrigLoading = 13;
        public static double CatDecayRate = -0.4 / (60 * 60 * 24); //% productivity change/tonne/second
        public static double FreshCatAct = 9 / (60 * 60 * 24); //Tonnes product per second per tonne catalyst : Productivity
        public static double RegenCatAct = FreshCatAct * 0.90;  //% : productivity of regenerated catalyst - It looses 10% productivity
        public static double NrRegen = 4.0;
        public static double LostInRegen = 5; //% : percentage weight lost during the regen process that can never be used again since it is fines - basically spent
        // catalyst.
        public static double CatTake = 20 * 1000 * 12 / 365 / 24 / 3600; //kg/second out of the reactor (spent catalyst + catalyst to be regenned).  Does not include losses in regen.
        public static double RegenIn = CatTake * NrRegen / (1 + NrRegen) * (100 - LostInRegen) / 100;  //tonnes per month
        public static double RegenLosses = CatTake * NrRegen / (1 + NrRegen) * (LostInRegen) / 100;  //tonnes per month
        public static double CatSpent = CatTake - RegenIn - RegenLosses;
        public static double FreshCatIn = CatTake - RegenIn; //tonnes per month
        public static double[] FreshCatLoadingConst = new double[] {106,
                                          80,
                                          40,
                                          30,
                                          20,
                                          20,
                                          20,
                                          20,
                                          20,
                                          20,
                                          20,
                                          20,
                                          20,
                                          20};

        //gaspipe class constants
        public static double PipeDefaultLength = 100; //m
        public static double PipeDefaultDiameter = 0.5; //m
        public static double PipeDefaultPressure = Ps; //Pa
        public static double PipeDefaultTemperature = Ts + 25; //Kelvin
        //public static double PipeDefaultMoles = 100; //This will need to be changed.
        public static double PipeDefaultFiLocation = 0.5;

        

        //liquidpipe class constants

        public static string[] liquidpipeflowreferencestrings = new string[] {
            "Pipe entrance",
            "Pipe end"};

        

        

        

        //heatexchanger class constants -----------------------------------------------------------------------------------------
        public static double HEThermalPower = 5*1000000.0; //W  200.0 * 1000000   - > 200 MW.
        public static int HeatExchangerNIn = 2;
        public static int HeatExchangerNOut = 2;
        public static double[] HeatExchangerInPointsFraction = new double[] { 0.05, 0.95 }; //input 1: Hot Gas feed, Input 2: Water
        public static double[] HeatExchangerOutPointsFraction = new double[] { 0.05, 0.95 }; //Output 1: Cooled Gas, Output 2: Steam.
        public static double HeatExchangerRadius = 1; //m
        public static double HeatExchangerWidth = 6; //m
        

        public static int NStrm2Coils = 455;
        public static int HENSegments = 3;
        public static int HENNodes = HENSegments + 1; //The nodes are the boundaries, and the segements are what is between the nodes.

        //This is based on the Modelling design Excel file and Areva design.
        

        public static double HETStrm1Inlet = 188 + 273;//699.9 + 273; //K
        public static double HETStrm1Outlet = 45 + 273;//244.7 + 273; //245 + 273; //K
        public static double HETStrm2Inlet = 35 + 273; //35 + 273;//170.0 + 273; //K
        public static double HETStrm2Outlet = 45 + 273;//325 + 273;//530 + 273; //K

        public static double HEPStrm1Inlet = 10.3*100000; //65 * 100000; //Pa
        public static double HEPStrm1Outlet = HEPStrm1Inlet - 0.97 * 100000;//58.5 * 100000; //Pa
        public static double HEPStrm2Inlet = 1*100000;//210 * 100000; //Pa
        public static double HEPStrm2Outlet = HEPStrm2Inlet - 0.46*100000;//HEPStrm2Inlet - 20 * 100000; //Pa

        public static double HEPStrm1Delta = (HEPStrm1Inlet - HEPStrm1Outlet) / (HENSegments); //Not NSegments plus 1 since 
        //outflow[0] is now going to 
        //just be an extension of the final
        //segment
        public static double HEPStrm2Delta = (HEPStrm2Inlet - HEPStrm2Outlet) / (HENSegments);

        public static double[] HETStrm1T0 = new double[] { HETStrm1Outlet, 0.5 * (HETStrm1Outlet + HETStrm1Inlet), HETStrm1Inlet };
        public static double[] HETStrm2T0 = new double[] { HETStrm2Inlet, 0.5 * (HETStrm2Inlet + HETStrm2Outlet), HETStrm2Outlet };
        public static double[] HEPStrm1T0 = new double[] { HEPStrm1Outlet, HEPStrm1Inlet - 2*HEPStrm1Delta, HEPStrm1Inlet - HEPStrm1Delta};
        public static double[] HEPStrm2T0 = new double[] { HEPStrm2Inlet - HEPStrm2Delta, HEPStrm2Inlet - 2*HEPStrm2Delta, HEPStrm2Inlet - 3*HEPStrm2Delta };

        public static double HEMassFlowStrm1T0 = 99600.0 / 3600.0 / NStrm2Coils; //kg/s Per tube.
        public static double[] HEMassFlowArrayStrm1T0 = new double[] { HEMassFlowStrm1T0, HEMassFlowStrm1T0, HEMassFlowStrm1T0, HEMassFlowStrm1T0 }; //kg/s  From AREVA design.
        public static double MolFlowStrm1T0 = HEMassFlowStrm1T0 / CO2MolarMass; //MOLAR MASS TO BE CHANGED HERE TO BE GENERIC

        public static double HEMassFlowStrm2T0 = 477000.0 / 3600.0 / NStrm2Coils; //kg/s  Needs to be slighly higher
        public static double MolFlowStrm2T0 = HEMassFlowStrm2T0 / H2OMolarMass; //MOLAR MASS TO BE CHANGED HERE TO BE GENERIC

        //heatexchanger class : Metal differential equation constants in particular
        public static double HEM = 0.1;  //2.42305008; //kg/m; The mass of steam tube per unit length.  From Excel sheet where the properties

        //heatexchanger class : pressure drop / energy drop due to friction constants
        ///public static double HEAddFriction = 10.0;
        public static double HEStrm1AddFriction = 0.1; //1
        public static double HEStrm2AddFriction = 1; //1,   10.0;
        public static double[] HEStrm1DeltaPK = { 1, (HEPStrm1T0[1] - HEPStrm1T0[0]) / Math.Pow(MolFlowStrm1T0, 2.0), 
                                                    (HEPStrm1T0[2] - HEPStrm1T0[1]) / Math.Pow(MolFlowStrm1T0, 2.0), 
                                                    (HEPStrm1Inlet - HEPStrm1T0[2]) / Math.Pow(MolFlowStrm1T0, 2.0)};

        public static double[] HEStrm2DeltaPK = {   (HEPStrm2Inlet - HEPStrm2T0[0]) / Math.Pow(MolFlowStrm2T0, 2.0), 
                                                    (HEPStrm2T0[0] - HEPStrm2T0[1]) / Math.Pow(MolFlowStrm2T0, 2.0), 
                                                    (HEPStrm2T0[1] - HEPStrm2T0[2]) / Math.Pow(MolFlowStrm2T0, 2.0),
                                                    1};
        //public static double[] HEStrm2DeltaPK = HEPStrm2Delta / Math.Pow(MolFlowStrm2T0, 2.0);
        
        //heatexchanger class : heat exchange constants
        public static double HEHeatExchangeSurfaceArea = 329; //m2
        public static double HEOutsideDiameterTube = 19.05 / 1000.0; //m
        public static double HETubeWallThickness = 2.11 / 1000.0; //m
        public static double HEInsideDiameterTube = HEOutsideDiameterTube - 2 * HETubeWallThickness; //m
        public static double HENrPassesThroughShell = 2.0;
        public static double HETubeCircOutside = Math.PI * HEOutsideDiameterTube;  //m; Tube Circumferance on the outside.  
        public static double HETubeCircInside = Math.PI * HEInsideDiameterTube;  //m; Tube Circumferance on the inside.  
        public static double HETubeCircAve = 0.5 * (HETubeCircOutside + HETubeCircInside);
        public static double HEAveLengthPerTube = 6.1; //6.1; //102.6490726; //m; Lenth per tube in AREVA design as per sheet.
        public static double HEAveLengthPerSegment = HEAveLengthPerTube / HENSegments;
        public static double HEAStrm2 = Math.PI * Math.Pow(HEInsideDiameterTube / 2.0, 2.0); //m2; Cross sectional area of the pipes for the steam/water
        public static double HEStrm2TubeVolume = HEAStrm2* HEAveLengthPerTube; //0.042648188; //m^3
        public static double HEStrm2SegmentVolume = HEStrm2TubeVolume / HENSegments; //m^3
        public static double HEShellVolume = 1.040 * 6.096 - HEStrm2TubeVolume * NStrm2Coils;
        public static double HEStrm1TubeVolume = HEShellVolume / NStrm2Coils; //3.641659803; //m^3
        public static double HEAStrm1 = HEStrm1TubeVolume / HEAveLengthPerTube;  //m2;  From Excel sheet from AREVA design.
        public static double HEEffGasTubeCircInside = 2 * Math.Sqrt(HEAStrm1 / Math.PI) * Math.PI;//2 * Math.Sqrt(HEAStrm1 / Math.PI) * Math.PI;
        public static double HEStrm1SegmentVolume = HEStrm1TubeVolume / HENSegments; //m^3
        public static double HERi = 0.0001;  //0.001;  // K∙s³/kg = K m^2 / W .  Thermal conductivity's reciprocal times thickness of wall.
        
        //public static double HEAg = 0.035476792;  //m2;  From Excel sheet from AREVA design.
        public static double[] HEInPointsFraction = new double[] { 0.05, 0.95 }; //input 1: Hot Gas feed, Input 2: Water
        public static double[] HEOutPointsFraction = new double[] { 0.05, 0.95 }; //Output 1: Cooled Gas, Output 2: Steam.
        //public static double HETubeDiameter = 0.023; //m; Diameter of the tubes in the steam generator.  From Excel sheet
        //public static double HEAs = Math.PI * Math.Pow(HETubeDiameter / 2.0, 2.0); //m2; Cross sectional area of the pipes for the steam/water
        
        


        //thermal resistivity of the metal times the tube thickness.
        //This kgm and Ri, needs to be backed up with some more science.  Why is it so 
        //dificult to get these values and to fix things up properly?
        public static double HECsi = 0.1 ; //0.8;  //Dimensionless. These values need to be backed up with some more research.  Why is it so difficult to 
        //get proper values for these?
        public static double HECgi = 0.1; //0.8;  //Dimensionless. These values need to be backed up with some more research.  Why is it so difficult to 
        //get proper values for these? 
        public static double HEHeatTransferArea = global.HETubeCircAve * global.HEAveLengthPerTube / global.HENSegments;
        public static double[] HEKgm = new double[] {(HEHeatTransferArea * (0.5 * (HETStrm1T0[1] - HETStrm2T0[1])) /
            (HEThermalPower*1 / NStrm2Coils) - HERi*0.5) /
            Math.Pow(HEMassFlowStrm1T0,-HECgi),
            (HEHeatTransferArea * (0.5 * (HETStrm1T0[1] - HETStrm2T0[1])) /
            (HEThermalPower*1 / NStrm2Coils) - HERi*0.5) /
            Math.Pow(HEMassFlowStrm1T0,-HECgi),
            (HEHeatTransferArea * (0.5 * (HETStrm1T0[1] - HETStrm2T0[1])) /
            (HEThermalPower*1 / NStrm2Coils) - HERi*0.5) /
            Math.Pow(HEMassFlowStrm1T0,-HECgi)
            };
        public static double[] HEKms = new double[] {(HEHeatTransferArea * (0.5 * (HETStrm1T0[1] - HETStrm2T0[1])) /
            (HEThermalPower*1 / NStrm2Coils) - HERi*0.5) /
            Math.Pow(HEMassFlowStrm2T0,-HECsi),
            (HEHeatTransferArea * (0.5 * (HETStrm1T0[1] - HETStrm2T0[1])) /
            (HEThermalPower*1 / NStrm2Coils) - HERi*0.5) /
            Math.Pow(HEMassFlowStrm2T0,-HECsi),
            (HEHeatTransferArea * (0.5 * (HETStrm1T0[1] - HETStrm2T0[1])) /
            (HEThermalPower*1 / NStrm2Coils) - HERi*0.5) /
            Math.Pow(HEMassFlowStrm2T0,-HECsi)};
        //public static double[] HEKgm = new double[] {(HEHeatTransferArea * (0.5 * (HETStrm1T0[0] - HETStrm2T0[0])) /
        //    (HEThermalPower*1 / NStrm2Coils / HENSegments) - HERi*0.5) /
        //    Math.Exp(-HEMassFlowStrm1T0*HECgi),
        //    (HEHeatTransferArea * (0.5 * (HETStrm1T0[1] - HETStrm2T0[1])) /
        //    (HEThermalPower*1 / NStrm2Coils / HENSegments) - HERi*0.5) /
        //    Math.Exp(-HEMassFlowStrm1T0*HECgi),
        //    (HEHeatTransferArea * (0.5 * (HETStrm1T0[2] - HETStrm2T0[2])) /
        //    (HEThermalPower*1 / NStrm2Coils / HENSegments) - HERi*0.5) /
        //    Math.Exp(-HEMassFlowStrm1T0*HECgi)};
        //public static double[] HEKms = new double[] {(HEHeatTransferArea * 0.5 * (HETStrm1T0[0] - HETStrm2T0[0]) /
        //    (HEThermalPower*1 / NStrm2Coils / HENSegments) - HERi*0.5) /
        //    Math.Exp(-HEMassFlowStrm2T0*HECsi),
        //    (HEHeatTransferArea * 0.5 * (HETStrm1T0[1] - HETStrm2T0[1]) /
        //    (HEThermalPower*1 / NStrm2Coils / HENSegments) - HERi*0.5) /
        //    Math.Exp(-HEMassFlowStrm2T0*HECsi),
        //    (HEHeatTransferArea * 0.5 * (HETStrm1T0[2] - HETStrm2T0[2]) /
        //    (HEThermalPower*1 / NStrm2Coils / HENSegments) - HERi*0.5) /
        //    Math.Exp(-HEMassFlowStrm2T0*HECsi)};

        


        //heatexchangersimple (HES) class constants  -----------------------------------------------------------------------------
        //Strm1 is normally the warm stream, and Strm2 the cold stream 
        public static double HeatExchangerSimpleDefaultU = 500; // 497 from datasheet    330 * 1000000 / 3600; //W/(m^2*K);  Taken from the Muller/Craig article and converted to SI units.
        public static double HeatExchangerSimpleDefaultA = 441; // 287 m^2 ; From the Muller/Craig article this figure would have been 100.

        public static double HESMassFlowStrm1T0 = 325000.0 / 3600.0; //kg/s , biggest CW exchanger in CW circuit.
        public static double HESMolFlowStrm1T0 = HESMassFlowStrm1T0 / CO2MolarMass; //MOLAR MASS TO BE CHANGED HERE TO BE GENERIC
        public static double HESMassFlowStrm2T0 = 1366538.0 / 3600.0; //kg/s , for the whole strm2.
        public static double HESMolFlowStrm2T0 = HEMassFlowStrm2T0 / H2OMolarMass; //MOLAR MASS TO BE CHANGED HERE TO BE GENERIC

        public static double HESPStrm1Inlet = 3.7*Ps + Ps; //Pa
        public static double HESPStrm1Outlet = HESPStrm1Inlet - 0.5*Ps;
        public static double HESPStrm2Inlet = 3.5*Ps + Ps;
        public static double HESPStrm2Outlet = HESPStrm2Inlet - 2 * Ps;

        public static double HESStrm1FlowCoefficient = HESMassFlowStrm1T0/Math.Sqrt((HESPStrm1Inlet - HESPStrm1Outlet)/WaterDensity); //SPECIFIC GRAVITY OF THE STREAM TO BE ADDED LATER TO THIS CALC 
        public static double HESStrm2FlowCoefficient = 10.21076364; //This is fitted in the model - average one for all exchangers in model.  
        public static double HESStrm1TempTau = 6 * 60; //seconds.  Based on Muller-Craig.
        public static double HESStrm2TempTau = 6 * 60; //seconds.  Based on Muller-Craig.
        public static double HESStrm1FlowTau = 60; //seconds.  Based on Muller-Craig.
        public static double HESStrm2FlowTau = 60; //seconds.  Based on Muller-Craig.

        public static int HESNSegments = 2; //For the simple heat exchanger, the inflow and outflow streams on the two sides will be modelled as the only
                                            //2 segments.
        public static int HESNStrm2Coils = 1; //heatexchangersimple class will be modelled with one big coild for strm2 only.

        //heatexchangersimple class : heat exchange constants
        //public static double HEHeatExchangeSurfaceArea = 329; //m2
        //public static double HEOutsideDiameterTube = 19.05 / 1000.0; //m
        //public static double HETubeWallThickness = 2.11 / 1000.0; //m
        //public static double HEInsideDiameterTube = HEOutsideDiameterTube - 2 * HETubeWallThickness; //m
        //public static double HENrPassesThroughShell = 2.0;
        //public static double HETubeCircOutside = Math.PI * HEOutsideDiameterTube;  //m; Tube Circumferance on the outside.  
        //public static double HETubeCircInside = Math.PI * HEInsideDiameterTube;  //m; Tube Circumferance on the inside.  
        //public static double HETubeCircAve = 0.5 * (HETubeCircOutside + HETubeCircInside);
        //public static double HEAveLengthPerTube = 6.1; //6.1; //102.6490726; //m; Lenth per tube in AREVA design as per sheet.
        //public static double HEAveLengthPerSegment = HEAveLengthPerTube / HENSegments;
        //public static double HEAStrm2 = Math.PI * Math.Pow(HEInsideDiameterTube / 2.0, 2.0); //m2; Cross sectional area of the pipes for the steam/water
        //public static double HEStrm2TubeVolume = HEAStrm2 * HEAveLengthPerTube; //0.042648188; //m^3
        public static double HESStrm2Volume = HEStrm2TubeVolume * NStrm2Coils;
        public static double HESStrm2SegmentVolume = HESStrm2Volume / HENSegments; //m^3
        public static double HESShellVolume = 1.040 * 6.096 - HESStrm2Volume;
        public static double HESStrm1TubeVolume = HEShellVolume / NStrm2Coils; //3.641659803; //m^3
        //public static double HEAStrm1 = HEStrm1TubeVolume / HEAveLengthPerTube;  //m2;  From Excel sheet from AREVA design.
        //public static double HEEffGasTubeCircInside = 2 * Math.Sqrt(HEAStrm1 / Math.PI) * Math.PI;//2 * Math.Sqrt(HEAStrm1 / Math.PI) * Math.PI;
        public static double HESStrm1SegmentVolume = HESShellVolume / HESNSegments; //m^3

        //material class constants ---------------------------------------------------------------------------------------------------------------------
        public static double Udefault = 19500 * 100000; //Joule
        public static double Vdefault = 18.01528 * 100000 / 1000 / 1000; //m^3
        public static double fdefault = 0.5; //Vapour molar fraction. just a value in order to get some convergiance. 
        public static List<component> fluidpackage;
        public static materialphase MaterialInitPhase = materialphase.Liquid;
        public static int NMaterialIterations = 10;
        public static double ZNotDefined = -999999.0;
        public static double epsilonadd = 0.0001; //for derivatives where the denominator is added to the variable being differentiated with respect to, and not
        //multiplied.
        public static double epsilonfrac = 1.001;
        public static double Inf = 9999999999;

        //mixer class constants ------------------------------------------------------------------------------------------------------------------------
        public static double MixerLength = 1; //m
        public static double MixerDistanceBetweenBranches = 1; //m
        public static int MixerDefaultNIn = 2;
        public static double MixerBranchThickness = 0.1; //m
        public static double MixerInitRadiusDefault = MixerDefaultNIn * (MixerDistanceBetweenBranches + MixerBranchThickness);

        //molecule class constants
        public static double InitTc = 500;              //Kelvin
        public static double InitPc = 50 * 100000;      //Pa;
        public static double Initomega = 0.3;           //Default Acentric factor.

        //nmpc class constants ----------------------------------------------------------------------------------------------------------------------
        public static double NMPCWidth = 2; //m
        public static double NMPCHeight = 2; //m
        public static int DefaultN = 9000; //3000; //Default Optimisation horison 80
        public static int DefaultInitialDelay = 0;
        public static int DefaultRunInterval = 300; //Assuming TSample is 10 sec, so then the interval would be a multiple of 
                                                    //that.
        public static double Defaultalphak = 1.0; //0.1; //0.1   0.001; //How much of line search delta is implemented.
        public static nmpcalgorithm DefaultNMPCAlgorithm = nmpcalgorithm.ParticleSwarmOptimisation1; //nmpcalgorithm.ParticleSwarmOptimisation1; //nmpcalgorithm.UnconstrainedLineSearch;
        public static double DefaultNMPCSigma = 0.8; //Multiplier of mubarrier for each iteration of the nmpc algorithm.

        //Interior Point 1 - ConstrainedLineSearch algorithm constants
        public static double DefaultMuBarrier = 0.00000000000000001; //initial value / guess , for initialisation.
        public static double Defaultsvalue = 0.6; //since the constraints at this point are the valve openings in ChemSim, a fraction half will be
                                                        //the initial value of the valve openings, and thus the values will be 0.5 away from zero.
        public static double DefaultsvalueMultiplier = 0.9; //1.0;
        //public static double DefaultSigma = 0.1; //The fraction multiplied with mubarrier at the end of each iteration.
        public static double DefaulttauIP = 0.95; //Tau constant for Interior Point methods.
        public static double DefaultIPErrorTol = 0.0001; //If the max of the norm is below this figure, then the algorithm will stop, and we are close enough to a solution.
        public static double CholeskyDelta = 0.01; //small delta used in Hessian modification.
        public static double CholeskyBeta = 1000000; //Big value that will be used to try and cut down D matrix values if they become too large in Hessian modification.
        public static double MVMaxMovePerSampleTT0 = 0.1; //Fraction of MV range.
        public static double NMPCIPWeightPreTerm = 1;
        public static double NMPCIPWeightTerminal = 10;

        //Genetic Algorithm 1 constants (mostly default values for variables in the nmpc class).
        public static int DefaultNrChromosomes = 12; //The total number of total solutions that will be kept in memory each iteration.
        public static int DefaultNrSurvivingChromosomes = 9; //Nr of chromosomes that will be passed to the next iteration and not replaced by new random ones.
        public static int DefaultNrPairingChromosomes = 8; //The nr of chromosomes of the total population that will be pairing and producing children.
        public static double DefaultProbabilityOfMutation = 0.1; //The probability that a child will be mutated in one bit.
        public static int DefaultNrIterations = 10; //The number of iterations until the best GA solution will be passed to the update method.
        public static int DefaultCrossOverPoint = 3; //Bit index nr (starting from zero) from right to left in the binary representation, where
                                                     //cross over and mutation will start.
        
        //PSO constants
        public static int DefaultNrContinuousParticles = 20; //The total number of total solutions that will be kept in memory each iteration.
        //public static int DefaultNrParticles = 20; //The total number of total solutions that will be kept in memory each iteration.
        public static int DefaultNrBooleanParticles = DefaultNrContinuousParticles; //The total number of total solutions that will be kept in memory each iteration.
        public static int PSOMVBoundaryBuffer = 10; //Distance from boundary that particles are put at random when they cross the boundary.
        public static double PSOMaxBooleanSpeed = 1.0; //Max probability paramater for sigmoid function for boolean PSO.


        //pidcontroller class constants -------------------------------------------------------------------------------------------------------------
        public static int Direct = -1;
        public static int Reverse = 1;
        public static double PIDControllerInitRadius = 0.4; //m
        public static double PIDControllerInitK = 1;
        public static double PIDControllerInitI = 100;
        public static double PIDControllerInitD = 0;
        public static double PIDControllerInitMinOP = 0; //Engineering units.
        public static double PIDControllerInitMaxOP = 1; //Engineering units.
        public static double PIDControllerInitMinPV = 0; //Engineering units.
        public static double PIDControllerInitMaxPV = 1; //Engineering units.

        

        //pump class default properties -------------------------------------------------------------------------------------------------------------
        public static double PumpInitMaxDeltaPressure = 6.7*2*100000; //Pa
        public static double PumpInitMinDeltaPressure = 0; //Pa
        public static double PumpInitMaxActualFlow = 8700000*2/2.0 / 3600.0 / 1000; //m3/s  Dividing by 2 since we will now have 2 pumps in parallel.
                                                                                    //assume a density of 1000 as well.
        public static double PumpMinActualFlow = 0.01 * PumpInitMaxActualFlow; //This is for when the pumps is off its curve due to too high DP.
        public static double PumpCurveYAxis = WaterDensity * g * 70; //Pa.  Making this more than the data sheet for now to make sure the pump
                                                                     //can survive well at that level.
        public static double PumpCurvef1 = 8500/3600.0; //m3/s;  Actual flow.  From pump data sheet.
        public static double PumpCurvep1 = WaterDensity * g * 50; //Pa
        public static double PumpCurvef2 = 15000 / 3600.0; //m3/s; Actual flow.
        public static double PumpCurveSpeedT0 = 740 / 60.0; //rev per second.  From pump data sheet.
        public static double PumpSpeedTau = 60; //seconds.
        
        public static double PumpInitActualVolumeFlow = 0; //m3/s
        public static double PumpInitOn = 1; //0 for off, 1 for on
        public static double PumpInitRadius = 0.4; //m
        public static double PumpInitOutletLength = 0.5; //m
        public static double PumpInitOutletRadius = 0.05; //m

        //stream class default properties -------------------------------------------------------------------------------------------------------------
        public static int SignalNrPropDisplay = 1;

        //steamgenerator class constants
        public static double SteamGeneratorRadius = 2.4; //m
        public static double SteamGeneratorHeight = 12.1; //m
        public static int SteamGeneratorNSegments = 3;
        public static int SteamGeneratorNNodes = SteamGeneratorNSegments + 1; //The nodes are the boundaries, and the segements are what is between the nodes.
        public static double SteamGeneratorWaterTubeVolume = 0.042648188; //m^3
        public static double SteamGeneratorWaterSegmentVolume = SteamGeneratorWaterTubeVolume / SteamGeneratorNSegments; //m^3
        public static double SteamGeneratorGasTubeVolume = 3.641659803; //m^3
        public static double SteamGeneratorGasSegmentVolume = SteamGeneratorGasTubeVolume / SteamGeneratorNSegments; //m^3
        public static int SteamGeneratorNIn = 2;
        public static int SteamGeneratorNOut = 2;
        public static int NSteamCoils = 220; //From AREVA design.
        public static double ThermalPower = 200000000.0; //W  200.0 * 1000000   - > 200 MW.
        public static double TubeCircOutside = 0.092991143;  //m; Tube Circumferance on the outside.  
        //This is based on the Modelling design Excel file and Areva design.
        public static double TubeCircInside = 0.072256631;  //m; Tube Circumferance on the inside.  
        public static double TubeCircAve = 0.5 * (TubeCircOutside + TubeCircInside);
        public static double AveLengthPerTube = 102.6490726; //m; Lenth per tube in AREVA design as per sheet.
        public static double AveLengthPerSegment = AveLengthPerTube / SteamGeneratorNSegments;
        public static double[] SteamGeneratorInPointsFraction = new double[] { 0.05, 0.95 }; //input 1: Hot Gas feed, Input 2: Water
        public static double[] SteamGeneratorOutPointsFraction = new double[] { 0.05, 0.95 }; //Output 1: Cooled Gas, Output 2: Steam.

        public static double Ws0T0 = 77.0 / NSteamCoils; //kg/s  From AREVA design.



        public static double SGTGasInlet = 699.9 + 273; //K
        public static double SGTGasOutlet = 244.7 + 273; //245 + 273; //K
        public static double SGTWaterInlet = 170.0 + 273; //K
        public static double SGTWaterOutlet = 325 + 273;//530 + 273; //K
        public static double WgasT0 = HeCircuitFlowT0 / NSteamCoils; //kg/s
        public static double MolFlowGasT0 = WgasT0 / HeMolarMass;
        public static double WwaterT0 = 77.0 / NSteamCoils; //kg/s
        public static double MolFlowWaterT0 = WwaterT0 / H2OMolarMass;


        public static double SGPGasDelta = (SGPGasInlet - SGPGasOutlet) / (SteamGeneratorNSegments); //Not NSegments plus 1 since 
        //outflow[0] is now going to 
        //just be an extension of the final
        //segment
        public static double SGPWaterInlet = 210 * 100000; //Pa
        public static double SGPWaterOutlet = SGPWaterInlet - 20 * 100000; //Pa
        public static double SGPWaterDelta = (SGPWaterInlet - SGPWaterOutlet) / (SteamGeneratorNSegments);

        public static double[] TGasT0 = new double[] { SGTGasOutlet, 0.5 * (SGTGasOutlet + SGTGasInlet), SGTGasInlet };
        public static double[] TWaterT0 = new double[] { SGTWaterInlet, 0.5 * (SGTWaterInlet + SGTWaterOutlet), SGTWaterOutlet };
        public static double[] PGasT0 = new double[] { SGPGasOutlet, SGPGasInlet - 2*SGPGasDelta, 
            SGPGasInlet - SGPGasDelta};
        public static double[] PWaterT0 = new double[] { SGPWaterInlet - SGPWaterDelta, 
            SGPWaterInlet - 2*SGPWaterDelta, SGPWaterInlet - 3*SGPWaterDelta };


        //steamgenerator class : Heat exchange constants
        public static double Ri = 0.001;   // K∙s³/kg = K m^2 / W .  Thermal conductivity's reciprocal times thickness of wall.
        //thermal resistivity of the metal times the tube thickness.
        //This kgm and Ri, needs to be backed up with some more science.  Why is it so 
        //dificult to get these values and to fix things up properly?
        public static double Csi = 0.8;  //Dimensionless. These values need to be backed up with some more research.  Why is it so difficult to 
        //get proper values for these?
        public static double Cgi = 0.8;  //Dimensionless. These values need to be backed up with some more research.  Why is it so difficult to 
        //get proper values for these? 
        public static double HeatTransferArea = global.TubeCircAve * global.AveLengthPerTube / global.SteamGeneratorNSegments;
        //public static double Kgm = 
        //    (HeatTransferArea * (0.5 * (0.5 * (SGTGasInlet + SGTGasOutlet) - 0.5 * (SGTWaterOutlet + SGTWaterInlet))) /
        //    (ThermalPower / NSteamCoils / SteamGeneratorNSegments) - Ri * 0.5) /
        //    Math.Exp(-WgasT0 * Cgi); //THIS IS TO BE PUT BACK AS THE WAY OF INITIALISING LATER ON.
        //public static double Kgm = (HeatTransferArea * (0.5 * (0.5 * (TGasInlet + TGasOutlet) - 0.5 * (TWaterOutlet + TWaterInlet))) /
        //    (ThermalPower / NSteamCoils / SteamGeneratorNSegments)) /
        //    Math.Pow(WgasT0, -Cgi);

        public static double[] Kgm = new double[] {(HeatTransferArea * (0.5 * (TGasT0[0] - TWaterT0[0])) /
            (ThermalPower*1 / NSteamCoils / SteamGeneratorNSegments) - Ri*0.5) /
            Math.Exp(-WgasT0*Cgi),
            (HeatTransferArea * (0.5 * (TGasT0[1] - TWaterT0[1])) /
            (ThermalPower*1 / NSteamCoils / SteamGeneratorNSegments) - Ri*0.5) /
            Math.Exp(-WgasT0*Cgi),
            (HeatTransferArea * (0.5 * (TGasT0[2] - TWaterT0[2])) /
            (ThermalPower*1 / NSteamCoils / SteamGeneratorNSegments) - Ri*0.5) /
            Math.Exp(-WgasT0*Cgi)};

        //public static double Kms = 
        //    (HeatTransferArea * (0.5 * (0.5 * (SGTGasInlet + SGTGasOutlet) - 0.5 * (SGTWaterOutlet + SGTWaterInlet))) /
        //    (ThermalPower / NSteamCoils / SteamGeneratorNSegments) - Ri * 0.5) /
        //    Math.Exp(-WwaterT0 * Cgi);  //THIS IS TO BE PUT BACK AS THE WAY OF INITIALISING LATER ON.
        public static double[] Kms = new double[] {(HeatTransferArea * 0.5 * (TGasT0[0] - TWaterT0[0]) /
            (ThermalPower*1 / NSteamCoils / SteamGeneratorNSegments) - Ri*0.5) /
            Math.Exp(-WwaterT0*Cgi),
            (HeatTransferArea * 0.5 * (TGasT0[1] - TWaterT0[1]) /
            (ThermalPower*1 / NSteamCoils / SteamGeneratorNSegments) - Ri*0.5) /
            Math.Exp(-WwaterT0*Cgi),
            (HeatTransferArea * 0.5 * (TGasT0[2] - TWaterT0[2]) /
            (ThermalPower*1 / NSteamCoils / SteamGeneratorNSegments) - Ri*0.5) /
            Math.Exp(-WwaterT0*Cgi)};

        public static double[] WgT0 = new double[] { 85.0 / NSteamCoils, 85.0 / NSteamCoils, 85.0 / NSteamCoils, 85.0 / NSteamCoils }; //kg/s  From AREVA design.
        public static double Ag = 0.035476792;  //m2;  From Excel sheet from AREVA design.
        public static double EffGasTubeCircInside = 2 * Math.Sqrt(Ag / Math.PI) * Math.PI;
        public static double TubeDiameter = 0.023; //m; Diameter of the tubes in the steam generator.  From Excel sheet
        public static double As = Math.PI * Math.Pow(TubeDiameter / 2.0, 2.0); //m2; Cross sectional area of the pipes for the steam/water
        
        //steamgenerator class : Metal differential equation constants in particular
        public static double M = 2.42305008; //kg/m; The mass of steam tube per unit length.  From Excel sheet where the properties

        //steamgenerator class : pressure drop / energy drop due to friction constants
        public static double SteamGenAddFriction = 10.0;
        public static double SteamGenGasAddFriction = 1;
        public static double SGWaterDeltaPK = SGPWaterDelta / Math.Pow(MolFlowWaterT0, 2.0);
        public static double SGGasDeltaPK = SGPGasDelta / Math.Pow(MolFlowGasT0, 2.0);

        //steamgenerator class : MOMENTUM EQUATION constants
        public static double DynViscA = 0.2259; //Coefficient in front of Dynamic Viscocity equation as per Excel sheet.
        public static double DynViscB = -0.018; //Exponent coefficient of Dynamic Viscocity equation as per Excel sheet.

        //public static double[] Kfric = new double[] {(PsaveT0[0] - PsaveT0[1])/Math.Pow(Ws0T0,2),
        //                                             (PsaveT0[1] - PsaveT0[2])/Math.Pow(Ws0T0,2),
        //                                             (PsaveT0[2] - PsaveT0[3])/Math.Pow(Ws0T0,2)};

        //stream class default properties -------------------------------------------------------------------------------------------------------------

        public static double MinDistanceFromStream =  15; //pixels  0.5; //m Minimum distance from each stream for it to be selected
        public static double StreamArrowAngle = 30.0 / 180.0 * Math.PI; //radians
        public static double StreamArrowLength = 0.5; //m
        public static double StreamMaxMassFlow = 100000000; //kg/s (100,000 tps)
        public static double StreamMinMassFlow = -StreamMaxMassFlow;
        public static int StreamNrPropDisplay = 3;

        //tank class default properties ----------------------------------------------------------------------------------------------------------------
        public static double TankInitRadius = 22; //12.88280927; //m; from Cooling tower sump design info.
        public static double TankInitHeight = 13.2; //13.2; //m; //from cooling tower sump design.
        public static double TankRadiusDraw = 6.0; //meter
        public static double TankHeightDraw = 2.0; //meter
        public static double TankInitMaxVolume = Math.PI * Math.Pow(TankInitRadius, 2) * TankInitHeight; //m3
        public static double TankInitFracInventory = 0.5; //Fraction
        public static double TankInitInOutletDistanceFraction = 0.95; //fraction from the bottom or top that the inlet or outlet of the tank will be situated.
        public static double TankMinFracInventory = 0.02; //fraction 

        //tee class constants --------------------------------------------------------------------------------------------------------------------------
        public static double TeeLength = 1; //m
        public static double TeeDistanceBetweenBranches = 1; //m
        public static int TeeDefaultNOut = 2;
        public static double TeeBranchThickness = 0.1; //m
        public static double TeeInitRadiusDefault = TeeDefaultNOut * (TeeDistanceBetweenBranches + TeeBranchThickness);

        //trend class constants -----------------------------------------------------------------------------------------------------------------------
        public static float XAxisMargin = 20; //pixels 
        public static float YAxisMargin = 20; //pixels
        public static double YIncrement = 0.1; //fraction
        public static double YmaxMaxFactorAbove = 2.0; //factor

        //valve class constants -----------------------------------------------------------------------------------------------------------------------
        public static double ValveDefaultActualFlow = 800 / 3600.0; //From average HX flow in unit CW circuit.
        public static double ValveDefaultDP = 1.2 * Ps;
        public static double ValveDefaultOpening = 0.5; //Default valve opening.
        
        public static double ValveEqualPercR = 40; //Dimensionless constant for valve equalpercentage.
        public static double ValveDefaultCv = ValveDefaultActualFlow / (Math.Pow(ValveEqualPercR, ValveDefaultOpening - 1) * Math.Sqrt(ValveDefaultDP)); //m^3/s/(Pa^0.5)
        public static double ValveHydraulicTau = 10.0; //seconds.  Time constant of valve hydraulics.

        public static double ValveLength = 0.5; //m
        public static double ValveWidth = 0.4; //m



        static global()
        {
            initsimtimevector();

            fluidpackage = new List<component>();
            initmolecularlist();

        }

        private static void initmolecularlist()
        {
            //public molecule(string anabreviation, string aname, double amolarmass, double adynamicviscosity, double adensity, double aTc = 500, double aPc = 50*100000,
            //double aomega = 0.3, double aCpA = -4.224, double aCpB = 0.3063, double aCpC = -1.586e-04, double aCpD = 3.215e-08)

            fluidpackage.Add(new component(new molecule("Naphtha", "GTL Naphtha", 0.157, 0.00164, 661.4959, 273 + 495, 1.2411 * Math.Pow(10, 7),
                -1 - Math.Log10(110000 /1.2411 * Math.Pow(10, 7)) , 150.5, 0.6, 0, 0), 0));

            fluidpackage.Add(new component(new molecule("Air", "Air", 0.02897, 1.983 * Math.Pow(10, -5), 1.225, 132.41, 3.72 * Math.Pow(10, 6), 
                0.0335,
                0.8*31.15  + 0.2*28.11, 0.8*(-0.01357)  + 0.2*(-3.7) * Math.Pow(10, -6), 0.8*2.68*Math.Pow(10,-5)  + 0.2*1.746 * Math.Pow(10, -5),
            0.8 * (-1.168) * Math.Pow(10, -8) + 0.2 * (-1.065) * Math.Pow(10, -8)), 0));
            //Density: 1.977 kg/m3 (gas at 1 atm and 0 °C)

            fluidpackage.Add(new component(new molecule("CO2", "Carbon Dioxide", 0.018, 0.07 * 0.001, 1.977, 304.25, 7.39 * Math.Pow(10, 6), 0.228,
                19.8, 0.07344, -5.602E-05, 1.715E-08), 0));
            //Density: 1.977 kg/m3 (gas at 1 atm and 0 °C)

            fluidpackage.Add(new component(new molecule("CO", "Carbon Monoxide", 0.02801, 0.0001662 * 0.001, 1.145), 0));
            //Density: 1.145 kg/m3 at 25 °C, 1 atm

            fluidpackage.Add(new component(new molecule("H2", "Hydrogen", 0.0020158, 8.76 * Math.Pow(10, -6), 0.08988), 0));
            //Density: 0.08988 g/L = 0.08988 kg/m3 (0 °C, 101.325 kPa)

            fluidpackage.Add(new component(new molecule("He", "Helium", HeMolarMass, 0, 0.1786, 5.1953, 5.1953E6, -0.390,
                20.8, 0, 0, 0), 0));
            //essentially no viscosity.

            fluidpackage.Add(new component(new molecule("CH4", "Methane", 0.01604, 0.0001027 * 0.001, 0.6556), 0));
            //Density: 0.6556 g L−1 = 0.6556 kg/m3

            fluidpackage.Add(new component(new molecule("CH4O", "Methanol", 0.03204, 5.9E-04, 791.8, 513, 80.9 * 100000, 0.556,
                21.15, 0.07092, 2.587E-05, -2.852E-08), 0));
            //Density: 0.6556 g L−1 = 0.6556 kg/m3

            fluidpackage.Add(new component(new molecule("N", "Nitrogen", 0.028, 0.018 * 0.001, 1.251,126.192, 3.3958*Math.Pow(10,6), 0.04, 
                31.15, -0.01357, 2.68*Math.Pow(10,-5), -1.168*Math.Pow(10,-8)), 0));
            //Density: 1.251 g/L = 1.251 kg/m3
            fluidpackage.Add(new component(new molecule("O2", "Oxygen", 0.016, 2.04 * Math.Pow(10, -5), 1.429, 154.581, 5.043 * Math.Pow(10, 6),
                0.022, 28.11, -3.7 * Math.Pow(10, -6), 1.746 * Math.Pow(10, -5), -1.065 * Math.Pow(10, -8)), 0));

            fluidpackage.Add(new component(new molecule("H2O", "Water", 0.0180153, WaterDensity, 1, 647.096, 22060000, 0.344,
                7.243e01, 1.039e-2, -1.497e-6, 0), 1.0));
            //Density: 1000 kg/m3
            //Dynamic viscosity for water at 20 Deg C

            fluidpackage.Add(new component(new molecule("C2H6", "Ethane", 0.03007, 0, 1.3562 * 100), 0)); //Just assume no viscosity for the moment.
            fluidpackage.Add(new component(new molecule("C3H8", "Propane", 0.03007, 0, 1.3562 * 100, 369.8, 42.5 * 100000, 0.153,
                -4.224, 0.3063, -1.586e-04, 3.215e-08), 0)); //Just assume no viscosity for the moment.
            fluidpackage.Add(new component(new molecule("C4H10", "Butane", 58.12 / 1000, 0, 2.48 * 100, 425.2, 38.0 * 100000, 0.199,
                9.487, 0.3313, -1.108e-04, -2.822e-09), 0)); //Just assume no viscosity for the moment.
            fluidpackage.Add(new component(new molecule("C5H12", "Pentane", 72.15 / 1000, 240 / 1000000,
                0.626 * 1000), 0));
            fluidpackage.Add(new component(new molecule("C6H14", "2-Methylpentane", 86.18 / 1000, 0,
                653), 0));
            fluidpackage.Add(new component(new molecule("C7H16", "Heptane", 100.20 / 1000, 386 / 1000000,
                679.5), 0));
            fluidpackage.Add(new component(new molecule("C8H18", "Octane", 114.23 / 1000, 542 / 1000000,
                0.703 * 1000), 0));
            fluidpackage.Add(new component(new molecule("C9H20", "Nonane", 128.26 / 1000, 0.711 / 1000, 718), 0));
            fluidpackage.Add(new component(new molecule("C10H22", "Decane", 142.28 / 1000, 0.920 / 1000, 730, 617.8, 21.1 * 100000), 0));
            fluidpackage.Add(new component(new molecule("C11H24", "Undecane", 156.30826 / 1000, 0.920 / 1000, 740.2), 0)); // Just assume zero viscosity for now.
            fluidpackage.Add(new component(new molecule("C12H26", "Dodecane", 170.33 / 1000, 1.35 / 1000, 780.8), 0));
            fluidpackage.Add(new component(new molecule("C13H28", "Tridecane", 184.36 / 1000, 0, 756), 0)); // Just assume zero viscosity for now.
            fluidpackage.Add(new component(new molecule("C14H30", "Tetradecane", 198.39 / 1000, 2.18 / 1000, 756), 0)); // Just assume zero viscosity for now.
            fluidpackage.Add(new component(new molecule("C15H32", "Pentadecane", 212.41 / 1000, 0, 769), 0)); // Just assume zero viscosity for now.
            fluidpackage.Add(new component(new molecule("C16H34", "Hexadecane", 226.44 / 1000, 3.34 / 1000, 770), 0)); // Just assume zero viscosity for now.
            fluidpackage.Add(new component(new molecule("C17H36", "Heptadecane", 240.47 / 1000, 0, 777), 0)); // Just assume zero viscosity for now.
            fluidpackage.Add(new component(new molecule("C18H38", "Octadecane", 254.494 / 1000, 0, 777), 0)); // Just assume zero viscosity for now.
            fluidpackage.Add(new component(new molecule("C19H40", "Nonadecane", 268.5209 / 1000, 0, 786), 0)); // Just assume zero viscosity for now.
            fluidpackage.Add(new component(new molecule("C20H42", "Icosane", 282.55 / 1000, 0, 786), 0)); // Just assume zero viscosity for now.
            fluidpackage.Add(new component(new molecule("C21H44", "Heneicosane", 296.6 / 1000, 0, 792), 0)); // Just assume zero viscosity for now.
            fluidpackage.Add(new component(new molecule("C22H46", "Docosane", 310.61 / 1000, 0, 778), 0)); // Just assume zero viscosity for now.
            fluidpackage.Add(new component(new molecule("C23H48", "Tricosane ", 324.63 / 1000, 0, 797), 0)); // Just assume zero viscosity for now.
            fluidpackage.Add(new component(new molecule("C24H50", "Tetracosane", 338.66 / 1000, 0, 797), 0)); // Just assume zero viscosity for now.
            fluidpackage.Add(new component(new molecule("C25H52", "Pentacosane", 352.69 / 1000, 0, 801), 0)); // Just assume zero viscosity for now.
            fluidpackage.Add(new component(new molecule("C26H54", "Hexacosane", 366.71 / 1000, 0, 778), 0)); // Just assume zero viscosity for now.
            fluidpackage.Add(new component(new molecule("C27H56", "Heptacosane", 380.74 / 1000, 0, 780), 0)); // Just assume zero viscosity for now.
            fluidpackage.Add(new component(new molecule("C28H58", "Octacosane", 394.77 / 1000, 0, 807), 0)); // Just assume zero viscosity for now.
            fluidpackage.Add(new component(new molecule("C29H60", "Nonacosane", 408.80 / 1000, 0, 808), 0)); // Just assume zero viscosity for now.
            fluidpackage.Add(new component(new molecule("C30H62", "Triacontane", 422.82 / 1000, 0, 810), 0)); // Just assume zero viscosity for now.
            fluidpackage.Add(new component(new molecule("C31H64", "Hentriacontane", 436.85 / 1000, 0, 781), 0)); // Just assume zero viscosity for now.
            fluidpackage.Add(new component(new molecule("C32H66", "Dotriacontane", 450.88 / 1000, 0, 812), 0)); // Just assume zero viscosity for now.
            fluidpackage.Add(new component(new molecule("C33H68", "Tritriacontane", 464.90 / 1000, 0, 811), 0)); // Just assume zero viscosity for now.
            fluidpackage.Add(new component(new molecule("C34H70", "Tetratriacontane ", 478.93 / 1000, 0, 812), 0)); // Just assume zero viscosity for now.
            fluidpackage.Add(new component(new molecule("C35H72", "Pentatriacontane ", 492.96 / 1000, 0, 813), 0)); // Just assume zero viscosity for now.
            fluidpackage.Add(new component(new molecule("C36H74", "Hexatriacontane", 506.98 / 1000, 0, 814), 0)); // Just assume zero viscosity for now.
            fluidpackage.Add(new component(new molecule("C37H76", "Heptatriacontane", 520.99 / 1000, 0, 815), 0)); // Just assume zero viscosity for now.
            fluidpackage.Add(new component(new molecule("C38H78", "Octatriacontane", 535.03 / 1000, 0, 816), 0)); // Just assume zero viscosity for now.
            fluidpackage.Add(new component(new molecule("C39H80", "Nonatriacontane", 549.05 / 1000, 0, 817), 0)); // Just assume zero viscosity for now.
            fluidpackage.Add(new component(new molecule("C40H82", "Tetracontane", 563.08 / 1000, 0, 817), 0)); // Just assume zero viscosity for now.
            fluidpackage.Add(new component(new molecule("C41H84", "Hentetracontane", 577.11 / 1000, 0, 818), 0)); // Just assume zero viscosity for now.
            fluidpackage.Add(new component(new molecule("C42H86", "Dotetracontane", 591.13 / 1000, 0, 819), 0)); // Just assume zero viscosity for now.
            fluidpackage.Add(new component(new molecule("C43H88", "Triatetracontane", 605.15 / 1000, 0, 820), 0)); // Just assume zero viscosity for now.
            fluidpackage.Add(new component(new molecule("C44H90", "Tetratetracontane", 619.18 / 1000, 0, 820), 0)); // Just assume zero viscosity for now.
            fluidpackage.Add(new component(new molecule("C45H92", "Pentatetracontane", 633.21 / 1000, 0, 821), 0)); // Just assume zero viscosity for now.
            fluidpackage.Add(new component(new molecule("C46H94", "Hexatetracontane", 647.23 / 1000, 0, 822), 0)); // Just assume zero viscosity for now.
            fluidpackage.Add(new component(new molecule("C47H96", "Heptatetracontane", 661.26 / 1000, 0, 822), 0)); // Just assume zero viscosity for now.
            fluidpackage.Add(new component(new molecule("C48H98", "Octatetracontane", 675.29 / 1000, 0, 823), 0)); // Just assume zero viscosity for now.
            fluidpackage.Add(new component(new molecule("C49H100", "Nonatetracontane", 689.32 / 1000, 0, 823), 0)); // Just assume zero viscosity for now.
            fluidpackage.Add(new component(new molecule("C50H102", "Pentacontane", 703.34 / 1000, 0, 824), 0)); // Just assume zero viscosity for now.
            fluidpackage.Add(new component(new molecule("C51H104", "Henpentacontane", 717.37 / 1000, 0, 824), 0)); // Just assume zero viscosity for now.
            fluidpackage.Add(new component(new molecule("C52H106", "Dopentacontane", 731.39 / 1000, 0, 825), 0)); // Just assume zero viscosity for now.
            fluidpackage.Add(new component(new molecule("C53H108", "Tripentacontane", 745.42 / 1000, 0, 825), 0)); // Just assume zero viscosity for now.
            fluidpackage.Add(new component(new molecule("C54H110", "Tetrapentacontane", 759.45 / 1000, 0, 826), 0)); // Just assume zero viscosity for now.

            //public molecule(string anabreviation, string aname, double amolarmass, double adynamicviscosity (Pa·s), double adensity, double adefaultmolefraction)
        }

        public static double calcSampleT()
        {
            return TimerInterval / 1000.0 * SpeedUpFactor; // seconds - at this point SampleT 
        }

        public static int calcSimIterations()
        {
            double delta = (SampleT == 0) ? 0.01 : 0;
            return Convert.ToInt32(SimTime / (SampleT + delta)); //Nr of iterations of the simulation.
        }

        public static int calcSimVectorLength()
        {
            double delta = (SampleT == 0) ? 0.01 : 0;
            return Convert.ToInt32(SimTime / (SimVectorUpdateT + delta)); //Nr of iterations of the simulation.
        }

        public static void initsimtimevector()
        {
            if (simtimevector != null)
            {
                Array.Resize(ref simtimevector, global.SimVectorLength);
            }
            else
            {
                simtimevector = new double[global.SimVectorLength];
            }
            simtimevector[0] = 0;
            for (int i = 1; i < simtimevector.Length; i++)
            {
                simtimevector[i] = simtimevector[i - 1] + SimVectorUpdateT;
            }
        }


    }
}
