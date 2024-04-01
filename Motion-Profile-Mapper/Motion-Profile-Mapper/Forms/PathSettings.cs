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

namespace MotionProfileMapper.Forms
{
    public partial class PathSettings : Form
    {
        ProfilePath path;
        DataGridViewCell pathTableCell;
        public PathSettings(ProfilePath path, DataGridViewCell pathTable)
        {
            InitializeComponent();
            this.pathMaxVelInput.Text = path.MaxVel.ToString();
            this.pathMaxAccInput.Text = path.MaxAcc.ToString();
            this.pathMaxCenAccInput.Text = path.MaxCen.ToString();
            this.pathInVelInput.Text = path.InVel.ToString();
            this.pathOutVelInput.Text = path.OutVel.ToString();
            this.path = path;
            this.Text = path.Name + " Settings";
            this.pathNameInput.Text = path.Name;
            this.pathTableCell = pathTable;
            this.snapToPrevBox.Checked = path.SnapToPrevious;

            ActiveControl = pathNameInput;
        }

        private void save_Click(object sender, EventArgs e)
        {
            double maxvel;
            double maxacc;
            double maxcen;
            double invel;
            double outvel;
            bool a = double.TryParse(pathMaxVelInput.Text, out maxvel);
            bool b = double.TryParse(pathMaxAccInput.Text, out maxacc);
            bool c = double.TryParse(pathMaxCenAccInput.Text, out maxcen);
            bool d = double.TryParse(pathOutVelInput.Text, out outvel);
            bool f = double.TryParse(pathInVelInput.Text, out invel);

            if (!a || !b || !c || !d || !f)
            {
                MessageBox.Show("Incorrect Data Types", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            this.path.updateAll(this.pathNameInput.Text, this.snapToPrevBox.Checked, maxvel, invel, outvel, maxacc, maxcen);
           
            //this.pathTableCell.Value = this.path.Name;

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
