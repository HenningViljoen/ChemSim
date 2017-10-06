using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Reflection;

namespace chemsim
{
    public enum drawingmode { None, Streams, Signals, GasPipes, LiquidPipes };

    public partial class Form1 : Form
    {
        public simulation sim;
        public Graphics dbGraphics;
        public Graphics panelGraphics;
        private List<Form> detailtrends;
        Bitmap dbBitmap;
        Bitmap panelBitmap;
        drawingmode dmode;
        private bool activedrawing; //road is being drawn since one click already occurred.
        private point defaultlocation;
        private int tankhoveringover;
        private int pointpointedto; //especially for the traffic light case
        private int pumphoveringover;
        private int streamhoveringover;

        public Form1()
        {
            InitializeComponent();

            //Additional changes to the Forms
            this.toolStripMenuItem1.Click += new System.EventHandler(this.toolStripMenuItem1_Click);
            this.toolStripMenuItem2.Click += new System.EventHandler(this.toolStripMenuItem2_Click);
            this.toolStripMenuItem3.Click += new System.EventHandler(this.toolStripMenuItem3_Click);
            this.toolStripMenuItem4.Click += new System.EventHandler(this.toolStripMenuItem4_Click);

            //Own code

            this.SetStyle(ControlStyles.Opaque, true);
            typeof(Panel).InvokeMember("DoubleBuffered",
                BindingFlags.SetProperty | BindingFlags.Instance | BindingFlags.NonPublic,
                null, panel1, new object[] { true });
            panel1.Dock = DockStyle.Fill;
            timer1.Interval = global.TimerInterval;

            dbBitmap = null;
            panelBitmap = null;
            dbGraphics = null;
            panelGraphics = null;
            dmode = drawingmode.None;

            Form1_Resize(null, EventArgs.Empty);
            panel1_Resize(null, EventArgs.Empty);
            sim = new simulation();
            detailtrends = new List<Form>();

            panelGraphics.Clear(Color.White);
            //Invalidate();

            panel1.Invalidate();

            Focus();

            //toolStrip1.Anchor = AnchorStyles.None;
            //toolStrip1.Location = new Point(0, 0);

        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            // Get rid of old stuff
            if (dbGraphics != null)
            {
                dbGraphics.Dispose();
            }

            if (dbBitmap != null)
            {
                dbBitmap.Dispose();
            }

            if (ClientRectangle.Width > 0 && ClientRectangle.Height > 0)
            {
                // Create a bitmap
                dbBitmap = new Bitmap(ClientRectangle.Width,
                                        ClientRectangle.Height);

                // Grab its Graphics
                dbGraphics = Graphics.FromImage(dbBitmap);

                // Set up initial translation after resize (also at start)
                //dbGraphics->TranslateTransform((float)X, 25.0);
            }
        }

