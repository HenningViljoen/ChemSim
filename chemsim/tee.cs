using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Drawing2D;


namespace chemsim
{
    [Serializable]
    public class tee : unitop
    {
        public double teeinitradius;
        public mixer linkedmixer;  //Every tee will now have a mixer that is linked to it, so that the flows into the different branches of the tee can 
                                   // be dynamically calculated during the simulation to make sure all flows into the mixer, has the same pressure.
        public double[] k;  //Array of constants that will relate F^2*k^2 = deltaP^2 over each branch between the tee and the mixer.
        public double[] branchmassflow; //kg/s.  Array of mass flows per branch.
        public double[] branchdp; //Pascal.  Array of DPs from the tee to the mixer.

        //methods
        public tee(int anr, double ax, double ay, int anout)
            : base(anr, ax, ay, 1, anout)
        {
            inittee();
        }

        public tee(baseclass baseclasscopyfrom)
            : base(0, 0, 0, 1, ((tee)baseclasscopyfrom).nout) 
        {
            inittee();
            copyfrom(baseclasscopyfrom);
        }

        public void inittee()
        {
            objecttype = objecttypes.Tee;
            name = nr.ToString() + " " + objecttype.ToString();

            //inflow[0] = new stream(0, ax, ay, ax, ay);

            teeinitradius = global.TeeInitRadiusDefault;
            initk();
            initbranchflows();
            initbranchdp();

            updateinoutpointlocations();
        }

        public override void copyfrom(baseclass baseclasscopyfrom)
        {
            tee teecopyfrom = (tee)baseclasscopyfrom;

            base.copyfrom(teecopyfrom);

            teeinitradius = teecopyfrom.teeinitradius;
            //linkedmixer.copyfrom(teecopyfrom.linkedmixer);  //Every tee will now have a mixer that is linked to it, so that the flows into the different branches of the tee can 
            //                           // be dynamically calculated during the simulation to make sure all flows into the mixer, has the same pressure.
            Array.Copy(teecopyfrom.k,k,teecopyfrom.k.Length);  //Array of constants that will relate F^2*k^2 = deltaP^2 over each branch between the tee and the mixer.
            Array.Copy(teecopyfrom.branchmassflow,branchmassflow,teecopyfrom.branchmassflow.Length); //kg/s.  Array of mass flows per branch.
            Array.Copy(teecopyfrom.branchdp,branchdp,teecopyfrom.branchdp.Length); //Pascal.  Array of DPs from the tee to the mixer.
        }

        public void initk()
        {
            k = new double[nout];
        }

        public void initbranchflows()
        {
            initoutflow();
            branchmassflow = new double[nout];
            for (int i = 0; i < nout; i++)
            {
                outflow[i] = new stream(i, location.x, location.y, location.x, location.y);
                outflow[i].massflow.v = inflow[0].massflow.v / nout; //Initial value for the outflow until update method runs.
            }
        }

        public void initbranchdp()
        {
            branchdp = new double[nout];
            for (int i = 0; i < nout; i++)
            {
                branchdp[i] = 0;
            }
        }

        public override void update(int simi, bool historise)
        {
            if (linkedmixer != null)
            {
                double defaultbranchdp = inflow[0].mat.P.v - linkedmixer.outflow[0].mat.P.v;
                if (defaultbranchdp <= 0) { defaultbranchdp = global.Epsilon; }
                double defaultbranchmassflow = inflow[0].massflow.v / nout;
                if (defaultbranchmassflow == 0) { defaultbranchmassflow = global.Epsilon; }


                for (int i = 0; i < nout; i++)
                {
                    outflow[i].mat.copycompositiontothisobject(inflow[0].mat);
                    outflow[i].mat.density = inflow[0].mat.density;
                    outflow[i].mat.T = inflow[0].mat.T;
                    //outflow[i].mat.P = inflow[0].mat.P;
                    branchdp[i] = outflow[i].mat.P.v - linkedmixer.inflow[i].mat.P.v;
                    if (branchdp[i] <= 0)
                    {
                        branchdp[i] = defaultbranchdp;
                    }
                    //{ 
                    //    branchdp[i] = 0;
                    //    nrbranchdpzero++;
                    //}
                    if (branchmassflow[i] == 0)
                    {
                        branchmassflow[i] = defaultbranchmassflow;
                    }
                    k[i] = Math.Sqrt(branchdp[i]) / branchmassflow[i];
                }





                //Set-up matrices for sollution
                matrix A = new matrix(nout, nout);
                matrix B = new matrix(nout, 1);
                matrix lmatrix = new matrix(nout, nout);
                matrix umatrix = new matrix(nout, nout);
                matrix x = new matrix(nout, 1);
                matrix ymatrix = new matrix(nout, 1);
                for (int r = 0; r < nout - 1; r++)
                {
                    A.m[r][r] = k[r];
                    A.m[r][r + 1] = -k[r + 1];
                }
                for (int c = 0; c < nout; c++)
                {
                    A.m[nout - 1][c] = 1;
                }
                B.m[nout - 1][0] = inflow[0].massflow.v;

                //Solve linear set of equations
                A.ludecomposition(lmatrix, umatrix);
                matrix tempm = lmatrix * umatrix;
                matrix.solveLYequalsB(lmatrix, ymatrix, B);
                matrix tempm2 = lmatrix * ymatrix;
                matrix.solveUXequalsY(umatrix, x, ymatrix);
                matrix tempm3 = umatrix * x;
                for (int j = 0; j < nout; j++)
                {
                    branchmassflow[j] = x.m[j][0];
                }
                //}
                double totalbranchflow = 0;
                for (int i = 0; i < nout; i++)
                {
                    totalbranchflow += branchmassflow[i];
                }
                if (Math.Abs((totalbranchflow - inflow[0].massflow.v) / (totalbranchflow + global.Epsilon)) > global.ConvergeDiffFrac)
                {
                    for (int i = 0; i < nout; i++)
                    {
                        branchmassflow[i] *= inflow[0].massflow.v / (totalbranchflow + global.Epsilon);
                    }
                }

                for (int j = 0; j < nout; j++)
                {
                    outflow[j].massflow.v = branchmassflow[j];
                    //outflow[j].massflow.v = inflow[0].massflow.v/2;
                    //outflow[j].massflow.v = 500000 / 3600.0;
                }

                //This section will now calculate the upstream pressure.
                double[] massflowtouse = new double[nout];
                double totalmassflowtouse = 0;
                for (int j = 0; j < nout; j++)
                {
                    massflowtouse[j] = (inflow[0].massflow.v == 0) ? 1 : outflow[j].massflow.v;
                    totalmassflowtouse += massflowtouse[j];
                }

                for (int j = 0; j < nout; j++)
                {
                    inflow[0].mat.P.v += outflow[j].mat.P.v * massflowtouse[j];
                }

                inflow[0].mat.P.v /= totalmassflowtouse;
            }
            else //no linkedmixer, so in this case the dP is given to the network, and flows are calculated in each stream's unitops.
            {
                double totalinflow = 0;
                //inflow[0].massflow.v = 0;
                for (int j = 0; j < nout; j++)
                {
                    outflow[j].mat.P.v = inflow[0].mat.P.v;
                    outflow[j].mat.copycompositiontothisobject(inflow[0].mat);
                    outflow[j].mat.density = inflow[0].mat.density;
                    outflow[j].mat.T = inflow[0].mat.T;

                    totalinflow += outflow[j].massflow.v;

                }
                inflow[0].massflow.v = totalinflow;
            }
        }



