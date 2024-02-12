
namespace MotionProfileMapper.Forms
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
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
            this.Comment = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.loadLocalButton = new System.Windows.Forms.Button();
            this.connectionSettingsButton = new FontAwesome.Sharp.IconButton();
            this.saveLocalButton = new System.Windows.Forms.Button();
            this.saveAllLocalButton = new System.Windows.Forms.Button();
            this.infoLabel = new System.Windows.Forms.Label();
            this.rowContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.timeSinceUpload = new System.Windows.Forms.Label();
            this.iconButton2 = new FontAwesome.Sharp.IconButton();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
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
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(188)))), ((int)(((byte)(159)))), ((int)(((byte)(245)))));
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.filenameGrid.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.filenameGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.filenameGrid.ColumnHeadersVisible = false;
            this.filenameGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.FileName});
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(188)))), ((int)(((byte)(159)))), ((int)(((byte)(245)))));
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.filenameGrid.DefaultCellStyle = dataGridViewCellStyle3;
            this.filenameGrid.GridColor = System.Drawing.Color.Silver;
            this.filenameGrid.Location = new System.Drawing.Point(15, 111);
            this.filenameGrid.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.filenameGrid.MultiSelect = false;
            this.filenameGrid.Name = "filenameGrid";
            this.filenameGrid.RowHeadersVisible = false;
            this.filenameGrid.RowHeadersWidth = 51;
            this.filenameGrid.RowTemplate.Height = 24;
            this.filenameGrid.Size = new System.Drawing.Size(273, 391);
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
            this.newFileButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.newFileButton.ForeColor = System.Drawing.Color.DarkGray;
            this.newFileButton.IconChar = FontAwesome.Sharp.IconChar.Add;
            this.newFileButton.IconColor = System.Drawing.Color.Green;
            this.newFileButton.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.newFileButton.IconSize = 24;
            this.newFileButton.Location = new System.Drawing.Point(15, 511);
            this.newFileButton.Margin = new System.Windows.Forms.Padding(0);
            this.newFileButton.Name = "newFileButton";
            this.newFileButton.Size = new System.Drawing.Size(135, 30);
            this.newFileButton.TabIndex = 49;
            this.newFileButton.UseVisualStyleBackColor = false;
            this.newFileButton.Click += new System.EventHandler(this.newFileButton_Click);
            // 
            // deleteButton
            // 
            this.deleteButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.deleteButton.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.deleteButton.Enabled = false;
            this.deleteButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.deleteButton.ForeColor = System.Drawing.Color.DarkGray;
            this.deleteButton.IconChar = FontAwesome.Sharp.IconChar.Trash;
            this.deleteButton.IconColor = System.Drawing.Color.Firebrick;
            this.deleteButton.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.deleteButton.IconSize = 24;
            this.deleteButton.Location = new System.Drawing.Point(153, 511);
            this.deleteButton.Margin = new System.Windows.Forms.Padding(0);
            this.deleteButton.Name = "deleteButton";
            this.deleteButton.Size = new System.Drawing.Size(135, 30);
            this.deleteButton.TabIndex = 50;
            this.deleteButton.UseVisualStyleBackColor = false;
            this.deleteButton.Click += new System.EventHandler(this.deleteButton_Click);
            // 
            // saveToRioButton
            // 
            this.saveToRioButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.saveToRioButton.BackColor = System.Drawing.SystemColors.ActiveBorder;
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
            this.toolTip1.SetToolTip(this.saveToRioButton, "Save configuration files to rio. (This will overwrite any pervious version on the" +
        " rio, but will not delete files that do not exist here.)");
            this.saveToRioButton.UseVisualStyleBackColor = false;
            this.saveToRioButton.Click += new System.EventHandler(this.saveToRioButton_Click);
            // 
            // loadRIOButton
            // 
            this.loadRIOButton.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.loadRIOButton.FlatAppearance.BorderSize = 0;
            this.loadRIOButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.loadRIOButton.Font = new System.Drawing.Font("Verdana", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.loadRIOButton.ForeColor = System.Drawing.Color.Teal;
            this.loadRIOButton.Location = new System.Drawing.Point(15, 20);
            this.loadRIOButton.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.loadRIOButton.Name = "loadRIOButton";
            this.loadRIOButton.Size = new System.Drawing.Size(221, 46);
            this.loadRIOButton.TabIndex = 63;
            this.loadRIOButton.Text = "Load from RIO";
            this.toolTip1.SetToolTip(this.loadRIOButton, "Load the configuration files from the rio");
            this.loadRIOButton.UseVisualStyleBackColor = false;
            this.loadRIOButton.Click += new System.EventHandler(this.loadRIOButton_Click);
            // 
            // configurationGrid
            // 
            this.configurationGrid.AllowDrop = true;
            dataGridViewCellStyle4.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.configurationGrid.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle4;
            this.configurationGrid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.configurationGrid.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle5.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            dataGridViewCellStyle5.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle5.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(188)))), ((int)(((byte)(159)))), ((int)(((byte)(245)))));
            dataGridViewCellStyle5.SelectionForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.configurationGrid.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle5;
            this.configurationGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.configurationGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Variable_Name,
            this.Variable_Type,
            this.Value,
            this.Comment});
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle6.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            dataGridViewCellStyle6.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle6.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(188)))), ((int)(((byte)(159)))), ((int)(((byte)(245)))));
            dataGridViewCellStyle6.SelectionForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle6.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.configurationGrid.DefaultCellStyle = dataGridViewCellStyle6;
            this.configurationGrid.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this.configurationGrid.Enabled = false;
            this.configurationGrid.EnableHeadersVisualStyles = false;
            this.configurationGrid.Location = new System.Drawing.Point(299, 69);
            this.configurationGrid.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.configurationGrid.MultiSelect = false;
            this.configurationGrid.Name = "configurationGrid";
            dataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle7.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle7.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            dataGridViewCellStyle7.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle7.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(188)))), ((int)(((byte)(159)))), ((int)(((byte)(245)))));
            dataGridViewCellStyle7.SelectionForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle7.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.configurationGrid.RowHeadersDefaultCellStyle = dataGridViewCellStyle7;
            this.configurationGrid.RowHeadersWidth = 51;
            this.configurationGrid.RowTemplate.Height = 24;
            this.configurationGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.configurationGrid.Size = new System.Drawing.Size(931, 584);
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
            this.Variable_Name.Width = 300;
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
            // Comment
            // 
            this.Comment.HeaderText = "Comment";
            this.Comment.MinimumWidth = 6;
            this.Comment.Name = "Comment";
            this.Comment.Width = 234;
            // 
            // loadLocalButton
            // 
            this.loadLocalButton.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.loadLocalButton.FlatAppearance.BorderSize = 0;
            this.loadLocalButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.loadLocalButton.Font = new System.Drawing.Font("Verdana", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.loadLocalButton.ForeColor = System.Drawing.Color.Black;
            this.loadLocalButton.Location = new System.Drawing.Point(15, 69);
            this.loadLocalButton.Margin = new System.Windows.Forms.Padding(1);
            this.loadLocalButton.Name = "loadLocalButton";
            this.loadLocalButton.Size = new System.Drawing.Size(273, 27);
            this.loadLocalButton.TabIndex = 66;
            this.loadLocalButton.Text = "Load from local files";
            this.toolTip1.SetToolTip(this.loadLocalButton, "Load files from this device");
            this.loadLocalButton.UseVisualStyleBackColor = false;
            this.loadLocalButton.Click += new System.EventHandler(this.loadLocalButton_Click);
            // 
            // connectionSettingsButton
            // 
            this.connectionSettingsButton.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.connectionSettingsButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.connectionSettingsButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.connectionSettingsButton.ForeColor = System.Drawing.SystemColors.AppWorkspace;
            this.connectionSettingsButton.IconChar = FontAwesome.Sharp.IconChar.Gear;
            this.connectionSettingsButton.IconColor = System.Drawing.Color.Teal;
            this.connectionSettingsButton.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.connectionSettingsButton.IconSize = 40;
            this.connectionSettingsButton.Location = new System.Drawing.Point(239, 20);
            this.connectionSettingsButton.Margin = new System.Windows.Forms.Padding(0);
            this.connectionSettingsButton.Name = "connectionSettingsButton";
            this.connectionSettingsButton.Size = new System.Drawing.Size(49, 46);
            this.connectionSettingsButton.TabIndex = 67;
            this.toolTip1.SetToolTip(this.connectionSettingsButton, "Roborio connection settings");
            this.connectionSettingsButton.UseVisualStyleBackColor = false;
            this.connectionSettingsButton.Click += new System.EventHandler(this.connectionSettingsButton_Click);
            // 
            // saveLocalButton
            // 
            this.saveLocalButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.saveLocalButton.BackColor = System.Drawing.SystemColors.ActiveBorder;
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
            this.toolTip1.SetToolTip(this.saveLocalButton, "Save selected configuration to the local drive.");
            this.saveLocalButton.UseVisualStyleBackColor = false;
            this.saveLocalButton.Click += new System.EventHandler(this.saveLocalButton_Click);
            // 
            // saveAllLocalButton
            // 
            this.saveAllLocalButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.saveAllLocalButton.BackColor = System.Drawing.SystemColors.ActiveBorder;
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
            this.toolTip1.SetToolTip(this.saveAllLocalButton, "Save all configuration files to the local drive.");
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
            this.rowContextMenuStrip.Size = new System.Drawing.Size(123, 28);
            this.rowContextMenuStrip.Opening += new System.ComponentModel.CancelEventHandler(this.rowContextMenuStrip_Opening);
            // 
            // deleteToolStripMenuItem
            // 
            this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            this.deleteToolStripMenuItem.Size = new System.Drawing.Size(122, 24);
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
            this.timeSinceUpload.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.timeSinceUpload.Location = new System.Drawing.Point(12, 630);
            this.timeSinceUpload.Name = "timeSinceUpload";
            this.timeSinceUpload.Size = new System.Drawing.Size(276, 23);
            this.timeSinceUpload.TabIndex = 71;
            this.timeSinceUpload.Text = "Last Upload: ";
            this.timeSinceUpload.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // iconButton2
            // 
            this.iconButton2.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.iconButton2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.iconButton2.ForeColor = System.Drawing.Color.DarkGray;
            this.iconButton2.IconChar = FontAwesome.Sharp.IconChar.BezierCurve;
            this.iconButton2.IconColor = System.Drawing.Color.SlateBlue;
            this.iconButton2.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.iconButton2.Location = new System.Drawing.Point(299, 20);
            this.iconButton2.Margin = new System.Windows.Forms.Padding(0);
            this.iconButton2.Name = "iconButton2";
            this.iconButton2.Size = new System.Drawing.Size(56, 47);
            this.iconButton2.TabIndex = 78;
            this.toolTip1.SetToolTip(this.iconButton2, "Switch to motion profiling mode");
            this.iconButton2.UseVisualStyleBackColor = false;
            this.iconButton2.Click += new System.EventHandler(this.iconButton2_Click);
            // 
            // ConfigurationView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlLight;
            this.ClientSize = new System.Drawing.Size(1244, 666);
            this.Controls.Add(this.iconButton2);
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
        private System.Windows.Forms.ContextMenuStrip rowContextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Label timeSinceUpload;
        private FontAwesome.Sharp.IconButton iconButton2;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Variable_Name;
        private System.Windows.Forms.DataGridViewComboBoxColumn Variable_Type;
        private System.Windows.Forms.DataGridViewTextBoxColumn Value;
        private System.Windows.Forms.DataGridViewTextBoxColumn Comment;
    }
}