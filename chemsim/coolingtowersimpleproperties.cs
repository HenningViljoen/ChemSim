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
    public partial class coolingtowersimpleproperties : Form
    {
        private coolingtowersimple thecoolingtowersimple;
        private simulation thesim;

        public coolingtowersimpleproperties(coolingtowersimple acoolingtowersimple, simulation asim)
        {
            InitializeComponent();

            thecoolingtowersimple = acoolingtowersimple;
            thesim = asim;
            Text = "CoolingTowerSimple Properties - " + thecoolingtowersimple.name;
            refreshdialogue();
            if (thecoolingtowersimple.inflow[0] != null) { listView1.Items.Add(thecoolingtowersimple.inflow[0].nr.ToString()); }
            if (thecoolingtowersimple.outflow[0] != null) { listView2.Items.Add(thecoolingtowersimple.outflow[0].nr.ToString()); }
        }

        private void refreshdialogue()
        {
            textBox16.Text = thecoolingtowersimple.name;
            textBox1.Text = thecoolingtowersimple.tuningfactor.ToString("G5");

        }

        private void readthedialogue()
        {
            thecoolingtowersimple.name = textBox16.Text;
            thecoolingtowersimple.tuningfactor = Convert.ToDouble(textBox1.Text);
        }

        private void button1_Click(object sender, EventArgs e) //OK button
        {
            readthedialogue();
            Hide();
        }

        private void button3_Click(object sender, EventArgs e) //Update button
        {
            readthedialogue();
        }

        private void button4_Click(object sender, EventArgs e) //Cancel button
        {
            Hide();
        }


    }
}
