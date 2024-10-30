// NPP plugin platform for .Net v0.91.57 by Kasper B. Graversen etc.

using System;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using NavigateTo.Tests;
using Kbg.NppPluginNET.PluginInfrastructure;
using NavigateTo.Plugin.Namespace;
using NppPluginNET;

namespace Kbg.NppPluginNET
{
    class Main
    {
        static internal void CommandMenuInit()
        {
            NavigateTo.Plugin.Namespace.Main.CommandMenuInit();
        }

        static internal void PluginCleanUp()
        {
            if (NavigateTo.Plugin.Namespace.Main.frmNavigateTo != null)
            {
                NavigateTo.Plugin.Namespace.Main.frmNavigateTo.Close();
            }

            if (NavigateTo.Plugin.Namespace.Main.frmSettings != null)
            {
                NavigateTo.Plugin.Namespace.Main.frmSettings.Close();
            }

            NavigateTo.Plugin.Namespace.Main.PluginCleanUp();
        }

        static internal void SetToolBarIcon()
        {
            NavigateTo.Plugin.Namespace.Main.SetToolBarIcon();
        }

        public static void OnNotification(ScNotification notification)
        {
            switch (notification.Header.Code)
            {
                case (uint)NppMsg.NPPN_READY:
                    NavigateTo.Plugin.Namespace.Main.NavigateToDlg();
                    break;
                case (uint)NppMsg.NPPN_BEFORESHUTDOWN:
                    NavigateTo.Plugin.Namespace.Main.isShuttingDown = true;
                    break;
                case (uint)NppMsg.NPPN_FILEOPENED:
                case (uint)NppMsg.NPPN_FILECLOSED:
                case (uint)NppMsg.NPPN_FILERENAMED:
                case (uint)NppMsg.NPPN_FILELOADFAILED:
                case (uint)NppMsg.NPPN_FILEDELETED:
                case (uint)NppMsg.NPPN_DOCORDERCHANGED:
                case (uint)NppMsg.NPPN_BUFFERACTIVATED:
                    // if the user has multiple instances or views open,
                    // having a static unchanging editor object no longer works.
                    // this makes the editor track the current instance.
                    NavigateTo.Plugin.Namespace.Main.editor = new ScintillaGateway(PluginBase.GetCurrentScintilla());
                    var frmNavigateTo = NavigateTo.Plugin.Namespace.Main.frmNavigateTo;
                    if (frmNavigateTo == null || !frmNavigateTo.Visible
                        || NavigateTo.Plugin.Namespace.Main.isShuttingDown)
                        // DO NOT reload file list if:
                        // 1. Notepad++ is shutting down
                        // 2. NavigateTo form doesn't exist or is not visible
                        return;
                    // reload the file list whenever you open a file in a different directory
                    if (frmNavigateTo.currentDirectory == null ||
                        frmNavigateTo.currentDirectory != NavigateTo.Plugin.Namespace.Main.notepad.GetCurrentFileDirectory())
                        frmNavigateTo.shouldReloadFiles = true;
                    frmNavigateTo.ReloadFileList();
                    frmNavigateTo.FilterDataGrid("");
                    break;
                // the editor color scheme changed, so update form colors
                case (uint)NppMsg.NPPN_WORDSTYLESUPDATED:
                    NavigateTo.Plugin.Namespace.Main.RestyleEverything();
                    return;
            }
        }

        internal static string PluginName
        {
            get { return NavigateTo.Plugin.Namespace.Main.PluginName; }
        }
    }
}

namespace NavigateTo.Plugin.Namespace
{
    class Main
    {
        #region " Fields "

        public const string PluginName = "NavigateTo";

        static internal int idFormNavigateAll = 0;
        static internal int idFormSettings = 1;

        // toolbar icons
        static Bitmap tbBmp = Properties.Resources.star;
        static Bitmap tbBmp_tbTab = Properties.Resources.star_bmp;
        static Icon tbIco = Properties.Resources.star_black_ico;
        static Icon tbIcoDM = Properties.Resources.star_white_ico;
        static Icon tbIcon = null;

        public static IScintillaGateway editor = new ScintillaGateway(PluginBase.GetCurrentScintilla());
        public static INotepadPPGateway notepad = new NotepadPPGateway();
        public static FrmNavigateTo frmNavigateTo = null;
        public static FrmSettings frmSettings = new FrmSettings(editor, notepad);
        public static bool isShuttingDown = false;

        #endregion

        #region " Startup/CleanUp "

        static internal void CommandMenuInit()
        {
            PluginBase.SetCommand(idFormNavigateAll, "&NavigateTo", NavigateToDlg,
                new ShortcutKey(true, false, false, Keys.Oemcomma));
            PluginBase.SetCommand(idFormSettings, "&Settings", SettingsDlg);
            PluginBase.SetCommand(2, "---", null);
            PluginBase.SetCommand(3, "A&bout", ShowAboutForm);
            PluginBase.SetCommand(4, "Check &updates", OpenReleasePage);
            PluginBase.SetCommand(5, "Run &tests", TestRunner.RunAll);
        }

        static internal void SetToolBarIcon()
        {
            // create struct
            toolbarIcons tbIcons = new toolbarIcons();

            // add bmp icon
            tbIcons.hToolbarBmp = tbBmp.GetHbitmap();
            tbIcons.hToolbarIcon = tbIco.Handle; // icon with black lines
            tbIcons.hToolbarIconDarkMode = tbIcoDM.Handle; // icon with light grey lines

            // convert to c++ pointer
            IntPtr pTbIcons = Marshal.AllocHGlobal(Marshal.SizeOf(tbIcons));
            Marshal.StructureToPtr(tbIcons, pTbIcons, false);

            // call Notepad++ api
            Win32.SendMessage(PluginBase.nppData._nppHandle, (uint)NppMsg.NPPM_ADDTOOLBARICON_FORDARKMODE,
                PluginBase._funcItems.Items[idFormNavigateAll]._cmdID, pTbIcons);

            // release pointer
            Marshal.FreeHGlobal(pTbIcons);
        }

