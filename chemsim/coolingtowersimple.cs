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
    public class coolingtowersimple: unitop
    {
        public double wetbulbtemperature; //Kelvin.
        public double flowdriftloss; //kg/s
        public double flowevaporate; //kg/s
        public double tuningfactor; //dimensionless.  This factor will be used to tune the simultion in terms chilling in the cooling tower.
        public double outflow0temperatureoutnew; //Kelvin
        public double doutflow0temperatureoutnewdt; //Kelvin/sec

        public coolingtowersimple(int anr, double ax, double ay)
            : base(anr, ax, ay, global.CoolingTowerSimpleNIn, global.CoolingTowerSimpleNOut)
        {
            initcoolingtowersimple();
            update(0, true);
        }

        public coolingtowersimple(baseclass baseclasscopyfrom)
            : base(0, 0, 0, 1, 1)
        {
            initcoolingtowersimple();
            copyfrom(baseclasscopyfrom);
        }

        public void initcoolingtowersimple()
        {
            objecttype = objecttypes.CoolingTowerSimple;
            name = nr.ToString() + " " + objecttype.ToString();

            controlpropthisclass.Clear();
            //controlpropthisclass.AddRange(new List<string>(new string[] {"tuningfactor"}));

            tuningfactor = global.CTSTuningFactor;
        }

        public override void copyfrom(baseclass baseclasscopyfrom)
        {
            coolingtowersimple coolingtowersimplecopyfrom = (coolingtowersimple)baseclasscopyfrom;

            base.copyfrom(coolingtowersimplecopyfrom);

            wetbulbtemperature = coolingtowersimplecopyfrom.wetbulbtemperature; //Kelvin.
            flowdriftloss = coolingtowersimplecopyfrom.flowdriftloss; //kg/s
            flowevaporate = coolingtowersimplecopyfrom.flowevaporate; //kg/s
            tuningfactor = coolingtowersimplecopyfrom.tuningfactor;
            outflow0temperatureoutnew = coolingtowersimplecopyfrom.outflow0temperatureoutnew; //Kelvin
            doutflow0temperatureoutnewdt = coolingtowersimplecopyfrom.doutflow0temperatureoutnewdt; //Kelvin/sec
        }

        //public override controlvar selectedproperty(int selection)
        //{
        //    if (selection >= nrcontrolpropinherited)
        //    {
        //        switch (selection - nrcontrolpropinherited)
        //        {
        //            //case 0:
        //            //    return tuningfactor;
        //            //    break;
        //            //default:
        //            //    return null;
        //        }
        //    }
        //    else { return base.selectedproperty(selection); };
        //}

        public void ddt(int simi)   //Differential equations
        {
            doutflow0temperatureoutnewdt = -1 / global.CTSTemperatureTau * outflow[0].mat.T.v + 1 / global.CTSTemperatureTau * outflow0temperatureoutnew;
        }

        public override void update(int simi, bool historise)
        {
            double Ta = global.AmbientTemperature;
            double RH = global.RelativeHumidity;
            wetbulbtemperature = Ta * Math.Atan(0.151977 * Math.Pow(RH + 8.313659, 0.5)) + Math.Atan(Ta + RH) - 
                Math.Atan(RH - 1.676331) + 0.00391838 * Math.Pow(RH, 1.5) * Math.Atan(0.023101 * RH) - 4.686035;

            flowdriftloss = inflow[0].massflow.v * 0.001; //Calc for fd as per Craig Muller.
            flowevaporate = 2.0 / 3.0 * (global.CTSFlowMakeUp - flowdriftloss);
            flowevaporate *= inflow[0].mat.T.v / (wetbulbtemperature + global.CTSApproach);

            outflow0temperatureoutnew = inflow[0].mat.T.v - 
                tuningfactor*flowevaporate / (global.CTSVaporisationFraction * inflow[0].massflow.v + global.Epsilon);

            if (outflow0temperatureoutnew < 0) { outflow0temperatureoutnew = 0; }

            ddt(simi);

            outflow[0].mat.T.v += doutflow0temperatureoutnewdt * global.SampleT;

            outflow[0].massflow.v = inflow[0].massflow.v;
            outflow[0].mat.copycompositiontothisobject(inflow[0].mat);
            outflow[0].mat.density = inflow[0].mat.density;
            outflow[0].mat.P.v = global.Ps; //standard pressure since it is open to the atmosphere.
            
        }

        public override void setproperties(simulation asim) //Method that will be inherited and that will set the properties of the applicable object in a window
        {
            coolingtowersimpleproperties coolingtowersimpleprop = new coolingtowersimpleproperties(this, asim);
            coolingtowersimpleprop.Show();
        }

        public override bool mouseover(double x, double y)
        {
            return (x >= (location.x - 0.5 * global.CTSWidth) && x <= (location.x + 0.5*global.CTSWidth)
                && y >=
                (location.y - 0.5 * global.CTSHeight) && y <= (location.y + 0.5 * global.CTSHeight));
        }

        public override void updateinoutpointlocations()
        {
            //Update in and out point locations;

            inpoint[0].x = location.x - 0.5 * global.CTSWidth + global.CTSInPointsFraction[0] *
                global.CTSWidth;
            inpoint[0].y = location.y - 0.5*global.CTSHeight - global.InOutPointWidth;

            outpoint[0].x = location.x - 0.5 * global.CTSWidth + global.CTSOutPointsFraction[0] *
                global.CTSWidth;
            outpoint[0].y = location.y + 0.5*global.CTSHeight + global.InOutPointWidth;

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
            {new Point(global.OriginX + Convert.ToInt32(global.GScale*(location.x - 0.5*global.CTSWidth)), 
                    global.OriginY + Convert.ToInt32(global.GScale*(location.y + 0.5*global.CTSHeight))), 
            new Point(global.OriginX + Convert.ToInt32(global.GScale*(location.x - 0.5*global.CTSWidth)), 
                    global.OriginY + Convert.ToInt32(global.GScale*(location.y - 0.5*global.CTSHeight))), 
            new Point(global.OriginX + Convert.ToInt32(global.GScale*(location.x + 0.5*global.CTSWidth)), 
                    global.OriginY + Convert.ToInt32(global.GScale*(location.y - 0.5*global.CTSHeight))), 
            new Point(global.OriginX + Convert.ToInt32(global.GScale*(location.x + 0.5*global.CTSWidth)),
                    global.OriginY + Convert.ToInt32(global.GScale*(location.y + 0.5*global.CTSHeight)))};
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
