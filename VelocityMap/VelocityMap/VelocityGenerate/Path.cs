using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VelocityMap.VelocityGenerate
{
    class Path
    {
    }
    public class PState
    {
        private double distance;
        private Pose2d pose;

        private Rotation2d heading;
        private double curvature;

        public PState(double distance, Pose2d pose, Rotation2d heading, double curvature)
        {
            this.distance = distance;
            this.pose = pose;
            this.heading = heading;
            this.curvature = curvature;
        }

        public double getDistance()
        {
            return distance;
        }

        public Pose2d getPose2d()
        {
            return pose;
        }

        public Rotation2d getHeading()
        {
            return heading;
        }

        public double getCurvature()
        {
            return curvature;
        }


        public String toString()
        {
            string format = "0.###";
            return "(distance," + getDistance().ToString(format) +
                    ",pose," + getPose2d() +
                    ",heading," + getHeading() +
                    ",curvature," + getCurvature().ToString(format) + " )";
        }
    }
}