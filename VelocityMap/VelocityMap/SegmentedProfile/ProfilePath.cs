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

        /// <summary>
        /// Creates a new blank profile path
        /// </summary>
        public ProfilePath(string name)
        {
            this.name = name;
            this.controlPoints = new List<ControlPoint>();
            this.id = Guid.NewGuid().ToString();
        }

        /// <summary>
        /// Loads a profile path from a path JSON representation
        /// </summary>
        /// <param name="pathJSON">JSON-formatted path object<see cref="JObject"/></param>
        public ProfilePath(JObject pathJSON)
        {
            this.name = (string)pathJSON["name"];
            this.id = (string)pathJSON["id"];
            this.controlPoints = new List<ControlPoint>();

            foreach (JObject point in pathJSON["points"])
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

        public void clearPoints()
        {
            this.controlPoints.Clear();
        }

        public Boolean isEmpty()
        {
            return this.controlPoints.Count == 0;
        }

        public JObject toJSON()
        {
            JObject pathJSON = new JObject();
            pathJSON["name"] = this.name;
            pathJSON["id"] = this.id;

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
            string pathTxt = "";
            foreach (ControlPoint point in this.controlPoints)
            {
                pathTxt += point.toTxt();
            }
            return pathTxt;
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
