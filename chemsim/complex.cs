using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chemsim
{
    [Serializable]
    public class complex
    {
        public double a; //real component.
        public double b; //imaginary component.
        public static complex I = new complex(0, 1);

        public complex(double aa, double ab)
        {
            a = aa;
            b = ab;
        }

        public complex()
        {
            a = 0;
            b = 0;
        }

        public static complex c(double aa)
        {
            return new complex(aa, 0);
        }

        public static double rad(complex c)
        {
            return Math.Sqrt(c.a * c.a + c.b * c.b);
        }

        public static double alpha(complex c)
        {
            if (c.b == 0 && c.a < 0) { return -Math.PI; }
            else if (c.a == 0 && c.b > 0) { return Math.PI / 2.0; }
            else if (c.a == 0 && c.b < 0) { return -Math.PI / 2.0; }
            else { return Math.Atan(c.b / c.a); }
        }

        public static complex operator +(complex c1, complex c2)
        {
            return new complex(c1.a + c2.a, c1.b + c2.b);
        }

        public static complex operator +(double d, complex c)
        {
            return new complex(d + c.a, c.b);
        }


        public static complex operator +(complex c, double d)
        {
            return new complex(d + c.a, c.b);
        }

        public static complex operator -(complex c1, complex c2)
        {
            return new complex(c1.a - c2.a, c1.b - c2.b);
        }

        public static complex operator -(double d, complex c)
        {
            return new complex(d - c.a, -c.b);
        }

        public static complex operator -(complex c, double d)
        {
            return new complex(c.a - d, c.b);
        }

        public static complex operator *(complex c1, complex c2)
        {
            return new complex(c1.a * c2.a - c1.b * c2.b, c1.b * c2.a + c1.a * c2.b);
            //(a+bi) (c+di) = (ac-bd) + (bc+ad)i.\ 
        }

        public static complex operator *(double d, complex c)
        {
            return new complex(d * c.a, d * c.b);
        }

        public static complex operator *(complex c, double d)
        {
            return new complex(d * c.a, d * c.b);
        }

        public static complex pow(complex x, double y, int n = 0) //the specific root that is looked for will be specified here.  Could be a number between 0 and 1/y.  
        //This is assuming that y is smaller than 1.
        {
            double r = rad(x);
            double al = alpha(x);
            double newr = Math.Pow(r, y);
            double newalpha;
            if (y < 1)
            {
                newalpha = al * y + 2.0 * Math.PI * y * n;
            }
            else
            {
                newalpha = al * y;
            }

            return new complex(newr * Math.Cos(newalpha), newr * Math.Sin(newalpha));
        }

        public static complex pow(double x, double y)
        {
            if (x < 0) { return new complex(0, Math.Pow(-x, y)); }
            else { return new complex(Math.Pow(x, y), 0); }
        }

        public static complex conj(complex c)
        {
            return new complex(c.a, -c.b);
        }

        public static double realconj(complex c)
        {
            return c.a * c.a + c.b * c.b;
        }

        public static complex operator /(complex c1, complex c2)
        {
            complex num = c1 * conj(c2);
            double den;

            den = realconj(c2);
            return new complex(num.a / den, num.b / den);
        }

        public static complex operator /(complex c, double d)
        {
            return new complex(c.a / d, c.b / d);
        }

        public static complex operator /(double d, complex c)
        {
            complex num = d * conj(c);
            double den;

            den = realconj(c);
            return new complex(num.a / den, num.b / den);
        }
    }
}
