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
    public partial class heatexchangerproperties : Form
    {
        private heatexchanger theheatexchanger;

        public heatexchangerproperties(heatexchanger aheatexchanger, simulation asim)
        {
            InitializeComponent();
        }
    }
}
