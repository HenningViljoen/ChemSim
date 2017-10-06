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
    public partial class plotz : Form
    {
        private material m;

        public plotz()
        {
            InitializeComponent();
            drawgraph();
        }

        private void drawgraph()
        {
            m = new material(global.baseprocessclassInitVolume);
            m.mapvarstox();
            double[][] x = new double[1][];
            for (int i = 0; i < 1; i++) { x[i] = new double[1000]; }
            double[][] y = new double[1][];
            for (int i = 0; i < 1; i++) { y[i] = new double[1000]; }

            for (int j = 0; j < 1; j++)
            {
                m.calcZ(j);
                for (int i = 0; i < 1000; i++)
                {
                    x[j][i] = -0.26 + i / 1000.0 * 1.26;
                    y[j][i] = m.acomp[j] * Math.Pow(x[j][i], 3) + m.bcomp[j] * Math.Pow(x[j][i], 2) + m.ccomp[j] * x[j][i] + m.dcomp[j];
                }
            }

            trend t = new trend(pictureBox1);
            t.plot(x, y, "Z trend", false, false, true);

        }
    }
}

