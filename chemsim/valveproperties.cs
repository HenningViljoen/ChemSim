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
    public partial class valveproperties : Form
    {
        private valve thevalve;
        private simulation thesim;

        public valveproperties(valve avalve, simulation asim)
        {
            InitializeComponent();

            thevalve = avalve;
            thesim = asim;
            Text = "Valve Properties";
            refreshdialogue();
            if (thevalve.inflow[0] != null) { listView1.Items.Add(thevalve.inflow[0].nr.ToString()); }
            if (thevalve.outflow[0] != null) { listView2.Items.Add(thevalve.outflow[0].nr.ToString()); }
        }

        private void refreshdialogue()
        {
            textBox16.Text = thevalve.name;
            textBox1.Text = utilities.pascal2bara(thevalve.deltapressure.v).ToString("G5");
            textBox2.Text = utilities.fps2fph(thevalve.actualvolumeflow.v).ToString("G5");
            textBox3.Text = utilities.fps2fph(thevalve.standardvolumeflow.v).ToString("G5");
            textBox7.Text = utilities.fps2fph(thevalve.massflow.v).ToString("G5");
            textBox4.Text = thevalve.molarflow.v.ToString("G5");
            textBox5.Text = thevalve.Cv.ToString("G5");
            textBox6.Text = thevalve.op.v.ToString("G5");
        }

        private void readthedialogue(bool readcv = false)
        {
            thevalve.name = textBox16.Text;
            thevalve.Cv = Convert.ToDouble(textBox5.Text); 
            if (readcv) 
            { 
                thevalve.deltapressure.v = utilities.bara2pascal(Convert.ToDouble(textBox1.Text));
                thevalve.actualvolumeflow.v = utilities.fph2fps(Convert.ToDouble(textBox2.Text));
            }
            //thevalve.standardvolumeflow = utilities.fph2fps(Convert.ToDouble(textBox3.Text));

            
            thevalve.op.v = Convert.ToDouble(textBox6.Text);
        }

        private void button1_Click(object sender, EventArgs e) //OK Button
        {
            readthedialogue();
            //thevalve.update(thesim.simi);
            Hide();
        }

        private void button3_Click(object sender, EventArgs e) //Update Button
        {
            readthedialogue();
            //thevalve.update(thesim.simi);
        }

        private void button4_Click(object sender, EventArgs e) //Cancel Button
        {
            Hide();
        }

        private void button2_Click(object sender, EventArgs e) //Size valve Button
        {
            readthedialogue(true);

            //thevalve.sizevalvefromstandardflow();
            thevalve.sizevalvefromactualvolumeflow();
            refreshdialogue();
        }

        private void label16_Click(object sender, EventArgs e) //Double click on Mass flow label
        {
            trendvar thetrendvar = new trendvar(
                utilities.adddimension(global.simtimevector),
                utilities.adddimension(utilities.vectorprocessor(thevalve.massflow.simvector, utilities.fps2fph)),
                "Mass flow (kg/h)");
            thetrendvar.Show();
        }

        private void label6_Click(object sender, EventArgs e) //Double click on standard volume flow label for trending
        {
            trendvar thetrendvar = new trendvar(utilities.adddimension(global.simtimevector),
                utilities.adddimension(utilities.vectorprocessor(thevalve.standardvolumeflow.simvector, utilities.fps2fph)),
                "Standard flow (Nm3/h)");
            thetrendvar.Show();
        }





    }
}
