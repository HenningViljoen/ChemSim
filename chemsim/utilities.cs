using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Office.Interop.Excel;

namespace chemsim
{
    [Serializable]
    public static class utilities
    {
        public delegate double doubleprocessor(double d);

        public static double calcdirection(double deltay, double deltax)
        {
            double bias;
            if (deltax == 0) { deltax = 0.0001; }
            if (deltax < 0)
            {
                bias = Math.PI;
            }
            else
            {
                bias = 0;
            }

            return bias + Math.Atan(deltay / deltax);
        }

        public static double distance(point p1, point p2)
        {
            return Math.Sqrt(Math.Pow(p1.x - p2.x, 2) + Math.Pow(p1.y - p2.y, 2));
        }

        public static double distance(double deltax, double deltay)
        {
            return Math.Sqrt(Math.Pow(deltax, 2) + Math.Pow(deltay, 2));
        }

        public static double normsinv(double p)
        {
            const double a1 = -39.6968302866538, a2 = 220.946098424521, a3 = -275.928510446969;
            const double a4 = 138.357751867269, a5 = -30.6647980661472, a6 = 2.50662827745924;
            const double b1 = -54.4760987982241, b2 = 161.585836858041, b3 = -155.698979859887;
            const double b4 = 66.8013118877197, b5 = -13.2806815528857, c1 = -0.00778489400243029;
            const double c2 = -0.322396458041136, c3 = -2.40075827716184, c4 = -2.54973253934373;
            const double c5 = 4.37466414146497, c6 = 2.93816398269878, d1 = 0.00778469570904146;
            const double d2 = 0.32246712907004, d3 = 2.445134137143, d4 = 3.75440866190742;
            const double p_low = 0.02425, p_high = 1 - p_low;
            double q, r;
            if (p < 0 || p > 1)
            {
                return 0; //Error case
                //MessageBox.Show(
                //        "Error", 
                //        "OK?", MessageBoxButtons.YesNo, 
                //        MessageBoxIcon.Question);
            }
            else if (p < p_low)
            {
                q = Math.Sqrt(-2 * Math.Log(p));
                return (((((c1 * q + c2) * q + c3) * q + c4) * q + c5) * q + c6) /
                    ((((d1 * q + d2) * q + d3) * q + d4) * q + 1);
            }
            else if (p <= p_high)
            {
                q = p - 0.5;
                r = q * q;
                return (((((a1 * r + a2) * r + a3) * r + a4) * r + a5) * r + a6) * q /
                    (((((b1 * r + b2) * r + b3) * r + b4) * r + b5) * r + 1);
            }
            else
            {
                q = Math.Sqrt(-2 * Math.Log(1 - p));
                return -(((((c1 * q + c2) * q + c3) * q + c4) * q + c5) * q + c6) /
                    ((((d1 * q + d2) * q + d3) * q + d4) * q + 1);
            }
        }

        //Returns a normall distributed value around 0, with limits of 3 sigma above and below 0
        public static double randnormv(double a3sigma, Random r)
        {
            return (normsinv(r.NextDouble()) * 2 - 1) * a3sigma;
        }

        public static double matrixmax(double[][] m)
        {
            //int  j;
            double maxvalue = -9999999;
            for (int i = 0; i < m.Length; i++)
            {
                for (int j = 0; j < m[i].Length; j++)
                {
                    if (m[i][j] > maxvalue) { maxvalue = m[i][j]; }
                }
            }
            return maxvalue;
        }

        public static double matrixmin(double[][] m)
        {
            //Dim i, j As Integer
            double minvalue = 999999999;
            for (int i = 0; i < m.Length; i++)
            {
                for (int j = 0; j < m[i].Length; j++)
                {
                    if (m[i][j] < minvalue) { minvalue = m[i][j]; }
                }
            }
            return minvalue;
        }

        //Vector manipulation functions -------------------------------------------------------------------------------------------------------------------

        public static double[][] adddimension(double[] v)
        {
            double[][] m;
            int vlength = v.Length;
            m = new double[1][];
            m[0] = new double[vlength];
            for (int i = 0; i < vlength; i++) { m[0][i] = v[i]; }
            return m;
        }

        public static double[,] adddimensionclassic(double[] v)
        {
            double[,] m;
            int vlength = v.Length;
            m = new double[vlength, 1];
            
            for (int i = 0; i < vlength; i++) { m[i,0] = v[i]; }
            return m;
        }

        public static double[] extractvectorsim(double[] v, int starti, int endi) //Extract a vector from another vector from the start index to the end.
                                                                               //Index will be compensated for by the historisation frequency in 
                                                                               //SimVectorUpdatePeriod
        {
            int realstarti = starti / global.SimVectorUpdatePeriod;
            int realendi = endi/ global.SimVectorUpdatePeriod;

            double[] vector = new double[realendi - realstarti + 1];
            for (int i = realstarti; i <= realendi; i++)
            {
                vector[i - realstarti] = (i < v.Length) ? v[i] : v[v.Length - 1]; //If the index to too large, take the last value in v.
            }
            return vector;
        }

        //End: Vector manipulation functions --------------------------------------------------------------------------------------------------------------

        public static double[] vectorprocessor(double[] v, doubleprocessor d)
        {
            double[] m = new double[v.Length]; ;
            for (int i = 0; i < v.Length; i++)
            {
                m[i] = d(v[i]);
            }
            return m;
        }

