using Renci.SshNet;
using Renci.SshNet.Sftp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VelocityMap.Configuration;

namespace VelocityMap.Forms
{
    public partial class ConfigurationView : Form
    {

        private Action closeMain;

        public ConfigurationView(Action closeMain)
        {
            this.closeMain = closeMain;
            InitializeComponent();
        }

        private void dataGridView3_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            Console.WriteLine("Validating");
            List<String> list = new List<String>();
            for(int i = 0; i < configurationGrid.Rows.Count; i++)
            {
                DataGridViewRow row = configurationGrid.Rows[i];
                if(row.Cells[0].EditedFormattedValue.ToString() == "")
                {
                    continue;
                }
                if (list.Contains(row.Cells[0].EditedFormattedValue.ToString()))
                    row.ErrorText = "Variable name is already in use.";
                else
                    row.ErrorText = "";
                list.Add(row.Cells[0].EditedFormattedValue.ToString());


                if(row.Cells[1].EditedFormattedValue.ToString() == "Boolean" && !row.Cells[1].EditedFormattedValue.Equals(row.Cells[1].Value))
                {
                    DataGridViewCheckBoxCell cell = new DataGridViewCheckBoxCell();
                    cell.Value = false;
                    row.Cells[2] = cell;
                }
                if (row.Cells[1].EditedFormattedValue.ToString() == "Int" && !row.Cells[1].EditedFormattedValue.Equals(row.Cells[1].Value))
                {
                    DataGridViewTextBoxCell cell = new DataGridViewTextBoxCell();
                    cell.Value = "";
                    row.Cells[2] = cell;
                }
                if (row.Cells[1].EditedFormattedValue.ToString() == "Double" && !row.Cells[1].EditedFormattedValue.Equals(row.Cells[1].Value))
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

            }
        }
        private void dataGridView1_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (configurationGrid.CurrentCell == null)
            {
                return;
            }
            e.Control.KeyPress -= new KeyPressEventHandler(Column3_KeyPress_NULL);
            e.Control.KeyPress -= new KeyPressEventHandler(Column3_KeyPress_Int);
            e.Control.KeyPress -= new KeyPressEventHandler(Column3_KeyPress_Double);
            e.Control.KeyPress -= new KeyPressEventHandler(Column3_KeyPress_VariableName);

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
                else if (configurationGrid.CurrentRow.Cells[1].Value.ToString() == "Int")
                {
                    TextBox tb = e.Control as TextBox;
                    if (tb != null)
                    {
                        tb.KeyPress += new KeyPressEventHandler(Column3_KeyPress_Int);
                    }
                }
                else if (configurationGrid.CurrentRow.Cells[1].Value.ToString() == "Double")
                {
                    TextBox tb = e.Control as TextBox;
                    if (tb != null)
                    {
                        tb.KeyPress += new KeyPressEventHandler(Column3_KeyPress_Double);
                    }
                }
                
            }
        }

        private void Column3_KeyPress_Double(object sender, KeyPressEventArgs e)
        {
            // allows 0-9, backspace, and decimal
            if (((e.KeyChar < 48 || e.KeyChar > 57) && e.KeyChar != 8 && e.KeyChar != 46))
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
        }

        
        private void Column3_KeyPress_VariableName(object sender, KeyPressEventArgs e)
        {
            if ((sender as TextBox).TextLength == 0)
            {
                if (!char.IsLetter(e.KeyChar))
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
            if (((e.KeyChar < 48 || e.KeyChar > 57) && e.KeyChar != 8))
            {
                e.Handled = true;
                return;
            }

        }

        private void saveToRioButton_Click(object sender, EventArgs e)
        {
            
        }

        private void deleteProfileButton_Click(object sender, EventArgs e)
        {

        }

        private void newProfileButton_Click(object sender, EventArgs e)
        {

        }

        private void refresh_button_Click(object sender, EventArgs e)
        {

            Cursor = Cursors.WaitCursor;
            ConnectionInfo info = new ConnectionInfo(Properties.Settings.Default.IpAddress,
                Properties.Settings.Default.Username, new PasswordAuthenticationMethod(Properties.Settings.Default.Username, Properties.Settings.Default.Password));

            info.Timeout = TimeSpan.FromSeconds(5);

            SftpClient sftp = new SftpClient(info);
            /*SftpClient sftp = new SftpClient(
                Properties.Settings.Default.IpAddress,
                Properties.Settings.Default.Username,
                Properties.Settings.Default.Password
            );*/
            try
            {
                setStatus("Establishing RIO connection...", false);
                sftp.Connect();

                if (!sftp.Exists(Properties.Settings.Default.RioLocation))
                {
                    sftp.CreateDirectory(Properties.Settings.Default.RioLocation);
                    setStatus("No motion profiles found at RIO directory", false);
                    return;
                }

                bool foundFiles = false;
                foreach (SftpFile file in sftp.ListDirectory(Properties.Settings.Default.RioLocation))
                {
                    if (!file.Name.Contains(".mp")) continue;
                    foundFiles = true;

                    StreamReader reader = sftp.OpenText(file.FullName);
                    //profiles.Add(new Profile(JObject.Parse(reader.ReadToEnd())));
                    //profileTable.Rows.Add(profiles.Last().Name, profiles.Last().Edited);
                }
                if (foundFiles) setStatus("Profiles loaded from RIO", false);
                else setStatus("No motion profiles found at RIO directory", false);

                sftp.Disconnect();
            }
            catch (Renci.SshNet.Common.SshConnectionException exception)
            {
                Console.WriteLine("SshConnectionException, source: {0}", exception.StackTrace);
                setStatus("Failed to establish connection", true);
            }
            catch (Renci.SshNet.Common.SshOperationTimeoutException exception)
            {
                Console.WriteLine("SshConnectionException, source: {0}", exception.StackTrace);
                setStatus("Failed to establish connection", true);
            }
            catch (System.Net.Sockets.SocketException exception)
            {
                Console.WriteLine("SocketException, source: {0}", exception.StackTrace);
                setStatus("Failed to establish connection", true);
            }
            catch (Renci.SshNet.Common.SftpPermissionDeniedException exception)
            {
                Console.WriteLine("SftpPermissionDeniedException, source: {0}", exception.StackTrace);
                setStatus("Permission denied", true);
            }
            catch (Exception exception)
            {
                Console.WriteLine("Exception, source: {0}", exception.StackTrace);
                setStatus("Failed to load RIO profiles", true);
            }

            Cursor = Cursors.Default;
        }

        private void ConfigurationView_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.closeMain();
        }
        /// <summary>
        /// Set the status message at the top of the field display
        /// </summary>
        private void setStatus(string message, bool error)
        {
            infoLabel.Text = message;
            infoLabel.ForeColor = error ? Color.Red : Color.Black;
        }

        private void configFileList_SelectionChanged(object sender, EventArgs e)
        {
            if(configFileList.SelectedCells.Count>0)
            {
                configurationGrid.Enabled = true;
            }
        }
    }
}
