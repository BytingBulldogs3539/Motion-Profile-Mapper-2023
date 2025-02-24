using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MotionProfileMapper.Utilities {
    class INI {
        public string fileName { get; set; }
        public List<INIVariable> variables;

        public INI(string fileName, System.IO.StreamReader reader) {
            this.fileName = fileName;
            this.variables = new List<INIVariable>();

            string currentSection = "";
            while (!reader.EndOfStream) {
                string line = reader.ReadLine();

                if (line == "") continue;
                else if (line == $"[{fileName}]") currentSection = "Value";
                else if (line[0] == '[') currentSection = line.Substring(1, line.Length - 2);
                else {
                    string variable = line.Substring(0, line.IndexOf('=')).Trim();
                    string value = line.Substring(line.IndexOf('=') + 1).Trim();

                    int query = findVariable(variable);
                    if (query == -1) {
                        variables.Add(new INIVariable(name: variable));
                        query = variables.Count - 1;
                    }

                    switch (currentSection) {
                        case "Value":
                            updateValue(query, "Value", value);
                            break;
                        case "Type":
                            updateValue(query, "Type", value);
                            break;
                        case "Comment":
                            updateValue(query, "Comment", value);
                            break;
                    }
                }
            }
        }

        public INI() {
            this.fileName = "New_Configuration";
            this.variables = new List<INIVariable>();

        }

        public int findVariable(string name) {
            int i = 0;
            foreach (INIVariable var in variables) {
                if (var.name == name) return i;
                i++;

            }
            return -1;
        }

        public bool isValid() {
            return true;
        }

        public void loadTable(DataGridView table) {
            table.Rows.Clear();
            foreach (INIVariable variable in variables) {
                int rowIndex = table.Rows.Add(variable.name, variable.type, variable.value, variable.comment);
                if (variable.type == "boolean") {
                    DataGridViewCheckBoxCell cell = new DataGridViewCheckBoxCell();
                    cell.Value = variable.value;
                    table.Rows[rowIndex].Cells[2] = cell;
                }
            }
        }

        public Boolean checkVariableNames(Boolean useMessagebox = true) {
            foreach (INIVariable var in variables) {
                if (tryToString(var.name) == "") {
                    if (useMessagebox)
                        MessageBox.Show($"{this.fileName} contains a variable that does not have a valid name", "Invalid Type Or Value", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return true;
                }
            }
            return false;
        }
        public Boolean checkVariableTypes(Boolean useMessagebox = true) {
            foreach (INIVariable var in variables) {
                if (tryToString(var.type) == "") {
                    if (useMessagebox)
                        MessageBox.Show($"{this.fileName} contains a variable \"{var.name}\" that does not have a type ", "Invalid Type", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return true;
                }
            }
            return false;
        }

        public Boolean checkValues(Boolean useMessagebox = true) {
            foreach (INIVariable var in variables) {
                if (( tryToString(var.value).Trim() == "" ) && ( tryToString(var.type).ToLower() != "string" )) {
                    if (useMessagebox)
                        MessageBox.Show($"{this.fileName} contains a variable \"{var.name}\" that does not have a valid value", "Invalid Value", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return true;
                }
            }
            return false;
        }

        public Boolean checkVariableNameDuplicates(Boolean useMessagebox = true) {
            List<string> names = new List<string>();
            foreach (INIVariable var in variables) {
                if (names.Contains(var.name)) {
                    if (useMessagebox)
                        MessageBox.Show($"There are two or more variables named {var.name} in {this.fileName}", "Duplicate Variable Names", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return true;
                }
                names.Add(var.name);
            }
            return false;
        }

        private String tryToString(object o) {
            if (o != null) {
                return o.ToString();
            }
            return "";
        }

        public void updateValue(int index, string valueType, string value) {
            switch (valueType) {
                case "Name":
                    variables[index].name = value;
                    break;
                case "Value":
                    variables[index].value = value;
                    break;
                case "Type":
                    switch (value.ToLower()) {
                        case "int":
                            variables[index].type = "int";
                            break;
                        case "double":
                            variables[index].type = "double";
                            break;
                        case "boolean":
                            variables[index].type = "boolean";
                            break;
                        default:
                            variables[index].type = "String";
                            break;
                    }
                    break;
                case "Comment":
                    variables[index].comment = value;
                    break;
            }
        }
        public void updateVariable(int index, string name, string type, string value, string comment) {
            variables[index].name = name;
            variables[index].type = type;
            variables[index].value = value;
            variables[index].comment = comment;
        }


        public void addVariable(string name) {
            this.variables.Add(new INIVariable(name: name));
        }

        public void addVariable(string name, string type, string value) {
            this.variables.Add(new INIVariable(name: name, type: type, value: value));
        }

        public void clearVariables() {
            this.variables.Clear();
        }

        override
        public string ToString() {
            return this.fileName;
        }

        public void removeAt(int index) {
            variables.RemoveAt(index);
        }

        public string toIni() {
            string ini = $"[{fileName}]\n";
            foreach (INIVariable variable in variables) {
                ini += $"{variable.name} = {variable.value}\n";
            }

            List<string> others = new List<string>() { "Type", "Comment" };
            foreach (string dataType in others) {
                ini += $"\n[{dataType}]\n";

                foreach (INIVariable variable in variables) {
                    switch (dataType) {
                        case "Type":
                            ini += $"{variable.name} = {variable.type}\n";
                            break;
                        case "Comment":
                            ini += $"{variable.name} = {variable.comment}\n";
                            break;
                    }

                }
            }
            return ini;
        }
        public string toJava() {

            string fileContent = "package frc.robot.constants;\r\n\r\nimport org.frcteam3539.BulldogLibrary.INIConfiguration.BBConstants;\r\n\r\n";

            fileContent += $"public class {this.fileName.Replace(" ", "").Trim()} extends BBConstants " + "{" + "\r\n";

            fileContent += "\tpublic " + this.fileName + "() {\r\n\t\tsuper(\"" + Properties.Settings.Default.INILocation + this.fileName + ".ini\", true);\r\n\t\tsave();\r\n\t\twriteToNetworkTable();\r\n\t}\r\n\r\n";

            foreach (INIVariable variable in variables) {
                string commentStr = "";

                if (variable.comment.Trim() != "") {
                    commentStr = " // " + variable.comment.Trim();
                }

                if (variable.type.ToLower() == "string")
                    fileContent += $"\tpublic static {variable.type} {variable.name} = \"{variable.value}\";{commentStr}\r\n";
                else if (variable.type.ToLower() == "boolean") {
                    if (variable.value.ToLower() == "true") {
                        fileContent += $"\tpublic static {variable.type} {variable.name} = true;{commentStr}\r\n";
                    } else if (variable.value.ToLower() == "false") {
                        fileContent += $"\tpublic static {variable.type} {variable.name} = false;{commentStr}\r\n";
                    }
                } else
                    fileContent += $"\tpublic static {variable.type} {variable.name} = {variable.value};{commentStr}\r\n";
            }

            fileContent += "}\r";

            return fileContent;
        }
    }
}
