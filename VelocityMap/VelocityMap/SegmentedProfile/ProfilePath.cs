using MathNet.Numerics;
using MathNet.Numerics.Interpolation;
using MotionProfile.Spline;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VelocityMap;
using VelocityMap.VelocityGenerate;

namespace MotionProfile.SegmentedProfile
{
    public class ProfilePath
    {
        private string name;
        private List<ControlPoint> controlPoints;
        private string id;

        private bool snapToPrevious = VelocityMap.Properties.Settings.Default.SnapNewPaths;
        private double maxVel = VelocityMap.Properties.Settings.Default.MaxVel;
        private double maxAcc = VelocityMap.Properties.Settings.Default.MaxAcc;
        private double maxCen = VelocityMap.Properties.Settings.Default.MaxCen;
        List<double> cpdistances = new List<double>();
        private bool isSpline = false;
        public SplinePath path = new SplinePath();
        public VelocityGeneration gen = null;

        public IInterpolation xsMap = null;
        public IInterpolation ysMap = null;

        private Profile profile;


        /// <summary>
        /// Creates a new blank profile path
        /// </summary>
        public ProfilePath(Profile profile, string name, bool isSpline, ProfilePath previousPath = null)
        {
            this.profile = profile;
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
        public ProfilePath(JObject pathJSON, Profile profile)
        {
            this.profile = profile;
            this.name = (string)pathJSON["name"];
            this.id = (string)pathJSON["id"];
            this.maxVel = (double)pathJSON["maxVelocity"];
            this.maxAcc = (double)pathJSON["maxAcceleration"];
            try
            {
                this.maxCen = (double)pathJSON["maxCentripetalAcceleration"];
            }
            catch
            {

            }
            try
            {
                this.isSpline = (bool)pathJSON["isSpline"];
            }
            catch
            {

            }
            this.snapToPrevious = pathJSON.ContainsKey("snapToPrevious") ?
                (bool)pathJSON["snapToPrevious"] : false;

            this.controlPoints = new List<ControlPoint>();

            foreach (JObject point in pathJSON["points"])
            {
                this.controlPoints.Add(new ControlPoint(point, this));
            }
        }

        public ProfilePath(ProfilePath other, Profile profile)
        {
            this.profile = profile;
            this.name = other.name;
            this.isSpline = other.isSpline;
            this.id = Guid.NewGuid().ToString();
            this.controlPoints = new List<ControlPoint>();
            this.snapToPrevious = other.snapToPrevious;
            this.maxVel = other.maxVel;
            this.maxAcc = other.maxAcc;
            this.maxCen = other.maxCen;

            foreach (ControlPoint point in other.controlPoints)
            {
                this.controlPoints.Add(new ControlPoint(point, this));
            }
        }
        //Call before edit.
        public void newEdit()
        {
            profile.newEdit();
        }

        public ControlPoint addControlPoint(int x, int y, int heading)
        {
            newEdit();
            ControlPoint newPoint = new ControlPoint(this, x, y, heading);
            this.controlPoints.Add(newPoint);
            return newPoint;
        }

        public ControlPoint addControlPoint(ControlPoint other)
        {
            newEdit();
            ControlPoint newPoint = new ControlPoint(other, this);
            this.controlPoints.Add(newPoint);
            return newPoint;
        }

        public void deleteControlPoint(int index)
        {
            newEdit();
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
            newEdit();
            foreach (ControlPoint point in this.controlPoints)
            {
                point.X = fieldWidth - point.X;
            }
        }

        public void shiftPoints(double dx, double dy)
        {
            newEdit();
            foreach (ControlPoint point in this.controlPoints)
            {
                point.X += dx;
                point.Y += dy;
            }
        }
        public void reverse()
        {
            newEdit();
            this.controlPoints.Reverse();
        }

        public void clearPoints()
        {
            newEdit();
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
            if (this.controlPoints.Count == 0) this.controlPoints.Add(new ControlPoint(previous.controlPoints.Last(), this));
            else this.controlPoints[0] = new ControlPoint(previous.controlPoints.Last(), this);
        }

        public void snapLast(ControlPoint point)
        {
            if (this.controlPoints.Count == 0) return;

            this.controlPoints[this.controlPoints.Count - 1] = new ControlPoint(point, this);
        }

        public bool isEmpty()
        {
            if (length == 0.0)
                return true;

            return this.controlPoints.Count <= 1;
        }


        private List<State> pointList = new List<State>();
        private double length = 0.0;


        public void generate(bool quickGen = false)
        {
            double timeSample = .05;
            double sampleDistance = 0.05;
            if (quickGen)
            {
                timeSample = 0.1;
                sampleDistance = 0.2;
            }

            if (controlPoints.Count < 2)
                return;

            if (isSpline)
            {
                pointList.Clear();
                path.GenSpline(this.controlPoints);
                length = path.getLength();

                cpdistances = path.getControlPointDistances();

                TrajectoryConstraint[] constraints = { new MaxAccelerationConstraint(this.maxAcc), new MaxVelocityConstraint(this.maxVel), new CentripetalAccelerationConstraint(this.maxCen) };

                gen = new VelocityGeneration(this, constraints, sampleDistance, 0, 0);


                for (double time = 0; time < gen.getDuration(); time += timeSample)
                {
                    State s = gen.calculate(time);

                    pointList.Add(s);

                }

            }
            else
            {
                pointList.Clear();

                length = 0;
                List<double> distances = new List<double>();

                List<double> xs = new List<double>();
                List<double> ys = new List<double>();
                cpdistances.Clear();

                cpdistances.Add(0.0);
                for (int i = 0; i < controlPoints.Count - 1; i++)
                {
                    ControlPoint p1 = controlPoints[i];
                    ControlPoint p2 = controlPoints[i + 1];
                    length += Math.Sqrt(Math.Pow(p2.X - p1.X, 2) + Math.Pow(p2.Y - p1.Y, 2));
                    cpdistances.Add(length);
                }
                for (int i = 0; i < cpdistances.Count; i++)
                {
                    distances.Add(cpdistances[i]);
                    xs.Add(this.controlPoints[i].X);
                    ys.Add(this.controlPoints[i].Y);
                }

                xsMap = Interpolate.Linear(distances, xs);
                ysMap = Interpolate.Linear(distances, ys);

                TrajectoryConstraint[] constraints = { new MaxAccelerationConstraint(this.maxAcc), new MaxVelocityConstraint(this.maxVel), new CentripetalAccelerationConstraint(this.maxCen) };

                gen = new VelocityGeneration(this, constraints, sampleDistance, 0, 0);

                for (double time = 0; time < gen.getDuration(); time += timeSample)
                {
                    State s = gen.calculate(time);

                    pointList.Add(s);
                }
            }
        }

        public PState calculate(double distance)
        {
            if (controlPoints.Count == 0)
            {
                throw new Exception("No Path To Calculate");
            }
            if (controlPoints.Count == 1)
            {
                ControlPoint p = controlPoints[0];
                return new PState(distance, new Pose2d(p.X, p.Y, p.getRotation2d()), p.getRotation2d(), p.Radius);
            }
            if (isSpline)
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

                int indexCurrent = getCurrentControlPointIndex(distance);
                int indexNext = getNextControlPointIndex(distance);

                double distanceBetweenCP = cpdistances[indexNext] - cpdistances[indexCurrent];
                double distanceSinceCurrentCP = distance - cpdistances[indexCurrent];

                Rotation2d currentRotation = controlPoints[indexCurrent].getRotation2d();
                Rotation2d nextRotation = controlPoints[indexNext].getRotation2d();

                Rotation2d rot = currentRotation.interpolate(nextRotation, distanceSinceCurrentCP / distanceBetweenCP);


                return new PState(distance, new Pose2d(p.X, p.Y, rot), Rotation2d.fromRadians(Math.Atan2(p2.Y - p1.Y, p2.X - p1.X)), r);
            }
            else
            {
                SplinePoint p = linearInterpolation(distance);

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
                    p1 = linearInterpolation(distance1);
                    p2 = linearInterpolation(distance);
                    p3 = linearInterpolation(distance2);

                }
                else
                {
                    distance1 = distance + sampleDistance;
                    distance2 = distance + sampleDistance * 2;
                    p1 = linearInterpolation(distance);
                    p2 = linearInterpolation(distance1);
                    p3 = linearInterpolation(distance2);
                }
                int indexCurrent = getCurrentControlPointIndex(distance);
                int indexNext = getNextControlPointIndex(distance);

                double distanceBetweenCP = cpdistances[indexNext] - cpdistances[indexCurrent];
                double distanceSinceCurrentCP = distance - cpdistances[indexCurrent];

                Rotation2d currentRotation = controlPoints[indexCurrent].getRotation2d();
                Rotation2d nextRotation = controlPoints[indexNext].getRotation2d();

                Rotation2d rot = currentRotation.interpolate(nextRotation, distanceSinceCurrentCP / distanceBetweenCP);


                return new PState(distance, new Pose2d(p.X, p.Y, rot), Rotation2d.fromRadians(Math.Atan2(p2.Y - p1.Y, p2.X - p1.X)), double.PositiveInfinity);
            }
        }

