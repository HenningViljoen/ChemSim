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
    public partial class pumpproperties : Form
    {
        public pump thepump;
        public simulation thesim;

        public pumpproperties(pump apump, simulation asim)
        {
            InitializeComponent();

            

            thepump = apump;
            thesim = asim;
            Text = ""; // String.Concat("Properties for pump number: ", pumpnr.ToString());

            refreshdialogue();

            listView1.Items.Add(thepump.inflow[0].nr.ToString());
            listView2.Items.Add(thepump.outflow[0].nr.ToString());

            if (thepump.calcmethod == calculationmethod.DetermineFlow)
            {
                radioButton1.Checked = true; //Default will be that the pump will determine the flow through it.
            }
            else
            {
                radioButton2.Checked = true;
            }

            Text = String.Concat("Properties for pump number: ", thepump.nr.ToString());
        }

        private void refreshdialogue()
        {
            textBox1.Text = utilities.pascal2bara(thepump.deltapressure.v).ToString("N");
            textBox2.Text = utilities.fps2fph(thepump.actualvolumeflow.v).ToString("N");
            textBox3.Text = utilities.pascal2bara(thepump.maxdeltapressure).ToString("N");
            textBox4.Text = utilities.fps2fph(thepump.maxactualvolumeflow.v).ToString("N");
            textBox5.Text = thepump.pumpspeed.v.ToString("N");
            textBox6.Text = thepump.pumpspeeddynamic.ToString("N");
            textBox7.Text = thepump.speedtau.ToString("N");

            checkBox1.Checked = (thepump.on.v >= 0.5) ? true:false;
        }

        private void label1_Click(object sender, EventArgs e)
        //Double clicking Delta pressure will trend the history vector
        {
            trendvar thetrendvar = new trendvar(utilities.adddimension(global.simtimevector),
                utilities.adddimension(utilities.vectorprocessor(thepump.deltapressure.simvector, utilities.pascal2bara)),
                "Delta pressure (Bara)");
            thetrendvar.Show();
        }

        private void label4_Click(object sender, EventArgs e)
        //Double clicking Actual volume flow will trend the history vector
        {
            trendvar thetrendvar = new trendvar(utilities.adddimension(global.simtimevector),
                utilities.adddimension(utilities.vectorprocessor(thepump.actualvolumeflow.simvector, utilities.fps2fph)),
                "Actual volume flow (m3/h)");
            thetrendvar.Show();
        }

        private void readthedialogue()
        {
            thepump.maxdeltapressure = utilities.bara2pascal(Convert.ToDouble(textBox3.Text));
            thepump.maxactualvolumeflow.v = utilities.fph2fps(Convert.ToDouble(textBox4.Text));

            if (radioButton1.Checked)
            {
                thepump.calcmethod = calculationmethod.DetermineFlow;
            }
            else
            {
                thepump.calcmethod = calculationmethod.DeterminePressure;
            }

            thepump.on.v = (checkBox1.Checked) ? 1:0;
            thepump.pumpspeed.v = Convert.ToDouble(textBox5.Text);
            thepump.speedtau = Convert.ToDouble(textBox7.Text);
        }

        private void button1_Click(object sender, EventArgs e) //OK button click
        {
            readthedialogue();
            //thepump.calcpumpcurve();
            thepump.update(thesim.simi, false);
            Hide();
        }

        private void button2_Click(object sender, EventArgs e) //Update button click
        {
            readthedialogue();
            //thepump.calcpumpcurve();
            thepump.update(thesim.simi, false);
            refreshdialogue();
        }

        private void button3_Click(object sender, EventArgs e) //Cancel button click
        {
            Hide();
        }

        private void button4_Click(object sender, EventArgs e) //Pump hydraulic curve click event handler
        {
            double[] volumeflow = new double[11];
            double[] pressure = new double[11];
            for (int i = 0; i <= 10; i++)
            {
                volumeflow[i] = i/10.0*thepump.maxactualvolumeflow.v;
                pressure[i] = thepump.calcdeltapressurequadratic(volumeflow[i]);
                
                //pressure[i] = thepump.pumpcurvem*volumeflow[i] + thepump.pumpcurvec;
            }

            trendvar thetrendvar = new trendvar(
                utilities.adddimension(volumeflow),
                utilities.adddimension(pressure),
                "Pressure increase over pump (Pa)");
            thetrendvar.Show();
        }

        private void button5_Click(object sender, EventArgs e)  //Pump power curve click event handler
        {
            double[] volumeflow = new double[11];
            double[] pressure = new double[11];
            double[] massflow = new double[11];
            double[] power = new double[11];
            for (int i = 0; i <= 10; i++)
            {
                volumeflow[i] = i / 10.0 * thepump.maxactualvolumeflow.v;
                pressure[i] = thepump.calcdeltapressurequadratic(volumeflow[i]);
                massflow[i] = volumeflow[i] * thepump.inflow[0].mat.density.v;
                power[i] = thepump.calcpumppower(volumeflow[i], pressure[i]);

                //pressure[i] = thepump.pumpcurvem*volumeflow[i] + thepump.pumpcurvec;
            }

            trendvar thetrendvar = new trendvar(
                utilities.adddimension(volumeflow),
                utilities.adddimension(power),
                "Pump power consumption (W)");
            thetrendvar.Show();
        }



    }
}
