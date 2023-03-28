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

namespace NavigateTo.Plugin.Namespace
{
    public partial class AboutForm : Form
    {
        public AboutForm()
        {
            InitializeComponent();
            Title.Text = $"NavigateTo v{AssemblyVersionString()}";
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

        public static string AssemblyVersionString()
        {
            string version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            while (version.EndsWith(".0"))
                version = version.Substring(0, version.Length - 2);
            return version;
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