        private void panel1_Resize(object sender, EventArgs e)
        {
            if (panelGraphics != null)
            {
                panelGraphics.Dispose();
            }

            if (panelBitmap != null)
            {
                panelBitmap.Dispose();
            }

            if (panel1.ClientRectangle.Width > 0 && panel1.ClientRectangle.Height > 0)
            {
                // Create a bitmap
                panelBitmap = new Bitmap(panel1.AutoScrollMinSize.Width,
                                        panel1.AutoScrollMinSize.Height);

                // Grab its Graphics
                panelGraphics = Graphics.FromImage(panelBitmap);

                // Set up initial translation after resize (also at start)
                //dbGraphics->TranslateTransform((float)X, 25.0);
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (sim.simi >= global.SimIterations)
            {
                //doreset();  I would rather not do this automatically, only when Simulate is clicked.
            }
            else
            {
                sim.simulate(panelGraphics, detailtrends);

                toolStripLabel1.Text = sim.simtime.timerstring();
                panel1.Invalidate();
            }
        }

        private void toolStripButton31_Click(object sender, EventArgs e) //SimFast button click event.
        {
            //if (sim.simi >= global.SimIterations)
            //{
                doreset();
            //}
            sim.setsimulating(true);
            //sim.simulate(panelGraphics, detailtrends);

            toolStripLabel1.Text = sim.simtime.timerstring();
            Invalidate();

            for (int i = 0; i < global.SimIterations; i++)
            {
                sim.simulate(panelGraphics, null);

                //toolStripLabel1.Text = sim.simtime.timerstring();
                //panel1.Invalidate();

            }

            toolStripLabel1.Text = sim.simtime.timerstring();
            panel1.Invalidate();
            sim.simi--;

            //doreset();
            //for (sim.simi = 0; sim.simi < global.SimIterations; sim.simi++)
            //{

                for (int i = 0; i < detailtrends.Count; i++)
                {
                    if (detailtrends[i] != null && detailtrends[i].Visible)
                    {
                        detailtrends[i].Invalidate();
                        detailtrends[i].Update();

                        //if (sim.simi < global.SimIterations - 1)
                        //{
                        //    detailtrends[i].Invalidate();
                        //    detailtrends[i].Update();
                        //}
                        //else
                        //{
                        //    detailtrends[i].Invalidate();
                        //    detailtrends[i].Update();
                        //}

                    }
                }
            //}
                //toolStripLabel1.Text = sim.simtime.timerstring();
                

          
            //panel1.Invalidate();
            //doreset();
        }

        private void Form1_Paint(object sender, PaintEventArgs e) 
        {
            e.Graphics.TranslateTransform((float)AutoScrollPosition.X, (float)AutoScrollPosition.Y);
            //toolStrip1.Location = new Point((int)-e.Graphics.Transform.OffsetX, (int)-e.Graphics.Transform.OffsetY);
            e.Graphics.DrawImageUnscaled(dbBitmap, 0, 0);
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.TranslateTransform((float)panel1.AutoScrollPosition.X, (float)panel1.AutoScrollPosition.Y);
            //toolStrip1.Location = new Point((int)-e.Graphics.Transform.OffsetX, (int)-e.Graphics.Transform.OffsetY);
            e.Graphics.DrawImageUnscaled(panelBitmap, 0, 0);
        }

        private void Form1_MouseDown(object sender, MouseEventArgs e) //Run when a mouse button is 
                                                                      //pressed down.
        {
            
        }

        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            if (!sim.simulating)
            {
                int pointerx = e.X + (-panel1.AutoScrollPosition.X);
                int pointery = e.Y + (-panel1.AutoScrollPosition.Y);
                if (toolStripButton1.Checked || toolStripButton2.Checked || toolStripButton8.Checked || toolStripButton26.Checked ||
                   toolStripButton11.Checked || toolStripButton13.Checked || toolStripButton14.Checked || toolStripButton16.Checked || toolStripButton20.Checked ||
                    toolStripButton21.Checked || toolStripButton22.Checked || toolStripButton23.Checked || toolStripButton28.Checked || 
                    toolStripButton29.Checked)
                //Tank || Pump || Valve || FTReactor || Tee || Mixer || DistColmn || SteamGen || HX || HXSimple || CTSimple ||
                    // CTHESbutton || CT button is checked.
                {
                    for (int i = 0; i < sim.unitops[sim.unitops.Count - 1].nin; i++)
                    {
                        if (sim.unitops[sim.unitops.Count - 1].inpoint[i].highlighted)
                        {
                            sim.unitops[sim.unitops.Count - 1].inflow[i].points[1].x = sim.unitops[sim.unitops.Count - 1].inpoint[i].x;
                            sim.unitops[sim.unitops.Count - 1].inflow[i].points[1].y = sim.unitops[sim.unitops.Count - 1].inpoint[i].y;
                            sim.unitops[sim.unitops.Count - 1].inpoint[i].highlighted = false;
                        }
                    }

                    for (int i = 0; i < sim.unitops[sim.unitops.Count - 1].nout; i++)
                    {
                        if (sim.unitops[sim.unitops.Count - 1].outpoint[i].highlighted)
                        {
                            sim.unitops[sim.unitops.Count - 1].outflow[i].points[0].x = sim.unitops[sim.unitops.Count - 1].outpoint[i].x;
                            sim.unitops[sim.unitops.Count - 1].outflow[i].points[0].y = sim.unitops[sim.unitops.Count - 1].outpoint[i].y;
                            sim.unitops[sim.unitops.Count - 1].outpoint[i].highlighted = false;
                        }
                    }
                }

                if (toolStripButton1.Checked) { addnewtank(); }         //Tank button

                if (toolStripButton2.Checked) { addnewpump(); }         //Pump button

                if (toolStripButton8.Checked) { addnewvalve(); }          //Valve button 

                if (toolStripButton26.Checked) { addnewflange(); }          //Flange button 

                if (toolStripButton11.Checked) { addnewftreactor(); }          //FTReactor button

                if (toolStripButton16.Checked) { addnewdistillationcolumn(); }          //Distillation Column button

                if (toolStripButton20.Checked) { addnewsteamgenerator(); } //Steam Generator button

                if (toolStripButton21.Checked) { addnewheatexchanger(); } //HeatEx button

                if (toolStripButton22.Checked) { addnewheatexchangersimple(); } //HXSimple button

                if (toolStripButton29.Checked) { addnewcoolingtower(); } //CT button

                if (toolStripButton28.Checked) { addnewcoolingtowerheatexchangersimple(); } //CTHESimple button

                if (toolStripButton23.Checked) { addnewcoolingtowersimple(); } //CTSimple button

                if (toolStripButton13.Checked) { addnewtee(); }         //Tee button

                if (toolStripButton14.Checked) { addnewmixer(); }          //Mixer button

                if (toolStripButton15.Checked) { addnewpidcontroller(); }          //PIDController button

                if (toolStripButton33.Checked) { addnewcontrolmvsignalsplitter(); }          //MVSplitter button
                
                if (toolStripButton27.Checked) { addnewnmpccontroller(); }          //NMPCController button

                if (toolStripButton3.Checked)           //stream button
                {
                    point dynamicpoint = null;

                    if (dmode == drawingmode.Streams)       //Already drawing one stream
                    {
                        if (e.Button == System.Windows.Forms.MouseButtons.Left)
                        {
                            dmode = drawingmode.None;

                            for (int i = 0; i < sim.unitops.Count; i++)
                            {
                                for (int j = 0; j < sim.unitops[i].nin; j++)
                                {
                                    if (sim.unitops[i].inpoint[j].highlighted)
                                    {
                                        dynamicpoint = new point(sim.unitops[i].inpoint[j].x, sim.unitops[i].inpoint[j].y);
                                        sim.unitops[i].inflow[j] = sim.streams[sim.streams.Count - 1];
                                    }
                                }
                            }

                            if (dynamicpoint == null)
                            {
                                dynamicpoint = new point((pointerx - global.OriginX) / global.GScale, (pointery - global.OriginY) / global.GScale);
                            }

                            sim.streams[sim.streams.Count - 1].points[1].copyfrom(dynamicpoint);

                        }
                        else if (e.Button == System.Windows.Forms.MouseButtons.Right)
                        {
                            sim.streams[sim.streams.Count - 1].inbetweenpoints.Add(
                                new point((pointerx - global.OriginX) / global.GScale, (pointery - global.OriginY) / global.GScale));
                        }
                    }
                    else //A new stream is just going to be started to be drawn now
                    {
                        dmode = drawingmode.Streams;

                        for (int i = 0; i < sim.unitops.Count; i++)
                        {
                            for (int j = 0; j < sim.unitops[i].nout; j++)
                            {
                                if (sim.unitops[i].outpoint[j].highlighted)
                                {
                                    dynamicpoint = new point(sim.unitops[i].outpoint[j].x, sim.unitops[i].outpoint[j].y);
                                    sim.streams.Add(new stream(sim.streams.Count, dynamicpoint.x, dynamicpoint.y, dynamicpoint.x,
                                        dynamicpoint.y));
                                    sim.unitops[i].outflow[j] = sim.streams[sim.streams.Count - 1];
                                }
                            }
                        }

                        if (dynamicpoint == null)
                        {
                            dynamicpoint = new point((pointerx - global.OriginX) / global.GScale, (pointery - global.OriginY) / global.GScale);
                            sim.streams.Add(new stream(sim.streams.Count, dynamicpoint.x, dynamicpoint.y, dynamicpoint.x,
                                dynamicpoint.y));
                        }


                    }
                }

                if (toolStripButton32.Checked)           //Signal button
                {
                    point dynamicpoint = null;

                    if (dmode == drawingmode.Signals)       //Already drawing one signal
                    {
                        if (e.Button == System.Windows.Forms.MouseButtons.Left)
                        {
                            dmode = drawingmode.None;

                            //for (int i = 0; i < sim.unitops.Count; i++)
                            //{
                            //    for (int j = 0; j < sim.unitops[i].nin; j++)
                            //    {
                            //        if (sim.unitops[i].inpoint[j].highlighted)
                            //        {
                            //            dynamicpoint = new point(sim.unitops[i].inpoint[j].x, sim.unitops[i].inpoint[j].y);
                            //            sim.unitops[i].inflow[j] = sim.streams[sim.streams.Count - 1];
                            //        }
                            //    }
                            //}

                            if (dynamicpoint == null)
                            {
                                dynamicpoint = new point((pointerx - global.OriginX) / global.GScale, (pointery - global.OriginY) / global.GScale);
                            }

                            sim.signals[sim.signals.Count - 1].points[1].copyfrom(dynamicpoint);

                        }
                        else if (e.Button == System.Windows.Forms.MouseButtons.Right)
                        {
                            sim.signals[sim.signals.Count - 1].inbetweenpoints.Add(
                                new point((pointerx - global.OriginX) / global.GScale, (pointery - global.OriginY) / global.GScale));
                        }
                    }
                    else //A new signal is just going to be started to be drawn now
                    {
                        dmode = drawingmode.Signals;

                        //for (int i = 0; i < sim.unitops.Count; i++)
                        //{
                        //    for (int j = 0; j < sim.unitops[i].nout; j++)
                        //    {
                        //        if (sim.unitops[i].outpoint[j].highlighted)
                        //        {
                        //            dynamicpoint = new point(sim.unitops[i].outpoint[j].x, sim.unitops[i].outpoint[j].y);
                        //            sim.streams.Add(new stream(sim.streams.Count, dynamicpoint.x, dynamicpoint.y, dynamicpoint.x,
                        //                dynamicpoint.y));
                        //            sim.unitops[i].outflow[j] = sim.streams[sim.streams.Count - 1];
                        //        }
                        //    }
                        //}

                        if (dynamicpoint == null)
                        {
                            dynamicpoint = new point((pointerx - global.OriginX) / global.GScale, (pointery - global.OriginY) / global.GScale);
                            sim.signals.Add(new signal(sim.signals.Count, dynamicpoint.x, dynamicpoint.y, dynamicpoint.x,
                                dynamicpoint.y));
                        }


                    }
                }

                if (toolStripButton9.Checked) //GasPipe button
                {
                    point dynamicpoint = null;

                    if (dmode == drawingmode.GasPipes) //Already drawing one gaspipe
                    {
                        dmode = drawingmode.None;

                        if (dynamicpoint == null)
                        {
                            dynamicpoint = new point((pointerx - global.OriginX) / global.GScale, (pointery - global.OriginY) / global.GScale);
                        }

                        sim.unitops[sim.unitops.Count - 1].points[1].copyfrom(dynamicpoint);

                        for (int i = 0; i < sim.streams.Count; i++)
                        {
                            if (sim.streams[i].highlighted)
                            {
                                //sim.unitops[sim.unitops.Count - 1].inflow[0] = sim.streams[i];
                                sim.unitops[sim.unitops.Count - 1].outflow[0].points[0].x = sim.unitops[sim.unitops.Count - 1].points[1].x;
                                sim.unitops[sim.unitops.Count - 1].outflow[0].points[0].y = sim.unitops[sim.unitops.Count - 1].points[1].y;
                                sim.streams[i].highlighted = false;
                            }
                        }

                    }
                    else //A new gas pipe is just going to be started to be drawn now
                    {
                        dmode = drawingmode.GasPipes;

                        if (dynamicpoint == null)
                        {
                            dynamicpoint = new point((pointerx - global.OriginX) / global.GScale, (pointery - global.OriginY) / global.GScale);
                            sim.unitops.Add(new gaspipe(sim.unitops.Count, dynamicpoint.x, dynamicpoint.y, dynamicpoint.x,
                                dynamicpoint.y, global.PipeDefaultLength,
                                global.PipeDefaultDiameter, global.PipeDefaultFiLocation));

                            for (int i = 0; i < sim.streams.Count; i++)
                            {
                                if (sim.streams[i].highlighted)
                                {
                                    sim.unitops[sim.unitops.Count - 1].inflow[0] = sim.streams[i];
                                    sim.unitops[sim.unitops.Count - 1].inflow[0].points[1].x = sim.unitops[sim.unitops.Count - 1].inpoint[0].x;
                                    sim.unitops[sim.unitops.Count - 1].inflow[0].points[1].y = sim.unitops[sim.unitops.Count - 1].inpoint[0].y;
                                    sim.streams[i].highlighted = false;
                                }
                            }

                            //public gaspipe(double p0x, double p0y, double p1x, double p1y, double apressure, double atemperature, 
                            //double alength, double adiameter, 
                            //double afilocation)
                        }
                    }
                }


                if (toolStripButton10.Checked) //LiquidPipe button
                {
                    point dynamicpoint = null;

                    if (dmode == drawingmode.LiquidPipes) //Already drawing one liquidpipe
                    {
                        dmode = drawingmode.None;

                        if (dynamicpoint == null)
                        {
                            dynamicpoint = new point((pointerx - global.OriginX) / global.GScale, (pointery - global.OriginY) / global.GScale);
                        }

                        sim.unitops[sim.unitops.Count - 1].points[1].copyfrom(dynamicpoint);

                        for (int i = 0; i < sim.streams.Count; i++)
                        {
                            if (sim.streams[i].highlighted)
                            {
                                //sim.unitops[sim.unitops.Count - 1].inflow[0] = sim.streams[i];
                                sim.unitops[sim.unitops.Count - 1].outflow[0].points[0].x = sim.unitops[sim.unitops.Count - 1].points[1].x;
                                sim.unitops[sim.unitops.Count - 1].outflow[0].points[0].y = sim.unitops[sim.unitops.Count - 1].points[1].y;
                                sim.streams[i].highlighted = false;
                            }
                        }
                    }
                    else //A new gas pipe is just going to be started to be drawn now
                    {
                        dmode = drawingmode.LiquidPipes;

                        if (dynamicpoint == null)
                        {
                            dynamicpoint = new point((pointerx - global.OriginX) / global.GScale, (pointery - global.OriginY) / global.GScale);
                            sim.unitops.Add(new liquidpipe(sim.unitops.Count, dynamicpoint.x, dynamicpoint.y, dynamicpoint.x,
                                dynamicpoint.y, global.PipeDefaultTemperature, global.PipeDefaultLength,
                                global.PipeDefaultDiameter, global.PipeDefaultFiLocation, liquidpipeflowreference.PipeEntrance));

                            for (int i = 0; i < sim.streams.Count; i++)
                            {
                                if (sim.streams[i].highlighted)
                                {
                                    sim.unitops[sim.unitops.Count - 1].inflow[0] = sim.streams[i];
                                    sim.unitops[sim.unitops.Count - 1].inflow[0].points[1].x = sim.unitops[sim.unitops.Count - 1].inpoint[0].x;
                                    sim.unitops[sim.unitops.Count - 1].inflow[0].points[1].y = sim.unitops[sim.unitops.Count - 1].inpoint[0].y;
                                    sim.streams[i].highlighted = false;
                                }
                            }

                            //public liquidpipe(double p0x, double p0y, double p1x, double p1y, double atemperature, 
                            //double alength, double adiameter, double afilocation, string aflowreference)
                        }


                    }
                }


                else if (e.Button == System.Windows.Forms.MouseButtons.Right) //Right mouse button and no mode
                {

                    for (int i = 0; i < sim.unitops.Count; i++)
                    {
                        if (sim.unitops[i].highlighted) { contextMenuStrip1.Show(e.X, e.Y); }
                    }

                    for (int i = 0; i < sim.streams.Count; i++)
                    {
                        if (sim.streams[i].highlighted) { contextMenuStrip1.Show(e.X, e.Y); }
                    }

                    for (int i = 0; i < sim.signals.Count; i++)
                    {
                        if (sim.signals[i].highlighted) { contextMenuStrip1.Show(e.X, e.Y); }
                    }

                    for (int i = 0; i < sim.pidcontrollers.Count; i++)
                    {
                        if (sim.pidcontrollers[i].highlighted) { contextMenuStrip1.Show(e.X, e.Y); }
                    }

                    for (int i = 0; i < sim.blocks.Count; i++)
                    {
                        if (sim.blocks[i].highlighted) { contextMenuStrip1.Show(e.X, e.Y); }
                    }

                    for (int i = 0; i < sim.nmpccontrollers.Count; i++)
                    {
                        if (sim.nmpccontrollers[i].highlighted) { contextMenuStrip1.Show(e.X, e.Y); }
                    }
                }
                
            }
              
        }


