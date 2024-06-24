using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MotionProfileMapper.VelocityGenerate {
    public class CentripetalAccelerationConstraint : TrajectoryConstraint {
        private double maxCentripetalAcceleration;

        /**
         * @param maxCentripetalAcceleration the maximum centripetal acceleration
         */
        public CentripetalAccelerationConstraint(double maxCentripetalAcceleration) {
            this.maxCentripetalAcceleration = maxCentripetalAcceleration;
        }

        override public double getMaxVelocity(PState state) {
            // let A be the centripetal acceleration
            // let V be the max velocity
            // let C be the curvature of the path
            //
            // A = CV^2
            // A / C = V^2
            // sqrt(A / C) = V
            //
            // Curvature and max acceleration is always positive and we only expect a positive result so plus-minus is not
            // needed.

            // Special case when following a line, centripetal acceleration is 0 so don't constrain velocity
            if (state.getRadius() == 0.0) {
                return Double.PositiveInfinity;
            }
            if (state.getRadius() == Double.PositiveInfinity) {
                return Double.PositiveInfinity;
            }

            if (double.IsNaN(state.getRadius())) {
                return Double.PositiveInfinity;
            }

            return Math.Sqrt(( maxCentripetalAcceleration * state.getRadius() ));
        }
    }
}
