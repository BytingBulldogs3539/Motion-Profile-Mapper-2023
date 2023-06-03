using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VelocityMap.VelocityGenerate
{
    /**
 * A constraint that limits the acceleration.
 */
    public class MaxAccelerationConstraint : TrajectoryConstraint
    {
        private double maxAcceleration;
        private double maxDeceleration;

        public MaxAccelerationConstraint(double maxAbsAcceleration)
        {
            this.maxAcceleration = maxAbsAcceleration;
            this.maxDeceleration = maxAbsAcceleration;
        }

        public MaxAccelerationConstraint(double maxAcceleration, double maxDeceleration)
        {
            this.maxAcceleration = maxAcceleration;
            this.maxDeceleration = maxDeceleration;
        }


        override public double getMaxAcceleration(PState state, double velocity)
        {
            return maxAcceleration;
        }

        override public double getMaxDeceleration(PState state, double velocity)
        {
            return maxDeceleration;
        }
    }
}
