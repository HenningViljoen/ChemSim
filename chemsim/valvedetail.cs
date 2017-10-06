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
    public partial class valvedetail : Form
    {
        private simulation thesim;
        public valve thevalve;
        private trend[] thetrend;
        public int lastsimitrended; //The last simi value when the trend was last updated.

        private const int NrTrends = 28;

        public valvedetail(valve avalve, simulation asim)
        {
            InitializeComponent();

            thevalve = avalve;
            thesim = asim;

            allocatememory();

            Text = String.Concat("Detail trends for Valve number: ", thevalve.nr.ToString());

            thetrend = new trend[NrTrends];

            setuptrends();
        }

        private void allocatememory()
        {
            if (thevalve.deltapressure.simvector == null) { thevalve.deltapressure.simvector = new double[global.SimVectorLength]; }

            if (thevalve.op.simvector == null) { thevalve.op.simvector = new double[global.SimVectorLength]; }

            if (thevalve.actualvolumeflow.simvector == null) { thevalve.actualvolumeflow.simvector = new double[global.SimVectorLength]; }

            if (thevalve.standardvolumeflow.simvector == null) { thevalve.standardvolumeflow.simvector = new double[global.SimVectorLength]; }

            if (thevalve.massflow.simvector == null) { thevalve.massflow.simvector = new double[global.SimVectorLength]; }

            if (thevalve.molarflow.simvector == null) { thevalve.molarflow.simvector = new double[global.SimVectorLength]; }
        }

        private void setuptrends()
        {
            thetrend[0] = new trend(pictureBox1);
            thetrend[0].setuptrend("Mass flow (kg/h)", true, false, true,
                  0, global.simtimevector[global.simtimevector.Length - 1], -2, 20000000);

            thetrend[1] = new trend(pictureBox2);
            thetrend[1].setuptrend("Delta Pressure (Pa)", true, true, true,
                  0, global.simtimevector[global.simtimevector.Length - 1], -2, 20000000);

            thetrend[2] = new trend(pictureBox3);
            thetrend[2].setuptrend("Valve opening (fraction)", true, false, true,
                  0, global.simtimevector[global.simtimevector.Length - 1], -0.1, 200);
        }

        private void valvedetail_Paint(object sender, PaintEventArgs e)
        {
            if (thesim.simi > 1)
            {
                thetrend[0].addtoplot(utilities.adddimension(
                        utilities.extractvectorsim(global.simtimevector, lastsimitrended, thesim.simi)),
                        utilities.adddimension(
                        utilities.extractvectorsim(thevalve.massflow.simvector, lastsimitrended, thesim.simi)),
                        thesim.simi);

                thetrend[1].addtoplot(utilities.adddimension(
                        utilities.extractvectorsim(global.simtimevector, lastsimitrended, thesim.simi)),
                        utilities.adddimension(
                        utilities.extractvectorsim(thevalve.deltapressure.simvector, lastsimitrended, thesim.simi)),
                        thesim.simi);

                thetrend[2].addtoplot(utilities.adddimension(
                        utilities.extractvectorsim(global.simtimevector, lastsimitrended, thesim.simi)),
                        utilities.adddimension(
                        utilities.extractvectorsim(thevalve.op.simvector, lastsimitrended, thesim.simi)),
                        thesim.simi);
    
                lastsimitrended = (thesim.simi >= global.DefaultNrIterations - 1) ? 0 : thesim.simi;
            }
        }




    }
}
