using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;


namespace chemsim
{
    public class embeddedtrend : baseclass
    {
        public double radius;
        public double height;
        public PictureBox pbox;

        public embeddedtrend(double ax, double ay, double[][] xdata, double[][] ydata,
                        String tagname, Control aparent)
            : base(0, ax, ay)
        {
            pbox = new PictureBox();
            pbox.Parent = aparent;
            pbox.Location = new Point(global.OriginX + Convert.ToInt32(global.GScale * (location.x - global.EmbeddedTrendWidth / 2)),
                                global.OriginY + Convert.ToInt32(global.GScale * (location.y - global.EmbeddedTrendHeight)));
            pbox.Width = Convert.ToInt32(global.GScale * global.EmbeddedTrendWidth);
            pbox.Height = Convert.ToInt32(global.GScale * global.EmbeddedTrendHeight);
            trend thetrend = new trend(pbox);
            thetrend.plot(xdata, ydata, tagname, false, false, true);
        }
    }
}
