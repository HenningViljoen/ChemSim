using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace chemsim
{

    [Serializable]
    public class material
    {
        public controlvar P; //Pa
        
        public controlvar V; //m3.  This will be total stage volume in the case of a distillation column.
        public controlvar vmolar; //m3/mole.  Total volume per mole for the total material all phases.
        public controlvar vmolarL; //m3/mole.  Total volume per mole for the total liquid phase.
        public controlvar vmolarV; //m3/mole.  Total volume per mole for the total vapour phase.
        public controlvar density; //kg/m3
        
        public controlvar T; //Kelvin
        
        public controlvar mass; //kg
        public controlvar n; //amount of moles of the material (all phases) for all components.
        
        public controlvar f; //fraction.  Vapour molar fraction.
        
        public controlvar U; //Joule. Internal energy.

        public controlvar relativehumidity; //% ; Using relative humidity at the moment since that is the data that we have on site.
        

        public double[] x; //Molar fraction of the total liquid of the material per component.
        public double[] y; //Molar fraction of the total vapour of the material per component.
        public double hL; //Joule/mol.  Molar enthalpy for the liquid phase, e.g. on a stage in a distillation column.
        public double hV; //Joule/mol.  Molar enthalpy for the vapour phase, e.g. on a stage in a distillation column.
        public double ML; //moles.  Total molar liquid hold-up.
        public double MV; //moles.  Total molar vapour hold-up.
        public double[] z; //fraction.  Molar fraction per component of the total moles in the material.

        public controlvar umolar; //Joule/mol.  Molar internal energy.
        public double[] umolarL; //Joule/mol.  Molar internal energy for the total liquid phase.
        public double[] umolarV; //Joule/mol.  Molar internal energy for the total vapour phase.
        double totalumolar; //last calculation of the total vmolar;
        public double[] umolarideal; //Joule/mol.  Molar internal energy for the ideal gas case.
        public double[] fugacityL; //fugacity per component for the liquid phase.
        public double[] fugacityV; //fugacity per component for the vapour phase.
        public double[] aT; //kg⋅m^5 /( s^2 ⋅mol^2 ); figure used in Peng Robinson equation.
        public double[] ac; // kg⋅m^5 / (s^2 ⋅mol^2 ) ; figure used in Peng Robinson equation.
        public double[] alpha; //part of the equation for aT, and used in other equations in the Peng Robinson collection.
        public double[] b;  // m^3/mol ; figure used in Peng Robinson equation.
        public double[] K;  //dimensionless - depends on omega.
        public double[] A;  //used in equation for compressibility factor.
        public double[] B;  //used in equation for compressibility factor.
        public double[][] Z;  //Compressibility factor. More than one value should be present based on the amount of phases in the material
        public double[] discriminantZ; //The discriminant of the cubig PR equation for Z;
        public double[] Tr; //reduced temperature.
        public double[] acomp; //Cubic equation for compressibility factor coeficients.
        public double[] bcomp; //Cubic equation for compressibility factor coeficients.
        public double[] ccomp; //Cubic equation for compressibility factor coeficients.
        public double[] dcomp; //Cubic equation for compressibility factor coeficients.
        public double bs; //Used in scarlet cubic solver. bs for bscarlet, This is for another cubic solver: http://home.scarlet.be/~ping1339/cubic.htm
        public double cs; //Used in scarlet cubic solver.
        public double ds; //Used in scarlet cubic solver.
        public double rs; // Used in scarlet cubic solver.
        public double es; //Used in scarlet cubic solver.
        public double fs; //Used in scarlet cubic solver.
        public complex[] zs; //Used in scarlet cubic solver.
        public complex[] us; //Used in scarlet cubic solver.
        public complex[] ys; //Used in scarlet cubic solver.


        public double[] delta0; //used to solve the cubic equation for Z.
        public double[] delta1; //used to solve the cubic equation for Z.
        public double[] C; //used to solve the cubic equation for Z.
        public complex[] Cc; //used to solve the cubic equation for Z.
        public double[][] a; //used to solve the cubid equation for Z by making use of an alternative closed form formulat for them.
        public double[] Qc; //used in alternative formula.
        public double[] Rc; //used in alternative formula.
        public double[] Dc; //used in alternative formula.
        public complex[] Sc; //used in alternative formula.
        public complex[] Tc; //used in alternative formula.
        public double[] Cp; //Heat capacity of constant pressure for an ideal gas type scenario per component.

        public double totalCp; //Heat capacity of the material in totallity, with all its components includded.
        public double[] xvector;
        public matrix jacobian; //The jacobian matrix for the system of equations as defined below in fflash().
        public int uvflashsize; //The size of the k variables k equations problem to be solved for the UV flash.
        public int origuvflashsize;


        public List<component> composition; //mass in kg for each component in the material
        public double massofonemole; //kg/mole
        public double volumeofonemole; //m^3/mole
        public materialphase phase;

        //public int componentindex; //Index in the fuildpackage of the component of this material.  At this stage only 1 component
        //can be included per material in ChemSim.  To be updated later.  
                            //This should be taken out later.  Only reason it is here, is to make the convergence of the flashing algorithm work better at the moment.

        public material(double aV)
        {


            P = new controlvar();
            V = new controlvar();
            vmolar = new controlvar();
            vmolarL = new controlvar();
            vmolarV = new controlvar();
            density = new controlvar();
            T = new controlvar();
            mass = new controlvar();
            n = new controlvar();
            f = new controlvar();
            U = new controlvar();
            umolar = new controlvar();
            relativehumidity = new controlvar();

            double oldn = 0.0;
            composition = new List<component>();
            for (int i = 0; i < global.fluidpackage.Count; i++)
            {

                if (global.fluidpackage[i].n != 0)
                {
                    composition.Add(global.fluidpackage[i]);
                    oldn += composition[composition.Count - 1].n;
                }
            }



            x = new double[composition.Count];
            y = new double[composition.Count];
            z = new double[composition.Count];
            fugacityL = new double[composition.Count];
            fugacityV = new double[composition.Count];
            K = new double[composition.Count];
            Z = new double[composition.Count][];
            for (int i = 0; i < composition.Count; i++)
            {
                Z[i] = new double[6];
                nullZ(i);
            }

            discriminantZ = new double[composition.Count];
            Tr = new double[composition.Count];
            ac = new double[composition.Count];
            aT = new double[composition.Count];
            alpha = new double[composition.Count];
            b = new double[composition.Count];
            A = new double[composition.Count];
            B = new double[composition.Count];
            delta0 = new double[composition.Count];
            delta1 = new double[composition.Count];
            C = new double[composition.Count];
            Cc = new complex[composition.Count];


            Cp = new double[composition.Count];

            umolarL = new double[composition.Count];
            umolarV = new double[composition.Count];
            umolarideal = new double[composition.Count];

            acomp = new double[composition.Count]; //Cubic equation for compressibility factor coeficients.
            bcomp = new double[composition.Count]; //Cubic equation for compressibility factor coeficients.
            ccomp = new double[composition.Count]; //Cubic equation for compressibility factor coeficients.
            dcomp = new double[composition.Count]; //Cubic equation for compressibility factor coeficients.

            a = new double[composition.Count][];
            for (int i = 0; i < composition.Count; i++) { a[i] = new double[3]; }

            Qc = new double[composition.Count]; //used in alternative formula.
            Rc = new double[composition.Count]; //used in alternative formula.
            Dc = new double[composition.Count]; //used in alternative formula.
            Sc = new complex[composition.Count]; //used in alternative formula.
            Tc = new complex[composition.Count]; //used in alternative formula.

            zs = new complex[6]; //Used in scarlet cubic solver.
            us = new complex[2]; //Used in scarlet cubic solver.
            ys = new complex[6]; //Used in scarlet cubic solver.

            uvflashsize = 2 * composition.Count + 3;
            xvector = new double[uvflashsize];
            for (int i = 0; i < composition.Count; i++)
            {
                x[i] = 1.0 / (composition.Count); //Add 0.5 to get starting values that can converge.
                y[i] = 1.0 / (composition.Count);
            }
            if (composition.Count == 1) { uvflashsize = 3; } //x and y does not need to be part of the model anymore.
            origuvflashsize = uvflashsize;

            jacobian = new matrix(uvflashsize, uvflashsize);

            V.v = aV;
            P.v = global.baseprocessclassInitPressure;
            T.v = global.baseprocessclassInitTemperature;
            f.v = global.fdefault;

            mapvarstox(); //Variables are allocated according to the Flatby article sequence of equations.
            n.v = V.v / calcvmolaranddensity();

            for (int i = 0; i < composition.Count; i++)
            {
                composition[i].n = composition[i].n / oldn * n.v;
            }

            calcmass();



            U.v = n.v * calcumolar();
            //density.v = mass.v / V.v;

            calccompz();
            uvflash(); // This is commented out now, but needs to be put back in soon again.
            //P.simvector = new double[global.SimIterations];
            //T.simvector = new double[global.SimIterations];
            //f.simvector = new double[global.SimIterations];
            //n.simvector = new double[global.SimIterations];
            //U.simvector = new double[global.SimIterations];
            density.simvector = new double[global.SimVectorLength];

        }

        public material(List<component> acomposition, double aTemp, double aV, double aP, double af) //second constructor
        {
            P = new controlvar(); //Pa
            V = new controlvar(); //m3.  This will be total stage volume in the case of a distillation column.
            vmolar = new controlvar(); //m3/mole.  Total volume per mole for the total material all phases.
            vmolarL = new controlvar(); //m3/mole.  Total volume per mole for the total liquid phase.
            vmolarV = new controlvar(); //m3/mole.  Total volume per mole for the total vapour phase.
            density = new controlvar(); //kg/m3
            T = new controlvar(); //Kelvin
            mass = new controlvar(); //kg
            n = new controlvar(); //amount of moles of the material (all phases) for all components.
            f = new controlvar(); //fraction.  Vapour molar fraction.
            U = new controlvar(); //Joule. Internal energy.
            umolar = new controlvar();
            relativehumidity = new controlvar();
            composition = new List<component>();

            init(acomposition, aTemp, aV, aP, af);

            //P.simvector = new double[global.SimIterations];
            //T.simvector = new double[global.SimIterations];
            //f.simvector = new double[global.SimIterations];
            //n.simvector = new double[global.SimIterations];
            //U.simvector = new double[global.SimIterations];
            density.simvector = new double[global.SimVectorLength];
        }

        public void copyfrom(material materialcopyfrom)
        {
            P.v = materialcopyfrom.P.v; //Pa
            V.v = materialcopyfrom.V.v; //m3.  This will be total stage volume in the case of a distillation column.
            vmolar.v = materialcopyfrom.vmolar.v; //m3/mole.  Total volume per mole for the total material all phases.
            vmolarL.v = materialcopyfrom.vmolarL.v; //m3/mole.  Total volume per mole for the total liquid phase.
            vmolarV.v = materialcopyfrom.vmolarV.v; //m3/mole.  Total volume per mole for the total vapour phase.
            density.v = materialcopyfrom.density.v; //kg/m3
            T.copyfrom(materialcopyfrom.T); //Kelvin
            mass.v = materialcopyfrom.mass.v; //kg
            n.v = materialcopyfrom.n.v; //amount of moles of the material (all phases) for all components.
            f.v = materialcopyfrom.f.v; //fraction.  Vapour molar fraction.
            U.v = materialcopyfrom.U.v; //Joule. Internal energy.
            relativehumidity.v = materialcopyfrom.relativehumidity.v;

            Array.Copy(materialcopyfrom.x, x, materialcopyfrom.x.Length); //Molar fraction of the total liquid of the material per component.
            Array.Copy(materialcopyfrom.y, y, materialcopyfrom.y.Length); //Molar fraction of the total vapour of the material per component.
            hL = materialcopyfrom.hL; //Joule/mol.  Molar enthalpy for the liquid phase, e.g. on a stage in a distillation column.
            hV = materialcopyfrom.hV; //Joule/mol.  Molar enthalpy for the vapour phase, e.g. on a stage in a distillation column.
            ML = materialcopyfrom.ML; //moles.  Total molar liquid hold-up.
            MV = materialcopyfrom.MV; //moles.  Total molar vapour hold-up.
            Array.Copy(materialcopyfrom.z, z, materialcopyfrom.z.Length); //fraction.  Molar fraction per component of the total moles in the material.

            umolar.v = materialcopyfrom.umolar.v; //Joule/mol.  Molar internal energy.
            Array.Copy(materialcopyfrom.umolarL, umolarL, materialcopyfrom.umolarL.Length); //Joule/mol.  Molar internal energy for the total liquid phase.
            Array.Copy(materialcopyfrom.umolarV, umolarV, materialcopyfrom.umolarV.Length); //Joule/mol.  Molar internal energy for the total vapour phase.
            double totalumolar = materialcopyfrom.totalumolar; //last calculation of the total vmolar;
            Array.Copy(materialcopyfrom.umolarideal, umolarideal, materialcopyfrom.umolarideal.Length); //Joule/mol.  Molar internal energy for the ideal gas case.
            Array.Copy(materialcopyfrom.fugacityL, fugacityL, materialcopyfrom.fugacityL.Length); //fugacity per component for the liquid phase.
            Array.Copy(materialcopyfrom.fugacityV, fugacityV, materialcopyfrom.fugacityV.Length); //fugacity per component for the vapour phase.
            Array.Copy(materialcopyfrom.aT, aT, materialcopyfrom.aT.Length); //kg⋅m^5 /( s^2 ⋅mol^2 ); figure used in Peng Robinson equation.
            Array.Copy(materialcopyfrom.ac, ac, materialcopyfrom.ac.Length); // kg⋅m^5 / (s^2 ⋅mol^2 ) ; figure used in Peng Robinson equation.
            Array.Copy(materialcopyfrom.alpha, alpha, materialcopyfrom.alpha.Length); //part of the equation for aT, and used in other equations in the Peng Robinson collection.
            Array.Copy(materialcopyfrom.b, b, materialcopyfrom.b.Length);  // m^3/mol ; figure used in Peng Robinson equation.
            Array.Copy(materialcopyfrom.K, K, materialcopyfrom.K.Length);  //dimensionless - depends on omega.
            Array.Copy(materialcopyfrom.A, A, materialcopyfrom.A.Length);  //used in equation for compressibility factor.
            Array.Copy(materialcopyfrom.B, B, materialcopyfrom.B.Length);  //used in equation for compressibility factor.
            Array.Copy(materialcopyfrom.Z, Z, materialcopyfrom.Z.Length);  //Compressibility factor. More than one value should be present based on the amount of phases in the material
            Array.Copy(materialcopyfrom.discriminantZ, discriminantZ, materialcopyfrom.discriminantZ.Length); //The discriminant of the cubig PR equation for Z;
            Array.Copy(materialcopyfrom.Tr, Tr, materialcopyfrom.Tr.Length); //reduced temperature.
            Array.Copy(materialcopyfrom.acomp, acomp, materialcopyfrom.acomp.Length); //Cubic equation for compressibility factor coeficients.
            Array.Copy(materialcopyfrom.bcomp, bcomp, materialcopyfrom.bcomp.Length); //Cubic equation for compressibility factor coeficients.
            Array.Copy(materialcopyfrom.ccomp, ccomp, materialcopyfrom.ccomp.Length); //Cubic equation for compressibility factor coeficients.
            Array.Copy(materialcopyfrom.dcomp, dcomp, materialcopyfrom.dcomp.Length); //Cubic equation for compressibility factor coeficients.
            bs = materialcopyfrom.bs; //Used in scarlet cubic solver. bs for bscarlet, This is for another cubic solver: http://home.scarlet.be/~ping1339/cubic.htm
            cs = materialcopyfrom.cs; //Used in scarlet cubic solver.
            ds = materialcopyfrom.ds; //Used in scarlet cubic solver.
            rs = materialcopyfrom.rs; // Used in scarlet cubic solver.
            es = materialcopyfrom.es; //Used in scarlet cubic solver.
            fs = materialcopyfrom.fs; //Used in scarlet cubic solver.
            //public complex[] zs; //Used in scarlet cubic solver. I DO NOT THINK WE NEED TO COPY THESE VALUES AS THEY ARE INITED ON EACH ITERATION.
            //public complex[] us; //Used in scarlet cubic solver.
            //public complex[] ys; //Used in scarlet cubic solver.


            Array.Copy(materialcopyfrom.delta0, delta0, materialcopyfrom.delta0.Length); //used to solve the cubic equation for Z.
            Array.Copy(materialcopyfrom.delta1, delta1, materialcopyfrom.delta1.Length); //used to solve the cubic equation for Z.
            Array.Copy(materialcopyfrom.C, C, materialcopyfrom.C.Length); //used to solve the cubic equation for Z.
            Array.Copy(materialcopyfrom.Cc, Cc, materialcopyfrom.Cc.Length); //used to solve the cubic equation for Z.
            Array.Copy(materialcopyfrom.a, a, materialcopyfrom.a.Length); //used to solve the cubid equation for Z by making use of an alternative closed form formulat for them.
            Array.Copy(materialcopyfrom.Qc, Qc, materialcopyfrom.Qc.Length); //used in alternative formula.
            Array.Copy(materialcopyfrom.Rc, Rc, materialcopyfrom.Rc.Length); //used in alternative formula.
            Array.Copy(materialcopyfrom.Dc, Dc, materialcopyfrom.Dc.Length); //used in alternative formula.
            Array.Copy(materialcopyfrom.Sc, Sc, materialcopyfrom.Sc.Length); //used in alternative formula.
            Array.Copy(materialcopyfrom.Tc, Tc, materialcopyfrom.Tc.Length); //used in alternative formula.
            Array.Copy(materialcopyfrom.Cp, Cp, materialcopyfrom.Cp.Length); //Heat capacity of constant pressure for an ideal gas type scenario per component.

            totalCp = materialcopyfrom.totalCp; //Heat capacity of the material in totallity, with all its components includded.
            Array.Copy(materialcopyfrom.xvector, xvector, materialcopyfrom.xvector.Length);
            //public matrix jacobian; //The jacobian matrix for the system of equations as defined below in fflash().
            uvflashsize = materialcopyfrom.uvflashsize; //The size of the k variables k equations problem to be solved for the UV flash.
            origuvflashsize = materialcopyfrom.origuvflashsize;


            copycompositiontothismat(materialcopyfrom);
            massofonemole = materialcopyfrom.massofonemole; //kg/mole
            volumeofonemole = materialcopyfrom.volumeofonemole; //m^3/mole
            phase = materialcopyfrom.phase;

            //componentindex = materialcopyfrom.componentindex; 
        }

        public void copycompositiontothismat(material amat)
        {
            composition.Clear();
            for (int i = 0; i < amat.composition.Count; i++)
            {
                composition.Add(new component());

                composition[composition.Count - 1].copytothisobject(amat.composition[i]);
                //componentindex = i; //this will fall away later when there is more than one component per stream.
                composition[composition.Count - 1].molefraction = amat.composition[i].molefraction;
                //composition[composition.Count - 1].n = amat.composition[i].n / amat.n.v * n.v;  //this one will need to be tested might not be right.
            }
        }

        public static void copycompositiontothiscomposition(ref List<component> compositioncopyto, List<component> compositioncopyfrom)
        {
            compositioncopyto.Clear();
            for (int i = 0; i < compositioncopyfrom.Count; i++)
            {
                compositioncopyto.Add(new component());

                compositioncopyto[compositioncopyto.Count - 1].copytothisobject(compositioncopyfrom[i]);

                compositioncopyto[compositioncopyto.Count - 1].molefraction = compositioncopyfrom[i].molefraction;
            }
        }


        public void init(List<component> acomposition, double aTemp, double aV, double aP, double af)
        {
            //phase = global.MaterialInitPhase;

            composition.Clear();
            for (int i = 0; i < acomposition.Count; i++)
            {
                //if (global.fluidpackage[i].m.name == componentname)
                //{
                    composition.Add(new component());

                    composition[composition.Count - 1].copytothisobject(acomposition[i]);
                    //componentindex = i; //Should not be needed anymore soon.
                //}
            }
            x = new double[composition.Count];
            y = new double[composition.Count];
            z = new double[composition.Count];
            fugacityL = new double[composition.Count];
            fugacityV = new double[composition.Count];
            K = new double[composition.Count];

            Z = new double[composition.Count][];
            for (int i = 0; i < composition.Count; i++)
            {
                Z[i] = new double[6];
                nullZ(i);
            }

            discriminantZ = new double[composition.Count];
            Tr = new double[composition.Count];
            ac = new double[composition.Count];
            aT = new double[composition.Count];
            alpha = new double[composition.Count];
            b = new double[composition.Count];
            A = new double[composition.Count];
            B = new double[composition.Count];
            delta0 = new double[composition.Count];
            delta1 = new double[composition.Count];
            C = new double[composition.Count];
            Cc = new complex[composition.Count];
            //for (int i = 0; i < composition.Count; i++)
            //{
            //    Cc[i] = new complex(0, 0);
            //}

            Cp = new double[composition.Count];
            totalCp = 0;

            umolarL = new double[composition.Count];
            umolarV = new double[composition.Count];
            umolarideal = new double[composition.Count];

            acomp = new double[composition.Count]; //Cubic equation for compressibility factor coeficients.
            bcomp = new double[composition.Count]; //Cubic equation for compressibility factor coeficients.
            ccomp = new double[composition.Count]; //Cubic equation for compressibility factor coeficients.
            dcomp = new double[composition.Count]; //Cubic equation for compressibility factor coeficients.

            a = new double[composition.Count][];
            for (int i = 0; i < composition.Count; i++) { a[i] = new double[3]; }

            Qc = new double[composition.Count]; //used in alternative formula.
            Rc = new double[composition.Count]; //used in alternative formula.
            Dc = new double[composition.Count]; //used in alternative formula.
            Sc = new complex[composition.Count]; //used in alternative formula.
            Tc = new complex[composition.Count]; //used in alternative formula.

            zs = new complex[6]; //Used in scarlet cubic solver.
            us = new complex[2]; //Used in scarlet cubic solver.
            ys = new complex[6]; //Used in scarlet cubic solver.

            uvflashsize = 2 * composition.Count + 3;
            xvector = new double[uvflashsize];
            for (int i = 0; i < composition.Count; i++)
            {
                x[i] = 1.0 / (composition.Count); //Add 0.5 to get starting values that can converge.
                y[i] = 1.0 / (composition.Count);
            }
            if (composition.Count == 1) { uvflashsize = 3; } //x and y does not need to be part of the model anymore.
            origuvflashsize = uvflashsize;

            jacobian = new matrix(uvflashsize, uvflashsize);

            PTfVflash(aTemp, aV, aP, af);
            //calccompz(); Already done in the previous flash ptfv

        }

        public void setxybasedonf() //If f = 0 or f = 1, then the values of x and y can be preset based on z and full flashing is not needed
        {
            for (int i = 0; i < composition.Count; i++)
            {
                if (f.v == 0)
                {
                    x[i] = z[i];
                    y[i] = 0;
                }
                else if (f.v == 1)
                {
                    x[i] = 0;
                    y[i] = z[i];
                }
            }
        }

        public void PTfVflash(double aTemp, double aV, double aP, double af)
        {
            //composition.Clear();
            //for (int i = 0; i < acomposition.Count; i++)
            //{
            //    //if (global.fluidpackage[i].m.name == componentname)
            //    //{
            //    composition.Add(new component());

            //    composition[composition.Count - 1].copytothisobject(acomposition[i]);
            //    //componentindex = i;
            //    //}
            //}

            P.v = aP;
            T.v = aTemp;
            f.v = af;
            V.v = aV;
            calccompz();
            setxybasedonf();
            mapvarstox(); //Variables are allocated according to the Flatby article sequence of equations.
            //U.v = 10000000.0;
            //uvflash();
            vmolar = new controlvar(calcvmolaranddensity());
            n.v = (V.v / vmolar.v);
            //composition[0].n = n.v;  //ASSUMING ONLY ONE COMPONENT AT THE MOMENT.  Should not be needed anymore.

            //calcn(); //Should not be needed anymore.
            calcmass();
            //massofonemole = mass.v / n.v; Already caculated in calcvmolarandensity;

            umolar.v = calcumolar();
            U.v = n.v * umolar.v;
            calctotalCp();
           

        }

        public void nullZ(int j)
        {
            for (int i = 0; i < 3; i++) { Z[j][i] = global.ZNotDefined; }
        }



        //public void calcn() //This method could not be needed anymore.
        //{
        //    n.v = 0;
        //    for (int i = 0; i < composition.Count; i++)
        //    {
        //        n.v += composition[i].n;
        //    }
        //}

        //public void calcmassanddensity()
        //{
        //    calcn();
        //    massofonemole = 0;
        //    volumeofonemole = 0;
        //    double masscontribution, volumecontribution, moleof1kg = 0;
        //    for (int i = 0; i < composition.Count; i++)
        //    {
        //        moleof1kg += composition[i].massfraction / composition[i].m.molarmass;
        //    }
        //    for (int i = 0; i < composition.Count; i++)
        //    {
        //        composition[i].molefraction = composition[i].massfraction / composition[i].m.molarmass / moleof1kg;
        //        masscontribution = composition[i].molefraction * composition[i].m.molarmass;
        //        volumecontribution = masscontribution / (composition[i].m.density + 0.00001); //Adding a small number to negate 
        //        //very singularities.
        //        massofonemole += masscontribution;
        //        volumeofonemole += volumecontribution;
        //    }
        //    density.v = massofonemole / volumeofonemole;
        //}

        public void calcmass()
        {
            mass.v = 0.0;
            for (int i = 0; i < composition.Count; i++)
            {
                mass.v += n.v*composition[i].molefraction * composition[i].m.molarmass;
            }
        }

        public component match(string aname)
        {
            component c = null;
            for (int i = 0; i < composition.Count; i++)
            {
                if (composition[i].m.name == aname) { c = composition[i]; }
            }
            return c;

        }

        //public double totalmolefraction()
        //{
        //    double total = 0;
        //    for (int i = 0; i < composition.Count; i++) { total += composition[i].molefraction; } //This should now be adding up to 100%, if it
        //    //does not add up, then a scaling will need to
        //    //be done in order to have the total to be 100%
        //    return total;
        //}

        public void copycompositiontothisobject(material m)
        {
            for (int i = 0; i < composition.Count; i++)
            {
                composition[i].copytothisobject(m.composition[i]);
            }
        }

        public void mapvarstox() //Variables are allocated according to the Flatby article sequence of equations.
        {
            xvector[0] = T.v;
            xvector[1] = P.v;
            xvector[2] = f.v;
            for (int i = 0; i < composition.Count; i++)
            {
                xvector[3 + i] = x[i];
                xvector[3 + composition.Count + i] = y[i];
            }

        }

        public void mapxtovars() //Variables are allocated according to the Flatby article sequence of equations.
        {
            T.v = xvector[0];
            P.v = xvector[1];
            f.v = xvector[2];

            for (int i = 0; i < composition.Count; i++)
            {
                x[i] = xvector[3 + i];
                y[i] = xvector[3 + composition.Count + i];
            }

        }

        private complex calcxk(complex u, int j)
        {
            return -1.0 / (3.0 * acomp[j]) * (bcomp[j] + u * Cc[j] + delta0[j] / (u * Cc[j]));
        }

        public void calcZ(int j) //Calculate the roots of the compressibility equation for the particular component.
        {
            if (composition[j].m.omega <= 0.49)
            {
                K[j] = 0.37464 + 1.54226 * composition[j].m.omega - 0.26992 * Math.Pow(composition[j].m.omega, 2.0);
            }
            else
            {
                K[j] = 0.379642 + 1.48503 * composition[j].m.omega - 0.164423 * Math.Pow(composition[j].m.omega, 2.0) + 0.016666 * Math.Pow(composition[j].m.omega, 3.0);
            }
            Tr[j] = xvector[0] / composition[j].m.Tc; //T / composition[j].m.Tc;
            ac[j] = 0.45724 * Math.Pow(global.R, 2.0) * Math.Pow(composition[j].m.Tc, 2.0) / composition[j].m.Pc;
            alpha[j] = Math.Pow(1 + K[j] * (1 - Math.Sqrt(Tr[j])), 2.0);
            aT[j] = alpha[j] * ac[j];
            b[j] = 0.07780 * global.R * composition[j].m.Tc / composition[j].m.Pc;
            A[j] = aT[j] * xvector[1] / (Math.Pow(global.R, 2.0) * Math.Pow(xvector[0], 2.0)); //xvector[1]: P
            B[j] = b[j] * xvector[1] / (global.R * xvector[0]);

            acomp[j] = 1.0;
            bcomp[j] = (B[j] - 1.0);
            ccomp[j] = A[j] - 2.0 * B[j] - 3.0 * Math.Pow(B[j], 2.0);
            dcomp[j] = Math.Pow(B[j], 3.0) + Math.Pow(B[j], 2.0) - A[j] * B[j];

            //Delta = 18abcd -4b^3d + b^2c^2 - 4ac^3 - 27a^2d^2
            discriminantZ[j] = 18.0 * acomp[j] * bcomp[j] * ccomp[j] * dcomp[j] - 4.0 * Math.Pow(bcomp[j], 3.0) * dcomp[j] + Math.Pow(bcomp[j], 2.0) * Math.Pow(ccomp[j], 2.0) -
                4.0 * acomp[j] * Math.Pow(ccomp[j], 3.0) - 27.0 * Math.Pow(acomp[j], 2.0) * Math.Pow(dcomp[j], 2.0);

            //Delta_0 = b^2-3 a c
            //delta0[j] = Math.Pow(bcomp[j], 2.0) - 3.0 * acomp[j] * ccomp[j];

            //Delta_1 = 2 b^3-9 a b c+27 a^2 d
            //delta1[j] = 2.0 * Math.Pow(bcomp[j], 3.0) - 9.0 * acomp[j] * bcomp[j] * ccomp[j] + 27.0 * Math.Pow(acomp[j], 2.0) * dcomp[j];

            //C[j] = Math.Pow((delta1[j] + Math.Sqrt(Math.Pow(delta1[j], 2.0) - 4.0 * Math.Pow(delta0[j], 3.0))) / 2.0, 1.0 / 3.0);

            //Cc[j].a = Cc[j].b = 0.0;
            //complex Coper = new complex(Math.Pow(delta1[j], 2.0) - 4.0 * Math.Pow(delta0[j], 3.0), 0.0);
            //if (Coper < 0) { Cc[j].b = Math.Sqrt(-Coper); }
            //else { Cc[j].a = Math.Sqrt(Coper); }
            //Cc[j] = complex.Pow((delta1[j] - complex.Pow(Coper,0.5))/2.0, 1.0/3.0);

            nullZ(j);
            complex[] xk = new complex[3];
            //complex[] u = new complex[] {new complex(1,00), new complex(-0.5,Math.Pow(3,0.5)/2.0), new complex(-0.5, -Math.Pow(3,0.5)/2.0)};

            //Now an alternative way of getting the roots will be tried.  Wolfram algorithm.
            a[j][0] = dcomp[j] / acomp[j];
            a[j][1] = ccomp[j] / acomp[j];
            a[j][2] = bcomp[j] / acomp[j];
            //Qc[j] = (3.0 * a[j][1] - Math.Pow(a[j][2], 2.0)) / 9.0;
            //Rc[j] = (9.0 * a[j][2] * a[j][1] - 27.0 * a[j][0] - 2.0 * Math.Pow(a[j][2], 3.0)) / 54.0;
            //Dc[j] = Math.Pow(Qc[j], 3.0) + Math.Pow(Rc[j], 3.0);
            //Sc[j] = complex.pow(Rc[j] + complex.pow(Dc[j], 0.5), 1.0/3.0);
            //Tc[j] = complex.pow(Rc[j] - complex.pow(Dc[j], 0.5), 1.0/3.0);
            //xk[0] = -1.0 / 3.0 * a[j][2] + Sc[j] + Tc[j];
            //xk[1] = -1.0 / 3.0 * a[j][2] - 1.0 / 2.0 * (Sc[j] + Tc[j]) + 1.0 / 2.0 * complex.I * Math.Pow(3.0, 0.5) * (Sc[j] - Tc[j]);
            //xk[2] = -1.0 / 3.0 * a[j][2] - 1.0 / 2.0 * (Sc[j] + Tc[j]) - 1.0 / 2.0 * complex.I * Math.Pow(3.0, 0.5) * (Sc[j] - Tc[j]);

            //This is still another algorithm for the solution.  The scarlet algorithm.  http://home.scarlet.be/~ping1339/cubic.htm

            List<complex> xs = new List<complex>(); //Used in scarlet cubic solver.

            bs = a[j][2];
            cs = a[j][1];
            ds = a[j][0];
            rs = -bs / 3.0;

            //bs = 8.0/15.0;
            //cs = -7.0/45.0;
            //ds = -2.0/45.0;
            //rs = -bs / 3.0;

            //y^3  + (3 r + b) y^2  + (3 r^2  + 2 r b + c) y + r^3  + r^2  b + r c + d = 0
            //y^3  + e y + f = 0
            es = 3.0 * Math.Pow(rs, 2.0) + 2 * rs * bs + cs;
            fs = Math.Pow(rs, 3.0) + Math.Pow(rs, 2.0) * bs + rs * cs + ds;

            double bqz = fs; //The b term of the quadratic equation in z
            double cqz = -Math.Pow(es, 3.0) / 27.0;

            us[0] = (-bqz + complex.pow(Math.Pow(bqz, 2.0) - 4 * cqz, 0.5)) / (2.0);
            us[1] = (-bqz - complex.pow(Math.Pow(bqz, 2.0) - 4 * cqz, 0.5)) / (2.0);

            zs[0] = complex.pow(us[0], 1.0 / 3.0, 0);
            zs[1] = complex.pow(us[0], 1.0 / 3.0, 1);
            zs[2] = complex.pow(us[0], 1.0 / 3.0, 2);
            zs[3] = complex.pow(us[1], 1.0 / 3.0, 0);
            zs[4] = complex.pow(us[1], 1.0 / 3.0, 1);
            zs[5] = complex.pow(us[1], 1.0 / 3.0, 2);

            double temp = -es / 3.0;

            ys[0] = zs[0] - es / (3.0 * zs[0]);
            ys[1] = zs[1] - es / (3.0 * zs[1]);
            ys[2] = zs[2] - es / (3.0 * zs[2]);
            ys[3] = zs[3] - es / (3.0 * zs[3]);
            ys[4] = zs[4] - es / (3.0 * zs[4]);
            ys[5] = zs[5] - es / (3.0 * zs[5]);

            xs.Add(ys[0] - bs / 3.0);
            xs.Add(ys[1] - bs / 3.0);
            xs.Add(ys[2] - bs / 3.0);
            xs.Add(ys[3] - bs / 3.0);
            xs.Add(ys[4] - bs / 3.0);
            xs.Add(ys[5] - bs / 3.0);

            complexcomparer complexcompare = new complexcomparer();
            xs.Sort(complexcompare);

            for (int k = 1; k < xs.Count; k++)
            {
                if (Math.Round(xs[k].a, 6) == Math.Round(xs[k - 1].a, 6)) { xs.RemoveAt(k); }
            }

            //int nbelow05 = 0; //nr of roots that are below 0.5.
            //for (int k = 0; k < xs.Count; k++)
            //{
            //    if (xs[k].a < 0.5 && Math.Abs(xs[k].b) < global.ZeroImaginary) { nbelow05++; }
            //}
            if (xs.Count == 3) { xs.RemoveAt(1); } //Assume that the higher root value should be taken out.  To be verified.

            //End of Scarlet algorithm.




            int Zsvalid = 0;
            int Zsnotvalid = 0;
            int Zthatsvalid = 0;
            int Zthatsnotvalid = 0;
            int badZi = xs.Count - 1;
            int goodZi = 0;
            int rightiforZ;

            for (int k = 0; k < xs.Count; k++)
            {
                //xk[k] = calcxk(u[k], j);
                if (Math.Abs(xs[k].b) < global.ZeroImaginary && xs[k].a >= 0)
                {
                    Z[j][goodZi] = xs[k].a;
                    Zsvalid++;
                    Zthatsvalid = goodZi;
                    rightiforZ = goodZi;
                    //for (int l = goodZi - 1; l >= 0; l--)
                    //{
                    //    if (Z[j][l] > Z[j][goodZi]) {rightiforZ = l;}
                    //}
                    //for (int m = goodZi; m > rightiforZ; m--)
                    //{
                    //    Z[j][m] = Z[j][m - 1];
                    //}
                    //Z[j][rightiforZ] = xk[k].a;

                    goodZi++;
                }
                else
                {
                    Z[j][badZi--] = global.ZNotDefined;
                    Zsnotvalid++;

                }
            }
            if (Zsvalid > 1)
            {
                if (f.v == 0 || xvector[2] == 0)
                {
                    //f.v = 0.1;
                    //xvector[2] = f.v;
                }
                else if (f.v == 1 || xvector[2] == 1)
                {
                    //f.v = 0.9;
                    //xvector[2] = f.v;
                }
            }
            else if (Zsvalid == 1)
            {
                if (Z[j][Zthatsvalid] < 0.5 && composition.Count == 1)
                {
                    f.v = 0;
                    xvector[2] = f.v;
                } //only liquid
                else if (Z[j][Zthatsvalid] > 0.5 && composition.Count == 1)
                {
                    f.v = 1;
                    xvector[2] = f.v;
                } //only vapour
            }



            int dummy = 0;
            for (int k = 0; k < 3; k++)
            {
                if (Z[j][k] == global.ZNotDefined) { Z[j][k] = Z[j][Zthatsvalid]; }
            }



            //if (discriminantZ[j] < 0) //Should be the case if there is only one phase of the molecule present
            //{
            //    Z[j][0] = -1 / (3 * acomp[j]) * (bcomp[j] + C[j] + delta0[j] / C[j]);
            //    Z[j][1] = Z[j][0];
            //}
            //else
            //{
            //    Z[j][0] = (9*acomp[j]*dcomp[j] - bcomp[j]*ccomp[j])/(2*delta0[j]); //This should be the liquid root.
            //    Z[j][1] = (4*acomp[j]*bcomp[j]*ccomp[j] - 9*Math.Pow(acomp[j],2)*dcomp[j] - Math.Pow(bcomp[j],3))/(acomp[j]*delta0[j]); //This should be the gas root.
            //}
        }

        public void calcfugacity(int j) //assume that Z has already been calculated for both roots.  Z[j][0] is the liquid root, Z[j][1] is the gas root.
        {
            double[] logtheta = new double[] { 0, 0 };
            calcZ(j);
            for (int i = 0; i < 2; i++)
            {
                if (Z[j][i] != global.ZNotDefined)
                {
                    logtheta[i] = (Z[j][i] - 1.0) - Math.Log(Z[j][i] - B[j]) -
                        A[j] / (2.0 * Math.Sqrt(2.0) * B[j]) * Math.Log((Z[j][i] + (Math.Sqrt(2.0) + 1.0) * B[j]) / (Z[j][i] - (Math.Sqrt(2.0) - 1.0) * B[j]));
                }
            }
            fugacityL[j] = Math.Exp(logtheta[0]) * xvector[1];
            fugacityV[j] = Math.Exp(logtheta[1]) * xvector[1];
            //if (Z[j][0] == global.ZNotDefined) {fugacityL[j] = fugacityV[j];}
            //else if (Z[j][1] == global.ZNotDefined) { fugacityV[j] = fugacityL[j]; }
        }

        public double calcumolarwithZ(int j, int Zi) //calcs the molar internal energy for the given root of the Z equation, and for component j.
        {
            return -A[j] / (B[j] * Math.Sqrt(8.0)) * (1.0 + K[j] * Math.Sqrt(Tr[j]) / Math.Sqrt(alpha[j])) *
                Math.Log((Z[j][Zi] + (1.0 + Math.Sqrt(2.0)) * B[j]) / (Z[j][Zi] + (1.0 - Math.Sqrt(2.0)) * B[j])) * global.R * xvector[0] +    //xvector[0]: T
                umolarideal[j];
        }

        public void calcumolarpercomponent()
        {
            for (int j = 0; j < composition.Count; j++)
            {
                calcZ(j);
                Cp[j] = composition[j].m.calcCp(xvector[0]);
                umolarideal[j] = (Cp[j] - global.R) * xvector[0];
                //umolarideal[j] = 0.0;
                umolarL[j] = calcumolarwithZ(j, 0);
                umolarV[j] = calcumolarwithZ(j, 1);
            }
        }

        public void calctotalCp()
        {
            calccompz();
            totalCp = 0;
            for (int i = 0; i < composition.Count; i++)
            {
                totalCp += Cp[i]*z[i];
            }
        }

        public double calcumolar()
        {
            calcumolarpercomponent();

            double totalumolarL = 0.0;
            double totalumolarV = 0.0;
            for (int k = 0; k < composition.Count; k++)
            {
                totalumolarL += umolarL[k] * xvector[3 + k]*composition[k].molefraction;
                totalumolarV += umolarV[k] * xvector[3 + composition.Count + k] * composition[k].molefraction;
            }
            totalumolar = (1 - xvector[2]) * totalumolarL + xvector[2] * totalumolarV; //f: xvector[2]
            return totalumolar;
        }

        public double calcvmolaranddensity()
        {
            double totalvmolar = 0.0;
            double totalvmolarL = 0.0;
            double totalvmolarV = 0.0;
            massofonemole = 0.0;
            for (int k = 0; k < composition.Count; k++)
            {
                calcZ(k);
                totalvmolarL += b[k] * Z[k][0] / B[k] * xvector[3 + k] * composition[k].molefraction;
                totalvmolarV += b[k] * Z[k][1] / B[k] * xvector[3 + composition.Count + k] * composition[k].molefraction;
                //totalvmolarL += Z[k][0] * global.R * xvector[0] / (xvector[1] + 0.0000001) * xvector[3 + k];
                //totalvmolarV += Z[k][1] * global.R * xvector[0] / (xvector[1] + 0.0000001) * xvector[3 + composition.Count + k];
                massofonemole += composition[k].m.molarmass * composition[k].molefraction;
            }
            totalvmolar = (1 - xvector[2]) * totalvmolarL + xvector[2] * totalvmolarV;
            density.v = massofonemole / totalvmolar;  
            return totalvmolar;
        }

        private double dfdx(int fi, int xi)
        {
            double y0 = fflash(fi);
            double x0 = xvector[xi];
            //xvector[xi] *= global.epsilonfrac;
            xvector[xi] += global.epsilonadd;
            double den = xvector[xi] - x0;
            double y1 = fflash(fi);
            xvector[xi] = x0;
            return (y1 - y0) / den;
        }

        private double fflash(int i) //function that will define the functions to be solved for the UV flash
        {
            double ffreturn = 0.0; //the value that will be returned.
            if (i == 0) //internal molar energy equation.
            {
                ffreturn = umolar.v - calcumolar(); //f: xvector[2]
                return ffreturn;
            }
            else if (i == 1) //molar volume equation
            {
                //vmolar = calcvmolar(); //We are nulling this equation just for this particular case now.
                ffreturn = vmolar.v - calcvmolaranddensity();
                return ffreturn;
            }
            else if (i >= 2 && i < composition.Count + 2) //fugacity equation
            {
                calcfugacity(i - 2);
                if (xvector[2] == 0.0) { ffreturn = 0.0; }
                else
                {
                    ffreturn = xvector[3 + composition.Count + i - 2] * fugacityV[i - 2] -
                        xvector[3 + i - 2] * fugacityL[i - 2];
                } //y[j]*fl[j] - x[j]*fv[j] }
                return ffreturn;

            }

            else if (i >= composition.Count + 2 && i < 2 * composition.Count + 2)  //(1 - f)*x[j] + f*y[j] - z[j] 
            {
                ffreturn = (1 - xvector[2]) * xvector[3 + (i - composition.Count - 2)] +
                    xvector[2] * xvector[3 + composition.Count + (i - composition.Count - 2)] - z[i - composition.Count - 2];
                return ffreturn;
            }
            else if (i == 2 * composition.Count + 2) //composition sum
            {
                double sum = 0;
                for (int k = 0; k < composition.Count; k++)
                {
                    sum += xvector[3 + composition.Count + k] - xvector[3 + k];
                }
                return sum;
            }

            else { return 0; }

        }

        public void calcjacobian()
        {
            for (int r = 0; r < uvflashsize; r++)
            {
                for (int c = 0; c < uvflashsize; c++)
                {
                    jacobian.m[r][c] = dfdx(r, c);
                }
            }
        }

        private void limitxvector()
        {
            if (xvector[0] < 0.0) { xvector[0] = 0.1; } //T
            if (xvector[1] < 0.0) { xvector[1] = 0.1; } //P
            if (xvector[2] < 0.0) { xvector[2] = 0.0; } //f
            else if (xvector[2] > 1.0) { xvector[2] = 1.0; }
        }

        private void calccompz() //method to calculate the molar fraction of each component in the material
        {
            for (int i = 0; i < composition.Count; i++)
            {
                z[i] = composition[i].molefraction;
            }
        }

        public void uvflash()
        {
            matrix lmatrix = new matrix(uvflashsize, uvflashsize);
            matrix umatrix = new matrix(uvflashsize, uvflashsize);
            matrix deltax = new matrix(uvflashsize, 1);
            matrix ymatrix = new matrix(uvflashsize, 1);
            matrix bmatrix = new matrix(uvflashsize, 1);

            mapvarstox();
            //calcn();  This calcn will need to come in later when more than once component can be better simulated by the simulation.
            umolar.v = U.v / n.v;
            vmolar.v = V.v / n.v;

            calccompz();

            for (int i = 0; i < global.NMaterialIterations; i++)
            {
                if (xvector[2] == 0.0 || xvector[2] == 1.0)
                {
                    uvflashsize = 2;
                }
                else { uvflashsize = origuvflashsize; }

                jacobian.resize(uvflashsize, uvflashsize);
                lmatrix.resize(uvflashsize, uvflashsize);
                umatrix.resize(uvflashsize, uvflashsize);
                deltax.resize(uvflashsize, 1);
                ymatrix.resize(uvflashsize, 1);
                bmatrix.resize(uvflashsize, 1);

                calcjacobian();
                //jacobian.swoprowsinthematrix(1, 3);
                //jacobian.ludecomposition(lmatrix, umatrix);
                //jacobian.swoprowsinthematrix(2, 4);
                jacobian.ludecomposition(lmatrix, umatrix);
                matrix tempm = lmatrix * umatrix;
                for (int j = 0; j < uvflashsize; j++)
                {
                    bmatrix.m[j][0] = -fflash(j);
                }

                matrix.solveLYequalsB(lmatrix, ymatrix, bmatrix);
                matrix tempm2 = lmatrix * ymatrix;
                matrix.solveUXequalsY(umatrix, deltax, ymatrix);
                matrix tempm3 = umatrix * deltax;
                for (int j = 0; j < uvflashsize; j++)
                {
                    xvector[j] += deltax.m[j][0];
                }
                limitxvector();
                f.v = xvector[2];
            }
            mapxtovars();
        }

        //public void addtothisobject(material m)
        //{
        //    for (int i = 0; i < composition.Length; i++)
        //    {
        //        composition[i].massfraction += m.composition[i].massfraction;
        //    }
        //}

        public void zero()
        {
            for (int i = 0; i < composition.Count; i++)
            {
                composition[i].n = 0;
            }
        }

        public void update(int simi, bool historise)
        {
            if (historise && (simi % global.SimVectorUpdatePeriod == 0))
            {
                if (T.simvector != null) { T.simvector[simi/global.SimVectorUpdatePeriod] = T.v; }


                if (P.simvector != null) { P.simvector[simi / global.SimVectorUpdatePeriod] = P.v; }


                if (f.simvector != null) { f.simvector[simi / global.SimVectorUpdatePeriod] = f.v; }


                if (n.simvector != null) { n.simvector[simi / global.SimVectorUpdatePeriod] = n.v; }


                if (U.simvector != null) { U.simvector[simi / global.SimVectorUpdatePeriod] = U.v; }


                if (density.simvector != null) { density.simvector[simi / global.SimVectorUpdatePeriod] = density.v; }
                
            }
        }

        
    }
}
