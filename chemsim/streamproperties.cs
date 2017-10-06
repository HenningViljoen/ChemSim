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
    public partial class streamproperties : Form
    {
        private stream thestream;
        private simulation thesim;

        public streamproperties(stream astream, simulation asim)
        {
            InitializeComponent();

            radioButton1.Checked = true;

            thestream = astream;
            thesim = asim;
            Text = String.Concat("Properties for stream number: ", thestream.nr.ToString());

            refreshdialogue();
        }

        private void refreshdialogue()
        {
            textBox10.Text = thestream.name;
            textBox3.Text = utilities.pascal2barg(thestream.mat.P.v).ToString("G5");
            textBox7.Text = utilities.kelvin2celcius(thestream.mat.T.v).ToString("G5");
            textBox18.Text = thestream.mat.relativehumidity.v.ToString("G5");
            textBox11.Text = thestream.mat.V.v.ToString("G5");
            textBox12.Text = thestream.mat.f.v.ToString("G5");
            textBox8.Text = thestream.mat.density.v.ToString("G5");
            textBox14.Text = thestream.mat.massofonemole.ToString("G5");
            textBox1.Text = utilities.fps2fph(thestream.actualvolumeflow.v).ToString("G5");
            textBox6.Text = utilities.fps2fph(thestream.standardvolumeflow.v).ToString("G5");
            textBox2.Text = utilities.fps2fph(thestream.massflow.v).ToString("G5");
            textBox9.Text = thestream.molarflow.v.ToString("G5");
            textBox13.Text = thestream.mat.totalCp.ToString("G5");            
            textBox4.Text = thestream.direction.ToString("G5");
            textBox5.Text = thestream.distance.ToString("G5");

            double cpJpkgK = thestream.mat.totalCp / thestream.mat.massofonemole;
            textBox20.Text = cpJpkgK.ToString("G5");

            if (thestream.mat.T.datasource == datasourceforvar.Exceldata) { textBox17.Text = thestream.mat.T.excelsource.filename; };

            if (thestream.massflow.datasource == datasourceforvar.Exceldata) { textBox15.Text = thestream.massflow.excelsource.filename; };

            refreshcomponentlist();
        }

        private void refreshcomponentlist()
        {
            while (dataGridView1.Rows.Count < global.fluidpackage.Count)
            {
                DataGridViewRow row = (DataGridViewRow)dataGridView1.Rows[0].Clone();
                dataGridView1.Rows.Add(row);
            }

            double totalmolefraction = 0.0;

            for (int i = 0; i < global.fluidpackage.Count; i++)
            {
                DataGridViewRow row = (DataGridViewRow)dataGridView1.Rows[i];
                row.Cells[0].Value = thestream.mat.composition[i].m.name;
                row.Cells[1].Value = thestream.mat.composition[i].molefraction;
                totalmolefraction += thestream.mat.composition[i].molefraction;          
            }

            textBox16.Text = totalmolefraction.ToString();
        }

        private void label5_Click(object sender, EventArgs e) //Double click the Standard flow label for trending
        {
            trendvar thetrendvar = new trendvar(
                utilities.adddimension(global.simtimevector),
                utilities.adddimension(utilities.vectorprocessor(thestream.standardvolumeflow.simvector, utilities.fps2fph)),
                "Standard volume flow (Nm^3/h)");
            thetrendvar.Show();
        }

        private void label9_Click(object sender, EventArgs e) //Double click the Mass flow label for trending
        {
            trendvar thetrendvar = new trendvar(
                utilities.adddimension(global.simtimevector),
                utilities.adddimension(utilities.vectorprocessor(thestream.massflow.simvector, utilities.fps2fph)),
                "Mass flow (kg/h)");
            thetrendvar.Show();
        }

        private void label1_Click(object sender, EventArgs e) //Double click the Pressure label for trending
        {
            trendvar thetrendvar = new trendvar(utilities.adddimension(global.simtimevector),
                utilities.adddimension(utilities.vectorprocessor(thestream.mat.P.simvector, utilities.pascal2barg)),
                "Pressure (Barg)");
            thetrendvar.Show();
        }

        private void label14_Click(object sender, EventArgs e) //Click of Temperature label
        {
            trendvar thetrendvar = new trendvar(utilities.adddimension(global.simtimevector),
                utilities.adddimension(utilities.vectorprocessor(thestream.mat.T.simvector, utilities.kelvin2celcius)),
                "Temperature (deg C)");
            thetrendvar.Show();
        }

        private void readthedialogue()
        {
            if (radioButton1.Checked)
            {
                thestream.massflow.v = utilities.fph2fps(Convert.ToDouble(textBox2.Text));
            }
            else
            {
                thestream.standardvolumeflow.v = utilities.fph2fps(Convert.ToDouble(textBox6.Text));
                //thestream.calcmassflowfromstandardflow();
            }
            thestream.name = textBox10.Text;


            thestream.mat.P.v = utilities.barg2pascal(Convert.ToDouble(textBox3.Text));
            thestream.mat.T.v = utilities.celcius2kelvin(Convert.ToDouble(textBox7.Text));
            thestream.mat.relativehumidity.v = Convert.ToDouble(textBox18.Text);
            thestream.mat.V.v = Convert.ToDouble(textBox11.Text);
            thestream.mat.f.v = Convert.ToDouble(textBox12.Text);

            

            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                thestream.mat.composition[i].molefraction = Convert.ToDouble(dataGridView1.Rows[i].Cells[1].Value);

            }
        }

        private void updatethestreamwithflash()
        {
            readthedialogue();

            //List<component> inputcomposition = new List<component>();

            //material.copycompositiontothiscomposition(ref inputcomposition, global.fluidpackage);

            thestream.mat.PTfVflash(thestream.mat.T.v, thestream.mat.V.v,
                thestream.mat.P.v, thestream.mat.f.v);
            //thestream.update(thesim.simi);
            thestream.calcmolarflowfrommassflow();
            thestream.calcactualvolumeflowfrommassflow();
            thestream.calcstandardflowfrommoleflow();

            
        }

        private void updatethestreamwithnoflash() //reads various fields manually without flashing
        {
            readthedialogue();

            thestream.mat.density.v = Convert.ToDouble(textBox8.Text);
            thestream.mat.massofonemole = Convert.ToDouble(textBox14.Text);
            thestream.mat.totalCp = Convert.ToDouble(textBox13.Text);

            thestream.calcmolarflowfrommassflow();
            thestream.calcactualvolumeflowfrommassflow();
            thestream.calcstandardflowfrommoleflow();
        }

        private void button1_Click(object sender, EventArgs e) //OK button is clicked at the end of the session
        //on the dialogue
        {
            updatethestreamwithflash();
            refreshdialogue();
            Hide();
        }

        private void button2_Click(object sender, EventArgs e) //Update button updates the steam and flashes, does not close dialogue.
        {
            updatethestreamwithflash();
            refreshdialogue();
        }

        private void button3_Click(object sender, EventArgs e) //Cancel button closes the dialogue without updating.
        {
            Hide();
        }

        private void label26_Click(object sender, EventArgs e)
        {

        }

        private void button5_Click(object sender, EventArgs e) //Select File button for temperature for data to be imported.
        {
            thestream.mat.T.datasource = datasourceforvar.Simulation;
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            openFileDialog1.InitialDirectory = "c:\\";
            openFileDialog1.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            openFileDialog1.FilterIndex = 2;
            openFileDialog1.RestoreDirectory = true;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                textBox17.Text = openFileDialog1.FileName;
                thestream.mat.T.excelsource = new exceldataset(textBox17.Text);
                if (thestream.mat.T.excelsource != null) { thestream.mat.T.datasource = datasourceforvar.Exceldata; }
            }
        }

        private void button7_Click(object sender, EventArgs e) //Select File button for relative humidty for data to be imported.
        {
            thestream.mat.relativehumidity.datasource = datasourceforvar.Simulation;
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            openFileDialog1.InitialDirectory = "c:\\";
            openFileDialog1.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            openFileDialog1.FilterIndex = 2;
            openFileDialog1.RestoreDirectory = true;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                textBox19.Text = openFileDialog1.FileName;
                thestream.mat.relativehumidity.excelsource = new exceldataset(textBox19.Text);
                if (thestream.mat.relativehumidity.excelsource != null) { thestream.mat.relativehumidity.datasource = datasourceforvar.Exceldata; }
            }
        }

        private void button4_Click(object sender, EventArgs e) //Select File button for mass flow for data to be imported.
        {
            thestream.massflow.datasource = datasourceforvar.Simulation;
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            openFileDialog1.InitialDirectory = "c:\\";
            openFileDialog1.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            openFileDialog1.FilterIndex = 2;
            openFileDialog1.RestoreDirectory = true;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                textBox15.Text = openFileDialog1.FileName;
                thestream.massflow.excelsource = new exceldataset(textBox15.Text);
                if (thestream.massflow.excelsource != null) { thestream.massflow.datasource = datasourceforvar.Exceldata; }
            }
        }

        

        private void button6_Click(object sender, EventArgs e) //Update button updates the stream manually without flashing, does not close dialogue.
        {
            updatethestreamwithnoflash();
            refreshdialogue();
        }

        private void button8_Click(object sender, EventArgs e) //Click event for halfing mass flow.
        {
            thestream.massflow *= 0.5;
            refreshdialogue();
        }

        private void button9_Click(object sender, EventArgs e) //Click event for doubling mass flow.
        {
            thestream.massflow *= 2.0;
            refreshdialogue();
        }

        

        




    }
}
