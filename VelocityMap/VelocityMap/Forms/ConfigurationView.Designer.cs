
namespace VelocityMap.Forms
{
    partial class ConfigurationView
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConfigurationView));
            this.configFileList = new System.Windows.Forms.DataGridView();
            this.newProfileButton = new FontAwesome.Sharp.IconButton();
            this.deleteProfileButton = new FontAwesome.Sharp.IconButton();
            this.saveToRioButton = new System.Windows.Forms.Button();
            this.refresh_button = new System.Windows.Forms.Button();
            this.configurationGrid = new System.Windows.Forms.DataGridView();
            this.infoLabel = new System.Windows.Forms.Label();
            this.configObjectBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.Value = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Variable_Type = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.Variable_Name = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.configFileList)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.configurationGrid)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.configObjectBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // configFileList
            // 
            this.configFileList.BackgroundColor = System.Drawing.Color.White;
            this.configFileList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.configFileList.Location = new System.Drawing.Point(12, 64);
            this.configFileList.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.configFileList.Name = "configFileList";
            this.configFileList.RowHeadersWidth = 51;
            this.configFileList.RowTemplate.Height = 24;
            this.configFileList.Size = new System.Drawing.Size(247, 391);
            this.configFileList.TabIndex = 1;
            // 
            // newProfileButton
            // 
            this.newProfileButton.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.newProfileButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.newProfileButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.newProfileButton.ForeColor = System.Drawing.Color.DarkGray;
            this.newProfileButton.IconChar = FontAwesome.Sharp.IconChar.Plus;
            this.newProfileButton.IconColor = System.Drawing.Color.Green;
            this.newProfileButton.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.newProfileButton.IconSize = 24;
            this.newProfileButton.Location = new System.Drawing.Point(12, 459);
            this.newProfileButton.Margin = new System.Windows.Forms.Padding(0);
            this.newProfileButton.Name = "newProfileButton";
            this.newProfileButton.Size = new System.Drawing.Size(108, 30);
            this.newProfileButton.TabIndex = 49;
            this.newProfileButton.UseVisualStyleBackColor = false;
            this.newProfileButton.Click += new System.EventHandler(this.newProfileButton_Click);
            // 
            // deleteProfileButton
            // 
            this.deleteProfileButton.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.deleteProfileButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.deleteProfileButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.deleteProfileButton.ForeColor = System.Drawing.Color.DarkGray;
            this.deleteProfileButton.IconChar = FontAwesome.Sharp.IconChar.Trash;
            this.deleteProfileButton.IconColor = System.Drawing.Color.Firebrick;
            this.deleteProfileButton.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.deleteProfileButton.IconSize = 24;
            this.deleteProfileButton.Location = new System.Drawing.Point(151, 459);
            this.deleteProfileButton.Margin = new System.Windows.Forms.Padding(0);
            this.deleteProfileButton.Name = "deleteProfileButton";
            this.deleteProfileButton.Size = new System.Drawing.Size(108, 30);
            this.deleteProfileButton.TabIndex = 50;
            this.deleteProfileButton.UseVisualStyleBackColor = false;
            this.deleteProfileButton.Click += new System.EventHandler(this.deleteProfileButton_Click);
            // 
            // saveToRioButton
            // 
            this.saveToRioButton.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.saveToRioButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.saveToRioButton.FlatAppearance.BorderSize = 0;
            this.saveToRioButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.saveToRioButton.Font = new System.Drawing.Font("Verdana", 16.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.saveToRioButton.ForeColor = System.Drawing.Color.DarkCyan;
            this.saveToRioButton.Location = new System.Drawing.Point(12, 492);
            this.saveToRioButton.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.saveToRioButton.Name = "saveToRioButton";
            this.saveToRioButton.Size = new System.Drawing.Size(247, 50);
            this.saveToRioButton.TabIndex = 62;
            this.saveToRioButton.Text = "Save to RIO";
            this.saveToRioButton.UseVisualStyleBackColor = false;
            this.saveToRioButton.Click += new System.EventHandler(this.saveToRioButton_Click);
            // 
            // refresh_button
            // 
            this.refresh_button.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.refresh_button.Cursor = System.Windows.Forms.Cursors.Hand;
            this.refresh_button.FlatAppearance.BorderSize = 0;
            this.refresh_button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.refresh_button.Font = new System.Drawing.Font("Verdana", 16.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.refresh_button.ForeColor = System.Drawing.Color.DarkCyan;
            this.refresh_button.Location = new System.Drawing.Point(12, 12);
            this.refresh_button.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.refresh_button.Name = "refresh_button";
            this.refresh_button.Size = new System.Drawing.Size(247, 46);
            this.refresh_button.TabIndex = 63;
            this.refresh_button.Text = "Load from RIO";
            this.refresh_button.UseVisualStyleBackColor = false;
            this.refresh_button.Click += new System.EventHandler(this.refresh_button_Click);
            // 
            // configurationGrid
            // 
            this.configurationGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.configurationGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Variable_Name,
            this.Variable_Type,
            this.Value});
            this.configurationGrid.Location = new System.Drawing.Point(271, 64);
            this.configurationGrid.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.configurationGrid.Name = "configurationGrid";
            this.configurationGrid.RowHeadersWidth = 51;
            this.configurationGrid.RowTemplate.Height = 24;
            this.configurationGrid.Size = new System.Drawing.Size(697, 478);
            this.configurationGrid.TabIndex = 65;
            this.configurationGrid.CellValidating += new System.Windows.Forms.DataGridViewCellValidatingEventHandler(this.dataGridView3_CellValidating);
            this.configurationGrid.EditingControlShowing += new System.Windows.Forms.DataGridViewEditingControlShowingEventHandler(this.dataGridView1_EditingControlShowing);
            // 
            // infoLabel
            // 
            this.infoLabel.BackColor = System.Drawing.SystemColors.Control;
            this.infoLabel.Font = new System.Drawing.Font("Verdana", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.infoLabel.ForeColor = System.Drawing.Color.Black;
            this.infoLabel.Location = new System.Drawing.Point(271, 12);
            this.infoLabel.Name = "infoLabel";
            this.infoLabel.Size = new System.Drawing.Size(696, 40);
            this.infoLabel.TabIndex = 66;
            this.infoLabel.Text = "Configuration Editor";
            this.infoLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // configObjectBindingSource
            // 
            this.configObjectBindingSource.DataSource = typeof(VelocityMap.Configuration.ConfigObject);
            // 
            // Value
            // 
            this.Value.HeaderText = "Value";
            this.Value.MinimumWidth = 6;
            this.Value.Name = "Value";
            this.Value.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.Value.Width = 219;
            // 
            // Variable_Type
            // 
            this.Variable_Type.HeaderText = "Type";
            this.Variable_Type.Items.AddRange(new object[] {
            "Int",
            "Double",
            "Boolean",
            "String"});
            this.Variable_Type.MinimumWidth = 6;
            this.Variable_Type.Name = "Variable_Type";
            this.Variable_Type.Width = 125;
            // 
            // Variable_Name
            // 
            this.Variable_Name.HeaderText = "Name";
            this.Variable_Name.MinimumWidth = 6;
            this.Variable_Name.Name = "Variable_Name";
            this.Variable_Name.Width = 300;
            // 
            // ConfigurationView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(979, 554);
            this.Controls.Add(this.infoLabel);
            this.Controls.Add(this.configurationGrid);
            this.Controls.Add(this.refresh_button);
            this.Controls.Add(this.saveToRioButton);
            this.Controls.Add(this.deleteProfileButton);
            this.Controls.Add(this.newProfileButton);
            this.Controls.Add(this.configFileList);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "ConfigurationView";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Configure Constants";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.ConfigurationView_FormClosed);
            ((System.ComponentModel.ISupportInitialize)(this.configFileList)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.configurationGrid)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.configObjectBindingSource)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.DataGridView configFileList;
        private FontAwesome.Sharp.IconButton newProfileButton;
        private FontAwesome.Sharp.IconButton deleteProfileButton;
        private System.Windows.Forms.Button saveToRioButton;
        private System.Windows.Forms.Button refresh_button;
        private System.Windows.Forms.DataGridView configurationGrid;
        private System.Windows.Forms.Label infoLabel;
        private System.Windows.Forms.BindingSource configObjectBindingSource;
        private System.Windows.Forms.DataGridViewTextBoxColumn Variable_Name;
        private System.Windows.Forms.DataGridViewComboBoxColumn Variable_Type;
        private System.Windows.Forms.DataGridViewTextBoxColumn Value;
    }
}