        private int tankover(double x, double y)
        {
            int tanko = -1; //default - implying that the mouse is actually currently not over any tank.

            return tanko;
        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            
        }

        private void panel1_MouseMove(object sender, MouseEventArgs e)
        {
            if (!sim.simulating)
            {
                if (sim.nmpccontrollers == null) { sim.nmpccontrollers = new List<nmpc>(); } //THIS LINE SHOULD AT SOME POINT BE DELETED WHEN THE MODEL FILE HAS 
                //BEEN RECREATED WITH THE LATEST VERSION OF THIS CLASS.

                int pointerx = e.X + (-panel1.AutoScrollPosition.X);
                int pointery = e.Y + (-panel1.AutoScrollPosition.Y);
                double xonplant = (pointerx - global.OriginX) / global.GScale;
                double yonplant = (pointery - global.OriginY) / global.GScale;
                if (toolStripButton1.Checked || toolStripButton2.Checked || toolStripButton3.Checked ||
                    toolStripButton8.Checked || toolStripButton26.Checked || toolStripButton9.Checked || toolStripButton10.Checked ||
                    toolStripButton11.Checked || toolStripButton13.Checked || toolStripButton14.Checked ||
                    toolStripButton15.Checked || toolStripButton27.Checked || toolStripButton16.Checked || toolStripButton20.Checked ||
                    toolStripButton21.Checked || toolStripButton22.Checked || toolStripButton23.Checked || toolStripButton28.Checked ||
                    toolStripButton29.Checked || toolStripButton32.Checked || toolStripButton33.Checked)
                //tank, pump, stream, valve, flange, GasPipe, FTReactor, Tee button, PID Controller, NMPC, Mixer, DistColmn , SteamGen, HeatEx, 
                    //HXSimple, CTSimple, CTHESimple, CT, Signal, MVSplitter button checked
                {
                    if (toolStripButton1.Checked || toolStripButton2.Checked || toolStripButton8.Checked || toolStripButton26.Checked ||
                        toolStripButton11.Checked || toolStripButton13.Checked || toolStripButton14.Checked ||
                        toolStripButton16.Checked || toolStripButton20.Checked || toolStripButton21.Checked ||
                        toolStripButton22.Checked || toolStripButton23.Checked || toolStripButton28.Checked || toolStripButton29.Checked)
                    //tank, pump, valve, flange, FTReactor, Tee, Mixer, DistColmn, HeatEx, HXSimple, CTSimple, CTHESimple button
                    {
                        sim.unitops[sim.unitops.Count - 1].location.x = (pointerx - global.OriginX) / global.GScale;
                        sim.unitops[sim.unitops.Count - 1].location.y = (pointery - global.OriginY) / global.GScale;
                        //roadhoveringover = roadover(sim.cars[sim.cars.Count - 1],
                        //    ref pointpointedto);
                        sim.unitops[sim.unitops.Count - 1].updateinoutpointlocations();

                        int i = 0;
                        bool exitwhile = false;
                        while (i < sim.streams.Count && !exitwhile)
                        {
                            for (int j = 0; j < sim.unitops[sim.unitops.Count - 1].nin; j++)
                            {
                                if (utilities.distance((sim.streams[i].points[1].y - sim.unitops[sim.unitops.Count - 1].inpoint[j].y),
                                    (sim.streams[i].points[1].x - sim.unitops[sim.unitops.Count - 1].inpoint[j].x)) <=
                                    global.MinDistanceFromPoint)
                                {
                                    sim.unitops[sim.unitops.Count - 1].inpoint[j].highlighted = true;
                                    sim.unitops[sim.unitops.Count - 1].inflow[j] = sim.streams[i];
                                    exitwhile = true;
                                }
                                else
                                {
                                    sim.unitops[sim.unitops.Count - 1].inpoint[j].highlighted = false;
                                    sim.unitops[sim.unitops.Count - 1].inflow[j] = null;

                                }
                            }

                            if (i < sim.streams.Count)
                            {
                                for (int j = 0; j < sim.unitops[sim.unitops.Count - 1].nout; j++)
                                {
                                    if (utilities.distance((sim.streams[i].points[0].y - sim.unitops[sim.unitops.Count - 1].outpoint[j].y),
                                        (sim.streams[i].points[0].x - sim.unitops[sim.unitops.Count - 1].outpoint[j].x)) <=
                                        global.MinDistanceFromPoint)
                                    {
                                        sim.unitops[sim.unitops.Count - 1].outpoint[j].highlighted = true;
                                        sim.unitops[sim.unitops.Count - 1].outflow[j] = sim.streams[i];
                                        exitwhile = true;
                                    }
                                    else
                                    {
                                        sim.unitops[sim.unitops.Count - 1].outpoint[j].highlighted = false;
                                        sim.unitops[sim.unitops.Count - 1].outflow[j] = null;

                                    }
                                }
                            }

                            i++;

                        }
                    }

                    if (toolStripButton15.Checked) //PID controller
                    {
                        sim.pidcontrollers[sim.pidcontrollers.Count - 1].location.x = (pointerx - global.OriginX) / global.GScale;
                        sim.pidcontrollers[sim.pidcontrollers.Count - 1].location.y = (pointery - global.OriginY) / global.GScale;
                    }

                    if (toolStripButton33.Checked) //MVSplitter controller
                    {
                        sim.blocks[sim.blocks.Count - 1].location.x = (pointerx - global.OriginX) / global.GScale;
                        sim.blocks[sim.blocks.Count - 1].location.y = (pointery - global.OriginY) / global.GScale;
                    }

                    

                    if (toolStripButton27.Checked) //NMPC controller
                    {
                        sim.nmpccontrollers[sim.nmpccontrollers.Count - 1].location.x = (pointerx - global.OriginX) / global.GScale;
                        sim.nmpccontrollers[sim.nmpccontrollers.Count - 1].location.y = (pointery - global.OriginY) / global.GScale;
                    }

                    if (toolStripButton3.Checked) //Stream button
                    {
                        for (int i = 0; i < sim.unitops.Count; i++)
                        {
                            for (int j = 0; j < sim.unitops[i].nin; j++)
                            {
                                if (utilities.distance(((pointery - global.OriginY) / global.GScale - sim.unitops[i].inpoint[j].y),
                                    ((pointerx - global.OriginX) / global.GScale - sim.unitops[i].inpoint[j].x)) <=
                                    global.MinDistanceFromPoint)
                                {
                                    sim.unitops[i].inpoint[j].highlighted = true;

                                }
                                else
                                {
                                    sim.unitops[i].inpoint[j].highlighted = false;
                                }
                            }
                            for (int j = 0; j < sim.unitops[i].nout; j++)
                            {
                                if (utilities.distance(((pointery - global.OriginY) / global.GScale - sim.unitops[i].outpoint[j].y),
                                    ((pointerx - global.OriginX) / global.GScale - sim.unitops[i].outpoint[j].x)) <=
                                    global.MinDistanceFromPoint)
                                {
                                    sim.unitops[i].outpoint[j].highlighted = true;
                                }
                                else
                                {
                                    sim.unitops[i].outpoint[j].highlighted = false;
                                }
                            }
                        }

                        if (dmode == drawingmode.Streams)
                        {
                            sim.streams[sim.streams.Count - 1].updatepoint(1, (pointerx - global.OriginX) / global.GScale, (pointery - global.OriginY) / global.GScale);

                            //sim->roads[0]->draw(dbGraphics);
                        }
                    }

                    if (toolStripButton32.Checked) //Signal button
                    {

                        if (dmode == drawingmode.Signals)
                        {
                            sim.signals[sim.signals.Count - 1].updatepoint(1, (pointerx - global.OriginX) / 
                                global.GScale, (pointery - global.OriginY) / global.GScale);

                            //sim->roads[0]->draw(dbGraphics);
                        }
                    }

                    if (toolStripButton9.Checked || toolStripButton10.Checked) //GasPipe button, or LiquidPipe button
                    {
                        if (dmode == drawingmode.None)
                        {
                            int i = 0;
                            bool exitwhile = false;
                            while (i < sim.streams.Count && !exitwhile)
                            {
                                if (utilities.distance((sim.streams[i].points[1].y - (pointery - global.OriginY) / global.GScale),
                                    (sim.streams[i].points[1].x - (pointerx - global.OriginX) / global.GScale)) <=
                                    global.MinDistanceFromPoint)
                                {
                                    sim.streams[i].highlighted = true;
                                    //sim.unitops[sim.unitops.Count - 1].inflow[j] = sim.streams[i];
                                    exitwhile = true;
                                }
                                else
                                {
                                    sim.streams[i].highlighted = false;
                                    //sim.unitops[sim.unitops.Count - 1].inflow[j] = null;
                                }
                                i++;
                            }
                        }

                        if (dmode == drawingmode.GasPipes || dmode == drawingmode.LiquidPipes)
                        {
                            sim.unitops[sim.unitops.Count - 1].updatepoint(1, (pointerx - global.OriginX) / global.GScale, (pointery - global.OriginY) / global.GScale);

                            int i = 0;
                            bool exitwhile = false;
                            while (i < sim.streams.Count && !exitwhile)
                            {
                                if (utilities.distance((sim.streams[i].points[0].y - (pointery - global.OriginY) / global.GScale),
                                    (sim.streams[i].points[0].x - (pointerx - global.OriginX) / global.GScale)) <=
                                    global.MinDistanceFromPoint)
                                {
                                    sim.streams[i].highlighted = true;
                                    sim.unitops[sim.unitops.Count - 1].outflow[0] = sim.streams[i];
                                    exitwhile = true;
                                }
                                else
                                {
                                    sim.streams[i].highlighted = false;
                                    sim.unitops[sim.unitops.Count - 1].outflow[0] = null;
                                }
                                i++;
                            }
                        }
                    }

                }

                else //selection and editing of properties mode, and moving if left mouse button is pressed.
                {

                    for (int i = 0; i < sim.unitops.Count; i++)
                    {
                        if (sim.unitops[i].mouseover((pointerx - global.OriginX) / global.GScale, (pointery - global.OriginY) / global.GScale))
                        {
                            sim.unitops[i].highlighted = true;
                            if (e.Button == System.Windows.Forms.MouseButtons.Left)
                            {
                                sim.unitops[i].location.x = (pointerx - global.OriginX) / global.GScale;
                                sim.unitops[i].location.y = (pointery - global.OriginY) / global.GScale;
                            }
                        }
                        else { sim.unitops[i].highlighted = false; }
                    }

                    for (int i = 0; i < sim.streams.Count; i++)
                    {
                        if (sim.streams[i].mouseover((pointerx - global.OriginX) / global.GScale, (pointery - global.OriginY) / global.GScale))
                        {
                            sim.streams[i].highlighted = true;
                            if (e.Button == System.Windows.Forms.MouseButtons.Left)
                            {
                                point onplant = new point(xonplant,yonplant);
                                int pointtomove;
                                if (utilities.distance(sim.streams[i].points[0], onplant) < utilities.distance(sim.streams[i].points[1], onplant))
                                {
                                    pointtomove = 0;
                                }
                                else
                                {
                                    pointtomove = 1;
                                }

                                sim.streams[i].updatepoint(pointtomove,xonplant,yonplant);
                            }
                        }
                        else { sim.streams[i].highlighted = false; }
                    }

                    for (int i = 0; i < sim.signals.Count; i++)
                    {
                        if (sim.signals[i].mouseover((pointerx - global.OriginX) / global.GScale, (pointery - global.OriginY) / global.GScale))
                        {
                            sim.signals[i].highlighted = true;
                            if (e.Button == System.Windows.Forms.MouseButtons.Left)
                            {
                                point onplant = new point(xonplant, yonplant);
                                int pointtomove;
                                if (utilities.distance(sim.signals[i].points[0], onplant) < utilities.distance(sim.signals[i].points[1], onplant))
                                {
                                    pointtomove = 0;
                                }
                                else
                                {
                                    pointtomove = 1;
                                }

                                sim.signals[i].updatepoint(pointtomove, xonplant, yonplant);
                            }
                        }
                        else { sim.signals[i].highlighted = false; }
                    }

                    for (int i = 0; i < sim.pidcontrollers.Count; i++)
                    {
                        if (sim.pidcontrollers[i].mouseover((pointerx - global.OriginX) / global.GScale, (pointery - global.OriginY) / global.GScale))
                        {
                            sim.pidcontrollers[i].highlighted = true;
                            if (e.Button == System.Windows.Forms.MouseButtons.Left)
                            {
                                sim.pidcontrollers[i].location.x = (pointerx - global.OriginX) / global.GScale;
                                sim.pidcontrollers[i].location.y = (pointery - global.OriginY) / global.GScale;
                            }
                        }
                        else { sim.pidcontrollers[i].highlighted = false; }
                    }

                    if (sim.blocks != null)
                    {
                        for (int i = 0; i < sim.blocks.Count; i++)
                        {
                            if (sim.blocks[i].mouseover((pointerx - global.OriginX) / global.GScale, (pointery - global.OriginY) / global.GScale))
                            {
                                sim.blocks[i].highlighted = true;
                                if (e.Button == System.Windows.Forms.MouseButtons.Left)
                                {
                                    sim.blocks[i].location.x = (pointerx - global.OriginX) / global.GScale;
                                    sim.blocks[i].location.y = (pointery - global.OriginY) / global.GScale;
                                }
                            }
                            else { sim.blocks[i].highlighted = false; }
                        }
                    }

                    for (int i = 0; i < sim.nmpccontrollers.Count; i++)
                    {
                        if (sim.nmpccontrollers[i].mouseover((pointerx - global.OriginX) / global.GScale, (pointery - global.OriginY) / global.GScale))
                        {
                            sim.nmpccontrollers[i].highlighted = true;
                            if (e.Button == System.Windows.Forms.MouseButtons.Left)
                            {
                                sim.nmpccontrollers[i].location.x = (pointerx - global.OriginX) / global.GScale;
                                sim.nmpccontrollers[i].location.y = (pointery - global.OriginY) / global.GScale;
                            }
                        }
                        else { sim.nmpccontrollers[i].highlighted = false; }
                    }

                }
                sim.drawnetwork(panelGraphics);
                panel1.Invalidate();
            }
        }

