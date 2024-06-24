﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MotionProfileMapper {
    public partial class Settings : Form {
        public Settings() {
            InitializeComponent();
            this.ipaddress.Text = Properties.Settings.Default.IpAddress;
            this.username.Text = Properties.Settings.Default.Username;
            this.password.Text = Properties.Settings.Default.Password;
            this.riopath.Text = Properties.Settings.Default.RioLocation;
            this.iniPath.Text = Properties.Settings.Default.INILocation;
            this.checkBox1.Checked = Properties.Settings.Default.defaultAllianceIsRed;
            this.checkBox2.Checked = Properties.Settings.Default.autoCheckForUpdates;
            this.iniSavePath.Text = Properties.Settings.Default.iniSavePath;
            this.javaSavePath.Text = Properties.Settings.Default.javaSavePath;
            this.mpSavePath.Text = Properties.Settings.Default.mpSavePath;

            this.defMaxVelInput.Text = Properties.Settings.Default.MaxVel.ToString();
            this.defMaxAccInput.Text = Properties.Settings.Default.MaxAcc.ToString();
            this.defMaxCenAccInput.Text = Properties.Settings.Default.MaxCen.ToString();
            this.frameLengthInput.Text = Properties.Settings.Default.FrameLength.ToString();
            this.frameWidthInput.Text = Properties.Settings.Default.FrameWidth.ToString();
            this.defMaxRotVelInput.Text = Properties.Settings.Default.MaxRotVel.ToString();
            this.defMaxRotAccInput.Text = Properties.Settings.Default.MaxRotAcc.ToString();
            this.snapPathsCheckbox.Checked = Properties.Settings.Default.SnapNewPaths;
        }

        private void save_Click(object sender, EventArgs e) {
            if (!ValidateIPv4(this.ipaddress.Text)) {
                MessageBox.Show("This ip address is invalid!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            Properties.Settings.Default.IpAddress = this.ipaddress.Text;
            Properties.Settings.Default.Username = this.username.Text;
            Properties.Settings.Default.Password = this.password.Text;
            Properties.Settings.Default.RioLocation = this.riopath.Text + ( this.riopath.Text.Last().ToString() == "/" ? "" : "/" );
            Properties.Settings.Default.INILocation = this.iniPath.Text + ( this.iniPath.Text.Last().ToString() == "/" ? "" : "/" );
            Properties.Settings.Default.defaultAllianceIsRed = this.checkBox1.Checked;
            Properties.Settings.Default.autoCheckForUpdates = this.checkBox2.Checked;

            Properties.Settings.Default.iniSavePath = iniSavePath.Text;
            Properties.Settings.Default.javaSavePath = javaSavePath.Text;
            Properties.Settings.Default.mpSavePath = mpSavePath.Text;

            try {
                Properties.Settings.Default.MaxVel = double.Parse(defMaxVelInput.Text);
            } catch (Exception) {
                MessageBox.Show("Max velocity must be a number", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try {
                Properties.Settings.Default.FrameLength = double.Parse(this.frameLengthInput.Text);
            } catch (Exception) {
                MessageBox.Show("Max velocity must be a number", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            try {
                Properties.Settings.Default.FrameWidth = double.Parse(this.frameWidthInput.Text);
            } catch (Exception) {
                MessageBox.Show("Max velocity must be a number", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            try {
                Properties.Settings.Default.MaxAcc = double.Parse(defMaxAccInput.Text);
            } catch (Exception) {
                MessageBox.Show("Max acceleration must be a number", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try {
                Properties.Settings.Default.MaxCen = double.Parse(defMaxCenAccInput.Text);
            } catch (Exception) {
                MessageBox.Show("Max centripetal  acceleration must be a number", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try {
                Properties.Settings.Default.MaxRotVel = double.Parse(defMaxRotVelInput.Text);
            } catch (Exception) {
                MessageBox.Show("Max rotational velocity must be a number", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            try {
                Properties.Settings.Default.MaxRotAcc = double.Parse(defMaxRotAccInput.Text);
            } catch (Exception) {
                MessageBox.Show("Max rotational acceleration must be a number", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            Properties.Settings.Default.SnapNewPaths = snapPathsCheckbox.Checked;
            Properties.Settings.Default.Save();

            this.Close();
        }

        /// <summary>
        /// Used to validate the ip address of the robot to make sure that it is in an ipv4 format.
        /// </summary>
        /// <param name="ipString">The ip string value.</param>
        /// <returns>a boolean that tells you if the ip is in ipv4 format.</returns>
        public bool ValidateIPv4(string ipString) {
            // if the text contains a whitespace/space or a null value then it is clearly not a ip address.
            if (String.IsNullOrWhiteSpace(ipString)) {
                return false;
            }
            //Split the ip address into different parts
            string[] splitValues = ipString.Split('.');
            if (splitValues.Length != 4) {
                return false;
            }

            byte tempForParsing;
            //check to see if all of the values are bytes.
            return splitValues.All(r => byte.TryParse(r, out tempForParsing));
        }

        private void cancel_Click(object sender, EventArgs e) {
            this.Close();
        }

        private void iconButton1_Click(object sender, EventArgs e) {
            SaveFileDialog browser = new SaveFileDialog();
            browser.RestoreDirectory = true;
            browser.Filter = "Directory | directory";
            browser.Title = "Directory to save INI Files to";
            browser.OverwritePrompt = false;
            browser.FileName = "DoNotChangeMe.ini";

            if (browser.ShowDialog() != DialogResult.OK) return;

            iniSavePath.Text = Path.GetDirectoryName(browser.FileName.Trim());
        }

        private void iconButton2_Click(object sender, EventArgs e) {
            SaveFileDialog browser = new SaveFileDialog();
            browser.RestoreDirectory = true;
            browser.Filter = "Directory | directory";
            browser.Title = "Directory to save JAVA Files to";
            browser.OverwritePrompt = false;
            browser.FileName = "DoNotChangeMe.java";

            if (browser.ShowDialog() != DialogResult.OK) return;

            javaSavePath.Text = Path.GetDirectoryName(browser.FileName.Trim());
        }

        private void iconButton3_Click(object sender, EventArgs e) {
            SaveFileDialog browser = new SaveFileDialog();
            browser.RestoreDirectory = true;
            browser.Filter = "Directory | directory";
            browser.Title = "Directory to save MP Files to";
            browser.OverwritePrompt = false;
            browser.FileName = "DoNotChangeMe.mp";

            if (browser.ShowDialog() != DialogResult.OK) return;

            mpSavePath.Text = Path.GetDirectoryName(browser.FileName.Trim());
        }
    }
}
