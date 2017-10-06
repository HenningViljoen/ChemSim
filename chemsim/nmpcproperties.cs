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
    public partial class nmpcproperties : Form
    {
        private nmpc thenmpc;
        private simulation thesim;
        private int selectedbaseclasspv;
        private int selectedbaseclassop;
        private baseclasstypeenum baseclasstypepv;
        private baseclasstypeenum baseclasstypeop;
        private int selectedpv;
        private int selectedcv;
        private int selectedop;
        private int selectedmv;
        private int selectedboolmv;

        public nmpcproperties(nmpc anmpc, simulation asim)
        {
            InitializeComponent();

            selectedbaseclasspv = 0;
            selectedpv = 0;
            selectedcv = 0;
            selectedmv = 0;
            selectedboolmv = 0;
            baseclasstypepv = baseclasstypeenum.UnitOp;
            thenmpc = anmpc;
            thesim = asim;
            Text = "NMPC Controller Properties";
            refreshdialogue();
        }

        private void refreshdialogue()
        {
            refreshmaindialogue();
            //refreshcontroldirection();
            refreshbaseclasslistpv();
            refreshpropertylistdialoguepv();
            refreshcvlist();
            refreshbaseclasslistop();
            refreshpropertylistdialogueop();
            refreshmvlist();
        }

        private void readthedialogue()
        {
            thenmpc.N = Convert.ToInt32(textBox2.Text);
            thenmpc.initialdelay = Convert.ToInt32(textBox6.Text);
            thenmpc.runinterval = Convert.ToInt32(textBox4.Text);
            thenmpc.alphak = Convert.ToDouble(textBox5.Text);
            thenmpc.cvmaster[selectedcv].target.v = Convert.ToDouble(textBox15.Text);
            if (thenmpc.cvmaster[selectedcv].target.datasource == datasourceforvar.Simulation)
            {
                for (int i = 0; i < global.SimIterations; i++)
                {
                    thenmpc.cvmaster[selectedcv].target.simvector[i] = thenmpc.cvmaster[selectedcv].target.v;
                }
            }
            thenmpc.cvmaster[selectedcv].weight = Convert.ToDouble(textBox3.Text);
            thenmpc.cvmaster[selectedcv].min = Convert.ToDouble(textBox14.Text);
            thenmpc.cvmaster[selectedcv].max = Convert.ToDouble(textBox13.Text);
            if (thenmpc.mvmaster.Count > 0)
            {
                thenmpc.mvmaster[selectedmv].min = Convert.ToDouble(textBox10.Text);
                thenmpc.mvmaster[selectedmv].max = Convert.ToDouble(textBox12.Text);
            }
            if (thenmpc.mvboolmaster.Count > 0)
            {
                thenmpc.mvboolmaster[selectedboolmv].min = Convert.ToDouble(textBox17.Text);
                thenmpc.mvboolmaster[selectedboolmv].max = Convert.ToDouble(textBox16.Text);
            }
            refreshcvlist();
            refreshmvlist();
            thenmpc.validatesettings();
            thenmpc.initjacobian();
            
        }

        private void refreshmaindialogue()
        {
            textBox2.Text = thenmpc.N.ToString();
            textBox6.Text = thenmpc.initialdelay.ToString();
            textBox4.Text = thenmpc.runinterval.ToString();
            textBox5.Text = thenmpc.alphak.ToString();

            switch (baseclasstypepv)
            {
                case baseclasstypeenum.UnitOp:
                    textBox7.Text = thesim.unitops[selectedbaseclasspv].selectedproperty(selectedpv).v.ToString();
                    break;
                case baseclasstypeenum.Stream:
                    textBox7.Text = thesim.streams[selectedbaseclasspv - thesim.unitops.Count].selectedproperty(selectedpv).v.ToString();
                    break;
                default:
                    break;
            }

            if (thenmpc.cvmaster.Count > 0)
            {
                textBox15.Text = thenmpc.cvmaster[selectedcv].target.ToString();
                textBox3.Text = thenmpc.cvmaster[selectedcv].weight.ToString();
                textBox14.Text = thenmpc.cvmaster[selectedcv].min.ToString();
                textBox13.Text = thenmpc.cvmaster[selectedcv].max.ToString();
            
            }
            else
            {
                textBox15.Text = "";
                textBox3.Text = "";
                textBox14.Text = "";
                textBox13.Text = "";
            }

            if (thenmpc.mvmaster.Count > 0)
            {
                textBox8.Text = thenmpc.mvmaster[selectedmv].var.v.ToString();
                textBox10.Text = thenmpc.mvmaster[selectedmv].min.ToString();
                textBox12.Text = thenmpc.mvmaster[selectedmv].max.ToString();
            }
            else
            {
                textBox8.Text = "";
                textBox10.Text = "";
                textBox12.Text = "";
            }

            if (thenmpc.mvboolmaster.Count > 0)
            {
                textBox18.Text = thenmpc.mvboolmaster[selectedboolmv].var.v.ToString();
                textBox17.Text = thenmpc.mvboolmaster[selectedboolmv].min.ToString();
                textBox16.Text = thenmpc.mvboolmaster[selectedboolmv].max.ToString();
            }
            else
            {
                textBox18.Text = "";
                textBox17.Text = "";
                textBox16.Text = "";
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

        private void refreshcvlist()
        {
            listBox7.Items.Clear();
            listBox6.Items.Clear();
            listBox8.Items.Clear();
            listBox16.Items.Clear();
            listBox15.Items.Clear();
            listBox11.Items.Clear();
            for (int i = 0; i < thenmpc.cvmaster.Count; i++)
            {
                listBox7.Items.Add(thenmpc.cvmaster[i].name);
                listBox6.Items.Add(thenmpc.cvmaster[i].target.v);
                listBox8.Items.Add(thenmpc.cvmaster[i].weight);
                listBox16.Items.Add(thenmpc.cvmaster[i].min);
                listBox15.Items.Add(thenmpc.cvmaster[i].max);


                listBox11.Items.Add((thenmpc.cvmaster[i].target.datasource == datasourceforvar.Simulation) ?
                    thenmpc.cvmaster[i].target.datasource.ToString() : thenmpc.cvmaster[i].target.excelsource.filename);
            }
            if (thenmpc.cvmaster.Count > 0) 
            {
                if (selectedcv >= thenmpc.cvmaster.Count) { selectedcv = thenmpc.cvmaster.Count - 1; }
                listBox7.SetSelected(selectedcv, true); 
            }
        }

        private void refreshmvlist()
        {
            listBox9.Items.Clear();
            listBox10.Items.Clear();
            listBox5.Items.Clear();
            for (int i = 0; i < thenmpc.mvmaster.Count; i++)
            {
                listBox9.Items.Add(thenmpc.mvmaster[i].name);
                listBox10.Items.Add(thenmpc.mvmaster[i].min);
                listBox5.Items.Add(thenmpc.mvmaster[i].max);
            }
            if (thenmpc.mvmaster.Count > 0)
            {
                if (selectedmv >= thenmpc.mvmaster.Count) { selectedmv = thenmpc.mvmaster.Count - 1; }
                listBox9.SetSelected(selectedmv, true);
            }

            listBox12.Items.Clear();
            listBox13.Items.Clear();
            listBox14.Items.Clear();
            for (int i = 0; i < thenmpc.mvboolmaster.Count; i++)
            {
                listBox12.Items.Add(thenmpc.mvboolmaster[i].name);
                listBox13.Items.Add(thenmpc.mvboolmaster[i].min);
                listBox14.Items.Add(thenmpc.mvboolmaster[i].max);
            }
            if (thenmpc.mvboolmaster.Count > 0)
            {
                if (selectedboolmv >= thenmpc.mvboolmaster.Count) { selectedboolmv = thenmpc.mvboolmaster.Count - 1; }
                listBox12.SetSelected(selectedboolmv, true);
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
            for (int i = 0; i < thesim.blocks.Count; i++)
            {
                listBox3.Items.Add(thesim.blocks[i].name);
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
                case baseclasstypeenum.Block:
                    for (int i = 0; i < thesim.blocks[selectedbaseclassop - thesim.unitops.Count - thesim.streams.Count].controlproperties.Count; i++)
                    {
                        listBox4.Items.Add(thesim.blocks[selectedbaseclassop - thesim.unitops.Count - thesim.streams.Count].controlproperties[i]);
                    }
                    break;
                default:
                    break;
            }
        }

        

        private void button4_Click(object sender, EventArgs e) //Click event for Add CV button
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
                    thenmpc.cvmaster.Add(new mpcvar(thenmpc.mastersim.unitops[selectedbaseclasspv].selectedproperty(selectedpv), aname, 0, 1));
                    break;
                case baseclasstypeenum.Stream:
                    aname = thesim.streams[selectedbaseclasspv - thesim.unitops.Count].name + ": " + (string)listBox2.Items[selectedpv];
                    thenmpc.cvmaster.Add(new mpcvar(thenmpc.mastersim.streams[selectedbaseclasspv - thenmpc.sim1.unitops.Count].selectedproperty(selectedpv), 
                            aname, 0, 1));
                    break;
                default:
                    break;
            }
            thenmpc.cvmaster[thenmpc.cvmaster.Count - 1].target.simvector = new double[global.SimIterations];
            refreshcvlist();
            refreshmaindialogue();
        }

        private void button5_Click(object sender, EventArgs e) //Click event handler for Delete button from CV list.
        {
            for (int i = 0; i < listBox7.Items.Count; i++)
            {
                if (listBox7.GetSelected(i)) { selectedcv = i; }
            }
            if (selectedcv > -1) 
            {
                thenmpc.cvmaster.RemoveAt(selectedcv);
                refreshcvlist();
                refreshmaindialogue();
            }
        }

        private void button8_Click(object sender, EventArgs e) //Click event for "Select File for CV Target data"
        {
            for (int i = 0; i < listBox7.Items.Count; i++)
            {
                if (listBox7.GetSelected(i)) { selectedcv = i; }
            }
            if (selectedcv > -1)
            {
                thenmpc.cvmaster[selectedcv].target.datasource = datasourceforvar.Simulation;
                OpenFileDialog openFileDialog1 = new OpenFileDialog();

                openFileDialog1.InitialDirectory = "c:\\";
                openFileDialog1.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
                openFileDialog1.FilterIndex = 2;
                openFileDialog1.RestoreDirectory = true;

                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    listBox11.Items[selectedcv] = openFileDialog1.FileName;
                    thenmpc.cvmaster[selectedcv].target.excelsource = new exceldataset(openFileDialog1.FileName);
                    if (thenmpc.cvmaster[selectedcv].target.excelsource != null) 
                    { 
                        thenmpc.cvmaster[selectedcv].target.datasource = datasourceforvar.Exceldata;

                        for (int k = 0; k < global.SimIterations; k++)
                        {
                            int j = (k >= thenmpc.cvmaster[selectedcv].target.excelsource.data.Length) ?
                                thenmpc.cvmaster[selectedcv].target.excelsource.data.Length - 1 : k;
                            thenmpc.cvmaster[selectedcv].target.simvector[k] = thenmpc.cvmaster[selectedcv].target.excelsource.data[j];
                        }
                    }
                }



                refreshcvlist();
                refreshmaindialogue();
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e) //CV Object list selected index changed.
        {
            for (int i = 0; i < listBox1.Items.Count; i++)
            {
                if (listBox1.GetSelected(i)) { selectedbaseclasspv = i; }
            }
            if (selectedbaseclasspv < thesim.unitops.Count) { baseclasstypepv = baseclasstypeenum.UnitOp; }
            else { baseclasstypepv = baseclasstypeenum.Stream; }
            refreshpropertylistdialoguepv();
        }

        private void listBox2_SelectedIndexChanged(object sender, EventArgs e) //SelecteedIndexChanged event handler for CV 
                                                                                //Property List.
        {
            for (int i = 0; i < listBox2.Items.Count; i++)
            {
                if (listBox2.GetSelected(i)) { selectedpv = i; }
            }
            
            refreshmaindialogue();
        }

        private void listBox3_SelectedIndexChanged(object sender, EventArgs e)
        //Event handler for SelectedIndexChanged event for Main Object List for OPs
        {
            for (int i = 0; i < listBox3.Items.Count; i++)
            {
                if (listBox3.GetSelected(i)) { selectedbaseclassop = i; }
            }
            if (selectedbaseclassop < thesim.unitops.Count) { baseclasstypeop = baseclasstypeenum.UnitOp; }
            else if (selectedbaseclassop < thesim.unitops.Count + thesim.streams.Count) { baseclasstypeop = baseclasstypeenum.Stream; }
            else { baseclasstypeop = baseclasstypeenum.Block; }
            refreshpropertylistdialogueop();
        }

        private void listBox7_SelectedIndexChanged(object sender, EventArgs e) //Event handler for CV List selected index changed event.
        {
            for (int i = 0; i < listBox7.Items.Count; i++)
            {
                if (listBox7.GetSelected(i)) { selectedcv = i; }
            }

            refreshmaindialogue();
        }

        private void listBox9_SelectedIndexChanged(object sender, EventArgs e) //Event handler for MV (continuous) List selected index changed event.
        {

            for (int i = 0; i < listBox9.Items.Count; i++)
            {
                if (listBox9.GetSelected(i)) { selectedmv = i; }
            }

            refreshmaindialogue();

        }

        private void listBox12_SelectedIndexChanged(object sender, EventArgs e) //event for selected index changed for mv (bool/hybrid) changed.
        {
            for (int i = 0; i < listBox12.Items.Count; i++)
            {
                if (listBox12.GetSelected(i)) { selectedboolmv = i; }
            }

            refreshmaindialogue();
        }

        private void button6_Click(object sender, EventArgs e) //Event handler for Click event of Add MV (continuous) button.
        {
            int streamnr; //The particular unitop/stream nr to be selected and loaded in mvmaster.
            controlvar property; //to check if the particular MV is bool or not.

            string aname;
            for (int i = 0; i < listBox4.Items.Count; i++)
            {
                if (listBox4.GetSelected(i)) { selectedop = i; }
            }

            switch (baseclasstypeop)
            {
                case baseclasstypeenum.UnitOp:
                    aname = thesim.unitops[selectedbaseclassop].name + ": " + (string)listBox4.Items[selectedop];
                    //thenmpc.mvsim0.Add(new mpcvar(thenmpc.sim0.unitops[selectedbaseclassop].selectedproperty(selectedop), 
                    //    aname, 0, 0));
                    //thenmpc.mvsim1.Add(new mpcvar(thenmpc.sim1.unitops[selectedbaseclassop].selectedproperty(selectedop),
                    //    aname, 0, 0));
  
                    property = thenmpc.mastersim.unitops[selectedbaseclassop].selectedproperty(selectedop);
                    if (!property.isbool)
                    {
                        thenmpc.mvmaster.Add(new mpcvar(thenmpc.mastersim.unitops[selectedbaseclassop].selectedproperty(selectedop),
                            aname, 0, 0));
                    }
                    break;
                case baseclasstypeenum.Stream:
                    aname = thesim.streams[selectedbaseclassop - thesim.unitops.Count].name + ": " + 
                        (string)listBox4.Items[selectedop];
                    //thenmpc.mvsim0.Add(new mpcvar(thenmpc.sim0.streams[selectedbaseclassop - 
                    //    thenmpc.sim0.unitops.Count].selectedproperty(selectedop), aname, 0, 0));
                    //thenmpc.mvsim1.Add(new mpcvar(thenmpc.sim1.streams[selectedbaseclassop -
                    //    thenmpc.sim1.unitops.Count].selectedproperty(selectedop), aname, 0, 0));
                    streamnr = selectedbaseclassop - thenmpc.mastersim.unitops.Count;
                    property = thenmpc.mastersim.streams[streamnr].selectedproperty(selectedop);
                    if (!property.isbool)
                    {
                        thenmpc.mvmaster.Add(new mpcvar(thenmpc.mastersim.streams[streamnr].selectedproperty(selectedop), aname, 0, 0));
                    }
                    break;
                case baseclasstypeenum.Block:
                    aname = thesim.blocks[selectedbaseclassop - thesim.unitops.Count - thesim.streams.Count].name + 
                        ": " + (string)listBox4.Items[selectedop];

                    streamnr = selectedbaseclassop - thenmpc.mastersim.unitops.Count - thenmpc.mastersim.streams.Count;
                    property = thenmpc.mastersim.blocks[streamnr].selectedproperty(selectedop);
                    if (!property.isbool)
                    {
                        thenmpc.mvmaster.Add(new mpcvar(thenmpc.mastersim.blocks[streamnr].selectedproperty(selectedop),
                            aname, 0, 0));
                    }
                    break;
                default:
                    break;
            }
            refreshmvlist();
            refreshmaindialogue();
        }

        private void button10_Click(object sender, EventArgs e) //Click event for add MV (boolean/hybrid)
        {
            int streamnr; //The particular unitop/stream nr to be selected and loaded in mvmaster.
            controlvar property; //to check if the particular MV is bool or not.

            string aname;
            for (int i = 0; i < listBox4.Items.Count; i++)
            {
                if (listBox4.GetSelected(i)) { selectedop = i; }
            }

            switch (baseclasstypeop)
            {
                case baseclasstypeenum.UnitOp:
                    aname = thesim.unitops[selectedbaseclassop].name + ": " + (string)listBox4.Items[selectedop];

                    property = thenmpc.mastersim.unitops[selectedbaseclassop].selectedproperty(selectedop);
                    if (property.isbool)
                    {
                        thenmpc.mvboolmaster.Add(new mpcvar(thenmpc.mastersim.unitops[selectedbaseclassop].selectedproperty(selectedop),
                            aname, 0, 0, 0, 1));
                    }
                    break;
                case baseclasstypeenum.Stream:
                    aname = thesim.streams[selectedbaseclassop - thesim.unitops.Count].name + ": " +
                        (string)listBox4.Items[selectedop];

                    streamnr = selectedbaseclassop - thenmpc.mastersim.unitops.Count;
                    property = thenmpc.mastersim.streams[streamnr].selectedproperty(selectedop);
                    if (property.isbool)
                    {
                        thenmpc.mvboolmaster.Add(new mpcvar(thenmpc.mastersim.streams[streamnr].selectedproperty(selectedop), aname, 0, 0, 0, 1));
                    }
                    break;
                default:
                    break;
            }
            refreshmvlist();
            refreshmaindialogue();
        }

        private void button7_Click(object sender, EventArgs e) //Click event handler for Delete MV button
        {
            for (int i = 0; i < listBox9.Items.Count; i++)
            {
                if (listBox9.GetSelected(i)) { selectedmv = i; }
            }
            if (selectedmv > -1)
            {
                //thenmpc.mvsim0.RemoveAt(selectedmv);
                thenmpc.mvmaster.RemoveAt(selectedmv);
                refreshmvlist();
                refreshmaindialogue();
            }
        }

        private void button9_Click(object sender, EventArgs e) //click event for Delete MV (bool/hybrid) button.
        {
            for (int i = 0; i < listBox12.Items.Count; i++)
            {
                if (listBox12.GetSelected(i)) { selectedboolmv = i; }
            }
            if (selectedboolmv > -1)
            {
                thenmpc.mvboolmaster.RemoveAt(selectedboolmv);
                refreshmvlist();
                refreshmaindialogue();
            }
        }

        private void button1_Click(object sender, EventArgs e) //OK Button Click event handler
        {
            readthedialogue();
            Hide();
        }

        private void button2_Click(object sender, EventArgs e) //Update Button Click event handler
        {
            readthedialogue();
            refreshmaindialogue();
        }

        private void button3_Click(object sender, EventArgs e) //Cancel Button Click event handler
        {
            Hide();
        }






        private void label13_Click(object sender, EventArgs e)
        {

        }

        private void textBox10_TextChanged(object sender, EventArgs e)
        {

        }

        

        

        



        

        



        

        

        

        

        

        

        



    }
}
