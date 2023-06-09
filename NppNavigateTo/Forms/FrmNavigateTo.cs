using System;
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
    public enum DirectorySearchLevel
    {
        None,
        TopDirOnly,
        TopDirOnlyAndDontAsk,
        RecurseSubdirs,
        RecurseSubdirsAndDontAsk
    }

    partial class FrmNavigateTo : Form
    {
        private readonly IScintillaGateway editor;
        private readonly INotepadPPGateway notepad;
        private const string TABS = "Tabs";
        private const string CMD = "Cmd";
        private const string FOLDER_TOP = "Top Directory";
        private const string FOLDER_SUB = "Sub Directories";

        public List<FileModel> FileList { get; set; }

        public List<FileModel> FilteredFileList { get; set; }

        public List<string> SelectedFiles { get; set; }

        public string currentDirectory { get; set; }

        public string[] filesInCurrentDirectory { get; set; }

        public bool isShuttingDown { get; set; }

        public bool IsFuzzyResult { get; set; }

        public bool shouldReloadFiles { get; set; }

        private Dictionary<string, long> lastReloadTimes { get; set; }

        private Dictionary<string, DirectorySearchLevel> searchLevelOverrides { get; set; }

        /// <summary>
        /// if there are a lot of files in the current directory, displaying rows for them
        /// could take a VERY LONG TIME, so warn the user.
        /// </summary>
        public int minDirTreeSizeToWarn { get; set; } = 5000;

        public FrmNavigateTo(IScintillaGateway editor, INotepadPPGateway notepad)
        {
            this.editor = editor;
            this.notepad = notepad;
            this.SelectedFiles = new List<string>();
            currentDirectory = null;
            filesInCurrentDirectory = null;
            shouldReloadFiles = true;
            IsFuzzyResult = false;
            lastReloadTimes = new Dictionary<string, long>();
            searchLevelOverrides = new Dictionary<string, DirectorySearchLevel>();
            InitializeComponent();
            ReloadFileList();
            this.notepad.ReloadMenuItems();
            FormStyle.ApplyStyle(this, true, notepad.IsDarkModeEnabled());
        }

        private static bool MatchesAllWordsInFilter(string s, string[] words)
        {
            return words.All(word => 
                s.IndexOf(word, StringComparison.CurrentCultureIgnoreCase) >= 0
            );
        }

        string[] SearchCurrentDirectory(DirectorySearchLevel searchLevel, long nextTimeToRefresh, string currentDirectory)
        {
            if (searchLevel < DirectorySearchLevel.TopDirOnly)
                return new string[0];
            string[] currentFiles = null;
            if (filesInCurrentDirectory == null ||
                DateTime.UtcNow.Ticks >= nextTimeToRefresh)
            {
                // only load the files in current directory as needed
                var searchOption = searchLevel >= DirectorySearchLevel.RecurseSubdirs
                    ? SearchOption.AllDirectories
                    : SearchOption.TopDirectoryOnly;
                int fileCount = 0;
                string filesInMessage = searchLevel == DirectorySearchLevel.RecurseSubdirs
                    ? "directory tree"
                    : "top directory";
                DirectorySearchLevel newSearchLevel = searchLevel;
                currentFiles = Directory.EnumerateFiles(currentDirectory, "*.*", searchOption)
                    .CheckWhen(
                        x => ++fileCount >= minDirTreeSizeToWarn,
                        () =>
                        {
                            if (searchLevel == DirectorySearchLevel.RecurseSubdirsAndDontAsk ||
                                searchLevel == DirectorySearchLevel.TopDirOnlyAndDontAsk)
                                return false;
                            bool stopIteration = MessageBox.Show($"The {filesInMessage} under this file contains at least {fileCount} files " +
                                $"so it could cause severe latency when searching. Do you still want to search the {filesInMessage}?",
                                $"Very large {filesInMessage}",
                                MessageBoxButtons.YesNo, MessageBoxIcon.Warning
                            ) == DialogResult.No;
                            if (stopIteration)
                            {
                                // drop down to lower search level
                                if (searchLevel == DirectorySearchLevel.RecurseSubdirs)
                                    newSearchLevel = DirectorySearchLevel.TopDirOnly;
                                else
                                    newSearchLevel = DirectorySearchLevel.None;
                            }
                            else
                            {
                                // now that we've checked what the user wants
                                // and the user has confirmed their preferences, we don't need to ask again.
                                if (searchLevel == DirectorySearchLevel.RecurseSubdirs)
                                    newSearchLevel = DirectorySearchLevel.RecurseSubdirsAndDontAsk;
                                else
                                    newSearchLevel = DirectorySearchLevel.TopDirOnlyAndDontAsk;
                            }
                            searchLevelOverrides[currentDirectory] = newSearchLevel;
                            return stopIteration;
                        }
                    )
                    .ToArray();
                shouldReloadFiles = false;
                lastReloadTimes[currentDirectory] = DateTime.UtcNow.Ticks;
                if (newSearchLevel < searchLevel)
                {
                    // the user didn't want to search as many files as their previous option implied
                    if (newSearchLevel == DirectorySearchLevel.TopDirOnly)
                    {
                        return SearchCurrentDirectory(newSearchLevel, nextTimeToRefresh, currentDirectory);
                    }
                    return new string[0];
                }
                return currentFiles;
            }
            // no refresh, just use old files
            return filesInCurrentDirectory ?? new string[0];
        }

        void FilterCurrentDirectory(Func<string, bool> filterFunc)
        {
            string currentDirectory = notepad.GetCurrentFileDirectory();
            if (string.IsNullOrWhiteSpace(currentDirectory))
                return;
            bool userWantsSearchSubdirs = FrmSettings.Settings.GetBoolSetting(Settings.searchInSubDirs);
            DirectorySearchLevel searchLevel = userWantsSearchSubdirs
                ? DirectorySearchLevel.RecurseSubdirs
                : DirectorySearchLevel.TopDirOnly;
            long nextTimeToRefresh = 0;
            if (!shouldReloadFiles &&
                lastReloadTimes.TryGetValue(currentDirectory, out long nextRefresh))
                nextTimeToRefresh = nextRefresh;
            DirectorySearchLevel searchLevelOverride = searchLevel;
            if (searchLevelOverrides.TryGetValue(currentDirectory, out var searchOverride))
                searchLevelOverride = searchOverride;
            if (searchLevelOverride != searchLevel)
            {
                // if the user had previously chosen to do less searching in this directory
                // than the global settings, follow that local override
                if (searchLevel == DirectorySearchLevel.RecurseSubdirs
                    || (searchLevelOverride <= DirectorySearchLevel.TopDirOnlyAndDontAsk))
                {
                    searchLevel = searchLevelOverride;
                }
            }
            filesInCurrentDirectory = SearchCurrentDirectory(searchLevel, nextTimeToRefresh, currentDirectory);
            foreach (var filePath in filesInCurrentDirectory
                        .AsParallel().Where(filterFunc)
                        .ToList())
            {
                if (!FilteredFileList.Exists(file => file.FilePath.Equals(filePath)))
                {
                    FilteredFileList.Add(new FileModel(Path.GetFileName(filePath), filePath, -1, -1,
                        searchLevel >= DirectorySearchLevel.RecurseSubdirs ? FOLDER_SUB : FOLDER_TOP, 0));
                }
            }
        }

        public void FilterDataGrid(string filter)
        {
            if (!string.IsNullOrEmpty(searchComboBox.Text)) filter = searchComboBox.Text;

            if (!string.IsNullOrWhiteSpace(filter))
            {
                filter = filter.Trim();
            }

            IsFuzzyResult = false;

            foreach (DataGridViewRow selectedRow in dataGridFileList.SelectedRows)
            {
                SelectedFiles.Add(selectedRow.Cells[1].Value.ToString());
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
                    newRow.Cells[2].Value = file.Source;

                    dataGridFileList.Rows.Add(newRow);
                });
            }
            else
            {
                var words = filter.Split(' ').Where(x => !string.IsNullOrWhiteSpace(x)).ToHashSet().ToArray();
                Func<string, bool> filterFunc;
                if (words.Length == 0)
                    filterFunc = x => true;
                else
                    filterFunc = x => MatchesAllWordsInFilter(x, words);
                searchInTabs(filter);

                if (FrmSettings.Settings.GetBoolSetting(Settings.searchInCurrentFolder))
                {
                    FilterCurrentDirectory(filterFunc);
                }

                if (FrmSettings.Settings.GetBoolSetting(Settings.searchMenuCommands))
                {
                    FilterMenuCommands(filterFunc);
                }

                FilteredFileList.ForEach(file =>
                {
                    DataGridViewRow newRow = new DataGridViewRow();

                    newRow.CreateCells(dataGridFileList);

                    newRow.Cells[0].Value = file.FileName;
                    newRow.Cells[1].Value = file.FilePath;
                    newRow.Cells[2].Value = file.Source;

                    dataGridFileList.Rows.Add(newRow);
                });
            }

            //auto sort
            if (FrmSettings.Settings.GetIntSetting(Settings.sortAfterFilterBy) != -1)
            {
                dataGridFileList.Sort(
                    dataGridFileList.Columns[FrmSettings.Settings.GetIntSetting(Settings.sortAfterFilterBy)],
                    FrmSettings.Settings.GetIntSetting(Settings.sortOrderAfterFilterBy) == 0
                        ? ListSortDirection.Ascending
                        : ListSortDirection.Descending);
            }

            //restore selection
            if (SelectedFiles.Count != 0)
            {
                foreach (DataGridViewRow row in dataGridFileList.Rows)
                {
                    row.Selected = SelectedFiles.Contains(row.Cells[1].Value);
                }
            }

            if (FilteredFileList != null && FilteredFileList.Count != 0 && dataGridFileList.SelectedRows.Count == 0)
            {
                SelectFirstRow();
            }

            SelectedFiles.Clear();

            dataGridFileList.TopLeftHeaderCell.Value = dataGridFileList.Rows.Count.ToString();
        }

        private void searchInTabs(string filter)
        {
            //Normal index of search
            var words = filter.Split(' ').Where(x => !string.IsNullOrWhiteSpace(x)).ToHashSet().ToArray();
            Func<string, bool> filterFunc = x => MatchesAllWordsInFilter(x, words);
            if (FrmSettings.Settings.GetBoolSetting(Settings.preferFilenameResults))
            {
                FilteredFileList = FileList.AsParallel()
                    .Where(e => filterFunc(e.FileName))
                    .ToList();

                FileList.AsParallel()
                    .Where(e => MatchesAllWordsInFilter(e.FilePath, words))
                    .ToList().ForEach(filePath =>
                        {
                            if (!FilteredFileList.Exists(file => file.FilePath.Equals(filePath.FilePath)))
                            {
                                FilteredFileList.Add(filePath);
                            }
                        }
                    );
            }
            else
            {
                FilteredFileList = FileList.AsParallel()
                    .Where(e => filterFunc(e.FileName) || filterFunc(e.FilePath))
                    .ToList();
            }

            //run fuzzy search if there are no results from normal search and it is enabled
            if (FilteredFileList != null && FilteredFileList.Count == 0 &&
                FrmSettings.Settings.GetBoolSetting(Settings.fuzzySearch))
            {
                if (FrmSettings.Settings.GetBoolSetting(Settings.preferFilenameResults))
                {
                    FilteredFileList = SearchUtils.FuzzySearchFileName(filter, FileList,
                        FrmSettings.Settings.GetIntSetting(Settings.fuzzynessTolerance));

                    SearchUtils.FuzzySearchFilePath(filter, FileList,
                        FrmSettings.Settings.GetIntSetting(Settings.fuzzynessTolerance)).ForEach(filePath =>
                        {
                            if (!FilteredFileList.Exists(file => file.FilePath.Equals(filePath.FilePath)))
                            {
                                FilteredFileList.Add(filePath);
                            }
                        }
                    );
                }
                else
                {
                    FilteredFileList = SearchUtils.FuzzySearch(filter, FileList,
                        FrmSettings.Settings.GetIntSetting(Settings.fuzzynessTolerance));
                }

                IsFuzzyResult = true;
            }
        }

        private void FilterMenuCommands(Func<string, bool> filterFunc)
        {
            foreach (var nppMenuCmd in notepad.MainMenuItems.AsParallel()
                .Where(e => filterFunc(e.Value))
                .ToList())
            {
                FilteredFileList.Add(new FileModel(
                    nppMenuCmd.Key.ToString(),
                    nppMenuCmd.Value, nppMenuCmd.GetHashCode(), (uint)nppMenuCmd.Key,
                    CMD, 0));
            }
        }

        public void ReloadFileList()
        {
            if (FileList == null) FileList = new List<FileModel>();
            FileList.Clear();
            int firstViewCount =
                (int)Win32.SendMessage(PluginBase.nppData._nppHandle, (uint)NppMsg.NPPM_GETNBOPENFILES, 0, 1);
            var viewCount = firstViewCount;

            int secondViewCount =
                (int)Win32.SendMessage(PluginBase.nppData._nppHandle, (uint)NppMsg.NPPM_GETNBOPENFILES, 0, 2);
            viewCount += secondViewCount;

            if (viewCount > 0)
            {
                using (ClikeStringArray cStrArray =
                       new ClikeStringArray(viewCount, Win32.MAX_PATH))
                {
                    if (Win32.SendMessage(PluginBase.nppData._nppHandle, (uint)NppMsg.NPPM_GETOPENFILENAMES,
                            cStrArray.NativePointer, viewCount) != IntPtr.Zero)
                    {
                        for (int index = 0; index < viewCount; index++)
                        {
                            int view = (index >= firstViewCount) ? 1 : 0;
                            int pos = (index < firstViewCount) ? index : index - (firstViewCount);
                            IntPtr bufferId = Win32.SendMessage(PluginBase.nppData._nppHandle,
                                (uint)NppMsg.NPPM_GETBUFFERIDFROMPOS, pos, view);

                            bool isPhantomFile = pos == 0 
                                                   && ((secondViewCount == 1 && view == 1) || (firstViewCount == 1 && view==0)) 
                                                   && cStrArray.ManagedStringsUnicode[index].Contains("new ");

                            if (bufferId != IntPtr.Zero && !isPhantomFile)
                            {
                                FileList.Add(new FileModel(Path.GetFileName(cStrArray.ManagedStringsUnicode[index]),
                                    cStrArray.ManagedStringsUnicode[index], pos, bufferId.ToInt64(), TABS, view));
                            }
                        }
                    }
                }
            }
        }

        private void RefreshDataGridStyles()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 =
                new System.Windows.Forms.DataGridViewCellStyle();
            dataGridViewCellStyle1.ForeColor = FrmSettings.Settings.GetColorSetting(Settings.gridTextColor);
            dataGridViewCellStyle1.BackColor = FrmSettings.Settings.GetColorSetting(Settings.rowBackgroundColor);
            dataGridViewCellStyle1.SelectionBackColor =
                FrmSettings.Settings.GetColorSetting(Settings.gridSelectedRowBackground);
            dataGridViewCellStyle1.SelectionForeColor =
                FrmSettings.Settings.GetColorSetting(Settings.gridSelectedRowForeground);
            dataGridFileList.DefaultCellStyle = dataGridViewCellStyle1;
            dataGridFileList.BackgroundColor = FrmSettings.Settings.GetColorSetting(Settings.gridBackgroundColor);
            dataGridFileList.BackColor = FrmSettings.Settings.GetColorSetting(Settings.rowBackgroundColor);
        }

        void frmNavigateAll_VisibleChanged(object sender, EventArgs e)
        {
            if (Visible)
            {
                SetColumnsWidth();
                RefreshDataGridStyles();
            }
            else
            {
                if (FrmSettings.Settings.GetBoolSetting(Settings.clearOnClose))
                {
                    searchComboBox.Text = "";
                }

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
                        FileModel file = FileList.Find(f =>
                            f.FilePath.Equals(dataGridFileList.Rows[currentMouseOverRow].Cells[1].Value));
                        Win32.SendMessage(PluginBase.nppData._nppHandle, (uint)NppMsg.NPPM_TRIGGERTABBARCONTEXTMENU,
                            file.View,
                            (int)file.FileIndex);
                    }
                }
                else if (dataGridFileList.SelectedRows.Count > 1)
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
                    Control next = GetNextControl((Control)sender, !e.Shift);
                    while ((next == null) || (!next.TabStop)) next = GetNextControl(next, !e.Shift);
                    next.Focus();
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                    break;

                case Keys.Enter:
                    ExecuteCurrentAction();
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                    break;

                case Keys.Escape:
                    HideWindow();
                    editor.GrabFocus();
                    e.Handled = true;
                    e.SuppressKeyPress = true;
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
                SelectFirstRow();
                return true;
            }

            int lastSelectedIndex = dataGridFileList.SelectedRows[dataGridFileList.SelectedRows.Count - 1].Index;
            if (dataGridFileList.MultiSelect)
            {
                dataGridFileList.MultiSelect = false;
                dataGridFileList.MultiSelect = true;
            }


            if (dataGridFileList.Rows[lastSelectedIndex].Index > 0)
            {
                dataGridFileList.Rows[lastSelectedIndex - 1].Selected = true;
                dataGridFileList.CurrentCell = dataGridFileList.Rows[lastSelectedIndex - 1].Cells[0];
            }
            else
            {
                dataGridFileList.Rows[dataGridFileList.Rows.Count - 1].Selected = true;
                dataGridFileList.CurrentCell = dataGridFileList.Rows[dataGridFileList.Rows.Count - 1].Cells[0];
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
                SelectFirstRow();
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
                dataGridFileList.CurrentCell = dataGridFileList.Rows[lastSelectedIndex + 1].Cells[0];
            }
            else
            {
                SelectFirstRow();
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

            if (FrmSettings.Settings.GetBoolSetting(Settings.selectFirstRowOnFilter))
            {
                SelectFirstRow();
            }
        }

        private void SelectFirstRow()
        {
            if (dataGridFileList.Rows.Count > 0)
            {
                dataGridFileList.MultiSelect = false;
                dataGridFileList.MultiSelect = true;
                dataGridFileList.Rows[0].Selected = true;
                dataGridFileList.CurrentCell = dataGridFileList.Rows[0].Cells[0];
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

        private void SwitchToFile(string path, bool isTab = true)
        {
            notepad.SwitchToFile(path, isTab);
            HideWindow(!FrmSettings.Settings.GetBoolSetting(Settings.keepDlgOpen));
            string fileName = Path.GetFileName(notepad.GetCurrentFilePath());
            searchComboBox.Items.Remove(fileName);
            searchComboBox.Items.Insert(0, fileName);
        }

        private void HideWindow(bool disabled = true)
        {
            if (disabled)
            {
                if (FrmSettings.Settings.GetBoolSetting(Settings.clearOnClose))
                {
                    searchComboBox.Text = "";
                }

                Win32.SendMessage(PluginBase.nppData._nppHandle, (uint)NppMsg.NPPM_DMMHIDE, 0, Handle);
                Win32.SendMessage(PluginBase.nppData._nppHandle, (uint)NppMsg.NPPM_SETMENUITEMCHECK,
                    PluginBase._funcItems.Items[0]._cmdID, 0);
            }
        }

        private void ExecuteCurrentAction()
        {
            if (dataGridFileList.Rows.Count == 0 || dataGridFileList.SelectedRows.Count == 0)
            {
                string fname = searchComboBox.Text.Trim('"');
                if (File.Exists(fname))
                {
                    notepad.OpenFile(fname);
                }
                return;
            }

            if (dataGridFileList.SelectedRows[0].Cells[2].Value.Equals(CMD))
            {
                NppMenuCmd cmd;
                NppMenuCmd.TryParse(dataGridFileList.SelectedRows[0].Cells[0].Value.ToString(), true, out cmd);
                Win32.SendMessage(PluginBase.nppData._nppHandle, (uint)NppMsg.NPPM_MENUCOMMAND, 0, cmd);
            }
            else
            {
                SwitchToFile(dataGridFileList.SelectedRows[0].Cells[1].Value.ToString(),
                    dataGridFileList.SelectedRows[0].Cells[2].Value.Equals(TABS));
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
                String search = searchComboBox.Text.Trim();
                var filterList = IsFuzzyResult ? search.ToCharArray().Select(c => c.ToString()) : search.Split(' ');
                filterList = filterList.Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();
                List<Rectangle> rectangleList = new List<Rectangle>();

                if (!String.IsNullOrWhiteSpace(search))
                {
                    foreach (var word in filterList)
                    {
                        String gridCellValue = e.FormattedValue.ToString();
                        var startIndexes = gridCellValue.AllIndexesOf(word.Trim(), true);

                        foreach (var startIndexInCellValue in startIndexes)
                        {
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
                e.SuppressKeyPress = true;
            }

            if (e.KeyData == Keys.Enter)
            {
                ExecuteCurrentAction();
                e.Handled = true;
                e.SuppressKeyPress = true;
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
                    Control next = GetNextControl((Control)sender, !e.Shift);
                    while ((next == null) || (!next.TabStop)) next = GetNextControl(next, !e.Shift);
                    next.Focus();
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                    break;

                case Keys.Enter:
                    ExecuteCurrentAction();
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                    break;

                case Keys.Escape:
                    e.SuppressKeyPress = true;
                    HideWindow();
                    editor.GrabFocus();
                    break;
            }
        }

        private void FrmNavigateAll_KeyPress(object sender, KeyPressEventArgs e)
        {
            searchComboBox.Focus();
            Char pressedKey = e.KeyChar;
            if (Char.IsLetter(pressedKey) || Char.IsSeparator(pressedKey) || Char.IsPunctuation(pressedKey) || Char.IsNumber(pressedKey))
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
                    Win32.SendMessage(PluginBase.nppData._nppHandle, (uint)NppMsg.NPPM_MENUCOMMAND, 0,
                        NppMenuCmd.IDM_FILE_CLOSE);
                }
            }
        }

        private void dataGridFileList_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            object o = dataGridFileList.Rows[e.RowIndex].HeaderCell.Value;

            e.Graphics.DrawString(
                o != null ? o.ToString() : "",
                dataGridFileList.Font,
                Brushes.Black,
                new PointF((float)e.RowBounds.Left + 2, (float)e.RowBounds.Top + 4));
        }
    }
}