using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace chemsim
{
    [Serializable()]
    public class simtimer
    {
        //Properties
        public double days;
        public double hours;
        public double minutes;
        public double seconds;
        public double totalseconds;

        //Methods
        public simtimer(double adays, double ahours, double aminutes, double aseconds)
        {
            days = adays;
            hours = ahours;
            minutes = aminutes;
            seconds = aseconds;
            totalseconds = 0;
        }

        public void reset()
        {
            days = 0;
            hours = 0;
            minutes = 0;
            seconds = 0;
            totalseconds = 0;
        }

        public void tick()
        {
            //Assume SampleT is in seconds
            double remainder;

            totalseconds += global.SampleT;
            days = Math.Floor(totalseconds / (3600 * 24));
            remainder = totalseconds % (3600 * 24);
            hours = Math.Floor(remainder / (3600));
            remainder = remainder % 3600;
            minutes = Math.Floor(remainder / 60);
            seconds = remainder % 60;



        }

        public String timerstring()
        {

            return String.Concat("Days: ", days.ToString(), "  Time: ",
                String.Concat(hours.ToString(), ":", minutes.ToString(),
                String.Concat(":", seconds.ToString())));
        }

        public static bool operator ==(simtimer simtimer1, simtimer simtimer2)
        {
            return (simtimer1.hours == simtimer2.hours &&
                simtimer1.minutes == simtimer2.minutes && simtimer1.seconds == simtimer2.seconds);
        }

        public static bool operator !=(simtimer simtimer1, simtimer simtimer2)
        {
            return !(simtimer1.hours == simtimer2.hours &&
                simtimer1.minutes == simtimer2.minutes && simtimer1.seconds == simtimer2.seconds);
        }

        public static bool operator <(simtimer simtimer1, simtimer simtimer2)
        {
            return (3600 * simtimer1.hours + 60 * simtimer1.minutes + simtimer1.seconds <
                3600 * simtimer2.hours + 60 * simtimer2.minutes + simtimer2.seconds);
        }

        public static bool operator >(simtimer simtimer1, simtimer simtimer2)
        {
            return (3600 * simtimer1.hours + 60 * simtimer1.minutes + simtimer1.seconds >
                3600 * simtimer2.hours + 60 * simtimer2.minutes + simtimer2.seconds);
        }

        public static bool operator >=(simtimer simtimer1, simtimer simtimer2)
        {
            return (3600 * simtimer1.hours + 60 * simtimer1.minutes + simtimer1.seconds >=
                3600 * simtimer2.hours + 60 * simtimer2.minutes + simtimer2.seconds);
        }

        public static bool operator <=(simtimer simtimer1, simtimer simtimer2)
        {
            return (3600 * simtimer1.hours + 60 * simtimer1.minutes + simtimer1.seconds <=
                3600 * simtimer2.hours + 60 * simtimer2.minutes + simtimer2.seconds);
        }



    }
}
