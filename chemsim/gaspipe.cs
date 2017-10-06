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
    public class gaspipe : unitop
    {
        //variables

        public double holduptime; //seconds - Time one molecule will spend in the pipe.

        public double length; //m
        public controlvar diameter; //m
        public controlvar filocation; //fraction being the distance from the start of the pipe to the end where the flow measurement will be located.
        public controlvar dndtin;
        public controlvar dndtout; //These 3 molar flows are part of the internal properties of the gas pipe.

        public controlvar direction; //radians
        public controlvar distance; //m

        //methods
        public gaspipe(int anr, double p0x, double p0y, double p1x, double p1y, double alength, double adiameter,
            double afilocation)
            : base(anr, (p0x + p1x) / 2, (p0y + p1y) / 2, 1, 1)
        {
            initgaspipe(p0x, p0y, p1x, p1y, alength, adiameter, afilocation);
            update(0, false);
        }

        public gaspipe(baseclass baseclasscopyfrom)
            : base(0, 0, 0, 1, 1)
        {
            initgaspipe(0, 0, 0, 0, 0, 0, 0); //correct settings will be copied with object in copyfrom method.
            copyfrom(baseclasscopyfrom);
        }

        public void initgaspipe(double p0x, double p0y, double p1x, double p1y, double alength, double adiameter,
            double afilocation)
        {
            diameter = new controlvar();
            filocation = new controlvar();
            dndtin = new controlvar();
            dndtout = new controlvar();
            direction = new controlvar();
            distance = new controlvar();

            objecttype = objecttypes.GasPipe;
            name = nr.ToString() + " " + objecttype.ToString();

            controlpropthisclass.Clear();
            controlpropthisclass.AddRange(new List<string>(new string[] {"length",
                                            "diameter",
                                            "filocation",
                                            "dndtin",
                                            "dndtout",
                                            "direction",
                                            "distance"}));
            nrcontrolpropinherited = controlproperties.Count;
            controlproperties.AddRange(controlpropthisclass);

            points = new point[2];
            points[0] = new point(p0x, p0y);
            points[1] = new point(p1x, p1y);

            massflow.simvector = new double[global.SimIterations];
            //pressuresimvector = new double[global.SimIterations];
            actualvolumeflow.simvector = new double[global.SimIterations];
            standardvolumeflow.simvector = new double[global.SimIterations];

            dndtin.v = 0;
            dndtout.v = 0;

            length = alength;
            diameter.v = adiameter;
            mat.V.v = Math.PI * Math.Pow(diameter.v / 2, 2) * length;

            filocation.v = afilocation;

            calcn();

            updateinoutpointlocations();
        }

        public override void copyfrom(baseclass baseclasscopyfrom)
        {
            gaspipe gaspipecopyfrom = (gaspipe)baseclasscopyfrom;

            base.copyfrom(gaspipecopyfrom);

            holduptime = gaspipecopyfrom.holduptime; //seconds - Time one molecule will spend in the pipe.

            length = gaspipecopyfrom.length; //m
            diameter.v = gaspipecopyfrom.diameter.v; //m
            filocation.v = gaspipecopyfrom.filocation.v; //fraction being the distance from the start of the pipe to the end where the flow measurement will be located.
            dndtin.v = gaspipecopyfrom.dndtin.v;
            dndtout.v = gaspipecopyfrom.dndtout.v; //These 3 molar flows are part of the internal properties of the gas pipe.

            direction.v = gaspipecopyfrom.direction.v; //radians
            distance.v = gaspipecopyfrom.distance.v;
        }

        //controlpropthisclass.AddRange(new List<string>(new string[] {"length",
        //                                    "diameter",
        //                                    "filocation",
        //                                    "dndtin",
        //                                    "dndtout",
        //                                    "direction",
        //                                    "distance"}));

        public override controlvar selectedproperty(int selection)
        {
            if (selection >= nrcontrolpropinherited)
            {
                switch (selection - nrcontrolpropinherited)
                {
                    case 0:
                        return diameter;
                    case 1:
                        return filocation;
                    case 2:
                        return dndtin;
                    case 3:
                        return dndtout;
                    case 4:
                        return direction;
                    case 5:
                        return distance;
                    default:
                        return null;

                }
            }
            else { return base.selectedproperty(selection); };
        }

        public void calcn()
        {
            mat.n = mat.P * mat.V / (global.R * mat.T + 0.00001);
        }


        public void calcvolflow()
        {

        }

        public void calcstandardvolflow()
        {

        }

        public void updatedirection()
        {
            direction.v = utilities.calcdirection(points[1].y - points[0].y, points[1].x - points[0].x);
            distance.v = utilities.distance(points[0], points[1]);
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

                dndtin.v = 0;
                for (int i = 0; i < inflow.Length; i++) { dndtin += inflow[i].molarflow; }

                dndtout.v = 0;
                for (int i = 0; i < outflow.Length; i++) { dndtout += outflow[i].molarflow; }
                mat.n = mat.n + (dndtin - dndtout) * global.SampleT;
                if (mat.n < 0) { mat.n.v = 0; }

                mat.copycompositiontothisobject(inflow[0].mat);
                mat.T = inflow[0].mat.T;
                molarflow = dndtin + filocation * (dndtout - dndtin);
                mat.P = mat.n * global.R * mat.T / mat.V;
                actualvolumeflow = molarflow * global.R * mat.T / (mat.P + 0.001); //add small amount to prevent singularity.

                calcmassflowfrommolarflow();
                if (massflow.v != 0 && actualvolumeflow.v != 0) { mat.density = massflow / (actualvolumeflow + 0.001); }

                inflow[0].mat.P = mat.P;

                outflow[0].mat.P = mat.P;
                outflow[0].mat.density = mat.density;
                outflow[0].mat.T = mat.T;
                outflow[0].mat.copycompositiontothisobject(mat);
            }

            standardvolumeflow = molarflow * global.R * global.Ts / global.Ps;

            holduptime = mat.V.v / (actualvolumeflow.v + 0.001); //making sure the calc works even if there is no flow

            if (historise)
            {
                actualvolumeflow.simvector[simi] = actualvolumeflow.v;
                standardvolumeflow.simvector[simi] = standardvolumeflow.v;
                massflow.simvector[simi] = massflow.v;
                molarflow.simvector[simi] = molarflow.v;
                //pressuresimvector[simi] = mat.P.v;
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
            deltadirection = mousedirectionfrompoint0 - direction.v;
            if (Math.Cos(deltadirection) > 0)
            {
                distancefrompoint0tomouse = utilities.distance(dx, dy);
                distancefrompipe = Math.Sin(deltadirection) * distancefrompoint0tomouse;
                if (Math.Abs(distancefrompipe) <=
                    global.MinDistanceFromGasPipe &&
                    distancefrompoint0tomouse <= distance.v)
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
            gaspipeproperties gaspipeprop = new gaspipeproperties(this, asim);
            gaspipeprop.Show();
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
                                global.StreamArrowLength * Math.Cos(global.StreamArrowAngle + Math.PI + direction.v))),
                          global.OriginY + Convert.ToInt32(global.GScale * (points[1].y +
                                global.StreamArrowLength * Math.Sin(global.StreamArrowAngle + Math.PI + direction.v))));
            plot1.AddLine(global.OriginX + Convert.ToInt32(global.GScale * points[1].x),
                          global.OriginY + Convert.ToInt32(global.GScale * points[1].y),
                          global.OriginX + Convert.ToInt32(global.GScale * (points[1].x +
                                global.StreamArrowLength * Math.Cos(-global.StreamArrowAngle + Math.PI + direction.v))),
                          global.OriginY + Convert.ToInt32(global.GScale * (points[1].y +
                                global.StreamArrowLength * Math.Sin(-global.StreamArrowAngle + Math.PI + direction.v))));

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