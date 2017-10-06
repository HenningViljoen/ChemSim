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
    public partial class mixerproperties : Form
    {
        private mixer themixer;
        private simulation thesim;

        public mixerproperties(mixer amixer, simulation asim)
        {
            InitializeComponent();

            themixer = amixer;
            thesim = asim;

            Text = themixer.name;

            refreshdialogue();
        }

        private void refreshdialogue()
        {
            textBox1.Text = themixer.nin.ToString("G");
        }

        private void readthedialogue()
        {
            themixer.nin = Convert.ToInt32(textBox1.Text);
            themixer.initinpoint();
            themixer.initinflow();
        }

        private void button1_Click(object sender, EventArgs e) //OK button
        {
            readthedialogue();
            Hide();
        }

        private void button2_Click(object sender, EventArgs e) //Update button
        {
            readthedialogue();
            refreshdialogue();
        }

        private void button3_Click(object sender, EventArgs e) //Cancel button
        {
            Hide();
        }



    }
}
