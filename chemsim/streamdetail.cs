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
    [Serializable]
    public partial class streamdetail : Form
    {
        private simulation thesim;
        public stream thestream;
        private trend[] thetrend;
        public int lastsimitrended; //The last simi value when the trend was last updated.

        private const int NrTrends = 28;

        public streamdetail(stream astream, simulation asim)
        {
            InitializeComponent();

            thestream = astream;
            thesim = asim;

            allocatememory();

            Text = String.Concat("Detail trends for stream number: ", thestream.nr.ToString());

            thetrend = new trend[NrTrends];

            setuptrends();

            lastsimitrended = 0;
        }

        private void allocatememory()
        {
            if (thestream.mat.T.simvector == null) { thestream.mat.T.simvector = new double[global.SimVectorLength]; }

            if (thestream.mat.P.simvector == null) { thestream.mat.P.simvector = new double[global.SimVectorLength]; }

            if (thestream.actualvolumeflow.simvector == null) { thestream.actualvolumeflow.simvector = new double[global.SimVectorLength]; }

            if (thestream.standardvolumeflow.simvector == null) { thestream.standardvolumeflow.simvector = new double[global.SimVectorLength]; }

            if (thestream.massflow.simvector == null) { thestream.massflow.simvector = new double[global.SimVectorLength]; }

            if (thestream.molarflow.simvector == null) { thestream.molarflow.simvector = new double[global.SimVectorLength]; }
        }

        private void setuptrends()
        {

            thetrend[0] = new trend(pictureBox1);
            thetrend[0].setuptrend("Mass flow (kg/s)", true, false, true,
                  0, global.simtimevector[global.simtimevector.Length - 1], -2, 200);

            thetrend[1] = new trend(pictureBox2);
            thetrend[1].setuptrend("Pressure (Pa)", true, true, true,
                  0, global.simtimevector[global.simtimevector.Length - 1], -2, 20000000);

            thetrend[2] = new trend(pictureBox3);
            thetrend[2].setuptrend("Temperature (K)", true, false, true,
                  0, global.simtimevector[global.simtimevector.Length - 1], thestream.mat.T.v * 0.9, thestream.mat.T.v * 1.1);

            thetrend[3] = new trend(pictureBox4);
            thetrend[3].setuptrend("Molar flow (mol/s)", true, false, true,
                  0, global.simtimevector[global.simtimevector.Length - 1], -2, 50000);
        }


        private void streamdetail_Paint(object sender, PaintEventArgs e)
        {
            if (thesim.simi > 1)
            {
                

                thetrend[0].addtoplot(utilities.adddimension(
                        utilities.extractvectorsim(global.simtimevector, lastsimitrended, thesim.simi)),
                        utilities.adddimension(
                        utilities.extractvectorsim(thestream.massflow.simvector, lastsimitrended, thesim.simi)),
                        thesim.simi);

                thetrend[1].addtoplot(utilities.adddimension(
                        utilities.extractvectorsim(global.simtimevector, lastsimitrended, thesim.simi)),
                        utilities.adddimension(
                        utilities.extractvectorsim(thestream.mat.P.simvector, lastsimitrended, thesim.simi)),
                        thesim.simi);

                thetrend[2].addtoplot(utilities.adddimension(
                        utilities.extractvectorsim(global.simtimevector, lastsimitrended, thesim.simi)),
                        utilities.adddimension(
                        utilities.extractvectorsim(thestream.mat.T.simvector, lastsimitrended, thesim.simi)),
                        thesim.simi);

                thetrend[3].addtoplot(utilities.adddimension(
                        utilities.extractvectorsim(global.simtimevector, lastsimitrended, thesim.simi)),
                        utilities.adddimension(
                        utilities.extractvectorsim(thestream.molarflow.simvector, lastsimitrended, thesim.simi)),
                        thesim.simi);

                lastsimitrended = (thesim.simi >= global.DefaultNrIterations - 1) ? 0 : thesim.simi;
            }

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox1_DoubleClick(object sender, EventArgs e) //Doubleclick on first picture box - mass flow
        {
            utilities.exporttoexcel(utilities.adddimensionclassic(global.simtimevector), utilities.adddimensionclassic(thestream.massflow.simvector), thesim.simi);
        }

        private void pictureBox2_DoubleClick(object sender, EventArgs e) //Doubleclick on 2nd picture box - Pressure
        {
            utilities.exporttoexcel(utilities.adddimensionclassic(global.simtimevector), utilities.adddimensionclassic(thestream.mat.P.simvector), thesim.simi);
        }

        private void pictureBox3_DoubleClick(object sender, EventArgs e) //Doubleclick on 3rd picture box - Temperature
        {
            utilities.exporttoexcel(utilities.adddimensionclassic(global.simtimevector), utilities.adddimensionclassic(thestream.mat.T.simvector), thesim.simi);
        }





    }
}
