using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using NppPluginNET;

namespace NavigateTo.Plugin.Namespace
{
    public partial class AboutForm : Form
    {
        public AboutForm()
        {
            InitializeComponent();
            Title.Text = $"NavigateTo v{MiscUtils.AssemblyVersionString()}";
            FormStyle.ApplyStyle(this, true, Main.notepad.IsDarkModeEnabled());
        }

        /// <summary>
        /// open GitHub repo with the web browser
        /// </summary>
        private void GitHubLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            string help_url = "https://github.com/young-developer/nppNavigateTo";
            try
            {
                var ps = new ProcessStartInfo(help_url)
                {
                    UseShellExecute = true,
                    Verb = "open"
                };
                Process.Start(ps);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(),
                    "Could not open documentation",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Escape key exits the form.
        /// </summary>
        protected override bool ProcessDialogKey(Keys keyData)
        {
            if (ModifierKeys == Keys.None && keyData == Keys.Escape)
            {
                this.Close();
                return true;
            }
            return base.ProcessDialogKey(keyData);
        }
    }
}