        private int getCurrentControlPointIndex(double distance)
        {
            if (distance == length)
            {
                return cpdistances.Count - 2;
            }
            int index = -1;
            for (int i = 0; i < cpdistances.Count; i++)
            {
                double dist = cpdistances[i];
                if (dist <= distance)
                    index = i;
            }
            return index;
        }
        private int getNextControlPointIndex(double distance)
        {
            if (distance == length)
            {
                return cpdistances.Count - 1;
            }
            for (int i = 0; i < cpdistances.Count; i++)
            {
                double dist = cpdistances[i];
                if (dist > distance)
                    return i;
            }
            return -1;

        }


        private SplinePoint linearInterpolation(double distance)
        {
            if (isSpline) return null;
            return new SplinePoint(xsMap.Interpolate(distance), ysMap.Interpolate(distance));
        }

        static double TOL = 0.0000001;
        private double circleFromPoints(SplinePoint p1, SplinePoint p2, SplinePoint p3)
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

        public JObject toJSON()
        {

            JObject pathJSON = new JObject();
            pathJSON["name"] = this.name;
            pathJSON["id"] = this.id;
            pathJSON["isSpline"] = this.isSpline;
            pathJSON["maxVelocity"] = this.maxVel;
            pathJSON["maxAcceleration"] = this.maxAcc;
            pathJSON["maxCentripetalAcceleration"] = this.maxCen;
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

            this.generate();
            if (isSpline)
            {
                if (pointList.Count == 0)
                {
                    MessageBox.Show("Error no points to export.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return "";
                }
                List<ControlPoint> tmpCTL = new List<ControlPoint>();
                foreach (State s in pointList)
                {
                    Pose2d pose = s.getPathState().getPose2d();
                    tmpCTL.Add(new ControlPoint(this, pose.getX(), pose.getY(), pose.getRotation().getDegrees(), s.getPathState().getRadius()));
                }
                foreach (ControlPoint point in tmpCTL)
                {
                    pointStrings.Add(point.toJava());
                }
            }
            else
            {
                foreach (ControlPoint point in this.controlPoints)
                {
                    pointStrings.Add(point.toJava());
                }

            }

            path += String.Join(",\n", pointStrings) + "\n";
            path += "\t\t}";
            return path;
        }

        public string toTxt()
        {
            this.generate();
            if (isSpline)
            {
                string pathTxt = $"{this.maxVel} {this.maxAcc} {this.maxCen}\n";
                if (pointList.Count == 0)
                {
                    MessageBox.Show("Error no points to export.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return "";
                }
                List<ControlPoint> tmpCTL = new List<ControlPoint>();
                foreach (State s in pointList)
                {
                    Pose2d pose = s.getPathState().getPose2d();
                    tmpCTL.Add(new ControlPoint(this, pose.getX(), pose.getY(), pose.getRotation().getDegrees(), s.getPathState().getRadius()));
                }
                foreach (ControlPoint point in tmpCTL)
                {
                    pathTxt += point.toTxt();
                }
                return pathTxt + "@@@";
            }
            else
            {
                string pathTxt = $"{this.maxVel} {this.maxAcc} {this.maxCen}\n";
                foreach (ControlPoint point in this.controlPoints)
                {
                    pathTxt += point.toTxt();
                }
                return pathTxt + "@@@";

            }

        }

        public void updateAll(string name, bool snapToPrevious, double vel, double acc, double cen)
        {
            if (this.name != name || this.snapToPrevious != snapToPrevious || this.maxVel != vel || this.MaxAcc != acc || this.maxCen != cen)
                newEdit();
            this.maxVel = vel;
            this.maxAcc = acc;
            this.maxCen = cen;
            this.name = name;
            this.snapToPrevious = snapToPrevious;

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
                if (value.Trim() != this.name)
                {
                    newEdit();
                    MotionProfiler.saveUndoState();
                    this.name = value.Trim();
                }

            }
        }
        public string Id
        {
            get
            {
                return this.id;
            }
            set
            {
                if (this.id != value)
                {
                    newEdit();
                    this.id = value;
                }
            }
        }
        public bool IsSpline
        {
            get
            {
                return this.isSpline;
            }
            set
            {
                if (this.isSpline != value)
                {
                    newEdit();
                    this.isSpline = value;
                }
            }
        }
        public double MaxVel
        {
            get
            {
                return this.maxVel;
            }
            set
            {
                if (this.maxVel != value)
                {
                    newEdit();
                    this.maxVel = value;
                }
            }
        }
        public double MaxAcc
        {
            get
            {
                return this.maxAcc;
            }
            set
            {
                if (this.maxAcc != value)
                {
                    newEdit();
                    this.maxAcc = value;
                }
            }
        }

        public double MaxCen
        {
            get
            {
                return this.maxCen;
            }
            set
            {
                if (this.maxCen != value)
                {
                    newEdit();
                    this.maxCen = value;
                }
            }
        }

        public bool SnapToPrevious
        {
            get
            {
                return this.snapToPrevious;
            }
            set
            {
                if (this.snapToPrevious != value)
                {
                    newEdit();
                    this.snapToPrevious = value;
                }
            }
        }

        public List<ControlPoint> ControlPoints
        {
            get
            {
                return this.controlPoints;
            }
            set
            {
                if (this.controlPoints != value)
                {
                    newEdit();
                    this.controlPoints = value;
                }
            }
        }
    }
}
