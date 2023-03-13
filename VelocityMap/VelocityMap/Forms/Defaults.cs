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
    public partial class Defaults : Form
    {
        public Defaults()
        {
            InitializeComponent();
            this.defMaxVelInput.Text = Properties.Settings.Default.MaxVel.ToString();
            this.defMaxAccInput.Text = Properties.Settings.Default.MaxAcc.ToString();
            this.defMaxRotVelInput.Text = Properties.Settings.Default.MaxRotVel.ToString();
            this.defMaxRotAccInput.Text = Properties.Settings.Default.MaxRotAcc.ToString();
        }

        private void save_Click(object sender, EventArgs e)
        {
            try
            {
                Properties.Settings.Default.MaxVel = double.Parse(defMaxVelInput.Text);
                Properties.Settings.Default.Save();
            }
            catch (Exception)
            {
                MessageBox.Show("Max velocity must be a number", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            try
            {
                Properties.Settings.Default.MaxAcc = double.Parse(defMaxAccInput.Text);
                Properties.Settings.Default.Save();
            }
            catch (Exception)
            {
                MessageBox.Show("Max acceleration must be a number", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            try
            {
                Properties.Settings.Default.MaxRotVel = double.Parse(defMaxRotVelInput.Text);
                Properties.Settings.Default.Save();
            }
            catch (Exception)
            {
                MessageBox.Show("Max rotational velocity must be a number", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            try
            {
                Properties.Settings.Default.MaxRotAcc = double.Parse(defMaxRotAccInput.Text);
                Properties.Settings.Default.Save();
            }
            catch (Exception)
            {
                MessageBox.Show("Max rotational acceleration must be a number", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            this.Close();
        }

        private void cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
