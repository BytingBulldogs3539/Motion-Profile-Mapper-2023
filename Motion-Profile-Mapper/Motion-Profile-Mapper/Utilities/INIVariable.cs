using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MotionProfileMapper.Utilities
{
    class INIVariable
    {
        public string name { get; set; }
        public string type { get; set; }
        public string value { get; set; }
        public string comment { get; set; }

        public INIVariable(string name = null, string type = null, string value = null, string comment = null)
        {
            this.name = name;
            this.type = type;
            this.value = value;
            this.comment = comment;
        }
    }
}
