using System;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using Kbg.NppPluginNET.PluginInfrastructure;

namespace NppPluginNET
{
    /// <summary>
    /// contains connectors to Scintilla (editor) and Notepad++ (notepad)
    /// and miscellaneous other helper functions
    /// </summary>
    public class MiscUtils
    {
        /// <summary>
        /// connector to Scintilla
        /// </summary>
        public static IScintillaGateway editor = NavigateTo.Plugin.Namespace.Main.editor;
        /// <summary>
        /// connector to Notepad++
        /// </summary>
        public static INotepadPPGateway notepad = NavigateTo.Plugin.Namespace.Main.notepad;

        /// <summary>
        /// append text to current doc, then append newline and move cursor
        /// </summary>
        /// <param name="inp"></param>
        public static void AddLine(string inp)
        {
            editor.AppendText(Encoding.UTF8.GetByteCount(inp), inp);
            editor.AppendText(Environment.NewLine.Length, Environment.NewLine);
        }

        /// <summary>
        /// input is one of 'p', 'd', 'f'<br></br>
        /// if 'p', get full path to current file (default)<br></br>
        /// if 'd', get directory of current file<br></br>
        /// if 'f', get filename of current file
        /// </summary>
        /// <param name="which"></param>
        /// <returns></returns>
        public static string GetCurrentPath(char which = 'p')
        {
            NppMsg msg = NppMsg.NPPM_GETFULLCURRENTPATH;
            switch (which)
            {
            case 'p': break;
            case 'd': msg = NppMsg.NPPM_GETCURRENTDIRECTORY; break;
            case 'f': msg = NppMsg.NPPM_GETFILENAME; break;
            default: throw new ArgumentException("GetCurrentPath argument must be one of 'p', 'd', 'f'");
            }

            StringBuilder path = new StringBuilder(Win32.MAX_PATH);
            Win32.SendMessage(PluginBase.nppData._nppHandle, (uint)msg, 0, path);

            return path.ToString();
        }

        /// <summary>
        /// Trying to copy an empty string or null to the clipboard raises an error.<br></br>
        /// This shows a message box if the user tries to do that.
        /// </summary>
        /// <param name="text"></param>
        public static void TryCopyToClipboard(string text)
        {
            if (text == null || text.Length == 0)
            {
                MessageBox.Show("Couldn't find anything to copy to the clipboard",
                    "Nothing to copy to clipboard",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return;
            }
            Clipboard.SetText(text);
        }

        public static string AssemblyVersionString()
        {
            string version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            while (version.EndsWith(".0"))
                version = version.Substring(0, version.Length - 2);
            return version;
        }
    }
}
