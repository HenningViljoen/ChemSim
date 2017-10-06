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
    public partial class steamgeneratordetail : Form
    {
        private simulation thesim;
        private steamgenerator thesteamgenerator;
        private trend[] thetrend;

        private const int NrTrends = 28;

        public steamgeneratordetail(steamgenerator asteamgenerator, simulation asim)
        {
            InitializeComponent();

            thesteamgenerator = asteamgenerator;
            thesim = asim;
            thetrend = new trend[NrTrends];

            setuptrends();
        }

        private void setuptrends()
        {
            thetrend[0] = new trend(pictureBox1);
            thetrend[0].setuptrend("Ps[0] (Pascal)", false, false, true,
                  0, global.simtimevector[global.simtimevector.Length - 1], -2, 20000000);

            thetrend[1] = new trend(pictureBox2);
            thetrend[1].setuptrend("Ps[1] (Pascal)", false, false, true,
                  0, global.simtimevector[global.simtimevector.Length - 1], -2, 20000000);

            thetrend[2] = new trend(pictureBox3);
            thetrend[2].setuptrend("Ps[2] (Pascal)", false, false, true,
                  0, global.simtimevector[global.simtimevector.Length - 1], -2, 20000000);

            thetrend[3] = new trend(pictureBox4);
            thetrend[3].setuptrend("Tgave[0] (K)", false, false, true,
                  0, global.simtimevector[global.simtimevector.Length - 1], -2, 1000);

            thetrend[4] = new trend(pictureBox5);
            thetrend[4].setuptrend("Tgave[1] (K)", false, false, true,
                  0, global.simtimevector[global.simtimevector.Length - 1], -2, 1000);

            thetrend[5] = new trend(pictureBox6);
            thetrend[5].setuptrend("Tgave[2] (K)", false, false, true,
                  0, global.simtimevector[global.simtimevector.Length - 1], -2, 1000);

            thetrend[6] = new trend(pictureBox7);
            thetrend[6].setuptrend("Gas[0] Psimvector (Pa)", false, false, true,
                  0, global.simtimevector[global.simtimevector.Length - 1], -2, 200);

            thetrend[7] = new trend(pictureBox8);
            thetrend[7].setuptrend("Gas[1] Psimvector (Pa)", false, false, true,
                  0, global.simtimevector[global.simtimevector.Length - 1], -2, 200);

            thetrend[8] = new trend(pictureBox9);
            thetrend[8].setuptrend("Gas[2] Psimvector (Pa)", false, false, true,
                  0, global.simtimevector[global.simtimevector.Length - 1], -2, 200);

            thetrend[9] = new trend(pictureBox10);
            thetrend[9].setuptrend("Tmave[0] (K)", false, false, true,
                  0, global.simtimevector[global.simtimevector.Length - 1], -2, 1000);

            thetrend[10] = new trend(pictureBox11);
            thetrend[10].setuptrend("Tmave[1] (K)", false, false, true,
                  0, global.simtimevector[global.simtimevector.Length - 1], -2, 1000);

            thetrend[11] = new trend(pictureBox12);
            thetrend[11].setuptrend("Tmave[2] (K)", false, false, true,
                  0, global.simtimevector[global.simtimevector.Length - 1], -2, 1000);

            thetrend[12] = new trend(pictureBox13);
            thetrend[12].setuptrend("Qgm[0] (W)", false, false, true,
                  0, global.simtimevector[global.simtimevector.Length - 1], -2, 1000000);

            thetrend[13] = new trend(pictureBox14);
            thetrend[13].setuptrend("Qgm[1] (W)", false, false, true,
                  0, global.simtimevector[global.simtimevector.Length - 1], -2, 1000000);

            thetrend[14] = new trend(pictureBox15);
            thetrend[14].setuptrend("Qgm[2] (W)", false, false, true,
                  0, global.simtimevector[global.simtimevector.Length - 1], -2, 1000000);

            thetrend[15] = new trend(pictureBox16);
            thetrend[15].setuptrend("Qms[0] (W)", false, false, true,
                  0, global.simtimevector[global.simtimevector.Length - 1], -2, 1000000);

            thetrend[16] = new trend(pictureBox17);
            thetrend[16].setuptrend("Qms[1] (W)", false, false, true,
                  0, global.simtimevector[global.simtimevector.Length - 1], -2, 1000000);

            thetrend[17] = new trend(pictureBox18);
            thetrend[17].setuptrend("Qms[2] (W)", false, false, true,
                  0, global.simtimevector[global.simtimevector.Length - 1], -2, 1000000);

            thetrend[18] = new trend(pictureBox19);
            thetrend[18].setuptrend("Tsave[0] (W)", false, false, true,
                  0, global.simtimevector[global.simtimevector.Length - 1], -2, 1000);

            thetrend[19] = new trend(pictureBox20);
            thetrend[19].setuptrend("Tsave[1] (W)", false, false, true,
                  0, global.simtimevector[global.simtimevector.Length - 1], -2, 1000);

            thetrend[20] = new trend(pictureBox21);
            thetrend[20].setuptrend("Tsave[2] (W)", false, false, true,
                  0, global.simtimevector[global.simtimevector.Length - 1], -2, 1200);

            thetrend[21] = new trend(pictureBox22);
            thetrend[21].setuptrend("Hs[0] (W)", false, false, true,
                  0, global.simtimevector[global.simtimevector.Length - 1], -2, 1000000);

            thetrend[22] = new trend(pictureBox23);
            thetrend[22].setuptrend("Hs[1] (W)", false, false, true,
                  0, global.simtimevector[global.simtimevector.Length - 1], -2, 1000000);

            thetrend[23] = new trend(pictureBox24);
            thetrend[23].setuptrend("Hs[2] (W)", false, false, true,
                  0, global.simtimevector[global.simtimevector.Length - 1], -2, 1000000);

            thetrend[24] = new trend(pictureBox25);
            thetrend[24].setuptrend("Water[0] f", false, false, true,
                  0, global.simtimevector[global.simtimevector.Length - 1], -2, 2000);

            thetrend[25] = new trend(pictureBox26);
            thetrend[25].setuptrend("Water[1] f", false, false, true,
                  0, global.simtimevector[global.simtimevector.Length - 1], -2, 2000);

            thetrend[26] = new trend(pictureBox27);
            thetrend[26].setuptrend("Water[2] f", false, false, true,
                  0, global.simtimevector[global.simtimevector.Length - 1], -2, 2000);

            thetrend[27] = new trend(pictureBox28);
            thetrend[27].setuptrend("Tm[3] (W)", false, false, true,
                  0, global.simtimevector[global.simtimevector.Length - 1], -2, 2000);
        }

        private void steamgeneratordetail_Paint2(object sender, PaintEventArgs e)    //To be invoked through Invalidate when we have graphs
        // that is to be updated.
        {
            if (thesim.simi > 1)
            {
                thetrend[0].addtoplot(utilities.adddimension(
                        new double[] { global.simtimevector[thesim.simi - 2], global.simtimevector[thesim.simi - 1] }),
                        utilities.adddimension(
                        new double[] { thesteamgenerator.watersegments[0].P.simvector[thesim.simi - 2], 
                            thesteamgenerator.watersegments[0].P.simvector[thesim.simi - 1]}),
                        thesim.simi);

                thetrend[1].addtoplot(utilities.adddimension(
                        new double[] { global.simtimevector[thesim.simi - 2], global.simtimevector[thesim.simi - 1] }),
                        utilities.adddimension(
                        new double[] { thesteamgenerator.watersegments[1].P.simvector[thesim.simi - 2], 
                            thesteamgenerator.watersegments[1].P.simvector[thesim.simi - 1]}),
                        thesim.simi);

                thetrend[2].addtoplot(utilities.adddimension(
                        new double[] { global.simtimevector[thesim.simi - 2], global.simtimevector[thesim.simi - 1] }),
                        utilities.adddimension(
                        new double[] { thesteamgenerator.watersegments[2].P.simvector[thesim.simi - 2], 
                            thesteamgenerator.watersegments[2].P.simvector[thesim.simi - 1]}),
                        thesim.simi);

                thetrend[3].addtoplot(utilities.adddimension(
                        new double[] { global.simtimevector[thesim.simi - 2], global.simtimevector[thesim.simi - 1] }),
                        utilities.adddimension(
                        new double[] { thesteamgenerator.gassegments[0].T.simvector[thesim.simi - 2], 
                            thesteamgenerator.gassegments[0].T.simvector[thesim.simi - 1] }),
                        thesim.simi);

                thetrend[4].addtoplot(utilities.adddimension(
                        new double[] { global.simtimevector[thesim.simi - 2], global.simtimevector[thesim.simi - 1] }),
                        utilities.adddimension(
                        new double[] { thesteamgenerator.gassegments[1].T.simvector[thesim.simi - 2], 
                            thesteamgenerator.gassegments[1].T.simvector[thesim.simi - 1] }),
                        thesim.simi);

                thetrend[5].addtoplot(utilities.adddimension(
                        new double[] { global.simtimevector[thesim.simi - 2], global.simtimevector[thesim.simi - 1] }),
                        utilities.adddimension(
                        new double[] { thesteamgenerator.gassegments[2].T.simvector[thesim.simi - 2], 
                            thesteamgenerator.gassegments[2].T.simvector[thesim.simi - 1] }),
                        thesim.simi);

                thetrend[6].addtoplot(utilities.adddimension(
                        new double[] { global.simtimevector[thesim.simi - 2], global.simtimevector[thesim.simi - 1] }),
                        utilities.adddimension(
                        new double[] { thesteamgenerator.gassegments[0].P.simvector[thesim.simi - 2], 
                            thesteamgenerator.gassegments[0].P.simvector[thesim.simi - 1]}),
                        thesim.simi);

                thetrend[7].addtoplot(utilities.adddimension(
                        new double[] { global.simtimevector[thesim.simi - 2], global.simtimevector[thesim.simi - 1] }),
                        utilities.adddimension(
                        new double[] { thesteamgenerator.gassegments[1].P.simvector[thesim.simi - 2], 
                            thesteamgenerator.gassegments[1].P.simvector[thesim.simi - 1]}),
                        thesim.simi);

                thetrend[8].addtoplot(utilities.adddimension(
                        new double[] { global.simtimevector[thesim.simi - 2], global.simtimevector[thesim.simi - 1] }),
                        utilities.adddimension(
                        new double[] { thesteamgenerator.gassegments[2].P.simvector[thesim.simi - 2], 
                            thesteamgenerator.gassegments[2].P.simvector[thesim.simi - 1]}),
                        thesim.simi);

                thetrend[9].addtoplot(utilities.adddimension(
                        new double[] { global.simtimevector[thesim.simi - 2], global.simtimevector[thesim.simi - 1] }),
                        utilities.adddimension(
                        new double[] { thesteamgenerator.Tmavesimvector[0][thesim.simi - 2], 
                            thesteamgenerator.Tmavesimvector[0][thesim.simi - 1] }),
                        thesim.simi);

                thetrend[10].addtoplot(utilities.adddimension(
                        new double[] { global.simtimevector[thesim.simi - 2], global.simtimevector[thesim.simi - 1] }),
                        utilities.adddimension(
                        new double[] { thesteamgenerator.Tmavesimvector[1][thesim.simi - 2], 
                            thesteamgenerator.Tmavesimvector[1][thesim.simi - 1] }),
                        thesim.simi);

                thetrend[11].addtoplot(utilities.adddimension(
                        new double[] { global.simtimevector[thesim.simi - 2], global.simtimevector[thesim.simi - 1] }),
                        utilities.adddimension(
                        new double[] { thesteamgenerator.Tmavesimvector[2][thesim.simi - 2], 
                            thesteamgenerator.Tmavesimvector[2][thesim.simi - 1] }),
                        thesim.simi);

                thetrend[12].addtoplot(utilities.adddimension(
                        new double[] { global.simtimevector[thesim.simi - 2], global.simtimevector[thesim.simi - 1] }),
                        utilities.adddimension(
                        new double[] { thesteamgenerator.Qgmsimvector[0][thesim.simi - 2], thesteamgenerator.Qgmsimvector[0][thesim.simi - 1] }),
                        thesim.simi);

                thetrend[13].addtoplot(utilities.adddimension(
                        new double[] { global.simtimevector[thesim.simi - 2], global.simtimevector[thesim.simi - 1] }),
                        utilities.adddimension(
                        new double[] { thesteamgenerator.Qgmsimvector[1][thesim.simi - 2], thesteamgenerator.Qgmsimvector[1][thesim.simi - 1] }),
                        thesim.simi);

                thetrend[14].addtoplot(utilities.adddimension(
                        new double[] { global.simtimevector[thesim.simi - 2], global.simtimevector[thesim.simi - 1] }),
                        utilities.adddimension(
                        new double[] { thesteamgenerator.Qgmsimvector[2][thesim.simi - 2], thesteamgenerator.Qgmsimvector[2][thesim.simi - 1] }),
                        thesim.simi);

                thetrend[15].addtoplot(utilities.adddimension(
                        new double[] { global.simtimevector[thesim.simi - 2], global.simtimevector[thesim.simi - 1] }),
                        utilities.adddimension(
                        new double[] { thesteamgenerator.Qmssimvector[0][thesim.simi - 2], thesteamgenerator.Qmssimvector[0][thesim.simi - 1] }),
                        thesim.simi);

                thetrend[16].addtoplot(utilities.adddimension(
                        new double[] { global.simtimevector[thesim.simi - 2], global.simtimevector[thesim.simi - 1] }),
                        utilities.adddimension(
                        new double[] { thesteamgenerator.Qmssimvector[1][thesim.simi - 2], thesteamgenerator.Qmssimvector[1][thesim.simi - 1] }),
                        thesim.simi);

                thetrend[17].addtoplot(utilities.adddimension(
                        new double[] { global.simtimevector[thesim.simi - 2], global.simtimevector[thesim.simi - 1] }),
                        utilities.adddimension(
                        new double[] { thesteamgenerator.Qmssimvector[2][thesim.simi - 2], thesteamgenerator.Qmssimvector[2][thesim.simi - 1] }),
                        thesim.simi);

                thetrend[18].addtoplot(utilities.adddimension(
                        new double[] { global.simtimevector[thesim.simi - 2], global.simtimevector[thesim.simi - 1] }),
                        utilities.adddimension(
                        new double[] { thesteamgenerator.watersegments[0].T.simvector[thesim.simi - 2], 
                            thesteamgenerator.watersegments[0].T.simvector[thesim.simi - 1] }),
                        thesim.simi);

                thetrend[19].addtoplot(utilities.adddimension(
                        new double[] { global.simtimevector[thesim.simi - 2], global.simtimevector[thesim.simi - 1] }),
                        utilities.adddimension(
                        new double[] { thesteamgenerator.watersegments[1].T.simvector[thesim.simi - 2], 
                            thesteamgenerator.watersegments[1].T.simvector[thesim.simi - 1] }),
                        thesim.simi);

                thetrend[20].addtoplot(utilities.adddimension(
                        new double[] { global.simtimevector[thesim.simi - 2], global.simtimevector[thesim.simi - 1] }),
                        utilities.adddimension(
                        new double[] { thesteamgenerator.watersegments[2].T.simvector[thesim.simi - 2], 
                            thesteamgenerator.watersegments[2].T.simvector[thesim.simi - 1] }),
                        thesim.simi);

                thetrend[21].addtoplot(utilities.adddimension(
                        new double[] { global.simtimevector[thesim.simi - 2], global.simtimevector[thesim.simi - 1] }),
                        utilities.adddimension(
                        new double[] { thesteamgenerator.watersegments[0].U.simvector[thesim.simi - 2], 
                            thesteamgenerator.watersegments[0].U.simvector[thesim.simi - 1] }),
                        thesim.simi);

                thetrend[22].addtoplot(utilities.adddimension(
                        new double[] { global.simtimevector[thesim.simi - 2], global.simtimevector[thesim.simi - 1] }),
                        utilities.adddimension(
                        new double[] { thesteamgenerator.watersegments[1].U.simvector[thesim.simi - 2], 
                            thesteamgenerator.watersegments[1].U.simvector[thesim.simi - 1] }),
                        thesim.simi);

                thetrend[23].addtoplot(utilities.adddimension(
                        new double[] { global.simtimevector[thesim.simi - 2], global.simtimevector[thesim.simi - 1] }),
                        utilities.adddimension(
                        new double[] { thesteamgenerator.watersegments[2].U.simvector[thesim.simi - 2], 
                            thesteamgenerator.watersegments[2].U.simvector[thesim.simi - 1] }),
                        thesim.simi);

                thetrend[24].addtoplot(utilities.adddimension(
                        new double[] { global.simtimevector[thesim.simi - 2], global.simtimevector[thesim.simi - 1] }),
                        utilities.adddimension(
                        new double[] { thesteamgenerator.watersegments[0].f.simvector[thesim.simi - 2], 
                            thesteamgenerator.watersegments[0].f.simvector[thesim.simi - 1] }),
                        thesim.simi);

                thetrend[25].addtoplot(utilities.adddimension(
                        new double[] { global.simtimevector[thesim.simi - 2], global.simtimevector[thesim.simi - 1] }),
                        utilities.adddimension(
                        new double[] { thesteamgenerator.watersegments[1].f.simvector[thesim.simi - 2], 
                            thesteamgenerator.watersegments[1].f.simvector[thesim.simi - 1] }),
                        thesim.simi);

                thetrend[26].addtoplot(utilities.adddimension(
                        new double[] { global.simtimevector[thesim.simi - 2], global.simtimevector[thesim.simi - 1] }),
                        utilities.adddimension(
                        new double[] { thesteamgenerator.watersegments[2].f.simvector[thesim.simi - 2], 
                            thesteamgenerator.watersegments[2].f.simvector[thesim.simi - 1] }),
                        thesim.simi);

            }
        }

        private void steamgeneratordetail_Paint(object sender, PaintEventArgs e)
        {
            if (thesim.simi > 1)
            {
                thetrend[0].addtoplot(utilities.adddimension(
                        new double[] { global.simtimevector[thesim.simi - 2], global.simtimevector[thesim.simi - 1] }),
                        utilities.adddimension(
                        new double[] { thesteamgenerator.watersegments[0].P.simvector[thesim.simi - 2], 
                            thesteamgenerator.watersegments[0].P.simvector[thesim.simi - 1]}),
                        thesim.simi);

                thetrend[1].addtoplot(utilities.adddimension(
                        new double[] { global.simtimevector[thesim.simi - 2], global.simtimevector[thesim.simi - 1] }),
                        utilities.adddimension(
                        new double[] { thesteamgenerator.watersegments[1].P.simvector[thesim.simi - 2], 
                            thesteamgenerator.watersegments[1].P.simvector[thesim.simi - 1]}),
                        thesim.simi);

                thetrend[2].addtoplot(utilities.adddimension(
                        new double[] { global.simtimevector[thesim.simi - 2], global.simtimevector[thesim.simi - 1] }),
                        utilities.adddimension(
                        new double[] { thesteamgenerator.watersegments[2].P.simvector[thesim.simi - 2], 
                            thesteamgenerator.watersegments[2].P.simvector[thesim.simi - 1]}),
                        thesim.simi);

                thetrend[3].addtoplot(utilities.adddimension(
                        new double[] { global.simtimevector[thesim.simi - 2], global.simtimevector[thesim.simi - 1] }),
                        utilities.adddimension(
                        new double[] { thesteamgenerator.gassegments[0].T.simvector[thesim.simi - 2], 
                            thesteamgenerator.gassegments[0].T.simvector[thesim.simi - 1] }),
                        thesim.simi);

                thetrend[4].addtoplot(utilities.adddimension(
                        new double[] { global.simtimevector[thesim.simi - 2], global.simtimevector[thesim.simi - 1] }),
                        utilities.adddimension(
                        new double[] { thesteamgenerator.gassegments[1].T.simvector[thesim.simi - 2], 
                            thesteamgenerator.gassegments[1].T.simvector[thesim.simi - 1] }),
                        thesim.simi);

                thetrend[5].addtoplot(utilities.adddimension(
                        new double[] { global.simtimevector[thesim.simi - 2], global.simtimevector[thesim.simi - 1] }),
                        utilities.adddimension(
                        new double[] { thesteamgenerator.gassegments[2].T.simvector[thesim.simi - 2], 
                            thesteamgenerator.gassegments[2].T.simvector[thesim.simi - 1] }),
                        thesim.simi);

                thetrend[6].addtoplot(utilities.adddimension(
                        new double[] { global.simtimevector[thesim.simi - 2], global.simtimevector[thesim.simi - 1] }),
                        utilities.adddimension(
                        new double[] { thesteamgenerator.gassegments[0].P.simvector[thesim.simi - 2], 
                            thesteamgenerator.gassegments[0].P.simvector[thesim.simi - 1]}),
                        thesim.simi);

                thetrend[7].addtoplot(utilities.adddimension(
                        new double[] { global.simtimevector[thesim.simi - 2], global.simtimevector[thesim.simi - 1] }),
                        utilities.adddimension(
                        new double[] { thesteamgenerator.gassegments[1].P.simvector[thesim.simi - 2], 
                            thesteamgenerator.gassegments[1].P.simvector[thesim.simi - 1]}),
                        thesim.simi);

                thetrend[8].addtoplot(utilities.adddimension(
                        new double[] { global.simtimevector[thesim.simi - 2], global.simtimevector[thesim.simi - 1] }),
                        utilities.adddimension(
                        new double[] { thesteamgenerator.gassegments[2].P.simvector[thesim.simi - 2], 
                            thesteamgenerator.gassegments[2].P.simvector[thesim.simi - 1]}),
                        thesim.simi);

                thetrend[9].addtoplot(utilities.adddimension(
                        new double[] { global.simtimevector[thesim.simi - 2], global.simtimevector[thesim.simi - 1] }),
                        utilities.adddimension(
                        new double[] { thesteamgenerator.Tmavesimvector[0][thesim.simi - 2], 
                            thesteamgenerator.Tmavesimvector[0][thesim.simi - 1] }),
                        thesim.simi);

                thetrend[10].addtoplot(utilities.adddimension(
                        new double[] { global.simtimevector[thesim.simi - 2], global.simtimevector[thesim.simi - 1] }),
                        utilities.adddimension(
                        new double[] { thesteamgenerator.Tmavesimvector[1][thesim.simi - 2], 
                            thesteamgenerator.Tmavesimvector[1][thesim.simi - 1] }),
                        thesim.simi);

                thetrend[11].addtoplot(utilities.adddimension(
                        new double[] { global.simtimevector[thesim.simi - 2], global.simtimevector[thesim.simi - 1] }),
                        utilities.adddimension(
                        new double[] { thesteamgenerator.Tmavesimvector[2][thesim.simi - 2], 
                            thesteamgenerator.Tmavesimvector[2][thesim.simi - 1] }),
                        thesim.simi);

                thetrend[12].addtoplot(utilities.adddimension(
                        new double[] { global.simtimevector[thesim.simi - 2], global.simtimevector[thesim.simi - 1] }),
                        utilities.adddimension(
                        new double[] { thesteamgenerator.Qgmsimvector[0][thesim.simi - 2], thesteamgenerator.Qgmsimvector[0][thesim.simi - 1] }),
                        thesim.simi);

                thetrend[13].addtoplot(utilities.adddimension(
                        new double[] { global.simtimevector[thesim.simi - 2], global.simtimevector[thesim.simi - 1] }),
                        utilities.adddimension(
                        new double[] { thesteamgenerator.Qgmsimvector[1][thesim.simi - 2], thesteamgenerator.Qgmsimvector[1][thesim.simi - 1] }),
                        thesim.simi);

                thetrend[14].addtoplot(utilities.adddimension(
                        new double[] { global.simtimevector[thesim.simi - 2], global.simtimevector[thesim.simi - 1] }),
                        utilities.adddimension(
                        new double[] { thesteamgenerator.Qgmsimvector[2][thesim.simi - 2], thesteamgenerator.Qgmsimvector[2][thesim.simi - 1] }),
                        thesim.simi);

                thetrend[15].addtoplot(utilities.adddimension(
                        new double[] { global.simtimevector[thesim.simi - 2], global.simtimevector[thesim.simi - 1] }),
                        utilities.adddimension(
                        new double[] { thesteamgenerator.Qmssimvector[0][thesim.simi - 2], thesteamgenerator.Qmssimvector[0][thesim.simi - 1] }),
                        thesim.simi);

                thetrend[16].addtoplot(utilities.adddimension(
                        new double[] { global.simtimevector[thesim.simi - 2], global.simtimevector[thesim.simi - 1] }),
                        utilities.adddimension(
                        new double[] { thesteamgenerator.Qmssimvector[1][thesim.simi - 2], thesteamgenerator.Qmssimvector[1][thesim.simi - 1] }),
                        thesim.simi);

                thetrend[17].addtoplot(utilities.adddimension(
                        new double[] { global.simtimevector[thesim.simi - 2], global.simtimevector[thesim.simi - 1] }),
                        utilities.adddimension(
                        new double[] { thesteamgenerator.Qmssimvector[2][thesim.simi - 2], thesteamgenerator.Qmssimvector[2][thesim.simi - 1] }),
                        thesim.simi);

                thetrend[18].addtoplot(utilities.adddimension(
                        new double[] { global.simtimevector[thesim.simi - 2], global.simtimevector[thesim.simi - 1] }),
                        utilities.adddimension(
                        new double[] { thesteamgenerator.watersegments[0].T.simvector[thesim.simi - 2], 
                            thesteamgenerator.watersegments[0].T.simvector[thesim.simi - 1] }),
                        thesim.simi);

                thetrend[19].addtoplot(utilities.adddimension(
                        new double[] { global.simtimevector[thesim.simi - 2], global.simtimevector[thesim.simi - 1] }),
                        utilities.adddimension(
                        new double[] { thesteamgenerator.watersegments[1].T.simvector[thesim.simi - 2], 
                            thesteamgenerator.watersegments[1].T.simvector[thesim.simi - 1] }),
                        thesim.simi);

                thetrend[20].addtoplot(utilities.adddimension(
                        new double[] { global.simtimevector[thesim.simi - 2], global.simtimevector[thesim.simi - 1] }),
                        utilities.adddimension(
                        new double[] { thesteamgenerator.watersegments[2].T.simvector[thesim.simi - 2], 
                            thesteamgenerator.watersegments[2].T.simvector[thesim.simi - 1] }),
                        thesim.simi);

                thetrend[21].addtoplot(utilities.adddimension(
                        new double[] { global.simtimevector[thesim.simi - 2], global.simtimevector[thesim.simi - 1] }),
                        utilities.adddimension(
                        new double[] { thesteamgenerator.watersegments[0].U.simvector[thesim.simi - 2], 
                            thesteamgenerator.watersegments[0].U.simvector[thesim.simi - 1] }),
                        thesim.simi);

                thetrend[22].addtoplot(utilities.adddimension(
                        new double[] { global.simtimevector[thesim.simi - 2], global.simtimevector[thesim.simi - 1] }),
                        utilities.adddimension(
                        new double[] { thesteamgenerator.watersegments[1].U.simvector[thesim.simi - 2], 
                            thesteamgenerator.watersegments[1].U.simvector[thesim.simi - 1] }),
                        thesim.simi);

                thetrend[23].addtoplot(utilities.adddimension(
                        new double[] { global.simtimevector[thesim.simi - 2], global.simtimevector[thesim.simi - 1] }),
                        utilities.adddimension(
                        new double[] { thesteamgenerator.watersegments[2].U.simvector[thesim.simi - 2], 
                            thesteamgenerator.watersegments[2].U.simvector[thesim.simi - 1] }),
                        thesim.simi);

                thetrend[24].addtoplot(utilities.adddimension(
                        new double[] { global.simtimevector[thesim.simi - 2], global.simtimevector[thesim.simi - 1] }),
                        utilities.adddimension(
                        new double[] { thesteamgenerator.watersegments[0].f.simvector[thesim.simi - 2], 
                            thesteamgenerator.watersegments[0].f.simvector[thesim.simi - 1] }),
                        thesim.simi);

                thetrend[25].addtoplot(utilities.adddimension(
                        new double[] { global.simtimevector[thesim.simi - 2], global.simtimevector[thesim.simi - 1] }),
                        utilities.adddimension(
                        new double[] { thesteamgenerator.watersegments[1].f.simvector[thesim.simi - 2], 
                            thesteamgenerator.watersegments[1].f.simvector[thesim.simi - 1] }),
                        thesim.simi);

                thetrend[26].addtoplot(utilities.adddimension(
                        new double[] { global.simtimevector[thesim.simi - 2], global.simtimevector[thesim.simi - 1] }),
                        utilities.adddimension(
                        new double[] { thesteamgenerator.watersegments[2].f.simvector[thesim.simi - 2], 
                            thesteamgenerator.watersegments[2].f.simvector[thesim.simi - 1] }),
                        thesim.simi);

            }
        }
    }
}
