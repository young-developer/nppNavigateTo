﻿// NPP plugin platform for .Net v0.94.00 by Kasper B. Graversen etc.

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using NppPluginNET.PluginInfrastructure;

namespace Kbg.NppPluginNET.PluginInfrastructure
{
    public interface INotepadPPGateway
    {
        void FileNew();

        void AddToolbarIcon(int funcItemsIndex, toolbarIcons icon);
        void AddToolbarIcon(int funcItemsIndex, Bitmap icon);
        string GetNppPath();
        string GetPluginConfigPath();
        string GetCurrentFilePath();
        string GetCurrentFileDirectory();
        unsafe string GetFilePath(int bufferId);
        void SetCurrentLanguage(LangType language);
        bool OpenFile(string path);
        bool SwitchToFile(string path, bool isTab);
        Dictionary<NppMenuCmd, string> MainMenuItems { get; set; }
        string ReloadMenuItems();
        int getCurrentView();
        int[] GetNppVersion();
        Color GetDefaultForegroundColor();
        Color GetDefaultBackgroundColor();
        bool IsDarkModeEnabled();
        IntPtr GetDarkModeColors();
    }

    /// <summary>
    /// This class holds helpers for sending messages defined in the Msgs_h.cs file. It is at the moment
    /// incomplete. Please help fill in the blanks.
    /// </summary>
    public partial class NotepadPPGateway : INotepadPPGateway
    {
        private const int Unused = 0;

        public Dictionary<NppMenuCmd, string> MainMenuItems { get; set; }

        public NotepadPPGateway()
        {
            MainMenuItems = new Dictionary<NppMenuCmd, string>();
        }


