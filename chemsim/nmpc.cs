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
    public class nmpc : baseclass   //nonlinear model predictive control
    {
        public int callingsimi; //The simulation index in the calling simulation object.

        public nmpcalgorithm algorithm; //The NMPC algorithm that will be used in this controller

        public List<mpcvar> cvmaster; //This List needs to point to mastersim
        //public List<mpcvar> mvsim0; //This List needs to point to sim0
        //public List<mpcvar> mvsim1; //This List needs to point to sim1;
        public List<mpcvar> mvmaster; //The master sim's mapping should be to this list.
        public List<mpcvar> mvboolmaster; //The master sim's boolean mpc variables will be stored here.  This is to enable hybrid nmpc.

        
        public int N; //Optimisation horizon
        
        public int initialdelay; //The amount of iterations of the total simulation from T0 to the first execution of nmpc.
        public int runinterval; //The amount of iterations of the total simulation between runs of the NMPC.
        public simulation mastersim; //This will be copied from the master sim each time update method is run initiated.
        public simulation sim0, sim1; //pointers to the main simulation will need to be maintained in the NMPC in order to run the 
                               //simulateplant method from within this class.  sim0 will move with update method, and sim1 will 
                               //move with each J calc.
        //public double J; //Last objective function run value.
        public double J0; //The objective function value at the start of the update() method.
        public double mk; //the value of the model that is to be minimised.
        public double alphak; //the fraction of the calculated step that will actually be implemented.
        public double[] jacobianmk; //The Jacobian of the Objective function with each row begin the derivitive of mk with respect to that MV.
        public double[] jacobian;
        public double[] hessian;
        public double[][] mvmatrix; //Record of trajectory of MVs per update iteration.

        public int systemhasbool; //True (1) if there are boolean mvs in the system.

        //properties for the Interior point 1 ConstrainedLineSearch algorithm ------------------------------------------------------------------------------------------
        public int nrinequalityconstraints;
        public double interiorpointerror;
        public matrix constraintvectorlinesearch; //The vector of the cI(x) function, that will reflect the distance from zero for each 
                                          //constraint: CI(x) >= 0
        public matrix IDmatrixinterior; //Identity matrix;
        public matrix AInequality; //matrix of the x derivative of the contrant function vector.
        public matrix svector; //slack variable vector.
        public matrix Smatrix; //slack variable matrix.
        public matrix zvector; //Lagrange multiplier for inequality constraints.
        public matrix Zmatrix; //Lagrange matrix.
        public double mubarrier; //mu value barrier parameter in the algorithm.
        public double nmpcsigma; //Multiplied with mubarrier at the end of each iteration.
        public matrix hessianconstrainedlinesearch;
        public matrix jacobianconstrainedvectorlinesearch;
        public matrix jacobianconstrainedvectorlinesearchbase; //the one at the start of the controller iteration that will be used to compare against.
        public matrix Amatrixconstrainedlinesearch; //This is the A matrix of the AX = B primal dual system.
        public matrix Bvectorconstrainedlinesearch; //This is the B vector of the AX = B primal dual system.
        int sizeprimedual;
        public matrix deltaoptim; //vector of delta for mvs, s and z vectors.

        //Properties for the Active Set algorithm 1. -------------------------------------------------------------------------------------------------
        public List<int> constraintaccountingvector; //The indices of the active constraints.
        //public matrix fullconstraintvectoractiveset1; //The total constraint vector with rows zero for ones that are not in the problem.
        public matrix fulllambdaactiveset1; //The full multiplier vector with rows zero for constraints that are not in the problem at a particular point
                                                    //in time.
        public matrix activeconstraintvector0; //Constraints in the problem at a particular point in time, for active set 1 algorithm.
        public matrix activeconstraintvector1; //A temp version of the previous that can be set equal to the previous at some point when needed.
        public matrix activelambdaactiveset1; //Lagrangian multiplier for Active Set 1 algorithm.
        public int nractiveconstraints;
        public matrix jacobianactiveset1;
        public matrix Amatrixactiveset1; //This is the A matrix of the AX = B active set 1 system.
        public matrix Bvectoractiveset1; //This is the B vector of the AX = B active set 1 system.
        public int sizeactiveset1; //Row size of A and B matrices.

        //Properties for the Genetic Algorithm 1 -----------------------------------------------------------------------------------------------------
        public int nrchromosomes; //The total number of total solutions that will be kept in memory each iteration.
        public int nrsurvivingchromosomes; //Nr of chromosomes that will be passed to the next iteration and not replaced by new random ones.
        public int nrpairingchromosomes; //The nr of chromosomes of the total population that will be pairing and producing children.
        public int chromosomelistsize; //Total memory to be allocated.
        public double defaultprobabilityofmutation; //The probability that a child will be mutated in one bit.
        public int nriterations; //The number of iterations until the best GA solution will be passed to the update method.
        public int crossoverpoint; //Bit index nr (starting from zero) from right to left in the binary representation, where
                                            //cross over and mutation will start.

        private Random rand;

        public List<chromosome> chromosomes; //The solutions that are kept in memory for the GA.  This list will be sorted in each iteration.
        private chromosomecomparerfit comparerfitness;
        private IComparer<chromosome> chromosomecomparer;

        //Properties for the Particle Swarm Optimisation 1 -----------------------------------------------------------------------------------------------------
        //public int nrparticles;
        public int nrcontinuousparticles; //The total number of total continuous solutions that will be kept in memory each iteration.
        public int bestsolutioncontinuousparticleindex; //The particle that has had the best solution to the problem so far.
        public int nrbooleanparticles; //The total number of total continuous solutions that will be kept in memory each iteration.
        public int bestsolutionbooleanparticleindex; //The particle that has had the best solution to the problem so far.
        public double bestfitnesscontinuous; //the fitness of the best particle historically.
        public double bestfitnessboolean; //the fitness of the best particle historically.
        public List<particle> continuousparticles;
        public List<particle> booleanparticles;
        public double[] mvboolinterrim; //An internal class array used for interrim storage of the boolean mvs of various particles.

        public nmpc(int anr, double ax, double ay, simulation asim)
            : base(anr, ax, ay)
        {
            objecttype = objecttypes.NMPC;

            name = nr.ToString() + " " + objecttype.ToString();

            algorithm = global.DefaultNMPCAlgorithm;

            callingsimi = 0;
            cvmaster = new List<mpcvar>();
            //mvsim0 = new List<mpcvar>();
            //mvsim1 = new List<mpcvar>();
            mvmaster = new List<mpcvar>();
            mvboolmaster = new List<mpcvar>();
            systemhasbool = 0;
            N = global.DefaultN;
            initialdelay = global.DefaultInitialDelay;
            runinterval = global.DefaultRunInterval;
            mastersim = asim;
            sim0 = new simulation(asim);
            sim1 = new simulation(asim);
            J0 = 0;
            alphak = global.Defaultalphak;

            

            nmpcsigma = global.DefaultNMPCSigma;

            rand = new Random();

            initjacobian();
        }

        public void initjacobian() //This function is used as an Init method for the class, and also to update the class from
                                   // the dialogue of the properties class.
        {
            jacobian = new double[mvmaster.Count];
            hessian = new double[mvmaster.Count];
            jacobianmk = new double[mvmaster.Count];
            mvmatrix = new double[mvmaster.Count][];
            
            for (int i = 0; i < mvmaster.Count; i++)
            {
                mvmatrix[i] = new double[N];
            }

            //Interior Point ConstrainedLineSearch algorithm:
            nrinequalityconstraints = mvmaster.Count*2;
            interiorpointerror = double.MaxValue;
            IDmatrixinterior = matrix.identitymatrix(nrinequalityconstraints);
     
            svector = new matrix(nrinequalityconstraints, 1);
            Smatrix = new matrix(nrinequalityconstraints, nrinequalityconstraints); //slack variable matrix.
            constraintvectorlinesearch = new matrix(nrinequalityconstraints, 1);
            AInequality = new matrix(nrinequalityconstraints, mvmaster.Count); //matrix of the x derivative of the contrant function vector.
            calcconstraintvector();
            calcAInequality();
            zvector = new matrix(nrinequalityconstraints, 1);
            Zmatrix = new matrix(nrinequalityconstraints, nrinequalityconstraints); //Lagrange matrix.
            for (int i = 0; i < nrinequalityconstraints; i++)
            {
                svector.m[i][0] = constraintvectorlinesearch.m[i][0] * global.DefaultsvalueMultiplier;
                zvector.m[i][0] = mubarrier / (svector.m[i][0] + global.Epsilon);
            }
            updateSmatrix();
            updateZmatrix();
            mubarrier = global.DefaultMuBarrier;
            sizeprimedual = mvmaster.Count + nrinequalityconstraints * 2;
            Amatrixconstrainedlinesearch = new matrix(sizeprimedual, sizeprimedual);
            Bvectorconstrainedlinesearch = new matrix(sizeprimedual, 1);
            jacobianconstrainedvectorlinesearch = new matrix(mvmaster.Count,1);
            jacobianconstrainedvectorlinesearchbase = new matrix(mvmaster.Count, 1);
            hessianconstrainedlinesearch = new matrix(mvmaster.Count, mvmaster.Count);
            deltaoptim = new matrix(sizeprimedual, 1);

            //Active Set algorithm 1 :
            sizeactiveset1 = mvmaster.Count + nrinequalityconstraints;
            constraintaccountingvector = new List<int>(nrinequalityconstraints); 
            //fullconstraintvectoractiveset1 = new matrix(nrinequalityconstraints, 1); 
            fulllambdaactiveset1 = new matrix(nrinequalityconstraints, 1);
            activeconstraintvector0 = new matrix(nrinequalityconstraints, 1);
            activeconstraintvector1 = new matrix(nrinequalityconstraints, 1);
            activelambdaactiveset1 = new matrix(nrinequalityconstraints, 1);
            jacobianactiveset1 = new matrix(mvmaster.Count, 1);
            Amatrixactiveset1 = new matrix(sizeactiveset1, sizeactiveset1);
            Bvectoractiveset1 = new matrix(sizeactiveset1, 1);
            nractiveconstraints = 0;

            //Genetic Algorithm 1 :
            if (algorithm == nmpcalgorithm.GeneticAlgorithm1) { initgeneticalgorithm1(); }

            //Particle Swarm Optimisation 1:
            if (algorithm == nmpcalgorithm.ParticleSwarmOptimisation1) { initparticleswarmoptimisation1(); }
        }

        


        //Unconstrained optimisation -------------------------------------------------------------------------------------------------------------

        private double calcobjectivefunction() //The objective function will always be calced with sim1
        {
            double function = 0;
            sim1.copyfrom(mastersim);
            for (int i = 0; i < N; i++) //i can be started here at index as well.  But I thought it might be best to start it at zero to get a better jacobian calc done all through the opimisation horizon.
            {
                mastersim.simulateplant(false);
                for (int j = 0; j < cvmaster.Count; j++)
                {
                    function += cvmaster[j].weight / cvmaster[j].target.simvector[callingsimi + i] *
                        Math.Pow(cvmaster[j].var.v - cvmaster[j].target.simvector[callingsimi + i], 2);  //The cv vector will have to poin to sim1's variables.
                }
                mastersim.simi++;
            }
            mastersim.copyfrom(sim1);
            return function;
        }

        private void calcjacobian()
        {
            
            
            //J = calcobjectivefunction();
            //mastersim.copyfrom(sim1);
            double JT1 = 0; //This will be J value after each MV move.
            double oldmv = 0;
            for (int i = 0; i < mvmaster.Count; i++)
            {
                //sim1.copyfrom(mastersim);
                oldmv = mvmaster[i].var.v;
                double h = mvmaster[i].var.v * global.limitjacnmpc; ;
                mvmaster[i].var.v += h;
                JT1 = calcobjectivefunction();
                
                jacobian[i] = (JT1 - J0) / h;
                mvmaster[i].var.v = oldmv;
                
            }
            //J = JT0;
        }

        private void calchessian() //this assumes that the jacobian has just been calculated, and it will be used in this calc.  
                                   //Hessian will be calculated by stepping back the mv, where jacobian stepped it forward, and then calculating rate of 
                                   //change of slopes.
        {
            sim1.copyfrom(mastersim);
            double JT1 = 0; //This will be J value after each MV move.
            double oldmv = 0;
            for (int i = 0; i < mvmaster.Count; i++)
            {
                //sim1.copyfrom(mastersim);
                oldmv = mvmaster[i].var.v;
                double h = mvmaster[i].var.v * global.limitjacnmpc;
                mvmaster[i].var.v -= h; //in the oposite direction as for first Jacobian calc.
                JT1 = calcobjectivefunction();
                
                hessian[i] = (J0 - JT1) / h; //Temporary variable for storing this value, to calc full Hessian in next line.
                hessian[i] = (jacobian[i] - hessian[i]) / h;
                mvmaster[i].var.v = oldmv;

            }

        }

        private void calcmk(double dk) //this is the model as specified in unconstrained optimisation in nmpc book
        {
            


        }

        private void calcjacobianmk()
        {
            calcjacobian();
            calchessian();
            for (int i = 0; i < mvmaster.Count; i++)
            {
                //sim1.copyfrom(mastersim);
                double dk = mvmaster[i].var.v * global.limitjacnmpc;
                jacobianmk[i] = (dk*jacobian[i] + 0.5*dk*hessian[i]*dk) / dk;
            }
        }


        //Interior Point 1 ------------------ Contrained Line Search -----------------------------------------------------------------------------------------

        private double calclagrangianconstrainedlinesearch()
        {
            matrix mat;
            double d;
            double predictweight = 0; //Weight to be applied to each step until N.

            double lagrangian = 0;
            double barrier = 0;
            sim1.copyfrom(mastersim);
            calcconstraintvector(); //This is for the mvs, if there are cvs also that have constraints, then this will need to be in the loop below.
            int phorison = (sim1.simi + N > global.SimIterations) ? global.SimIterations - sim1.simi : N;
            for (int i = 0; i < phorison; i++) //i can be started here at index as well.  But I thought it might be best to start it at zero to get a better jacobian calc 
                                        //done all through the opimisation horizon.
            {
                mastersim.simulateplant(false);
                if (i < phorison) //(i == phorison - 1) //only have final value at the moment at the end of the phorison
                {
                    predictweight = (i < phorison - 1) ? global.NMPCIPWeightPreTerm/N : global.NMPCIPWeightTerminal;
                    
                    for (int j = 0; j < cvmaster.Count; j++)
                    {
                        lagrangian += predictweight*cvmaster[j].weight *
                            Math.Pow((cvmaster[j].var.v - cvmaster[j].target.simvector[callingsimi + i])/(cvmaster[j].max - cvmaster[j].min + global.Epsilon), 2);  //The cv vector will have to poin to sim1's variables.
                    }
                    for (int j = 0; j < nrinequalityconstraints; j++)
                    {
                        barrier += Math.Log(constraintvectorlinesearch.m[j][0]);
                    }
                    lagrangian += -mubarrier * barrier;
                }

                //mat = matrix.transpose(zvector) * (constraintvectorlinesearch - svector); //as long as the constraints are only mvs, this step can 
                //be put before the loop, but if there are cvs in the 
                //constraints, then this needs to be moved earlier.
                //d = mat.m[0][0]; //mat will be a 1x1 matrix.
                //lagrangian += -d;
                mastersim.simi++;
            }
            
            mastersim.copyfrom(sim1);
            return lagrangian;
        }

        private void selectbestnodeip1() //For choosing teh best branch and bound node for hybrid.  And also implementing it.
        {
            double bestlagriangian = double.MaxValue;
            double permutationlagrangian = 0;
            double[] bestmvboolmaster = new double[mvboolmaster.Count];
            

            //for (int i = 0; i < mvboolmaster.Count; i++) { mvboolmaster[i].var.v = 0; }
            
            for (int i = 0; i < Math.Pow(2,mvboolmaster.Count); i++)
            {
                string boolstring = "";        

                string interrimstring = Convert.ToString(i, 2);
                boolstring = interrimstring.PadLeft(mvboolmaster.Count, '0');

                for (int j = 0; j < mvboolmaster.Count; j++)
                {
                    mvboolmaster[j].var.v = (boolstring[j].Equals('0')) ? mvboolmaster[j].min : mvboolmaster[j].max;
                }

                permutationlagrangian = calclagrangianconstrainedlinesearch();
                if (permutationlagrangian < bestlagriangian)
                {
                    for (int j = 0; j < mvboolmaster.Count; j++)
                    {
                        bestmvboolmaster[j] = mvboolmaster[j].var.v;
                    }
                    bestlagriangian = permutationlagrangian;
                }

            }

            for (int i = 0; i < mvboolmaster.Count; i++) { mvboolmaster[i].var.v = bestmvboolmaster[i]; }
        }

        private void calcjacobianconstrainedlinesearch(int nrrows)
        {
            double LT0 = calclagrangianconstrainedlinesearch();
            double LT1 = 0; //This will be J value after each MV move.
            double oldmv = 0;
            for (int i = 0; i < nrrows; i++)
            {
                //sim1.copyfrom(mastersim);
                oldmv = mvmaster[i].var.v;
                double h = (mvmaster[i].max - mvmaster[i].min) * global.limitjacnmpc; ;
                mvmaster[i].var.v += h;
                LT1 = calclagrangianconstrainedlinesearch();

                jacobianconstrainedvectorlinesearch.m[i][0] = (LT1 - LT0) / h;
                mvmaster[i].var.v = oldmv;
                //calcconstraintvector(); //THIS CAN BE OPTIMISED so that not all the constraints and MVs have to be evaluated each time.
            }
        }

        private void calchessianconstrainedlinesearch() //will calculate the Hessian of the Lagrangian, in the case of constrained line search. Only one half is triangle (half) is calced, 
                                                        //as the other half is an exact transpose of it.
        {
            //calcjacobianconstrainedlinesearch(mvmaster.Count);
            matrix JT0 = new matrix(jacobianconstrainedvectorlinesearchbase);
                
            double HT1 = 0; //This will be J value after each MV move.
            double oldmv = 0;
            for (int c = 0; c < mvmaster.Count; c++)
            {
                //sim1.copyfrom(mastersim);
                oldmv = mvmaster[c].var.v;
                double h = (mvmaster[c].max - mvmaster[c].min) * global.limitjacnmpc; ;
                mvmaster[c].var.v += h;
                calcjacobianconstrainedlinesearch(c + 1);

                for (int r = 0; r <= c; r++)
                {
                    hessianconstrainedlinesearch.m[r][c] = (jacobianconstrainedvectorlinesearch.m[r][0] - JT0.m[r][0]) / h;
                }

                mvmaster[c].var.v = oldmv;
                //calcconstraintvector(); //THIS CAN BE OPTIMISED so that not all the constraints and MVs have to be evaluated each time.
            }

            for (int c = 0; c < mvmaster.Count - 1; c++)
            {
                for (int r = c + 1; r < mvmaster.Count; r++)
                {
                    hessianconstrainedlinesearch.m[r][c] = hessianconstrainedlinesearch.m[c][r];
                }
            }

            
        }

        private void calcBvectorconstrainedlinesearch()
        {
            calcjacobianconstrainedlinesearch(mvmaster.Count);
            jacobianconstrainedvectorlinesearchbase.copyfrom(jacobianconstrainedvectorlinesearch);
            updateSmatrix();
            calcconstraintvector();
            matrix mat1 = jacobianconstrainedvectorlinesearchbase - matrix.transpose(AInequality) * zvector;
            matrix mat2 = Smatrix * zvector - mubarrier * IDmatrixinterior;
            matrix mat3 = constraintvectorlinesearch - svector;
            interiorpointerror = Math.Max(matrix.euclideannorm(mat1), Math.Max(matrix.euclideannorm(mat2), matrix.euclideannorm(mat3)));
            for (int r = 0; r < mvmaster.Count; r++)
            {
                Bvectorconstrainedlinesearch.m[r][0] = mat1.m[r][0];
            }

            for (int r = 0; r < nrinequalityconstraints; r++)
            {
                Bvectorconstrainedlinesearch.m[mvmaster.Count + r][0] = mat2.m[r][0];
            }

            for (int r = 0; r < nrinequalityconstraints; r++)
            {
                Bvectorconstrainedlinesearch.m[mvmaster.Count + nrinequalityconstraints + r][0] = mat3.m[r][0];
            }

        }

        private void calcAmatrixconstrainedlinesearch()
        {
            //calcBvectorconstrainedlinesearch();
            calchessianconstrainedlinesearch();

            matrix l = new matrix(mvmaster.Count, mvmaster.Count);
            matrix d = new matrix(mvmaster.Count,mvmaster.Count);
            matrix.choleskyLDLT(hessianconstrainedlinesearch, l, d);
            for (int i = 0; i < mvmaster.Count; i++)
            {
                if (d.m[i][i] < global.CholeskyDelta) {d.m[i][i] = global.CholeskyDelta;}
                //else 
                //{
                //    double maxvalue = 
                //}
            }
            hessianconstrainedlinesearch = l * d * matrix.transpose(l);
            updateZmatrix();
            //matrix Bvectorbase = new matrix(Bvectorconstrainedlinesearch);
            //double oldmv = 0;
            for (int r = 0; r < mvmaster.Count; r++)
            {
                for (int c = 0; c < mvmaster.Count; c++)
                {
                    Amatrixconstrainedlinesearch.m[r][c] = hessianconstrainedlinesearch.m[r][c];
                }

                for (int c = 0; c < nrinequalityconstraints; c++)
                {

                    Amatrixconstrainedlinesearch.m[r][c + mvmaster.Count + nrinequalityconstraints] = -1 * AInequality.m[c][r]; //note the transpose of AIineq by swapping c and r.

                }
            }

            for (int r = 0; r < nrinequalityconstraints; r++)
            {
                for (int c = 0; c < nrinequalityconstraints; c++)
                {
                    Amatrixconstrainedlinesearch.m[r + mvmaster.Count][c + mvmaster.Count] = Zmatrix.m[r][c];
                    Amatrixconstrainedlinesearch.m[r + mvmaster.Count][c + mvmaster.Count + nrinequalityconstraints] = Smatrix.m[r][c];
                    Amatrixconstrainedlinesearch.m[r + mvmaster.Count + nrinequalityconstraints][c + mvmaster.Count] = -1*IDmatrixinterior.m[r][c];
                }

                for (int c = 0; c < mvmaster.Count; c++)
                {
                    Amatrixconstrainedlinesearch.m[r + mvmaster.Count + nrinequalityconstraints][c] = AInequality.m[r][c];
                }

            }
           
        }

        public void updateSmatrix()
        {
            for (int i = 0; i < nrinequalityconstraints; i++)
            {
                Smatrix.m[i][i] = svector.m[i][0];
            }
        }

        public void updateZmatrix()
        {
            for (int i = 0; i < nrinequalityconstraints; i++)
            {
                Zmatrix.m[i][i] = zvector.m[i][0];
            }
        }

        public void calcAInequality()
        {
            calcconstraintvector();
            matrix C0 = new matrix(constraintvectorlinesearch);
            
            double oldmv = 0;
            
                for (int c = 0; c < mvmaster.Count; c++)
                {
                    oldmv = mvmaster[c].var.v;
                    double h = (mvmaster[c].max - mvmaster[c].min) * global.limitjacnmpc; ;
                    mvmaster[c].var.v += h;
                    calcconstraintvector();

                    for (int r = 0; r < mvmaster.Count; r++)
                    {

                        AInequality.m[r][c] = (constraintvectorlinesearch.m[r][0] - C0.m[r][0]) / h;
                        AInequality.m[r + mvmaster.Count][c] = (constraintvectorlinesearch.m[r + mvmaster.Count][0] - C0.m[r + mvmaster.Count][0]) / h;
                    }
                    mvmaster[c].var.v = oldmv;
                    //calcconstraintvector();
                }
               

            
        }

        public void calcconstraintvector()
        {
            // At this point we will assume that the MVs are the only variables with constraints.  They will be included in the vector.
            //The minimum constraints will be defined first, and then the maximum.

            for (int i = 0; i < mvmaster.Count; i++)
            {
                constraintvectorlinesearch.m[i][0] = mvmaster[i].var.v - mvmaster[i].min;
                constraintvectorlinesearch.m[i + mvmaster.Count][0] = mvmaster[i].max - mvmaster[i].var.v;
            }
        }


        //update methods  ------------------------------------------------------------------------------------------------------------------------------

        public void updatetarget(int simi)
        {
            for (int i = 0; i < cvmaster.Count; i++)
            {
                if (cvmaster[i].target.datasource == datasourceforvar.Exceldata)
                {
                    int j = (simi >= cvmaster[i].target.excelsource.data.Length) ? cvmaster[i].target.excelsource.data.Length - 1 : simi;
                    cvmaster[i].target.v = cvmaster[i].target.excelsource.data[j];
                }
            }
        }

        public void updateinteriorpoint1()
        {
            double news, newz; //new values for the interior point method iteration.
            double scompare, zcompare; //values to compare the new s and z against.
            double maxmove = 0;
            double absdeltaoptim = 0;

            selectbestnodeip1();
            if (mvmaster.Count > 0)
            {
                calcBvectorconstrainedlinesearch();
                calcAmatrixconstrainedlinesearch();
                if (interiorpointerror >= global.DefaultIPErrorTol)
                {
                    matrix Bvector = -1 * Bvectorconstrainedlinesearch;
                    matrix.solveAXequalsB(Amatrixconstrainedlinesearch, deltaoptim, Bvector);

                    deltaoptim = alphak * deltaoptim;

                    for (int r = 0; r < mvmaster.Count; r++)
                    {
                        maxmove = global.MVMaxMovePerSampleTT0 * (mvmaster[r].max - mvmaster[r].min);
                        absdeltaoptim = Math.Abs(deltaoptim.m[r][0]);
                        if (absdeltaoptim > maxmove) { deltaoptim.m[r][0] = maxmove * (deltaoptim.m[r][0] / absdeltaoptim); } //Keep the sign of deltaoptim, but limt abs value.
                        mvmaster[r].var.v += deltaoptim.m[r][0];
                        if (mvmaster[r].var.v >= mvmaster[r].max) { mvmaster[r].var.v = mvmaster[r].max - global.Epsilon; }
                        else if (mvmaster[r].var.v <= mvmaster[r].min) { mvmaster[r].var.v = mvmaster[r].min + global.Epsilon; }
                    }
                    for (int r = 0; r < nrinequalityconstraints; r++)
                    {
                        news = svector.m[r][0] + deltaoptim.m[r + mvmaster.Count][0];
                        scompare = (1 - global.DefaulttauIP) * svector.m[r][0];
                        svector.m[r][0] = (news >= scompare) ? news : scompare;
                    }
                    for (int r = 0; r < nrinequalityconstraints; r++)
                    {
                        newz = zvector.m[r][0] + deltaoptim.m[r + mvmaster.Count + nrinequalityconstraints][0];
                        zcompare = (1 - global.DefaulttauIP) * zvector.m[r][0];
                        zvector.m[r][0] += (newz >= zcompare) ? newz : zcompare;
                    }

                    mubarrier *= nmpcsigma;
                }
            }
        }


        public override void update(int simi, bool historise)
        {
            
            if ((simi - initialdelay) % runinterval == 0)
            {
                callingsimi = simi;

                if (algorithm == nmpcalgorithm.UnconstrainedLineSearch)
                {
                    J0 = calcobjectivefunction();

                    mk = J0;
                    for (int index = 1; index < 2; index++) //this loop was going until N.  No need for this as we will only implement the first move anyway.
                    {
                        calcjacobianmk();
                        for (int j = 0; j < mvmaster.Count; j++)
                        {
                            double invslope = (jacobianmk[j] == 0) ? 0 : 1 / jacobianmk[j];
                            double reductioninvar = invslope * mk * alphak;
                            mvmaster[j].var.v = mvmaster[j].var.v - reductioninvar;
                            mvmatrix[j][index] = mvmaster[j].var.v;
                        }
                        //mastersim.simulateplant(false);
                        //mastersim.simi++;
                    }
                    //mastersim.copyfrom(sim0);
                }
                else if (algorithm == nmpcalgorithm.InteriorPoint1) {updateinteriorpoint1();}
                
                else if (algorithm == nmpcalgorithm.ActiveSet1)
                {
                    evaluateconstraintsactiveset1();
                    calcAmatrixactiveset1();
                    matrix Bvector = -1 * Bvectoractiveset1;
                    matrix.solveAXequalsB(Amatrixactiveset1, deltaoptim, Bvector);

                    deltaoptim = alphak * deltaoptim;

                    for (int r = 0; r < mvmaster.Count; r++)
                    {
                        mvmaster[r].var.v += deltaoptim.m[r][0];
                    }
                    for (int r = 0; r < nractiveconstraints; r++)
                    {
                        activelambdaactiveset1.m[r][0] += deltaoptim.m[r + mvmaster.Count][0];
                    }
                }
                else if (algorithm == nmpcalgorithm.GeneticAlgorithm1)
                {
                    replacechromosomes();
                    crossoverchromosomes();
                    mutatechromosomes();
                    assignfitness();
                    rankchromosomes();
                    implementchromosome();
                }
                else if (algorithm == nmpcalgorithm.ParticleSwarmOptimisation1)
                {
                    updateparticles();
                    assignparticlefitness();
                    bestparticle();
                    implementparticle();
                }

                //for (int j = 0; j < mvmaster.Count; j++) //The mastersim's mv list will now be copied from the mvmatrix' first column.
                //{
                //    mvmaster[j].var.v = mvmatrix[j][1]; //T1 is the one to implement.  T0 is the current value.
                //}
            }

        }

      

        // Genetic Algorithm1 (GA) ---------------------------------------------------------------------------------------------------------------------

        private void initgeneticalgorithm1()
        {
            nrchromosomes = global.DefaultNrChromosomes;
            nrsurvivingchromosomes = global.DefaultNrSurvivingChromosomes;
            nrpairingchromosomes = global.DefaultNrPairingChromosomes;
            chromosomelistsize = nrchromosomes + nrpairingchromosomes;
            defaultprobabilityofmutation = global.DefaultProbabilityOfMutation;
            nriterations = global.DefaultNrIterations;
            crossoverpoint = global.DefaultCrossOverPoint;

            chromosomes = new List<chromosome>();
            comparerfitness = new chromosomecomparerfit();
            chromosomecomparer = comparerfitness;

            for (int i = 0; i < chromosomelistsize; i++)
            {
                chromosomes.Add(new chromosome(mvmaster.Count, rand));
            }
        }

        private double calcfitnessga1()
        {
            //matrix mat;
            //double d;

            double fitness = 0;
            sim1.copyfrom(mastersim);
            //calcconstraintvectoractiveset1(); //This is for the mvs, if there are cvs also that have constraints, then this will need to be in the loop below.
            int phorison = (sim1.simi + N > global.SimIterations) ? global.SimIterations - sim1.simi : N;
            for (int i = 0; i < phorison; i++) //i can be started here at index as well.  But I thought it might be best to start it at zero to get a better jacobian calc done all through the opimisation horizon.
            {
                mastersim.simulateplant(false);
                for (int j = 0; j < cvmaster.Count; j++)
                {
                    fitness += cvmaster[j].weight *
                            Math.Pow((cvmaster[j].var.v - cvmaster[j].target.simvector[callingsimi + i]) / (cvmaster[j].max - cvmaster[j].min + global.Epsilon), 2); ;  //The cv vector will have to poin to sim1's variables.
                }

                //if (nractiveconstraints > 0)
                //{
                //    mat = matrix.transpose(activeconstraintvector1) * activelambdaactiveset1; //as long as the constraints are only mvs, this step can 
                //    //be put before the loop, but if there are cvs in the 
                //    //constraints, then this needs to be moved earlier.
                //    d = mat.m[0][0]; //mat will be a 1x1 matrix.
                //    lagrangian += -d;
                //}
                mastersim.simi++;
            }

            mastersim.copyfrom(sim1);
            return fitness;
        }

        private void replacechromosomes() //the bottom ranked chromosomes will be replaced with new ones.
        {
            for (int i = nrsurvivingchromosomes; i < nrchromosomes; i++)
            {
                chromosomes[i].initrandom(rand);
            }
        }

        public void replaceat(ref string input, int index, char newChar)
        {
            if (input == null)
            {
                throw new ArgumentNullException("input");
            }
            char[] chars = input.ToCharArray();
            chars[index] = newChar;
            input = new string(chars);
        }

        private void crossoverchromosomes()
        {
            string parent1, parent2, child1, child2;
            for (int i = 0; i < nrpairingchromosomes / 2; i++)
            {
                //chromosomes[nrchromosomes + i * 2].copyfrom(chromosomes[i * 2]);
                //chromosomes[nrchromosomes + i * 2 + 1].copyfrom(chromosomes[i * 2 + 1]);
                for (int j = 0; j < mvmaster.Count; j++)
                {
                    parent1 = Convert.ToString(chromosomes[i * 2].mvs[j], 2);
                    parent2 = Convert.ToString(chromosomes[i * 2 + 1].mvs[j], 2);
                    child1 = Convert.ToString(chromosomes[i * 2].mvs[j], 2);
                    child2 = Convert.ToString(chromosomes[i * 2 + 1].mvs[j], 2);
                    int minparentlength = Math.Min(parent1.Length, parent2.Length);
                    int maxcrossoverpoint = Math.Min(minparentlength, crossoverpoint);
                    for (int k = 0; k < maxcrossoverpoint; k++) // k starts at 2^0 and works to the left in the binary number as string.
                    {
                        replaceat(ref child1, child1.Length - 1 - k, parent2[parent2.Length - 1 - k]);
                        replaceat(ref child2, child2.Length - 1 - k, parent1[parent1.Length - 1 - k]);
                    }
                    chromosomes[nrchromosomes + i * 2].mvs[j] = Convert.ToInt32(child1, 2);
                    chromosomes[nrchromosomes + i * 2 + 1].mvs[j] = Convert.ToInt32(child2, 2);
                }
            }
        }

        private void mutatechromosomes()
        {

        }

        private void assignfitness()
        {
            double[] oldmv = new double[mvmaster.Count];

            for (int j = 0; j < mvmaster.Count; j++)
            {
                oldmv[j] = mvmaster[j].var.v;
            }
            
            for (int i = 0; i < chromosomes.Count; i++)
            {
                for (int j = 0; j < mvmaster.Count; j++)
                {
                    mvmaster[j].var.v = mvmaster[j].rangetoeu(chromosomes[i].mvs[j] / 100.0);
                }

                chromosomes[i].fitness = calcfitnessga1();
            }

            for (int j = 0; j < mvmaster.Count; j++)
            {
                mvmaster[j].var.v = oldmv[j];
            }
        }

        private void rankchromosomes()
        {
            chromosomes.Sort(chromosomecomparer);
        }

        private void implementchromosome()
        {
            for (int i = 0; i < mvmaster.Count; i++)
            {
                mvmaster[i].var.v +=  (mvmaster[i].rangetoeu(chromosomes[0].mvs[i] / 100.0) - mvmaster[i].var.v)*alphak;
            }
        }

        //Particle Swarm Optimisation 1 (PSO) -----------------------------------------------------------------------------------------------

        private void initparticleswarmoptimisation1()  //The boolean MVs will be implemented as one extra real/floating point MV.  Coded from the different bits.
        {
            double defaultboolmv = 0;
            //nrparticles = global.DefaultNrParticles;
            nrcontinuousparticles = global.DefaultNrContinuousParticles;
            nrbooleanparticles = global.DefaultNrBooleanParticles;
            bestsolutioncontinuousparticleindex = 0;
            bestsolutionbooleanparticleindex = 0;
            bestfitnesscontinuous = Double.MaxValue;
            bestfitnessboolean = Double.MaxValue;


            continuousparticles = new List<particle>();
            booleanparticles = new List<particle>();
            //mvboolinterrim = new double[mvboolmaster.Count];
            //systemhasbool = (mvboolmaster.Count > 0) ? 1 : 0;
            //if (systemhasbool > 0) {defaultboolmv = convertboolmvstopsomv();}

            for (int i = 0; i < nrcontinuousparticles; i++)
            {
                continuousparticles.Add(new particle(mvmaster.Count, rand, global.DefaultMaxValueforMVs)); //continuous as one extra mv that will combine all the booleans in each particle.
                for (int j = 0; j < mvmaster.Count; j++)
                {
                    continuousparticles[i].bestmvs[j] = mvmaster[j].fracofrange() * 100.0; //Set best solution for now to be equal to the current mvs. Use percentage.
                }
                
            }

            for (int i = 0; i < nrbooleanparticles; i++)
            {
                booleanparticles.Add(new particle(mvboolmaster.Count, rand, 1.0)); //continuous as one extra mv that will combine all the booleans in each particle.
                for (int j = 0; j < mvboolmaster.Count; j++)
                {
                    booleanparticles[i].bestmvs[j] = mvboolmaster[j].var.v;
                }

                //for (int j = 0; j < mvboolmaster.Count; j++)
                //{
                //    //particles[i].bestmvs[j + mvmaster.Count] = rand.Next(Convert.ToInt32(Math.Round(mvboolmaster[j].var.v*50)), 
                //    //    Convert.ToInt32(Math.Round((mvboolmaster[j].var.v + 1)*50))); //Set best solution for now to be equal to the current mvs. Use percentage.
                //    //particles[i].bestmvs[j + mvmaster.Count] = rand.NextDouble();
                //    particles[i].bestmvs[j + mvmaster.Count] = mvboolmaster[j].var.v;
                //}

                //if (systemhasbool > 0) { particles[i].bestmvs[mvmaster.Count] = defaultboolmv; }// 50.0; }// defaultboolmv; }

            }
        }

        private double convertboolmvstopsomv() //Convert the bit string of boolean mvs to a PSO MV that will fall between 0 and 
                                                        //global.DefaultMaxValueforMVs
        {
            string boolstring = "";
            for (int i = 0; i < mvboolmaster.Count; i++)
            {
                boolstring = (mvboolmaster[i].var.v == mvboolmaster[i].min) ? boolstring + "0" : boolstring + "1";
            }
            int boolstringlength = boolstring.Length;
            int interrimvalue = Convert.ToInt32(boolstring, 2);
            double interrimdouble = Convert.ToDouble(interrimvalue);
            return interrimdouble / (Math.Pow(2.0, boolstringlength) - 1) * global.DefaultMaxValueforMVs;
        }

        private void convertpsomvtomvboolinterrim(double mv)
        {
            double interrimdouble = mv/100.0*(Math.Pow(2,mvboolmaster.Count) - 1);
            int interrimint = Convert.ToInt32(Math.Round(interrimdouble));
            string interrimstring = Convert.ToString(interrimint,2);
            string boolstring = interrimstring.PadLeft(mvboolmaster.Count,'0');

            for (int i = 0; i < mvboolmaster.Count; i++)
            {
                mvboolinterrim[i] = (boolstring[i].Equals('0')) ? mvboolmaster[i].min : mvboolmaster[i].max;
            }
        }



        private void updateparticles()
        {
            double psi1, psi2; //random vars for PSO.
            double absbooleanspeed = 0;
            double sigmoidbooleanspeed = 0.5;

            for (int i = 0; i < nrcontinuousparticles; i++)
            {
                
                for (int j = 0; j < mvmaster.Count; j++)
                {
                    psi1 = rand.NextDouble();
                    psi2 = rand.NextDouble();
                    continuousparticles[i].currentspeed[j] += 2 * psi1 * (continuousparticles[i].bestmvs[j] - continuousparticles[i].currentmvs[j]) +
                        2 * psi2 * (continuousparticles[bestsolutioncontinuousparticleindex].bestmvs[j] - continuousparticles[i].currentmvs[j]);
                    continuousparticles[i].currentmvs[j] += continuousparticles[i].currentspeed[j];

                    if (continuousparticles[i].currentmvs[j] < 0) 
                    {
 
                            //particles[i].currentmvs[j] = 0;
                        continuousparticles[i].currentmvs[j] = rand.Next(0, global.PSOMVBoundaryBuffer);

                       
                    }
                    else if (continuousparticles[i].currentmvs[j] > global.DefaultMaxValueforMVs) 
                    {

                            //particles[i].currentmvs[j] = global.DefaultMaxValueforMVs;
                        continuousparticles[i].currentmvs[j] = rand.Next(global.DefaultMaxValueforMVs - global.PSOMVBoundaryBuffer, global.DefaultMaxValueforMVs);


                    }
                }

            }

            for (int i = 0; i < nrbooleanparticles; i++)
            {

                for (int j = 0; j < mvboolmaster.Count; j++)
                {
                    psi1 = rand.NextDouble();
                    psi2 = rand.NextDouble();
                    booleanparticles[i].currentspeed[j] += 2 * psi1 * (booleanparticles[i].bestmvs[j] - booleanparticles[i].currentmvs[j]) +
                        2 * psi2 * (booleanparticles[bestsolutionbooleanparticleindex].bestmvs[j] - booleanparticles[i].currentmvs[j]);
                    absbooleanspeed = Math.Abs(booleanparticles[i].currentspeed[j]);
                    if (absbooleanspeed > global.PSOMaxBooleanSpeed)
                    { booleanparticles[i].currentspeed[j] = booleanparticles[i].currentspeed[j] / absbooleanspeed * global.PSOMaxBooleanSpeed; }
                    sigmoidbooleanspeed = utilities.sigmoid(booleanparticles[i].currentspeed[j]);
                    if (rand.NextDouble() < sigmoidbooleanspeed) { booleanparticles[i].currentmvs[j] = 1.0; }
                    else { booleanparticles[i].currentmvs[j] = 0.0; }
                    //if (particles[i].currentmvs[j] < 0)
                    //{

                    //        //particles[i].currentmvs[j] = rand.Next(0, 50);
                    //        particles[i].currentmvs[j] = rand.Next(0, global.DefaultMaxValueforMVs);

                    //}
                    //else if (particles[i].currentmvs[j] > global.DefaultMaxValueforMVs)
                    //{

                    //        //particles[i].currentmvs[j] = rand.Next(50, global.DefaultMaxValueforMVs);
                    //        particles[i].currentmvs[j] = rand.Next(0, global.DefaultMaxValueforMVs);

                    //}
                }

            }
        }

        private void assignparticlefitness()
        {
            double[] oldmv = new double[mvmaster.Count];
            double[] oldmvbool = new double[mvboolmaster.Count];

            for (int j = 0; j < mvmaster.Count; j++)
            {
                oldmv[j] = mvmaster[j].var.v;
            }

            for (int j = 0; j < mvboolmaster.Count; j++)
            {
                oldmvbool[j] = mvboolmaster[j].var.v;
            }

            for (int i = 0; i < nrcontinuousparticles; i++)
            {
                for (int j = 0; j < mvmaster.Count; j++)
                {
                    mvmaster[j].var.v = mvmaster[j].rangetoeu(continuousparticles[i].currentmvs[j] / 100.0);
                }

                for (int j = 0; j < mvboolmaster.Count; j++)
                {
                    //mvboolmaster[j].var.v = Math.Round(particles[i].currentmvs[j + mvmaster.Count] / 100.0);
                    mvboolmaster[j].var.v = booleanparticles[i].currentmvs[j];
                }


                continuousparticles[i].currentfitness = calcfitnessga1();
                booleanparticles[i].currentfitness = continuousparticles[i].currentfitness;

                if (continuousparticles[i].currentfitness > continuousparticles[i].bestfitness)
                {
                    continuousparticles[i].bestfitness = continuousparticles[i].currentfitness;
                    bestfitnesscontinuous = continuousparticles[i].currentfitness;
                }

                if (booleanparticles[i].currentfitness > booleanparticles[i].bestfitness)
                {
                    booleanparticles[i].bestfitness = booleanparticles[i].currentfitness;
                    bestfitnessboolean = booleanparticles[i].currentfitness;
                }
            }

            //for (int i = 0; i < nrbooleanparticles; i++)
            //{
            //    for (int j = 0; j < mvboolmaster.Count; j++)
            //    {
            //        //mvboolmaster[j].var.v = Math.Round(particles[i].currentmvs[j + mvmaster.Count] / 100.0);
            //        mvboolmaster[j].var.v = booleanparticles[i].currentmvs[j];
            //    }

            //    booleanparticles[i].currentfitness = calcfitnessga1();

            //    if (booleanparticles[i].currentfitness > booleanparticles[i].bestfitness)
            //    {
            //        booleanparticles[i].bestfitness = booleanparticles[i].currentfitness;
            //        bestfitnessboolean = booleanparticles[i].currentfitness;
            //    }
            //}

            

            for (int j = 0; j < mvmaster.Count; j++)
            {
                mvmaster[j].var.v = oldmv[j];
            }

            for (int j = 0; j < mvboolmaster.Count; j++)
            {
                mvboolmaster[j].var.v = oldmvbool[j];
            }
        }

        private void bestparticle() //This routine will check the fittest solutions per particle and update, as well as the global fittest solution 
                                    // and update.
        {

            for (int i = 0; i < continuousparticles.Count; i++)
            {
                if (continuousparticles[i].currentfitness < continuousparticles[i].bestfitness)
                {
                    continuousparticles[i].bestfitness = continuousparticles[i].currentfitness;
                    for (int j = 0; j < mvmaster.Count; j++)
                    {
                        continuousparticles[i].bestmvs[j] = continuousparticles[i].currentmvs[j];
                    }
                }
                if (continuousparticles[i].currentfitness < bestfitnesscontinuous)
                {
                    bestfitnesscontinuous = continuousparticles[i].currentfitness;
                    bestsolutioncontinuousparticleindex = i;
                }
            }

            for (int i = 0; i < booleanparticles.Count; i++)
            {
                if (booleanparticles[i].currentfitness < booleanparticles[i].bestfitness)
                {
                    booleanparticles[i].bestfitness = booleanparticles[i].currentfitness;
                    for (int j = 0; j < mvboolmaster.Count; j++)
                    {
                        booleanparticles[i].bestmvs[j] = booleanparticles[i].currentmvs[j];
                    }
                }
                if (booleanparticles[i].currentfitness < bestfitnessboolean)
                {
                    bestfitnessboolean = booleanparticles[i].currentfitness;
                    bestsolutionbooleanparticleindex = i;
                }
            }
        }

        private void implementparticle()
        {
            double deltaoptim = 0;
            double maxmove = 0;
            double absdeltaoptim = 0;
            for (int j = 0; j < mvmaster.Count; j++)
            {
                deltaoptim = (mvmaster[j].rangetoeu(continuousparticles[bestsolutioncontinuousparticleindex].bestmvs[j] / 100.0) - mvmaster[j].var.v) * 
                    alphak;
                maxmove = global.MVMaxMovePerSampleTT0 * (mvmaster[j].max - mvmaster[j].min);
                absdeltaoptim = Math.Abs(deltaoptim);
                if (absdeltaoptim > maxmove) { deltaoptim = maxmove * (deltaoptim / absdeltaoptim); } //Keep the sign of deltaoptim, but limt abs value.

                mvmaster[j].var.v += deltaoptim;
            }

            //if (systemhasbool > 0) { convertpsomvtomvboolinterrim(particles[bestsolutionparticleindex].bestmvs[mvmaster.Count]); }

            for (int j = 0; j < mvboolmaster.Count; j++)
            {
                //mvboolmaster[j].var.v = Math.Round(particles[bestsolutionparticleindex].bestmvs[j + mvmaster.Count] / 100.0);
                mvboolmaster[j].var.v = booleanparticles[bestsolutionbooleanparticleindex].bestmvs[j];
            }
        }


        //Active Set algorithm 1 ----------------------------------------------------------------------------------------------------------------------

        private double calclagrangianactiveset1()
        {
            matrix mat;
            double d;

            double lagrangian = 0;
            sim1.copyfrom(mastersim);
            calcconstraintvectoractiveset1(); //This is for the mvs, if there are cvs also that have constraints, then this will need to be in the loop below.
            for (int i = 0; i < N; i++) //i can be started here at index as well.  But I thought it might be best to start it at zero to get a better jacobian calc done all through the opimisation horizon.
            {
                mastersim.simulateplant(false);
                for (int j = 0; j < cvmaster.Count; j++)
                {
                    lagrangian += cvmaster[j].weight / cvmaster[j].target.simvector[callingsimi + i] *
                        Math.Pow(cvmaster[j].var.v - cvmaster[j].target.simvector[callingsimi + i], 2);  //The cv vector will have to poin to sim1's variables.
                }

                if (nractiveconstraints > 0)
                {
                    mat = matrix.transpose(activeconstraintvector1) * activelambdaactiveset1; //as long as the constraints are only mvs, this step can 
                    //be put before the loop, but if there are cvs in the 
                    //constraints, then this needs to be moved earlier.
                    d = mat.m[0][0]; //mat will be a 1x1 matrix.
                    lagrangian += -d;
                }
                mastersim.simi++;
            }

            mastersim.copyfrom(sim1);
            return lagrangian;
        }

        private void calcconstraintvectoractiveset1() //This will calculate constraint 1 for algorithm 1, based on which constraints are active as per the
        //constraintaccountingvector.
        {
            int fullindex = 0;
            double var = 0;
            for (int j = 0; j < nractiveconstraints; j++)
            {
                fullindex = constraintaccountingvector[j];

                if (fullindex < mvmaster.Count)
                {
                    var = mvmaster[fullindex].var.v - mvmaster[fullindex].min;
                }
                else
                {
                    var = mvmaster[fullindex - mvmaster.Count].max - mvmaster[fullindex - mvmaster.Count].var.v;
                }
                activeconstraintvector1.m[j][0] = var;
            }
        }

        private void evaluateconstraintsactiveset1() //At the end of each iteration, the constraints will be evaluated, as well as the sign of the lambdas
        {
            double var = 0;
            for (int i = 0; i < nractiveconstraints; i++)
            {
                fulllambdaactiveset1.m[constraintaccountingvector[i]][0] = activelambdaactiveset1.m[i][0];
            }

            constraintaccountingvector.Clear();
            activeconstraintvector0.m.Clear();
            activeconstraintvector1.m.Clear();
            activelambdaactiveset1.m.Clear();
            nractiveconstraints = 0;

            for (int i = 0; i < nrinequalityconstraints; i++)
            {
                if (i < mvmaster.Count)
                {
                    var = mvmaster[i].var.v - mvmaster[i].min;
                }
                else
                {
                    var = mvmaster[i - mvmaster.Count].max - mvmaster[i - mvmaster.Count].var.v;
                }

                if (var <= 0) // && fulllambdaactiveset1.m[i][0] >= 0) //THIS CONDITION CAN BE ADDED AGAIN LATER.
                {
                    activeconstraintvector0.m.Add(new List<double>());
                    activeconstraintvector0.m[activeconstraintvector0.m.Count - 1].Add(var);
                    activelambdaactiveset1.m.Add(new List<double>());
                    activelambdaactiveset1.m[activelambdaactiveset1.m.Count - 1].Add(fulllambdaactiveset1.m[i][0]);
                    constraintaccountingvector.Add(i);
                    nractiveconstraints++;
                }
            }
            activeconstraintvector1.copyfrom(activeconstraintvector0);
            sizeactiveset1 = mvmaster.Count + nractiveconstraints;
            Amatrixactiveset1.initmatrix(sizeactiveset1, sizeactiveset1);
            Bvectoractiveset1.initmatrix(sizeactiveset1, 1);

        }

        private void calcjacobianactiveset1()
        {
            double LT0 = calclagrangianactiveset1();
            double LT1 = 0; //This will be J value after each MV move.
            double oldmv = 0;
            for (int i = 0; i < mvmaster.Count; i++)
            {

                oldmv = mvmaster[i].var.v;
                double h = global.limitjacnmpcadd;
                mvmaster[i].var.v += h;
                LT1 = calclagrangianactiveset1();

                jacobianactiveset1.m[i][0] = (LT1 - LT0) / h;
                mvmaster[i].var.v = oldmv;
                //calcconstraintvector(); //THIS CAN BE OPTIMISED so that not all the constraints and MVs have to be evaluated each time.
            }
        }

        private void calcBvectoractiveset1()
        {
            calcjacobianactiveset1();
            for (int r = 0; r < mvmaster.Count; r++)
            {
                Bvectoractiveset1.m[r][0] = jacobianactiveset1.m[r][0];
            }

            calcconstraintvectoractiveset1();
            for (int r = 0; r < nractiveconstraints; r++)
            {
                Bvectoractiveset1.m[mvmaster.Count + r][0] = activeconstraintvector1.m[r][0];
            }

        }

        private void calcAmatrixactiveset1()
        {
            calcBvectoractiveset1();
            matrix Bvectorbase = new matrix(Bvectoractiveset1);
            double oldmv = 0;
            for (int r = 0; r < sizeactiveset1; r++)
            {
                for (int c = 0; c < mvmaster.Count; c++)
                {

                    oldmv = mvmaster[c].var.v;
                    double h = global.limitjacnmpcadd;
                    mvmaster[c].var.v += h;
                    calcBvectoractiveset1();

                    Amatrixactiveset1.m[r][c] = (Bvectoractiveset1.m[r][0] - Bvectorbase.m[r][0]) / h;
                    mvmaster[c].var.v = oldmv;
                    //Bvectoractiveset1.copyfrom(Bvectorbase);

                }

                for (int c = 0; c < nractiveconstraints; c++)
                {

                    oldmv = activelambdaactiveset1.m[c][0];
                    double h = global.limitjacnmpcadd; ;
                    activelambdaactiveset1.m[c][0] += h;
                    calcBvectoractiveset1();

                    Amatrixactiveset1.m[r][mvmaster.Count + c] = (Bvectoractiveset1.m[r][0] - Bvectorbase.m[r][0]) / h;
                    activelambdaactiveset1.m[c][0] = oldmv;
                    //Bvectoractiveset1.copyfrom(Bvectorbase);

                }

            }
        }

        //other class methods ------------------------------------------------------------------------------------------------------------------------------------------------------

        public void validatesettings()
        {
            for (int i = 0; i < cvmaster.Count; i++)
            {
                if ((cvmaster[i].min == 0) && (cvmaster[i].max == 0)) {cvmaster[i].max = cvmaster[i].var.v;}

            }
        }

        public override void showtrenddetail(simulation asim, List<Form> detailtrendslist)
        {
            detailtrendslist.Add(new nmpcdetail(this, asim));
            detailtrendslist[detailtrendslist.Count - 1].Show();
        }


        public override void setproperties(simulation asim)
        {
            //update(asim.simi); These comments might need to be put back in again at some point.
            nmpcproperties nmpcprop = new nmpcproperties(this, asim);
            nmpcprop.Show();
        }

        public override bool mouseover(double x, double y)
        {
            return (utilities.distance(x - location.x, y - location.y) <= global.PIDControllerInitRadius);
        }

        public override void draw(Graphics G)
        {
            //updateinoutpointlocations();

            //Draw main tank
            GraphicsPath tankmain;
            Pen plotPen;
            float width = 1;

            tankmain = new GraphicsPath();
            plotPen = new Pen(Color.Black, width);

            Point[] myArray = new Point[] 
            {new Point(global.OriginX + Convert.ToInt32(global.GScale*(location.x - 0.5*global.NMPCWidth)), 
                    global.OriginY + Convert.ToInt32(global.GScale*(location.y + 0.5*global.NMPCHeight))), 
            new Point(global.OriginX + Convert.ToInt32(global.GScale*(location.x - 0.5*global.NMPCWidth)), 
                    global.OriginY + Convert.ToInt32(global.GScale*(location.y - 0.5*global.NMPCHeight))), 
            new Point(global.OriginX + Convert.ToInt32(global.GScale*(location.x + 0.5*global.NMPCWidth)), 
                    global.OriginY + Convert.ToInt32(global.GScale*(location.y - 0.5*global.NMPCHeight))), 
            new Point(global.OriginX + Convert.ToInt32(global.GScale*(location.x + 0.5*global.NMPCWidth)),
                    global.OriginY + Convert.ToInt32(global.GScale*(location.y + 0.5*global.NMPCHeight)))};
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
            PointF namepoint = new PointF(global.OriginX + Convert.ToInt32(global.GScale * (location.x) - name.Length * emSize / 2 / 2),
                global.OriginY + Convert.ToInt32(global.GScale * (location.y)));
            unitopname.AddString(name, family, myfontStyle, emSize, namepoint, format);
            G.FillPath(Brushes.Black, unitopname);

            //Draw inpoint
            base.draw(G);
        }
    }
}
