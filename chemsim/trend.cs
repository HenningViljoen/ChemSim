using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace chemsim
{
    [Serializable]
    public class trend
    {
        public static Color[] tcolour = new Color[] { Color.Blue, Color.Red, Color.Green, Color.Brown };

        private Matrix World;
        private SmoothingMode SmoothingMode;
        private PictureBox pbox;
        private Graphics G;
        private double Xmin, Xmax;
        private double Ymin, Ymax;
        private int XPixels;
        //private float xfactor;
        //private float yfactor;
        private double[][] xhistory;
        private double[][] yhistory;
        int ndatasets;
        String tagname;
        bool millionsx;
        bool millionsy;
        bool showlabels;

        public trend(PictureBox apicturebox)
        {
            World = new Matrix();
            SmoothingMode = SmoothingMode.Default;
            pbox = apicturebox;
            //pbox.SetStyle(ControlStyles.Opaque, true);
            G = getgraphicsobject(pbox);
            G.Clear(pbox.BackColor);
        }

        private Graphics getgraphicsobject(PictureBox pbox) //was passed by ref, should not be passed by val?
        {
            Bitmap bmp;
            bmp = new Bitmap(pbox.Width, pbox.Height);
            pbox.Image = bmp;
            Graphics g;
            g = Graphics.FromImage(bmp);
            return g;
        }

        private float transformy(double ay)
        {
            return (float)(pbox.Image.Size.Height - global.YAxisMargin -
                (ay - Ymin) / (Ymax - Ymin) * (pbox.Image.Size.Height - global.YAxisMargin * 2));
        }

        private float transformx(double ax)
        {
            return (float)(global.XAxisMargin +
                (ax - Xmin) / (Xmax - Xmin) * (pbox.Image.Size.Width - global.XAxisMargin * 2));
        }




        public void plot(double[][] xdata, double[][] ydata,
                        String tagname, bool millionsx,
                        bool millionsy, bool showlabels, double aymin = 0.0, double aymax = 0.0)
        {
            //Dim pastpv(), pastsp(), pastop() As Double
            int ndatasets = ydata.Length; //this must be the same amount of datasets as for the xdata - no check done
            //ndatacolumns = ydata->GetLength(1);
            //ReDim pastpv(ndatarows), pastsp(ndatarows), pastop(ndatarows)
            int i, j;

            //'Dim pastpvlength As Integer = UBound(pastpv)

            //'Dim imin, imax As Integer

            Ymin = float.MaxValue;
            Ymax = -float.MaxValue;

            Xmin = (float)utilities.matrixmin(xdata);
            Xmax = (float)utilities.matrixmax(xdata);

            //'Dim val As Single = pastpv(0) 'Just initialisation for debugging
            //'Dim valsp As Single = pastpv(0)
            XPixels = pbox.Width - 1;

            Ymin = (aymin == 0) ? utilities.matrixmin(ydata) : aymin;
            Ymax = (aymax == 0) ? utilities.matrixmax(ydata) : aymax;

            if (Ymin == Ymax) { Ymax = Ymin + 0.1f; }

            //xfactor = (pbox.Width * 0.9f) / (Xmax - Xmin);//(apicturebox->Width - 84.0) / (Xmax - Xmin);//  //
            //yfactor = -(pbox.Height * 0.8f) / (Ymax - Ymin); //-(apicturebox->Height - 44.0) / (Ymax - Ymin); //

            //World.Scale(xfactor, yfactor);
            ////World->Translate(-(Xmin - 40 / xfactor), -(Ymax - 22 / yfactor));
            //if (showlabels) { World.Translate(-(Xmin - 40.0f / xfactor), -(Ymax - 22.0f / yfactor)); }
            //else { World.Translate(-(Xmin - 10 / xfactor), -(Ymax - 0 / yfactor)); }

            GraphicsPath Xaxis = new GraphicsPath();
            GraphicsPath Yaxis = new GraphicsPath();
            Xaxis.AddLine(new PointF(transformx(Xmin), transformy(Ymin)),
                new PointF(transformx(Xmax), transformy(Ymin)));
            Yaxis.AddLine(new PointF(transformx(Xmin), transformy(Ymax)),
                new PointF(transformx(Xmin), transformy(Ymin)));



            float oldX, oldY;
            float X, Y;

            //' Each segment in the path goes from (oldX, oldY) to (X, y)
            //' At each iteration (X, Y) becomes (oldX, oldY) for the next point
            //' the following two paths correspond to the functions to be plotted

            GraphicsPath[] plot;
            plot = new GraphicsPath[ndatasets];
            for (i = 0; i < ndatasets; i++)
            {
                plot[i] = new GraphicsPath();
            }

            Pen plotPen = new Pen(Color.BlueViolet, 1);
            //' Calculate the min and max points of the plot

            for (j = 0; j < ndatasets; j++)
            {
                oldX = transformx(xdata[j][0]);
                if (double.IsNaN(ydata[j][0]))
                {
                    oldY = transformy(0);
                }
                else
                {
                    oldY = transformy(ydata[j][0]);
                }

                for (i = 0; i < ydata[j].Length; i++)
                {
                    X = transformx(xdata[j][i]);
                    if (double.IsNaN(ydata[j][i]))
                    {
                        Y = transformy(0);
                    }
                    else
                    {
                        Y = transformy(ydata[j][i]);
                    }

                    plot[j].AddLine(oldX, oldY, X, Y);

                    oldX = X;
                    oldY = Y;
                }
            }

            //' create the plot1 and plot2 paths
            pbox.BackColor = Color.White;
            G.Clear(pbox.BackColor);


            G.SmoothingMode = SmoothingMode;
            //' and finally draw everything
            //Xaxis.Transform(World);
            plotPen.Color = Color.Gray;
            G.DrawPath(plotPen, Xaxis);      //' The X axis
            //Yaxis.Transform(World);
            plotPen.Color = Color.Gray;
            G.DrawPath(plotPen, Yaxis);     //' The Y axis

            //GraphicsPath testline = new GraphicsPath();
            //testline.AddLine(0, 100, 270, 1);
            //G.DrawPath(plotPen, testline);

            for (j = 0; j < ndatasets; j++)
            {
                plotPen.Color = tcolour[j % tcolour.Length];
                //plot[j].Transform(World);      //' pastpv
                G.DrawPath(plotPen, plot[j]);
            }

            if (showlabels)
            {
                GraphicsPath myPathpv = new GraphicsPath();
                GraphicsPath myPathtime = new GraphicsPath();
                GraphicsPath myPathheading = new GraphicsPath();

                StringFormat format = StringFormat.GenericDefault;
                FontFamily family = new FontFamily("Arial");
                int myfontStyle = (int)FontStyle.Bold;
                int emSize = 12;

                //' Set up all the string parameters.
                double num;
                if (millionsx)
                {
                    num = 1000000.0;
                }
                else
                {
                    num = 1.0;
                }

                int nXpoints = 11;
                String[] stringtextx;
                stringtextx = new String[nXpoints];
                PointF[] xpoints;
                GraphicsPath[] rulerx;
                rulerx = new GraphicsPath[nXpoints];
                xpoints = new PointF[nXpoints];
                for (i = 0; i < nXpoints; i++)
                {
                    stringtextx[i] = ((Xmin + (Xmax - Xmin) / (nXpoints - 1) * i) / num).ToString("N0");
                    xpoints[i] = new PointF(Xaxis.PathPoints[0].X + (Xaxis.PathPoints[1].X - Xaxis.PathPoints[0].X) / (nXpoints - 1) * i -
                                            stringtextx[i].Length / 2 * 10,
                                            Xaxis.PathPoints[0].Y + 5);
                    myPathtime.AddString(stringtextx[i], family, myfontStyle, emSize, xpoints[i], format);
                    rulerx[i] = new System.Drawing.Drawing2D.GraphicsPath();
                    rulerx[i].AddLine(Xaxis.PathPoints[0].X + (Xaxis.PathPoints[1].X - Xaxis.PathPoints[0].X) / (nXpoints - 1) * i,
                                    Xaxis.PathPoints[0].Y + 5,
                                    Xaxis.PathPoints[0].X + (Xaxis.PathPoints[1].X - Xaxis.PathPoints[0].X) / (nXpoints - 1) * i,
                                    Xaxis.PathPoints[0].Y);
                    plotPen.Color = Color.Gray;
                    G.DrawPath(plotPen, rulerx[i]);
                }


                //'stringtextx(0) = (Math.Round(Xmin / num)).ToString("N")
                //'stringtextx(1) = (Math.Round(Xmax / num)).ToString("N")

                //'xpoints(0) = New PointF(Xaxis.PathPoints(0).X, Xaxis.PathPoints(0).Y + 5)
                //'xpoints(1) = New PointF(Xaxis.PathPoints(1).X, Xaxis.PathPoints(1).Y + 5)
                //'myPathtime.AddString(stringtextx(0), family, myfontStyle, emSize, xpoints(0), format)
                //'myPathtime.AddString(stringtextx(1), family, myfontStyle, emSize, xpoints(1), format)

                if (millionsy)
                {
                    num = 1000000.0;
                }
                else
                {
                    num = 1.0;
                }

                int nYpoints = 11;
                String[] stringtexty = new String[nYpoints];
                PointF[] ypoints = new PointF[nYpoints];
                GraphicsPath[] rulery = new GraphicsPath[nYpoints];

                for (i = 0; i < nYpoints; i++)
                {
                    stringtexty[i] = (((Ymin + (Ymax - Ymin) / (nYpoints - 1) * i) / num)).ToString("N");
                    ypoints[i] = new PointF(Yaxis.PathPoints[1].X - 20,
                                    Yaxis.PathPoints[1].Y + (Yaxis.PathPoints[0].Y - Yaxis.PathPoints[1].Y) / (nYpoints - 1) * i - 5);
                    myPathpv.AddString(stringtexty[i], family, myfontStyle, emSize, ypoints[i], format);
                    rulery[i] = new GraphicsPath();
                    rulery[i].AddLine(Yaxis.PathPoints[1].X - 5,
                                    Yaxis.PathPoints[1].Y + (Yaxis.PathPoints[0].Y - Yaxis.PathPoints[1].Y) / (nYpoints - 1) * i,
                                    Yaxis.PathPoints[1].X,
                                    Yaxis.PathPoints[1].Y + (Yaxis.PathPoints[0].Y - Yaxis.PathPoints[1].Y) / (nYpoints - 1) * i);
                    plotPen.Color = Color.Gray;
                    G.DrawPath(plotPen, rulery[i]);
                }


                PointF pheading = new PointF(Xaxis.PathPoints[1].X / 2 - tagname.Length / 2 * 5, Yaxis.PathPoints[0].Y - 20);





                //' Add the string to the path.


                myPathheading.AddString(tagname, family, myfontStyle, emSize, pheading, format);


                G.FillPath(Brushes.Gray, myPathtime);
                G.FillPath(Brushes.Red, myPathpv);
                G.FillPath(Brushes.Gray, myPathheading);
                //'G.FillPath(Brushes.Blue, myPathop)
            }


        }

        public void setuptrend(String atagname, bool amillionsx,
                        bool amillionsy, bool ashowlabels, double axmin, double axmax,
                        double aymin = 1000000, double aymax = 1.0)
        {

            tagname = atagname;
            millionsx = amillionsx;
            millionsy = amillionsy;
            showlabels = ashowlabels;

            Xmin = axmin;
            Xmax = axmax;

            Ymin = aymin;
            Ymax = aymax;

            if (Ymin == Ymax) { Ymax = Ymin + 0.1; }

            GraphicsPath Xaxis = new GraphicsPath();
            GraphicsPath Yaxis = new GraphicsPath();
            Xaxis.AddLine(new PointF(transformx(Xmin), transformy(Ymin)),
                new PointF(transformx(Xmax), transformy(Ymin)));
            Yaxis.AddLine(new PointF(transformx(Xmin), transformy(Ymax)),
                new PointF(transformx(Xmin), transformy(Ymin)));



            Pen plotPen = new Pen(Color.BlueViolet, 1);


            //' create the plot1 and plot2 paths
            pbox.BackColor = Color.White;
            G.Clear(pbox.BackColor);


            G.SmoothingMode = SmoothingMode;
            //' and finally draw everything
            //Xaxis.Transform(World);
            plotPen.Color = Color.Gray;
            G.DrawPath(plotPen, Xaxis);      //' The X axis
            //Yaxis.Transform(World);
            plotPen.Color = Color.Gray;
            G.DrawPath(plotPen, Yaxis);     //' The Y axis

            if (showlabels)
            {
                GraphicsPath myPathpv = new GraphicsPath();
                GraphicsPath myPathtime = new GraphicsPath();
                GraphicsPath myPathheading = new GraphicsPath();

                StringFormat format = StringFormat.GenericDefault;
                FontFamily family = new FontFamily("Arial");
                int myfontStyle = (int)FontStyle.Bold;
                int emSize = 10;

                //' Set up all the string parameters.
                double num;
                if (millionsx)
                {
                    num = 1000.0;
                }
                else
                {
                    num = 1.0;
                }

                int nXpoints = 11;
                String[] stringtextx;
                stringtextx = new String[nXpoints];
                PointF[] xpoints;
                GraphicsPath[] rulerx;
                rulerx = new GraphicsPath[nXpoints];
                xpoints = new PointF[nXpoints];
                for (int i = 0; i < nXpoints; i++)
                {
                    stringtextx[i] = Math.Round(((Xmin + (Xmax - Xmin) / (nXpoints - 1) * i) / num), 0).ToString("G3");
                    xpoints[i] = new PointF(Xaxis.PathPoints[0].X + (Xaxis.PathPoints[1].X - Xaxis.PathPoints[0].X) / (nXpoints - 1) * i -
                                            stringtextx[i].Length / 2 * 10,
                                            Xaxis.PathPoints[0].Y + 5);
                    myPathtime.AddString(stringtextx[i], family, myfontStyle, emSize, xpoints[i], format);
                    rulerx[i] = new System.Drawing.Drawing2D.GraphicsPath();
                    rulerx[i].AddLine(Xaxis.PathPoints[0].X + (Xaxis.PathPoints[1].X - Xaxis.PathPoints[0].X) / (nXpoints - 1) * i,
                                    Xaxis.PathPoints[0].Y + 5,
                                    Xaxis.PathPoints[0].X + (Xaxis.PathPoints[1].X - Xaxis.PathPoints[0].X) / (nXpoints - 1) * i,
                                    Xaxis.PathPoints[0].Y);
                    plotPen.Color = Color.Gray;
                    G.DrawPath(plotPen, rulerx[i]);
                }


                if (millionsy)
                {
                    num = 1000000.0;
                }
                else
                {
                    num = 1.0;
                }

                int nYpoints = 11;
                String[] stringtexty = new String[nYpoints];
                PointF[] ypoints = new PointF[nYpoints];
                GraphicsPath[] rulery = new GraphicsPath[nYpoints];

                for (int i = 0; i < nYpoints; i++)
                {
                    stringtexty[i] = (((Ymin + (Ymax - Ymin) / (nYpoints - 1) * i) / num)).ToString("G3");
                    ypoints[i] = new PointF(Yaxis.PathPoints[1].X - 24,
                                    Yaxis.PathPoints[1].Y + (Yaxis.PathPoints[0].Y - Yaxis.PathPoints[1].Y) / (nYpoints - 1) * i - 5);
                    myPathpv.AddString(stringtexty[i], family, myfontStyle, emSize, ypoints[i], format);
                    rulery[i] = new GraphicsPath();
                    rulery[i].AddLine(Yaxis.PathPoints[1].X - 5,
                                    Yaxis.PathPoints[1].Y + (Yaxis.PathPoints[0].Y - Yaxis.PathPoints[1].Y) / (nYpoints - 1) * i,
                                    Yaxis.PathPoints[1].X,
                                    Yaxis.PathPoints[1].Y + (Yaxis.PathPoints[0].Y - Yaxis.PathPoints[1].Y) / (nYpoints - 1) * i);
                    plotPen.Color = Color.Gray;
                    G.DrawPath(plotPen, rulery[i]);
                }


                PointF pheading = new PointF(Xaxis.PathPoints[1].X / 2 - tagname.Length / 2 * 5, Yaxis.PathPoints[0].Y - 20);



                //' Add the string to the path.


                myPathheading.AddString(tagname, family, myfontStyle, emSize, pheading, format);


                G.FillPath(Brushes.Gray, myPathtime);
                G.FillPath(Brushes.Red, myPathpv);
                G.FillPath(Brushes.Gray, myPathheading);
                //'G.FillPath(Brushes.Blue, myPathop)
            }


        }

        public void drawdatasets(double[][] xdata, double[][] ydata, int i0 = 0, int i1 = -1)
        {
            float oldX, oldY;
            float X, Y;

            G.SmoothingMode = SmoothingMode;

            GraphicsPath[] plot;
            plot = new GraphicsPath[ndatasets];
            for (int i = 0; i < ndatasets; i++)
            {
                plot[i] = new GraphicsPath();
            }

            Pen plotPen = new Pen(Color.BlueViolet, 1);

            for (int j = 0; j < ndatasets; j++)
            {
                //oldX = (float)xdata[j][0] * pbox.Image.Size.Width / Xmax;
                oldX = transformx(xdata[j][i0]);
                if (double.IsNaN(ydata[j][i0]))
                {
                    oldY = transformy(i0);
                }
                else
                {
                    //oldY = (float)(pbox.Image.Size.Height 
                    //    - ydata[j][0] * pbox.Image.Size.Height / Ymax);
                    oldY = transformy(ydata[j][i0]);
                }
                int yl = (i1 == -1) ? ydata[j].Length : i1;
                for (int i = 1; i < yl; i++)
                {
                    //X = (float)xdata[j][i] * pbox.Image.Size.Width / Xmax;
                    X = transformx(xdata[j][i]);
                    if (double.IsNaN(ydata[j][i]))
                    {
                        Y = transformy(0);
                    }
                    else
                    {
                        //Y = (float)(pbox.Image.Size.Height - 
                        //    ydata[j][i] * pbox.Image.Size.Height / Ymax);
                        Y = transformy(ydata[j][i]);
                    }

                    plot[j].AddLine(oldX, oldY,
                        X, Y);

                    oldX = X;
                    oldY = Y;
                }
            }

            //' create the plot1 and plot2 paths
            pbox.BackColor = Color.White;

            G.SmoothingMode = SmoothingMode;
            //' and finally draw everything

            for (int j = 0; j < ndatasets; j++)
            {
                plotPen.Color = tcolour[j % tcolour.Length];
                //plot[j].Transform(World);      //' pastpv
                G.DrawPath(plotPen, plot[j]);
            }

            //G.Flush();
            pbox.Invalidate();
        }

        public void addtoplot(double[][] xdata, double[][] ydata, int simi)
        {
            if (simi < global.SimIterations)
            {
                ndatasets = ydata.Length; //this must be the same amount of datasets as for the xdata - no check done

                if (yhistory == null)
                {
                    yhistory = new double[ndatasets][];
                    for (int i = 0; i < ndatasets; i++)
                    {
                        yhistory[i] = new double[global.SimIterations];
                        if (Ymax > global.YmaxMaxFactorAbove * ydata[i][1])
                        {
                            if (ydata[i][0] == 0) { Ymax = 10.0; }
                            else { Ymax = global.YmaxMaxFactorAbove * ydata[i][1]; }
                            setuptrend(tagname, millionsx, millionsy, showlabels, Xmin, Xmax, Ymin, Ymax);
                        }
                    }
                }

                if (xhistory == null)
                {
                    xhistory = new double[ndatasets][];
                    for (int i = 0; i < ndatasets; i++)
                    {
                        xhistory[i] = new double[global.SimIterations];
                    }
                }

                for (int i = 0; i < ndatasets; i++)
                {
                    //xhistory[i][simi - 1] = xdata[i][0];
                    //xhistory[i][simi] = xdata[i][1];
                    xhistory[i][simi] = xdata[i][0];

                    //yhistory[i][simi - 1] = ydata[i][0];
                    //yhistory[i][simi] = ydata[i][1];
                    yhistory[i][simi] = ydata[i][0];

                    for (int j = 1; j < ydata[0].Length; j++)
                    {
                        if (ydata[i][j] > Ymax)
                        {
                            Ymax += (ydata[i][j] - Ymax) + global.YIncrement * Ymax;
                            setuptrend(tagname, millionsx, millionsy, showlabels, Xmin, Xmax, Ymin, Ymax);
                            drawdatasets(xhistory, yhistory, 0, simi - 1);
                        }
                        else if (ydata[i][j] < Ymin)
                        {
                            Ymin -= (Ymin - ydata[i][j]) + global.YIncrement * Ymax;
                            setuptrend(tagname, millionsx, millionsy, showlabels, Xmin, Xmax, Ymin, Ymax);
                            drawdatasets(xhistory, yhistory, 0, simi - 1);
                        }
                    }

                }

                drawdatasets(xdata, ydata);
            }
        }
    }
}
