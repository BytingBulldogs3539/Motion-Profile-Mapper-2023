
namespace MotionProfileMapper.Forms
{
    partial class MirrorPath
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MirrorPath));
            this.mirrorSelectedButton = new System.Windows.Forms.Button();
            this.mirrorAllButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // mirrorSelectedButton
            // 
            this.mirrorSelectedButton.BackColor = System.Drawing.Color.DodgerBlue;
            this.mirrorSelectedButton.FlatAppearance.BorderSize = 0;
            this.mirrorSelectedButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.mirrorSelectedButton.Font = new System.Drawing.Font("Verdana", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mirrorSelectedButton.ForeColor = System.Drawing.Color.Black;
            this.mirrorSelectedButton.Location = new System.Drawing.Point(108, 41);
            this.mirrorSelectedButton.Name = "mirrorSelectedButton";
            this.mirrorSelectedButton.Size = new System.Drawing.Size(90, 25);
            this.mirrorSelectedButton.TabIndex = 18;
            this.mirrorSelectedButton.Text = "Selected";
            this.mirrorSelectedButton.UseVisualStyleBackColor = false;
            this.mirrorSelectedButton.Click += new System.EventHandler(this.mirrorSelectedButton_Click);
            // 
            // mirrorAllButton
            // 
            this.mirrorAllButton.BackColor = System.Drawing.Color.LimeGreen;
            this.mirrorAllButton.FlatAppearance.BorderSize = 0;
            this.mirrorAllButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.mirrorAllButton.Font = new System.Drawing.Font("Verdana", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mirrorAllButton.ForeColor = System.Drawing.Color.Black;
            this.mirrorAllButton.Location = new System.Drawing.Point(204, 41);
            this.mirrorAllButton.Name = "mirrorAllButton";
            this.mirrorAllButton.Size = new System.Drawing.Size(90, 25);
            this.mirrorAllButton.TabIndex = 23;
            this.mirrorAllButton.Text = "All";
            this.mirrorAllButton.UseVisualStyleBackColor = false;
            this.mirrorAllButton.Click += new System.EventHandler(this.mirrorAllButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.BackColor = System.Drawing.SystemColors.AppWorkspace;
            this.cancelButton.FlatAppearance.BorderSize = 0;
            this.cancelButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cancelButton.Font = new System.Drawing.Font("Verdana", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cancelButton.ForeColor = System.Drawing.Color.Black;
            this.cancelButton.Location = new System.Drawing.Point(12, 41);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(90, 25);
            this.cancelButton.TabIndex = 13;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = false;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Verdana", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.ForeColor = System.Drawing.Color.Black;
            this.label6.Location = new System.Drawing.Point(33, 12);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(243, 17);
            this.label6.TabIndex = 14;
            this.label6.Text = "Which paths should be mirrored?";
            // 
            // MirrorPath
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(307, 79);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.mirrorSelectedButton);
            this.Controls.Add(this.mirrorAllButton);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MirrorPath";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Mirror Paths";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button mirrorSelectedButton;
        private System.Windows.Forms.Button mirrorAllButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Label label6;
    }
}