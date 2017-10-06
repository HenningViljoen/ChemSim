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
    public class liquidpipe : unitop
    {
        //variables
        public controlvar pressure0; //Pa.  Pressure at start of pipe.
        public controlvar pressure1; //Pa.  Pressure at end of pipe.
        public double[] pressure0simvector; //Pa; history sim vector.
        public double[] pressure1simvector; //Pa; history sim vector.
        public liquidpipeflowreference flowreference; //The place where the flow to be in the pipe is set/determined.  According to the options in the array
        // global.liquidpipeflowreference 

        public controlvar dndtin;
        public controlvar dndtout; //These 3 molar flows are part of the internal properties of the liquid pipe.
        public controlvar holduptime; //seconds - Time one molecule will spend in the pipe.

        public double length; //m
        public double diameter; //m
        public double crosssectionalarea; //m2
        public controlvar filocation; //fraction being the distance from the start of the pipe to the end where the flow 
        //measurement will be located.


        public controlvar reynoldsnumber; //dimensionless.
        public controlvar meanvelocity;   //m/s. Average velocity of the liquid flow.
        public controlvar darcyfrictionfactor; //dimensionless.

        public double direction; //radians
        public double distance; //m

        //methods
        public liquidpipe(int anr, double p0x, double p0y, double p1x, double p1y, double atemperature, double alength, double adiameter,
            double afilocation, liquidpipeflowreference aflowreference)
            : base(anr, (p0x + p1x) / 2, (p0y + p1y) / 2, 1, 1)
        {
            pressure0 = new controlvar();
            pressure1 = new controlvar();
            dndtin = new controlvar();
            dndtout = new controlvar();
            holduptime = new controlvar();
            filocation = new controlvar();
            reynoldsnumber = new controlvar();
            meanvelocity = new controlvar();
            darcyfrictionfactor = new controlvar();

            objecttype = objecttypes.LiquidPipe;
            name = nr.ToString() + " " + objecttype.ToString();

            controlpropthisclass.Clear();
            controlpropthisclass.AddRange(new List<string>(new string[] {
                                                "pressure0",
                                                "pressure1",
                                                "dndtin",
                                                "dndtout",
                                                "holduptime",
                                                "filocation",
                                                "reynoldsnumber",
                                                "meanvelocity",
                                                "darcyfrictionfactor"}));
            nrcontrolpropinherited = controlproperties.Count;
            controlproperties.AddRange(controlpropthisclass);

            points = new point[2];
            points[0] = new point(p0x, p0y);
            points[1] = new point(p1x, p1y);

            massflow.simvector = new double[global.SimIterations];
            pressure0simvector = new double[global.SimIterations];
            pressure1simvector = new double[global.SimIterations];
            actualvolumeflow.simvector = new double[global.SimIterations];
            standardvolumeflow.simvector = new double[global.SimIterations];
            actualvolumeflow.v = 0;
            molarflow.v = 0;
            dndtin.v = 0;
            dndtout.v = 0;

            mat.T.v = atemperature;
            length = alength;
            diameter = adiameter;
            crosssectionalarea = Math.PI * Math.Pow(diameter / 2, 2);
            mat.V.v = crosssectionalarea * length;

            filocation.v = afilocation;
            flowreference = aflowreference;

            //n = pressure * volume / (global.R * temperature);

            updateinoutpointlocations();
            update(0, true);
        }

        public override controlvar selectedproperty(int selection)
        {
            if (selection >= nrcontrolpropinherited)
            {
                switch (selection - nrcontrolpropinherited)
                {
                    case 0:
                        return pressure0;
                    case 1:
                        return pressure1;
                    case 2:
                        return dndtin;
                    case 3:
                        return dndtout;
                    case 4:
                        return holduptime;
                    case 5:
                        return filocation;
                    case 6:
                        return reynoldsnumber;
                    case 7:
                        return meanvelocity;
                    case 8:
                        return darcyfrictionfactor;

                    default:
                        return null;

                }
            }
            else { return base.selectedproperty(selection); };
        }

        public void calcvolflow()
        {

        }

        public void calcstandardvolflow()
        {

        }

        public void calcreynoldsnumber()
        {
            //double totaldensity = 0; 
            double totaldynamicviscosity = 0;
            for (int i = 0; i < mat.composition.Count; i++)
            {
                //totaldensity += mat.composition[i].molefraction * mat.composition[i].m.density;
                totaldynamicviscosity += mat.composition[i].n/mat.n.v * mat.composition[i].m.dynamicviscosity;
            }
            calcmeanvelocity();
            //density = totaldensity;
            reynoldsnumber = mat.density * meanvelocity * diameter / totaldynamicviscosity;
        }

        public void calcdarcyfrictionfactor()
        {
            calcreynoldsnumber();
            if (meanvelocity > 0)
            {
                darcyfrictionfactor = 64 / reynoldsnumber;
            }
            else
            {
                darcyfrictionfactor.v = 0;
            }

            //if (reynoldsnumber < 2300) { darcyfrictionfactor = 64/reynoldsnumber;}
            //else {
        }

        public void calcmeanvelocity()
        {
            meanvelocity = actualvolumeflow / crosssectionalarea; //m/s
        }

        public void updatedirection()
        {
            direction = utilities.calcdirection(points[1].y - points[0].y, points[1].x - points[0].x);
            distance = utilities.distance(points[0], points[1]);
        }

        public override void updatepoint(int i, double x, double y)
        {
            points[i].x = x;
            points[i].y = y;
            updatedirection();
        }

        public override void update(int simi, bool historise)
        {
            if (inflow[0] != null && outflow[0] != null)
            {
                outflow[0].mat.copycompositiontothisobject(mat);

                switch (flowreference)
                {
                    case liquidpipeflowreference.PipeEntrance:
                        dndtin.v = 0;
                        for (int i = 0; i < inflow.Length; i++) { dndtin += inflow[i].molarflow; }
                        dndtout = dndtin; //this is assuming that this liquid pipe does not have any compression.
                        break;
                    case liquidpipeflowreference.PipeEnd:
                        dndtout.v = 0;
                        for (int i = 0; i < outflow.Length; i++) { dndtout += outflow[i].molarflow; }
                        dndtin = dndtout;

                        break;
                }

                mat.copycompositiontothisobject(inflow[0].mat);
                mat.density = inflow[0].mat.density;
                mat.T = inflow[0].mat.T;
                molarflow = dndtin;
                calcmassflowfrommolarflow();
                actualvolumeflow = massflow / mat.density;
                calcdarcyfrictionfactor();
                pressure0 = inflow[0].mat.P;
                pressure1 = pressure0 - darcyfrictionfactor * length / diameter * mat.density * controlvar.Pow(meanvelocity, 2) / 2;

                outflow[0].mat.copycompositiontothisobject(mat);
                outflow[0].mat.density = mat.density;
                outflow[0].mat.T = mat.T;
                outflow[0].mat.P = pressure1;

                switch (flowreference)
                {
                    case liquidpipeflowreference.PipeEntrance:
                        outflow[0].massflow = massflow;
                        break;
                    case liquidpipeflowreference.PipeEnd:
                        inflow[0].massflow = massflow;
                        break;
                }
            }

            standardvolumeflow = actualvolumeflow;
            holduptime = mat.V / (actualvolumeflow + 0.00000001); //making sure the calc works even if there is no flow

            if (historise)
            {
                massflow.simvector[simi] = massflow.v;
                pressure0simvector[simi] = pressure0.v;
                pressure1simvector[simi] = pressure1.v;
                actualvolumeflow.simvector[simi] = actualvolumeflow.v;
                standardvolumeflow.simvector[simi] = standardvolumeflow.v;
            }
        }

        public override bool mouseover(double x, double y)
        {
            bool pipeover = false; //default
            double dx, dy, mousedirectionfrompoint0, deltadirection, distancefrompipe, newdistancefrompipe;
            double distancetomiddlefrompoint0, distancefrompoint0tomouse;
            double midroadx, midroady;

            dx = x - points[0].x;
            dy = y - points[0].y;
            mousedirectionfrompoint0 = utilities.calcdirection(dy, dx);
            //mousedirectionfrompoint0 -= sim->roads[i]->direction;
            deltadirection = mousedirectionfrompoint0 - direction;
            if (Math.Cos(deltadirection) > 0)
            {
                distancefrompoint0tomouse = utilities.distance(dx, dy);
                distancefrompipe = Math.Sin(deltadirection) * distancefrompoint0tomouse;
                if (Math.Abs(distancefrompipe) <=
                    global.MinDistanceFromGasPipe &&
                    distancefrompoint0tomouse <= distance)
                {
                    pipeover = true;
                }
                else
                {
                    pipeover = false;
                }
            }
            else
            {
                pipeover = false;
            }

            return pipeover;
        }

        public override void updateinoutpointlocations()
        {

            //Update in and out point locations;
            inpoint[0].x = points[0].x - global.InOutPointWidth;
            inpoint[0].y = points[0].y;
            outpoint[0].x = points[1].x + global.InOutPointWidth;
            outpoint[0].y = points[1].y;

            //public static double ValveLength = 1; //m
            //public static double ValveWidth = 0.2; //m
        }

        public override void setproperties(simulation asim)
        {
            liquidpipeproperties liquidpipeprop = new liquidpipeproperties(this, asim);
            liquidpipeprop.Show();
        }

        public override void draw(Graphics G)
        {
            updateinoutpointlocations();

            GraphicsPath plot1;
            Pen plotpen;
            float width = 2;

            plot1 = new GraphicsPath();


            plot1.AddLine(global.OriginX + Convert.ToInt32(global.GScale * points[0].x),
                          global.OriginY + Convert.ToInt32(global.GScale * points[0].y),
                          global.OriginX + Convert.ToInt32(global.GScale * points[1].x),
                          global.OriginY + Convert.ToInt32(global.GScale * points[1].y));
            plot1.AddLine(global.OriginX + Convert.ToInt32(global.GScale * points[1].x),
                          global.OriginY + Convert.ToInt32(global.GScale * points[1].y),
                          global.OriginX + Convert.ToInt32(global.GScale * (points[1].x +
                                global.StreamArrowLength * Math.Cos(global.StreamArrowAngle + Math.PI + direction))),
                          global.OriginY + Convert.ToInt32(global.GScale * (points[1].y +
                                global.StreamArrowLength * Math.Sin(global.StreamArrowAngle + Math.PI + direction))));
            plot1.AddLine(global.OriginX + Convert.ToInt32(global.GScale * points[1].x),
                          global.OriginY + Convert.ToInt32(global.GScale * points[1].y),
                          global.OriginX + Convert.ToInt32(global.GScale * (points[1].x +
                                global.StreamArrowLength * Math.Cos(-global.StreamArrowAngle + Math.PI + direction))),
                          global.OriginY + Convert.ToInt32(global.GScale * (points[1].y +
                                global.StreamArrowLength * Math.Sin(-global.StreamArrowAngle + Math.PI + direction))));

            plotpen = new Pen(Color.Black, width);
            if (highlighted)
            {
                plotpen.Color = Color.Red;
            }
            G.DrawPath(plotpen, plot1);

            base.draw(G);
        }

    }
}