        private void addnewtank()
        {
            sim.unitops.Add(new tank(sim.unitops.Count, defaultlocation.x, defaultlocation.y,
                global.TankInitFracInventory, global.TankInitRadius, global.TankInitHeight));

            //tank(double ax, double ay, double amaxvolume, double ainventory, double aradius, double aheight, double adensity) 
        }

        private void addnewftreactor()
        {
            sim.unitops.Add(new ftreactor(sim.unitops.Count, defaultlocation.x, defaultlocation.y,
                global.FTReactorInitInventory));

            //ftreactor(double ax, double ay, double apercinventory)
        }

        private void addnewdistillationcolumn()
        {
            sim.unitops.Add(new distillationcolumn(sim.unitops.Count, defaultlocation.x, defaultlocation.y));

            //distillationcolumn(int anr, double ax, double ay)
        }

        private void addnewsteamgenerator()
        {
            sim.unitops.Add(new steamgenerator(sim.unitops.Count, defaultlocation.x, defaultlocation.y));

            //steamgenerator(int anr, double ax, double ay)
        }

        private void addnewheatexchanger()
        {
            sim.unitops.Add(new heatexchanger(sim.unitops.Count, defaultlocation.x, defaultlocation.y));
        }

        private void addnewheatexchangersimple()
        {
            sim.unitops.Add(new heatexchangersimple(sim.unitops.Count, defaultlocation.x, defaultlocation.y));
        }