        static internal void PluginCleanUp()
        {
            //cleanup
        }

        #endregion

        #region " Menu functions "

        public static void NavigateToDlg()
        {
            if (frmNavigateTo == null)
            {
                frmNavigateTo = new FrmNavigateTo(editor, notepad);
                using (Bitmap newBmp = new Bitmap(16, 16))
                {
                    Graphics g = Graphics.FromImage(newBmp);
                    ColorMap[] colorMap = new ColorMap[1];
                    colorMap[0] = new ColorMap();
                    colorMap[0].OldColor = Color.Fuchsia;
                    colorMap[0].NewColor = Color.FromKnownColor(KnownColor.ButtonFace);
                    ImageAttributes attr = new ImageAttributes();
                    attr.SetRemapTable(colorMap);
                    g.DrawImage(tbBmp_tbTab, new Rectangle(0, 0, 16, 16), 0, 0, 16, 16, GraphicsUnit.Pixel, attr);
                    tbIcon = Icon.FromHandle(newBmp.GetHicon());
                }

                NppTbData _nppTbData = new NppTbData();
                _nppTbData.hClient = frmNavigateTo.Handle;
                _nppTbData.pszName = PluginName;
                _nppTbData.dlgID = idFormNavigateAll;
                // define the default docking behaviour
                _nppTbData.uMask = NppTbMsg.CONT_LEFT | NppTbMsg.DWS_ICONTAB | NppTbMsg.DWS_ICONBAR;
                _nppTbData.hIconTab = (uint)tbIcon.Handle;
                _nppTbData.pszModuleName = PluginName;
                IntPtr _ptrNppTbData = Marshal.AllocHGlobal(Marshal.SizeOf(_nppTbData));
                Marshal.StructureToPtr(_nppTbData, _ptrNppTbData, false);
                Win32.SendMessage(PluginBase.nppData._nppHandle, (uint)NppMsg.NPPM_DMMREGASDCKDLG, 0, _ptrNppTbData);

                if (FrmSettings.Settings.GetBoolSetting(Settings.keepDlgOpen))
                {
                    Win32.SendMessage(PluginBase.nppData._nppHandle, (uint)NppMsg.NPPM_DMMSHOW, 0,
                        frmNavigateTo.Handle);
                    Win32.SendMessage(PluginBase.nppData._nppHandle, (uint)NppMsg.NPPM_SETMENUITEMCHECK,
                        PluginBase._funcItems.Items[idFormNavigateAll]._cmdID, 1);
                    frmNavigateTo.GrabFocus();
                }
                else
                {
                    Win32.SendMessage(PluginBase.nppData._nppHandle, (uint)NppMsg.NPPM_DMMHIDE, 0,
                        frmNavigateTo.Handle);
                    Win32.SendMessage(PluginBase.nppData._nppHandle, (uint)NppMsg.NPPM_SETMENUITEMCHECK,
                        PluginBase._funcItems.Items[idFormNavigateAll]._cmdID, 0);
                    editor.GrabFocus();
                }
            }
            else
            {
                if (!frmNavigateTo.Visible)
                {
                    Win32.SendMessage(PluginBase.nppData._nppHandle, (uint)NppMsg.NPPM_DMMSHOW, 0,
                        frmNavigateTo.Handle);
                    Win32.SendMessage(PluginBase.nppData._nppHandle, (uint)NppMsg.NPPM_SETMENUITEMCHECK,
                        PluginBase._funcItems.Items[idFormNavigateAll]._cmdID, 1);
                    frmNavigateTo.GrabFocus();
                }
                else
                {
                    Win32.SendMessage(PluginBase.nppData._nppHandle, (uint)NppMsg.NPPM_DMMHIDE, 0,
                        frmNavigateTo.Handle);
                    Win32.SendMessage(PluginBase.nppData._nppHandle, (uint)NppMsg.NPPM_SETMENUITEMCHECK,
                        PluginBase._funcItems.Items[idFormNavigateAll]._cmdID, 0);
                    editor.GrabFocus();
                }
            }
        }

        static void SettingsDlg()
        {
            if (frmSettings.Visible)
            {
                frmSettings.Hide();
            }
            else
            {
                frmSettings.Show();
            }
        }

        static void ShowAboutForm()
        {
            AboutForm aboutForm = new AboutForm();
            aboutForm.ShowDialog();
            aboutForm.Focus();
        }

        static void OpenReleasePage()
        {
            System.Diagnostics.Process.Start("https://github.com/young-developer/nppNavigateTo/releases");
        }

        #endregion

        /// <summary>
        /// Apply the appropriate styling
        /// (either generic control styling or Notepad++ styling as the case may be)
        /// to all forms.
        /// </summary>
        public static void RestyleEverything()
        {
            bool isDark = notepad.IsDarkModeEnabled();
            if (frmNavigateTo != null && !frmNavigateTo.IsDisposed)
                FormStyle.ApplyStyle(frmNavigateTo, true, isDark);
            if (frmSettings != null && !frmSettings.IsDisposed)
                FormStyle.ApplyStyle(frmSettings, true, isDark);
        }
    }
}