        public override void updateinoutpointlocations()
        {
            //Update in and out point locations;
            inpoint[0].x = location.x + global.TeeLength / 2;
            inpoint[0].y = location.y;
            for (int i = 0; i < nout; i++)
            {
                outpoint[i].x = location.x - global.TeeLength / 2;
                outpoint[i].y = location.y - (nout - 1) / 2.0 * global.TeeDistanceBetweenBranches +
                    i * global.TeeDistanceBetweenBranches;
            }

            base.updateinoutpointlocations();
        }

        public override void setproperties(simulation asim)
        {
            teeproperties teeprop = new teeproperties(this, asim);
            teeprop.Show();
        }

        public override bool mouseover(double x, double y)
        {
            return (utilities.distance(x - location.x, y - location.y) <= teeinitradius);
        }

        public override void draw(Graphics G)
        {
            updateinoutpointlocations();

            GraphicsPath plot1;
            Pen plotPen;
            float width = 1;

            plot1 = new GraphicsPath();
            plotPen = new Pen(Color.Black, width);


            Point[] input = new Point[] 
            {new Point(global.OriginX + Convert.ToInt32(global.GScale*(inpoint[0].x)), 
                    global.OriginY + Convert.ToInt32(global.GScale*(inpoint[0].y - global.TeeBranchThickness/2))), 
            new Point(global.OriginX + Convert.ToInt32(global.GScale*(inpoint[0].x - global.TeeLength/2)), 
                    global.OriginY + Convert.ToInt32(global.GScale*(inpoint[0].y - global.TeeBranchThickness/2))), 
            new Point(global.OriginX + Convert.ToInt32(global.GScale*(inpoint[0].x - global.TeeLength/2)), 
                    global.OriginY + Convert.ToInt32(global.GScale*(inpoint[0].y + global.TeeBranchThickness/2))), 
            new Point(global.OriginX + Convert.ToInt32(global.GScale*(inpoint[0].x)),
                    global.OriginY + Convert.ToInt32(global.GScale*(inpoint[0].y + global.TeeBranchThickness/2)))};

            plot1.AddPolygon(input);

            Rectangle upright = new Rectangle(
                global.OriginX + Convert.ToInt32(global.GScale * (location.x - global.TeeBranchThickness / 2)),
                global.OriginY + Convert.ToInt32(global.GScale * (location.y - (nout - 1) / 2.0 * global.TeeDistanceBetweenBranches)),
                Convert.ToInt32(global.GScale * (global.TeeBranchThickness)),
                Convert.ToInt32(global.GScale * ((nout - 1) * global.TeeDistanceBetweenBranches)));
            plot1.AddRectangle(upright);

            Rectangle[] branches = new Rectangle[nout];
            for (int i = 0; i < nout; i++)
            {
                branches[i] = new Rectangle(
                    global.OriginX + Convert.ToInt32(global.GScale * (location.x - global.TeeLength / 2)),
                    global.OriginY + Convert.ToInt32(global.GScale * (location.y - (nout - 1) / 2.0 * global.TeeDistanceBetweenBranches +
                    i * global.TeeDistanceBetweenBranches)),
                    Convert.ToInt32(global.GScale * (global.TeeLength / 2)),
                    Convert.ToInt32(global.GScale * (global.TeeBranchThickness)));
                plot1.AddRectangle(branches[i]);
            }

            plotPen.Color = Color.Black;

            SolidBrush brush = new SolidBrush(Color.Black);
            if (highlighted) { brush.Color = Color.Orange; }
            G.FillPath(brush, plot1);
            G.DrawPath(plotPen, plot1);

            base.draw(G);
        }
    }
}
