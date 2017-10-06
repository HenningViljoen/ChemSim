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
    public partial class nmpcdetail : Form
    {
        private simulation thesim;
        private nmpc thenmpc;
        private trend[] thetrend;

        private const int NrTrends = 36;
        private const int NrCVTrends = 18;
        private const int NrMVTrends = 14;
        private const int NrMVBoolTrends = 4;

        public int lastsimitrended; //The last simi value when the trend was last updated.

        public nmpcdetail(nmpc anmpc, simulation asim)
        {
            InitializeComponent();

            thenmpc = anmpc;
            thesim = asim;
            allocatememory();

            thetrend = new trend[NrTrends];

            setuptrends();

            lastsimitrended = 0;
        }

        private void allocatememory()
        {
            for (int i = 0; i < thenmpc.cvmaster.Count;  i++)
            {
                if (thenmpc.cvmaster[i].var.simvector == null) { thenmpc.cvmaster[i].var.simvector = new double[global.SimVectorLength]; }
            }

            for (int i = 0; i < thenmpc.mvmaster.Count; i++)
            {
                if (thenmpc.mvmaster[i].var.simvector == null) { thenmpc.mvmaster[i].var.simvector = new double[global.SimVectorLength]; }
            }

            for (int i = 0; i < thenmpc.mvboolmaster.Count; i++)
            {
                if (thenmpc.mvboolmaster[i].var.simvector == null) { thenmpc.mvboolmaster[i].var.simvector = new double[global.SimVectorLength]; }
            }

        }

        private void setuptrends()
        {


            //CVs
            thetrend[0] = new trend(pictureBox1);
            thetrend[0].setuptrend("CV 0 simvector", false, false, true,
                  0, global.simtimevector[global.simtimevector.Length - 1]);//, -2, 20000000);

            thetrend[1] = new trend(pictureBox2);
            thetrend[1].setuptrend("CV 1 simvector", false, false, true,
                  0, global.simtimevector[global.simtimevector.Length - 1]);//, -2, 20000000);

            thetrend[2] = new trend(pictureBox3);
            thetrend[2].setuptrend("CV 2 simvector", false, false, true,
                  0, global.simtimevector[global.simtimevector.Length - 1]);//, -2, 20000000);

            thetrend[3] = new trend(pictureBox4);
            thetrend[3].setuptrend("CV 3 simvector", false, false, true,
                  0, global.simtimevector[global.simtimevector.Length - 1]);//, -2, 20000000);

            thetrend[4] = new trend(pictureBox5);
            thetrend[4].setuptrend("CV 4 simvector", false, false, true,
                  0, global.simtimevector[global.simtimevector.Length - 1]);//, -2, 20000000);

            thetrend[5] = new trend(pictureBox6);
            thetrend[5].setuptrend("CV 5 simvector", false, false, true,
                  0, global.simtimevector[global.simtimevector.Length - 1]);//, -2, 20000000);

            thetrend[6] = new trend(pictureBox7);
            thetrend[6].setuptrend("CV 6 simvector", false, false, true,
                  0, global.simtimevector[global.simtimevector.Length - 1]);//, -2, 20000000);

            thetrend[7] = new trend(pictureBox8);
            thetrend[7].setuptrend("CV 7 simvector", false, false, true,
                  0, global.simtimevector[global.simtimevector.Length - 1]);//, -2, 20000000);

            thetrend[8] = new trend(pictureBox9);
            thetrend[8].setuptrend("CV 8 simvector", false, false, true,
                  0, global.simtimevector[global.simtimevector.Length - 1]);//, -2, 20000000);

            thetrend[9] = new trend(pictureBox10);
            thetrend[9].setuptrend("CV 9 simvector", false, false, true,
                  0, global.simtimevector[global.simtimevector.Length - 1]);//, -2, 20000000);

            thetrend[10] = new trend(pictureBox11);
            thetrend[10].setuptrend("CV 10 simvector", false, false, true,
                  0, global.simtimevector[global.simtimevector.Length - 1]);//, -2, 20000000);

            thetrend[11] = new trend(pictureBox12);
            thetrend[11].setuptrend("CV 11 simvector", false, false, true,
                  0, global.simtimevector[global.simtimevector.Length - 1]);//, -2, 20000000);

            thetrend[12] = new trend(pictureBox13);
            thetrend[12].setuptrend("CV 12 simvector", false, false, true,
                  0, global.simtimevector[global.simtimevector.Length - 1]);

            thetrend[13] = new trend(pictureBox14);
            thetrend[13].setuptrend("CV 13 simvector", false, false, true,
                  0, global.simtimevector[global.simtimevector.Length - 1]);

            thetrend[14] = new trend(pictureBox15);
            thetrend[14].setuptrend("CV 14 simvector", false, false, true,
                  0, global.simtimevector[global.simtimevector.Length - 1]);

            thetrend[15] = new trend(pictureBox16);
            thetrend[15].setuptrend("CV 15 simvector", false, false, true,
                  0, global.simtimevector[global.simtimevector.Length - 1]);

            thetrend[16] = new trend(pictureBox17);
            thetrend[16].setuptrend("CV 16 simvector", false, false, true,
                  0, global.simtimevector[global.simtimevector.Length - 1]);

            thetrend[17] = new trend(pictureBox18);
            thetrend[17].setuptrend("CV 17 simvector", false, false, true,
                  0, global.simtimevector[global.simtimevector.Length - 1]);


            //MVs
            //public void setuptrend(String atagname, bool amillionsx,
            //            bool amillionsy, bool ashowlabels, double axmin, double axmax,
            //            double aymin = 1000000, double aymax = 1.0)

            thetrend[18] = new trend(pictureBox19);
            thetrend[18].setuptrend("MV 0 simvector", true, false, true,
                  0, global.simtimevector[global.simtimevector.Length - 1], 0, 0);

            thetrend[19] = new trend(pictureBox20);
            thetrend[19].setuptrend("MV 1 simvector", true, false, true,
                  0, global.simtimevector[global.simtimevector.Length - 1], 0, 0);

            thetrend[20] = new trend(pictureBox21);
            thetrend[20].setuptrend("MV 2 simvector", true, false, true,
                  0, global.simtimevector[global.simtimevector.Length - 1], -0.1, 200);

            thetrend[21] = new trend(pictureBox22);
            thetrend[21].setuptrend("MV 3 simvector", true, false, true,
                  0, global.simtimevector[global.simtimevector.Length - 1], -0.1, 200);

            thetrend[22] = new trend(pictureBox23);
            thetrend[22].setuptrend("MV 4 simvector", true, false, true,
                  0, global.simtimevector[global.simtimevector.Length - 1], -0.1, 200);

            thetrend[23] = new trend(pictureBox24);
            thetrend[23].setuptrend("MV 5 simvector", true, false, true,
                  0, global.simtimevector[global.simtimevector.Length - 1], -0.1, 200);

            thetrend[24] = new trend(pictureBox25);
            thetrend[24].setuptrend("MV 6 simvector", true, false, true,
                  0, global.simtimevector[global.simtimevector.Length - 1], -0.1, 200);

            thetrend[25] = new trend(pictureBox26);
            thetrend[25].setuptrend("MV 7 simvector", true, false, true,
                  0, global.simtimevector[global.simtimevector.Length - 1], -0.1, 200);

            thetrend[26] = new trend(pictureBox27);
            thetrend[26].setuptrend("MV 8 simvector", true, false, true,
                  0, global.simtimevector[global.simtimevector.Length - 1], -0.1, 200);

            thetrend[27] = new trend(pictureBox28);
            thetrend[27].setuptrend("MV 9 simvector", true, false, true,
                  0, global.simtimevector[global.simtimevector.Length - 1], -0.1, 200);

            thetrend[28] = new trend(pictureBox29);
            thetrend[28].setuptrend("MV 10 simvector", true, false, true,
                  0, global.simtimevector[global.simtimevector.Length - 1], -0.1, 200);

            thetrend[29] = new trend(pictureBox30);
            thetrend[29].setuptrend("MV 11 simvector", true, false, true,
                  0, global.simtimevector[global.simtimevector.Length - 1], -0.1, 200);

            thetrend[30] = new trend(pictureBox31);
            thetrend[30].setuptrend("MV 12 simvector", true, false, true,
                  0, global.simtimevector[global.simtimevector.Length - 1], -0.1, 200);

            thetrend[31] = new trend(pictureBox32);
            thetrend[31].setuptrend("MV 13 simvector", true, false, true,
                  0, global.simtimevector[global.simtimevector.Length - 1], -0.1, 200);

            thetrend[32] = new trend(pictureBox33);
            thetrend[32].setuptrend("Boolean MV 0 simvector", true, false, true,
                  0, global.simtimevector[global.simtimevector.Length - 1], -0.1, 200);

            thetrend[33] = new trend(pictureBox34);
            thetrend[33].setuptrend("Boolean MV 1 simvector", true, false, true,
                  0, global.simtimevector[global.simtimevector.Length - 1], -0.1, 200);

            thetrend[34] = new trend(pictureBox35);
            thetrend[34].setuptrend("Boolean MV 2 simvector", true, false, true,
                  0, global.simtimevector[global.simtimevector.Length - 1], -0.1, 200);

            thetrend[35] = new trend(pictureBox36);
            thetrend[35].setuptrend("Boolean MV 3 simvector", true, false, true,
                  0, global.simtimevector[global.simtimevector.Length - 1], -0.1, 200);

        }

        private void nmpcdetail_Paint(object sender, PaintEventArgs e)
        {
            if (thesim.simi > 1)
            {
                for (int i = 0; i < thenmpc.cvmaster.Count; i++)
                {
                    thetrend[i].addtoplot(new double[][] {utilities.extractvectorsim(global.simtimevector, lastsimitrended, thesim.simi),
                        utilities.extractvectorsim(global.simtimevector, lastsimitrended, thesim.simi)},

                        new double[][] {utilities.extractvectorsim(thenmpc.cvmaster[i].var.simvector, lastsimitrended ,thesim.simi),
                            utilities.extractvectorsim(thenmpc.cvmaster[i].target.simvector, lastsimitrended,thesim.simi)},
                        thesim.simi);
                    
                    //utilities.extractvector(global.simtimevector, lastsimitrended, thesim.simi)

                }

                int nrmvtrendstouse = (thenmpc.mvmaster.Count > NrMVTrends) ? NrMVTrends : thenmpc.mvmaster.Count;

                for (int i = 0; i < nrmvtrendstouse; i++)
                {
                    thetrend[NrCVTrends + i].addtoplot(utilities.adddimension(
                        utilities.extractvectorsim(global.simtimevector, lastsimitrended, thesim.simi)),
                        utilities.adddimension(
                        utilities.extractvectorsim(thenmpc.mvmaster[i].var.simvector, lastsimitrended, thesim.simi)),
                        thesim.simi);
                }

                int nrmvbooltrendstouse = (thenmpc.mvboolmaster.Count > NrMVBoolTrends) ? NrMVBoolTrends : thenmpc.mvboolmaster.Count;

                for (int i = 0; i < nrmvbooltrendstouse; i++)
                {
                    thetrend[NrCVTrends + NrMVTrends + i].addtoplot(utilities.adddimension(
                        new double[] { global.simtimevector[thesim.simi - 2], global.simtimevector[thesim.simi - 1] }),
                        utilities.adddimension(
                        new double[] { thenmpc.mvboolmaster[i].var.simvector[thesim.simi - 2], 
                            thenmpc.mvboolmaster[i].var.simvector[thesim.simi - 1]}),
                        thesim.simi);
                }

                lastsimitrended = (thesim.simi >= global.DefaultNrIterations - 1) ? 0 : thesim.simi;

            }
        }

        private void pictureBox19_Click(object sender, EventArgs e)
        {

        }

        //MV double click event methods -------------------------------------------------------------------------------------------------------------------------------------------

        private void pictureBox19_DoubleClick(object sender, EventArgs e) //Double click event methods for MV trends for Excel export will be here and below.
        {
            utilities.exporttoexcel(utilities.adddimensionclassic(global.simtimevector), utilities.adddimensionclassic(thenmpc.mvmaster[0].var.simvector), thesim.simi);
        }

        private void pictureBox20_DoubleClick(object sender, EventArgs e)
        {
            utilities.exporttoexcel(utilities.adddimensionclassic(global.simtimevector), utilities.adddimensionclassic(thenmpc.mvmaster[1].var.simvector), thesim.simi);
        }

        private void pictureBox21_DoubleClick(object sender, EventArgs e)
        {
            utilities.exporttoexcel(utilities.adddimensionclassic(global.simtimevector), utilities.adddimensionclassic(thenmpc.mvmaster[2].var.simvector), thesim.simi);
        }

        private void pictureBox22_DoubleClick(object sender, EventArgs e)
        {
            utilities.exporttoexcel(utilities.adddimensionclassic(global.simtimevector), utilities.adddimensionclassic(thenmpc.mvmaster[3].var.simvector), thesim.simi);
        }

        private void pictureBox23_DoubleClick(object sender, EventArgs e)
        {
            utilities.exporttoexcel(utilities.adddimensionclassic(global.simtimevector), utilities.adddimensionclassic(thenmpc.mvmaster[4].var.simvector), thesim.simi);
      
        }

        private void pictureBox24_DoubleClick(object sender, EventArgs e)
        {
            utilities.exporttoexcel(utilities.adddimensionclassic(global.simtimevector), utilities.adddimensionclassic(thenmpc.mvmaster[5].var.simvector), thesim.simi);
      
        }

        private void pictureBox25_DoubleClick(object sender, EventArgs e)
        {
            utilities.exporttoexcel(utilities.adddimensionclassic(global.simtimevector), utilities.adddimensionclassic(thenmpc.mvmaster[6].var.simvector), thesim.simi);
    
        }

        private void pictureBox26_DoubleClick(object sender, EventArgs e)
        {
            utilities.exporttoexcel(utilities.adddimensionclassic(global.simtimevector), utilities.adddimensionclassic(thenmpc.mvmaster[7].var.simvector), thesim.simi);
    
        }

        private void pictureBox27_DoubleClick(object sender, EventArgs e)
        {
            utilities.exporttoexcel(utilities.adddimensionclassic(global.simtimevector), utilities.adddimensionclassic(thenmpc.mvmaster[8].var.simvector), thesim.simi);
    
        }

        private void pictureBox28_DoubleClick(object sender, EventArgs e)
        {
            utilities.exporttoexcel(utilities.adddimensionclassic(global.simtimevector), utilities.adddimensionclassic(thenmpc.mvmaster[9].var.simvector), thesim.simi);
    
        }

        private void pictureBox29_DoubleClick(object sender, EventArgs e)
        {
            utilities.exporttoexcel(utilities.adddimensionclassic(global.simtimevector), utilities.adddimensionclassic(thenmpc.mvmaster[10].var.simvector), thesim.simi);
    
        }

        private void pictureBox30_DoubleClick(object sender, EventArgs e)
        {
            utilities.exporttoexcel(utilities.adddimensionclassic(global.simtimevector), utilities.adddimensionclassic(thenmpc.mvmaster[11].var.simvector), thesim.simi);
    
        }

        private void pictureBox31_DoubleClick(object sender, EventArgs e)
        {
            utilities.exporttoexcel(utilities.adddimensionclassic(global.simtimevector), utilities.adddimensionclassic(thenmpc.mvmaster[12].var.simvector), thesim.simi);
    
        }

        private void pictureBox32_DoubleClick(object sender, EventArgs e)
        {
            utilities.exporttoexcel(utilities.adddimensionclassic(global.simtimevector), utilities.adddimensionclassic(thenmpc.mvmaster[13].var.simvector), thesim.simi);
    
        }

        //Boolean MV double click methods - ----------------------------------------------------------------------------------------------------------------------------------------

        private void pictureBox33_DoubleClick(object sender, EventArgs e)
        {
            utilities.exporttoexcel(utilities.adddimensionclassic(global.simtimevector), utilities.adddimensionclassic(thenmpc.mvboolmaster[0].var.simvector), thesim.simi);
        }

        private void pictureBox34_DoubleClick(object sender, EventArgs e)
        {
            utilities.exporttoexcel(utilities.adddimensionclassic(global.simtimevector), utilities.adddimensionclassic(thenmpc.mvboolmaster[1].var.simvector), thesim.simi);
        }

        private void pictureBox35_DoubleClick(object sender, EventArgs e)
        {
            utilities.exporttoexcel(utilities.adddimensionclassic(global.simtimevector), utilities.adddimensionclassic(thenmpc.mvboolmaster[2].var.simvector), thesim.simi);
        }

        private void pictureBox36_DoubleClick(object sender, EventArgs e)
        {
            utilities.exporttoexcel(utilities.adddimensionclassic(global.simtimevector), utilities.adddimensionclassic(thenmpc.mvboolmaster[3].var.simvector), thesim.simi);
        }

        

        

        

        

        

        

        

        

        

        




    }
}
