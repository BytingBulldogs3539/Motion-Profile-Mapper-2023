using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MotionProfileMapper.VelocityGenerate {
    class Path {
    }
    public class PState {
        private double distance;
        private Pose2d pose;

        private Rotation2d heading;
        private double radius;

        public PState(double distance, Pose2d pose, Rotation2d heading, double radius) {
            this.distance = distance;
            this.pose = pose;
            this.heading = heading;
            this.radius = radius;
        }

        public double getDistance() {
            return distance;
        }

        public Pose2d getPose2d() {
            return pose;
        }

        public Rotation2d getHeading() {
            return heading;
        }

        public double getRadius() {
            return radius;
        }


        public String toString() {
            string format = "0.###";
            return "(distance," + getDistance().ToString(format) +
                    ",pose," + getPose2d() +
                    ",heading," + getHeading() +
                    ",curvature," + getRadius().ToString(format) + " )";
        }
    }
}