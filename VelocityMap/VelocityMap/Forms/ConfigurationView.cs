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
using VelocityMap.Utilities;

namespace VelocityMap.Forms
{
    public partial class ConfigurationView : Form
    {

        private Action closeMain;
        private OpenFileDialog fileDialog;
        private List<INI> inis = new List<INI>();
        private INI selectedIni = null;

        public ConfigurationView(Action closeMain)
        {
            this.closeMain = closeMain;
            InitializeComponent();
        }

        private void configurationGrid_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
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

        private void ConfigurationView_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.closeMain();
        }

        private void loadLocalButton_Click(object sender, EventArgs e)
        {
            fileDialog = new OpenFileDialog();
            fileDialog.InitialDirectory = System.Environment.GetFolderPath(Environment.SpecialFolder.MyComputer);
            fileDialog.FileName = "";
            fileDialog.Filter = "Initialization (*.ini)|*.ini";
            fileDialog.Title = "Select initialization files to load";
            fileDialog.Multiselect = true;

            if (fileDialog.ShowDialog() != DialogResult.OK) return;

            Cursor = Cursors.WaitCursor;
            foreach (string filename in fileDialog.FileNames)
            {
                using (System.IO.StreamReader fileReader = new System.IO.StreamReader(filename))
                {
                    try
                    {
                        inis.Add(new INI(Path.GetFileName(filename), fileReader));
                        filenameGrid.Rows.Add(Path.GetFileName(filename));
                    }
                    catch
                    {
                        MessageBox.Show("Error loading file " + filename, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }

            inis.Last().loadTable(configurationGrid);
                
            Cursor = Cursors.Default;
        }

        private void filenameGrid_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            inis[e.RowIndex].loadTable(configurationGrid);
            selectedIni = inis[e.RowIndex];
        }

        private void configurationGrid_CellValidated(object sender, DataGridViewCellEventArgs e)
        {
            if (configurationGrid.SelectedCells.Count == 0 || e.RowIndex == configurationGrid.RowCount - 1) return;
            switch (e.ColumnIndex)
            {
                case 0:
                    selectedIni.changeVariableName(e.RowIndex, configurationGrid.SelectedCells[0].Value.ToString());
                    break;
                case 1:
                    selectedIni.updateValue(e.RowIndex, "Type", configurationGrid.SelectedCells[0].Value.ToString());
                    break;
                case 2:
                    selectedIni.updateValue(e.RowIndex, "Value", configurationGrid.SelectedCells[0].Value.ToString());
                    break;
            }
        }

        private void filenameGrid_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewCell cell = filenameGrid.Rows[e.RowIndex].Cells[e.ColumnIndex];
            cell.Value = cell.Value.ToString().Replace(" ", "");
            if (!cell.Value.ToString().EndsWith(".ini") || cell.Value.ToString().Length <= 4) cell.Value = inis[e.RowIndex].fileName;
            else
            {
                inis[e.RowIndex].fileName = cell.Value.ToString();
                inis[e.RowIndex].variableClass = cell.Value.ToString().Substring(0, cell.Value.ToString().IndexOf('.'));
            }
        }

        private void connectionSettingsButton_Click(object sender, EventArgs e)
        {
            Settings settings = new Settings();
            settings.Show();
        }

        private void saveLocalButton_Click(object sender, EventArgs e)
        {
            SaveFileDialog browser = new SaveFileDialog();
            browser.InitialDirectory = System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            browser.FileName = selectedIni.fileName;
            browser.Filter = "Initialization (*.ini)|*.ini";
            browser.Title = "Save initialization file locally";

            if (browser.ShowDialog() != DialogResult.OK || browser.FileName.Trim().Length <= 4) return;

            Cursor = Cursors.WaitCursor;
            string filePath = Path.Combine(
                Path.GetDirectoryName(browser.FileName.Trim()),
                Path.GetFileNameWithoutExtension(browser.FileName.Trim()) + ".ini"
            );
            using (var writer = new StreamWriter(filePath))
            {
                writer.Write(selectedIni.toIni());
            }

            Cursor = Cursors.Default;
        }
    }
}
