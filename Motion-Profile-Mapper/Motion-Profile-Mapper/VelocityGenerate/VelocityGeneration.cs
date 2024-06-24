using MotionProfile.SegmentedProfile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MotionProfileMapper.VelocityGenerate {
    public class VelocityGeneration {
        private ProfilePath path;
        private List<ConstrainedPathState> constrainedPathStates = new List<ConstrainedPathState>();
        private readonly double duration;
        private double[] pathStateStartTimes;

        public VelocityGeneration() {
            duration = 0;
            pathStateStartTimes = null;
        }

        public VelocityGeneration(ProfilePath path, TrajectoryConstraint[] trajectoryConstraints, double sampleDistance,
                      double trajectoryStartingVelocity, double trajectoryEndingVelocity) {
            this.path = path;

            double distance = 0.0;
            ConstrainedPathState lastState = new ConstrainedPathState(
                    path,
                    path.calculate(distance),
                    0.0,
                    0.0,
                    trajectoryStartingVelocity, // Trajectory starting velocity
                    0.0
            );

            while (distance < path.getLength()) {
                PState startingState = path.calculate(distance);


                double profileLength = sampleDistance;
                if (distance + sampleDistance > path.getLength()) {
                    profileLength = path.getLength() - distance;
                }

                PState endingState = path.calculate(distance + profileLength);

                double startingVelocity = lastState.endingVelocity;

                double maxEndingVelocity = Double.PositiveInfinity;

                foreach (TrajectoryConstraint constraint in trajectoryConstraints) {
                    maxEndingVelocity = Math.Min(constraint.getMaxVelocity(endingState), maxEndingVelocity);
                }

                ConstrainedPathState state = new ConstrainedPathState(
                        path,
                        startingState,
                        profileLength,
                        startingVelocity,
                        maxEndingVelocity,
                        0.0
                );

                // If the max ending velocity is lower than the starting velocity we know that we have to decelerate
                double maxDeltaVelocity = maxEndingVelocity - startingVelocity;

                // Calculate the optimal acceleration for this profile
                double optimalAcceleration = Math.Pow(maxDeltaVelocity, 2.0) / ( 2.0 * profileLength ) + ( startingVelocity / profileLength ) * maxDeltaVelocity;
                if (MathUtils.epsilonEquals(optimalAcceleration, 0.0)) {
                    // We are neither accelerating or decelerating
                    state.acceleration = 0.0;
                    state.endingVelocity = state.startingVelocity;
                } else if (optimalAcceleration > 0.0) {
                    // We are accelerating
                    double maxStartingAcceleration = Double.PositiveInfinity;
                    double maxEndingAcceleration = Double.PositiveInfinity;
                    foreach (TrajectoryConstraint constraint in trajectoryConstraints) {
                        maxStartingAcceleration = Math.Min(constraint.getMaxAcceleration(startingState, startingVelocity), maxStartingAcceleration);
                        maxEndingAcceleration = Math.Min(constraint.getMaxAcceleration(endingState, startingVelocity), maxEndingAcceleration); // TODO: Use endingVelocity instead of startingVelocity
                    }

                    // Take the lower of the two accelerations
                    double acceleration = Math.Min(maxStartingAcceleration, maxEndingAcceleration);

                    // Use the optimal acceleration if we can
                    acceleration = Math.Min(acceleration, optimalAcceleration);

                    // Find the maximum velocity we can reach during this profile
                    double[] roots = MathUtils.quadratic(0.5 * acceleration, startingVelocity, -profileLength);
                    double duration1 = Math.Max(roots[0], roots[1]);

                    state.endingVelocity = startingVelocity + acceleration * duration1;
                    state.acceleration = acceleration;
                } else {
                    // If we can decelerate before we reach the end of the profile, use that deceleration.
                    // This acceleration may not be achievable. When we go over the trajectory in reverse we will take care
                    // of this.
                    state.acceleration = optimalAcceleration;
                }

                constrainedPathStates.Add(state);
                lastState = state;

                distance += profileLength;
            }

            for (int i = constrainedPathStates.Count - 1; i >= 0; i--) {
                ConstrainedPathState constrainedState = constrainedPathStates[i];

                constrainedState.endingVelocity = trajectoryEndingVelocity; // Trajectory ending velocity
                if (i != constrainedPathStates.Count - 1) {
                    constrainedState.endingVelocity = constrainedPathStates[i + 1].startingVelocity;
                }

                // Check if we are decelerating
                double deltaVelocity = constrainedState.endingVelocity - constrainedState.startingVelocity;
                if (deltaVelocity < 0.0) {
                    // Use the deceleration constraint for when we decelerate
                    double deceleration = Double.PositiveInfinity;
                    foreach (TrajectoryConstraint constraint in trajectoryConstraints) {
                        deceleration = Math.Min(deceleration, constraint.getMaxDeceleration(constrainedState.pathState, constrainedState.endingVelocity));
                    }

                    // Find how long it takes for us to decelerate to the ending velocity
                    double decelTime = deltaVelocity / -deceleration;

                    // Find how far we travel while decelerating
                    double decelDist = 0.5 * deceleration * Math.Pow(decelTime, 2.0) + constrainedState.endingVelocity * decelTime;

                    // If we travel too far we have to decrease the starting velocity
                    if (decelDist > constrainedState.length) {
                        // We can't decelerate in time. Change the starting velocity of the segment so we can.
                        double[] roots = MathUtils.quadratic(0.5 * deceleration, constrainedState.endingVelocity, -constrainedState.length);

                        // Calculate the maximum time that we can decelerate
                        double maxAllowableDecelTime = Math.Max(roots[0], roots[1]);

                        // Find what are starting velocity can be in order to end at our ending velocity
                        constrainedState.acceleration = -deceleration;
                        constrainedState.startingVelocity = constrainedState.endingVelocity + deceleration * maxAllowableDecelTime;
                    }
                }
            }

            pathStateStartTimes = new double[constrainedPathStates.Count];

            double duration = 0.0;
            for (int i = 0; i < constrainedPathStates.Count; i++) {
                pathStateStartTimes[i] = duration;
                duration += constrainedPathStates[i].getDuration();
            }
            this.duration = duration;
        }

        public State calculate(double time) {
            int start = 0;
            int end = constrainedPathStates.Count - 1;
            int mid = start + ( end - start ) / 2;
            while (start <= end) {
                mid = ( start + end ) / 2;

                if (time > pathStateStartTimes[mid] + constrainedPathStates[mid].getDuration()) {
                    start = mid + 1;
                } else if (time < pathStateStartTimes[mid]) {
                    end = mid - 1;
                } else {
                    break;
                }
            }

            if (mid >= constrainedPathStates.Count) {
                // Out of bounds
                return new State(path.calculate(0.0), 0.0, 0.0);
            }

            ConstrainedPathState constrainedPathState = constrainedPathStates[mid];
            State s = constrainedPathState.calculate(time - pathStateStartTimes[mid]);
            time = MathUtils.clamp(time, 0.0, getDuration());
            s.setTime(time);
            return s;
        }

        public double getDuration() {
            return duration;
        }

        public ProfilePath getPath() {
            return path;
        }

        public String toString() {
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < constrainedPathStates.Count; ++i) {
                builder.Append(i);
                builder.Append(",");
                builder.Append(constrainedPathStates[i].pathState.toString());
                builder.Append("\n");
            }
            return builder.ToString();
        }

    }
}
