
namespace MotionProfileMapper.Forms
{
    partial class PoseVisualizer
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series2 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series3 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series4 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PoseVisualizer));
            this.loadFromRIOButton = new System.Windows.Forms.Button();
            this.rioConectionButton = new FontAwesome.Sharp.IconButton();
            this.trackBar1 = new System.Windows.Forms.TrackBar();
            this.mainField = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.GridCheckBox = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.mainField)).BeginInit();
            this.SuspendLayout();
            // 
            // loadFromRIOButton
            // 
            this.loadFromRIOButton.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.loadFromRIOButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.loadFromRIOButton.FlatAppearance.BorderSize = 0;
            this.loadFromRIOButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.loadFromRIOButton.Font = new System.Drawing.Font("Verdana", 16.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.loadFromRIOButton.ForeColor = System.Drawing.Color.Teal;
            this.loadFromRIOButton.Location = new System.Drawing.Point(12, 610);
            this.loadFromRIOButton.Name = "loadFromRIOButton";
            this.loadFromRIOButton.Size = new System.Drawing.Size(253, 50);
            this.loadFromRIOButton.TabIndex = 58;
            this.loadFromRIOButton.Text = "Load from RIO";
            this.loadFromRIOButton.UseVisualStyleBackColor = false;
            this.loadFromRIOButton.Click += new System.EventHandler(this.loadFromRIOButton_Click);
            // 
            // rioConectionButton
            // 
            this.rioConectionButton.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.rioConectionButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.rioConectionButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.rioConectionButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rioConectionButton.ForeColor = System.Drawing.SystemColors.AppWorkspace;
            this.rioConectionButton.IconChar = FontAwesome.Sharp.IconChar.Cog;
            this.rioConectionButton.IconColor = System.Drawing.Color.Teal;
            this.rioConectionButton.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.rioConectionButton.IconSize = 25;
            this.rioConectionButton.Location = new System.Drawing.Point(268, 610);
            this.rioConectionButton.Margin = new System.Windows.Forms.Padding(0);
            this.rioConectionButton.Name = "rioConectionButton";
            this.rioConectionButton.Size = new System.Drawing.Size(54, 50);
            this.rioConectionButton.TabIndex = 59;
            this.rioConectionButton.UseVisualStyleBackColor = false;
            // 
            // trackBar1
            // 
            this.trackBar1.Location = new System.Drawing.Point(12, 527);
            this.trackBar1.Name = "trackBar1";
            this.trackBar1.Size = new System.Drawing.Size(1044, 45);
            this.trackBar1.TabIndex = 63;
            // 
            // mainField
            // 
            this.mainField.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.mainField.BackColor = System.Drawing.SystemColors.ControlLight;
            chartArea1.BackImageWrapMode = System.Windows.Forms.DataVisualization.Charting.ChartImageWrapMode.Scaled;
            chartArea1.Name = "field";
            this.mainField.ChartAreas.Add(chartArea1);
            this.mainField.Cursor = System.Windows.Forms.Cursors.Cross;
            this.mainField.Location = new System.Drawing.Point(2, 2);
            this.mainField.Margin = new System.Windows.Forms.Padding(3, 0, 3, 3);
            this.mainField.Name = "mainField";
            series1.ChartArea = "field";
            series1.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Point;
            series1.MarkerBorderColor = System.Drawing.Color.Transparent;
            series1.MarkerColor = System.Drawing.Color.Transparent;
            series1.Name = "background";
            series2.ChartArea = "field";
            series2.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series2.Color = System.Drawing.Color.LightGray;
            series2.MarkerSize = 2;
            series2.Name = "right";
            series3.ChartArea = "field";
            series3.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series3.Color = System.Drawing.Color.LightGray;
            series3.MarkerSize = 2;
            series3.Name = "left";
            series4.ChartArea = "field";
            series4.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series4.Color = System.Drawing.Color.Aqua;
            series4.MarkerSize = 4;
            series4.Name = "path";
            this.mainField.Series.Add(series1);
            this.mainField.Series.Add(series2);
            this.mainField.Series.Add(series3);
            this.mainField.Series.Add(series4);
            this.mainField.Size = new System.Drawing.Size(1042, 519);
            this.mainField.TabIndex = 64;
            this.mainField.Text = "chart2";
            // 
            // GridCheckBox
            // 
            this.GridCheckBox.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.GridCheckBox.AutoSize = true;
            this.GridCheckBox.BackColor = System.Drawing.SystemColors.ControlLight;
            this.GridCheckBox.Cursor = System.Windows.Forms.Cursors.Hand;
            this.GridCheckBox.Font = new System.Drawing.Font("Verdana", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.GridCheckBox.ForeColor = System.Drawing.Color.Black;
            this.GridCheckBox.Location = new System.Drawing.Point(856, 629);
            this.GridCheckBox.Margin = new System.Windows.Forms.Padding(1);
            this.GridCheckBox.Name = "GridCheckBox";
            this.GridCheckBox.Size = new System.Drawing.Size(83, 17);
            this.GridCheckBox.TabIndex = 65;
            this.GridCheckBox.Text = "Show grid";
            this.GridCheckBox.UseVisualStyleBackColor = false;
            this.GridCheckBox.CheckedChanged += new System.EventHandler(this.GridCheckBox_CheckedChanged);
            // 
            // PoseVisualizer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlLight;
            this.ClientSize = new System.Drawing.Size(1068, 674);
            this.Controls.Add(this.GridCheckBox);
            this.Controls.Add(this.mainField);
            this.Controls.Add(this.trackBar1);
            this.Controls.Add(this.rioConectionButton);
            this.Controls.Add(this.loadFromRIOButton);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "PoseVisualizer";
            this.Text = "Pose Visualizer";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.PoseVisualizer_FormClosed);
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.mainField)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private FontAwesome.Sharp.IconButton rioConectionButton;
        private System.Windows.Forms.Button loadFromRIOButton;
        private System.Windows.Forms.TrackBar trackBar1;
        private System.Windows.Forms.DataVisualization.Charting.Chart mainField;
        private System.Windows.Forms.CheckBox GridCheckBox;
    }
}