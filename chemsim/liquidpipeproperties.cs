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
    [Serializable]
    public partial class liquidpipeproperties : Form
    {
        public liquidpipe theliquidpipe;
        public simulation thesim;

        public liquidpipeproperties(liquidpipe aliquidpipe, simulation asim)
        {
            InitializeComponent();

            theliquidpipe = aliquidpipe;
            thesim = asim;

            refreshdialogue();
        }

        public void refreshdialogue()
        {
            textBox1.Text = utilities.pascal2barg(theliquidpipe.pressure0.v).ToString("G5");
            textBox2.Text = utilities.pascal2barg(theliquidpipe.pressure1.v).ToString("G5");
            textBox3.Text = utilities.kelvin2celcius(theliquidpipe.mat.T.v).ToString("G5");
            textBox4.Text = utilities.fps2fph(theliquidpipe.actualvolumeflow.v).ToString("G5");
            textBox5.Text = utilities.fps2fph(theliquidpipe.standardvolumeflow.v).ToString("G5");
            textBox6.Text = utilities.fps2fph(theliquidpipe.massflow.v).ToString("G5");
            textBox7.Text = theliquidpipe.molarflow.v.ToString("G5");
            textBox8.Text = theliquidpipe.mat.n.v.ToString("G5");
            textBox9.Text = theliquidpipe.mat.density.v.ToString("G5");
            textBox10.Text = theliquidpipe.holduptime.v.ToString("G5");
            textBox11.Text = theliquidpipe.mat.V.v.ToString("G5");
            textBox12.Text = theliquidpipe.length.ToString("G5");
            textBox13.Text = theliquidpipe.diameter.ToString("G5");
            textBox14.Text = theliquidpipe.crosssectionalarea.ToString("G5");
            textBox15.Text = theliquidpipe.filocation.v.ToString("G5");
            textBox16.Text = theliquidpipe.reynoldsnumber.v.ToString("G5");
            textBox17.Text = theliquidpipe.meanvelocity.v.ToString("G5");
            textBox18.Text = theliquidpipe.darcyfrictionfactor.v.ToString("G5");
            textBox19.Text = utilities.radians2degrees(theliquidpipe.direction).ToString("G5");
            textBox20.Text = theliquidpipe.distance.ToString("G5");

            for (int i = 0; i < global.liquidpipeflowreferencestrings.Length; i++)
            { listBox1.Items.Add(global.liquidpipeflowreferencestrings[i]); }
            listBox1.SetSelected((int)theliquidpipe.flowreference, true);



        }

        private void label2_Click(object sender, EventArgs e) //Double click the Pressure1 label for trending
        {
            trendvar thetrendvar = new trendvar(utilities.adddimension(global.simtimevector),
                utilities.adddimension(utilities.vectorprocessor(theliquidpipe.pressure1simvector, utilities.pascal2barg)),
                "Pressure (Barg)");
            thetrendvar.Show();
        }

        private void updatetheliquidpipe()
        {
            theliquidpipe.pressure0.v = utilities.barg2pascal(Convert.ToDouble(textBox1.Text));

            for (int i = 0; i < global.liquidpipeflowreferencestrings.Length; i++)
            {
                if (listBox1.GetSelected(i)) { theliquidpipe.flowreference = (liquidpipeflowreference)i; }
            }
            theliquidpipe.update(thesim.simi, true);
        }

    }
}
