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
    public partial class heatexchangerdetail : Form
    {
        private simulation thesim;
        private heatexchanger theheatexchanger;
        private trend[] thetrend;

        private const int NrTrends = 30;

        public heatexchangerdetail(heatexchanger aheatexchanger, simulation asim)
        {
            InitializeComponent();

            theheatexchanger = aheatexchanger;
            thesim = asim;
            thetrend = new trend[NrTrends];

            setuptrends();
        }

        private void setuptrends()
        {
            thetrend[0] = new trend(pictureBox1);
            thetrend[0].setuptrend("Strm2 P[0] (Pascal)", false, false, true,
                  0, global.simtimevector[global.simtimevector.Length - 1], -2, 20000000);

            thetrend[1] = new trend(pictureBox2);
            thetrend[1].setuptrend("Strm2 P[1] (Pascal)", false, false, true,
                  0, global.simtimevector[global.simtimevector.Length - 1], -2, 20000000);

            thetrend[2] = new trend(pictureBox3);
            thetrend[2].setuptrend("Strm2 P[2] (Pascal)", false, false, true,
                  0, global.simtimevector[global.simtimevector.Length - 1], -2, 20000000);

            thetrend[3] = new trend(pictureBox4);
            thetrend[3].setuptrend("Strm1 T[0] (K)", false, false, true,
                  0, global.simtimevector[global.simtimevector.Length - 1]);//, -2, 1000);

            thetrend[4] = new trend(pictureBox5);
            thetrend[4].setuptrend("Strm1 T[1] (K)", false, false, true,
                  0, global.simtimevector[global.simtimevector.Length - 1]);//, -2, 1000);

            thetrend[5] = new trend(pictureBox6);
            thetrend[5].setuptrend("Strm1 T[2] (K)", false, false, true,
                  0, global.simtimevector[global.simtimevector.Length - 1]);//, -2, 1000);

            thetrend[6] = new trend(pictureBox7);
            thetrend[6].setuptrend("Strm1 P[0] (Pa)", false, false, true,
                  0, global.simtimevector[global.simtimevector.Length - 1], -2, 200);

            thetrend[7] = new trend(pictureBox8);
            thetrend[7].setuptrend("Strm1 P[1] (Pa)", false, false, true,
                  0, global.simtimevector[global.simtimevector.Length - 1], -2, 200);

            thetrend[8] = new trend(pictureBox9);
            thetrend[8].setuptrend("Strm1 P[2] (Pa)", false, false, true,
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
            thetrend[12].setuptrend("Qstrm1m[0] (W)", false, false, true,
                  0, global.simtimevector[global.simtimevector.Length - 1], -2, 1000000);

            thetrend[13] = new trend(pictureBox14);
            thetrend[13].setuptrend("Qstrm1m[1] (W)", false, false, true,
                  0, global.simtimevector[global.simtimevector.Length - 1], -2, 1000000);

            thetrend[14] = new trend(pictureBox15);
            thetrend[14].setuptrend("Qstrm1m[2] (W)", false, false, true,
                  0, global.simtimevector[global.simtimevector.Length - 1], -2, 1000000);

            thetrend[15] = new trend(pictureBox16);
            thetrend[15].setuptrend("Qmstrm2[0] (W)", false, false, true,
                  0, global.simtimevector[global.simtimevector.Length - 1], -2, 1000000);

            thetrend[16] = new trend(pictureBox17);
            thetrend[16].setuptrend("Qmstrm2[1] (W)", false, false, true,
                  0, global.simtimevector[global.simtimevector.Length - 1], -2, 1000000);

            thetrend[17] = new trend(pictureBox18);
            thetrend[17].setuptrend("Qmstrm2[2] (W)", false, false, true,
                  0, global.simtimevector[global.simtimevector.Length - 1], -2, 1000000);

            thetrend[18] = new trend(pictureBox19);
            thetrend[18].setuptrend("Strm2 T[0] (K)", false, false, true,
                  0, global.simtimevector[global.simtimevector.Length - 1], -2, 1000);

            thetrend[19] = new trend(pictureBox20);
            thetrend[19].setuptrend("Strm2 T[1] (K)", false, false, true,
                  0, global.simtimevector[global.simtimevector.Length - 1], -2, 1000);

            thetrend[20] = new trend(pictureBox21);
            thetrend[20].setuptrend("Strm2 T[2] (K)", false, false, true,
                  0, global.simtimevector[global.simtimevector.Length - 1], -2, 1200);

            thetrend[21] = new trend(pictureBox22);
            thetrend[21].setuptrend("Strm2 U[0] (J)", false, false, true,
                  0, global.simtimevector[global.simtimevector.Length - 1], -2, 1000000);

            thetrend[22] = new trend(pictureBox23);
            thetrend[22].setuptrend("Strm2 U[1] (J)", false, false, true,
                  0, global.simtimevector[global.simtimevector.Length - 1], -2, 1000000);

            thetrend[23] = new trend(pictureBox24);
            thetrend[23].setuptrend("Strm2 U[2] (J)", false, false, true,
                  0, global.simtimevector[global.simtimevector.Length - 1], -2, 1000000);

            thetrend[24] = new trend(pictureBox25);
            thetrend[24].setuptrend("Strm2 f[0]", false, false, true,
                  0, global.simtimevector[global.simtimevector.Length - 1], -2, 2000);

            thetrend[25] = new trend(pictureBox26);
            thetrend[25].setuptrend("Strm2 f[1]", false, false, true,
                  0, global.simtimevector[global.simtimevector.Length - 1], -2, 2000);

            thetrend[26] = new trend(pictureBox27);
            thetrend[26].setuptrend("Strm2 f[2]", false, false, true,
                  0, global.simtimevector[global.simtimevector.Length - 1], -2, 2000);

            thetrend[27] = new trend(pictureBox28);
            thetrend[27].setuptrend("dnstrm2boundarydt[0] (mol/s)", false, false, true,
                  0, global.simtimevector[global.simtimevector.Length - 1], -2, 2000);

            thetrend[28] = new trend(pictureBox29);
            thetrend[28].setuptrend("dnstrm2boundarydt[1] (mol/s)", false, false, true,
                  0, global.simtimevector[global.simtimevector.Length - 1], -2, 2000);

            thetrend[29] = new trend(pictureBox30);
            thetrend[29].setuptrend("dnstrm2boundarydt[2] (mol/s)", false, false, true,
                  0, global.simtimevector[global.simtimevector.Length - 1], -2, 2000);
        }

        private void heatexchangerdetail_Paint(object sender, PaintEventArgs e)
        {
            if (thesim.simi > 1)
            {
                thetrend[0].addtoplot(utilities.adddimension(
                        new double[] { global.simtimevector[thesim.simi - 2], global.simtimevector[thesim.simi - 1] }),
                        utilities.adddimension(
                        new double[] { theheatexchanger.strm2segments[0].P.simvector[thesim.simi - 2], 
                            theheatexchanger.strm2segments[0].P.simvector[thesim.simi - 1]}),
                        thesim.simi);

                thetrend[1].addtoplot(utilities.adddimension(
                        new double[] { global.simtimevector[thesim.simi - 2], global.simtimevector[thesim.simi - 1] }),
                        utilities.adddimension(
                        new double[] { theheatexchanger.strm2segments[1].P.simvector[thesim.simi - 2], 
                            theheatexchanger.strm2segments[1].P.simvector[thesim.simi - 1]}),
                        thesim.simi);

                thetrend[2].addtoplot(utilities.adddimension(
                        new double[] { global.simtimevector[thesim.simi - 2], global.simtimevector[thesim.simi - 1] }),
                        utilities.adddimension(
                        new double[] { theheatexchanger.strm2segments[2].P.simvector[thesim.simi - 2], 
                            theheatexchanger.strm2segments[2].P.simvector[thesim.simi - 1]}),
                        thesim.simi);

                thetrend[3].addtoplot(utilities.adddimension(
                        new double[] { global.simtimevector[thesim.simi - 2], global.simtimevector[thesim.simi - 1] }),
                        utilities.adddimension(
                        new double[] { theheatexchanger.strm1segments[0].T.simvector[thesim.simi - 2], 
                            theheatexchanger.strm1segments[0].T.simvector[thesim.simi - 1] }),
                        thesim.simi);

                thetrend[4].addtoplot(utilities.adddimension(
                        new double[] { global.simtimevector[thesim.simi - 2], global.simtimevector[thesim.simi - 1] }),
                        utilities.adddimension(
                        new double[] { theheatexchanger.strm1segments[1].T.simvector[thesim.simi - 2], 
                            theheatexchanger.strm1segments[1].T.simvector[thesim.simi - 1] }),
                        thesim.simi);

                thetrend[5].addtoplot(utilities.adddimension(
                        new double[] { global.simtimevector[thesim.simi - 2], global.simtimevector[thesim.simi - 1] }),
                        utilities.adddimension(
                        new double[] { theheatexchanger.strm1segments[2].T.simvector[thesim.simi - 2], 
                            theheatexchanger.strm1segments[2].T.simvector[thesim.simi - 1] }),
                        thesim.simi);

                thetrend[6].addtoplot(utilities.adddimension(
                        new double[] { global.simtimevector[thesim.simi - 2], global.simtimevector[thesim.simi - 1] }),
                        utilities.adddimension(
                        new double[] { theheatexchanger.strm1segments[0].P.simvector[thesim.simi - 2], 
                            theheatexchanger.strm1segments[0].P.simvector[thesim.simi - 1]}),
                        thesim.simi);

                thetrend[7].addtoplot(utilities.adddimension(
                        new double[] { global.simtimevector[thesim.simi - 2], global.simtimevector[thesim.simi - 1] }),
                        utilities.adddimension(
                        new double[] { theheatexchanger.strm1segments[1].P.simvector[thesim.simi - 2], 
                            theheatexchanger.strm1segments[1].P.simvector[thesim.simi - 1]}),
                        thesim.simi);

                thetrend[8].addtoplot(utilities.adddimension(
                        new double[] { global.simtimevector[thesim.simi - 2], global.simtimevector[thesim.simi - 1] }),
                        utilities.adddimension(
                        new double[] { theheatexchanger.strm1segments[2].P.simvector[thesim.simi - 2], 
                            theheatexchanger.strm1segments[2].P.simvector[thesim.simi - 1]}),
                        thesim.simi);

                thetrend[9].addtoplot(utilities.adddimension(
                        new double[] { global.simtimevector[thesim.simi - 2], global.simtimevector[thesim.simi - 1] }),
                        utilities.adddimension(
                        new double[] { theheatexchanger.Tmavesimvector[0][thesim.simi - 2], 
                            theheatexchanger.Tmavesimvector[0][thesim.simi - 1] }),
                        thesim.simi);

                thetrend[10].addtoplot(utilities.adddimension(
                        new double[] { global.simtimevector[thesim.simi - 2], global.simtimevector[thesim.simi - 1] }),
                        utilities.adddimension(
                        new double[] { theheatexchanger.Tmavesimvector[1][thesim.simi - 2], 
                            theheatexchanger.Tmavesimvector[1][thesim.simi - 1] }),
                        thesim.simi);

                thetrend[11].addtoplot(utilities.adddimension(
                        new double[] { global.simtimevector[thesim.simi - 2], global.simtimevector[thesim.simi - 1] }),
                        utilities.adddimension(
                        new double[] { theheatexchanger.Tmavesimvector[2][thesim.simi - 2], 
                            theheatexchanger.Tmavesimvector[2][thesim.simi - 1] }),
                        thesim.simi);

                thetrend[12].addtoplot(utilities.adddimension(
                        new double[] { global.simtimevector[thesim.simi - 2], global.simtimevector[thesim.simi - 1] }),
                        utilities.adddimension(
                        new double[] { theheatexchanger.Qstrm1msimvector[0][thesim.simi - 2], theheatexchanger.Qstrm1msimvector[0][thesim.simi - 1] }),
                        thesim.simi);

                thetrend[13].addtoplot(utilities.adddimension(
                        new double[] { global.simtimevector[thesim.simi - 2], global.simtimevector[thesim.simi - 1] }),
                        utilities.adddimension(
                        new double[] { theheatexchanger.Qstrm1msimvector[1][thesim.simi - 2], theheatexchanger.Qstrm1msimvector[1][thesim.simi - 1] }),
                        thesim.simi);

                thetrend[14].addtoplot(utilities.adddimension(
                        new double[] { global.simtimevector[thesim.simi - 2], global.simtimevector[thesim.simi - 1] }),
                        utilities.adddimension(
                        new double[] { theheatexchanger.Qstrm1msimvector[2][thesim.simi - 2], theheatexchanger.Qstrm1msimvector[2][thesim.simi - 1] }),
                        thesim.simi);

                thetrend[15].addtoplot(utilities.adddimension(
                        new double[] { global.simtimevector[thesim.simi - 2], global.simtimevector[thesim.simi - 1] }),
                        utilities.adddimension(
                        new double[] { theheatexchanger.Qmstrm2simvector[0][thesim.simi - 2], theheatexchanger.Qmstrm2simvector[0][thesim.simi - 1] }),
                        thesim.simi);

                thetrend[16].addtoplot(utilities.adddimension(
                        new double[] { global.simtimevector[thesim.simi - 2], global.simtimevector[thesim.simi - 1] }),
                        utilities.adddimension(
                        new double[] { theheatexchanger.Qmstrm2simvector[1][thesim.simi - 2], theheatexchanger.Qmstrm2simvector[1][thesim.simi - 1] }),
                        thesim.simi);

                thetrend[17].addtoplot(utilities.adddimension(
                        new double[] { global.simtimevector[thesim.simi - 2], global.simtimevector[thesim.simi - 1] }),
                        utilities.adddimension(
                        new double[] { theheatexchanger.Qmstrm2simvector[2][thesim.simi - 2], theheatexchanger.Qmstrm2simvector[2][thesim.simi - 1] }),
                        thesim.simi);

                thetrend[18].addtoplot(utilities.adddimension(
                        new double[] { global.simtimevector[thesim.simi - 2], global.simtimevector[thesim.simi - 1] }),
                        utilities.adddimension(
                        new double[] { theheatexchanger.strm2segments[0].T.simvector[thesim.simi - 2], 
                            theheatexchanger.strm2segments[0].T.simvector[thesim.simi - 1] }),
                        thesim.simi);

                thetrend[19].addtoplot(utilities.adddimension(
                        new double[] { global.simtimevector[thesim.simi - 2], global.simtimevector[thesim.simi - 1] }),
                        utilities.adddimension(
                        new double[] { theheatexchanger.strm2segments[1].T.simvector[thesim.simi - 2], 
                            theheatexchanger.strm2segments[1].T.simvector[thesim.simi - 1] }),
                        thesim.simi);

                thetrend[20].addtoplot(utilities.adddimension(
                        new double[] { global.simtimevector[thesim.simi - 2], global.simtimevector[thesim.simi - 1] }),
                        utilities.adddimension(
                        new double[] { theheatexchanger.strm2segments[2].T.simvector[thesim.simi - 2], 
                            theheatexchanger.strm2segments[2].T.simvector[thesim.simi - 1] }),
                        thesim.simi);

                thetrend[21].addtoplot(utilities.adddimension(
                        new double[] { global.simtimevector[thesim.simi - 2], global.simtimevector[thesim.simi - 1] }),
                        utilities.adddimension(
                        new double[] { theheatexchanger.strm2segments[0].U.simvector[thesim.simi - 2], 
                            theheatexchanger.strm2segments[0].U.simvector[thesim.simi - 1] }),
                        thesim.simi);

                thetrend[22].addtoplot(utilities.adddimension(
                        new double[] { global.simtimevector[thesim.simi - 2], global.simtimevector[thesim.simi - 1] }),
                        utilities.adddimension(
                        new double[] { theheatexchanger.strm2segments[1].U.simvector[thesim.simi - 2], 
                            theheatexchanger.strm2segments[1].U.simvector[thesim.simi - 1] }),
                        thesim.simi);

                thetrend[23].addtoplot(utilities.adddimension(
                        new double[] { global.simtimevector[thesim.simi - 2], global.simtimevector[thesim.simi - 1] }),
                        utilities.adddimension(
                        new double[] { theheatexchanger.strm2segments[2].U.simvector[thesim.simi - 2], 
                            theheatexchanger.strm2segments[2].U.simvector[thesim.simi - 1] }),
                        thesim.simi);

                thetrend[24].addtoplot(utilities.adddimension(
                        new double[] { global.simtimevector[thesim.simi - 2], global.simtimevector[thesim.simi - 1] }),
                        utilities.adddimension(
                        new double[] { theheatexchanger.strm2segments[0].f.simvector[thesim.simi - 2], 
                            theheatexchanger.strm2segments[0].f.simvector[thesim.simi - 1] }),
                        thesim.simi);

                thetrend[25].addtoplot(utilities.adddimension(
                        new double[] { global.simtimevector[thesim.simi - 2], global.simtimevector[thesim.simi - 1] }),
                        utilities.adddimension(
                        new double[] { theheatexchanger.strm2segments[1].f.simvector[thesim.simi - 2], 
                            theheatexchanger.strm2segments[1].f.simvector[thesim.simi - 1] }),
                        thesim.simi);

                thetrend[26].addtoplot(utilities.adddimension(
                        new double[] { global.simtimevector[thesim.simi - 2], global.simtimevector[thesim.simi - 1] }),
                        utilities.adddimension(
                        new double[] { theheatexchanger.strm2segments[2].f.simvector[thesim.simi - 2], 
                            theheatexchanger.strm2segments[2].f.simvector[thesim.simi - 1] }),
                        thesim.simi);

                thetrend[27].addtoplot(utilities.adddimension(
                        new double[] { global.simtimevector[thesim.simi - 2], global.simtimevector[thesim.simi - 1] }),
                        utilities.adddimension(
                        new double[] { theheatexchanger.dnstrm2boundarydtsimvector[0][thesim.simi - 2], 
                            theheatexchanger.dnstrm2boundarydtsimvector[0][thesim.simi - 1] }),
                        thesim.simi);

                thetrend[28].addtoplot(utilities.adddimension(
                        new double[] { global.simtimevector[thesim.simi - 2], global.simtimevector[thesim.simi - 1] }),
                        utilities.adddimension(
                        new double[] { theheatexchanger.dnstrm2boundarydtsimvector[1][thesim.simi - 2], 
                            theheatexchanger.dnstrm2boundarydtsimvector[1][thesim.simi - 1] }),
                        thesim.simi);

                thetrend[29].addtoplot(utilities.adddimension(
                        new double[] { global.simtimevector[thesim.simi - 2], global.simtimevector[thesim.simi - 1] }),
                        utilities.adddimension(
                        new double[] { theheatexchanger.dnstrm2boundarydtsimvector[2][thesim.simi - 2], 
                            theheatexchanger.dnstrm2boundarydtsimvector[2][thesim.simi - 1] }),
                        thesim.simi);
            }
        }
    }
}
