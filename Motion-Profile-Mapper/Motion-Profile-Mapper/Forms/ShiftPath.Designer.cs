
namespace MotionProfileMapper.Forms
{
    partial class ShiftPath
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ShiftPath));
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.dyInput = new System.Windows.Forms.TextBox();
            this.dxInput = new System.Windows.Forms.TextBox();
            this.cancel = new System.Windows.Forms.Button();
            this.shiftButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.BackColor = System.Drawing.SystemColors.ControlLight;
            this.label9.Font = new System.Drawing.Font("Verdana", 7.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.ForeColor = System.Drawing.Color.Black;
            this.label9.Location = new System.Drawing.Point(214, 15);
            this.label9.Margin = new System.Windows.Forms.Padding(1, 0, 1, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(44, 12);
            this.label9.TabIndex = 64;
            this.label9.Text = "meters";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.BackColor = System.Drawing.SystemColors.ControlLight;
            this.label10.Font = new System.Drawing.Font("Verdana", 7.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.ForeColor = System.Drawing.Color.Black;
            this.label10.Location = new System.Drawing.Point(214, 37);
            this.label10.Margin = new System.Windows.Forms.Padding(1, 0, 1, 0);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(44, 12);
            this.label10.TabIndex = 63;
            this.label10.Text = "meters";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Verdana", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.Black;
            this.label2.Location = new System.Drawing.Point(12, 36);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(77, 13);
            this.label2.TabIndex = 62;
            this.label2.Text = "Vertical shift";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Verdana", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.Black;
            this.label1.Location = new System.Drawing.Point(12, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(92, 13);
            this.label1.TabIndex = 61;
            this.label1.Text = "Horizontal shift";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // dyInput
            // 
            this.dyInput.BackColor = System.Drawing.Color.White;
            this.dyInput.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.dyInput.Font = new System.Drawing.Font("Verdana", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dyInput.ForeColor = System.Drawing.Color.Black;
            this.dyInput.Location = new System.Drawing.Point(111, 33);
            this.dyInput.Name = "dyInput";
            this.dyInput.Size = new System.Drawing.Size(100, 20);
            this.dyInput.TabIndex = 60;
            this.dyInput.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.dyInput.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.dyInput_KeyPress);
            // 
            // dxInput
            // 
            this.dxInput.BackColor = System.Drawing.Color.White;
            this.dxInput.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.dxInput.Font = new System.Drawing.Font("Verdana", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dxInput.ForeColor = System.Drawing.Color.Black;
            this.dxInput.Location = new System.Drawing.Point(111, 11);
            this.dxInput.Name = "dxInput";
            this.dxInput.Size = new System.Drawing.Size(100, 20);
            this.dxInput.TabIndex = 59;
            this.dxInput.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.dxInput.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.dxInput_KeyPress);
            // 
            // cancel
            // 
            this.cancel.BackColor = System.Drawing.SystemColors.AppWorkspace;
            this.cancel.FlatAppearance.BorderSize = 0;
            this.cancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cancel.Font = new System.Drawing.Font("Verdana", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cancel.ForeColor = System.Drawing.Color.Black;
            this.cancel.Location = new System.Drawing.Point(66, 62);
            this.cancel.Name = "cancel";
            this.cancel.Size = new System.Drawing.Size(90, 25);
            this.cancel.TabIndex = 66;
            this.cancel.Text = "Cancel";
            this.cancel.UseVisualStyleBackColor = false;
            this.cancel.Click += new System.EventHandler(this.cancel_Click);
            // 
            // shiftButton
            // 
            this.shiftButton.BackColor = System.Drawing.Color.LimeGreen;
            this.shiftButton.FlatAppearance.BorderSize = 0;
            this.shiftButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.shiftButton.Font = new System.Drawing.Font("Verdana", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.shiftButton.ForeColor = System.Drawing.Color.Black;
            this.shiftButton.Location = new System.Drawing.Point(162, 62);
            this.shiftButton.Name = "shiftButton";
            this.shiftButton.Size = new System.Drawing.Size(90, 25);
            this.shiftButton.TabIndex = 65;
            this.shiftButton.Text = "Shift";
            this.shiftButton.UseVisualStyleBackColor = false;
            this.shiftButton.Click += new System.EventHandler(this.shiftButton_Click);
            // 
            // ShiftPath
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.SystemColors.ControlLight;
            this.ClientSize = new System.Drawing.Size(266, 97);
            this.Controls.Add(this.cancel);
            this.Controls.Add(this.shiftButton);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.dyInput);
            this.Controls.Add(this.dxInput);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Name = "ShiftPath";
            this.Text = "Shift Path";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox dyInput;
        private System.Windows.Forms.TextBox dxInput;
        private System.Windows.Forms.Button cancel;
        private System.Windows.Forms.Button shiftButton;
    }
}