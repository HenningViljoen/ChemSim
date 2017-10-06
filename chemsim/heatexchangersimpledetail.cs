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
    public partial class heatexchangersimpledetail : Form
    {
        private simulation thesim;
        public heatexchangersimple theheatexchangersimple;
        private trend[] thetrend;

        private const int NrTrends = 30;

        public heatexchangersimpledetail(heatexchangersimple aheatexchangersimple, simulation asim)
        {
            InitializeComponent();

            theheatexchangersimple = aheatexchangersimple;
            thesim = asim;



            thetrend = new trend[NrTrends];

            setuptrends();
        }

        private void allocatememory()
        {
            if (theheatexchangersimple.U.simvector == null) { theheatexchangersimple.U.simvector = new double[global.SimVectorLength]; }

            if (theheatexchangersimple.A.simvector == null) { theheatexchangersimple.A.simvector = new double[global.SimVectorLength]; }

            if (theheatexchangersimple.strm1temptau.simvector == null) { theheatexchangersimple.strm1temptau.simvector = new double[global.SimVectorLength]; }

            if (theheatexchangersimple.strm2temptau.simvector == null) { theheatexchangersimple.strm2temptau.simvector = new double[global.SimVectorLength]; }

            if (theheatexchangersimple.strm2flowcoefficient.simvector == null)
                { theheatexchangersimple.strm2flowcoefficient.simvector = new double[global.SimVectorLength]; }
        }

        private void setuptrends()
        {
            thetrend[0] = new trend(pictureBox1);
            thetrend[0].setuptrend("Strm1 Pressure Out (Pa)", false, false, true,
                  0, global.simtimevector[global.simtimevector.Length - 1], -2, 20000000);

            thetrend[1] = new trend(pictureBox2);
            thetrend[1].setuptrend("Strm2 Pressure Out (Pa)", false, false, true,
                  0, global.simtimevector[global.simtimevector.Length - 1], -2, 20000000);

            thetrend[2] = new trend(pictureBox3);
            thetrend[2].setuptrend("Strm1 Temp Out (K)", false, false, true,
                  0, global.simtimevector[global.simtimevector.Length - 1]);//, -2, 20000000);

            thetrend[3] = new trend(pictureBox4);
            thetrend[3].setuptrend("Strm2 Temp Out (K)", false, false, true,
                  0, global.simtimevector[global.simtimevector.Length - 1]);//, -2, 20000000);

            thetrend[4] = new trend(pictureBox5);
            thetrend[4].setuptrend("Strm1 Mass Flow (kg/s)", false, false, true,
                  0, global.simtimevector[global.simtimevector.Length - 1], -2, 20000000);

            thetrend[5] = new trend(pictureBox6);
            thetrend[5].setuptrend("Strm2 Mass Flow (kg/s)", false, false, true,
                  0, global.simtimevector[global.simtimevector.Length - 1], -2, 20000000);

            thetrend[6] = new trend(pictureBox7);
            thetrend[6].setuptrend("U (W/(m^2*K))", false, false, true,
                  0, global.simtimevector[global.simtimevector.Length - 1], -2, 20000000);

            thetrend[7] = new trend(pictureBox8);
            thetrend[7].setuptrend("strm1temptau (sec))", false, false, true,
                  0, global.simtimevector[global.simtimevector.Length - 1], -2, 20000000);
        }

        private void heatexchangersimpledetail_Paint(object sender, PaintEventArgs e)
        {
            if (thesim.simi > 1)
            {
                thetrend[0].addtoplot(utilities.adddimension(
                        new double[] { global.simtimevector[thesim.simi - 2], global.simtimevector[thesim.simi - 1] }),
                        utilities.adddimension(
                        new double[] { theheatexchangersimple.outflow[0].mat.P.simvector[thesim.simi - 2], 
                            theheatexchangersimple.outflow[0].mat.P.simvector[thesim.simi - 1]}),
                        thesim.simi);

                thetrend[1].addtoplot(utilities.adddimension(
                        new double[] { global.simtimevector[thesim.simi - 2], global.simtimevector[thesim.simi - 1] }),
                        utilities.adddimension(
                        new double[] { theheatexchangersimple.outflow[1].mat.P.simvector[thesim.simi - 2], 
                            theheatexchangersimple.outflow[1].mat.P.simvector[thesim.simi - 1]}),
                        thesim.simi);

                thetrend[2].addtoplot(utilities.adddimension(
                        new double[] { global.simtimevector[thesim.simi - 2], global.simtimevector[thesim.simi - 1] }),
                        utilities.adddimension(
                        new double[] { theheatexchangersimple.outflow[0].mat.T.simvector[thesim.simi - 2], 
                            theheatexchangersimple.outflow[0].mat.T.simvector[thesim.simi - 1]}),
                        thesim.simi);

                thetrend[3].addtoplot(utilities.adddimension(
                        new double[] { global.simtimevector[thesim.simi - 2], global.simtimevector[thesim.simi - 1] }),
                        utilities.adddimension(
                        new double[] { theheatexchangersimple.outflow[1].mat.T.simvector[thesim.simi - 2], 
                            theheatexchangersimple.outflow[1].mat.T.simvector[thesim.simi - 1]}),
                        thesim.simi);

                thetrend[4].addtoplot(utilities.adddimension(
                        new double[] { global.simtimevector[thesim.simi - 2], global.simtimevector[thesim.simi - 1] }),
                        utilities.adddimension(
                        new double[] { theheatexchangersimple.inflow[0].massflow.simvector[thesim.simi - 2], 
                            theheatexchangersimple.inflow[0].massflow.simvector[thesim.simi - 1]}),
                        thesim.simi);

                thetrend[5].addtoplot(utilities.adddimension(
                        new double[] { global.simtimevector[thesim.simi - 2], global.simtimevector[thesim.simi - 1] }),
                        utilities.adddimension(
                        new double[] { theheatexchangersimple.inflow[1].massflow.simvector[thesim.simi - 2], 
                            theheatexchangersimple.inflow[1].massflow.simvector[thesim.simi - 1]}),
                        thesim.simi);

                thetrend[6].addtoplot(utilities.adddimension(
                        new double[] { global.simtimevector[thesim.simi - 2], global.simtimevector[thesim.simi - 1] }),
                        utilities.adddimension(
                        new double[] { theheatexchangersimple.U.simvector[thesim.simi - 2], 
                            theheatexchangersimple.U.simvector[thesim.simi - 1]}),
                        thesim.simi);

                thetrend[7].addtoplot(utilities.adddimension(
                        new double[] { global.simtimevector[thesim.simi - 2], global.simtimevector[thesim.simi - 1] }),
                        utilities.adddimension(
                        new double[] { theheatexchangersimple.strm1temptau.simvector[thesim.simi - 2], 
                            theheatexchangersimple.strm1temptau.simvector[thesim.simi - 1]}),
                        thesim.simi);
                
            }
        }
    }
}
