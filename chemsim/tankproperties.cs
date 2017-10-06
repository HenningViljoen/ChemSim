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
    public partial class tankproperties : Form
    {
        public simulation thesim;
        public tank thetank;
        public int tanknr;

        public tankproperties(tank atank, simulation asim)
        {
            InitializeComponent();

            thetank = atank;
            thesim = asim;
            Text = ""; // String.Concat("Properties for tank number: ", tanknr.ToString());

            refreshdialogue();


        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e) // Double clicking on the Percentage Inventory variable, for trending.
        {
            trendvar thetrendvar = new trendvar(utilities.adddimension(global.simtimevector),
                utilities.adddimension(thetank.fracinventory.simvector), "Fraction inventory (%)");
            thetrendvar.Show();
        }

        private void label7_Click(object sender, EventArgs e) // Double clicking on the Pressure at bottom variable, for trending.
        {
            trendvar thetrendvar = new trendvar(utilities.adddimension(global.simtimevector),
                utilities.adddimension(utilities.vectorprocessor(thetank.pressureatbottom.simvector, utilities.pascal2barg)),
                "Pressure at bottom (Barg)");
            thetrendvar.Show();
        }

        private void refreshdialogue()
        {
            textBox1.Text = thetank.maxvolume.ToString("N");
            textBox2.Text = thetank.radius.ToString("N");
            textBox3.Text = thetank.height.ToString("N");
            textBox8.Text = thetank.actualvolumeinventory.ToString("N");
            textBox4.Text = thetank.massinventory.ToString("N");
            textBox5.Text = thetank.fracinventory.ToString("N");
            textBox6.Text = thetank.inventoryheight.ToString("N");
            textBox7.Text = utilities.pascal2barg(thetank.pressureatbottom).ToString("N");

        }

        private void readthedialogue()
        {
            thetank.radius = Convert.ToDouble(textBox2.Text);
            thetank.height = Convert.ToDouble(textBox3.Text);
            //thetank.fracinventory.v = Convert.ToDouble(textBox5.Text);
            thetank.massinventory.v = Convert.ToDouble(textBox4.Text);
        }

        private void button1_Click(object sender, EventArgs e) //OK button click
        {
            
        }

        private void button2_Click(object sender, EventArgs e) //Update button click
        {
            
        }

        private void button3_Click(object sender, EventArgs e) //Cancel button click
        {
            Hide();
        }

        private void button1_Click_1(object sender, EventArgs e) //OK button
        {
            readthedialogue();
            thetank.inventorycalcs();
            //thetank.update(thesim.simi, false);
            Hide();
        }

        private void button2_Click_1(object sender, EventArgs e) //Update button click
        {
            readthedialogue();
            thetank.inventorycalcs();
            //thetank.update(thesim.simi, false);
            refreshdialogue();
        }

        private void button3_Click_1(object sender, EventArgs e) //Cancel button click
        {
            Hide();
        }
    }
}
