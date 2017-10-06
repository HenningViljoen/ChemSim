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
    public partial class coolingtowerheatexchangersimpleproperties : Form
    {
        private coolingtowerheatexchangersimple thecoolingtowerheatexchangersimple;
        private simulation thesim;

        public coolingtowerheatexchangersimpleproperties(coolingtowerheatexchangersimple acoolingtowerheatexchangersimple, simulation asim)
        {
            InitializeComponent();

            thecoolingtowerheatexchangersimple = acoolingtowerheatexchangersimple;
            thesim = asim;
            Text = "HeatexchangerSimple Properties - " + thecoolingtowerheatexchangersimple.name;
            refreshdialogue();
            if (thecoolingtowerheatexchangersimple.inflow[0] != null) { listView1.Items.Add(thecoolingtowerheatexchangersimple.inflow[0].nr.ToString()); }
            if (thecoolingtowerheatexchangersimple.outflow[0] != null) { listView2.Items.Add(thecoolingtowerheatexchangersimple.outflow[0].nr.ToString()); }
        }

        private void refreshdialogue()
        {
            textBox16.Text = thecoolingtowerheatexchangersimple.name;
            textBox1.Text = thecoolingtowerheatexchangersimple.U.ToString("G5");
            textBox2.Text = thecoolingtowerheatexchangersimple.A.ToString("G5");

            textBox3.Text = thecoolingtowerheatexchangersimple.strm1flowcoefficient.ToString("G5");
            textBox5.Text = thecoolingtowerheatexchangersimple.strm1temptau.ToString("G5");
            textBox4.Text = thecoolingtowerheatexchangersimple.strm1flowtau.ToString("G5");

            textBox6.Text = thecoolingtowerheatexchangersimple.strm2flowcoefficient.ToString("G5");
            textBox8.Text = thecoolingtowerheatexchangersimple.strm2temptau.ToString("G5");
            textBox7.Text = thecoolingtowerheatexchangersimple.strm2flowtau.ToString("G5");

        }

        private void readthedialogue()
        {
            thecoolingtowerheatexchangersimple.name = textBox16.Text;
            thecoolingtowerheatexchangersimple.U = Convert.ToDouble(textBox1.Text);
            thecoolingtowerheatexchangersimple.A = Convert.ToDouble(textBox2.Text);

            thecoolingtowerheatexchangersimple.strm1flowcoefficient = Convert.ToDouble(textBox3.Text);
            thecoolingtowerheatexchangersimple.strm1temptau = Convert.ToDouble(textBox5.Text);
            thecoolingtowerheatexchangersimple.strm1flowtau = Convert.ToDouble(textBox4.Text);

            thecoolingtowerheatexchangersimple.strm2flowcoefficient = Convert.ToDouble(textBox6.Text);
            thecoolingtowerheatexchangersimple.strm2temptau = Convert.ToDouble(textBox8.Text);
            thecoolingtowerheatexchangersimple.strm2flowtau = Convert.ToDouble(textBox7.Text);
        }

        private void button1_Click(object sender, EventArgs e) //OK button
        {
            readthedialogue();
            Hide();
        }

        private void button3_Click(object sender, EventArgs e) //Update button
        {
            readthedialogue();
            refreshdialogue();
        }

        private void button4_Click(object sender, EventArgs e) //Cancel button
        {
            Hide();
        }




    }
}
