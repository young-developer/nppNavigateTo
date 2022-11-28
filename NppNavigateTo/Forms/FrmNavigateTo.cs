﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Shapes;
using Kbg.NppPluginNET;
using Kbg.NppPluginNET.PluginInfrastructure;
using NppPluginNET;
using static System.String;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using Control = System.Windows.Forms.Control;
using KeyEventArgs = System.Windows.Forms.KeyEventArgs;
using MouseEventArgs = System.Windows.Forms.MouseEventArgs;
using Path = System.IO.Path;
using Rectangle = System.Drawing.Rectangle;

namespace NavigateTo.Plugin.Namespace
{
    partial class FrmNavigateTo : Form
    {
        private readonly IScintillaGateway editor;
        private readonly INotepadPPGateway notepad;
        private static ArrayList ListFiles = new ArrayList();
        private const string TABS = "Tabs";
        private const string CMD = "Cmd";
        private const string FOLDER_TOP = "Top Directory";
        private const string FOLDER_SUB = "Sub Directories";

        public List<FileModel> FileList { get; set; }

        public List<FileModel> FilteredFileList { get; set; }

        public FrmNavigateTo(IScintillaGateway editor, INotepadPPGateway notepad)
        {
            this.editor = editor;
            this.notepad = notepad;

            InitializeComponent();
            ReloadFileList();
            this.notepad.ReloadMenuItems();
        }

        void SearchInCurrentDirectory(string s, string searchPattern1)
        {
            string currentFilePath = notepad.GetCurrentFileDirectory();
            if (!String.IsNullOrWhiteSpace(currentFilePath))
            {
                bool searchInSubDirs = FrmSettings.Settings.GetBoolSetting(Settings.searchInSubDirs);
                foreach (var filePath in Directory.GetFiles(currentFilePath, searchPattern1,
                                 searchInSubDirs ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly)
                             .AsParallel().Where(e =>
                                 e.IndexOf(s, StringComparison.CurrentCultureIgnoreCase) >= 0 ||
                                 e.Contains(s))
                             .ToList())
                {
                    if (!FilteredFileList.Exists(file => file.FilePath.Equals(filePath)))
                    {
                        FilteredFileList.Add(new FileModel(Path.GetFileName(filePath), filePath, -1, -1,
                            searchInSubDirs ? FOLDER_SUB : FOLDER_TOP));
                    }
                }
            }
        }

        public void FilterDataGrid(string filter)
        {
            if (!string.IsNullOrEmpty(searchComboBox.Text)) filter = searchComboBox.Text;

            string searchPattern = "*";
            if (filter.Contains("*"))
            {
                searchPattern = filter;
            }

            dataGridFileList.Rows.Clear();
            if (IsNullOrWhiteSpace(filter))
            {
                FileList.ForEach(file =>
                {
                    DataGridViewRow newRow = new DataGridViewRow();

                    newRow.CreateCells(dataGridFileList);

                    newRow.Cells[0].Value = file.FileName;
                    newRow.Cells[1].Value = file.FilePath;
                    newRow.Cells[2].Value = file.View;

                    dataGridFileList.Rows.Add(newRow);
                });
            }
            else
            {
                FilteredFileList = FileList.AsParallel()
                    .Where(e => e.FilePath.IndexOf(filter, StringComparison.CurrentCultureIgnoreCase) >= 0 ||
                                e.FileName.Contains(filter))
                    .ToList();

                if (FrmSettings.Settings.GetBoolSetting(Settings.searchInCurrentFolder))
                {
                    SearchInCurrentDirectory(filter, searchPattern);
                }

                if (FrmSettings.Settings.GetBoolSetting(Settings.searchMenuCommands))
                {
                    FilterMenuCommands(filter);
                }

                FilteredFileList.ForEach(file =>
                {
                    DataGridViewRow newRow = new DataGridViewRow();

                    newRow.CreateCells(dataGridFileList);

                    newRow.Cells[0].Value = file.FileName;
                    newRow.Cells[1].Value = file.FilePath;
                    newRow.Cells[2].Value = file.View;

                    dataGridFileList.Rows.Add(newRow);
                });
            }
        }

        private void FilterMenuCommands(string filter)
        {
            foreach (var nppMenuCmd in notepad.MainMenuItems.AsParallel().Where(e => e.Value.IndexOf(filter,
                             StringComparison.CurrentCultureIgnoreCase) >= 0 ||
                         e.Value.Contains(filter)).ToList())
            {
                FilteredFileList.Add(new FileModel(
                    nppMenuCmd.Key.ToString(),
                    nppMenuCmd.Value, nppMenuCmd.GetHashCode(), (uint)nppMenuCmd.Key,
                    CMD));
            }
        }

