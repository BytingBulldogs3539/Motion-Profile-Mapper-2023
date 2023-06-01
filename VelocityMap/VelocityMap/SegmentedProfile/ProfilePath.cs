using MotionProfile.Spline;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public bool isSpline = false;


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
        private List<SplinePoint> pointList = new List<SplinePoint>();
        private List<VelocityPoint> velocityPoints = new List<VelocityPoint>();
        private double length = 0.0;

        public void generate()
        {
            if (isSpline)
            {
                pointList.Clear();
                SplinePath.GenSpline(this.controlPoints);

                length = SplinePath.getLength();

                velocityPoints = new VelocityGenerator(
                    this.maxVel,
                    this.maxAcc,
                    10000,
                    .05
                ).GeneratePoints(length);

                List<ControlPointSegment> splineSegments = SplinePath.GenSpline(this.controlPoints, velocityPoints);


                SplinePoint lastPoint = splineSegments[0].points[0];
                SplinePoint currentPoint;
                foreach (ControlPointSegment segment in splineSegments)
                {

                    currentPoint = segment.points[Math.Min(2, segment.points.Count - 1)];
                    double pointDistance = distance(currentPoint, lastPoint);

                    // Additional calculation for point at the end of the path
                    if (segment == splineSegments.Last())
                    {
                        lastPoint = segment.points[Math.Max(0, segment.points.Count - 3)];
                        currentPoint = segment.points.Last();
                        pointDistance = distance(currentPoint, lastPoint);
                    }
                    lastPoint = segment.points[Math.Max(0, segment.points.Count - 3)];

                    foreach (SplinePoint point in segment.points)
                    {
                        pointList.Add(point);
                    }
                }
            }
            else
            {
                length = 0;
                for (int i = 0; i<controlPoints.Count-1; i++)
                {
                    ControlPoint p1 = controlPoints[i];
                    ControlPoint p2 = controlPoints[i+1];
                    length += Math.Sqrt(Math.Pow(p2.X - p1.X, 2) + Math.Pow(p2.Y - p1.Y, 2));

                }
                for (int i = 0; i < controlPoints.Count; i++)
                {
                    ControlPoint p1 = controlPoints[i];
                    pointList.Add(new SplinePoint(p1.X, p1.Y, i));
                }

                velocityPoints = new VelocityGenerator(
                    this.maxVel,
                    this.maxAcc,
                    10000,
                    .05
                    ).GeneratePoints(length);
                
            }
        }

        public List<SplinePoint> getPoints()
        {
            return pointList;
        }
        public List<VelocityPoint> getVelocityPoints()
        {
            return velocityPoints;
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