        private void addnewcoolingtower()
        {
            sim.unitops.Add(new coolingtower(sim.unitops.Count, defaultlocation.x, defaultlocation.y));
        }

        private void addnewcoolingtowerheatexchangersimple()
        {
            sim.unitops.Add(new coolingtowerheatexchangersimple(sim.unitops.Count, defaultlocation.x, defaultlocation.y));
        }

        private void addnewcoolingtowersimple()
        {
            sim.unitops.Add(new coolingtowersimple(sim.unitops.Count, defaultlocation.x, defaultlocation.y));
        }

        private void addnewpump()
        {
            sim.unitops.Add(new pump(sim.unitops.Count, defaultlocation.x, defaultlocation.y, global.PumpInitMaxDeltaPressure, global.PumpInitMinDeltaPressure,
                global.PumpInitMaxActualFlow, global.PumpInitActualVolumeFlow, global.PumpInitOn));

            //pump(double ax, double ay, double amaxdeltapressure, double amaxactualflow, double anactualvolumeflow, bool aon)
        }

        private void addnewvalve()
        {
            sim.unitops.Add(new valve(sim.unitops.Count, defaultlocation.x, defaultlocation.y, global.ValveDefaultCv,
                global.ValveDefaultOpening));

            //valve(double ax, double ay, double aCv, double aop) : base(ax, ay, 1, 1)
        }