        public string ReloadMenuItems()
        {
            var cmdList = Enum.GetValues(typeof(NppMenuCmd));
            IntPtr mainMenu = Win32.SendMessage(
                PluginBase.nppData._nppHandle,
                (uint)NppMsg.NPPM_GETMENUHANDLE,
                (int)NppMsg.NPPMAINMENU,
                0);
            MainMenuItems = new Dictionary<NppMenuCmd, string>();
            try
            {
                foreach (var cmd in cmdList)
                {
                    string text = Win32.GetMenuItemText(mainMenu, (uint)cmd, false);

                    if (text.Length != 0 && !MainMenuItems.ContainsKey((NppMenuCmd)cmd))
                    {
                        if (text.Contains("\t"))
                        {
                            text += " ]";
                        }

                        MainMenuItems.Add((NppMenuCmd)cmd, text.Replace("&", "").Replace("\t", " [ "));
                    }
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

            return "";
        }

        public int getCurrentView()
        {
            return (int)Win32.SendMessage(PluginBase.nppData._nppHandle, (uint)NppMsg.NPPM_GETCURRENTVIEW, Unused,
                Unused);
        }

        public void FileNew()
        {
            Win32.SendMessage(PluginBase.nppData._nppHandle, (uint)NppMsg.NPPM_MENUCOMMAND, Unused,
                NppMenuCmd.IDM_FILE_NEW);
        }

        public void AddToolbarIcon(int funcItemsIndex, toolbarIcons icon)
        {
            IntPtr pTbIcons = Marshal.AllocHGlobal(Marshal.SizeOf(icon));
            try
            {
                Marshal.StructureToPtr(icon, pTbIcons, false);
                _ = Win32.SendMessage(
                    PluginBase.nppData._nppHandle,
                    (uint)NppMsg.NPPM_ADDTOOLBARICON,
                    PluginBase._funcItems.Items[funcItemsIndex]._cmdID,
                    pTbIcons);
            }
            finally
            {
                Marshal.FreeHGlobal(pTbIcons);
            }
        }

        public void AddToolbarIcon(int funcItemsIndex, Bitmap icon)
        {
            var tbi = new toolbarIcons();
            tbi.hToolbarBmp = icon.GetHbitmap();
            AddToolbarIcon(funcItemsIndex, tbi);
        }

        /// <summary>
        /// Gets the path of the current document.
        /// </summary>
        public string GetCurrentFilePath()
        {
            var path = new StringBuilder(2000);
            Win32.SendMessage(PluginBase.nppData._nppHandle, (uint)NppMsg.NPPM_GETFULLCURRENTPATH, 0, path);
            return path.ToString();
        }

        /// <summary>
        /// Gets the directory of the current document.
        /// </summary>
        public string GetCurrentFileDirectory()
        {
            var path = new StringBuilder(2000);
            Win32.SendMessage(PluginBase.nppData._nppHandle, (uint)NppMsg.NPPM_GETCURRENTDIRECTORY, 0, path);
            return path.ToString();
        }

        /// <summary>
        /// This method incapsulates a common pattern in the Notepad++ API: when
        /// you need to retrieve a string, you can first query the buffer size.
        /// This method queries the necessary buffer size, allocates the temporary
        /// memory, then returns the string retrieved through that buffer.
        /// </summary>
        /// <param name="message">Message ID of the data string to query.</param>
        /// <returns>String returned by Notepad++.</returns>
        public string GetString(NppMsg message)
        {
            int len = Win32.SendMessage(
                          PluginBase.nppData._nppHandle,
                          (uint)message, Unused, Unused).ToInt32()
                      + 1;
            var res = new StringBuilder(len);
            _ = Win32.SendMessage(
                PluginBase.nppData._nppHandle, (uint)message, len, res);
            return res.ToString();
        }

        /// <returns>The path to the Notepad++ executable.</returns>
        public string GetNppPath()
            => GetString(NppMsg.NPPM_GETNPPDIRECTORY);

        /// <returns>The path to the Config folder for plugins.</returns>
        public string GetPluginConfigPath()
            => GetString(NppMsg.NPPM_GETPLUGINSCONFIGDIR);

        /// <summary>
        /// Open a file for editing in Notepad++, pretty much like using the app's
        /// File - Open menu.
        /// </summary>
        /// <param name="path">The path to the file to open.</param>
        /// <returns>True on success.</returns>
        public bool OpenFile(string path)
            => Win32.SendMessage(
                   PluginBase.nppData._nppHandle, (uint)NppMsg.NPPM_DOOPEN, Unused, path).ToInt32()
               != 0;

        public bool SwitchToFile(string path, bool isTab = true)
        {
            if (isTab)
            {
                return Win32.SendMessage(
                           PluginBase.nppData._nppHandle, (uint)NppMsg.NPPM_SWITCHTOFILE, Unused, path).ToInt32()
                       != 0;
            }

            return OpenFile(path);
        }

        /// <summary>
        /// Gets the path of the current document.
        /// </summary>
        public unsafe string GetFilePath(int bufferId)
        {
            var path = new StringBuilder(2000);
            Win32.SendMessage(PluginBase.nppData._nppHandle, (uint)NppMsg.NPPM_GETFULLPATHFROMBUFFERID, bufferId, path);
            return path.ToString();
        }

        public void SetCurrentLanguage(LangType language)
        {
            Win32.SendMessage(PluginBase.nppData._nppHandle, (uint)NppMsg.NPPM_SETCURRENTLANGTYPE, Unused,
                (int)language);
        }

        /// <summary>
		/// 3-int array: {major, minor, bugfix}<br></br>
		/// Thus GetNppVersion() would return {8, 5, 0} for version 8.5.0
		/// and {7, 7, 1} for version 7.7.1
		/// </summary>
		/// <returns></returns>
		public int[] GetNppVersion()
        {
            int version = Win32.SendMessage(PluginBase.nppData._nppHandle, (uint)NppMsg.NPPM_GETNPPVERSION, 0, 0).ToInt32();
            int major = version >> 16;
            int minor = Math.DivRem(version & 0xffff, 10, out int bugfix);
            return new int[] { major, minor, bugfix };
        }

        public Color GetDefaultForegroundColor()
        {
            var rawColor = (int)Win32.SendMessage(PluginBase.nppData._nppHandle, (uint)NppMsg.NPPM_GETEDITORDEFAULTFOREGROUNDCOLOR, 0, 0);
            return Color.FromArgb(rawColor & 0xff, (rawColor >> 8) & 0xff, (rawColor >> 16) & 0xff);
        }

        public Color GetDefaultBackgroundColor()
        {
            var rawColor = (int)Win32.SendMessage(PluginBase.nppData._nppHandle, (uint)NppMsg.NPPM_GETEDITORDEFAULTBACKGROUNDCOLOR, 0, 0);
            return Color.FromArgb(rawColor & 0xff, (rawColor >> 8) & 0xff, (rawColor >> 16) & 0xff);
        }
    }

    /// <summary>
    /// This class holds helpers for sending messages defined in the Resource_h.cs file. It is at the moment
    /// incomplete. Please help fill in the blanks.
    /// </summary>
    class NppResource
    {
        private const int Unused = 0;

        public void ClearIndicator()
        {
            Win32.SendMessage(PluginBase.nppData._nppHandle, (uint)Resource.NPPM_INTERNAL_CLEARINDICATOR, Unused,
                Unused);
        }
    }
}