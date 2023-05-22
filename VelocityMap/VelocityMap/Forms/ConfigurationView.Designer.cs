
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConfigurationView));
            this.filenameGrid = new System.Windows.Forms.DataGridView();
            this.FileName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.newFileButton = new FontAwesome.Sharp.IconButton();
            this.deleteButton = new FontAwesome.Sharp.IconButton();
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
            this.infoLabel = new System.Windows.Forms.Label();
            this.rowContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.timeSinceUpload = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.filenameGrid)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.configurationGrid)).BeginInit();
            this.rowContextMenuStrip.SuspendLayout();
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
            this.filenameGrid.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.filenameGrid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.filenameGrid.BackgroundColor = System.Drawing.Color.White;
            this.filenameGrid.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.filenameGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.filenameGrid.ColumnHeadersVisible = false;
            this.filenameGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.FileName});
            this.filenameGrid.GridColor = System.Drawing.Color.Silver;
            this.filenameGrid.Location = new System.Drawing.Point(15, 103);
            this.filenameGrid.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.filenameGrid.MultiSelect = false;
            this.filenameGrid.Name = "filenameGrid";
            this.filenameGrid.RowHeadersVisible = false;
            this.filenameGrid.RowHeadersWidth = 51;
            this.filenameGrid.RowTemplate.Height = 24;
            this.filenameGrid.Size = new System.Drawing.Size(273, 379);
            this.filenameGrid.TabIndex = 1;
            this.filenameGrid.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.filenameGrid_CellEndEdit);
            this.filenameGrid.RowEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.filenameGrid_RowEnter);
            // 
            // FileName
            // 
            this.FileName.HeaderText = "File Name";
            this.FileName.MinimumWidth = 6;
            this.FileName.Name = "FileName";
            // 
            // newFileButton
            // 
            this.newFileButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.newFileButton.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.newFileButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.newFileButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.newFileButton.ForeColor = System.Drawing.Color.DarkGray;
            this.newFileButton.IconChar = FontAwesome.Sharp.IconChar.Plus;
            this.newFileButton.IconColor = System.Drawing.Color.Green;
            this.newFileButton.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.newFileButton.IconSize = 24;
            this.newFileButton.Location = new System.Drawing.Point(15, 503);
            this.newFileButton.Margin = new System.Windows.Forms.Padding(0);
            this.newFileButton.Name = "newFileButton";
            this.newFileButton.Size = new System.Drawing.Size(137, 30);
            this.newFileButton.TabIndex = 49;
            this.newFileButton.UseVisualStyleBackColor = false;
            this.newFileButton.Click += new System.EventHandler(this.newFileButton_Click);
            // 
            // deleteButton
            // 
            this.deleteButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.deleteButton.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.deleteButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.deleteButton.Enabled = false;
            this.deleteButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.deleteButton.ForeColor = System.Drawing.Color.DarkGray;
            this.deleteButton.IconChar = FontAwesome.Sharp.IconChar.Trash;
            this.deleteButton.IconColor = System.Drawing.Color.Firebrick;
            this.deleteButton.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.deleteButton.IconSize = 24;
            this.deleteButton.Location = new System.Drawing.Point(152, 503);
            this.deleteButton.Margin = new System.Windows.Forms.Padding(0);
            this.deleteButton.Name = "deleteButton";
            this.deleteButton.Size = new System.Drawing.Size(136, 30);
            this.deleteButton.TabIndex = 50;
            this.deleteButton.UseVisualStyleBackColor = false;
            this.deleteButton.Click += new System.EventHandler(this.deleteButton_Click);
            // 
            // saveToRioButton
            // 
            this.saveToRioButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.saveToRioButton.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.saveToRioButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.saveToRioButton.Enabled = false;
            this.saveToRioButton.FlatAppearance.BorderSize = 0;
            this.saveToRioButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.saveToRioButton.Font = new System.Drawing.Font("Verdana", 16.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.saveToRioButton.ForeColor = System.Drawing.Color.Teal;
            this.saveToRioButton.Location = new System.Drawing.Point(15, 575);
            this.saveToRioButton.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.saveToRioButton.Name = "saveToRioButton";
            this.saveToRioButton.Size = new System.Drawing.Size(273, 50);
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
            this.loadRIOButton.Location = new System.Drawing.Point(15, 14);
            this.loadRIOButton.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.loadRIOButton.Name = "loadRIOButton";
            this.loadRIOButton.Size = new System.Drawing.Size(220, 46);
            this.loadRIOButton.TabIndex = 63;
            this.loadRIOButton.Text = "Load from RIO";
            this.loadRIOButton.UseVisualStyleBackColor = false;
            this.loadRIOButton.Click += new System.EventHandler(this.loadRIOButton_Click);
            // 
            // configurationGrid
            // 
            this.configurationGrid.AllowDrop = true;
            this.configurationGrid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.configurationGrid.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.configurationGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.configurationGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Variable_Name,
            this.Variable_Type,
            this.Value});
            this.configurationGrid.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this.configurationGrid.Enabled = false;
            this.configurationGrid.Location = new System.Drawing.Point(299, 63);
            this.configurationGrid.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.configurationGrid.MultiSelect = false;
            this.configurationGrid.Name = "configurationGrid";
            this.configurationGrid.RowHeadersWidth = 51;
            this.configurationGrid.RowTemplate.Height = 24;
            this.configurationGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.configurationGrid.Size = new System.Drawing.Size(931, 590);
            this.configurationGrid.TabIndex = 65;
            this.configurationGrid.CellValidated += new System.Windows.Forms.DataGridViewCellEventHandler(this.configurationGrid_CellValidated);
            this.configurationGrid.CellValidating += new System.Windows.Forms.DataGridViewCellValidatingEventHandler(this.configurationGrid_CellValidating);
            this.configurationGrid.EditingControlShowing += new System.Windows.Forms.DataGridViewEditingControlShowingEventHandler(this.configurationGrid_EditingControlShowing);
            this.configurationGrid.RowHeaderMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.configurationGrid_RowHeaderMouseClick);
            this.configurationGrid.Sorted += new System.EventHandler(this.configurationGrid_Sorted);
            this.configurationGrid.UserDeletedRow += new System.Windows.Forms.DataGridViewRowEventHandler(this.configurationGrid_UserDeletedRow);
            this.configurationGrid.DragDrop += new System.Windows.Forms.DragEventHandler(this.configurationGrid_DragDrop);
            this.configurationGrid.DragOver += new System.Windows.Forms.DragEventHandler(this.configurationGrid_DragOver);
            this.configurationGrid.MouseDown += new System.Windows.Forms.MouseEventHandler(this.configurationGrid_MouseDown);
            this.configurationGrid.MouseMove += new System.Windows.Forms.MouseEventHandler(this.configurationGrid_MouseMove);
            // 
            // Variable_Name
            // 
            this.Variable_Name.HeaderText = "Variable Name";
            this.Variable_Name.MinimumWidth = 6;
            this.Variable_Name.Name = "Variable_Name";
            this.Variable_Name.Width = 534;
            // 
            // Variable_Type
            // 
            this.Variable_Type.HeaderText = "Variable Type";
            this.Variable_Type.Items.AddRange(new object[] {
            "int",
            "double",
            "boolean",
            "String"});
            this.Variable_Type.MinimumWidth = 6;
            this.Variable_Type.Name = "Variable_Type";
            this.Variable_Type.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.Variable_Type.Width = 125;
            // 
            // Value
            // 
            this.Value.HeaderText = "Value";
            this.Value.MinimumWidth = 6;
            this.Value.Name = "Value";
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
            this.loadLocalButton.Location = new System.Drawing.Point(15, 63);
            this.loadLocalButton.Margin = new System.Windows.Forms.Padding(1);
            this.loadLocalButton.Name = "loadLocalButton";
            this.loadLocalButton.Size = new System.Drawing.Size(273, 27);
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
            this.connectionSettingsButton.Location = new System.Drawing.Point(239, 14);
            this.connectionSettingsButton.Margin = new System.Windows.Forms.Padding(0);
            this.connectionSettingsButton.Name = "connectionSettingsButton";
            this.connectionSettingsButton.Size = new System.Drawing.Size(49, 46);
            this.connectionSettingsButton.TabIndex = 67;
            this.connectionSettingsButton.UseVisualStyleBackColor = false;
            this.connectionSettingsButton.Click += new System.EventHandler(this.connectionSettingsButton_Click);
            // 
            // saveLocalButton
            // 
            this.saveLocalButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.saveLocalButton.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.saveLocalButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.saveLocalButton.Enabled = false;
            this.saveLocalButton.FlatAppearance.BorderSize = 0;
            this.saveLocalButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.saveLocalButton.Font = new System.Drawing.Font("Verdana", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.saveLocalButton.ForeColor = System.Drawing.Color.Black;
            this.saveLocalButton.Location = new System.Drawing.Point(15, 544);
            this.saveLocalButton.Margin = new System.Windows.Forms.Padding(1);
            this.saveLocalButton.Name = "saveLocalButton";
            this.saveLocalButton.Size = new System.Drawing.Size(135, 27);
            this.saveLocalButton.TabIndex = 68;
            this.saveLocalButton.Text = "Save to local";
            this.saveLocalButton.UseVisualStyleBackColor = false;
            this.saveLocalButton.Click += new System.EventHandler(this.saveLocalButton_Click);
            // 
            // saveAllLocalButton
            // 
            this.saveAllLocalButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.saveAllLocalButton.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.saveAllLocalButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.saveAllLocalButton.Enabled = false;
            this.saveAllLocalButton.FlatAppearance.BorderSize = 0;
            this.saveAllLocalButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.saveAllLocalButton.Font = new System.Drawing.Font("Verdana", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.saveAllLocalButton.ForeColor = System.Drawing.Color.Black;
            this.saveAllLocalButton.Location = new System.Drawing.Point(153, 544);
            this.saveAllLocalButton.Margin = new System.Windows.Forms.Padding(1);
            this.saveAllLocalButton.Name = "saveAllLocalButton";
            this.saveAllLocalButton.Size = new System.Drawing.Size(135, 27);
            this.saveAllLocalButton.TabIndex = 69;
            this.saveAllLocalButton.Text = "Save all local";
            this.saveAllLocalButton.UseVisualStyleBackColor = false;
            this.saveAllLocalButton.Click += new System.EventHandler(this.saveAllLocalButton_Click);
            // 
            // infoLabel
            // 
            this.infoLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.infoLabel.BackColor = System.Drawing.SystemColors.ControlLight;
            this.infoLabel.Font = new System.Drawing.Font("Verdana", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.infoLabel.ForeColor = System.Drawing.Color.Black;
            this.infoLabel.Location = new System.Drawing.Point(299, 7);
            this.infoLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.infoLabel.Name = "infoLabel";
            this.infoLabel.Size = new System.Drawing.Size(929, 49);
            this.infoLabel.TabIndex = 70;
            this.infoLabel.Text = "Configure Robot Constants";
            this.infoLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // rowContextMenuStrip
            // 
            this.rowContextMenuStrip.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.rowContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.deleteToolStripMenuItem});
            this.rowContextMenuStrip.Name = "rowContextMenuStrip";
            this.rowContextMenuStrip.Size = new System.Drawing.Size(155, 28);
            this.rowContextMenuStrip.Opening += new System.ComponentModel.CancelEventHandler(this.rowContextMenuStrip_Opening);
            // 
            // deleteToolStripMenuItem
            // 
            this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            this.deleteToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.Delete;
            this.deleteToolStripMenuItem.Size = new System.Drawing.Size(154, 24);
            this.deleteToolStripMenuItem.Text = "Delete";
            this.deleteToolStripMenuItem.Click += new System.EventHandler(this.deleteToolStripMenuItem_Click);
            // 
            // timer1
            // 
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // timeSinceUpload
            // 
            this.timeSinceUpload.Location = new System.Drawing.Point(12, 630);
            this.timeSinceUpload.Name = "timeSinceUpload";
            this.timeSinceUpload.Size = new System.Drawing.Size(276, 23);
            this.timeSinceUpload.TabIndex = 71;
            this.timeSinceUpload.Text = "Last Upload: ";
            this.timeSinceUpload.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // ConfigurationView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlLight;
            this.ClientSize = new System.Drawing.Size(1244, 666);
            this.Controls.Add(this.timeSinceUpload);
            this.Controls.Add(this.infoLabel);
            this.Controls.Add(this.saveAllLocalButton);
            this.Controls.Add(this.saveLocalButton);
            this.Controls.Add(this.connectionSettingsButton);
            this.Controls.Add(this.loadLocalButton);
            this.Controls.Add(this.configurationGrid);
            this.Controls.Add(this.loadRIOButton);
            this.Controls.Add(this.saveToRioButton);
            this.Controls.Add(this.deleteButton);
            this.Controls.Add(this.newFileButton);
            this.Controls.Add(this.filenameGrid);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "ConfigurationView";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Configure Constants";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.ConfigurationView_FormClosed);
            ((System.ComponentModel.ISupportInitialize)(this.filenameGrid)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.configurationGrid)).EndInit();
            this.rowContextMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.DataGridView filenameGrid;
        private FontAwesome.Sharp.IconButton newFileButton;
        private FontAwesome.Sharp.IconButton deleteButton;
        private System.Windows.Forms.Button saveToRioButton;
        private System.Windows.Forms.Button loadRIOButton;
        private System.Windows.Forms.DataGridView configurationGrid;
        private System.Windows.Forms.Button loadLocalButton;
        private FontAwesome.Sharp.IconButton connectionSettingsButton;
        private System.Windows.Forms.DataGridViewTextBoxColumn FileName;
        private System.Windows.Forms.Button saveLocalButton;
        private System.Windows.Forms.Button saveAllLocalButton;
        private System.Windows.Forms.Label infoLabel;
        private System.Windows.Forms.DataGridViewTextBoxColumn Variable_Name;
        private System.Windows.Forms.DataGridViewComboBoxColumn Variable_Type;
        private System.Windows.Forms.DataGridViewTextBoxColumn Value;
        private System.Windows.Forms.ContextMenuStrip rowContextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Label timeSinceUpload;
    }
}