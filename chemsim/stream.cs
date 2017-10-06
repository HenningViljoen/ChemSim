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
    public class stream : baseprocessclass
    {
        public double direction; //radians
        public double distance; //m
        public List<point> inbetweenpoints; //points that are between the main in and out points, that will enable straight lines for the stream.
        public point[] displaypoints; //The locations of the T, P and flow properties that are being displayed for the stream.

        
        
        public stream(int anr, double p0x, double p0y, double p1x, double p1y)
            : base(anr, (p0x + p1x) / 2, (p0y + p1y) / 2)
        {
            streaminit(p0x, p0y, p1x, p1y);
        }

        public stream(stream streamcopyfrom)
            : base(streamcopyfrom.nr, (streamcopyfrom.points[0].x + streamcopyfrom.points[1].x) / 2,
                (streamcopyfrom.points[0].y + streamcopyfrom.points[1].y) / 2)
        {
            streaminit(streamcopyfrom.points[0].x, streamcopyfrom.points[0].y, streamcopyfrom.points[1].x, 
                streamcopyfrom.points[1].y);
            copyfrom(streamcopyfrom);
        }

        private void streaminit(double p0x, double p0y, double p1x, double p1y)
        {
            objecttype = objecttypes.Stream;
            name = nr.ToString() + " " + objecttype.ToString();

            points = new point[2];
            points[0] = new point(p0x, p0y);
            points[1] = new point(p1x, p1y);

            updatedirection();
            inbetweenpoints = new List<point>(0);
            displaypoints = new point[global.StreamNrPropDisplay];
            for (int i = 0; i < global.StreamNrPropDisplay; i++)
            {
                displaypoints[i] = new point(0, 0);
            }


            update(0, false);
        }

        public override void copyfrom(baseclass baseclasscopyfrom)
        {
            stream streamcopyfrom = (stream)baseclasscopyfrom;

            base.copyfrom(streamcopyfrom);

            direction = streamcopyfrom.direction; //radians
            distance = streamcopyfrom.distance; //m
        }

        public void updatedirection()
        {
            direction = utilities.calcdirection(points[1].y - points[0].y, points[1].x - points[0].x);
            distance = utilities.distance(points[0], points[1]);
        }

        public void updatemassflowsource(string afilename)
        {

        }

        public override void updatepoint(int i, double x, double y)
        {
            points[i].x = x;
            points[i].y = y;
            updatedirection();
        }

        public override void update(int simi, bool historise)
        {
            if (simi > 0)
            {
                // the reference flow is the mass flow
                calcactualvolumeflowfrommassflow();
                calcmolarflowfrommassflow();
                calcstandardflowfrommoleflow(); //should run after calcdndt since molar flow to be calculated first.
                if (massflow.datasource == datasourceforvar.Exceldata) 
                {
                    int i = (simi >= massflow.excelsource.data.Length) ? i = massflow.excelsource.data.Length - 1 : simi;
                    massflow.v = massflow.excelsource.data[i];
                }

                if (mat.T.datasource == datasourceforvar.Exceldata) //This part should actually move to material.update().
                {
                    int i = (simi >= mat.T.excelsource.data.Length) ? i = mat.T.excelsource.data.Length - 1 : simi;
                    mat.T.v = mat.T.excelsource.data[i];
                }
                if (mat.relativehumidity.datasource == datasourceforvar.Exceldata) //This part should actually move to material.update().
                {
                    int i = (simi >= mat.relativehumidity.excelsource.data.Length) ? i = mat.relativehumidity.excelsource.data.Length - 1 : simi;
                    mat.relativehumidity.v = mat.relativehumidity.excelsource.data[i];
                }
            }

            if (historise && (simi % global.SimVectorUpdatePeriod == 0))
            {

                if (actualvolumeflow.simvector != null) { actualvolumeflow.simvector[simi/global.SimVectorUpdatePeriod] = actualvolumeflow.v; }


                if (standardvolumeflow.simvector != null) { standardvolumeflow.simvector[simi / global.SimVectorUpdatePeriod] = 
                        standardvolumeflow.v; }


                if (massflow.simvector != null) { massflow.simvector[simi / global.SimVectorUpdatePeriod] = massflow.v; }


                if (molarflow.simvector != null) { molarflow.simvector[simi / global.SimVectorUpdatePeriod] = molarflow.v; }
                
                //pressuresimvector[simi] = mat.P.v;
            }

            base.update(simi, historise);
        }

        public override void showtrenddetail(simulation asim, List<Form> detailtrendslist)
        {
            detailtrendslist.Add(new streamdetail(this, asim));
            detailtrendslist[detailtrendslist.Count - 1].Show();
        }

        public override bool mouseover(double x, double y)
        {
            bool streamover = false; //default
            double pixelstoclosestpoint = 999999999;
            double pixelstopoint;

            for (int i = 0; i < 2; i++)
            {
                pixelstopoint = utilities.distance(x - points[i].x, y - points[i].y)*global.GScale;
                if (pixelstopoint < pixelstoclosestpoint) { pixelstoclosestpoint = pixelstopoint; }
            }
            for (int i = 0; i < inbetweenpoints.Count; i++)
            {
                pixelstopoint = utilities.distance(x - inbetweenpoints[i].x, y - inbetweenpoints[i].y) * global.GScale;
                if (pixelstopoint < pixelstoclosestpoint) { pixelstoclosestpoint = pixelstopoint; }
            }
            
            
            
            if (pixelstoclosestpoint <= global.MinDistanceFromStream)
            {
                    streamover = true;
             }
                else
                {
                    streamover = false;
                }


            return streamover;
        }

        public override void setproperties(simulation asim)
        {
            streamproperties streamprop = new streamproperties(this, asim);
            streamprop.Show();
        }

        private void drawsection(GraphicsPath plot, point p0, point p1, bool addarrow) //This method will draw one segment of the stream
                                                                                       //between two of its end points, or inbetween points.  
        {
            point pinterim = new point(0, 0);
            double dx = Math.Abs(p1.x - p0.x);
            double dy = Math.Abs(p1.y - p0.y);

            if (dx > dy)
            {
                pinterim.x = p0.x;
                pinterim.y = p1.y;
            }
            else
            {
                pinterim.x = p1.x;
                pinterim.y = p0.y;
            }

            plot.AddLine(global.OriginX + Convert.ToInt32(global.GScale * p0.x),
                          global.OriginY + Convert.ToInt32(global.GScale * p0.y),
                          global.OriginX + Convert.ToInt32(global.GScale * pinterim.x),
                          global.OriginY + Convert.ToInt32(global.GScale * pinterim.y));
            plot.AddLine(global.OriginX + Convert.ToInt32(global.GScale * pinterim.x),
                          global.OriginY + Convert.ToInt32(global.GScale * pinterim.y),
                          global.OriginX + Convert.ToInt32(global.GScale * p1.x),
                          global.OriginY + Convert.ToInt32(global.GScale * p1.y));
            if (addarrow)
            {
                double lastdirection = utilities.calcdirection(p1.y - p0.y, p1.x - p0.x);
                plot.AddLine(global.OriginX + Convert.ToInt32(global.GScale * p1.x),
                          global.OriginY + Convert.ToInt32(global.GScale * p1.y),
                          global.OriginX + Convert.ToInt32(global.GScale * (p1.x +
                                global.StreamArrowLength * Math.Cos(global.StreamArrowAngle + Math.PI + lastdirection))),
                          global.OriginY + Convert.ToInt32(global.GScale * (p1.y +
                                global.StreamArrowLength * Math.Sin(global.StreamArrowAngle + Math.PI + lastdirection))));
                plot.AddLine(global.OriginX + Convert.ToInt32(global.GScale * p1.x),
                          global.OriginY + Convert.ToInt32(global.GScale * p1.y),
                          global.OriginX + Convert.ToInt32(global.GScale * (p1.x +
                                global.StreamArrowLength * Math.Cos(-global.StreamArrowAngle + Math.PI + lastdirection))),
                          global.OriginY + Convert.ToInt32(global.GScale * (p1.y +
                                global.StreamArrowLength * Math.Sin(-global.StreamArrowAngle + Math.PI + lastdirection))));
            }
        }

        private void calcdisplaypoints() //Calculate the location on the screen for the properties of the stream that is going to be displayed 
                                         //on the stream
        {
            point longestp0 = points[0]; //Need to have a value accordingto compiler.
            point longestp1 = points[1];
            point tempp0;
            point tempp1;
            double distancebetweenpoints;
            double furthestdistancebetweenpoints = 0;

            if (inbetweenpoints.Count == 0)
            {
                longestp0 = points[0];
                longestp1 = points[1];
            }
            else
            {

                tempp0 = points[0];
                tempp1 = inbetweenpoints[0];
                distancebetweenpoints = utilities.distance(tempp0, tempp1);
                if (distancebetweenpoints > furthestdistancebetweenpoints) 
                { 
                    furthestdistancebetweenpoints = distancebetweenpoints;
                    longestp0 = points[0];
                    longestp1 = inbetweenpoints[0];
                }

                for (int i = 1; i < inbetweenpoints.Count; i++)
                {
                    tempp0 = inbetweenpoints[i - 1];
                    tempp1 = inbetweenpoints[i];
                    distancebetweenpoints = utilities.distance(tempp0, tempp1);
                    if (distancebetweenpoints > furthestdistancebetweenpoints) 
                    { 
                        furthestdistancebetweenpoints = distancebetweenpoints;
                        longestp0 = inbetweenpoints[i - 1];
                        longestp1 = inbetweenpoints[i];
                    }
                }

                tempp0 = inbetweenpoints[inbetweenpoints.Count - 1];
                tempp1 = points[1];
                distancebetweenpoints = utilities.distance(tempp0, tempp1);
                if (distancebetweenpoints > furthestdistancebetweenpoints)
                {
                    furthestdistancebetweenpoints = distancebetweenpoints;
                    longestp0 = inbetweenpoints[inbetweenpoints.Count - 1];
                    longestp1 = points[1];
                }

            }

            if (Math.Abs(longestp1.x - longestp0.x) > Math.Abs(longestp1.y - longestp0.y))
            {
                double deltax = longestp1.x - longestp0.x;
                for (int i = 0; i < global.StreamNrPropDisplay; i++)
                {
                    displaypoints[i].x = longestp0.x + (i + 1)*deltax / (global.StreamNrPropDisplay + 1);
                    displaypoints[i].y = longestp1.y;
                }
            }
            else
            {
                double deltay = longestp1.y - longestp0.y;
                for (int i = 0; i < global.StreamNrPropDisplay; i++)
                {
                    displaypoints[i].x = longestp1.x;
                    displaypoints[i].y = longestp0.y + (i + 1) * deltay / (global.StreamNrPropDisplay + 1);
                    
                }

            }


        }

        public override void draw(Graphics G)
        {
            GraphicsPath plot1;
            Pen plotpen;
            float width = 1;

            plot1 = new GraphicsPath();
            if (inbetweenpoints.Count > 0)
            {
                drawsection(plot1, points[0], inbetweenpoints[0], false);
                for (int i = 1; i < inbetweenpoints.Count; i++)
                {
                    drawsection(plot1, inbetweenpoints[i - 1], inbetweenpoints[i], false);
                }
                drawsection(plot1, inbetweenpoints[inbetweenpoints.Count - 1 ], points[1], true);
            }
            else
            {
                drawsection(plot1, points[0], points[1], true);
            }

            plotpen = new Pen(Color.Black, width);
            if (highlighted)
            {
                plotpen.Color = Color.Red;
            }
            G.DrawPath(plotpen, plot1);

            //The writing of the massflow, pressure and temperature of the stream on the PFD.
            calcdisplaypoints();

            GraphicsPath massflowpath = new GraphicsPath();
            StringFormat format = StringFormat.GenericDefault;
            FontFamily family = new FontFamily("Arial");
            int myfontStyle = (int)FontStyle.Bold;
            int emSize = 8;
            String massflowstring = utilities.fps2fph(massflow.v).ToString("G5");
            PointF massflowpoint = new PointF(global.OriginX +
                Convert.ToInt32(global.GScale * displaypoints[0].x - massflowstring.Length * emSize / 2 / 2),
                global.OriginY + Convert.ToInt32(global.GScale*(displaypoints[0].y)));
            massflowpath.AddString(massflowstring, family, myfontStyle, emSize, massflowpoint, format);
            G.FillPath(Brushes.Blue, massflowpath);

            GraphicsPath pressurepath = new GraphicsPath();
            String pressurestring = utilities.pascal2barg(mat.P.v).ToString("G5");
            PointF pressurepoint = new PointF(global.OriginX +
                Convert.ToInt32(global.GScale * displaypoints[1].x - pressurestring.Length * emSize / 2 / 2),
                global.OriginY + Convert.ToInt32(global.GScale * displaypoints[1].y));
            pressurepath.AddString(pressurestring, family, myfontStyle, emSize, pressurepoint, format);
            G.FillPath(Brushes.Green, pressurepath);

            GraphicsPath temperaturepath = new GraphicsPath();
            String temperaturestring = utilities.kelvin2celcius(mat.T.v).ToString("G5");
            PointF temperaturepoint = new PointF(global.OriginX +
                Convert.ToInt32(global.GScale * displaypoints[2].x - temperaturestring.Length * emSize / 2 / 2),
                global.OriginY + Convert.ToInt32(global.GScale * displaypoints[2].y));
            temperaturepath.AddString(temperaturestring, family, myfontStyle, emSize, temperaturepoint, format);
            G.FillPath(Brushes.Red, temperaturepath);

        }

    }
}
