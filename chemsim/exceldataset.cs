using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Office.Interop.Excel;

namespace chemsim
{
    [Serializable]
    public class exceldataset
    {
        Microsoft.Office.Interop.Excel.Application excelapp;
        public string filename;
        Microsoft.Office.Interop.Excel.Workbook xlWorkbook;
        Microsoft.Office.Interop.Excel._Worksheet xlWorksheet;
        Microsoft.Office.Interop.Excel.Range xlRange;

        public double[] data;
        int arraysize;


        public exceldataset(string afilename)
        {
            double[] tempdata;
            int temparraysize;
            int inbetweendatapoints;
            excelapp = new Microsoft.Office.Interop.Excel.Application();
            //filename = @"C:\myexcel.xlsx";
            filename = afilename;
            xlWorkbook = excelapp.Workbooks.Open(filename);
            xlWorksheet = xlWorkbook.Sheets[1];
            xlRange = xlWorksheet.UsedRange;

            int rowCount = xlRange.Rows.Count;
            int colCount = xlRange.Columns.Count;
            temparraysize = rowCount * colCount;
            inbetweendatapoints = Convert.ToInt32(Math.Ceiling(global.PlantDataSampleT / global.SampleT));
            arraysize = temparraysize * inbetweendatapoints;
            tempdata = new double[temparraysize];
            data = new double[arraysize];


            for (int i = 0; i < rowCount; i++)
            {
                for (int j = 0; j < colCount; j++)
                {
                    tempdata[rowCount * j + i] = Convert.ToDouble(xlRange.Cells[i + 1, j + 1].Value2);
                    //MessageBox.Show(xlRange.Cells[i, j].Value2.ToString());
                }
            }
            for (int i = 0; i < temparraysize; i++)
            {
                for (int j = 0; j < inbetweendatapoints; j++)
                {
                    data[i * inbetweendatapoints + j] = tempdata[i];

                }
            }
        }

        
    }
}