        public void ReloadFileList()
        {
            if (FileList == null) FileList = new List<FileModel>();
            int filesCount =
                (int)Win32.SendMessage(PluginBase.nppData._nppHandle, (uint)NppMsg.NPPM_GETNBOPENFILES, 0, 0);
            int filesCount2 =
                (int)Win32.SendMessage(PluginBase.nppData._nppHandle, (uint)NppMsg.NPPM_GETNBOPENFILES, 0, 1);
            FileList.Clear();
            using (ClikeStringArray cStrArray = new ClikeStringArray(filesCount, Win32.MAX_PATH))
            {
                if (Win32.SendMessage(PluginBase.nppData._nppHandle, (uint)NppMsg.NPPM_GETOPENFILENAMESPRIMARY,
                        cStrArray.NativePointer, filesCount) != IntPtr.Zero)
                    for (int index = 0; index < filesCount; ++index)
                    {
                        IntPtr bufferId = Win32.SendMessage(PluginBase.nppData._nppHandle,
                            (uint)NppMsg.NPPM_GETBUFFERIDFROMPOS, index, 0);
                        if (bufferId != IntPtr.Zero)
                        {
                            FileList.Add(new FileModel(Path.GetFileName(cStrArray.ManagedStringsUnicode[index]),
                                cStrArray.ManagedStringsUnicode[index], index, bufferId.ToInt64(), TABS));
                        }
                    }
            }

            using (ClikeStringArray cStrArray = new ClikeStringArray(filesCount2, Win32.MAX_PATH))
            {
                if (Win32.SendMessage(PluginBase.nppData._nppHandle, (uint)NppMsg.NPPM_GETOPENFILENAMESSECOND,
                        cStrArray.NativePointer, filesCount2) != IntPtr.Zero)
                    for (int index = 0; index < filesCount2; ++index)
                    {
                        IntPtr bufferId = Win32.SendMessage(PluginBase.nppData._nppHandle,
                            (uint)NppMsg.NPPM_GETBUFFERIDFROMPOS, index, 1);
                        if (bufferId != IntPtr.Zero)
                        {
                            FileList.Add(new FileModel(Path.GetFileName(cStrArray.ManagedStringsUnicode[index]),
                                cStrArray.ManagedStringsUnicode[index], index, bufferId.ToInt64(), TABS));
                        }
                    }
            }
        }

        private void RefreshDataGridStyles()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 =
                new System.Windows.Forms.DataGridViewCellStyle();
            dataGridViewCellStyle1.ForeColor = FrmSettings.Settings.GetColorSetting(Settings.gridTextColor);
            dataGridViewCellStyle1.SelectionBackColor =
                FrmSettings.Settings.GetColorSetting(Settings.gridSelectedRowBackground);
            dataGridViewCellStyle1.SelectionForeColor =
                FrmSettings.Settings.GetColorSetting(Settings.gridSelectedRowForeground);
            dataGridFileList.DefaultCellStyle = dataGridViewCellStyle1;
            dataGridFileList.BackgroundColor = FrmSettings.Settings.GetColorSetting(Settings.gridBackgroundColor);
        }

        void frmNavigateAll_VisibleChanged(object sender, EventArgs e)
        {
            if (Visible)
            {
                SetColumnsWidth();
                RefreshDataGridStyles();
                searchComboBox.Focus();
                searchComboBox.Select();
            }
            else
            {
                Win32.SendMessage(PluginBase.nppData._nppHandle, (uint)NppMsg.NPPM_SETMENUITEMCHECK,
                    PluginBase._funcItems.Items[Main.idFormNavigateAll]._cmdID, 0);
            }
        }

        private void SetColumnsWidth()
        {
            dataGridFileList.Columns[0].FillWeight = FrmSettings.Settings.GetIntSetting(Settings.columnNameWidth);
            dataGridFileList.Columns[1].FillWeight = FrmSettings.Settings.GetIntSetting(Settings.columnPathWidth);
            dataGridFileList.Columns[2].FillWeight = FrmSettings.Settings.GetIntSetting(Settings.columnSourceWidth);
        }

        private void frmNavigateAll_Load(object sender, EventArgs e)
        {
            ReloadFileList();
            FilterDataGrid("");
        }

