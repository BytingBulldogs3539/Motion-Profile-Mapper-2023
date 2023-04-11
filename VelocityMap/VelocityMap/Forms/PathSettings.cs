using MotionProfile.SegmentedProfile;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VelocityMap.Forms
{
    public partial class PathSettings : Form
    {
        ProfilePath path;
        DataGridViewCell pathTableCell;
        public PathSettings(ProfilePath path, DataGridViewCell pathTable)
        {
            InitializeComponent();
            this.pathMaxVelInput.Text = path.maxVel.ToString();
            this.pathMaxAccInput.Text = path.maxAcc.ToString();
            this.path = path;
            this.Text = path.Name + " Settings";
            this.pathNameInput.Text = path.Name;
            this.pathTableCell = pathTable;
            this.snapToPrevBox.Checked = path.snapToPrevious;

            ActiveControl = pathNameInput;
        }

        private void save_Click(object sender, EventArgs e)
        {
            try
            {
                path.maxVel = double.Parse(pathMaxVelInput.Text);
            }
            catch (Exception)
            {
                MessageBox.Show("Max velocity must be a number", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            try
            {
                path.maxAcc = double.Parse(pathMaxAccInput.Text);
            }
            catch (Exception)
            {
                MessageBox.Show("Max acceleration must be a number", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            this.path.Name = this.pathNameInput.Text;
            this.pathTableCell.Value = this.path.Name;
            this.path.snapToPrevious = this.snapToPrevBox.Checked;

            this.Close();
        }

        private void cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void pathNameInput_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter) save_Click(null, null);
        }

        private void pathMaxVelInput_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter) save_Click(null, null);
        }

        private void pathMaxAccInput_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter) save_Click(null, null);
        }
    }
}
