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
    public partial class ShiftPath : Form
    {
        private ProfilePath path;
        private Action<int> selectPath;
        public ShiftPath(ProfilePath path, Action<int> selectPath)
        {
            InitializeComponent();
            this.path = path;
            this.selectPath = selectPath;
        }

        private void cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void shiftButton_Click(object sender, EventArgs e)
        {
            double dx, dy;
            if (double.TryParse(dxInput.Text == "" ? "0.0" : dxInput.Text, out dx) && double.TryParse(dyInput.Text == "" ? "0.0" : dyInput.Text, out dy))
            {
                this.path.shiftPoints(
                    dx, dy
                );
                this.selectPath(-1);
            }
            else
            {
                MessageBox.Show("Shift values must be numbers", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            this.Close();
        }

        private void dxInput_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter) shiftButton_Click(null, null);
        }

        private void dyInput_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter) shiftButton_Click(null, null);
        }
    }
}