        public static double fph2dndt(double aflowsph)
        {
            return aflowsph * global.Ps / (global.R * global.Ts) / 3600;
        }

        public static double fph2dndt(controlvar aflowsph)
        {
            return fph2dndt(aflowsph.v);
        }

        public static double dndt2fph(double adndt)
        {
            return adndt * (global.R * global.Ts) * 3600 / global.Ps;
        }

        public static double dndt2fph(controlvar adndt)
        {
            return dndt2fph(adndt.v);
        }

        public static double fps2dndt(double aflowsps)
        {
            return aflowsps * global.Ps / (global.R * global.Ts);
        }

        public static double fps2dndt(controlvar aflowsps)
        {
            return fps2dndt(aflowsps.v);
        }

        public static double dndt2fps(double adndt)
        {
            return adndt * (global.R * global.Ts) / global.Ps;
        }

        public static double dndt2fps(controlvar adndt)
        {
            return dndt2fps(adndt.v);
        }

        public static double barg2pascal(double apres)
        {
            return apres * global.Ps + global.Ps;
        }

        public static double barg2pascal(controlvar apres)
        {
            return barg2pascal(apres.v);
        }

        public static double pascal2barg(double apres)
        {
            return (apres - global.Ps) / global.Ps;
        }

        public static double pascal2barg(controlvar apres)
        {
            return pascal2barg(apres.v);
        }

        public static double bara2pascal(double apres)
        {
            return apres * global.Ps;
        }

        public static double bara2pascal(controlvar apres)
        {
            return bara2pascal(apres.v);
        }

        public static double pascal2bara(double apres)
        {
            return (apres) / global.Ps;
        }

        public static double pascal2bara(controlvar apres)
        {
            return pascal2bara(apres.v);
        }

        public static double fph2fps(double aflowph)
        {
            return aflowph / 3600;
        }

        public static double fph2fps(controlvar aflowph)
        {
            return fph2fps(aflowph.v);
        }

        public static double fps2fph(double aflowps)
        {
            return aflowps * 3600;
        }

        public static double fps2fph(controlvar aflowps)
        {
            return fps2fph(aflowps.v);
        }

        public static double celcius2kelvin(double atempcelcius)
        {
            return atempcelcius + 273.15;
        }

        public static double celcius2kelvin(controlvar atempcelcius)
        {
            return celcius2kelvin(atempcelcius.v);
        }

        public static double kelvin2celcius(double atempkelvin)
        {
            return atempkelvin - 273.15;
        }

        public static double kelvin2celcius(controlvar atempkelvin)
        {
            return kelvin2celcius(atempkelvin.v);
        }

        public static double radians2degrees(double aangleradians)
        {
            return aangleradians * 180 / Math.PI;
        }

        public static double radians2degrees(controlvar aangleradians)
        {
            return radians2degrees(aangleradians.v);
        }

        public static double deegrees2radians(double aangledegrees)
        {
            return aangledegrees * Math.PI / 180;
        }

        public static double deegrees2radians(controlvar aangledegrees)
        {
            return deegrees2radians(aangledegrees.v);
        }

        public static double flowsqrt(double dp, double previousflow) //If v is negative, will return 0.  This can be used for hydraulic flow calcs where
        // you want to prevent bugs and negative flow.
        {
            double y;
            if (dp < 0) { y = 0.8 * previousflow; }
            else { y = Math.Sqrt(dp); }
            return y;
        }

        public static double sigmoid(double t)
        {
            return 1 / (1 + Math.Exp(-t));
        }
        

        public static void exporttoexcel(double[,] xdata, double[,] ydata, int simi) //simi is thel last index to be included in the trend
        {
            Microsoft.Office.Interop.Excel.Application excelapp;
            Microsoft.Office.Interop.Excel.Workbook xlWorkbook;
            Microsoft.Office.Interop.Excel._Worksheet xlWorksheet;
            Microsoft.Office.Interop.Excel.Range xlRange0, xlRange1;

            excelapp = new Microsoft.Office.Interop.Excel.Application();
            excelapp.Visible = true;
            xlWorkbook = (Microsoft.Office.Interop.Excel.Workbook)(excelapp.Workbooks.Add(""));
            xlWorksheet = (Microsoft.Office.Interop.Excel._Worksheet)xlWorkbook.ActiveSheet;
            xlWorksheet.Cells[1, 1] = "Time (s)";
            xlWorksheet.Cells[1, 2] = "Data (SI units)";
            simi = (simi > xdata.Length - 1) ? xdata.Length - 1 : simi;
            int row = 2 + simi - 1;
            xlRange0 = xlWorksheet.Range["A2", "A" + row.ToString()];
            xlRange0.Value2 = xdata;
            xlRange0 = xlWorksheet.Range["B2", "B" + row.ToString()];
            xlRange0.Value2 = ydata;
            //xlRange0 = xlWorksheet.get_Range(xlWorksheet.Cells[2, 1], xlWorksheet.Cells[2 - 1 + xdata.Length, 1]);
            //for (int i = 0; i < simi; i++)
            //{
            //    xlRange0.Value2  value2 = xdata;
            //    ///xlWorksheet.Cells[i + 2, 1] = xdata[i];
            //    //xlWorksheet.Cells[i + 2, 2] = ydata[i];
            //}
            //xlWorkbook.Close();
        }

    }
}
