using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MotionProfileMapper.VelocityGenerate {
    public abstract class TrajectoryConstraint {
        /**
         * Gets the maximum velocity this constraint allows for a path state.
         *
         * @param state the path state.
         * @return the maximum velocity.
         */
        public virtual double getMaxVelocity(PState state) {
            return Double.PositiveInfinity;
        }

        /**
         * Gets the maximum acceleration this constraint allows for a path state and velocity.
         *
         * @param state    the path state.
         * @param velocity the velocity.
         * @return the maximum acceleration.
         */
        public virtual double getMaxAcceleration(PState state, double velocity) {
            return Double.PositiveInfinity;
        }

        /**
         * Gets the maximum deceleration this constraint allows for a path state and velocity.
         *
         * @param state    the path state.
         * @param velocity the velocity.
         * @return the maximum deceleration.s
         */
        public virtual double getMaxDeceleration(PState state, double velocity) {
            return getMaxAcceleration(state, velocity);
        }
    }

}
