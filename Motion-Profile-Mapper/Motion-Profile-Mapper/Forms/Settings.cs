using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MotionProfileMapper {
    public partial class Settings : Form {
        public Settings() {
            InitializeComponent();
            this.teamNumber.Text = Properties.Settings.Default.teamNumber.ToString();
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


        public static string getRioStaticIp(int teamNumber) {
            return String.Format("10.{0:D2}.{1:D2}.2", teamNumber / 100, teamNumber % 100);

        }
        public static string GetRobotIpAddress() {
            string[] possibleIps = new string[]
            {
                getRioStaticIp(Properties.Settings.Default.teamNumber),
                "roboRIO-"+Properties.Settings.Default.teamNumber+"-FRC.local",
                "172.22.11.2",
                "roboRIO-"+Properties.Settings.Default.teamNumber+"-FRC.lan",
                "roboRIO-"+Properties.Settings.Default.teamNumber+"-FRC.frc-field.local"
            };

            Debug.WriteLine("Possible IPs:");
            foreach (string ip in possibleIps) {
                Debug.WriteLine(ip);
            }

            foreach (string ip in possibleIps) {
                using (Ping ping = new Ping()) {
                    try {
                        PingReply reply = ping.Send(ip, 1000);
                        if (reply.Status == IPStatus.Success) {
                            return ip;
                        }
                    } catch (PingException) {
                        // Ignore and try next IP
                    }
                }
            }

            return null;
        }


        private void save_Click(object sender, EventArgs e) {
            if (!ValidateTeamNumber(this.teamNumber.Text)) {
                MessageBox.Show("This team number is invalid!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            Properties.Settings.Default.teamNumber = int.Parse(this.teamNumber.Text);
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
        /// Used to validate the team number to make sure that it is up to 5 digits.
        /// </summary>
        /// <param name="teamNumberString">The team number string value.</param>
        /// <returns>a boolean that tells you if the team number is valid.</returns>
        public bool ValidateTeamNumber(string teamNumberString) {
            return int.TryParse(teamNumberString, out int teamNumber) && teamNumberString.Length <= 5;
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
