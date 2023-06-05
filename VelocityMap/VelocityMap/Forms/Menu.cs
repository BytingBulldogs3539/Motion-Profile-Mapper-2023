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
    public partial class Menu : Form
    {
        public Menu()
        {
            InitializeComponent();
        }

        private void mpButton_Click(object sender, EventArgs e)
        {
            VelocityMap.MotionProfiler mp = new VelocityMap.MotionProfiler(this.Close);
            mp.Show();
            this.Hide();
        }

        private void constantsButton_Click(object sender, EventArgs e)
        {
            ConfigurationView constants = new ConfigurationView(this.Close);
            constants.Show();
            this.Hide();
        }

        private void poseVisualizer_Click(object sender, EventArgs e)
        {
            PoseVisualizer poseVisualizer = new PoseVisualizer(this.Close);
            poseVisualizer.Show();
            this.Hide();
        }
    }
}
