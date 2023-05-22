using NppPluginNET;

namespace NavigateTo.Plugin.Namespace
{
	partial class FrmNavigateTo
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.searchComboBox = new System.Windows.Forms.ComboBox();
            this.dataGridFileList = new System.Windows.Forms.DataGridView();
            this.ColumnName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnPath = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnView = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.contextMenuStripMultiSelect = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuItemClose = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridFileList)).BeginInit();
            this.contextMenuStripMultiSelect.SuspendLayout();
            this.SuspendLayout();
            // 
            // searchComboBox
            // 
            this.searchComboBox.AccessibleRole = System.Windows.Forms.AccessibleRole.ComboBox;
            this.searchComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.searchComboBox.CausesValidation = false;
            this.searchComboBox.Location = new System.Drawing.Point(1, 0);
            this.searchComboBox.Name = "searchComboBox";
            this.searchComboBox.Size = new System.Drawing.Size(602, 21);
            this.searchComboBox.TabIndex = 1;
            this.searchComboBox.TextChanged += new System.EventHandler(this.SearchComboBoxTextChanged);
            this.searchComboBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.SearchComboBoxKeyDown);
            // 
            // dataGridFileList
            // 
            this.dataGridFileList.AllowUserToAddRows = false;
            this.dataGridFileList.AllowUserToDeleteRows = false;
            this.dataGridFileList.AllowUserToResizeRows = false;
            this.dataGridFileList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridFileList.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridFileList.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.DisplayedCellsExceptHeaders;
            this.dataGridFileList.BackgroundColor = System.Drawing.SystemColors.ButtonShadow;
            this.dataGridFileList.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleVertical;
            this.dataGridFileList.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ColumnName,
            this.ColumnPath,
            this.ColumnView});
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle3.NullValue = "-";
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridFileList.DefaultCellStyle = dataGridViewCellStyle3;
            this.dataGridFileList.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.dataGridFileList.Location = new System.Drawing.Point(1, 27);
            this.dataGridFileList.Name = "dataGridFileList";
            this.dataGridFileList.ReadOnly = true;
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle4.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle4.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridFileList.RowHeadersDefaultCellStyle = dataGridViewCellStyle4;
            this.dataGridFileList.RowHeadersWidth = 25;
            this.dataGridFileList.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dataGridFileList.RowTemplate.ReadOnly = true;
            this.dataGridFileList.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.dataGridFileList.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridFileList.ShowEditingIcon = false;
            this.dataGridFileList.ShowRowErrors = false;
            this.dataGridFileList.Size = new System.Drawing.Size(602, 412);
            this.dataGridFileList.TabIndex = 5;
            this.dataGridFileList.CellPainting += new System.Windows.Forms.DataGridViewCellPaintingEventHandler(this.dataGridFileList_CellPainting);
            this.dataGridFileList.RowPostPaint += new System.Windows.Forms.DataGridViewRowPostPaintEventHandler(this.dataGridFileList_RowPostPaint);
            this.dataGridFileList.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dataGridFileList_KeyDown);
            this.dataGridFileList.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.dataGridFileList_KeyPress);
            this.dataGridFileList.MouseClick += new System.Windows.Forms.MouseEventHandler(this.dataGridView1_MouseClick);
            this.dataGridFileList.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.dataGridView1_MouseDoubleClick);
            this.dataGridFileList.Resize += new System.EventHandler(this.dataGridFileList_Resize);
            // 
            // ColumnName
            // 
            this.ColumnName.FillWeight = 20F;
            this.ColumnName.HeaderText = "Name";
            this.ColumnName.MinimumWidth = 25;
            this.ColumnName.Name = "ColumnName";
            this.ColumnName.ReadOnly = true;
            // 
            // ColumnPath
            // 
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.ColumnPath.DefaultCellStyle = dataGridViewCellStyle1;
            this.ColumnPath.FillWeight = 70F;
            this.ColumnPath.HeaderText = "Path";
            this.ColumnPath.MinimumWidth = 50;
            this.ColumnPath.Name = "ColumnPath";
            this.ColumnPath.ReadOnly = true;
            // 
            // ColumnView
            // 
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopCenter;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.ColumnView.DefaultCellStyle = dataGridViewCellStyle2;
            this.ColumnView.FillWeight = 10F;
            this.ColumnView.HeaderText = "Source";
            this.ColumnView.MinimumWidth = 40;
            this.ColumnView.Name = "ColumnView";
            this.ColumnView.ReadOnly = true;
            // 
            // contextMenuStripMultiSelect
            // 
            this.contextMenuStripMultiSelect.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItemClose});
            this.contextMenuStripMultiSelect.Name = "contextMenuStripMultiSelect";
            this.contextMenuStripMultiSelect.Size = new System.Drawing.Size(177, 26);
            // 
            // toolStripMenuItemClose
            // 
            this.toolStripMenuItemClose.Name = "toolStripMenuItemClose";
            this.toolStripMenuItemClose.Size = new System.Drawing.Size(176, 22);
            this.toolStripMenuItemClose.Text = "Close Selected Tabs";
            this.toolStripMenuItemClose.ToolTipText = "Close All Selected Tabs";
            this.toolStripMenuItemClose.Click += new System.EventHandler(this.toolStripMenuItemClose_Click);
            // 
            // FrmNavigateTo
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.ClientSize = new System.Drawing.Size(603, 440);
            this.Controls.Add(this.dataGridFileList);
            this.Controls.Add(this.searchComboBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FrmNavigateTo";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "NavigateTo";
            this.Load += new System.EventHandler(this.frmNavigateAll_Load);
            this.VisibleChanged += new System.EventHandler(this.frmNavigateAll_VisibleChanged);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.FrmNavigateAll_KeyDown);
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.FrmNavigateAll_KeyPress);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridFileList)).EndInit();
            this.contextMenuStripMultiSelect.ResumeLayout(false);
            this.ResumeLayout(false);

		}

		#endregion
        private System.Windows.Forms.ComboBox searchComboBox;
        private System.Windows.Forms.DataGridView dataGridFileList;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnName;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnPath;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnView;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripMultiSelect;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemClose;
    }
}