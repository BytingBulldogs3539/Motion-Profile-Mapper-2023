using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MotionProfile.ControlPoint;

namespace MotionProfile.Spline
{
    public class SplinePoint
    {
        double x;
        double y;
        public ControlPoint startPoint;
        public ControlPoint endPoint;


        public double X
        {
            get
            {
                return x;
            }

            set
            {
                x = value;
            }
        }

        public double Y
        {
            get
            {
                return y;
            }

            set
            {
                y = value;
            }
        }



        public SplinePoint(double x, double y)
        {
            this.x = x;
            this.y = y;
        }

    }
}
