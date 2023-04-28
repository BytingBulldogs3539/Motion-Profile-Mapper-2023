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
        public List<INIVariable> variables;

        public INI(string fileName, System.IO.StreamReader reader)
        {
            this.fileName = fileName;
            this.variables = new List<INIVariable>();
            
            string currentSection = "";
            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine();

                if (line == "") continue;
                else if (line == $"[{fileName}]") currentSection = "Value";
                else if (line[0] == '[') currentSection = line.Substring(1, line.Length - 2);
                else
                {
                    string variable = line.Substring(0, line.IndexOf('='));
                    string value = line.Substring(line.IndexOf('=') + 1);

                    INIVariable query = findVariable(variable);
                    if (query == null)
                    {
                        variables.Add(new INIVariable(name: variable));
                        query = variables.Last();
                    }
                    
                    switch (currentSection)
                    {
                        case "Value":
                            query.value = value;
                            break;
                        case "Type":
                            query.type = value;
                            break;
                    }
                }
            }
        }

        public INI()
        {
            this.fileName = "temp";
            this.variables = new List<INIVariable>();
        }

        public INIVariable findVariable(string name)
        {
            foreach (INIVariable var in variables)
            {
                if (var.name == name) return var;
            }
            return null;
        }

        public void loadTable(DataGridView table)
        {
            table.Rows.Clear();
            foreach (INIVariable variable in variables)
            {
                int rowIndex = table.Rows.Add(variable.name, variable.type, variable.value);
                if (variable.type == "Boolean")
                {
                    DataGridViewCheckBoxCell cell = new DataGridViewCheckBoxCell();
                    cell.Value = variable.value;
                    table.Rows[rowIndex].Cells[2] = cell;
                }
            }
        }

        public void updateValue(int index, string valueType, string value)
        {
            switch (valueType)
            {
                case "Name":
                    variables[index].name = value;
                    break;
                case "Value":
                    variables[index].value = value;
                    break;
                case "Type":
                    variables[index].type = value;
                    break;
            }
        }

        public void addVariable(string name)
        {
            this.variables.Add(new INIVariable(name: name));
        }

        public string ToString()
        {
            return this.fileName;
        }

        public string toIni()
        {
            string ini = $"[{fileName}]\n";
            foreach (INIVariable variable in variables)
            {
                ini += $"{variable.name}={variable.value}\n";
            }

            List<string> others = new List<string>() { "Type" };
            foreach (string dataType in others)
            {
                ini += $"\n[{dataType}]\n";
                foreach (INIVariable variable in variables)
                {
                    switch (dataType)
                    {
                        case "Type":
                            ini += $"{variable.name}={variable.type}\n";
                            break;
                    }
                    
                }
            }
            return ini;
        }
    }
}
