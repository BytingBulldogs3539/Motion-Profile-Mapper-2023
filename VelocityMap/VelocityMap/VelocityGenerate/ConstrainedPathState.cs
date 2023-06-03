using MotionProfile.SegmentedProfile;
using MotionProfile.Spline;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VelocityMap.VelocityGenerate
{
    class ConstrainedPathState
    {
        public PState pathState;
        public double length;
        public double startingVelocity;
        public double endingVelocity;
        public double acceleration;
        public ProfilePath path;

        public ConstrainedPathState(ProfilePath path, PState pathState, double length, double startingVelocity, double endingVelocity, double acceleration)
        {
            this.path = path;
            this.pathState = pathState;
            this.length = length;
            this.startingVelocity = startingVelocity;
            this.endingVelocity = endingVelocity;
            this.acceleration = acceleration;
        }

        public double getDuration()
        {
            if (MathUtils.epsilonEquals(acceleration, 0.0))
            {
                return length / startingVelocity;
            }

            if (MathUtils.epsilonEquals(endingVelocity, 0.0))
            {
                return (startingVelocity / -acceleration);
            }

            double[] roots = MathUtils.quadratic(0.5 * acceleration, startingVelocity, -length);

            if (acceleration > 0.0)
            {
                return Math.Max(roots[0], roots[1]);
            }
            else
            {
                return Math.Min(roots[0], roots[1]);
            }
        }

        public State calculate(double time)
        {
            time = MathUtils.clamp(time, 0.0, getDuration());

            double distance = 0.5 * acceleration * Math.Pow(time, 2.0) + startingVelocity * time + pathState.getDistance();

            return new State(
                    path.calculate(distance),
                    acceleration * time + startingVelocity,
                    acceleration
            );
        }


    }
    public class State
    {
        private PState pathState;
        private double velocity;
        private double acceleration;
        private double time;

        public State(PState pathState, double velocity, double acceleration)
        {
            this.pathState = pathState;
            this.velocity = velocity;
            this.acceleration = acceleration;
        }

        public void setTime(double time)
        {
            this.time = time;
        }

        public PState getPathState()
        {
            return pathState;
        }

        public double getVelocity()
        {
            return velocity;
        }

        public double getAcceleration()
        {
            return acceleration;
        }
        public double getTime()
        {
            return time;
        }
    }
}