        private void addnewflange()
        {
            
            sim.unitops.Add(new flange(sim.unitops.Count, defaultlocation.x, defaultlocation.y));
            //flange(int anr, double ax, double ay)
        }

        private void addnewtee()
        {
            sim.unitops.Add(new tee(sim.unitops.Count, defaultlocation.x, defaultlocation.y, global.TeeDefaultNOut));

            //tee(double ax, double ay, int anout) : base(ax, ay, 1, anout)
        }

        private void addnewmixer()
        {
            sim.unitops.Add(new mixer(sim.unitops.Count, defaultlocation.x, defaultlocation.y, global.MixerDefaultNIn));

            //mixer(double ax, double ay, int anin) : base(ax, ay, anin, 1)
        }

        private void addnewpidcontroller()
        {
            sim.pidcontrollers.Add(new pidcontroller(sim.unitops.Count, defaultlocation.x, defaultlocation.y, global.PIDControllerInitK,
                global.PIDControllerInitI, global.PIDControllerInitD, global.PIDControllerInitMinPV, global.PIDControllerInitMinPV,
                global.PIDControllerInitMaxPV, global.PIDControllerInitMinOP, global.PIDControllerInitMaxOP));

            //public pidcontroller(double ax, double ay, double aK, double aI, double aD, double aminpv, double amaxpv, 
            //  double aminop, double amaxop)
            //  : base(ax, ay)
        }

        private void addnewnmpccontroller()
        {
            sim.nmpccontrollers.Add(new nmpc(sim.unitops.Count, defaultlocation.x, defaultlocation.y, sim));

            //public nmpc(int anr, double ax, double ay)
        }

        private void addnewcontrolmvsignalsplitter()
        {
            sim.blocks.Add(new controlmvsignalsplitter(sim.blocks.Count, defaultlocation.x, defaultlocation.y));
        }

        private void toolStripButton1_Click(object sender, EventArgs e) // Tank button
        {
            if (toolStripButton1.Checked)
            {
                //if (toolStripButton2.Checked) {toolStripButton2.Checked = false;}

                if (defaultlocation == null)
                { defaultlocation = new point(global.DefaultLocationX, global.DefaultLocationY); }
                addnewtank();

            }
            if (!toolStripButton1.Checked)
            {
                sim.unitops.RemoveAt(sim.unitops.Count - 1);
                sim.drawnetwork(panelGraphics);
                Invalidate();
            }
        }

        private void toolStripButton2_Click(object sender, EventArgs e) //Pump button
        {
            if (toolStripButton2.Checked)
            {
                //if (toolStripButton2.Checked) {toolStripButton2.Checked = false;}

                if (defaultlocation == null)
                { defaultlocation = new point(global.DefaultLocationX, global.DefaultLocationY); }
                addnewpump();

            }
            if (!toolStripButton2.Checked)
            {
                sim.unitops.RemoveAt(sim.unitops.Count - 1);
                sim.drawnetwork(panelGraphics);
                Invalidate();
            }
        }

        private void toolStripButton26_Click(object sender, EventArgs e) //Flange button
        {
            if (toolStripButton26.Checked)
            {
                //if (toolStripButton2.Checked) {toolStripButton2.Checked = false;}

                if (defaultlocation == null) { defaultlocation = new point(global.DefaultLocationX, global.DefaultLocationY); }
                addnewflange();

            }
            if (!toolStripButton26.Checked)
            {
                sim.unitops.RemoveAt(sim.unitops.Count - 1);
                sim.drawnetwork(panelGraphics);
                Invalidate();
            }
        }

        private void toolStripButton8_Click(object sender, EventArgs e) //Valve button
        {
            if (toolStripButton8.Checked)
            {
                //if (toolStripButton2.Checked) {toolStripButton2.Checked = false;}

                if (defaultlocation == null) { defaultlocation = new point(global.DefaultLocationX, global.DefaultLocationY); }
                addnewvalve();

            }
            if (!toolStripButton8.Checked)
            {
                sim.unitops.RemoveAt(sim.unitops.Count - 1);
                sim.drawnetwork(panelGraphics);
                Invalidate();
            }
        }

        private void toolStripButton13_Click(object sender, EventArgs e) //Tee button
        {
            if (toolStripButton13.Checked)
            {
                if (defaultlocation == null) { defaultlocation = new point(global.DefaultLocationX, global.DefaultLocationY); }
                addnewtee();
            }
            if (!toolStripButton13.Checked)
            {
                sim.unitops.RemoveAt(sim.unitops.Count - 1);
                sim.drawnetwork(panelGraphics);
                Invalidate();
            }
        }

        private void toolStripButton14_Click(object sender, EventArgs e) //Mixer button
        {
            if (toolStripButton14.Checked)
            {
                if (defaultlocation == null) { defaultlocation = new point(global.DefaultLocationX, global.DefaultLocationY); }
                addnewmixer();
            }
            if (!toolStripButton14.Checked)
            {
                sim.unitops.RemoveAt(sim.unitops.Count - 1);
                sim.drawnetwork(panelGraphics);
                Invalidate();
            }
        }

        private void toolStripButton15_Click(object sender, EventArgs e) //PID controller button
        {
            if (toolStripButton15.Checked)
            {
                if (defaultlocation == null) { defaultlocation = new point(global.DefaultLocationX, global.DefaultLocationY); }
                addnewpidcontroller();

            }
            if (!toolStripButton15.Checked)
            {
                sim.pidcontrollers.RemoveAt(sim.pidcontrollers.Count - 1);
                sim.drawnetwork(panelGraphics);
                Invalidate();
            }
        }

