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
    public partial class MirrorPath : Form
    {
        Profile selectedProfile;
        ProfilePath selectedPath;
        Action<int> selectPath;
        double fieldWidth;
        public MirrorPath(Profile selectedProfile, ProfilePath selectedPath, Action<int> selectPath, double fieldWidth)
        {
            InitializeComponent();
            this.selectedPath = selectedPath;
            this.selectedProfile = selectedProfile;
            this.selectPath = selectPath;
            this.fieldWidth = fieldWidth;
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void mirrorSelectedButton_Click(object sender, EventArgs e)
        {
            this.selectedProfile.mirrorPath(this.selectedPath, this.fieldWidth);
            this.selectPath(-1);
            this.Close();
        }

        private void mirrorAllButton_Click(object sender, EventArgs e)
        {
            this.selectedProfile.mirrorAllPaths(fieldWidth);
            this.selectPath(-1);
            this.Close();
        }
    }
}
