using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MotionProfile
{
    public class ControlPoint
    {
        private double x;
        private double y;
        private int heading;

        private double tangentX;
        private double tangentY;

        float velocity;
        string id;

        /// <summary>
        /// Creates a new control point based on coordinates
        /// </summary>
        public ControlPoint(double x, double y, int heading)
        {
            this.x = x;
            this.y = y;
            this.heading = heading;
            this.id = Guid.NewGuid().ToString();
        }

        /// <summary>
        /// Creates a control point copy from another control point
        /// </summary>
        /// <param name="other">ControlPoint object to copy<see cref="ControlPoint"/></param>
        public ControlPoint(ControlPoint other)
        {
            this.x = other.X;
            this.y = other.Y;
            this.heading = other.Heading;
            this.id = other.Id;
        }

        /// <summary>
        /// Loads a control point from a point JSON representation
        /// </summary>
        /// <param name="pointJSON">JSON-formatted point object<see cref="JObject"/></param>
        public ControlPoint(JObject pointJSON)
        {
            this.x = (double)pointJSON["x"];
            this.y = (double)pointJSON["y"];
            this.heading = (int)pointJSON["heading"];
            this.tangentX = (double)pointJSON["dx"];
            this.tangentY = (double)pointJSON["dy"];
            this.id = (string)pointJSON["id"];
        }

        /// <summary>
        /// Parses the ControlPoint into JSON format
        /// </summary>
        public JObject toJSON()
        {
            JObject pointJSON = new JObject();
            pointJSON["x"] = this.X;
            pointJSON["y"] = this.Y;
            pointJSON["heading"] = this.Heading;
            pointJSON["dx"] = this.TangentX;
            pointJSON["dy"] = this.TangentY;
            pointJSON["id"] = this.Id;
            return pointJSON;
        }

        public string toJava()
        {
            return $"\t\t\t{{{this.Y}, {this.X}, {this.TangentY}, {this.TangentX}, {this.heading}}}";
        }

        public string toTxt()
        {
            return $"{this.Y} {this.X} {this.TangentY} {this.TangentX} {this.heading}\n";
        }

        public int Heading
        {
            get
            {
                return this.heading;
            }
            set
            {
                this.heading = value;
            }
        }

        public double X
        {
            get
            {
                return Math.Round(this.x, 5);
            }

            set
            {
                this.x = value;
            }
        }

        public double Y
        {
            get
            {
                return Math.Round(this.y, 5);
            }

            set
            {
                this.y = value;
            }
        }

        public float Velocity
        {
            get
            {
                return this.velocity;
            }
            set
            {
                this.velocity = value;
            }
        }

        public string Id
        {
            get
            {
                return this.id;
            }
        }

        public void setTangents(double dx, double dy)
        {
            this.tangentX = dx;
            this.tangentY = dy;
            // REVERSED AND NEGATED BECAUSE THIS COORDINATE SYSTEM IS STUPID
            //this.tangentX = dy;
            //this.tangentY = -dx;
        }

        public double TangentX
        {
            get
            {
                return Math.Round(this.tangentX, 5);
            }
        }

        public double TangentY
        {
            get
            {
                return Math.Round(this.tangentY, 5);
            }
        }
    }
}
