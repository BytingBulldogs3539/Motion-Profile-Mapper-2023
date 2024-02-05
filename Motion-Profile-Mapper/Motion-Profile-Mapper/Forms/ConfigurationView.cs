using Renci.SshNet;
using Renci.SshNet.Sftp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MotionProfileMapper.Utilities;

namespace MotionProfileMapper.Forms
{
    public partial class ConfigurationView : Form
    {

        private Menu menu;
        private OpenFileDialog fileDialog;
        private List<INI> inis = new List<INI>();
        private INI selectedIni = null;
        private DateTime timeOfUpload;

        private List<string> javaKeywords = new List<string>()
        {
            "abstract", "continue", "for", "new", "switch",
            "assert", "default", "goto", "package", "synchronized",
            "boolean", "do", "if", "private", "this",
            "break", "double", "implements", "protected", "throw",
            "byte", "else", "import", "public", "throws",
            "case", "enum", "instanceof", "return", "transient",
            "catch", "extends", "int", "short", "try",
            "char", "final", "interface", "static", "void",
            "class", "finally", "long", "strictfp", "volatile",
            "const", "float", "native", "super", "while"
        };

        public ConfigurationView(Menu menu)
        {
            this.menu = menu;
            InitializeComponent();
        }

        private void configurationGrid_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            List<String> list = new List<String>();
            for (int i = 0; i < configurationGrid.Rows.Count; i++)
            {
                DataGridViewRow row = configurationGrid.Rows[i];

                if (row.Index == configurationGrid.RowCount - 1)
                    continue;

                if (row.Cells[1].EditedFormattedValue.ToString() == "boolean" && !row.Cells[1].EditedFormattedValue.Equals(row.Cells[1].Value))
                {
                    DataGridViewCheckBoxCell cell = new DataGridViewCheckBoxCell();
                    cell.Value = false;
                    row.Cells[2] = cell;
                }
                if (row.Cells[1].EditedFormattedValue.ToString() == "int" && !row.Cells[1].EditedFormattedValue.Equals(row.Cells[1].Value))
                {
                    DataGridViewTextBoxCell cell = new DataGridViewTextBoxCell();
                    cell.Value = "";
                    row.Cells[2] = cell;
                }
                if (row.Cells[1].EditedFormattedValue.ToString() == "double" && !row.Cells[1].EditedFormattedValue.Equals(row.Cells[1].Value))
                {
                    DataGridViewTextBoxCell cell = new DataGridViewTextBoxCell();
                    cell.Value = "";
                    row.Cells[2] = cell;
                }
                if (row.Cells[1].EditedFormattedValue.ToString() == "String" && !row.Cells[1].EditedFormattedValue.Equals(row.Cells[1].Value))
                {
                    DataGridViewTextBoxCell cell = new DataGridViewTextBoxCell();
                    cell.Value = "";
                    row.Cells[2] = cell;
                }

                if (javaKeywords.Contains(row.Cells[0].EditedFormattedValue.ToString().ToLower()))
                {
                    row.ErrorText = "Variable name is a keyword in java";
                }
                else if (tryToString(row.Cells[0].EditedFormattedValue).ToString() == "")
                    row.ErrorText = "Name is not valid";

                else if (list.Contains(row.Cells[0].EditedFormattedValue.ToString()))
                    row.ErrorText = "Variable name is already in use.";
                else if (tryToString(row.Cells[1].EditedFormattedValue).ToString() == "")
                    row.ErrorText = "Type is not valid";
                else if (tryToString(row.Cells[1].EditedFormattedValue).ToString().ToLower() != "string" && tryToString(row.Cells[2].EditedFormattedValue).ToString() == "")
                    row.ErrorText = "Value is not valid";
                else
                    row.ErrorText = "";
                list.Add(row.Cells[0].EditedFormattedValue.ToString());
            }
        }
        private void configurationGrid_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (configurationGrid.CurrentCell == null)
            {
                return;
            }
            e.Control.KeyPress -= new KeyPressEventHandler(Column3_KeyPress_NULL);
            e.Control.KeyPress -= new KeyPressEventHandler(Column3_KeyPress_Int);
            e.Control.KeyPress -= new KeyPressEventHandler(Column3_KeyPress_Double);
            e.Control.KeyPress -= new KeyPressEventHandler(Column3_KeyPress_VariableName);