        private void toolStripButton27_Click(object sender, EventArgs e) //NMPC button event handler.
        {
            if (toolStripButton27.Checked)
            {
                if (defaultlocation == null) { defaultlocation = new point(global.DefaultLocationX, global.DefaultLocationY); }
                addnewnmpccontroller();

            }
            if (!toolStripButton27.Checked)
            {
                sim.nmpccontrollers.RemoveAt(sim.nmpccontrollers.Count - 1);
                sim.drawnetwork(panelGraphics);
                Invalidate();
            }
        }
        
       

        private void toolStripButton4_Click(object sender, EventArgs e) //Simulate button
        {
            if (sim.simi >= global.SimIterations)
            {
                doreset();
            }
            timer1.Enabled = true;
            sim.setsimulating(true);
            sim.simulate(panelGraphics, detailtrends);

            toolStripLabel1.Text = sim.simtime.timerstring();
            Invalidate();
            //sim.simulating = false;
        }


        
        
        private void handlestopevent()
        {
            timer1.Enabled = false;
            sim.setsimulating(false);
        }

        private void doreset()
        {
            handlestopevent();
            sim.setsimulationready();
        }

        private void toolStripButton5_Click(object sender, EventArgs e) //Stop simulation button
        {
            handlestopevent();
        }

        private void toolStripButton6_Click(object sender, EventArgs e)  //Save button
        {
            Stream myStream;
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "csm files (*.csm)|*.csm";
            saveFileDialog1.FilterIndex = 2;
            saveFileDialog1.RestoreDirectory = true;
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                if ((myStream = saveFileDialog1.OpenFile()) != null)
                {

                    // Code to write the stream goes here.
                    BinaryFormatter bf = new BinaryFormatter();
                    bf.Serialize(myStream, sim);
                    myStream.Close();
                }
            }


            /*FileStream ^fstream = File::Create("data.dat");
            BinaryFormatter ^bf = gcnew BinaryFormatter();
            bf->Serialize(fstream, sim);
            fstream->Close();*/
        }

