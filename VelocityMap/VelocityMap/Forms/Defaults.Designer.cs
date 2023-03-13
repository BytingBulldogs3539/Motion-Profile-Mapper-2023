
namespace VelocityMap.Forms
{
    partial class Defaults
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
            this.label6 = new System.Windows.Forms.Label();
            this.cancel = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.defMaxRotAccInput = new System.Windows.Forms.TextBox();
            this.defMaxRotVelInput = new System.Windows.Forms.TextBox();
            this.defMaxAccInput = new System.Windows.Forms.TextBox();
            this.defMaxVelInput = new System.Windows.Forms.TextBox();
            this.save = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Verdana", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.ForeColor = System.Drawing.Color.Black;
            this.label6.Location = new System.Drawing.Point(62, 9);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(191, 17);
            this.label6.TabIndex = 23;
            this.label6.Text = "Robot Movement Defaults";
            // 
            // cancel
            // 
            this.cancel.BackColor = System.Drawing.SystemColors.AppWorkspace;
            this.cancel.FlatAppearance.BorderSize = 0;
            this.cancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cancel.Font = new System.Drawing.Font("Verdana", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cancel.ForeColor = System.Drawing.Color.Black;
            this.cancel.Location = new System.Drawing.Point(126, 146);
            this.cancel.Name = "cancel";
            this.cancel.Size = new System.Drawing.Size(90, 25);
            this.cancel.TabIndex = 22;
            this.cancel.Text = "Cancel";
            this.cancel.UseVisualStyleBackColor = false;
            this.cancel.Click += new System.EventHandler(this.cancel_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Verdana", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.ForeColor = System.Drawing.Color.Black;
            this.label5.Location = new System.Drawing.Point(12, 119);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(201, 17);
            this.label5.TabIndex = 21;
            this.label5.Text = "Max Rotational Acceleration";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(30, 110);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(0, 15);
            this.label4.TabIndex = 20;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Verdana", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.Color.Black;
            this.label3.Location = new System.Drawing.Point(12, 92);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(170, 17);
            this.label3.TabIndex = 19;
            this.label3.Text = "Max Rotational Velocity";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Verdana", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.Black;
            this.label2.Location = new System.Drawing.Point(12, 65);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(125, 17);
            this.label2.TabIndex = 18;
            this.label2.Text = "Max Acceleration";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Verdana", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.Black;
            this.label1.Location = new System.Drawing.Point(12, 38);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(94, 17);
            this.label1.TabIndex = 17;
            this.label1.Text = "Max Velocity";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // defMaxRotAccInput
            // 
            this.defMaxRotAccInput.BackColor = System.Drawing.Color.White;
            this.defMaxRotAccInput.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.defMaxRotAccInput.Font = new System.Drawing.Font("Verdana", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.defMaxRotAccInput.ForeColor = System.Drawing.Color.Black;
            this.defMaxRotAccInput.Location = new System.Drawing.Point(222, 117);
            this.defMaxRotAccInput.Name = "defMaxRotAccInput";
            this.defMaxRotAccInput.Size = new System.Drawing.Size(100, 23);
            this.defMaxRotAccInput.TabIndex = 16;
            // 
            // defMaxRotVelInput
            // 
            this.defMaxRotVelInput.BackColor = System.Drawing.Color.White;
            this.defMaxRotVelInput.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.defMaxRotVelInput.Font = new System.Drawing.Font("Verdana", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.defMaxRotVelInput.ForeColor = System.Drawing.Color.Black;
            this.defMaxRotVelInput.Location = new System.Drawing.Point(222, 90);
            this.defMaxRotVelInput.Name = "defMaxRotVelInput";
            this.defMaxRotVelInput.Size = new System.Drawing.Size(100, 23);
            this.defMaxRotVelInput.TabIndex = 15;
            // 
            // defMaxAccInput
            // 
            this.defMaxAccInput.BackColor = System.Drawing.Color.White;
            this.defMaxAccInput.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.defMaxAccInput.Font = new System.Drawing.Font("Verdana", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.defMaxAccInput.ForeColor = System.Drawing.Color.Black;
            this.defMaxAccInput.Location = new System.Drawing.Point(222, 63);
            this.defMaxAccInput.Name = "defMaxAccInput";
            this.defMaxAccInput.Size = new System.Drawing.Size(100, 23);
            this.defMaxAccInput.TabIndex = 14;
            // 
            // defMaxVelInput
            // 
            this.defMaxVelInput.BackColor = System.Drawing.Color.White;
            this.defMaxVelInput.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.defMaxVelInput.Font = new System.Drawing.Font("Verdana", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.defMaxVelInput.ForeColor = System.Drawing.Color.Black;
            this.defMaxVelInput.Location = new System.Drawing.Point(222, 36);
            this.defMaxVelInput.Name = "defMaxVelInput";
            this.defMaxVelInput.Size = new System.Drawing.Size(100, 23);
            this.defMaxVelInput.TabIndex = 13;
            // 
            // save
            // 
            this.save.BackColor = System.Drawing.Color.LimeGreen;
            this.save.FlatAppearance.BorderSize = 0;
            this.save.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.save.Font = new System.Drawing.Font("Verdana", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.save.ForeColor = System.Drawing.Color.Black;
            this.save.Location = new System.Drawing.Point(222, 146);
            this.save.Name = "save";
            this.save.Size = new System.Drawing.Size(100, 25);
            this.save.TabIndex = 12;
            this.save.Text = "Save";
            this.save.UseVisualStyleBackColor = false;
            this.save.Click += new System.EventHandler(this.save_Click);
            // 
            // Defaults
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlLight;
            this.ClientSize = new System.Drawing.Size(341, 189);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.cancel);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.defMaxRotAccInput);
            this.Controls.Add(this.defMaxRotVelInput);
            this.Controls.Add(this.defMaxAccInput);
            this.Controls.Add(this.defMaxVelInput);
            this.Controls.Add(this.save);
            this.Name = "Defaults";
            this.Text = "Movement Defaults";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button cancel;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox defMaxRotAccInput;
        private System.Windows.Forms.TextBox defMaxRotVelInput;
        private System.Windows.Forms.TextBox defMaxAccInput;
        private System.Windows.Forms.TextBox defMaxVelInput;
        private System.Windows.Forms.Button save;
    }
}