        private void dataGridView1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                int currentMouseOverRow = dataGridFileList.HitTest(e.X, e.Y).RowIndex;
                if (dataGridFileList.SelectedRows.Count == 1)
                {
                    if (currentMouseOverRow != -1 &&
                        dataGridFileList.Rows[currentMouseOverRow].Cells[2].Value.Equals(TABS))
                    {
                        Win32.SendMessage(PluginBase.nppData._nppHandle, (uint)NppMsg.NPPM_TRIGGERTABBARCONTEXTMENU, 0,
                            FileList.FindIndex(f =>
                                f.FilePath.Equals(dataGridFileList.Rows[currentMouseOverRow].Cells[1].Value)));
                    }
                }
                else
                {
                    contextMenuStripMultiSelect.Show(dataGridFileList, new Point(e.X, e.Y));
                }
            }
        }

        private void SearchComboBoxKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Down:
                    e.Handled = NavigateGridDown((Control.ModifierKeys & Keys.Shift) == Keys.Shift);
                    break;

                case Keys.Up:
                    e.Handled = NavigateGridUp((Control.ModifierKeys & Keys.Shift) == Keys.Shift);
                    break;

                case Keys.Tab:
                {
                    Control next = GetNextControl((Control)sender, !e.Shift);
                    while ((next == null) || (!next.TabStop)) next = GetNextControl(next, !e.Shift);
                    next.Focus();
                    e.Handled = true;
                }
                    break;
                case Keys.Enter:
                    ExecuteCurrentAction();
                    e.Handled = true;
                    break;
                case Keys.Escape:
                {
                    HideWindow();
                    editor.GrabFocus();
                }
                    break;
            }
        }


        private bool NavigateGridUp(bool isShiftPressed)
        {
            if (dataGridFileList.Rows.Count == 0)
            {
                return true;
            }

            if (dataGridFileList.SelectedRows.Count == 0)
            {
                dataGridFileList.Rows[0].Selected = true;
                return true;
            }

            int lastSelectedIndex = dataGridFileList.SelectedRows[dataGridFileList.SelectedRows.Count - 1].Index;
            if (dataGridFileList.MultiSelect)
            {
                dataGridFileList.MultiSelect = false;
                dataGridFileList.MultiSelect = true;
            }


            if (dataGridFileList.Rows[lastSelectedIndex].Index > 0)
                dataGridFileList.Rows[lastSelectedIndex - 1].Selected = true;
            else
            {
                dataGridFileList.Rows[dataGridFileList.Rows.Count - 1].Selected = true;
            }

            dataGridFileList.FirstDisplayedScrollingRowIndex = dataGridFileList.SelectedRows[0].Index;
            return true;
        }

        private bool NavigateGridDown(bool isShiftPressed)
        {
            if (dataGridFileList.Rows.Count == 0)
            {
                return true;
            }

            if (dataGridFileList.SelectedRows.Count == 0)
            {
                dataGridFileList.Rows[0].Selected = true;
                return true;
            }

            int lastSelectedIndex = dataGridFileList.SelectedRows[dataGridFileList.SelectedRows.Count - 1].Index;
            if (dataGridFileList.MultiSelect)
            {
                dataGridFileList.MultiSelect = false;
                dataGridFileList.MultiSelect = true;
            }

            if (dataGridFileList.Rows[lastSelectedIndex].Index + 1 < dataGridFileList.Rows.Count)
            {
                dataGridFileList.Rows[lastSelectedIndex + 1].Selected = true;
            }
            else
            {
                dataGridFileList.Rows[0].Selected = true;
            }

            dataGridFileList.FirstDisplayedScrollingRowIndex = dataGridFileList.SelectedRows[0].Index;
            return true;
        }

        private void SearchComboBoxTextChanged(object sender, EventArgs e)
        {
            if (searchComboBox.Text.Length != 0 &&
                searchComboBox.Text.Length > FrmSettings.Settings.GetIntSetting(Settings.minTypeCharLimit))
            {
                FilterDataGrid(searchComboBox.Text);
            }
            else if (searchComboBox.Text.Length == 0)
            {
                ReloadFileList();
                FilterDataGrid("");
            }
        }

        private void dataGridView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            int index = dataGridFileList.HitTest(e.X, e.Y).RowIndex;
            if (index != -1)
            {
                ExecuteCurrentAction();
            }
        }

        private void SwitchToFile(StringBuilder path, bool isTab = true)
        {
            notepad.SwitchToFile(path.ToString(), isTab);
            HideWindow(!FrmSettings.Settings.GetBoolSetting(Settings.keepDlgOpen));
            string fileName = Path.GetFileName(notepad.GetCurrentFilePath());
            searchComboBox.Items.Remove(fileName);
            searchComboBox.Items.Insert(0, fileName);
        }

        private void HideWindow(bool disabled = true)
        {
            if (disabled)
            {
                Win32.SendMessage(PluginBase.nppData._nppHandle, (uint)NppMsg.NPPM_DMMHIDE, 0, Handle);
                Win32.SendMessage(PluginBase.nppData._nppHandle, (uint)NppMsg.NPPM_SETMENUITEMCHECK,
                    PluginBase._funcItems.Items[0]._cmdID, 0);
            }
        }

        private void ExecuteCurrentAction()
        {
            if (dataGridFileList.SelectedRows[0].Cells[2].Value.Equals(CMD))
            {
                NppMenuCmd cmd;
                NppMenuCmd.TryParse(dataGridFileList.SelectedRows[0].Cells[0].Value.ToString(), true, out cmd);
                Win32.SendMessage(PluginBase.nppData._nppHandle, (uint)NppMsg.NPPM_MENUCOMMAND, 0, cmd);
            }
            else
            {
                StringBuilder path = new StringBuilder(Win32.MAX_PATH);
                path.Append(dataGridFileList.SelectedRows[0].Cells[1].Value);
                SwitchToFile(path, dataGridFileList.SelectedRows[0].Cells[2].Value.Equals(TABS));
            }
        }

        private void dataGridFileList_Resize(object sender, EventArgs e)
        {
            bool minWidthThreshold = dataGridFileList.Width > FrmSettings.Settings.GetIntSetting(Settings.gridMinWidth);
            dataGridFileList.Columns[1].Visible = minWidthThreshold;
            dataGridFileList.Columns[2].Visible = minWidthThreshold;

            if (minWidthThreshold)
            {
                SetColumnsWidth();
            }
        }

        private void dataGridFileList_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.Value == null) return;
            if (searchComboBox.Text.Length == 0) return;

            if (e.RowIndex > -1 && e.ColumnIndex > -1)
            {
                String search = searchComboBox.Text;
                var filterList = search.Split(null);
                List<Rectangle> rectangleList = new List<Rectangle>();

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
                    new SolidBrush(
                        Color.FromArgb(FrmSettings.Settings.GetIntSetting(Settings.highlightColorBackground)));

                foreach (var recto in rectangleList)
                {
                    e.Graphics.FillRectangle(hlBrush, recto);
                }

                hlBrush.Dispose();
                e.PaintContent(e.CellBounds);
            }
        }

        private void dataGridFileList_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Escape)
            {
                HideWindow();
                editor.GrabFocus();
                e.Handled = true;
            }

            if (e.KeyData == Keys.Enter)
            {
                ExecuteCurrentAction();
                e.Handled = true;
            }
        }

        private void FrmNavigateAll_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Down:
                    e.Handled = NavigateGridDown((Control.ModifierKeys & Keys.Shift) == Keys.Shift);
                    break;

                case Keys.Up:
                    e.Handled = NavigateGridUp((Control.ModifierKeys & Keys.Shift) == Keys.Shift);
                    break;

                case Keys.Tab:
                {
                    Control next = GetNextControl((Control)sender, !e.Shift);
                    while ((next == null) || (!next.TabStop)) next = GetNextControl(next, !e.Shift);
                    next.Focus();
                    e.Handled = true;
                }
                    break;
                case Keys.Enter:
                    ExecuteCurrentAction();
                    e.Handled = true;
                    break;
                case Keys.Escape:
                {
                    HideWindow();
                    editor.GrabFocus();
                }
                    break;
            }
        }

        private void FrmNavigateAll_KeyPress(object sender, KeyPressEventArgs e)
        {
            searchComboBox.Focus();
            Char pressedKey = e.KeyChar;
            if (Char.IsLetter(pressedKey) || Char.IsSeparator(pressedKey) || Char.IsPunctuation(pressedKey))
            {
                if (searchComboBox.Text.Length == 0)
                {
                    searchComboBox.Text = e.KeyChar.ToString();
                    searchComboBox.Select(1, 1);
                }
                else
                {
                    searchComboBox.Text += e.KeyChar.ToString();
                    searchComboBox.Select(searchComboBox.Text.Length, 1);
                }

                e.Handled = true;
            }
            else
            {
                searchComboBox.Select(searchComboBox.Text.Length, 1);
            }
        }

        private void dataGridFileList_KeyPress(object sender, KeyPressEventArgs e)
        {
            searchComboBox.Focus();
            Char pressedKey = e.KeyChar;
            if (Char.IsLetter(pressedKey) || Char.IsSeparator(pressedKey) || Char.IsPunctuation(pressedKey))
            {
                if (searchComboBox.Text.Length == 0)
                {
                    searchComboBox.Text = e.KeyChar.ToString();
                    searchComboBox.Select(1, 1);
                }
                else
                {
                    searchComboBox.Text += e.KeyChar.ToString();
                    searchComboBox.Select(searchComboBox.Text.Length, 1);
                }

                e.Handled = true;
            }
            else
            {
                searchComboBox.Select(searchComboBox.Text.Length, 1);
            }
        }

        private void toolStripMenuItemClose_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow selectedRow in dataGridFileList.SelectedRows)
            {
                if (selectedRow.Cells[2].Value.Equals(TABS))
                {
                    notepad.SwitchToFile(selectedRow.Cells[1].Value.ToString(), true);
                    Win32.SendMessage(PluginBase.nppData._nppHandle, (uint)NppMsg.NPPM_MENUCOMMAND, 0, NppMenuCmd.IDM_FILE_CLOSE);
                }
            }
        }
    }
}