        private void toolStripButton7_Click(object sender, EventArgs e)  //Open button
        {
            Stream myStream;
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            openFileDialog1.InitialDirectory = "c:\\";
            openFileDialog1.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            openFileDialog1.FilterIndex = 2;
            openFileDialog1.RestoreDirectory = true;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                if ((myStream = openFileDialog1.OpenFile()) != null)
                {
                    // Insert code to read the stream here.
                    if (sim != null) { sim.Dispose(); }
                    BinaryFormatter bf = new BinaryFormatter();
                    sim = (simulation)bf.Deserialize(myStream);
                    myStream.Close();
                }
            }
        }

        private void toolStripButton11_Click(object sender, EventArgs e)
            //Click event: Insert an FT reactor.
        {
            if (toolStripButton11.Checked)
            {
                if (defaultlocation == null) { defaultlocation = new point(global.DefaultLocationX, global.DefaultLocationY); }
                addnewftreactor();
            }
            if (!toolStripButton11.Checked)
            {
                sim.unitops.RemoveAt(sim.unitops.Count - 1);
                sim.drawnetwork(panelGraphics);
                Invalidate();
            }
        }

        private void toolStripButton16_Click(object sender, EventArgs e)   
            //Click event: Insert a distillation column.
        {
            if (toolStripButton16.Checked)
            {
                if (defaultlocation == null) { defaultlocation = new point(global.DefaultLocationX, global.DefaultLocationY); }
                addnewdistillationcolumn();
            }
            if (!toolStripButton16.Checked)
            {
                sim.unitops.RemoveAt(sim.unitops.Count - 1);
                sim.drawnetwork(panelGraphics);
                Invalidate();
            }
        }

        private void toolStripButton20_Click(object sender, EventArgs e)
            //Click event: Insert a steam generator
        {
            if (toolStripButton20.Checked)
            {
                if (defaultlocation == null) { defaultlocation = new point(global.DefaultLocationX, global.DefaultLocationY); }
                addnewsteamgenerator();
            }
            if (!toolStripButton20.Checked)
            {
                sim.unitops.RemoveAt(sim.unitops.Count - 1);
                sim.drawnetwork(panelGraphics);
                Invalidate();
            }
        }

        private void toolStripButton21_Click(object sender, EventArgs e)
        //Event: Insert a heat exchanger
        {
            if (toolStripButton21.Checked)
            {
                if (defaultlocation == null) { defaultlocation = new point(global.DefaultLocationX, global.DefaultLocationY); }
                addnewheatexchanger();
            }
            if (!toolStripButton21.Checked)
            {
                sim.unitops.RemoveAt(sim.unitops.Count - 1);
                sim.drawnetwork(panelGraphics);
                Invalidate();
            }
        }

        private void toolStripButton22_Click(object sender, EventArgs e)
        //Event: Insert a heatexchangersimple
        {
            if (toolStripButton22.Checked)
            {
                if (defaultlocation == null) { defaultlocation = new point(global.DefaultLocationX, global.DefaultLocationY); }
                addnewheatexchangersimple();
            }
            if (!toolStripButton22.Checked)
            {
                sim.unitops.RemoveAt(sim.unitops.Count - 1);
                sim.drawnetwork(panelGraphics);
                Invalidate();
            }
        }

        private void toolStripButton29_Click(object sender, EventArgs e) //Click event for Cooling Tower button
        {
            if (toolStripButton29.Checked)
            {
                if (defaultlocation == null) { defaultlocation = new point(global.DefaultLocationX, global.DefaultLocationY); }
                addnewcoolingtower();
            }
            if (!toolStripButton29.Checked)
            {
                sim.unitops.RemoveAt(sim.unitops.Count - 1);
                sim.drawnetwork(panelGraphics);
                Invalidate();
            }
        }

        private void toolStripButton28_Click(object sender, EventArgs e) //Click event for CTHX button
        {
            if (toolStripButton28.Checked)
            {
                if (defaultlocation == null) { defaultlocation = new point(global.DefaultLocationX, global.DefaultLocationY); }
                addnewcoolingtowerheatexchangersimple();
            }
            if (!toolStripButton28.Checked)
            {
                sim.unitops.RemoveAt(sim.unitops.Count - 1);
                sim.drawnetwork(panelGraphics);
                Invalidate();
            }
        }
        
        private void toolStripButton23_Click(object sender, EventArgs e) 
            //Cooling Tower (CT) Simple button click event.
        {
            if (toolStripButton23.Checked)
            {
                if (defaultlocation == null) { defaultlocation = new point(global.DefaultLocationX, global.DefaultLocationY); }
                addnewcoolingtowersimple();
            }
            if (!toolStripButton23.Checked)
            {
                sim.unitops.RemoveAt(sim.unitops.Count - 1);
                sim.drawnetwork(panelGraphics);
                Invalidate();
            }
        }



        private void toolStripButton12_Click(object sender, EventArgs e)
            //Click event for global options button
        {
            globaloptionsform optionsform = new globaloptionsform(sim);
            optionsform.Show();
        }

        private void toolStripButton17_Click(object sender, EventArgs e)
            //Click event for Matrix button - which will be used to test matrix class 
            //and methods that are being developed.
        {
            matrix mat = new matrix();
            mat.m = new List<List<double>>(4);
            mat.m[0] = new List<double> { 5, 1, 6, -3 };
            mat.m[0] = new List<double> { -2, 6, -5, -2 };
            mat.m[0] = new List<double> { 4, 2, -1, 3 };
            mat.m[0] = new List<double> { 3, -4, 2, 5 };

            //    new double[,] { { 5,1,6,-3  }, { -2,6,-5,-2  }, { 4,2,-1,3 }, { 3, -4, 2, 5 } }; //This one worked

            //mat.m = new double[,] {{1, 2, 4}, {3, 8, 14}, {2,6,13}}; //This one worked
            //mat.m = new double[,] {{2, -1, 3}, {4,2,1}, {-6,-1,2}};  this one worked
            //mat.m = new double[,] { { 1, 4, 2, 3 }, { 1, 2, 1, 0 }, { 2, 6, 3, 1 }, { 0, 0, 1, 4 } };  I could not get the right answer to this one yet.
            matrix l = new matrix(4, 4);
            matrix u = new matrix(4, 4);
            mat.ludecomposition(l, u);
        }

        private void toolStripButton18_Click(object sender, EventArgs e)
            //PlotZ form launch event
        {
            plotz pz = new plotz();
            pz.Show();
        }

        private void toolStripButton19_Click(object sender, EventArgs e)
            //UV Flash will launch from this event for testing.
        {
            material mat = new material(global.baseprocessclassInitVolume);
            mat.uvflash();
        }

        private void toolStripButton32_Click(object sender, EventArgs e) //Click event for Signal button
        {

        }

        private void toolStripButton33_Click(object sender, EventArgs e) //Click event for MVSplitter button
        {
            if (toolStripButton33.Checked)
            {
                if (defaultlocation == null) { defaultlocation = new point(global.DefaultLocationX, global.DefaultLocationY); }
                addnewcontrolmvsignalsplitter();
            }
            if (!toolStripButton33.Checked)
            {
                sim.blocks.RemoveAt(sim.blocks.Count - 1);
                sim.drawnetwork(panelGraphics);
                Invalidate();
            }
        }









        private void toolStripMenuItem1_Click(object sender, EventArgs e) //Event handler for Trend menu item.
        {
            for (int i = 0; i < sim.streams.Count; i++)
            {
                //int pointerx = e.X + (-panel1.AutoScrollPosition.X);
                //int pointery = e.Y + (-panel1.AutoScrollPosition.Y);
                if (sim.streams[i].highlighted)
                {
                    embeddedtrend thetrendvar = new embeddedtrend(    //This does not work right now - to be fixed later.  I think part of
                                                                      //the problem is the autoscrollposition point that needs to be added.
                        (sim.streams[i].points[0].x + sim.streams[i].points[1].x) / 2,
                        (sim.streams[i].points[0].y + sim.streams[i].points[1].y) / 2,
                        utilities.adddimension(global.simtimevector),
                        utilities.adddimension(utilities.vectorprocessor(sim.streams[i].massflow.simvector, utilities.fps2fph)),
                        "Mass flow (kg/h)", this);
                }
            }

            for (int i = 0; i < sim.unitops.Count; i++)
            {
                if (sim.unitops[i].highlighted)
                {
                    if (sim.unitops[i].objecttype == objecttypes.FTReactor)
                    {
                        //ftreactortrends fttrends = new ftreactortrends((ftreactor)sim.unitops[i], sim);
                        //fttrends.Show();
                    }



                }
            }
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e) //Event handler for Trend detail menu item.
        {
            for (int i = 0; i < sim.unitops.Count; i++)
            {
                if (sim.unitops[i].highlighted)
                {
                    sim.unitops[i].showtrenddetail(sim, detailtrends);
                }
            }

            for (int i = 0; i < sim.streams.Count; i++)
            {
                if (sim.streams[i].highlighted)
                {
                    sim.streams[i].showtrenddetail(sim, detailtrends);
                }
            }

            for (int i = 0; i < sim.pidcontrollers.Count; i++)
            {
                if (sim.pidcontrollers[i].highlighted)
                {
                    sim.pidcontrollers[i].showtrenddetail(sim, detailtrends);
                }
            }

            for (int i = 0; i < sim.nmpccontrollers.Count; i++)
            {
                if (sim.nmpccontrollers[i].highlighted)
                {
                    sim.nmpccontrollers[i].showtrenddetail(sim, detailtrends);
                }
            }
        }

        private void toolStripMenuItem3_Click(object sender, EventArgs e) //Event handler for the Properties menu item.
        {
            for (int i = 0; i < sim.unitops.Count; i++)
            {
                if (sim.unitops[i].highlighted)
                {
                    sim.unitops[i].setproperties(sim);
                }
            }

            for (int i = 0; i < sim.streams.Count; i++)
            {
                if (sim.streams[i].highlighted)
                {
                    sim.streams[i].setproperties(sim);
                }
            }

            for (int i = 0; i < sim.pidcontrollers.Count; i++)
            {
                if (sim.pidcontrollers[i].highlighted)
                {
                    sim.pidcontrollers[i].setproperties(sim);
                }
            }

            for (int i = 0; i < sim.blocks.Count; i++)
            {
                if (sim.blocks[i].highlighted)
                {
                    sim.blocks[i].setproperties(sim);
                }
            }

            for (int i = 0; i < sim.nmpccontrollers.Count; i++)
            {
                if (sim.nmpccontrollers[i].highlighted)
                {
                    sim.nmpccontrollers[i].setproperties(sim);
                }
            }
        }

        private void toolStripMenuItem4_Click(object sender, EventArgs e) //Event handler for the Delete menu item.
        {
            int idelete = -1;
            for (int i = 0; i < sim.unitops.Count; i++)
            {
                if (sim.unitops[i].highlighted)
                {
                    idelete = i;
                }
            }
            if (idelete >= 0) { sim.unitops.RemoveAt(idelete); }

            idelete = -1;
            for (int i = 0; i < sim.streams.Count; i++)
            {
                if (sim.streams[i].highlighted)
                {
                    idelete = i;
                }
            }
            if (idelete >= 0) { sim.streams.RemoveAt(idelete); }

            idelete = -1;
            for (int i = 0; i < sim.signals.Count; i++)
            {
                if (sim.signals[i].highlighted)
                {
                    idelete = i;
                }
            }
            if (idelete >= 0) { sim.signals.RemoveAt(idelete); }

            idelete = -1;
            for (int i = 0; i < sim.pidcontrollers.Count; i++)
            {
                if (sim.pidcontrollers[i].highlighted)
                {
                    idelete = i;
                }
            }
            if (idelete >= 0) { sim.pidcontrollers.RemoveAt(idelete); }

            idelete = -1;
            for (int i = 0; i < sim.blocks.Count; i++)
            {
                if (sim.blocks[i].highlighted)
                {
                    idelete = i;
                }
            }
            if (idelete >= 0) { sim.blocks.RemoveAt(idelete); }

            idelete = -1;
            for (int i = 0; i < sim.nmpccontrollers.Count; i++)
            {
                if (sim.nmpccontrollers[i].highlighted)
                {
                    idelete = i;
                }
            }
            if (idelete >= 0) { sim.nmpccontrollers.RemoveAt(idelete); }
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {

        }

        private void toolStripButton24_Click(object sender, EventArgs e) //Plus button for zoom in
        {
            global.GScale++;
   
        }

        private void toolStripButton25_Click(object sender, EventArgs e) //Minus button for zoom out
        {
            global.GScale--;
       
        }

        private void toolStripButton34_Click(object sender, EventArgs e) //Click event to reset the simulation
        {
            doreset();
        }

        



        

        



        

        

        



        

        

        

        

        








        








    }
}
