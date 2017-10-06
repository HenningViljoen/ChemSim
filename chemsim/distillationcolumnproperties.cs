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
    public partial class distillationcolumnproperties : Form
    {
        private simulation thesim;
        private distillationcolumn thedistillationcolumn;

        public distillationcolumnproperties(distillationcolumn adistillationcolumn, simulation asim)
        {
            InitializeComponent();

            thedistillationcolumn = adistillationcolumn;
            thesim = asim;
        }


        private void button1_Click(object sender, EventArgs e)
        {
            materialproperties mp = new materialproperties(thedistillationcolumn.inv[(int)numericUpDown1.Value], thesim);
            mp.Show();
        }
    }
}
