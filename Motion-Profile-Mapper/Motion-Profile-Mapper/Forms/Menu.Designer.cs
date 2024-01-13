
namespace MotionProfileMapper.Forms
{
    partial class Menu
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Menu));
            this.mpButton = new System.Windows.Forms.Button();
            this.constantsButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // mpButton
            // 
            this.mpButton.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.mpButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.mpButton.FlatAppearance.BorderSize = 0;
            this.mpButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.mpButton.Font = new System.Drawing.Font("Verdana", 16.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mpButton.ForeColor = System.Drawing.Color.Teal;
            this.mpButton.Location = new System.Drawing.Point(12, 12);
            this.mpButton.Name = "mpButton";
            this.mpButton.Size = new System.Drawing.Size(265, 50);
            this.mpButton.TabIndex = 26;
            this.mpButton.Text = "Motion Profiler";
            this.mpButton.UseVisualStyleBackColor = false;
            this.mpButton.Click += new System.EventHandler(this.mpButton_Click);
            // 
            // constantsButton
            // 
            this.constantsButton.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.constantsButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.constantsButton.FlatAppearance.BorderSize = 0;
            this.constantsButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.constantsButton.Font = new System.Drawing.Font("Verdana", 16.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.constantsButton.ForeColor = System.Drawing.Color.SlateBlue;
            this.constantsButton.Location = new System.Drawing.Point(12, 68);
            this.constantsButton.Name = "constantsButton";
            this.constantsButton.Size = new System.Drawing.Size(265, 50);
            this.constantsButton.TabIndex = 27;
            this.constantsButton.Text = "Configure Constants";
            this.constantsButton.UseVisualStyleBackColor = false;
            this.constantsButton.Click += new System.EventHandler(this.constantsButton_Click);
            // 
            // Menu
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(289, 131);
            this.Controls.Add(this.constantsButton);
            this.Controls.Add(this.mpButton);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Menu";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "3539 Robot Tools";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button mpButton;
        private System.Windows.Forms.Button constantsButton;
    }
}