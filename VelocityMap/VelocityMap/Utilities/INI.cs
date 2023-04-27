using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VelocityMap.Utilities
{
    class INI
    {
        public string fileName { get; set; }
        public string variableClass { get; set; }
        private SortedDictionary<string, SortedDictionary<string, string>> variables;

        public INI(string fileName, System.IO.StreamReader reader)
        {
            this.fileName = fileName;
            this.variableClass = fileName.Substring(0, fileName.IndexOf('.'));
            this.variables = new SortedDictionary<string, SortedDictionary<string, string>>();
            
            string currentSection = "";
            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine();

                if (line == "") continue;
                else if (line == $"[{variableClass}]") currentSection = "Value";
                else if (line[0] == '[') currentSection = line.Substring(1, line.Length - 2);
                else
                {
                    string variable = line.Substring(0, line.IndexOf('='));
                    string value = line.Substring(line.IndexOf('=') + 1);
                    if (!variables.ContainsKey(variable)) variables.Add(variable, new SortedDictionary<string, string>());
                    variables[variable].Add(currentSection, value);
                }
            }
        }

        public INI()
        {

        }

        public void loadTable(DataGridView table)
        {
            table.Rows.Clear();
            foreach (string variable in variables.Keys)
            {
                table.Rows.Add(variable, variables[variable]["Type"], variables[variable]["Value"]);
            }
        }

        public void updateValue(int index, string valueType, string value)
        {
            string variable = variables.ElementAt(index).Key;
            variables[variable][valueType] = value;
        }

        public void changeVariableName(int index, string newName)
        {
            if (variables.ContainsKey(newName)) return;

            string oldName = variables.ElementAt(index).Key;
            variables.Add(newName, new SortedDictionary<string, string>());
            variables[newName]["Value"] = variables[oldName]["Value"];
            variables[newName]["Type"] = variables[oldName]["Type"];
            variables.Remove(oldName);
        }

        public string toIni()
        {
            string ini = $"[{variableClass}]\n";
            foreach (string variable in variables.Keys)
            {
                ini += $"{variable}={variables[variable]["Value"]}\n";
            }

            List<string> others = new List<string>() { "Type" };
            foreach (string dataType in others)
            {
                ini += $"\n[{dataType}]\n";
                foreach (string variable in variables.Keys)
                {
                    ini += $"{variable}={variables[variable][dataType]}\n";
                }
            }
            return ini;
        }
    }
}
