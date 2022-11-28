﻿namespace NavigateTo.Plugin.Namespace
{
    partial class FrmSettings
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
            this.keepOpenDlgCheckBox = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.checkBoxKeepSelected = new System.Windows.Forms.CheckBox();
            this.checkBoxCleanSearch = new System.Windows.Forms.CheckBox();
            this.checkBoxSearchInFolder = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.numericUpDownMinGridWidth = new System.Windows.Forms.NumericUpDown();
            this.checkBoxSearchInSubDirs = new System.Windows.Forms.CheckBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.comboBoxSortedByAfterFilter = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.numericUpDownCharSearchLimit = new System.Windows.Forms.NumericUpDown();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.checkBoxFileNameResults = new System.Windows.Forms.CheckBox();
            this.checkBoxSearchMenu = new System.Windows.Forms.CheckBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.gridBackground = new System.Windows.Forms.Button();
            this.buttonResetStyle = new System.Windows.Forms.Button();
            this.buttonSelectedRowText = new System.Windows.Forms.Button();
            this.buttonSelectedRowBack = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.buttonGridTextColor = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.colorDialog1 = new System.Windows.Forms.ColorDialog();
            this.dataGridFileListPreview = new System.Windows.Forms.DataGridView();
            this.ColumnName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnPath = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnView = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.label2 = new System.Windows.Forms.Label();
            this.comboBoxSortOrder = new System.Windows.Forms.ComboBox();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownMinGridWidth)).BeginInit();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownCharSearchLimit)).BeginInit();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridFileListPreview)).BeginInit();
            this.SuspendLayout();
            // 
            // keepOpenDlgCheckBox
            // 
            this.keepOpenDlgCheckBox.AutoSize = true;
            this.keepOpenDlgCheckBox.Location = new System.Drawing.Point(6, 28);
            this.keepOpenDlgCheckBox.Name = "keepOpenDlgCheckBox";
            this.keepOpenDlgCheckBox.Size = new System.Drawing.Size(114, 17);
            this.keepOpenDlgCheckBox.TabIndex = 2;
            this.keepOpenDlgCheckBox.Text = "Keep dialog visible";
            this.keepOpenDlgCheckBox.UseVisualStyleBackColor = true;
            this.keepOpenDlgCheckBox.CheckedChanged += new System.EventHandler(this.KeepOpenDlgCheckBox_CheckedChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.checkBoxKeepSelected);
            this.groupBox1.Controls.Add(this.checkBoxCleanSearch);
            this.groupBox1.Controls.Add(this.keepOpenDlgCheckBox);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(283, 111);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "General";
            // 
            // checkBoxKeepSelected
            // 
            this.checkBoxKeepSelected.AutoSize = true;
            this.checkBoxKeepSelected.Location = new System.Drawing.Point(6, 74);
            this.checkBoxKeepSelected.Name = "checkBoxKeepSelected";
            this.checkBoxKeepSelected.Size = new System.Drawing.Size(143, 17);
            this.checkBoxKeepSelected.TabIndex = 4;
            this.checkBoxKeepSelected.Text = "Keep First Row Selected";
            this.checkBoxKeepSelected.UseVisualStyleBackColor = true;
            this.checkBoxKeepSelected.CheckedChanged += new System.EventHandler(this.checkBoxKeepSelected_CheckedChanged);
            // 
            // checkBoxCleanSearch
            // 
            this.checkBoxCleanSearch.AutoSize = true;
            this.checkBoxCleanSearch.Location = new System.Drawing.Point(6, 51);
            this.checkBoxCleanSearch.Name = "checkBoxCleanSearch";
            this.checkBoxCleanSearch.Size = new System.Drawing.Size(111, 17);
            this.checkBoxCleanSearch.TabIndex = 3;
            this.checkBoxCleanSearch.Text = "Clear search input";
            this.checkBoxCleanSearch.UseVisualStyleBackColor = true;
            this.checkBoxCleanSearch.CheckedChanged += new System.EventHandler(this.checkBoxCleanSearch_CheckedChanged);
            // 
            // checkBoxSearchInFolder
            // 
            this.checkBoxSearchInFolder.AutoSize = true;
            this.checkBoxSearchInFolder.Location = new System.Drawing.Point(6, 19);
            this.checkBoxSearchInFolder.Name = "checkBoxSearchInFolder";
            this.checkBoxSearchInFolder.Size = new System.Drawing.Size(251, 17);
            this.checkBoxSearchInFolder.TabIndex = 5;
            this.checkBoxSearchInFolder.Text = "Search in current file folder ( Top directory only )";
            this.checkBoxSearchInFolder.UseVisualStyleBackColor = true;
            this.checkBoxSearchInFolder.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(67, 29);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(77, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Min Grid Width";
            // 
            // numericUpDownMinGridWidth
            // 
            this.numericUpDownMinGridWidth.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numericUpDownMinGridWidth.Location = new System.Drawing.Point(12, 25);
            this.numericUpDownMinGridWidth.Maximum = new decimal(new int[] {
            5000,
            0,
            0,
            0});
            this.numericUpDownMinGridWidth.Minimum = new decimal(new int[] {
            20,
            0,
            0,
            0});
            this.numericUpDownMinGridWidth.Name = "numericUpDownMinGridWidth";
            this.numericUpDownMinGridWidth.Size = new System.Drawing.Size(49, 20);
            this.numericUpDownMinGridWidth.TabIndex = 4;
            this.numericUpDownMinGridWidth.Value = new decimal(new int[] {
            250,
            0,
            0,
            0});
            this.numericUpDownMinGridWidth.ValueChanged += new System.EventHandler(this.numericUpDown1_ValueChanged);
            // 
            // checkBoxSearchInSubDirs
            // 
            this.checkBoxSearchInSubDirs.AutoSize = true;
            this.checkBoxSearchInSubDirs.Location = new System.Drawing.Point(26, 42);
            this.checkBoxSearchInSubDirs.Name = "checkBoxSearchInSubDirs";
            this.checkBoxSearchInSubDirs.Size = new System.Drawing.Size(160, 17);
            this.checkBoxSearchInSubDirs.TabIndex = 5;
            this.checkBoxSearchInSubDirs.Text = "Search in sub directories too";
            this.checkBoxSearchInSubDirs.UseVisualStyleBackColor = true;
            this.checkBoxSearchInSubDirs.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged_1);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.comboBoxSortOrder);
            this.groupBox2.Controls.Add(this.comboBoxSortedByAfterFilter);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.numericUpDownCharSearchLimit);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.numericUpDownMinGridWidth);
            this.groupBox2.Location = new System.Drawing.Point(311, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(256, 111);
            this.groupBox2.TabIndex = 6;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "File List";
            // 
            // comboBoxSortedByAfterFilter
            // 
            this.comboBoxSortedByAfterFilter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxSortedByAfterFilter.FormattingEnabled = true;
            this.comboBoxSortedByAfterFilter.Items.AddRange(new object[] {
            "-",
            "Name",
            "Path"});
            this.comboBoxSortedByAfterFilter.Location = new System.Drawing.Point(120, 80);
            this.comboBoxSortedByAfterFilter.Name = "comboBoxSortedByAfterFilter";
            this.comboBoxSortedByAfterFilter.Size = new System.Drawing.Size(53, 21);
            this.comboBoxSortedByAfterFilter.TabIndex = 9;
            this.comboBoxSortedByAfterFilter.SelectedIndexChanged += new System.EventHandler(this.comboBoxSortedByAfterFilter_SelectedIndexChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(12, 83);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(102, 13);
            this.label6.TabIndex = 8;
            this.label6.Text = "Sort after search by:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(67, 58);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(86, 13);
            this.label5.TabIndex = 7;
            this.label5.Text = "Min Char Search";
            // 
            // numericUpDownCharSearchLimit
            // 
            this.numericUpDownCharSearchLimit.Location = new System.Drawing.Point(12, 56);
            this.numericUpDownCharSearchLimit.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numericUpDownCharSearchLimit.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDownCharSearchLimit.Name = "numericUpDownCharSearchLimit";
            this.numericUpDownCharSearchLimit.Size = new System.Drawing.Size(49, 20);
            this.numericUpDownCharSearchLimit.TabIndex = 6;
            this.numericUpDownCharSearchLimit.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.numericUpDownCharSearchLimit.ValueChanged += new System.EventHandler(this.numericUpDownCharSearchLimit_ValueChanged_1);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.checkBoxFileNameResults);
            this.groupBox3.Controls.Add(this.checkBoxSearchMenu);
            this.groupBox3.Controls.Add(this.checkBoxSearchInFolder);
            this.groupBox3.Controls.Add(this.checkBoxSearchInSubDirs);
            this.groupBox3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox3.Location = new System.Drawing.Point(12, 129);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(283, 123);
            this.groupBox3.TabIndex = 7;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Search behavior";
            // 
            // checkBoxFileNameResults
            // 
            this.checkBoxFileNameResults.AutoSize = true;
            this.checkBoxFileNameResults.Location = new System.Drawing.Point(6, 87);
            this.checkBoxFileNameResults.Name = "checkBoxFileNameResults";
            this.checkBoxFileNameResults.Size = new System.Drawing.Size(144, 17);
            this.checkBoxFileNameResults.TabIndex = 7;
            this.checkBoxFileNameResults.Text = "Prefer filename over path";
            this.checkBoxFileNameResults.UseVisualStyleBackColor = true;
            this.checkBoxFileNameResults.CheckedChanged += new System.EventHandler(this.checkBoxFileNameResults_CheckedChanged);
            // 
            // checkBoxSearchMenu
            // 
            this.checkBoxSearchMenu.AutoSize = true;
            this.checkBoxSearchMenu.Location = new System.Drawing.Point(6, 65);
            this.checkBoxSearchMenu.Name = "checkBoxSearchMenu";
            this.checkBoxSearchMenu.Size = new System.Drawing.Size(211, 17);
            this.checkBoxSearchMenu.TabIndex = 6;
            this.checkBoxSearchMenu.Text = "Search menu commands (experimental)";
            this.checkBoxSearchMenu.UseVisualStyleBackColor = true;
            this.checkBoxSearchMenu.CheckedChanged += new System.EventHandler(this.checkBoxSearchMenu_CheckedChanged);
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.gridBackground);
            this.groupBox4.Controls.Add(this.buttonResetStyle);
            this.groupBox4.Controls.Add(this.buttonSelectedRowText);
            this.groupBox4.Controls.Add(this.buttonSelectedRowBack);
            this.groupBox4.Controls.Add(this.label4);
            this.groupBox4.Controls.Add(this.label3);
            this.groupBox4.Controls.Add(this.buttonGridTextColor);
            this.groupBox4.Controls.Add(this.button1);
            this.groupBox4.Location = new System.Drawing.Point(311, 129);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(256, 123);
            this.groupBox4.TabIndex = 8;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Appearance";
            // 
            // gridBackground
            // 
            this.gridBackground.Location = new System.Drawing.Point(12, 94);
            this.gridBackground.Name = "gridBackground";
            this.gridBackground.Size = new System.Drawing.Size(114, 23);
            this.gridBackground.TabIndex = 8;
            this.gridBackground.Text = "Grid Background";
            this.gridBackground.UseVisualStyleBackColor = true;
            this.gridBackground.Click += new System.EventHandler(this.gridBackground_Click);
            // 
            // buttonResetStyle
            // 
            this.buttonResetStyle.Location = new System.Drawing.Point(203, 94);
            this.buttonResetStyle.Name = "buttonResetStyle";
            this.buttonResetStyle.Size = new System.Drawing.Size(47, 23);
            this.buttonResetStyle.TabIndex = 7;
            this.buttonResetStyle.Text = "Reset";
            this.buttonResetStyle.UseVisualStyleBackColor = true;
            this.buttonResetStyle.Click += new System.EventHandler(this.buttonResetStyle_Click);
            // 
            // buttonSelectedRowText
            // 
            this.buttonSelectedRowText.Location = new System.Drawing.Point(179, 53);
            this.buttonSelectedRowText.Name = "buttonSelectedRowText";
            this.buttonSelectedRowText.Size = new System.Drawing.Size(40, 23);
            this.buttonSelectedRowText.TabIndex = 6;
            this.buttonSelectedRowText.Text = "Text";
            this.buttonSelectedRowText.UseVisualStyleBackColor = true;
            this.buttonSelectedRowText.Click += new System.EventHandler(this.buttonSelectedRowText_Click);
            // 
            // buttonSelectedRowBack
            // 
            this.buttonSelectedRowBack.Location = new System.Drawing.Point(99, 53);
            this.buttonSelectedRowBack.Name = "buttonSelectedRowBack";
            this.buttonSelectedRowBack.Size = new System.Drawing.Size(74, 23);
            this.buttonSelectedRowBack.TabIndex = 5;
            this.buttonSelectedRowBack.Text = "Background";
            this.buttonSelectedRowBack.UseVisualStyleBackColor = true;
            this.buttonSelectedRowBack.Click += new System.EventHandler(this.buttonSelectedRowBack_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(9, 58);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(74, 13);
            this.label4.TabIndex = 4;
            this.label4.Text = "Selected Row";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(9, 24);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(48, 13);
            this.label3.TabIndex = 3;
            this.label3.Text = "Highlight";
            // 
            // buttonGridTextColor
            // 
            this.buttonGridTextColor.Location = new System.Drawing.Point(179, 19);
            this.buttonGridTextColor.Name = "buttonGridTextColor";
            this.buttonGridTextColor.Size = new System.Drawing.Size(40, 23);
            this.buttonGridTextColor.TabIndex = 2;
            this.buttonGridTextColor.Text = "Text";
            this.buttonGridTextColor.UseVisualStyleBackColor = true;
            this.buttonGridTextColor.Click += new System.EventHandler(this.buttonGridTextColor_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(99, 19);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(74, 23);
            this.button1.TabIndex = 1;
            this.button1.Text = "Background";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.buttonHighlightColor_Click);
            // 
            // dataGridFileListPreview
            // 
            this.dataGridFileListPreview.AllowUserToAddRows = false;
            this.dataGridFileListPreview.AllowUserToDeleteRows = false;
            this.dataGridFileListPreview.AllowUserToOrderColumns = true;
            this.dataGridFileListPreview.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridFileListPreview.BackgroundColor = System.Drawing.SystemColors.ControlLight;
            this.dataGridFileListPreview.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dataGridFileListPreview.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
            this.dataGridFileListPreview.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridFileListPreview.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ColumnName,
            this.ColumnPath,
            this.ColumnView});
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridFileListPreview.DefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridFileListPreview.Location = new System.Drawing.Point(12, 274);
            this.dataGridFileListPreview.Name = "dataGridFileListPreview";
            this.dataGridFileListPreview.ReadOnly = true;
            this.dataGridFileListPreview.RowHeadersVisible = false;
            this.dataGridFileListPreview.RowTemplate.ReadOnly = true;
            this.dataGridFileListPreview.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridFileListPreview.Size = new System.Drawing.Size(555, 155);
            this.dataGridFileListPreview.TabIndex = 9;
            this.dataGridFileListPreview.CellPainting += new System.Windows.Forms.DataGridViewCellPaintingEventHandler(this.dataGridFileListPreview_CellPainting);
            this.dataGridFileListPreview.ColumnWidthChanged += new System.Windows.Forms.DataGridViewColumnEventHandler(this.dataGridFileListPreview_ColumnWidthChanged);
            // 
            // ColumnName
            // 
            this.ColumnName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.ColumnName.FillWeight = 24F;
            this.ColumnName.HeaderText = "Name";
            this.ColumnName.MinimumWidth = 25;
            this.ColumnName.Name = "ColumnName";
            this.ColumnName.ReadOnly = true;
            // 
            // ColumnPath
            // 
            this.ColumnPath.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.ColumnPath.FillWeight = 67F;
            this.ColumnPath.HeaderText = "Path";
            this.ColumnPath.MinimumWidth = 50;
            this.ColumnPath.Name = "ColumnPath";
            this.ColumnPath.ReadOnly = true;
            // 
            // ColumnView
            // 
            this.ColumnView.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.ColumnView.FillWeight = 10F;
            this.ColumnView.HeaderText = "Source";
            this.ColumnView.MinimumWidth = 40;
            this.ColumnView.Name = "ColumnView";
            this.ColumnView.ReadOnly = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(15, 255);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(48, 13);
            this.label2.TabIndex = 10;
            this.label2.Text = "Preview:";
            // 
            // comboBoxSortOrder
            // 
            this.comboBoxSortOrder.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxSortOrder.FormattingEnabled = true;
            this.comboBoxSortOrder.Items.AddRange(new object[] {
            "ASC",
            "DESC"});
            this.comboBoxSortOrder.Location = new System.Drawing.Point(179, 80);
            this.comboBoxSortOrder.Name = "comboBoxSortOrder";
            this.comboBoxSortOrder.Size = new System.Drawing.Size(53, 21);
            this.comboBoxSortOrder.TabIndex = 10;
            this.comboBoxSortOrder.SelectedIndexChanged += new System.EventHandler(this.comboBoxSortOrder_SelectedIndexChanged);
            // 
            // FrmSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(579, 443);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.dataGridFileListPreview);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Name = "FrmSettings";
            this.ShowIcon = false;
            this.Text = "Navigate To - Settings";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmSettings_FormClosing);
            this.Load += new System.EventHandler(this.frmSettings_Load);
            this.Shown += new System.EventHandler(this.frmSettings_Shown);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownMinGridWidth)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownCharSearchLimit)).EndInit();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridFileListPreview)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.CheckBox keepOpenDlgCheckBox;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown numericUpDownMinGridWidth;
        private System.Windows.Forms.CheckBox checkBoxSearchInFolder;
        private System.Windows.Forms.CheckBox checkBoxSearchInSubDirs;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.ColorDialog colorDialog1;
        private System.Windows.Forms.Button buttonGridTextColor;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.DataGridView dataGridFileListPreview;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button buttonSelectedRowText;
        private System.Windows.Forms.Button buttonSelectedRowBack;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button buttonResetStyle;
        private System.Windows.Forms.CheckBox checkBoxSearchMenu;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.NumericUpDown numericUpDownCharSearchLimit;
        private System.Windows.Forms.Button gridBackground;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnName;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnPath;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnView;
        private System.Windows.Forms.CheckBox checkBoxCleanSearch;
        private System.Windows.Forms.CheckBox checkBoxKeepSelected;
        private System.Windows.Forms.CheckBox checkBoxFileNameResults;
        private System.Windows.Forms.ComboBox comboBoxSortedByAfterFilter;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox comboBoxSortOrder;
    }
}