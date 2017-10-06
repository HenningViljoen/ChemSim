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
    public partial class tankdetail : Form
    {
        private simulation thesim;
        public tank thetank;
        private trend[] thetrend;
        public int lastsimitrended; //The last simi value when the trend was last updated.

        private const int NrTrends = 28;

        public tankdetail(tank atank, simulation asim)
        {
            InitializeComponent();

            thetank = atank;
            thesim = asim;

            allocatememory();

            Text = String.Concat("Detail trends for tank number: ", thetank.nr.ToString());

            thetrend = new trend[NrTrends];

            setuptrends();

        }

        private void allocatememory()
        {
            if (thetank.fracinventory.simvector == null) { thetank.fracinventory.simvector = new double[global.SimVectorLength]; }

            if (thetank.mat.T.simvector == null) { thetank.mat.T.simvector = new double[global.SimVectorLength]; }

            if (thetank.pressureatbottom.simvector == null) { thetank.pressureatbottom.simvector = new double[global.SimVectorLength]; }
        }

        private void setuptrends()
        {

            thetrend[0] = new trend(pictureBox1);
            thetrend[0].setuptrend("Fraction inventory (kg/s)", true, false, true,
                  0, global.simtimevector[global.simtimevector.Length - 1], -0.02, 1.02);

            thetrend[1] = new trend(pictureBox2);
            thetrend[1].setuptrend("Temperature", true, true, true,
                  0, global.simtimevector[global.simtimevector.Length - 1], thetank.mat.T.v * 0.9, thetank.mat.T.v * 1.1);

            thetrend[2] = new trend(pictureBox3);
            thetrend[2].setuptrend("Pressure at the bottom (Pa)", true, false, true,
                  0, global.simtimevector[global.simtimevector.Length - 1], 100000, 150000);

            thetrend[3] = new trend(pictureBox4);
            thetrend[3].setuptrend("Massinventory (kg)", true, false, true,
                  0, global.simtimevector[global.simtimevector.Length - 1], -2, 50000);
        }

        private void tankdetail_Paint(object sender, PaintEventArgs e)
        {
            if (thesim.simi > 1)
            {
                thetrend[0].addtoplot(utilities.adddimension(
                        utilities.extractvectorsim(global.simtimevector, lastsimitrended, thesim.simi)),
                        utilities.adddimension(
                        utilities.extractvectorsim(thetank.fracinventory.simvector, lastsimitrended, thesim.simi)),
                        thesim.simi);

                thetrend[1].addtoplot(utilities.adddimension(
                        utilities.extractvectorsim(global.simtimevector, lastsimitrended, thesim.simi)),
                        utilities.adddimension(
                        utilities.extractvectorsim(thetank.mat.T.simvector, lastsimitrended, thesim.simi)),
                        thesim.simi);

                thetrend[2].addtoplot(utilities.adddimension(
                        utilities.extractvectorsim(global.simtimevector, lastsimitrended, thesim.simi)),
                        utilities.adddimension(
                        utilities.extractvectorsim(thetank.pressureatbottom.simvector, lastsimitrended, thesim.simi)),
                        thesim.simi);

                //thetrend[0].addtoplot(utilities.adddimension(
                //        new double[] { global.simtimevector[thesim.simi - 2], global.simtimevector[thesim.simi - 1] }),
                //        utilities.adddimension(
                //        new double[] { thetank.fracinventory.simvector[thesim.simi - 2], 
                //            thetank.fracinventory.simvector[thesim.simi - 1]}),
                //        thesim.simi);

                //thetrend[1].addtoplot(utilities.adddimension(
                //        new double[] { global.simtimevector[thesim.simi - 2], global.simtimevector[thesim.simi - 1] }),
                //        utilities.adddimension(
                //        new double[] { thetank.mat.T.simvector[thesim.simi - 2], 
                //            thetank.mat.T.simvector[thesim.simi - 1]}),
                //        thesim.simi);

                //thetrend[2].addtoplot(utilities.adddimension(
                //        new double[] { global.simtimevector[thesim.simi - 2], global.simtimevector[thesim.simi - 1] }),
                //        utilities.adddimension(
                //        new double[] { thetank.pressureatbottom.simvector[thesim.simi - 2], 
                //            thetank.pressureatbottom.simvector[thesim.simi - 1]}),
                //        thesim.simi);

                lastsimitrended = (thesim.simi >= global.DefaultNrIterations - 1) ? 0 : thesim.simi;
            }
        }


    }
}
