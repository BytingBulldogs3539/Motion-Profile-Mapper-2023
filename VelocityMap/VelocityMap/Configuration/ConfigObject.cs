using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VelocityMap.Configuration
{
    class ConfigObject
    {
        public string Variable_Name { get; set; }

        public String Variable_Type { get; set; }

        public String Value { get; set; }
        public ConfigObject(String name, String type, String value)
        {
            this.Value = value;
            this.Variable_Name = name;
            this.Variable_Type = type;
        }
    }
}
