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
    public partial class materialproperties : Form
    {
        private material thematerial;
        private simulation thesim;

        public materialproperties(material amaterial, simulation asim)
        {
            InitializeComponent();

            thematerial = amaterial;
            thesim = asim;

            refreshdialogue();
        }

        private void refreshdialogue()
        {
            textBox1.Text = thematerial.P.ToString("N");
            textBox2.Text = thematerial.V.ToString("N");
            textBox3.Text = thematerial.T.ToString("N");
            textBox4.Text = thematerial.mass.ToString("N");
            textBox5.Text = thematerial.n.ToString("N");
            textBox6.Text = thematerial.f.ToString("N");
            textBox7.Text = thematerial.U.ToString("N");

        }

        private void label7_Click(object sender, EventArgs e)
        {

        }
    }
}
