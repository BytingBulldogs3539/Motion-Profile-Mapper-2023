using MotionProfile.SegmentedProfile;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MotionProfileMapper.VelocityGenerate;

namespace MotionProfile
{
    public class ControlPoint
    {
        private double x;
        private double y;
        private double rotation;
        private double radius;

        private float velocity;
        private string id;
        ProfilePath path;

        /// <summary>
        /// Creates a new control point based on coordinates
        /// </summary>
        public ControlPoint(ProfilePath path, double x, double y, double rotation)
        {
            this.path = path;
            this.x = x;
            this.y = y;
            this.rotation = rotation;
            this.id = Guid.NewGuid().ToString();
            radius = double.PositiveInfinity;
        }
        public ControlPoint(ProfilePath path, double x, double y, double rotation, double radius)
        {
            this.path = path;
            this.x = x;
            this.y = y;
            this.rotation = rotation;
            this.radius = radius;
            this.id = Guid.NewGuid().ToString();
        }

        public bool Equals(ControlPoint other)
        {
            Console.WriteLine(this.x + "  " + other.x);
            if (this.x == other.x && this.y == other.y && this.rotation == other.rotation)
            {
                return true;
            }
            return false;
        }

        public Rotation2d getRotation2d()
        {
            return Rotation2d.fromDegrees(rotation);
        }

        /// <summary>
        /// Creates a control point copy from another control point
        /// </summary>
        /// <param name="other">ControlPoint object to copy<see cref="ControlPoint"/></param>
        public ControlPoint(ControlPoint other, ProfilePath path)
        {
            this.path = path;
            this.x = other.X;
            this.y = other.Y;
            this.rotation = other.Rotation;
            this.radius = other.Radius;
            this.id = Guid.NewGuid().ToString();
        }

        /// <summary>
        /// Loads a control point from a point JSON representation
        /// </summary>
        /// <param name="pointJSON">JSON-formatted point object<see cref="JObject"/></param>
        public ControlPoint(JObject pointJSON, ProfilePath path)
        {
            this.path = path;
            this.x = (double)pointJSON["x"];
            this.y = (double)pointJSON["y"];
            this.rotation = (double)pointJSON["rotation"];
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
            pointJSON["rotation"] = this.Rotation;
            pointJSON["id"] = this.Id;
            return pointJSON;
        }

        public void newEdit(string reason)
        {
            path.newEdit(reason);
        }


        public string toJava()
        {
            if (Double.IsInfinity(this.Radius))
                return $"\t\t\t{{{this.Y}, {this.X}, {this.Rotation}, Double.POSITIVE_INFINITY}}";
            return $"\t\t\t{{{this.Y}, {this.X}, {this.Rotation}, {this.Radius}}}";
        }

        public string toTxt()
        {
            if (Double.IsInfinity(this.Radius))
                return $"{this.Y} {this.X} {this.Rotation} Infinity\n";
            return $"{this.Y} {this.X} {this.Rotation} {this.Radius}\n";
        }

        public double Radius
        {
            get
            {
                return Math.Round(this.radius, 5);
            }
        }


        public double Rotation
        {
            get
            {
                return Math.Round(this.rotation, 5);
            }
            set
            {
                newEdit("Control Point Rotation Change");
                this.rotation = value;
            }
        }

        public void quickChangeX(double x)
        {
            this.x = x;
        }
        public void quickChangeY(double y)
        {
            this.y = y;
        }
        public void quickChangeRotation(double rotation)
        {
            this.rotation = rotation;
        }


        public double X
        {
            get
            {
                return Math.Round(this.x, 5);
            }

            set
            {
                if (value != this.x)
                {
                    newEdit("Control Point X Change");
                    this.x = value;
                }
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
                if (value != this.y)
                {
                    newEdit("Control Point Y Change");
                    this.y = value;
                }
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
                if (this.velocity != value)
                {
                    newEdit("Control Point Velocity Change");
                    this.velocity = value;
                }
            }
        }

        public string Id
        {
            get
            {
                return this.id;
            }
        }
        public ProfilePath Path
        {
            get
            {
                return this.path;
            }
        }
    }
}
