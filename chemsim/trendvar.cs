using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace chemsim
{
    public partial class trendvar : Form
    {
        public double[][] xdata;
        public double[][] ydata;

        public trendvar(double[][] axdata, double[][] aydata,
                        String tagname, bool millionsx = false,
                        bool millionsy = false, bool showlabels = true, double aymin = 0.0, double aymax = 0.0)
        {
            InitializeComponent();

            xdata = axdata;
            ydata = aydata;
            trend thetrend = new trend(pictureBox1);
            thetrend.plot(xdata, ydata, tagname, millionsx, millionsy, showlabels, aymin, aymax);
            //public void plot(double[][] xdata, double[][] ydata,
            //            String tagname, bool millionsx,
            //            bool millionsy, bool showlabels, double aymin = 0.0, double aymax = 0.0)
        }

        private void pictureBox1_DoubleClick(object sender, EventArgs e) //double click event to export to Excel
        {
            utilities.exporttoexcel(utilities.adddimensionclassic(xdata[0]), utilities.adddimensionclassic(ydata[0]),
                xdata[0].Length);
        }
    }
}