            e.Control.KeyDown -= new KeyEventHandler(Column3_KeyDown_Int);
            e.Control.KeyDown -= new KeyEventHandler(Column3_KeyDown_Double);


            if (configurationGrid.CurrentCell.ColumnIndex == 0)
            {
                TextBox tb = e.Control as TextBox;
                if (tb != null)
                {
                    tb.KeyPress += new KeyPressEventHandler(Column3_KeyPress_VariableName);
                }
            }
            if (configurationGrid.CurrentCell.ColumnIndex == 2)
            {
                if (configurationGrid.CurrentRow.Cells[1].Value == null)
                {
                    TextBox tb = e.Control as TextBox;
                    if (tb != null)
                    {
                        tb.KeyPress += new KeyPressEventHandler(Column3_KeyPress_NULL);
                    }
                }
                else if (configurationGrid.CurrentRow.Cells[1].Value.ToString() == "int")
                {
                    TextBox tb = e.Control as TextBox;
                    if (tb != null)
                    {
                        tb.KeyPress += new KeyPressEventHandler(Column3_KeyPress_Int);
                        tb.KeyDown += new KeyEventHandler(Column3_KeyDown_Int);
                    }
                }
                else if (configurationGrid.CurrentRow.Cells[1].Value.ToString() == "double")
                {
                    TextBox tb = e.Control as TextBox;
                    if (tb != null)
                    {
                        tb.KeyPress += new KeyPressEventHandler(Column3_KeyPress_Double);
                        tb.KeyDown += new KeyEventHandler(Column3_KeyDown_Double);
                    }
                }
            }
        }
        private void Column3_KeyDown_Double(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.V)
            {
                double output;
                if (!double.TryParse(Clipboard.GetText(), out output))
                {
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                }
            }
        }
        private void Column3_KeyDown_Int(object sender, KeyEventArgs e)
        {

            if (e.Control && e.KeyCode == Keys.V)
            {
                int output;
                if (!int.TryParse(Clipboard.GetText(), out output))
                {
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                }
            }
        }
        private void Column3_KeyPress_Double(object sender, KeyPressEventArgs e)
        {

            // allows 0-9, backspace, and decimal
            if (((e.KeyChar < 48 || e.KeyChar > 57) && e.KeyChar != 8 && e.KeyChar != 46 && e.KeyChar != '-' && !char.IsControl(e.KeyChar)))
            {
                e.Handled = true;
                return;
            }

            // checks to make sure only 1 decimal is allowed
            if (e.KeyChar == 46)
            {
                if ((sender as TextBox).Text.IndexOf(e.KeyChar) != -1)
                    e.Handled = true;
            }
            // checks to make sure only 1 - is allowed
            if (e.KeyChar == '-')
            {
                if ((sender as TextBox).SelectionStart != 0)
                    e.Handled = true;
                if ((sender as TextBox).Text.IndexOf(e.KeyChar) != -1)
                    e.Handled = true;
            }
        }

        private void Column3_KeyPress_VariableName(object sender, KeyPressEventArgs e)
        {
            if ((sender as TextBox).TextLength == 0)
            {
                if (!char.IsLetter(e.KeyChar) && !char.IsControl(e.KeyChar))
                {
                    e.Handled = true;
                }
            }
        }

        private void Column3_KeyPress_NULL(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }
        private void Column3_KeyPress_Int(object sender, KeyPressEventArgs e)
        {
            // checks to make sure only 1 decimal is allowed
            if (e.KeyChar == 46)
            {
                MessageBox.Show("Did you mean to use Double? Integers can't contain a '.'", "Wrong Type", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            // allows 0-9, backspace, and decimal
            if (((e.KeyChar < 48 || e.KeyChar > 57) && e.KeyChar != 8 && e.KeyChar != '-' && !char.IsControl(e.KeyChar)))
            {
                e.Handled = true;
                return;
            }

            // checks to make sure only 1 - is allowed
            if (e.KeyChar == '-')
            {
                if ((sender as TextBox).SelectionStart != 0)
                    e.Handled = true;
                if ((sender as TextBox).Text.IndexOf(e.KeyChar) != -1)
                    e.Handled = true;
            }
        }

        private Boolean checkForDuplicateNames()
        {
            List<string> names = new List<string>();
            foreach (INI ini in inis)
            {
                if (names.Contains(ini.fileName))
                {
                    MessageBox.Show($"There are two or more files named {ini.fileName}", "Duplicate File Names", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return true;
                }
                if (ini.checkVariableNameDuplicates()) return true;
                names.Add(ini.fileName);
            }
            return false;
        }
        private Boolean checkForVariableValues()
        {
            foreach (INI ini in inis)
            {
                if (ini.checkValues()) return true;
            }
            return false;
        }
        private Boolean checkForVariableType()
        {
            foreach (INI ini in inis)
            {
                if (ini.checkVariableTypes()) return true;
            }
            return false;
        }
        private Boolean checkForVariableNames()
        {
            foreach (INI ini in inis)
            {
                if (ini.checkVariableNames()) return true;
            }
            return false;
        }
        private Boolean validateINIS()
        {
            if (checkForVariableNames()) return true;
            if (checkForDuplicateNames()) return true;
            if (checkForVariableType()) return true;
            if (checkForVariableValues()) return true;
            return false;
        }

        private void saveAllLocalButton_Click(object sender, EventArgs e)
        {
            if (validateINIS()) return;

            SaveFileDialog browser = new SaveFileDialog();
            browser.RestoreDirectory = true;
            String files = "";
            foreach (INI ini in inis)
            {
                files += $"\"{ini.fileName}\" ";
            }

            browser.Filter = "Java and INI Files(*.java, *.ini)|directory|INI Files(*.ini)|directory|Java Files(*.java)|directory";
            browser.Title = "Save all files locally";
            browser.FileName = files;
            browser.OverwritePrompt = false;

            String iniBasePath = "";
            String javaBasePath = "";

            if (!Directory.Exists(Properties.Settings.Default.javaSavePath) || !Directory.Exists(Properties.Settings.Default.iniSavePath))
            {
                if (browser.ShowDialog() != DialogResult.OK) return;
                iniBasePath = Path.GetDirectoryName(browser.FileName.Trim());
                javaBasePath = iniBasePath;
            }
            else
            {
                iniBasePath = Properties.Settings.Default.iniSavePath;
                javaBasePath = Properties.Settings.Default.javaSavePath;
            }

                Cursor = Cursors.WaitCursor;
            setStatus("Saving profiles to file system...", Color.Black);

            List<string> paths = new List<string>();
            List<INI> iniList = new List<INI>();



            foreach (INI ini in inis)
            {
                string iniPath = Path.Combine(iniBasePath, Path.GetFileNameWithoutExtension(ini.fileName) + ".ini");
                string javaPath = Path.Combine(javaBasePath, Path.GetFileNameWithoutExtension(ini.fileName) + ".java");
                    
                if (browser.FilterIndex == 2)
                {
                    paths.Add(iniPath);
                    iniList.Add(ini);
                }
                else if (browser.FilterIndex == 3)
                {
                    paths.Add(javaPath);
                    iniList.Add(ini);
                }
                else
                {
                    paths.Add(javaPath);
                    iniList.Add(ini);
                    paths.Add(iniPath);
                    iniList.Add(ini);
                }
            }
            bool yesToAll = false;
            for (int i = 0; i < paths.Count; i++)
            {
                string path = paths[i];
                INI ini = iniList[i];
                string filePath = path;
                if (File.Exists(filePath) && !yesToAll)
                {
                    MessageBoxManager.Yes = "Yes";
                    MessageBoxManager.No = "No";
                    MessageBoxManager.Cancel = "Yes To All";
                    MessageBoxManager.Register();
                    DialogResult result = MessageBox.Show($"{path} already exists. \nDo you want to replace it?",
                        "Confirm Save As", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
                    MessageBoxManager.Unregister();
                    switch (result)
                    {
                        case DialogResult.OK:
                            break;
                        case DialogResult.No:
                            continue;
                        case DialogResult.Cancel:
                            yesToAll = true;
                            break;
                    }

                }
                if (Path.GetExtension(path) == ".ini")
                    using (var writer = new StreamWriter(filePath))
                    { 
                        writer.Write(ini.toIni());
                    }
                if (Path.GetExtension(path) == ".java")
                    using (var writer = new StreamWriter(filePath))
                    {
                        writer.Write(ini.toJava());
                    }
            }



            setStatus("Configure Robot Constants", Color.Black);
            Cursor = Cursors.Default;
        }
        private void saveLocalButton_Click(object sender, EventArgs e)
        {
            if (validateINIS()) return;

            if (selectedIni == null)
                return;
            SaveFileDialog browser = new SaveFileDialog();
            browser.RestoreDirectory = true;
            browser.FileName = selectedIni.fileName + ".ini";
            browser.AddExtension = false;
            browser.Filter = "Java and INI Files(*.java, *.ini)|*.ini;*.java;|INI File(*.ini)|*.ini|Java File(*.java)|*.java";
            browser.Title = "Save initialization file locally";
            browser.OverwritePrompt = false;

            String iniBasePath = "";
            String javaBasePath = "";

            if (!Directory.Exists(Properties.Settings.Default.javaSavePath) || !Directory.Exists(Properties.Settings.Default.iniSavePath))
            {
                if (browser.ShowDialog() != DialogResult.OK) return;
                iniBasePath = Path.GetDirectoryName(browser.FileName.Trim());
                javaBasePath = iniBasePath;
            }
            else
            {
                iniBasePath = Properties.Settings.Default.iniSavePath;
                javaBasePath = Properties.Settings.Default.javaSavePath;
            }

            Cursor = Cursors.WaitCursor;
            string iniPath = Path.Combine(iniBasePath,
                selectedIni.fileName + ".ini"
            );

            string javaPath = Path.Combine(javaBasePath,
                selectedIni.fileName + ".java"
            );

            List<string> paths = new List<string>();
            Console.WriteLine(browser.FilterIndex);
            if (browser.FilterIndex == 2)
                paths.Add(iniPath);
            else if (browser.FilterIndex == 3)
                paths.Add(javaPath);
            else
            {
                paths.Add(javaPath);
                paths.Add(iniPath);
            }


            bool yesToAll = false;
            foreach (string path in paths)
            {
                string filePath = path;
                if (File.Exists(filePath) && !yesToAll)
                {
                    MessageBoxManager.Yes = "Yes";
                    MessageBoxManager.No = "No";
                    MessageBoxManager.Cancel = "Yes To All";
                    MessageBoxManager.Register();
                    DialogResult result = MessageBox.Show($"{path} already exists. \nDo you want to replace it?", "Confirm Save As", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
                    MessageBoxManager.Unregister();
                    switch (result)
                    {
                        case DialogResult.OK:
                            break;
                        case DialogResult.No:
                            continue;
                        case DialogResult.Cancel:
                            yesToAll = true;
                            break;
                    }

                }
                if (Path.GetExtension(path) == ".ini")
                    using (var writer = new StreamWriter(filePath))
                    {
                        writer.Write(selectedIni.toIni());
                    }
                if (Path.GetExtension(path) == ".java")
                    using (var writer = new StreamWriter(filePath))
                    {
                        writer.Write(selectedIni.toJava());
                    }
            }

            Cursor = Cursors.Default;
        }
        private void saveToRioButton_Click(object sender, EventArgs e)
        {
            if (validateINIS()) return;

            if (inis.Count == 0)
            {
                setStatus("No INIs to save to RIO", Color.Red);
                return;
            }
            Cursor = Cursors.WaitCursor;

            ConnectionInfo info = new ConnectionInfo(
               Properties.Settings.Default.IpAddress,
               Properties.Settings.Default.Username,
               new PasswordAuthenticationMethod(
                   Properties.Settings.Default.Username,
                   Properties.Settings.Default.Password
               )
           );
            info.Timeout = TimeSpan.FromSeconds(5);

            SftpClient sftp = new SftpClient(info);

            try
            {
                setStatus("Establishing RIO connection...", Color.Black);
                sftp.Connect();

                setStatus("Uploading files to RIO...", Color.Black);
                if (!sftp.Exists(Properties.Settings.Default.INILocation)) sftp.CreateDirectory(Properties.Settings.Default.INILocation);

                List<INI> invalidINIs = new List<INI>();
                foreach (INI ini in inis)
                {
                    if (!ini.isValid())
                    {
                        invalidINIs.Add(ini);
                        continue;
                    }
                    // Upload txt file for robot to read in auton
                    MemoryStream txtStream = new MemoryStream(Encoding.UTF8.GetBytes(ini.toIni()));
                    sftp.UploadFile(txtStream, Path.Combine(
                        Properties.Settings.Default.INILocation,
                        ini.fileName.Replace(' ', '_') + ".ini"
                    ));
                }

                setStatus("Verifying file contents...", Color.Black);
                bool verified = true;
                foreach (INI ini in inis)
                {
                    if (invalidINIs.Contains(ini)) continue;
                    StreamReader reader = sftp.OpenText(
                        Path.Combine(Properties.Settings.Default.INILocation, ini.fileName.Replace(' ', '_') + ".ini")
                    );
                    if (ini.toIni() != reader.ReadToEnd()) verified = false;
                }

                if (invalidINIs.Count > 0) MessageBox.Show(
                    "One or more files were not deployed due to being invalid",
                    "Warning",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                if (verified)
                {
                    setStatus("INI(s) uploaded and verified successfully", Color.Green);
                    timer1.Enabled = true;
                    timeOfUpload = DateTime.Now;
                }
                else setStatus("Failed to verify uploaded file content", Color.Red);
                sftp.Disconnect();
            }
            catch (Renci.SshNet.Common.SshConnectionException exception)
            {
                Console.WriteLine("SshConnectionException, source: {0}", exception.StackTrace);
                setStatus("Failed to establish connection", Color.Red);
            }
            catch (Renci.SshNet.Common.SshOperationTimeoutException exception)
            {
                Console.WriteLine("SshConnectionException, source: {0}", exception.StackTrace);
                setStatus("Failed to establish connection", Color.Red);
            }
            catch (System.Net.Sockets.SocketException exception)
            {
                Console.WriteLine("SocketException, source: {0}", exception.StackTrace);
                setStatus("Failed to establish connection", Color.Red);
            }
            catch (Renci.SshNet.Common.SftpPermissionDeniedException exception)
            {
                Console.WriteLine("SftpPermissionDeniedException, source: {0}", exception.StackTrace);
                setStatus("Permission denied", Color.Red);
            }
            catch (Exception exception)
            {
                Console.WriteLine("Exception, source: {0}", exception.StackTrace);
                setStatus("Failed to upload INI to RIO", Color.Red);
            }

            Cursor = Cursors.Default;
        }

        private void loadRIOButton_Click(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;
            ConnectionInfo info = new ConnectionInfo(
                Properties.Settings.Default.IpAddress,
                Properties.Settings.Default.Username,
                new PasswordAuthenticationMethod(
                    Properties.Settings.Default.Username,
                    Properties.Settings.Default.Password
                )
            );
            info.Timeout = TimeSpan.FromSeconds(5);

            SftpClient sftp = new SftpClient(info);
            /*SftpClient sftp = new SftpClient(
                Properties.Settings.Default.IpAddress,
                Properties.Settings.Default.Username,
                Properties.Settings.Default.Password
            );*/
            try
            {
                setStatus("Establishing RIO connection...", Color.Black);
                sftp.Connect();

                if (!sftp.Exists(Properties.Settings.Default.INILocation))
                {
                    sftp.CreateDirectory(Properties.Settings.Default.INILocation);
                    setStatus("No INI files found at RIO directory", Color.Black);
                    return;
                }

                bool foundFiles = false;
                foreach (SftpFile file in sftp.ListDirectory(Properties.Settings.Default.INILocation))
                {
                    if (!file.Name.EndsWith(".ini")) continue;
                    foundFiles = true;

                    using (StreamReader reader = sftp.OpenText(file.FullName))
                    {
                        addFile(new INI(Path.GetFileNameWithoutExtension(file.Name), reader));
                    }
                }
                if (foundFiles) setStatus("INIs loaded from RIO", Color.Black);
                else setStatus("No INI files found at RIO directory", Color.Black);

                sftp.Disconnect();
            }
            catch (Renci.SshNet.Common.SshConnectionException exception)
            {
                Console.WriteLine("SshConnectionException, source: {0}", exception.StackTrace);
                setStatus("Failed to establish connection", Color.Red);
            }
            catch (Renci.SshNet.Common.SshOperationTimeoutException exception)
            {
                Console.WriteLine("SshConnectionException, source: {0}", exception.StackTrace);
                setStatus("Failed to establish connection", Color.Red);
            }
            catch (System.Net.Sockets.SocketException exception)
            {
                Console.WriteLine("SocketException, source: {0}", exception.StackTrace);
                setStatus("Failed to establish connection", Color.Red);
            }
            catch (Renci.SshNet.Common.SftpPermissionDeniedException exception)
            {
                Console.WriteLine("SftpPermissionDeniedException, source: {0}", exception.StackTrace);
                setStatus("Permission denied", Color.Red);
            }
            catch (Exception exception)
            {
                Console.WriteLine("Exception, source: {0}", exception.StackTrace);
                setStatus("Failed to load INI files", Color.Red);
            }

            Cursor = Cursors.Default;
        }

        private void ConfigurationView_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.menu.Close();
        }

        private void loadLocalButton_Click(object sender, EventArgs e)
        {
            fileDialog = new OpenFileDialog();
            fileDialog.InitialDirectory = System.Environment.GetFolderPath(Environment.SpecialFolder.MyComputer);
            fileDialog.FileName = "";
            fileDialog.Filter = "Initialization (*.ini)|*.ini";
            fileDialog.Title = "Select initialization files to load";
            fileDialog.Multiselect = true;

            if (Directory.Exists(Properties.Settings.Default.iniSavePath))
            {
                fileDialog.InitialDirectory = Properties.Settings.Default.iniSavePath;
            }

            if (fileDialog.ShowDialog() != DialogResult.OK) return;

            Cursor = Cursors.WaitCursor;
            foreach (string filename in fileDialog.FileNames)
            {
                using (System.IO.StreamReader fileReader = new System.IO.StreamReader(filename))
                {
                    try
                    {
                        addFile(new INI(Path.GetFileNameWithoutExtension(filename), fileReader));
                    }
                    catch
                    {
                        MessageBox.Show("Error loading file " + filename, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }

            inis.Last().loadTable(configurationGrid);
            configurationGrid_CellValidating(null, null);
            Cursor = Cursors.Default;
        }
        private void checkEnableStatuses()
        {
            saveToRioButton.Enabled = inis.Count > 0;
            saveLocalButton.Enabled = selectedIni != null;
            saveAllLocalButton.Enabled = inis.Count > 0;
            deleteButton.Enabled = selectedIni != null;
            configurationGrid.Enabled = filenameGrid.SelectedCells.Count > 0;
            if (configurationGrid.Enabled)
                configurationGrid_CellValidating(null, null);
            else
            {
                configurationGrid.Rows.Clear();
            }
        }

        private void filenameGrid_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            inis[e.RowIndex].loadTable(configurationGrid);
            selectedIni = inis[e.RowIndex];
            checkEnableStatuses();

        }

        private void configurationGrid_CellValidated(object sender, DataGridViewCellEventArgs e)
        {
            if (configurationGrid.SelectedCells.Count == 0
                || e.RowIndex == configurationGrid.RowCount - 1)
                return;
            if (e.RowIndex == selectedIni.variables.Count)
            {
                selectedIni.addVariable("");
            }
            int index = configurationGrid.SelectedCells[0].RowIndex;
            selectedIni.updateVariable(e.RowIndex, tryToString(configurationGrid.Rows[index].Cells[0].Value), tryToString(configurationGrid.Rows[index].Cells[1].Value), tryToString(configurationGrid.Rows[index].Cells[2].Value));
        }

        private void filenameGrid_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewCell cell = filenameGrid.Rows[e.RowIndex].Cells[e.ColumnIndex];
            cell.Value = cell.Value.ToString().Replace(" ", "");
            if (cell.Value.ToString().Length == 0) cell.Value = inis[e.RowIndex].fileName;
            else
            {
                inis[e.RowIndex].fileName = cell.Value.ToString();
            }
        }

        private void connectionSettingsButton_Click(object sender, EventArgs e)
        {
            Settings settings = new Settings();
            settings.Show();
        }

        /// <summary>
        /// Set the status message at the top of the field display
        /// </summary>
        private void setStatus(string message, Color color)
        {
            infoLabel.Text = message;
            infoLabel.ForeColor = color;
        }
        private void addFile(INI ini)
        {
            inis.Add(ini);
            int rowIndex = filenameGrid.Rows.Add(inis.Last());
            inis.Last().loadTable(configurationGrid);
            selectedIni = inis.Last();
            filenameGrid.ClearSelection();
            filenameGrid.Rows[rowIndex].Selected = true;
            checkEnableStatuses();
        }
        private void removeFile(int index)
        {
            inis.RemoveAt(index);
            filenameGrid.Rows.RemoveAt(index);
            if (filenameGrid.RowCount > 0)
            {
                filenameGrid.ClearSelection();
                if (index > 0)
                {
                    filenameGrid.Rows[index - 1].Selected = true;
                    inis[index - 1].loadTable(configurationGrid);
                    selectedIni = inis[index - 1];
                }
                else
                {
                    filenameGrid.Rows[0].Selected = true;
                    inis[index].loadTable(configurationGrid);
                    selectedIni = inis[index];
                }
            }
            checkEnableStatuses();
        }
        private void newFileButton_Click(object sender, EventArgs e)
        {
            addFile(new INI());
        }
        private void configurationGrid_UserDeletedRow(object sender, DataGridViewRowEventArgs e)
        {
            reloadVariablesFromTable();
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            configurationGrid.Rows.Remove(configurationGrid.SelectedRows[0]);
            reloadVariablesFromTable();
        }

        private void deleteButton_Click(object sender, EventArgs e)
        {
            removeFile(filenameGrid.SelectedCells[0].RowIndex);
        }

        private Rectangle dragBoxFromMouseDown;
        private int rowIndexFromMouseDown;
        private int rowIndexOfItemUnderMouseToDrop;

        private void configurationGrid_MouseMove(object sender, MouseEventArgs e)
        {
            if ((e.Button & MouseButtons.Left) == MouseButtons.Left)
            {
                // If the mouse moves outside the rectangle, start the drag.
                if (dragBoxFromMouseDown != Rectangle.Empty &&
                !dragBoxFromMouseDown.Contains(e.X, e.Y))
                {
                    // Proceed with the drag and drop, passing in the list item.                    
                    DragDropEffects dropEffect = configurationGrid.DoDragDrop(
                          configurationGrid.Rows[rowIndexFromMouseDown],
                          DragDropEffects.Move);
                }
            }
        }

        private void configurationGrid_MouseDown(object sender, MouseEventArgs e)
        {
            // Get the index of the item the mouse is below.
            rowIndexFromMouseDown = configurationGrid.HitTest(e.X, e.Y).RowIndex;

            if (rowIndexFromMouseDown != -1)
            {
                // Remember the point where the mouse down occurred. 
                // The DragSize indicates the size that the mouse can move 
                // before a drag event should be started.                
                Size dragSize = SystemInformation.DragSize;

                // Create a rectangle using the DragSize, with the mouse position being
                // at the center of the rectangle.
                dragBoxFromMouseDown = new Rectangle(
                          new Point(
                            e.X - (dragSize.Width / 2),
                            e.Y - (dragSize.Height / 2)),
                      dragSize);
            }
            else
                // Reset the rectangle if the mouse is not over an item in the ListBox.
                dragBoxFromMouseDown = Rectangle.Empty;
        }

        private void configurationGrid_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;
        }
        private void reloadVariablesFromTable()
        {
            selectedIni.clearVariables();
            foreach (DataGridViewRow row in configurationGrid.Rows)
            {
                if (row.Index >= configurationGrid.RowCount - 1) continue;
                selectedIni.addVariable(tryToString(row.Cells[0].Value), tryToString(row.Cells[1].Value), tryToString(row.Cells[2].Value));
            }
        }
        private void configurationGrid_DragDrop(object sender, DragEventArgs e)
        {
            // The mouse locations are relative to the screen, so they must be 
            // converted to client coordinates.
            Point clientPoint = configurationGrid.PointToClient(new Point(e.X, e.Y));

            // Get the row index of the item the mouse is below. 
            rowIndexOfItemUnderMouseToDrop = configurationGrid.HitTest(clientPoint.X, clientPoint.Y).RowIndex;

            // If the drag operation was a move then remove and insert the row.
            if (e.Effect == DragDropEffects.Move)
            {
                DataGridViewRow rowToMove = e.Data.GetData(typeof(DataGridViewRow)) as DataGridViewRow;
                configurationGrid.Rows.RemoveAt(rowIndexFromMouseDown);
                configurationGrid.Rows.Insert(rowIndexOfItemUnderMouseToDrop, rowToMove);

            }
            reloadVariablesFromTable();
        }

        private String tryToString(object o)
        {
            if (o != null)
            {
                return o.ToString();
            }
            return "";
        }

        private void configurationGrid_Sorted(object sender, EventArgs e)
        {
            reloadVariablesFromTable();
        }


        private void rowContextMenuStrip_Opening(object sender, CancelEventArgs e)
        {
            if (configurationGrid.SelectedRows.Count < 1) { e.Cancel = true; return; }
            if (configurationGrid.SelectedRows[0].Index == configurationGrid.RowCount - 1) e.Cancel = true;
        }

        private void configurationGrid_RowHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {

                configurationGrid.ClearSelection();
                configurationGrid.Rows[e.RowIndex].Selected = true;

                rowContextMenuStrip.Show(Cursor.Position);

            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            TimeSpan ts = DateTime.Now - timeOfUpload;


            timeSinceUpload.Text = "Last Upload: " + ts.ToString("h'h 'm'm 's's'");
        }

        private void iconButton2_Click(object sender, EventArgs e)
        {
            menu.mp.Show();
            this.Hide();
        }
    }
}
