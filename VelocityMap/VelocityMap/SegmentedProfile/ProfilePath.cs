using MotionProfile.Spline;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VelocityMap.VelocityGenerate;

namespace MotionProfile.SegmentedProfile
{
    public class ProfilePath
    {
        private string name;
        public List<ControlPoint> controlPoints;
        public string id;

        public bool snapToPrevious = VelocityMap.Properties.Settings.Default.SnapNewPaths;
        public double maxVel = VelocityMap.Properties.Settings.Default.MaxVel;
        public double maxAcc = VelocityMap.Properties.Settings.Default.MaxAcc;
        public double maxCen = VelocityMap.Properties.Settings.Default.MaxCen;
        public bool isSpline = false;
        public SplinePath path = new SplinePath();


        /// <summary>
        /// Creates a new blank profile path
        /// </summary>
        public ProfilePath(string name, bool isSpline, ProfilePath previousPath = null)
        {
            this.name = name;
            this.controlPoints = new List<ControlPoint>();
            this.id = Guid.NewGuid().ToString();
            this.isSpline = isSpline;

            if (this.snapToPrevious) this.snap(previousPath);
        }

        /// <summary>
        /// Loads a profile path from a path JSON representation
        /// </summary>
        /// <param name="pathJSON">JSON-formatted path object</param>
        public ProfilePath(JObject pathJSON)
        {
            this.name = (string)pathJSON["name"];
            this.id = (string)pathJSON["id"];
            this.maxVel = (double)pathJSON["maxVelocity"];
            this.maxAcc = (double)pathJSON["maxAcceleration"];
            this.isSpline = (bool)pathJSON["isSpline"];
            this.snapToPrevious = pathJSON.ContainsKey("snapToPrevious") ?
                (bool)pathJSON["snapToPrevious"] : false;

            this.controlPoints = new List<ControlPoint>();

            foreach (JObject point in pathJSON["points"])
            {
                this.controlPoints.Add(new ControlPoint(point));
            }
        }

        public ProfilePath(ProfilePath other)
        {
            this.name = other.name;
            this.isSpline = other.isSpline;
            this.id = Guid.NewGuid().ToString();
            this.controlPoints = new List<ControlPoint>();
            this.snapToPrevious = other.snapToPrevious;

            foreach (ControlPoint point in other.controlPoints)
            {
                this.controlPoints.Add(new ControlPoint(point));
            }
        }

        public ControlPoint addControlPoint(int x, int y, int heading)
        {
            ControlPoint newPoint = new ControlPoint(x, y, heading);
            this.controlPoints.Add(newPoint);
            return newPoint;
        }

        public ControlPoint addControlPoint(ControlPoint other)
        {
            ControlPoint newPoint = new ControlPoint(other);
            this.controlPoints.Add(newPoint);
            return newPoint;
        }

        public void deleteControlPoint(int index)
        {
            this.controlPoints.RemoveAt(index);
            if (this.controlPoints.Count == 0) this.snapToPrevious = false;
        }

        public List<string> pointIds()
        {
            List<string> ids = new List<string>();
            foreach (ControlPoint point in this.controlPoints)
            {
                ids.Add(point.Id);
            }
            return ids;
        }

        public void mirrorPoints(double fieldWidth)
        {
            foreach (ControlPoint point in this.controlPoints)
            {
                point.X = fieldWidth - point.X;
            }
        }

        public void shiftPoints(double dx, double dy)
        {
            foreach (ControlPoint point in this.controlPoints)
            {
                point.X += dx;
                point.Y += dy;
            }
        }

        public void clearPoints()
        {
            this.controlPoints.Clear();
        }

        public void snap(ProfilePath previous)
        {
            if (previous == null || previous.controlPoints.Count == 0)
            {
                this.snapToPrevious = false;
                return;
            }

            this.snapToPrevious = true;
            if (this.controlPoints.Count == 0) this.controlPoints.Add(new ControlPoint(previous.controlPoints.Last()));
            else this.controlPoints[0] = new ControlPoint(previous.controlPoints.Last());
        }

        public void snapLast(ControlPoint point)
        {
            if (this.controlPoints.Count == 0) return;

            this.controlPoints[this.controlPoints.Count - 1] = new ControlPoint(point);
        }

        public bool isEmpty()
        {
            return this.controlPoints.Count == 0;
        }
        private List<State> pointList = new List<State>();
        private double length = 0.0;

