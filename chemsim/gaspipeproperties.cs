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
    public partial class gaspipeproperties : Form
    {
        public gaspipe thegaspipe;
        public simulation thesim;

        public gaspipeproperties(gaspipe agaspipe, simulation asim)
        {
            InitializeComponent();

            thegaspipe = agaspipe;
            thesim = asim;

            refreshdialogue();

        }

        private void refreshdialogue()
        {
            textBox18.Text = thegaspipe.name;
            textBox16.Text = utilities.pascal2barg(thegaspipe.mat.P).ToString("G5");
            textBox17.Text = utilities.kelvin2celcius(thegaspipe.mat.T).ToString("G5");
            textBox8.Text = thegaspipe.mat.density.ToString("G5");
            textBox1.Text = utilities.fps2fph(thegaspipe.actualvolumeflow).ToString("G5");
            textBox15.Text = utilities.fps2fph(thegaspipe.standardvolumeflow).ToString("G5");
            textBox2.Text = utilities.fps2fph(thegaspipe.massflow).ToString("G5");
            textBox3.Text = thegaspipe.molarflow.ToString("G5");
            textBox4.Text = utilities.fps2fph(thegaspipe.dndtin).ToString("G5");
            textBox5.Text = utilities.fps2fph(thegaspipe.dndtout).ToString("G5");
            textBox6.Text = thegaspipe.filocation.ToString("G5");
            textBox7.Text = thegaspipe.mat.n.ToString("G5");
            textBox14.Text = thegaspipe.holduptime.ToString("G5");
            textBox13.Text = thegaspipe.mat.V.ToString("G5");
            textBox12.Text = thegaspipe.length.ToString("G5");
            textBox11.Text = thegaspipe.diameter.ToString("G5");
            textBox10.Text = thegaspipe.direction.ToString("G5");
            textBox9.Text = thegaspipe.distance.ToString("G5");
        }

        private void label32_Click(object sender, EventArgs e) //Double click the Pressure label for trending
        {
            trendvar thetrendvar = new trendvar(utilities.adddimension(global.simtimevector),
                utilities.adddimension(utilities.vectorprocessor(thegaspipe.mat.P.simvector, utilities.pascal2barg)),
                "Pressure (Barg)");
            thetrendvar.Show();
        }

        private void updatethegaspipe()
        {
            thegaspipe.name = textBox18.Text;

            if (utilities.barg2pascal(Convert.ToDouble(textBox16.Text)) != thegaspipe.mat.P.v)
            {
                thegaspipe.mat.P.v = utilities.barg2pascal(Convert.ToDouble(textBox16.Text));
                thegaspipe.calcn();
            }
            thegaspipe.massflow.v = utilities.fph2fps(Convert.ToDouble(textBox2.Text));
            thegaspipe.update(thesim.simi, true);
        }


        private void button1_Click(object sender, EventArgs e) //OK button click. Will update and close.
        {
            updatethegaspipe();
            Hide();
        }

        private void button2_Click(object sender, EventArgs e) //Update button click.
        {
            updatethegaspipe();
        }

        private void button3_Click(object sender, EventArgs e) //Cancel button click.
        {
            Hide();
        }

        private void label4_Click(object sender, EventArgs e) //Double click the mass flow label for trending
        {
            trendvar thetrendvar = new trendvar(utilities.adddimension(global.simtimevector),
                utilities.adddimension(utilities.vectorprocessor(thegaspipe.massflow.simvector, utilities.fps2fph)),
                "Mass flow (kg/h)");
            thetrendvar.Show();
        }

        private void label30_Click(object sender, EventArgs e) //Double click the standard flow label for trending
        {
            trendvar thetrendvar = new trendvar(utilities.adddimension(global.simtimevector),
                utilities.adddimension(utilities.vectorprocessor(thegaspipe.standardvolumeflow.simvector, utilities.fps2fph)),
                "Standard flow (Nm3/h)");
            thetrendvar.Show();
        }
    }
}
