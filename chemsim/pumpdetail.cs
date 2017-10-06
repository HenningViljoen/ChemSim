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
    public partial class pumpdetail : Form
    {
        private simulation thesim;
        public pump thepump;

        private trend[] thetrend;

        private const int NrTrends = 30;
        public int lastsimitrended; //The last simi value when the trend was last updated.

        public pumpdetail(pump apump, simulation asim)
        {
            InitializeComponent();

            thepump = apump;
            thesim = asim;
            allocatememory();

            Text = String.Concat("Detail trends for pump number: ", thepump.nr.ToString());

            thetrend = new trend[NrTrends];

            setuptrends();

            lastsimitrended = 0;
        }

        private void allocatememory()
        {
            if (thepump.deltapressure.simvector == null) { thepump.deltapressure.simvector = new double[global.SimVectorLength]; }

            if (thepump.actualvolumeflow.simvector == null) { thepump.actualvolumeflow.simvector = new double[global.SimVectorLength]; }

            if (thepump.pumpspeed.simvector == null) { thepump.pumpspeed.simvector = new double[global.SimVectorLength]; }

            if (thepump.pumppower.simvector == null) { thepump.pumppower.simvector = new double[global.SimVectorLength]; }
        }

        private void setuptrends()
        {
            thetrend[0] = new trend(pictureBox1);
            thetrend[0].setuptrend("Pump power (W)", true, false, true,
                  0, global.simtimevector[global.simtimevector.Length - 1], -2, 20000000);

            thetrend[1] = new trend(pictureBox2);
            thetrend[1].setuptrend("Pump speed (RPS)", true, false, true,
                  0, global.simtimevector[global.simtimevector.Length - 1], -2, 20000000);

            
        }

        private void pumpdetail_Paint(object sender, PaintEventArgs e)
        {
            if (thesim.simi > 1)
            {

                thetrend[0].addtoplot(utilities.adddimension(
                        utilities.extractvectorsim(global.simtimevector, lastsimitrended, thesim.simi)),
                        utilities.adddimension(
                        utilities.extractvectorsim(thepump.pumppower.simvector, lastsimitrended, thesim.simi)),
                        thesim.simi);

                //utilities.extractvector(global.simtimevector,lastsimitrended,thesim.simi)

                thetrend[1].addtoplot(utilities.adddimension(
                        utilities.extractvectorsim(global.simtimevector, lastsimitrended, thesim.simi)),
                        utilities.adddimension(
                        utilities.extractvectorsim(thepump.pumpspeed.simvector, lastsimitrended, thesim.simi)),
                        thesim.simi);

                lastsimitrended = (thesim.simi >= global.DefaultNrIterations - 1) ? 0 : thesim.simi;
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox1_DoubleClick(object sender, EventArgs e) //Excel export of data for pumppower
        {
            utilities.exporttoexcel(utilities.adddimensionclassic(global.simtimevector), 
                utilities.adddimensionclassic(thepump.pumppower.simvector), thesim.simi);
        }

        private void pictureBox2_DoubleClick(object sender, EventArgs e) //Excel export of data for pumpspeed
        {
            utilities.exporttoexcel(utilities.adddimensionclassic(global.simtimevector),
                utilities.adddimensionclassic(thepump.pumpspeed.simvector), thesim.simi);
        }


    }
}