        public void generate()
        {
            if (isSpline)
            {
                pointList.Clear();
                path.GenSpline(this.controlPoints);
                length = path.getLength();


                TrajectoryConstraint[] constraints = {  new MaxAccelerationConstraint(this.maxAcc)as TrajectoryConstraint,
                new MaxVelocityConstraint(this.maxVel)as TrajectoryConstraint, new CentripetalAccelerationConstraint(this.maxCen) as TrajectoryConstraint };

                VelocityGeneration gen = new VelocityGeneration(this, constraints, .01, 0, 0);

                for (double time = 0; time < gen.getDuration(); time += .05)
                {
                    State s = gen.calculate(time);

                    pointList.Add(s);

                }

                //Console.WriteLine(path.getLength());

                //path.getControlPointDistances();

            }
            else
            {
                length = 0;
                for (int i = 0; i < controlPoints.Count - 1; i++)
                {
                    ControlPoint p1 = controlPoints[i];
                    ControlPoint p2 = controlPoints[i + 1];
                    length += Math.Sqrt(Math.Pow(p2.X - p1.X, 2) + Math.Pow(p2.Y - p1.Y, 2));

                }
                for (int i = 0; i < controlPoints.Count; i++)
                {
                    ControlPoint p1 = controlPoints[i];
                    //pointList.Add(new SplinePoint(p1.X, p1.Y, i));
                }
            }
        }

        public PState calculate(double distance)
        {
            SplinePoint p = path.calculate(distance);

            double distance1 = 0.0;
            double distance2 = 0.0;
            double sampleDistance = .02;
            SplinePoint p1 = null;
            SplinePoint p2 = null;
            SplinePoint p3 = null;

            if (distance > sampleDistance)
            {
                distance1 = distance - sampleDistance;
                distance2 = distance + sampleDistance;
                p1 = path.calculate(distance1);
                p2 = path.calculate(distance);
                p3 = path.calculate(distance2);

            }
            else
            {
                distance1 = distance + sampleDistance;
                distance2 = distance + sampleDistance * 2;
                p1 = path.calculate(distance);
                p2 = path.calculate(distance1);
                p3 = path.calculate(distance2);
            }

            double r = circleFromPoints(p1, p2, p3);

            return new PState(distance, new Pose2d(p.X, p.Y, Rotation2d.fromDegrees(0)), Rotation2d.fromDegrees(0), r);
        }

        static double TOL = 0.0000001;
        public double circleFromPoints(SplinePoint p1, SplinePoint p2, SplinePoint p3)
        {
            double offset = Math.Pow(p2.X, 2) + Math.Pow(p2.Y, 2);
            double bc = (Math.Pow(p1.X, 2) + Math.Pow(p1.Y, 2) - offset) / 2.0;
            double cd = (offset - Math.Pow(p3.X, 2) - Math.Pow(p3.Y, 2)) / 2.0;
            double det = (p1.X - p2.X) * (p2.Y - p3.Y) - (p2.X - p3.X) * (p1.Y - p2.Y);

            if (Math.Abs(det) < TOL) { return double.PositiveInfinity; }

            double idet = 1 / det;

            double centerx = (bc * (p2.Y - p3.Y) - cd * (p1.Y - p2.Y)) * idet;
            double centery = (cd * (p1.X - p2.X) - bc * (p2.X - p3.X)) * idet;
            double radius =
               Math.Sqrt(Math.Pow(p2.X - centerx, 2) + Math.Pow(p2.Y - centery, 2));

            return radius;
        }

        public double getLength()
        {
            return length;
        }

        public List<State> getPoints()
        {
            return pointList;
        }

        private double distance(SplinePoint p1, SplinePoint p2)
        {
            return Math.Sqrt(Math.Pow(p2.X - p1.X, 2) + Math.Pow(p2.Y - p1.Y, 2));
        }

        public JObject toJSON()
        {
            JObject pathJSON = new JObject();
            pathJSON["name"] = this.name;
            pathJSON["id"] = this.id;
            pathJSON["isSpline"] = this.isSpline;
            pathJSON["maxVelocity"] = this.maxVel;
            pathJSON["maxAcceleration"] = this.maxAcc;
            pathJSON["snapToPrevious"] = this.snapToPrevious;

            JArray pointsJSON = new JArray();
            foreach (ControlPoint point in this.controlPoints)
            {
                pointsJSON.Add(point.toJSON());
            }
            pathJSON["points"] = pointsJSON;

            return pathJSON;
        }

        public string toJava()
        {
            string path = "\t\t{\n";
            List<string> pointStrings = new List<string>();
            foreach (ControlPoint point in this.controlPoints)
            {
                pointStrings.Add(point.toJava());
            }
            path += String.Join(",\n", pointStrings) + "\n";
            path += "\t\t}";
            return path;
        }

        public string toTxt()
        {
            string pathTxt = $"{this.maxVel} {this.maxAcc}\n";
            foreach (ControlPoint point in this.controlPoints)
            {
                pathTxt += point.toTxt();
            }
            return pathTxt + "@@@";
        }

        public string Name
        {
            get
            {
                return this.name;
            }
            set
            {
                if (value.Trim() == "") return;
                this.name = value.Trim();
            }
        }
    }
}
