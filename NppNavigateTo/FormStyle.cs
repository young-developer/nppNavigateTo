using Kbg.NppPluginNET.PluginInfrastructure;
using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace NavigateTo.Plugin.Namespace
{
    public class FormStyle
    {
        public static Color SlightlyDarkControl = Color.FromArgb(
            3 * SystemColors.Control.R / 4 + SystemColors.ControlDark.R / 4,
            3 * SystemColors.Control.G / 4 + SystemColors.ControlDark.G / 4,
            3 * SystemColors.Control.B / 4 + SystemColors.ControlDark.B / 4
        );


        /// <summary>
        /// Changes the background and text color of the form
        /// and any child forms to match the editor window.<br></br>
        /// Fires when the form is first opened
        /// and also whenever the style is changed.<br></br>
        /// Heavily based on CsvQuery (https://github.com/jokedst/CsvQuery)
        /// </summary>
        public static void ApplyStyle(Control ctrl, bool use_npp_style, bool isDark = false)
        {
            if (ctrl == null || ctrl.IsDisposed) return;
            int[] version = Main.notepad.GetNppVersion();
            if (version[0] < 8)
                use_npp_style = false; // trying to follow editor style looks weird for Notepad++ 7.3.3
            if (ctrl is Form form)
            {
                foreach (Form childForm in form.OwnedForms)
                {
                    ApplyStyle(childForm, use_npp_style, isDark);
                }
            }
            Color backColor = Main.notepad.GetDefaultBackgroundColor();
            Color foreColor = Main.notepad.GetDefaultForegroundColor();
            Color InBetween = Color.FromArgb(
                foreColor.R / 4 + 3 * backColor.R / 4,
                foreColor.G / 4 + 3 * backColor.G / 4,
                foreColor.B / 4 + 3 * backColor.B / 4
            );
            IntPtr themePtr = Main.notepad.GetDarkModeColors();
            if (isDark && themePtr != IntPtr.Zero)
            {
                var theme = (DarkModeColors)Marshal.PtrToStructure(themePtr, typeof(DarkModeColors));
                ctrl.BackColor = NppDarkMode.BGRToColor(theme.Background);
                ctrl.ForeColor = NppDarkMode.BGRToColor(theme.Text);
                foreach (Control child in ctrl.Controls)
                {
                    // this doesn't actually make disabled controls have different colors
                    // windows forms don't make it easy for the user to choose the
                    // color of a disabled control. See https://stackoverflow.com/questions/136129/windows-forms-how-do-you-change-the-font-color-for-a-disabled-label
                    var textTheme = child.Enabled ? theme.Text : theme.DisabledText;
                    if (child is GroupBox)
                        ApplyStyle(child, use_npp_style, isDark);
                    else if (child is Button btn)
                    {
                        btn.BackColor = NppDarkMode.BGRToColor(theme.SofterBackground);
                        btn.ForeColor = NppDarkMode.BGRToColor(textTheme);
                    }
                    else if (child is LinkLabel llbl)
                    {
                        llbl.BackColor = NppDarkMode.BGRToColor(theme.PureBackground);
                        llbl.ForeColor = NppDarkMode.BGRToColor(textTheme);
                        llbl.LinkColor = NppDarkMode.BGRToColor(theme.LinkText);
                        llbl.ActiveLinkColor = NppDarkMode.BGRToColor(textTheme);
                        llbl.VisitedLinkColor = NppDarkMode.BGRToColor(theme.DarkerText);
                    }
                    // other common text-based controls
                    else if (child is TextBox
                        || child is Label
                        || child is ListBox
                        || child is ComboBox)
                    {
                        child.BackColor = NppDarkMode.BGRToColor(theme.PureBackground);
                        child.ForeColor = NppDarkMode.BGRToColor(textTheme);
                    }
                    else if (child is TreeView tv)
                    {
                        tv.BackColor = NppDarkMode.BGRToColor(theme.HotBackground);
                        tv.ForeColor = NppDarkMode.BGRToColor(textTheme);
                    }
                    else if (child is DataGridView dgv)
                    {
                        // for reasons that I cannot fathom, everything EXCEPT the background gets correctly colored
                        // when you first open the form in dark mode.
                        // The background is untouched by the current style until you change to another style and then back,
                        // at which point the background remembers what color it's supposed to be. Very odd.
                        dgv.BackgroundColor = InBetween;
                        dgv.ForeColor = NppDarkMode.BGRToColor(textTheme);
                        dgv.GridColor = NppDarkMode.BGRToColor(theme.HotBackground);
                        dgv.RowHeadersDefaultCellStyle.ForeColor = NppDarkMode.BGRToColor(textTheme);
                        dgv.RowHeadersDefaultCellStyle.BackColor = NppDarkMode.BGRToColor(theme.PureBackground);
                        dgv.RowsDefaultCellStyle.ForeColor = NppDarkMode.BGRToColor(textTheme);
                        dgv.RowsDefaultCellStyle.BackColor = NppDarkMode.BGRToColor(theme.PureBackground);
                    }
                    else
                    {
                        // other controls I haven't thought of yet
                        child.BackColor = NppDarkMode.BGRToColor(theme.SofterBackground);
                        child.ForeColor = NppDarkMode.BGRToColor(textTheme);
                    }
                }
                Marshal.FreeHGlobal(themePtr);
                return;
            }
            else if (!use_npp_style || (
                backColor.R > 240 &&
                backColor.G > 240 &&
                backColor.B > 240))
            {
                // if the background is basically white,
                // use the system defaults because they
                // look best on a white or nearly white background
                ctrl.BackColor = SystemColors.Control;
                ctrl.ForeColor = SystemColors.ControlText;
                foreach (Control child in ctrl.Controls)
                {
                    if (child is GroupBox)
                        ApplyStyle(child, use_npp_style, isDark);
                    // controls containing text
                    else if (child is TextBox || child is ListBox || child is ComboBox || child is TreeView)
                    {
                        child.BackColor = SystemColors.Window; // white background
                        child.ForeColor = SystemColors.WindowText;
                    }
                    else if (child is LinkLabel llbl)
                    {
                        llbl.LinkColor = Color.Blue;
                        llbl.ActiveLinkColor = Color.Red;
                        llbl.VisitedLinkColor = Color.Purple;
                    }
                    else if (child is DataGridView dgv)
                    {
                        dgv.BackgroundColor = SystemColors.ControlDark;
                        dgv.ForeColor = SystemColors.ControlText;
                        dgv.GridColor = SystemColors.ControlDarkDark;
                        dgv.RowHeadersDefaultCellStyle.ForeColor = SystemColors.WindowText;
                        dgv.RowHeadersDefaultCellStyle.BackColor = SystemColors.Window;
                        dgv.RowsDefaultCellStyle.ForeColor = SystemColors.ControlText;
                        dgv.RowsDefaultCellStyle.BackColor = SystemColors.Window;
                    }
                    else
                    {
                        // buttons should be a bit darker but everything else is the same color as the background
                        child.BackColor = (child is Button) ? SlightlyDarkControl : SystemColors.Control;
                        child.ForeColor = SystemColors.ControlText;
                    }
                }
                return;
            }
            // use NPP styling for non-dark-mode
            ctrl.BackColor = backColor;
            foreach (Control child in ctrl.Controls)
            {
                child.BackColor = backColor;
                child.ForeColor = foreColor;
                if (child is LinkLabel llbl)
                {
                    llbl.LinkColor = foreColor;
                    llbl.ActiveLinkColor = foreColor;
                    llbl.VisitedLinkColor = foreColor;
                }
                else if (child is DataGridView dgv)
                {
                    dgv.BackgroundColor = InBetween;
                    dgv.ForeColor = foreColor;
                    dgv.GridColor = foreColor;
                    dgv.RowHeadersDefaultCellStyle.ForeColor = foreColor;
                    dgv.RowHeadersDefaultCellStyle.BackColor = backColor;
                    dgv.RowsDefaultCellStyle.ForeColor = foreColor;
                    dgv.RowsDefaultCellStyle.BackColor = backColor;
                }
            }
        }
    }
}
