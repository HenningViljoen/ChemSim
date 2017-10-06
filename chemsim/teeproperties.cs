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
    public partial class teeproperties : Form
    {

        private tee thetee;
        private simulation thesim;
        private List<unitop> mixerlist;

        public teeproperties(tee atee, simulation asim)
        {
            InitializeComponent();

            thetee = atee;
            thesim = asim;

            mixerlist = new List<unitop>();

            refreshdialogue();
        }

        private void populatemixerlist()
        {
            mixerlist.Clear();
            listBox1.Items.Clear();
            int j = 0;
            for (int i = 0; i < thesim.unitops.Count; i++)
            {
                
                if (thesim.unitops[i].objecttype == objecttypes.Mixer)
                {
                    mixerlist.Add(thesim.unitops[i]);
                    listBox1.Items.Add(thesim.unitops[i].name);

                    if (object.ReferenceEquals(thetee.linkedmixer, thesim.unitops[i]))
                    {
                        listBox1.SetSelected(j, true);
                    }
                    j++;
                }
            }
        }

        private void refreshdialogue()
        {
            textBox1.Text = thetee.nout.ToString("G");
            populatemixerlist();
        }

        private void readthedialogue()
        {
            int anout = Convert.ToInt32(textBox1.Text);
            if (thetee.nout != anout)
            {
                thetee.nout = anout;
                thetee.initoutpoint();
                thetee.initinflow();
                thetee.initoutflow();
                thetee.initk();
                thetee.initbranchflows();
                thetee.initbranchdp();
            }
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

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e) ////This event fires when the mixer to be linked 
        // changes.
        {
            for (int i = 0; i < listBox1.Items.Count; i++)
            {
                if (listBox1.GetSelected(i)) { thetee.linkedmixer = (mixer)mixerlist[i]; }
            }
        }

    }
}
