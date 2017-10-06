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
    public partial class pidcontrollerproperties : Form
    {
        public pidcontroller thecontroller;
        private simulation thesim;
        private int selectedbaseclasspv;
        private int selectedbaseclassop;
        private baseclasstypeenum baseclasstypepv;
        private baseclasstypeenum baseclasstypeop;
        private int selectedpv;
        private int selectedop;

        public pidcontrollerproperties(pidcontroller acontroller, simulation asim)
        {
            InitializeComponent();

            selectedbaseclasspv = 0;
            selectedpv = 0;
            baseclasstypepv = baseclasstypeenum.UnitOp;
            thecontroller = acontroller;
            thesim = asim;
            Text = "PID Controller Properties";
            refreshdialogue();
        }

        private void refreshdialogue()
        {
            refreshmaindialogue();
            refreshcontroldirection();
            refreshbaseclasslistpv();
            refreshpropertylistdialoguepv();
            refreshbaseclasslistop();
            refreshpropertylistdialogueop();
        }

        private void refreshmaindialogue()
        {
            textBox2.Text = thecontroller.K.ToString();
            textBox3.Text = thecontroller.I.ToString();
            textBox4.Text = thecontroller.D.ToString();
            textBox15.Text = thecontroller.sp.ToString();
            textBox5.Text = thecontroller.pvname;
            textBox6.Text = thecontroller.opname;
            textBox9.Text = thecontroller.minpv.ToString();
            textBox11.Text = thecontroller.maxpv.ToString();
            textBox10.Text = thecontroller.minop.ToString();
            textBox12.Text = thecontroller.maxop.ToString();

            
                if (thecontroller.pv != null)
                {
                    textBox7.Text = thecontroller.pv.v.ToString();
                }
                if (thecontroller.op != null)
                {
                    textBox8.Text = thecontroller.op.v.ToString();
                }
                if (thecontroller.pv != null && thecontroller.op != null)
                {
                    thecontroller.init();
                    textBox17.Text = thecontroller.bias.ToString();
                }


        }

        private void refreshcontroldirection()
        {
            for (int i = 0; i < Enum.GetValues(typeof(piddirection)).Length; i++)
            {
                listBox5.Items.Add(Enum.GetValues(typeof(piddirection)).GetValue(i));
                if (thecontroller.direction == (piddirection)Enum.GetValues(typeof(piddirection)).GetValue(i))
                { listBox5.SetSelected(i, true); }
            }
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

        private void refreshbaseclasslistop()
        {
            for (int i = 0; i < thesim.unitops.Count; i++)
            {
                listBox3.Items.Add(thesim.unitops[i].name);
            }
            for (int i = 0; i < thesim.streams.Count; i++)
            {
                listBox3.Items.Add(thesim.streams[i].name);
            }
        }

        private void refreshpropertylistdialogueop()
        {
            listBox4.Items.Clear();
            switch (baseclasstypeop)
            {
                case baseclasstypeenum.UnitOp:
                    for (int i = 0; i < thesim.unitops[selectedbaseclassop].controlproperties.Count; i++)
                    {
                        listBox4.Items.Add(thesim.unitops[selectedbaseclassop].controlproperties[i]);
                    }
                    break;
                case baseclasstypeenum.Stream:
                    for (int i = 0; i < thesim.streams[selectedbaseclassop - thesim.unitops.Count].controlproperties.Count; i++)
                    {
                        listBox4.Items.Add(thesim.streams[selectedbaseclassop - thesim.unitops.Count].controlproperties[i]);
                    }
                    break;
                default:
                    break;
            }
        }

        private void readthedialogue()
        {
            thecontroller.K = Convert.ToDouble(textBox2.Text);
            thecontroller.I = Convert.ToDouble(textBox3.Text);
            thecontroller.D = Convert.ToDouble(textBox4.Text);
            thecontroller.sp = Convert.ToDouble(textBox15.Text);

            for (int i = 0; i < listBox5.Items.Count; i++)
            {
                if (listBox5.GetSelected(i))
                { thecontroller.direction = (piddirection)Enum.GetValues(typeof(piddirection)).GetValue(i); }
            }


            //thecontroller.pvname = textBox5.Text = ;
            //textBox6.Text = thecontroller.opname;
            thecontroller.minpv = Convert.ToDouble(textBox9.Text);
            thecontroller.maxpv = Convert.ToDouble(textBox11.Text);
            thecontroller.minop = Convert.ToDouble(textBox10.Text);
            thecontroller.maxop = Convert.ToDouble(textBox12.Text);
            

            thecontroller.init();
        }

        private void button1_Click(object sender, EventArgs e) //OK button
        {
            readthedialogue();
            Hide();
        }

        private void button2_Click(object sender, EventArgs e) //Update button
        {
            readthedialogue();
            refreshmaindialogue();
        }

        private void button3_Click(object sender, EventArgs e) //Cancel button
        {
            Hide();
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)  //This event fires when the pv baseclass that 
        //is selected, changes.
        {
            for (int i = 0; i < listBox1.Items.Count; i++)
            {
                if (listBox1.GetSelected(i)) { selectedbaseclasspv = i; }
            }
            if (selectedbaseclasspv < thesim.unitops.Count) { baseclasstypepv = baseclasstypeenum.UnitOp; }
            else { baseclasstypepv = baseclasstypeenum.Stream; }
            refreshpropertylistdialoguepv();
        }

        private void listBox2_SelectedIndexChanged(object sender, EventArgs e) //This event fires when the pv property that is 
        //selected, changes.
        {
            for (int i = 0; i < listBox2.Items.Count; i++)
            {
                if (listBox2.GetSelected(i)) { selectedpv = i; }
            }

                switch (baseclasstypepv)
                {
                    case baseclasstypeenum.UnitOp:
                        thecontroller.pv = thesim.unitops[selectedbaseclasspv].selectedproperty(selectedpv);
                        thecontroller.pvname = thesim.unitops[selectedbaseclasspv].name + ": " +
                            (string)listBox2.Items[selectedpv];
                        thecontroller.sp = thecontroller.pv.v;
                        break;
                    case baseclasstypeenum.Stream:
                        thecontroller.pv = thesim.streams[selectedbaseclasspv - thesim.unitops.Count].
                            selectedproperty(selectedpv);
                        thecontroller.pvname = (string)listBox2.Items[selectedpv];
                        thecontroller.sp = thecontroller.pv.v;
                        break;
                    default:
                        break;
                }

            refreshmaindialogue();
        }

        private void listBox3_SelectedIndexChanged(object sender, EventArgs e) //This event fires when the op baseclass that 
        //is selected, changes.
        {
            for (int i = 0; i < listBox3.Items.Count; i++)
            {
                if (listBox3.GetSelected(i)) { selectedbaseclassop = i; }
            }
            if (selectedbaseclassop < thesim.unitops.Count) { baseclasstypeop = baseclasstypeenum.UnitOp; }
            else { baseclasstypeop = baseclasstypeenum.Stream; }
            refreshpropertylistdialogueop();
        }

        private void listBox4_SelectedIndexChanged(object sender, EventArgs e) //This event fires when the op property that is 
        //selected, changes.
        {
            for (int i = 0; i < listBox4.Items.Count; i++)
            {
                if (listBox4.GetSelected(i)) { selectedop = i; }
            }

                switch (baseclasstypeop)
                {
                    case baseclasstypeenum.UnitOp:
                        thecontroller.op = thesim.unitops[selectedbaseclassop].selectedproperty(selectedop);
                        thecontroller.opname = thesim.unitops[selectedbaseclassop].name + ": " +
                            (string)listBox4.Items[selectedop];
                        break;
                    case baseclasstypeenum.Stream:
                        thecontroller.op = thesim.streams[selectedbaseclassop - thesim.unitops.Count].
                            selectedproperty(selectedop);
                        thecontroller.opname = (string)listBox4.Items[selectedop];
                        break;
                    default:
                        break;
                }

            refreshmaindialogue();
        }





    }
}
