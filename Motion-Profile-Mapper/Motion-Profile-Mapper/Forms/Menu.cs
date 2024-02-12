using Octokit;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MotionProfileMapper.Forms
{
    public partial class Menu : Form
    {
        public MotionProfileMapper.MotionProfiler mp;
        public ConfigurationView constants;


        public Menu()
        {
            mp = new MotionProfileMapper.MotionProfiler(this);
            constants = new ConfigurationView(this);

            InitializeComponent();
            bool internet = checkForInternet();
            if (Properties.Settings.Default.autoCheckForUpdates && internet)
            {
                GitHubClient client = new GitHubClient(new ProductHeaderValue("Motion-Profile-Mapper", System.Windows.Forms.Application.ProductVersion));

                Task<Release> task = client.Repository.Release.GetLatest("BytingBulldogs3539", "Motion-Profile-Mapper");
                task.Wait();
                var latest = task.Result;

                //Setup the versions
                Version latestGitHubVersion = new Version(latest.TagName);
                Version localVersion = new Version(System.Windows.Forms.Application.ProductVersion);

                //Compare the Versions
                //Source: https://stackoverflow.com/questions/7568147/compare-version-numbers-without-using-split-function
                int versionComparison = localVersion.CompareTo(latestGitHubVersion);
                if (versionComparison < 0)
                {
                    if (MessageBox.Show(
                        "Update available would you like to download?",
                        "Update Available",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Asterisk) == DialogResult.Yes)
                    {
                        System.Diagnostics.Process.Start("https://github.com/BytingBulldogs3539/Motion-Profile-Mapper/releases/latest");
                    }
                }
            }
            else if(!internet)
            {
                MessageBox.Show("Update Check Failed", "Update Check Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool checkForInternet()
        {
            try
            {
                Ping myPing = new Ping();
                String host = "google.com";
                byte[] buffer = new byte[32];
                int timeout = 1000;
                PingOptions pingOptions = new PingOptions();
                PingReply reply = myPing.Send(host, timeout, buffer, pingOptions);
                return (reply.Status == IPStatus.Success);
            }
            catch (Exception)
            {
                return false;
            }
        }

        private void mpButton_Click(object sender, EventArgs e)
        {
            mp.Show();
            this.Hide();
        }

        private void constantsButton_Click(object sender, EventArgs e)
        {
            constants.Show();
            this.Hide();
        }
    }
}
