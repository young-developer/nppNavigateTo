using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Kbg.NppPluginNET;
using Kbg.NppPluginNET.PluginInfrastructure;
using NppPluginNET;

namespace NavigateTo.Plugin.Namespace
{
    partial class FrmSettings : Form
    {
        private readonly IScintillaGateway editor;
        private readonly INotepadPPGateway notepad;
        public static Settings Settings { get; } = new Settings();

        public FrmSettings(IScintillaGateway editor, INotepadPPGateway notepad)
        {
            this.editor = editor;
            this.notepad = notepad;
            InitializeComponent();
            Settings.LoadConfigValues();
        }

        private void frmSettings_Load(object sender, EventArgs e)
        {
            keepOpenDlgCheckBox.Checked = Settings.GetBoolSetting(Settings.keepDlgOpen);
            dataGridFileListPreview.Columns[0].FillWeight = FrmSettings.Settings.GetIntSetting(Settings.columnNameWidth);
            dataGridFileListPreview.Columns[1].FillWeight = FrmSettings.Settings.GetIntSetting(Settings.columnPathWidth);
            dataGridFileListPreview.Columns[2].FillWeight = FrmSettings.Settings.GetIntSetting(Settings.columnSourceWidth);
        }

        private void frmSettings_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                Hide();
            }
        }

        private void KeepOpenDlgCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            Settings.SetBoolSetting(Settings.keepDlgOpen, keepOpenDlgCheckBox.Checked);
        }

        private void checkBoxSearchMenu_CheckedChanged(object sender, EventArgs e)
        {
            Settings.SetBoolSetting(Settings.searchMenuCommands, checkBoxSearchMenu.Checked);
        }

        private void frmSettings_Shown(object sender, EventArgs e)
        {
            keepOpenDlgCheckBox.Checked = Settings.GetBoolSetting(Settings.keepDlgOpen);
            numericUpDownMinGridWidth.Value = Settings.GetIntSetting(Settings.gridMinWidth);
            checkBoxSearchInFolder.Checked = Settings.GetBoolSetting(Settings.searchInCurrentFolder);
            checkBoxSearchInSubDirs.Checked = Settings.GetBoolSetting(Settings.searchInSubDirs);
            checkBoxSearchMenu.Checked = Settings.GetBoolSetting(Settings.searchMenuCommands);
            numericUpDownCharSearchLimit.Value = Settings.GetIntSetting(Settings.minTypeCharLimit);

            DataGridViewRow newRow = new DataGridViewRow();
            newRow.CreateCells(dataGridFileListPreview);
            newRow.Cells[0].Value = "Test Name";
            newRow.Cells[1].Value = "C:/TestTabs1/Tabs2/Tabs1/Tabs3/Path";
            newRow.Cells[2].Value = "Tabs";
            dataGridFileListPreview.Rows.Add(newRow);

            DataGridViewRow newRow2 = new DataGridViewRow();
            newRow2.CreateCells(dataGridFileListPreview);
            newRow2.Cells[0].Value = "Test Name 2";
            newRow2.Cells[1].Value = "C:/TestDir2/Dir3/Dir4/Dir1/Path";
            newRow2.Cells[2].Value = "Top Directory";
            dataGridFileListPreview.Rows.Add(newRow2);
            RefreshDataGridStyles();
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            Settings.SetIntSetting(Settings.gridMinWidth, (int)numericUpDownMinGridWidth.Value);
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            Settings.SetBoolSetting(Settings.searchInCurrentFolder, checkBoxSearchInFolder.Checked);
        }

        private void checkBox1_CheckedChanged_1(object sender, EventArgs e)
        {
            Settings.SetBoolSetting(Settings.searchInSubDirs, checkBoxSearchInSubDirs.Checked);
        }

        private void buttonHighlightColor_Click(object sender, EventArgs e)
        {
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                Settings.SetColorSetting(Settings.highlightColorBackground, colorDialog1.Color);
                dataGridFileListPreview.Refresh();
            }
        }

        private void buttonGridTextColor_Click(object sender, EventArgs e)
        {
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                Settings.SetColorSetting(Settings.gridTextColor, colorDialog1.Color);
            }

            RefreshDataGridStyles();
        }

        private void buttonSelectedRowBack_Click(object sender, EventArgs e)
        {
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                Settings.SetColorSetting(Settings.gridSelectedRowBackground, colorDialog1.Color);
            }

            RefreshDataGridStyles();
        }

        private void buttonSelectedRowText_Click(object sender, EventArgs e)
        {
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                Settings.SetColorSetting(Settings.gridSelectedRowForeground, colorDialog1.Color);
            }

            RefreshDataGridStyles();
        }

        private void RefreshDataGridStyles()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 =
                new System.Windows.Forms.DataGridViewCellStyle();
            dataGridViewCellStyle1.ForeColor = Settings.GetColorSetting(Settings.gridTextColor);
            dataGridViewCellStyle1.SelectionBackColor = Settings.GetColorSetting(Settings.gridSelectedRowBackground);
            dataGridViewCellStyle1.SelectionForeColor = Settings.GetColorSetting(Settings.gridSelectedRowForeground);
            dataGridFileListPreview.DefaultCellStyle = dataGridViewCellStyle1;
            dataGridFileListPreview.BackgroundColor = Settings.GetColorSetting(Settings.gridBackgroundColor);
            dataGridFileListPreview.Refresh();
        }

        private void dataGridFileListPreview_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.Value == null) return;

            if (e.RowIndex > -1 && e.ColumnIndex > -1)
            {
                String search = "Test";
                var filterList = search.Split(null);
                List<Rectangle> rectangleList = new List<Rectangle>();

                // Check data for search  
                if (!String.IsNullOrWhiteSpace(search.Trim()))
                {
                    foreach (var word in filterList)
                    {
                        String gridCellValue = e.FormattedValue.ToString();
                        int startIndexInCellValue = gridCellValue.ToLower().IndexOf(word.Trim().ToLower(),
                            StringComparison.CurrentCultureIgnoreCase);
                        if (startIndexInCellValue >= 0)
                        {
                            e.Handled = true;
                            e.PaintBackground(e.CellBounds, true);
                            int indexOfN = gridCellValue.IndexOf("\n", StringComparison.CurrentCultureIgnoreCase);
                            Rectangle hlRect = new Rectangle();
                            hlRect.Y = e.CellBounds.Y + 2;

                            String sBeforeSearchword = gridCellValue.Substring(0, startIndexInCellValue);
                            String sSearchWord = gridCellValue.Substring(startIndexInCellValue, word.Trim().Length);
                            Size s1 = TextRenderer.MeasureText(e.Graphics, sBeforeSearchword, e.CellStyle.Font,
                                e.CellBounds.Size, TextFormatFlags.TextBoxControl);
                            Size s2 = TextRenderer.MeasureText(e.Graphics, sSearchWord, e.CellStyle.Font,
                                e.CellBounds.Size, TextFormatFlags.TextBoxControl);
                            if (s1.Width > 5)
                            {
                                hlRect.X = e.CellBounds.X + s1.Width - 5;
                                hlRect.Width = s2.Width - 6;

                                if (indexOfN != -1)
                                {
                                    if (startIndexInCellValue > indexOfN)
                                    {
                                        int cellRow2Index = startIndexInCellValue - indexOfN;
                                        String breakWord = gridCellValue.Substring(indexOfN, cellRow2Index);
                                        Size s3 = TextRenderer.MeasureText(e.Graphics, breakWord, e.CellStyle.Font,
                                            e.CellBounds.Size, TextFormatFlags.TextBoxControl);
                                        hlRect.X = e.CellBounds.X + 2 + s3.Width - 7;
                                        hlRect.Y = e.CellBounds.Y + s2.Height + 2;
                                    }
                                }
                            }
                            else
                            {
                                hlRect.X = e.CellBounds.X + 2;
                                hlRect.Width = s2.Width - 6;
                            }

                            if (indexOfN == -1)
                            {
                                Size s4 = TextRenderer.MeasureText(e.Graphics, gridCellValue, e.CellStyle.Font,
                                    e.CellBounds.Size, TextFormatFlags.TextBoxControl);
                                if (s4.Width < e.CellBounds.Width)
                                {
                                    hlRect.Y = e.CellBounds.Y + ((e.CellBounds.Height - s4.Height) / 2);
                                }
                            }

                            hlRect.Height = s2.Height;
                            rectangleList.Add(hlRect);
                        }
                    }
                }

                SolidBrush hlBrush =
                    new SolidBrush(Color.FromArgb(Settings.GetIntSetting(Settings.highlightColorBackground)));

                foreach (var recto in rectangleList)
                {
                    e.Graphics.FillRectangle(hlBrush, recto);
                }

                hlBrush.Dispose();
                e.PaintContent(e.CellBounds);
            }
        }

        private void buttonResetStyle_Click(object sender, EventArgs e)
        {
            Settings.SetColorSetting(Settings.gridTextColor, System.Drawing.SystemColors.ControlText);
            Settings.SetColorSetting(Settings.gridSelectedRowBackground, System.Drawing.SystemColors.Highlight);
            Settings.SetColorSetting(Settings.gridSelectedRowForeground, System.Drawing.SystemColors.ControlText);
            Settings.SetColorSetting(Settings.gridBackgroundColor, System.Drawing.SystemColors.AppWorkspace);
            Settings.SetColorSetting(Settings.highlightColorBackground, Color.Orange);
            RefreshDataGridStyles();
        }

        private void numericUpDownCharSearchLimit_ValueChanged_1(object sender, EventArgs e)
        {
            Settings.SetIntSetting(Settings.minTypeCharLimit, (int)numericUpDownCharSearchLimit.Value);
        }

        private void dataGridFileListPreview_ColumnWidthChanged(object sender, DataGridViewColumnEventArgs e)
        {
            int nameWidth = dataGridFileListPreview.Columns[0].Width;
            int pathWidth = dataGridFileListPreview.Columns[1].Width;
            int sourceWidth = dataGridFileListPreview.Columns[2].Width;
            int sum = nameWidth + pathWidth + sourceWidth;

            float percentName = (float)Math.Round((double)(100 * nameWidth) / sum);
            float percentPath = (float)Math.Round((double)(100 * pathWidth) / sum);
            float percentSource =(float)Math.Round((double)(100 * sourceWidth) / sum);

            if (percentName > 0 && percentPath > 0 && percentSource > 0)
            {
                Settings.SetIntSetting(Settings.columnNameWidth, (int)percentName);
                Settings.SetIntSetting(Settings.columnPathWidth, (int)percentPath);
                Settings.SetIntSetting(Settings.columnSourceWidth, (int)percentSource);
            }
        }

        private void gridBackground_Click(object sender, EventArgs e)
        {
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                Settings.SetColorSetting(Settings.gridBackgroundColor, colorDialog1.Color);
            }

            RefreshDataGridStyles();
        }
    }
}