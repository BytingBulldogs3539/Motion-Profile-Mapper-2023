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
    public partial class ShiftPath : Form
    {
        private ProfilePath path;
        private Action<int> selectPath;
        public ShiftPath(ProfilePath path, Action<int> updateField)
        {
            InitializeComponent();
            this.path = path;
            this.selectPath = updateField;
        }

        private void cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void shiftButton_Click(object sender, EventArgs e)
        {
            try
            {
                this.path.shiftPoints(
                    double.Parse(dxInput.Text == ""? "0.0" : dxInput.Text),
                    double.Parse(dyInput.Text == ""? "0.0" : dyInput.Text)
                );
                this.selectPath(-1);
            }
            catch (Exception)
            {
                MessageBox.Show("Shift values must be numbers", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
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
