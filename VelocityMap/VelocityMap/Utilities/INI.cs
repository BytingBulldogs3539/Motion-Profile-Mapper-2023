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
                    string variable = line.Substring(0, line.IndexOf('=')).Trim();
                    string value = line.Substring(line.IndexOf('=') + 1).Trim();

                    int query = findVariable(variable);
                    if (query == -1)
                    {
                        variables.Add(new INIVariable(name: variable));
                        query = variables.Count-1;
                    }
                    
                    switch (currentSection)
                    {
                        case "Value":
                            updateValue(query, "Value", value);
                            break;
                        case "Type":
                            Console.WriteLine("Type");
                            updateValue(query, "Type", value);
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

        public int findVariable(string name)
        {
            int i = 0;
            foreach (INIVariable var in variables)
            {
                if (var.name == name) return i;
                i++;

            }
            return -1;
        }

        public bool isValid()
        {
            return true;
        }

        public void loadTable(DataGridView table)
        {
            table.Rows.Clear();
            foreach (INIVariable variable in variables)
            {
                int rowIndex = table.Rows.Add(variable.name, variable.type, variable.value);
                if (variable.type == "boolean")
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
                    Console.WriteLine(value);
                    variables[index].value = value;
                    break;
                case "Type":
                    switch (value.ToLower())
                    {
                        case "int":
                            variables[index].type = "int";
                            break;
                        case "double":
                            variables[index].type = "double";
                            break;
                        case "boolean":
                            variables[index].type = "boolean";
                            break;
                        default :
                            variables[index].type = "String";
                            break;
                    }
                    
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
                ini += $"{variable.name} = {variable.value}\n";
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
                            ini += $"{variable.name} = {variable.type}\n";
                            break;
                    }
                    
                }
            }
            return ini;
        }
    }
}
