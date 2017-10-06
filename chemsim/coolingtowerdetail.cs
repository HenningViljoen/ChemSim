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
    public partial class coolingtowerdetail : Form
    {
        private simulation thesim;
        public coolingtower thecoolingtower;

        private trend[] thetrend;

        private const int NrTrends = 30;
        public int lastsimitrended; //The last simi value when the trend was last updated.

        public coolingtowerdetail(coolingtower acoolingtower, simulation asim)
        {
            InitializeComponent();

            thecoolingtower = acoolingtower;
            thesim = asim;

            allocatememory();

            thetrend = new trend[NrTrends];

            setuptrends();

            lastsimitrended = 0;
        }

        private void allocatememory()
        {
            if (thecoolingtower.fanpower.simvector == null || thecoolingtower.fanpower.simvector.Length != global.SimVectorLength)
                { thecoolingtower.fanpower.simvector = new double[global.SimVectorLength]; }

            if (thecoolingtower.fanspeed.simvector == null || thecoolingtower.fanspeed.simvector.Length != global.SimVectorLength)
                { thecoolingtower.fanspeed.simvector = new double[global.SimVectorLength]; }

            for (int i = 0; i < thecoolingtower.nrstages; i++)
            {
                if (thecoolingtower.watersegment[i].T.simvector == null || 
                    thecoolingtower.watersegment[i].T.simvector.Length != global.SimVectorLength) 
                    { thecoolingtower.watersegment[i].T.simvector = new double[global.SimVectorLength]; }

                if (thecoolingtower.interfacetemperaturesimvector[i] == null || 
                    thecoolingtower.interfacetemperaturesimvector[i].Length != global.SimVectorLength)
                { thecoolingtower.interfacetemperaturesimvector[i] = new double[global.SimVectorLength]; }
            }
        }

        private void setuptrends()
        {
            thetrend[0] = new trend(pictureBox1);
            thetrend[0].setuptrend("interface humidity[0] kg/kg", true, false, true,
                  0, global.simtimevector[global.simtimevector.Length - 1], -2, 20000000);

            thetrend[1] = new trend(pictureBox2);
            thetrend[1].setuptrend("interface humidity[4] kg/kg", true, false, true,
                  0, global.simtimevector[global.simtimevector.Length - 1], -2, 20000000);

            thetrend[2] = new trend(pictureBox3);
            thetrend[2].setuptrend("interface humidity[9] kg/kg", true, false, true,
                  0, global.simtimevector[global.simtimevector.Length - 1], -2, 20000000);

            thetrend[3] = new trend(pictureBox4);
            thetrend[3].setuptrend("airabshumidity[0] kg/kg", true, false, true,
                  0, global.simtimevector[global.simtimevector.Length - 1], -2, 20000000);

            thetrend[4] = new trend(pictureBox5);
            thetrend[4].setuptrend("airabshumidity[4] kg/kg", true, false, true,
                  0, global.simtimevector[global.simtimevector.Length - 1], -2, 20000000);

            thetrend[5] = new trend(pictureBox6);
            thetrend[5].setuptrend("airabshumidity[9] kg/kg", true, false, true,
                  0, global.simtimevector[global.simtimevector.Length - 1], -2, 20000000);

            thetrend[6] = new trend(pictureBox7);
            thetrend[6].setuptrend("watersegment[0].T Kelvin", true, false, true,
                  0, global.simtimevector[global.simtimevector.Length - 1], thecoolingtower.watersegment[0].T.v * 0.9, 
                  thecoolingtower.watersegment[0].T.v * 1.1);

            thetrend[7] = new trend(pictureBox8);
            thetrend[7].setuptrend("watersegment[4].T Kelvin", true, false, true,
                  0, global.simtimevector[global.simtimevector.Length - 1], thecoolingtower.watersegment[4].T.v * 0.9,
                  thecoolingtower.watersegment[4].T.v * 1.1);

            thetrend[8] = new trend(pictureBox9);
            thetrend[8].setuptrend("watersegment[9].T Kelvin", true, false, true,
                  0, global.simtimevector[global.simtimevector.Length - 1], thecoolingtower.watersegment[9].T.v * 0.9,
                  thecoolingtower.watersegment[9].T.v * 1.1);

            thetrend[9] = new trend(pictureBox10);
            thetrend[9].setuptrend("interfacetemperaturesimvector[0] Kelvin", true, false, true,
                  0, global.simtimevector[global.simtimevector.Length - 1], thecoolingtower.interfacetemperature[0] * 0.9,
                  thecoolingtower.interfacetemperature[0] * 1.1);

            thetrend[10] = new trend(pictureBox11);
            thetrend[10].setuptrend("interfacetemperaturesimvector[4] Kelvin", true, false, true,
                  0, global.simtimevector[global.simtimevector.Length - 1], thecoolingtower.interfacetemperature[4] * 0.9,
                  thecoolingtower.interfacetemperature[4] * 1.1);

            thetrend[11] = new trend(pictureBox12);
            thetrend[11].setuptrend("interfacetemperaturesimvector[9] Kelvin", true, false, true,
                  0, global.simtimevector[global.simtimevector.Length - 1], thecoolingtower.interfacetemperature[9] * 0.9,
                  thecoolingtower.interfacetemperature[9] * 1.1);


            

            thetrend[12] = new trend(pictureBox13);
            thetrend[12].setuptrend("Fan Power Consumption (W)", true, false, true,
                  0, global.simtimevector[global.simtimevector.Length - 1], -2, 20000000);

            thetrend[13] = new trend(pictureBox14);
            thetrend[13].setuptrend("Fan Speed (rps)", true, false, true,
                  0, global.simtimevector[global.simtimevector.Length - 1], -2, 20000000);
        }

        private void coolingtowerdetail_Paint(object sender, PaintEventArgs e)
        {
            if (thesim.simi > 1)
            {
                thetrend[0].addtoplot(utilities.adddimension(
                        utilities.extractvectorsim(global.simtimevector, lastsimitrended, thesim.simi)),
                        utilities.adddimension(
                        utilities.extractvectorsim(thecoolingtower.interfaceabshumiditysimvector[0],lastsimitrended,thesim.simi)),
                        thesim.simi);

                thetrend[1].addtoplot(utilities.adddimension(
                        utilities.extractvectorsim(global.simtimevector, lastsimitrended,thesim.simi)),
                        utilities.adddimension(
                        utilities.extractvectorsim(thecoolingtower.interfaceabshumiditysimvector[4], lastsimitrended,thesim.simi)),
                        thesim.simi);

                thetrend[2].addtoplot(utilities.adddimension(
                        utilities.extractvectorsim(global.simtimevector, lastsimitrended,thesim.simi)),
                        utilities.adddimension(
                        utilities.extractvectorsim(thecoolingtower.interfaceabshumiditysimvector[9],lastsimitrended,thesim.simi)),
                        thesim.simi);

                thetrend[3].addtoplot(utilities.adddimension(
                       utilities.extractvectorsim(global.simtimevector,lastsimitrended,thesim.simi)),
                       utilities.adddimension(
                       utilities.extractvectorsim(thecoolingtower.airabshumiditysimvector[0],lastsimitrended,thesim.simi)),
                       thesim.simi);

                thetrend[4].addtoplot(utilities.adddimension(
                        utilities.extractvectorsim(global.simtimevector,lastsimitrended,thesim.simi)),
                        utilities.adddimension(
                        utilities.extractvectorsim(thecoolingtower.airabshumiditysimvector[4],lastsimitrended,thesim.simi)),
                        thesim.simi);

                thetrend[5].addtoplot(utilities.adddimension(
                        utilities.extractvectorsim(global.simtimevector,lastsimitrended,thesim.simi)),
                        utilities.adddimension(
                        utilities.extractvectorsim(thecoolingtower.airabshumiditysimvector[9],lastsimitrended,thesim.simi)),
                        thesim.simi);

                thetrend[6].addtoplot(utilities.adddimension(
                        utilities.extractvectorsim(global.simtimevector,lastsimitrended,thesim.simi)),
                        utilities.adddimension(
                        utilities.extractvectorsim(thecoolingtower.watersegment[0].T.simvector,lastsimitrended,thesim.simi)),
                        thesim.simi);

                thetrend[7].addtoplot(utilities.adddimension(
                        utilities.extractvectorsim(global.simtimevector,lastsimitrended,thesim.simi)),
                        utilities.adddimension(
                        utilities.extractvectorsim(thecoolingtower.watersegment[4].T.simvector,lastsimitrended,thesim.simi)),
                        thesim.simi);

                thetrend[8].addtoplot(utilities.adddimension(
                        utilities.extractvectorsim(global.simtimevector,lastsimitrended,thesim.simi)),
                        utilities.adddimension(
                        utilities.extractvectorsim(thecoolingtower.watersegment[9].T.simvector,lastsimitrended,thesim.simi)),
                        thesim.simi);

                thetrend[9].addtoplot(utilities.adddimension(
                        utilities.extractvectorsim(global.simtimevector,lastsimitrended,thesim.simi)),
                        utilities.adddimension(
                        utilities.extractvectorsim(thecoolingtower.interfacetemperaturesimvector[0], lastsimitrended, thesim.simi)),
                        thesim.simi);

                thetrend[10].addtoplot(utilities.adddimension(
                        utilities.extractvectorsim(global.simtimevector,lastsimitrended,thesim.simi)),
                        utilities.adddimension(
                        utilities.extractvectorsim(thecoolingtower.interfacetemperaturesimvector[4],lastsimitrended,thesim.simi)),
                        thesim.simi);

                thetrend[11].addtoplot(utilities.adddimension(
                        utilities.extractvectorsim(global.simtimevector,lastsimitrended,thesim.simi)),
                        utilities.adddimension(
                        utilities.extractvectorsim(thecoolingtower.interfacetemperaturesimvector[9],lastsimitrended,thesim.simi)),
                        thesim.simi);




                thetrend[12].addtoplot(utilities.adddimension(
                        utilities.extractvectorsim(global.simtimevector,lastsimitrended,thesim.simi)),
                        utilities.adddimension(
                        utilities.extractvectorsim(thecoolingtower.fanpower.simvector,lastsimitrended,thesim.simi)),
                        thesim.simi);

                thetrend[13].addtoplot(utilities.adddimension(
                        utilities.extractvectorsim(global.simtimevector,lastsimitrended,thesim.simi)),
                        utilities.adddimension(
                        utilities.extractvectorsim(thecoolingtower.fanspeed.simvector,lastsimitrended,thesim.simi)),
                        thesim.simi);

                lastsimitrended = (thesim.simi >= global.DefaultNrIterations - 1) ? 0 : thesim.simi;
            }
        }

        private void pictureBox9_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox9_DoubleClick(object sender, EventArgs e) //Double click event for pb 9 
        {
            
        }

        private void pictureBox10_DoubleClick(object sender, EventArgs e) //Double click event for pb 10 
        {
            

        }

        private void pictureBox13_Click(object sender, EventArgs e) //Double click event for pb 13 which is the fan power one.
        {
            utilities.exporttoexcel(utilities.adddimensionclassic(global.simtimevector), utilities.adddimensionclassic(thecoolingtower.fanpower.simvector), thesim.simi);
        }

        private void pictureBox14_Click(object sender, EventArgs e) //Double click event for pb 14 which is the fan speed one.
        {
            utilities.exporttoexcel(utilities.adddimensionclassic(global.simtimevector), utilities.adddimensionclassic(thecoolingtower.fanspeed.simvector), thesim.simi);
        }


    }
}
