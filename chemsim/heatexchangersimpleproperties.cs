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
    public partial class heatexchangersimpleproperties : Form
    {
        private heatexchangersimple theheatexchangersimple;
        private simulation thesim;

        public heatexchangersimpleproperties(heatexchangersimple aheatexchangersimple, simulation asim)
        {
            InitializeComponent();

            theheatexchangersimple = aheatexchangersimple;
            thesim = asim;
            Text = "HeatexchangerSimple Properties - " + theheatexchangersimple.name;
            refreshdialogue();
            if (theheatexchangersimple.inflow[0] != null) { listView1.Items.Add(theheatexchangersimple.inflow[0].nr.ToString()); }
            if (theheatexchangersimple.outflow[0] != null) { listView2.Items.Add(theheatexchangersimple.outflow[0].nr.ToString()); }
        }

        private void refreshdialogue()
        {
            textBox16.Text = theheatexchangersimple.name;
            textBox1.Text = theheatexchangersimple.U.ToString("G5");
            textBox2.Text = theheatexchangersimple.A.ToString("G5");

            textBox3.Text = theheatexchangersimple.strm1flowcoefficient.ToString("G5");
            textBox5.Text = theheatexchangersimple.strm1temptau.ToString("G5");
            textBox4.Text = theheatexchangersimple.strm1flowtau.ToString("G5");

            textBox6.Text = theheatexchangersimple.strm2flowcoefficient.ToString("G5");
            textBox8.Text = theheatexchangersimple.strm2temptau.ToString("G5");
            textBox7.Text = theheatexchangersimple.strm2flowtau.ToString("G5");

        }

        private void readthedialogue()
        {
            theheatexchangersimple.name = textBox16.Text;
            theheatexchangersimple.U.v = Convert.ToDouble(textBox1.Text);
            theheatexchangersimple.A.v = Convert.ToDouble(textBox2.Text);

            theheatexchangersimple.strm1flowcoefficient = Convert.ToDouble(textBox3.Text);
            theheatexchangersimple.strm1temptau.v = Convert.ToDouble(textBox5.Text);
            theheatexchangersimple.strm1flowtau = Convert.ToDouble(textBox4.Text);

            theheatexchangersimple.strm2flowcoefficient.v = Convert.ToDouble(textBox6.Text);
            theheatexchangersimple.strm2temptau.v = Convert.ToDouble(textBox8.Text);
            theheatexchangersimple.strm2flowtau = Convert.ToDouble(textBox7.Text);
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
