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
    public partial class controlmvsignalsplitterproperties : Form
    {
        public controlmvsignalsplitter thecontrolmvsignalsplitter;
        public simulation thesim;
        private int selectedbaseclasspv;

        private baseclasstypeenum baseclasstypepv;

        private int selectedpv;
        private int selectedcv;


        public controlmvsignalsplitterproperties(controlmvsignalsplitter acontrolmvsignalsplitter, simulation asim)
        {
            InitializeComponent();

            thecontrolmvsignalsplitter = acontrolmvsignalsplitter;
            thesim = asim;
            Text = thecontrolmvsignalsplitter.name + " Properties";
            refreshdialogue();
        }

        private void refreshdialogue()
        {
            refreshmaindialogue();
            //refreshcontroldirection();
            refreshbaseclasslistpv();
            refreshpropertylistdialoguepv();
            refreshcvlist();

        }

        private void readthedialogue()
        {
            thecontrolmvsignalsplitter.inputsignal.v = Convert.ToDouble(textBox1.Text);
            refreshcvlist();
        }

        private void refreshmaindialogue()
        {
            textBox1.Text = thecontrolmvsignalsplitter.inputsignal.v.ToString("G5");


        }

        private void refreshbaseclasslistpv()
        {
            for (int i = 0; i < thesim.unitops.Count; i++)
            {
                listBox1.Items.Add(thesim.unitops[i].name);
            }
            for (int i = 0; i < thesim.streams.Count; i++)
            {
                listBox1.Items.Add(thesim.streams[i].name);
            }
        }

        private void refreshpropertylistdialoguepv()
        {
            listBox2.Items.Clear();
            switch (baseclasstypepv)
            {
                case baseclasstypeenum.UnitOp:
                    for (int i = 0; i < thesim.unitops[selectedbaseclasspv].controlproperties.Count; i++)
                    {
                        listBox2.Items.Add(thesim.unitops[selectedbaseclasspv].controlproperties[i]);
                    }
                    break;
                case baseclasstypeenum.Stream:
                    for (int i = 0; i < thesim.streams[selectedbaseclasspv - thesim.unitops.Count].controlproperties.Count; i++)
                    {
                        listBox2.Items.Add(thesim.streams[selectedbaseclasspv - thesim.unitops.Count].controlproperties[i]);
                    }
                    break;
                default:
                    break;
            }
        }

        private void refreshcvlist()
        {
            listBox7.Items.Clear();
            
            for (int i = 0; i < thecontrolmvsignalsplitter.outputsignals.Count; i++)
            {
                listBox7.Items.Add(thecontrolmvsignalsplitter.outputsignals[i].name);
                
            }
            if (thecontrolmvsignalsplitter.outputsignals.Count > 0)
            {
                if (selectedcv >= thecontrolmvsignalsplitter.outputsignals.Count) { selectedcv = thecontrolmvsignalsplitter.outputsignals.Count - 1; }
                listBox7.SetSelected(selectedcv, true);
            }
        }

        private void button4_Click(object sender, EventArgs e) //Click event for Add Output signal button
        {
            string aname;
            for (int i = 0; i < listBox2.Items.Count; i++)
            {
                if (listBox2.GetSelected(i)) { selectedpv = i; }
            }

            switch (baseclasstypepv)
            {
                case baseclasstypeenum.UnitOp:
                    aname = thesim.unitops[selectedbaseclasspv].name + ": " + (string)listBox2.Items[selectedpv];
                    thecontrolmvsignalsplitter.outputsignals.Add(new mpcvar(thesim.unitops[selectedbaseclasspv].selectedproperty(selectedpv), aname, 0, 1));
                    thecontrolmvsignalsplitter.inputsignal.v =
                        thecontrolmvsignalsplitter.outputsignals[thecontrolmvsignalsplitter.outputsignals.Count - 1].var.v;
                    break;
                case baseclasstypeenum.Stream:
                    aname = thesim.streams[selectedbaseclasspv - thesim.unitops.Count].name + ": " + (string)listBox2.Items[selectedpv];
                    thecontrolmvsignalsplitter.outputsignals.Add(new mpcvar(thesim.streams[selectedbaseclasspv - thesim.unitops.Count].
                        selectedproperty(selectedpv), aname, 0, 1));
                    thecontrolmvsignalsplitter.inputsignal.v =
                        thecontrolmvsignalsplitter.outputsignals[thecontrolmvsignalsplitter.outputsignals.Count - 1].var.v;
                    break;
                default:
                    break;
            }
            
            refreshcvlist();
            refreshmaindialogue();
        }

        private void button5_Click(object sender, EventArgs e) //Click event for Delete Outpout signal button
        {
            for (int i = 0; i < listBox7.Items.Count; i++)
            {
                if (listBox7.GetSelected(i)) { selectedcv = i; }
            }
            if (selectedcv > -1)
            {
                thecontrolmvsignalsplitter.outputsignals.RemoveAt(selectedcv);
                refreshcvlist();
                refreshmaindialogue();
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < listBox1.Items.Count; i++)
            {
                if (listBox1.GetSelected(i)) { selectedbaseclasspv = i; }
            }
            if (selectedbaseclasspv < thesim.unitops.Count) { baseclasstypepv = baseclasstypeenum.UnitOp; }
            else { baseclasstypepv = baseclasstypeenum.Stream; }
            refreshpropertylistdialoguepv();
        }

        private void listBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < listBox2.Items.Count; i++)
            {
                if (listBox2.GetSelected(i)) { selectedpv = i; }
            }

            refreshmaindialogue();
        }

        private void listBox7_SelectedIndexChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < listBox7.Items.Count; i++)
            {
                if (listBox7.GetSelected(i)) { selectedcv = i; }
            }

            refreshmaindialogue();
        }



        private void button1_Click(object sender, EventArgs e) //Click event for OK button.
        {
            readthedialogue();
            Hide();
        }

        private void button2_Click(object sender, EventArgs e) //Click event for Update button.
        {
            readthedialogue();
            refreshmaindialogue();
        }

        private void button3_Click(object sender, EventArgs e) //Click event for Cancel button.
        {
            Hide();
        }

        

        
    }
}
