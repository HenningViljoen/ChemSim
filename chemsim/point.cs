using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace chemsim
{
    [Serializable]
    public class point
    {
        //Properties
        public double x, y;
        public bool highlighted;

        //Methods
        public point(double ax, double ay)
        {
            setxy(ax, ay);
            highlighted = false;
        }

        public void copyfrom(point pointcopyfrom)
        {
            setxy(pointcopyfrom.x, pointcopyfrom.y);
            highlighted = pointcopyfrom.highlighted;
        }


        public void setxy(double ax, double ay)
        {
            x = ax;
            y = ay;
        }

        



    }
}

