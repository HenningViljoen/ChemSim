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
    public class ftreactor : unitop
    {
        public double maxvolume; //m^3
        public controlvar massinventory; //kg
        public controlvar actualvolumeinventory; //m3
        public controlvar percinventory; //percentage
        public double[] percinventorysimvector; //percentage history sim vector
        public controlvar inventoryheight; //m
        public controlvar pressureatbottom; //Pa
        public double[] pressureatbottomsimvector; //pressure at the bottom of the tank history sim vector.

        public double[][] catbatweightnew;
        public double[][] catbatweightnewcum;
        public double[][] catbatweightnewperc; //Percentage of the total that is due to each batch
        public double[][] catbatactnew;
        public double[][] catbatactnewcum;
        public double[][] catbatweightregen;
        public double[][] catbatweightregencum;
        public double[][] catbatweightregenperc; //Percentage of the total that is due to each batch
        public double[][] catbatactregen;
        public double[][] catbatactregencum;
        public double[][] timevector2d;

        public double[] catinreactor; //total cat in reactor
        public double[] catactoverall;
        public double[] timevector1d;
        public double[] freshcatloading;
        public double[] cattakeout;
        public double[] cattakeoutinventory;
        public double[] catregen;
        public double[] reactorprod; //tonnes : Reactor production
        public double[] regenhist; //histogram for the regeneration of the catalyst.  Probability of regenning a certain amount of times.
        public double[] cummregendist; //commulative probability distribution function for number of regens occurring.

        public double prevcat;

        //Methods
        public ftreactor(int anr, double ax, double ay, double apercinventory)
            : base(anr, ax, ay, global.FTReactorNIn, global.FTReactorNOut)
        {
            massinventory = new controlvar();
            actualvolumeinventory = new controlvar();
            percinventory = new controlvar();
            inventoryheight = new controlvar();
            pressureatbottom = new controlvar();

            objecttype = objecttypes.FTReactor;
            name = nr.ToString() + " " + objecttype.ToString();

            controlpropthisclass.Clear();
            controlpropthisclass.AddRange(new List<string>(new string[] {"maxvolume",
                                            "massinventory",
                                            "actualvolumeinventory",
                                            "percinventory",
                                            "inventoryheight",
                                            "pressureatbottom"}));
            nrcontrolpropinherited = controlproperties.Count;
            controlproperties.AddRange(controlpropthisclass);

            percinventory.v = apercinventory;
            calcmassinventoryfrompercinventory();
            percinventorysimvector = new double[global.SimIterations];
            pressureatbottomsimvector = new double[global.SimIterations];
            updateinoutpointlocations();

            catbatweightnew = new double[global.SimIterations][];
            catbatweightnewcum = new double[global.SimIterations][];
            catbatweightregen = new double[global.SimIterations][];
            catbatweightregencum = new double[global.SimIterations][];
            catbatweightnewperc = new double[global.SimIterations][];
            catbatweightregenperc = new double[global.SimIterations][];
            timevector2d = new double[global.SimIterations][];
            catbatactnew = new double[global.SimIterations][];
            catbatactnewcum = new double[global.SimIterations][];
            catbatactregen = new double[global.SimIterations][];
            catbatactregencum = new double[global.SimIterations][];

            for (int i = 0; i < global.SimIterations; i++)
            {
                catbatweightnew[i] = new double[global.SimIterations];
                catbatweightnewcum[i] = new double[global.SimIterations];
                catbatweightregen[i] = new double[global.SimIterations];
                catbatweightregencum[i] = new double[global.SimIterations];
                catbatweightnewperc[i] = new double[global.SimIterations];
                catbatweightregenperc[i] = new double[global.SimIterations];
                timevector2d[i] = new double[global.SimIterations];
                catbatactnew[i] = new double[global.SimIterations];
                catbatactnewcum[i] = new double[global.SimIterations];
                catbatactregen[i] = new double[global.SimIterations];
                catbatactregencum[i] = new double[global.SimIterations];
            }

            catinreactor = new double[global.SimIterations];
            timevector1d = new double[global.SimIterations];
            catactoverall = new double[global.SimIterations];
            freshcatloading = new double[global.SimIterations];
            cattakeout = new double[global.SimIterations];
            cattakeoutinventory = new double[global.SimIterations];
            catregen = new double[global.SimIterations];
            reactorprod = new double[global.SimIterations];
            regenhist = new double[global.SimIterations];
            cummregendist = new double[global.SimIterations];

            for (int i = 0; i < global.SimIterations; i++)
            {
                freshcatloading[i] = 0;
                cattakeout[i] = 0; //global.CatTake * global.SampleT;
                cattakeoutinventory[i] = 0;
                catregen[i] = 0;

                timevector1d[i] = i;
                catinreactor[i] = 0;
                catactoverall[i] = 0;
                freshcatloading[i] = 0;
                reactorprod[i] = 0;
                regenhist[i] = 0;
                cummregendist[i] = 0;
            }

            //for (int i = 0; i <= global.OrigLoading; i++)
            //{

            //}

            for (int i = 0; i < global.SimIterations; i++)
            {
                for (int j = 0; j < global.SimIterations; j++)
                {
                    catbatweightnew[j][i] = 0;
                    catbatweightnewcum[j][i] = 0;
                    catbatweightregen[j][i] = 0;
                    catbatweightregencum[j][i] = 0;
                    catbatweightnewperc[j][i] = 0;
                    catbatweightregenperc[j][i] = 0;
                    catbatactnew[j][i] = 0;
                    catbatactnewcum[j][i] = 0;
                    catbatactregen[j][i] = 0;
                    catbatactregencum[j][i] = 0;
                    timevector2d[j][i] = i;
                }
            }

            //for (int i = (global.OrigLoading + 1); i < global.SimIterations; i++)
            //{
            //    cattakeout[i] = global.CatTake;
            //    catregen[i] = global.RegenIn;
            //    cattakeoutinventory[i] = cattakeoutinventory[i - 1] + (cattakeout[i] - catregen[i]);
            //}

            prevcat = 0.01; //small number



            update(0, false);
        }

        //controlpropthisclass.AddRange(new List<string>(new string[] {"maxvolume",
        //                                    "massinventory",
        //                                    "actualvolumeinventory",
        //                                    "percinventory",
        //                                    "inventoryheight",
        //                                    "pressureatbottom"}));

        public override controlvar selectedproperty(int selection)
        {
            if (selection >= nrcontrolpropinherited)
            {
                switch (selection - nrcontrolpropinherited)
                {
                    case 0:
                        return massinventory;
                    case 1:
                        return actualvolumeinventory;
                    case 2:
                        return percinventory;
                    case 3:
                        return inventoryheight;
                    case 4:
                        return pressureatbottom;
                    default:
                        return null;

                }
            }
            else { return base.selectedproperty(selection); };
        }

        public void calcmassinventoryfrompercinventory()
        {
            actualvolumeinventory = percinventory / 100 * maxvolume;
            massinventory = actualvolumeinventory * mat.density;
        }

        public void inventorycalcs()
        {
            actualvolumeinventory = massinventory / mat.density;
            percinventory = actualvolumeinventory / maxvolume * 100;
            inventoryheight = actualvolumeinventory / (Math.PI * Math.Pow(global.FTReactorRadius, 2));
        }

        public override void update(int simi, bool historise) //This will have to be edited in order to take the 
                                                              //historise parameter properly into account.
        {
            if (inflow[0] != null) { freshcatloading[simi] = inflow[0].massflow.v * global.SampleT; }
            if (outflow[1] != null) { cattakeout[simi] = outflow[1].massflow.v * global.SampleT; }

            if (simi != 0) { prevcat = catinreactor[simi - 1]; }
            catinreactor[simi] = prevcat + freshcatloading[simi] - cattakeout[simi] + catregen[simi];

            regenhist[simi] = Math.Pow(global.NrRegen / (global.NrRegen + 1), simi) * 1 / (global.NrRegen + 1);
            if (simi > 0) { cummregendist[simi] = cummregendist[simi - 1] + (regenhist[simi - 1] + regenhist[simi]) / 2; }

            for (int j = 0; j < global.SimIterations; j++)
            {
                if (j < simi)
                {
                    catbatweightnew[j][simi] = catbatweightnew[j][simi - 1] -
                        cattakeout[simi] * catbatweightnewperc[j][simi - 1] / 100;
                    catbatweightregen[j][simi] = catbatweightregen[j][simi - 1] -
                        cattakeout[simi] * catbatweightregenperc[j][simi - 1] / 100;
                    catbatactnew[j][simi] = catbatactnew[j][simi - 1] * Math.Pow((1 + global.CatDecayRate / 100), global.SampleT);
                    catbatactregen[j][simi] = catbatactregen[j][simi - 1] * Math.Pow((1 + global.CatDecayRate / 100), global.SampleT);
                }
                else if (j == simi)
                {
                    catbatweightnew[j][simi] = freshcatloading[simi];
                    catbatweightregen[j][simi] = catregen[simi];
                    catbatactnew[j][simi] = global.FreshCatAct; //%
                    catbatactregen[j][simi] = catregen[simi] > 0 ? global.RegenCatAct : 0; //%
                }
                if (j <= simi)
                {
                    catbatweightnewperc[j][simi] = catbatweightnew[j][simi] / catinreactor[simi] * 100;
                    catbatweightregenperc[j][simi] = catbatweightregen[j][simi] / catinreactor[simi] * 100;

                    catbatweightnewcum[j][simi] = catbatweightnew[j][simi];
                    catbatweightregencum[j][simi] = catbatweightregen[j][simi];
                    catbatactnewcum[j][simi] = catbatactnew[j][simi] * catbatweightnewperc[j][simi] / 100;
                    catbatactregencum[j][simi] = catbatactregen[j][simi] * catbatweightregenperc[j][simi] / 100;

                    if (j > 0)
                    {
                        catbatweightnewcum[j][simi] = catbatweightnewcum[j][simi] + catbatweightnewcum[j - 1][simi];
                        catbatweightregencum[j][simi] = catbatweightregencum[j][simi] + catbatweightregencum[j - 1][simi];
                        catbatactnewcum[j][simi] = catbatactnewcum[j][simi] + catbatactnewcum[j - 1][simi];
                        catbatactregencum[j][simi] = catbatactregencum[j][simi] + catbatactregencum[j - 1][simi];
                    }

                    catactoverall[simi] = catactoverall[simi] + catbatactnew[j][simi] * catbatweightnewperc[j][simi] / 100 +
                        catbatactregen[j][simi] * catbatweightregenperc[j][simi] / 100;
                    reactorprod[simi] = catactoverall[simi] * global.SampleT * catinreactor[simi];
                }
            }

        }

        public override bool mouseover(double x, double y)
        {
            return (x >= (location.x - global.FTReactorRadius) && x <= (location.x + global.FTReactorRadius) && y >=
                (location.y - 0.5 * global.FTReactorHeight) && y <= (location.y + 0.5 * global.FTReactorHeight));
        }

        public override void updateinoutpointlocations()
        {
            //Update in and out point locations;
            for (int i = 0; i < global.FTReactorNIn; i++)
            {
                inpoint[i].x = location.x - global.FTReactorRadius - global.InOutPointWidth;
                inpoint[i].y = location.y - global.FTReactorHeight * 0.5 + global.FTReactorInPointsFraction[i] * global.FTReactorHeight;
            }

            for (int i = 0; i < global.FTReactorNOut; i++)
            {
                outpoint[i].x = location.x + global.FTReactorRadius + global.InOutPointWidth;
                outpoint[i].y = location.y - global.FTReactorHeight * 0.5 + global.FTReactorOutPointsFraction[i] * global.FTReactorHeight;
            }
        }

        public override void setproperties(simulation asim)
        {
            ftreactorproperties ftreactorprop = new ftreactorproperties(this, asim);
            ftreactorprop.Show();
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
            {new Point(global.OriginX + Convert.ToInt32(global.GScale*(location.x - global.FTReactorRadius)), 
                    global.OriginY + Convert.ToInt32(global.GScale*(location.y + 0.5*global.FTReactorHeight))), 
            new Point(global.OriginX + Convert.ToInt32(global.GScale*(location.x - global.FTReactorRadius)), 
                    global.OriginY + Convert.ToInt32(global.GScale*(location.y - 0.5*global.FTReactorHeight))), 
            new Point(global.OriginX + Convert.ToInt32(global.GScale*(location.x + global.FTReactorRadius)), 
                    global.OriginY + Convert.ToInt32(global.GScale*(location.y - 0.5*global.FTReactorHeight))), 
            new Point(global.OriginX + Convert.ToInt32(global.GScale*(location.x + global.FTReactorRadius)),
                    global.OriginY + Convert.ToInt32(global.GScale*(location.y + 0.5*global.FTReactorHeight)))};
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
