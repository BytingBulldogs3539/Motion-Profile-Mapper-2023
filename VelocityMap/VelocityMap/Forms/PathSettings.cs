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
        public PathSettings(ProfilePath path)
        {
            InitializeComponent();
            this.pathMaxVelInput.Text = path.maxVel.ToString();
            this.pathMaxAccInput.Text = path.maxAcc.ToString();
            this.path = path;
            this.Text = path.Name + " Constraints";
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

            this.Close();
        }
    }
}
