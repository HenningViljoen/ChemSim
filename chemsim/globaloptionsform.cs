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
    public partial class globaloptionsform : Form
    {
        private simulation sim;

        public globaloptionsform(simulation asim)
        {
            InitializeComponent();

            sim = asim;
            refreshdialogue();
        }

        private void refreshdialogue()
        {
            textBox1.Text = global.TimerInterval.ToString("G5");
            textBox2.Text = global.SpeedUpFactor.ToString("G5");
            label6.Text = global.SampleT.ToString("G5");
            textBox3.Text = global.SimTime.ToString("G5");
            label7.Text = global.SimIterations.ToString("G5");
        }

        private void readdialogue()
        {
            global.TimerInterval = Convert.ToInt32(textBox1.Text);
            global.SpeedUpFactor = Convert.ToDouble(textBox2.Text);
            global.SampleT = global.calcSampleT();
            global.SimTime = Convert.ToDouble(textBox3.Text);
            global.SimIterations = global.calcSimIterations();

            global.initsimtimevector();

            refreshdialogue();
        }

        private void label8_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e) //Click event for OK button
        {
            readdialogue();
            Hide();
        }

        private void button2_Click(object sender, EventArgs e) //Click event for Update button
        {
            readdialogue();
        }

        private void button3_Click(object sender, EventArgs e) //Click event for Cancel button
        {
            Hide();
        }
    }
}
