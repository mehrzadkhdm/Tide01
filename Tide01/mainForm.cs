using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OceanTide;
using PandasNet;
using Tensorflow;

namespace Tide01
{
    public partial class mainForm : Form
    {
        public mainForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            const double RAD = Math.PI / 180.0;
            ForemanTide foremanTide = new ForemanTide(2022,1, 1, 0, 0, 0, 30.0);
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(foremanTide.Constituents.Aggregate("Height", (s, x) => s + "," + $"C_{x.Key},S_{x.Key}"));

            String b;
            List<HourlyHeights> allValues = File.ReadAllLines( Application.StartupPath + "\\Data\\h126.csv")
                                            .Select(v => HourlyHeights.FromCsv(v))
                                            .ToList();

            List<HourlyHeights> values = allValues.Where(v => v.year == 2016 && v.month==1 && v.height > 0).ToList();
            //List<HourlyHeights> values = allValues.Where(v => v.height > 0).ToList();

            double msl = values.Average(x => x.height);

            foreach (HourlyHeights value in values)
            {
                foremanTide = new ForemanTide(value.year, value.month, value.day, value.hour, 0, 0, 30.0);
                //var a = foremanTide.Constituents.Select(x => new {x.Value.name ,Cname = "C" + x.Key, Carg= Math.Cos(RAD * x.Value.argument),
                //    Sarg = Math.Sin(RAD * x.Value.argument), x.Value.nodefactor });
                //var b = foremanTide.Constituents.Aggregate("", (s, n) => s + ",C_" + n.Value.name);
                b = foremanTide.Constituents.Aggregate($"{value.height-msl:F2}", (s, x) => s + "," + $"{Math.Cos(RAD * x.Value.argument)* x.Value.nodefactor :F7}, {Math.Sin(RAD * x.Value.argument) * x.Value.nodefactor:F7}");
                sb.AppendLine(b);
                //foremanTide.Constituents
            }
            File.WriteAllText(Application.StartupPath + "\\Data\\h126_2016_1m.csv", sb.ToString());
            string aa = sb.ToString();
            MessageBox.Show("Done");
        }

        private void mainForm_Load(object sender, EventArgs e)
        {

        }
    }
    public class HourlyHeights
    {
        public int year;
        public int month;
        public int day;
        public int hour;
        public int height;

        public static HourlyHeights FromCsv(string csvLine)
        {
            string[] values = csvLine.Split(',');
            HourlyHeights hourlyHeights = new HourlyHeights();

            hourlyHeights.year = int.Parse(values[0]);
            hourlyHeights.month = int.Parse(values[1]);
            hourlyHeights.day = int.Parse(values[2]);
            hourlyHeights.hour = int.Parse(values[3]);
            hourlyHeights.height = int.Parse(values[4]);
            
            return hourlyHeights;
        }
    }
}
