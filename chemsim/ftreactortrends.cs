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
    public partial class ftreactortrends : Form
    {
        public ftreactor theftreactor;
        public simulation sim;

        public ftreactortrends(ftreactor aftreactor, simulation asim)
        {
            InitializeComponent();

            theftreactor = aftreactor;
            sim = asim;

            trend t1 = new trend(pictureBox1);
            t1.plot(utilities.adddimension(global.simtimevector), utilities.adddimension(theftreactor.catinreactor),
                "Catalyst in reactor over time (tonne)", false, false, true);
            trend t2 = new trend(pictureBox2);
            t2.plot(theftreactor.timevector2d, theftreactor.catbatweightnewcum, "Fresh catalyst in reactor - weight per batch cumulative (tonne)",
                false, false, true);
            trend t3 = new trend(pictureBox3);
            t3.plot(theftreactor.timevector2d, theftreactor.catbatweightregencum, "Regen catalyst in reactor - weight per batch cumulative (tonne)",
                false, false, true);
            trend t4 = new trend(pictureBox4);
            t4.plot(theftreactor.timevector2d, theftreactor.catbatactnewcum, "Fresh Catalyst batches productivity over time cumulative (Tonnes product per day per tonne catalyst)",
                false, false, true);
            trend t5 = new trend(pictureBox5);
            t5.plot(theftreactor.timevector2d, theftreactor.catbatactregencum, "Regen Catalyst batches productivity over time cumulative (Tonnes product per day per tonne catalyst)",
                false, false, true);
            trend t6 = new trend(pictureBox6);
            t6.plot(theftreactor.timevector2d, theftreactor.catbatweightnew, "Fresh catalyst in reactor - weight per batch (tonne)",
                false, false, true);
            trend t7 = new trend(pictureBox7);
            t7.plot(theftreactor.timevector2d, theftreactor.catbatweightregen, "Regen catalyst in reactor - weight per batch (tonne)",
                false, false, true);
            trend t8 = new trend(pictureBox8);
            t8.plot(theftreactor.timevector2d, theftreactor.catbatweightnewperc, "Fresh catalyst in reactor - weight per batch as percentage of total (%)",
                false, false, true);
            trend t9 = new trend(pictureBox9);
            t9.plot(theftreactor.timevector2d, theftreactor.catbatweightregenperc,
                "Regen Catalyst in reactor - weight per batch as percentage of total", false, false, true);
            trend t10 = new trend(pictureBox10);
            t10.plot(theftreactor.timevector2d, theftreactor.catbatactnew, "Fresh catalyst batches productivity over time (tonne product per day per tonne catalyst)",
                false, false, true);
            trend t11 = new trend(pictureBox11);
            t11.plot(theftreactor.timevector2d, theftreactor.catbatactregen, "Regen catalyst batches productivity over time (tonne product per day per tonne catalyst)",
                false, false, true);
            trend t12 = new trend(pictureBox12);
            t12.plot(utilities.adddimension(global.simtimevector), utilities.adddimension(theftreactor.cattakeout),
                "Catalyst taken out of the reactor over time (tonne per month)",
                false, false, true);
            trend t13 = new trend(pictureBox13);
            t13.plot(utilities.adddimension(global.simtimevector), utilities.adddimension(theftreactor.catregen),
                "Catalyst regenerated and put back in reactor over time (tonne per month)",
                false, false, true);
            trend t14 = new trend(pictureBox14);
            t14.plot(utilities.adddimension(global.simtimevector), utilities.adddimension(theftreactor.cattakeoutinventory),
                "Inventory catalyst taken out but not regenerated over time (tonne)",
                false, false, true);
            trend t15 = new trend(pictureBox15);
            t15.plot(utilities.adddimension(global.simtimevector), utilities.adddimension(theftreactor.catactoverall),
                "Overall Catalyst productivity over time (tonne product per day per tonne catalyst",
                false, false, true);
            trend t16 = new trend(pictureBox16);
            t16.plot(utilities.adddimension(global.simtimevector), utilities.adddimension(theftreactor.reactorprod),
                "Reactor production over time (tonne product per day per reactor)",
                false, false, true);
            trend t17 = new trend(pictureBox17);
            t17.plot(utilities.adddimension(global.simtimevector), utilities.adddimension(theftreactor.freshcatloading),
                "Fresh catalyst loaded over time (tonne per month)",
                false, false, true);
            trend t18 = new trend(pictureBox18);
            t18.plot(utilities.adddimension(global.simtimevector), utilities.adddimension(theftreactor.regenhist),
                "Probability distribution function for number of regeneration cycles of batches",
                false, false, true);
            trend t19 = new trend(pictureBox19);
            t19.plot(utilities.adddimension(global.simtimevector), utilities.adddimension(theftreactor.cummregendist),
                "Cumulative probability distribution function for number of regeneration cycles of batches",
                false, false, true);
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {

        }
    }
}
