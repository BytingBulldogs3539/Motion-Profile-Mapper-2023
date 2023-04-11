using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MotionProfile.SegmentedProfile
{
    public class Profile
    {
        private string name;
        private string edited;
        public List<ProfilePath> paths;
        static int profileCounter = 1;

        /// <summary>
        /// Creates a new blank profile
        /// </summary>
        public Profile()
        {
            this.name = "new profile " + profileCounter++;
            this.edited = DateTime.Now.ToString("MM/dd/yy, hh:mm tt");
            this.paths = new List<ProfilePath>();
        }

        /// <summary>
        /// Loads a profile from a profile JSON representation
        /// </summary>
        /// <param name="profileJSON">JSON-formatted profile object<see cref="JObject"/></param>
        public Profile(JObject profileJSON)
        {
            this.name = (string)profileJSON["name"];
            this.edited = (string)profileJSON["edited"];
            this.paths = new List<ProfilePath>();

            foreach (JObject pathJSON in profileJSON["paths"])
            {
                this.paths.Add(new ProfilePath(pathJSON));
            }
        }

        public Profile(Profile other)
        {
            this.name = other.name;
            this.edited = other.edited;
            this.paths = new List<ProfilePath>();

            foreach (ProfilePath path in other.paths)
            {
                this.paths.Add(new ProfilePath(path));
            }
        }

        public void newPath(string name, ProfilePath previous = null)
        {
            this.paths.Add(new ProfilePath(name, previous));
        }

        public void movePathOrderUp(ProfilePath pathToMove)
        {
            int pathIndex = this.paths.IndexOf(pathToMove);

            if (pathIndex < 1) return;

            ProfilePath temp = this.paths[pathIndex];
            this.paths.RemoveAt(pathIndex);
            this.paths.Insert(pathIndex - 1, temp);
        }

        public void movePathOrderDown(ProfilePath pathToMove)
        {
            int pathIndex = this.paths.IndexOf(pathToMove);

            if (pathIndex == -1 || pathIndex == this.paths.Count - 1) return;

            ProfilePath temp = this.paths[pathIndex];
            this.paths.RemoveAt(pathIndex);
            this.paths.Insert(pathIndex + 1, temp);
        }

        public void mirrorPath(ProfilePath pathToMirror, double fieldWidth)
        {
            int index = this.paths.IndexOf(pathToMirror);
            this.paths[index].mirrorPoints(fieldWidth);
        }

        public void mirrorAllPaths(double fieldWidth)
        {
            foreach (ProfilePath path in this.paths)
            {
                path.mirrorPoints(fieldWidth);
            }
        }

        public bool isValid()
        {
            if (this.paths.Count == 0) return false;
            foreach (ProfilePath path in this.paths)
            {
                if (path.controlPoints.Count < 2) return false;
            }
            return true;
        }

        public JObject toJSON()
        {
            JObject profile = new JObject();
            profile["name"] = this.name;
            profile["edited"] = this.edited;

            JArray pathsJSON = new JArray();
            foreach (ProfilePath path in this.paths)
            {
                pathsJSON.Add(path.toJSON());
            }
            profile["paths"] = pathsJSON;

            return profile;
        }

        public string toJava()
        {
            string profile = "public class Test {\n";

            profile += "\tpublic final double[][][] paths = {\n";
            List<string> pathStrings = new List<string>();
            foreach (ProfilePath path in this.paths)
            {
                pathStrings.Add(path.toJava());
            }
            profile += String.Join(",\n", pathStrings) + "\n";
            profile += "\t};\n";
            profile += "}\n";

            return profile;
        }

        public string toTxt()
        {
            List<string> pathStrings = new List<string>();
            foreach (ProfilePath path in this.paths)
            {
                pathStrings.Add(path.toTxt());
            }
            return String.Join("\n", pathStrings);
        }

        public string newEdit()
        {
            this.edited = DateTime.Now.ToString("MM/dd/yy, hh:mm tt");
            return this.Edited;
        }

        public string Name
        {
            get
            {
                return this.name;
            }

            set
            {
                string newName = value.Trim();
                if (newName == "") return;
                this.name = newName;
            }
        }

        public int PathCount
        {
            get
            {
                return this.paths.Count;
            }
        }

        public String Edited
        {
            get
            {
                return this.edited;
            }
        }
    }
}
