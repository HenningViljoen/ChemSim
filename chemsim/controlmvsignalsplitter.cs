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
    public class controlmvsignalsplitter : block   //a class that will split an mv control var signal coming in, and copy it to a number of receiving unit ops' control var mvs.
    {
        public controlvar inputsignal;
        public List<mpcvar> outputsignals;

        public controlmvsignalsplitter(int anr, double ax, double ay)
            : base(anr, ax, ay)
        {
            controlmvsignalsplitterinit();
        }

        public controlmvsignalsplitter(baseclass baseclasscopyfrom)
            : base(baseclasscopyfrom.nr, baseclasscopyfrom.location.x, baseclasscopyfrom.location.y)
        {
            controlmvsignalsplitterinit();
            copyfrom(baseclasscopyfrom);
        }

        private void controlmvsignalsplitterinit()
        {
            objecttype = objecttypes.ControlMVSignalSplitter;
            name = nr.ToString() + " " + objecttype.ToString();

            inputsignal = new controlvar();
            outputsignals = new List<mpcvar>();

            controlpropthisclass.Clear();
            controlpropthisclass.AddRange(new List<string>(new string[] {
                                            "inputsignal"}));
            nrcontrolpropinherited = controlproperties.Count;
            controlproperties.AddRange(controlpropthisclass);
        }

        public override void copyfrom(baseclass baseclasscopyfrom)
        {
            controlmvsignalsplitter controlmvsignalsplittercopyfrom = (controlmvsignalsplitter)baseclasscopyfrom;

            base.copyfrom(controlmvsignalsplittercopyfrom);

            inputsignal.copyfrom(controlmvsignalsplittercopyfrom.inputsignal);

            if (outputsignals.Count != controlmvsignalsplittercopyfrom.outputsignals.Count) 
            { 
                outputsignals = new List<mpcvar>();
                for (int i = 0; i < controlmvsignalsplittercopyfrom.outputsignals.Count; i++)
                {
                    outputsignals.Add(new mpcvar(new controlvar(), "New mpcvar", 0, 0));
                }
            }

            for (int i = 0; i < controlmvsignalsplittercopyfrom.outputsignals.Count; i++)
            {
                outputsignals[i].copyfrom(controlmvsignalsplittercopyfrom.outputsignals[i]);
            }

        }

        public override controlvar selectedproperty(int selection)
        {
            if (selection >= nrcontrolpropinherited)
            {
                switch (selection - nrcontrolpropinherited)
                {
                    case 0:
                        return inputsignal;
                    
                    default:
                        return null;

                }
            }
            else { return base.selectedproperty(selection); };
        }

        public override void update(int simi, bool historise)
        {
            for (int i = 0; i < outputsignals.Count; i++)
            {
                outputsignals[i].var.v = inputsignal.v;
            }

            if (historise)
            {
                if (inputsignal.simvector != null) { inputsignal.simvector[simi] = inputsignal.v; }

            }
        }

        public override void setproperties(simulation asim)
        {
            //update(asim.simi); These comments might need to be put back in again at some point.
            controlmvsignalsplitterproperties controlmvsignalsplitterprop = new controlmvsignalsplitterproperties(this, asim);
            controlmvsignalsplitterprop.Show();
        }

        public override bool mouseover(double x, double y)
        {
            return (utilities.distance(x - location.x, y - location.y) <= global.PIDControllerInitRadius);
        }

        public override void draw(Graphics G)
        {
            //updateinoutpointlocations();

            GraphicsPath plot1;
            Pen plotPen;
            float width = 1;

            plot1 = new GraphicsPath();
            plotPen = new Pen(Color.Black, width);

            //plot1.AddEllipse(global.OriginX + Convert.ToInt32(global.GScale * (location.x - global.PIDControllerInitRadius)),
            //                global.OriginY + Convert.ToInt32(global.GScale * (location.y - global.PIDControllerInitRadius)),
            //                Convert.ToInt32(global.GScale * (global.PIDControllerInitRadius * 2)),
            //                Convert.ToInt32(global.GScale * (global.PIDControllerInitRadius * 2)));

            Point[] myArray = new Point[] 
            {new Point(global.OriginX + Convert.ToInt32(global.GScale*(location.x - global.PIDControllerInitRadius)), 
                    global.OriginY + Convert.ToInt32(global.GScale*(location.y + global.PIDControllerInitRadius))), 
            new Point(global.OriginX + Convert.ToInt32(global.GScale*(location.x - global.PIDControllerInitRadius)), 
                    global.OriginY + Convert.ToInt32(global.GScale*(location.y - global.PIDControllerInitRadius))), 
            new Point(global.OriginX + Convert.ToInt32(global.GScale*(location.x + global.PIDControllerInitRadius)), 
                    global.OriginY + Convert.ToInt32(global.GScale*(location.y - global.PIDControllerInitRadius))), 
            new Point(global.OriginX + Convert.ToInt32(global.GScale*(location.x + global.PIDControllerInitRadius)),
                    global.OriginY + Convert.ToInt32(global.GScale*(location.y + global.PIDControllerInitRadius)))};
            plot1.AddPolygon(myArray);
            plotPen.Color = Color.Black;

            SolidBrush brush = new SolidBrush(Color.White);
            if (highlighted) { brush.Color = Color.Orange; }
            G.FillPath(brush, plot1);
            G.DrawPath(plotPen, plot1);

            base.draw(G);
        }

    }
}
