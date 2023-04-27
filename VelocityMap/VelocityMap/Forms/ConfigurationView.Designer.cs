
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConfigurationView));
            this.filenameGrid = new System.Windows.Forms.DataGridView();
            this.FileName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.newProfileButton = new FontAwesome.Sharp.IconButton();
            this.deleteProfileButton = new FontAwesome.Sharp.IconButton();
            this.saveToRioButton = new System.Windows.Forms.Button();
            this.loadRIOButton = new System.Windows.Forms.Button();
            this.configurationGrid = new System.Windows.Forms.DataGridView();
            this.Variable_Name = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Variable_Type = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.Value = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.loadLocalButton = new System.Windows.Forms.Button();
            this.connectionSettingsButton = new FontAwesome.Sharp.IconButton();
            this.saveLocalButton = new System.Windows.Forms.Button();
            this.saveAllLocalButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.filenameGrid)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.configurationGrid)).BeginInit();
            this.SuspendLayout();
            // 
            // filenameGrid
            // 
            this.filenameGrid.AllowUserToAddRows = false;
            this.filenameGrid.AllowUserToDeleteRows = false;
            this.filenameGrid.AllowUserToResizeColumns = false;
            this.filenameGrid.AllowUserToResizeRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.filenameGrid.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.filenameGrid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.filenameGrid.BackgroundColor = System.Drawing.Color.White;
            this.filenameGrid.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.filenameGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.filenameGrid.ColumnHeadersVisible = false;
            this.filenameGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.FileName});
            this.filenameGrid.GridColor = System.Drawing.Color.Silver;
            this.filenameGrid.Location = new System.Drawing.Point(9, 75);
            this.filenameGrid.Margin = new System.Windows.Forms.Padding(2);
            this.filenameGrid.Name = "filenameGrid";
            this.filenameGrid.RowHeadersVisible = false;
            this.filenameGrid.RowHeadersWidth = 51;
            this.filenameGrid.RowTemplate.Height = 24;
            this.filenameGrid.Size = new System.Drawing.Size(205, 315);
            this.filenameGrid.TabIndex = 1;
            this.filenameGrid.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.filenameGrid_CellEndEdit);
            this.filenameGrid.RowEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.filenameGrid_RowEnter);
            // 
            // FileName
            // 
            this.FileName.HeaderText = "File Name";
            this.FileName.Name = "FileName";
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
            this.newProfileButton.Location = new System.Drawing.Point(9, 390);
            this.newProfileButton.Margin = new System.Windows.Forms.Padding(0);
            this.newProfileButton.Name = "newProfileButton";
            this.newProfileButton.Size = new System.Drawing.Size(103, 24);
            this.newProfileButton.TabIndex = 49;
            this.newProfileButton.UseVisualStyleBackColor = false;
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
            this.deleteProfileButton.Location = new System.Drawing.Point(112, 390);
            this.deleteProfileButton.Margin = new System.Windows.Forms.Padding(0);
            this.deleteProfileButton.Name = "deleteProfileButton";
            this.deleteProfileButton.Size = new System.Drawing.Size(102, 24);
            this.deleteProfileButton.TabIndex = 50;
            this.deleteProfileButton.UseVisualStyleBackColor = false;
            // 
            // saveToRioButton
            // 
            this.saveToRioButton.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.saveToRioButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.saveToRioButton.FlatAppearance.BorderSize = 0;
            this.saveToRioButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.saveToRioButton.Font = new System.Drawing.Font("Verdana", 16.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.saveToRioButton.ForeColor = System.Drawing.Color.Teal;
            this.saveToRioButton.Location = new System.Drawing.Point(9, 448);
            this.saveToRioButton.Margin = new System.Windows.Forms.Padding(2);
            this.saveToRioButton.Name = "saveToRioButton";
            this.saveToRioButton.Size = new System.Drawing.Size(205, 41);
            this.saveToRioButton.TabIndex = 62;
            this.saveToRioButton.Text = "Save to RIO";
            this.saveToRioButton.UseVisualStyleBackColor = false;
            this.saveToRioButton.Click += new System.EventHandler(this.saveToRioButton_Click);
            // 
            // loadRIOButton
            // 
            this.loadRIOButton.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.loadRIOButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.loadRIOButton.FlatAppearance.BorderSize = 0;
            this.loadRIOButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.loadRIOButton.Font = new System.Drawing.Font("Verdana", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.loadRIOButton.ForeColor = System.Drawing.Color.Teal;
            this.loadRIOButton.Location = new System.Drawing.Point(9, 10);
            this.loadRIOButton.Margin = new System.Windows.Forms.Padding(2);
            this.loadRIOButton.Name = "loadRIOButton";
            this.loadRIOButton.Size = new System.Drawing.Size(165, 37);
            this.loadRIOButton.TabIndex = 63;
            this.loadRIOButton.Text = "Load from RIO";
            this.loadRIOButton.UseVisualStyleBackColor = false;
            // 
            // configurationGrid
            // 
            this.configurationGrid.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.configurationGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.configurationGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Variable_Name,
            this.Variable_Type,
            this.Value});
            this.configurationGrid.Location = new System.Drawing.Point(222, 10);
            this.configurationGrid.Margin = new System.Windows.Forms.Padding(2);
            this.configurationGrid.Name = "configurationGrid";
            this.configurationGrid.RowHeadersWidth = 51;
            this.configurationGrid.RowTemplate.Height = 24;
            this.configurationGrid.Size = new System.Drawing.Size(698, 479);
            this.configurationGrid.TabIndex = 65;
            this.configurationGrid.CellValidated += new System.Windows.Forms.DataGridViewCellEventHandler(this.configurationGrid_CellValidated);
            this.configurationGrid.CellValidating += new System.Windows.Forms.DataGridViewCellValidatingEventHandler(this.configurationGrid_CellValidating);
            this.configurationGrid.EditingControlShowing += new System.Windows.Forms.DataGridViewEditingControlShowingEventHandler(this.configurationGrid_EditingControlShowing);
            // 
            // Variable_Name
            // 
            this.Variable_Name.HeaderText = "Variable Name";
            this.Variable_Name.MinimumWidth = 6;
            this.Variable_Name.Name = "Variable_Name";
            this.Variable_Name.Width = 300;
            // 
            // Variable_Type
            // 
            this.Variable_Type.HeaderText = "Variable Type";
            this.Variable_Type.Items.AddRange(new object[] {
            "Int",
            "Double",
            "Boolean",
            "String"});
            this.Variable_Type.MinimumWidth = 6;
            this.Variable_Type.Name = "Variable_Type";
            this.Variable_Type.Width = 125;
            // 
            // Value
            // 
            this.Value.HeaderText = "Value";
            this.Value.MinimumWidth = 6;
            this.Value.Name = "Value";
            this.Value.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.Value.Width = 219;
            // 
            // loadLocalButton
            // 
            this.loadLocalButton.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.loadLocalButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.loadLocalButton.FlatAppearance.BorderSize = 0;
            this.loadLocalButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.loadLocalButton.Font = new System.Drawing.Font("Verdana", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.loadLocalButton.ForeColor = System.Drawing.Color.Black;
            this.loadLocalButton.Location = new System.Drawing.Point(9, 50);
            this.loadLocalButton.Margin = new System.Windows.Forms.Padding(1);
            this.loadLocalButton.Name = "loadLocalButton";
            this.loadLocalButton.Size = new System.Drawing.Size(205, 22);
            this.loadLocalButton.TabIndex = 66;
            this.loadLocalButton.Text = "Load from local files";
            this.loadLocalButton.UseVisualStyleBackColor = false;
            this.loadLocalButton.Click += new System.EventHandler(this.loadLocalButton_Click);
            // 
            // connectionSettingsButton
            // 
            this.connectionSettingsButton.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.connectionSettingsButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.connectionSettingsButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.connectionSettingsButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.connectionSettingsButton.ForeColor = System.Drawing.SystemColors.AppWorkspace;
            this.connectionSettingsButton.IconChar = FontAwesome.Sharp.IconChar.Cog;
            this.connectionSettingsButton.IconColor = System.Drawing.Color.Teal;
            this.connectionSettingsButton.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.connectionSettingsButton.IconSize = 22;
            this.connectionSettingsButton.Location = new System.Drawing.Point(177, 10);
            this.connectionSettingsButton.Margin = new System.Windows.Forms.Padding(0);
            this.connectionSettingsButton.Name = "connectionSettingsButton";
            this.connectionSettingsButton.Size = new System.Drawing.Size(37, 37);
            this.connectionSettingsButton.TabIndex = 67;
            this.connectionSettingsButton.UseVisualStyleBackColor = false;
            this.connectionSettingsButton.Click += new System.EventHandler(this.connectionSettingsButton_Click);
            // 
            // saveLocalButton
            // 
            this.saveLocalButton.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.saveLocalButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.saveLocalButton.FlatAppearance.BorderSize = 0;
            this.saveLocalButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.saveLocalButton.Font = new System.Drawing.Font("Verdana", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.saveLocalButton.ForeColor = System.Drawing.Color.Black;
            this.saveLocalButton.Location = new System.Drawing.Point(9, 423);
            this.saveLocalButton.Margin = new System.Windows.Forms.Padding(1);
            this.saveLocalButton.Name = "saveLocalButton";
            this.saveLocalButton.Size = new System.Drawing.Size(101, 22);
            this.saveLocalButton.TabIndex = 68;
            this.saveLocalButton.Text = "Save to local";
            this.saveLocalButton.UseVisualStyleBackColor = false;
            this.saveLocalButton.Click += new System.EventHandler(this.saveLocalButton_Click);
            // 
            // saveAllLocalButton
            // 
            this.saveAllLocalButton.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.saveAllLocalButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.saveAllLocalButton.FlatAppearance.BorderSize = 0;
            this.saveAllLocalButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.saveAllLocalButton.Font = new System.Drawing.Font("Verdana", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.saveAllLocalButton.ForeColor = System.Drawing.Color.Black;
            this.saveAllLocalButton.Location = new System.Drawing.Point(113, 423);
            this.saveAllLocalButton.Margin = new System.Windows.Forms.Padding(1);
            this.saveAllLocalButton.Name = "saveAllLocalButton";
            this.saveAllLocalButton.Size = new System.Drawing.Size(101, 22);
            this.saveAllLocalButton.TabIndex = 69;
            this.saveAllLocalButton.Text = "Save all local";
            this.saveAllLocalButton.UseVisualStyleBackColor = false;
            // 
            // ConfigurationView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(929, 500);
            this.Controls.Add(this.saveAllLocalButton);
            this.Controls.Add(this.saveLocalButton);
            this.Controls.Add(this.connectionSettingsButton);
            this.Controls.Add(this.loadLocalButton);
            this.Controls.Add(this.configurationGrid);
            this.Controls.Add(this.loadRIOButton);
            this.Controls.Add(this.saveToRioButton);
            this.Controls.Add(this.deleteProfileButton);
            this.Controls.Add(this.newProfileButton);
            this.Controls.Add(this.filenameGrid);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "ConfigurationView";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Configure Constants";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.ConfigurationView_FormClosed);
            ((System.ComponentModel.ISupportInitialize)(this.filenameGrid)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.configurationGrid)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.DataGridView filenameGrid;
        private FontAwesome.Sharp.IconButton newProfileButton;
        private FontAwesome.Sharp.IconButton deleteProfileButton;
        private System.Windows.Forms.Button saveToRioButton;
        private System.Windows.Forms.Button loadRIOButton;
        private System.Windows.Forms.DataGridView configurationGrid;
        private System.Windows.Forms.DataGridViewTextBoxColumn Variable_Name;
        private System.Windows.Forms.DataGridViewComboBoxColumn Variable_Type;
        private System.Windows.Forms.DataGridViewTextBoxColumn Value;
        private System.Windows.Forms.Button loadLocalButton;
        private FontAwesome.Sharp.IconButton connectionSettingsButton;
        private System.Windows.Forms.DataGridViewTextBoxColumn FileName;
        private System.Windows.Forms.Button saveLocalButton;
        private System.Windows.Forms.Button saveAllLocalButton;
    }
}