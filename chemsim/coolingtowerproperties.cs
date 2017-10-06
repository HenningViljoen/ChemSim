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
    public partial class coolingtowerproperties : Form
    {
        public coolingtower thecoolingtower;
        public simulation thesim;
        public List<unitop> sprayvalvelist;

        public coolingtowerproperties(coolingtower acoolingtower, simulation asim)
        {
            InitializeComponent();

            thecoolingtower = acoolingtower;
            thesim = asim;
            Text = "CoolingTower Properties - " + thecoolingtower.name;
            sprayvalvelist = new List<unitop>();
            refreshdialogue();
            if (thecoolingtower.inflow[0] != null) { listView1.Items.Add(thecoolingtower.inflow[0].nr.ToString()); }
            if (thecoolingtower.outflow[0] != null) { listView2.Items.Add(thecoolingtower.outflow[0].nr.ToString()); }
        }

        private void populatesprayvalvelist()
        {
            sprayvalvelist.Clear();
            listBox1.Items.Clear();
            int j = 0;
            for (int i = 0; i < thesim.unitops.Count; i++)
            {

                if (thesim.unitops[i].objecttype == objecttypes.Valve)
                {
                    sprayvalvelist.Add(thesim.unitops[i]);
                    listBox1.Items.Add(thesim.unitops[i].name);

                    if (object.ReferenceEquals(thecoolingtower.linkedsprayvalve, thesim.unitops[i]))
                    {
                        listBox1.SetSelected(j, true);
                    }
                    j++;
                }
            }
        }

        private void refreshdialogue()
        {
            textBox16.Text = thecoolingtower.name;
            textBox4.Text = thecoolingtower.watervolumefraction.ToString("G5");
            textBox2.Text = thecoolingtower.masstransfercoefair.ToString("G5");
            textBox3.Text = thecoolingtower.heattransfercoefwater.ToString("G5");
            textBox5.Text = thecoolingtower.heattransfercoefair.ToString("G5");
            textBox1.Text = thecoolingtower.fanspeed.ToString("G5");

            checkBox1.Checked = (thecoolingtower.on.v >= 0.5) ? true:false;

            populatesprayvalvelist();
        }

        private void readthedialogue()
        {
            thecoolingtower.name = textBox16.Text;
            thecoolingtower.watervolumefraction.v = Convert.ToDouble(textBox4.Text);
            thecoolingtower.masstransfercoefair.v = Convert.ToDouble(textBox2.Text);
            thecoolingtower.heattransfercoefwater.v = Convert.ToDouble(textBox3.Text);
            thecoolingtower.heattransfercoefair.v = Convert.ToDouble(textBox5.Text);
            thecoolingtower.on.v = (checkBox1.Checked) ? 1:0;
            thecoolingtower.fanspeed.v = Convert.ToDouble(textBox1.Text);

            thecoolingtower.updatefrompropertydialogue();
        }

        private void button1_Click(object sender, EventArgs e) //OK button click event
        {
            readthedialogue();
            Hide();
        }

        private void button3_Click(object sender, EventArgs e) //Update button click event
        {
            readthedialogue();
            refreshdialogue();
        }

        private void button4_Click(object sender, EventArgs e) //Cancel button click event
        {
            Hide();
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)  //This event fires when the sprayvalve to be linked 
        // changes.
        {
            for (int i = 0; i < listBox1.Items.Count; i++)
            {
                if (listBox1.GetSelected(i)) { thecoolingtower.linkedsprayvalve = (valve)sprayvalvelist[i]; }
            }
        }
    }
}
