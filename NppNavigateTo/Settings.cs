﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using Kbg.NppPluginNET.PluginInfrastructure;
using NavigateTo.Plugin.Namespace;

namespace NppPluginNET
{
    internal class Settings
    {
        public static string keepDlgOpen = "keepDlgOpen";
        public static string columnNameWidth = "columnNameWidth";
        public static string columnPathWidth = "columnPathWidth";
        public static string columnSourceWidth = "columnSourceWidth";
        public static string gridMinWidth = "gridMinWidth";
        public static string searchInCurrentFolder = "searchInCurrentFolder";
        public static string searchInSubDirs = "searchInSubDirs";
        public static string highlightColorBackground = "highlightColorBackground";
        public static string gridSelectedRowBackground = "gridSelectedRowBackground";
        public static string gridSelectedRowForeground = "gridSelectedRowForeground";
        public static string gridTextColor = "gridTextColor";
        public static string gridBackgroundColor = "gridBackgroundColor";
        //public static string fontSize = "fontSize";
        public static string rowBackgroundColor = "rowBackgroundColor";
        public static string searchMenuCommands = "searchMenuCommands";
        public static string minTypeCharLimit = "minTypeCharLimit";
        public static string clearOnClose = "clearOnClose";
        public static string selectFirstRowOnFilter = "selectFirstRowOnFilter";
        public static string preferFilenameResults = "preferFilenameResults";
        public static string sortAfterFilterBy = "sortAfterFilterBy";
        public static string sortOrderAfterFilterBy = "sortOrderAfterFilterBy";
        public static string fuzzySearch = "fuzzySearch";
        public static string fuzzynessTolerance = "fuzzynessTolerance";
        public static string secondsBetweenDirectoryScans = "secondsBetweenDirectoryScans";
        public static string maxResultsHighlightingEnabled = "maxResultsHighlightingEnabled";
        public static string searchDelayMs = "searchDelayMs";

        static string iniFilePath;
        static string lpAppName = "NavigateTo";

        private Dictionary<string, string> data = new Dictionary<string, string>();

        public string GetSetting(string name)
        {
            if (data.ContainsKey(name))
            {
                return data[name];
            }

            return null;
        }

        public int GetIntSetting(string name)
        {
            return Int32.Parse(GetSetting(name));
        }

        public bool GetBoolSetting(string name)
        {
            string value = GetSetting(name);

            return value == "1" || value == "True";
        }

        public void SetBoolSetting(string name, bool value)
        {
            SetSetting(name, value ? "1" : "0");
        }

        public void SetIntSetting(string name, int value)
        {
            SetSetting(name, value.ToString());
        }

        public void SetSetting(string name, string value)
        {
            data[name] = value;
            Win32.WritePrivateProfileString(lpAppName, name, value, iniFilePath);
        }

        public void LoadSetting(string name, string value)
        {
            data[name] = value;
        }

        public void LoadIntSetting(string name, int value)
        {
            LoadSetting(name, value.ToString());
        }

        public void LoadConfigValues()
        {
            StringBuilder sbIniFilePath = new StringBuilder(Win32.MAX_PATH);
            Win32.SendMessage(PluginBase.nppData._nppHandle, (uint)NppMsg.NPPM_GETPLUGINSCONFIGDIR, Win32.MAX_PATH,
                sbIniFilePath);
            iniFilePath = sbIniFilePath.ToString();

            // if config path doesn't exist, we create it
            if (!Directory.Exists(iniFilePath))
            {
                Directory.CreateDirectory(iniFilePath);
            }

            // make your plugin config file full file path name
            iniFilePath = Path.Combine(iniFilePath, Main.PluginName + ".ini");
            LoadIntSetting(keepDlgOpen,
                Win32.GetPrivateProfileInt(lpAppName, keepDlgOpen, 1, iniFilePath));
            LoadIntSetting(columnNameWidth,
                Win32.GetPrivateProfileInt(lpAppName, columnNameWidth, 30, iniFilePath));
            LoadIntSetting(columnPathWidth,
                Win32.GetPrivateProfileInt(lpAppName, columnPathWidth, 60, iniFilePath));
            LoadIntSetting(columnSourceWidth,
                Win32.GetPrivateProfileInt(lpAppName, columnSourceWidth, 10, iniFilePath));
            LoadIntSetting(gridMinWidth,
                Win32.GetPrivateProfileInt(lpAppName, gridMinWidth, 300, iniFilePath));
            LoadIntSetting(searchInCurrentFolder,
                Win32.GetPrivateProfileInt(lpAppName, searchInCurrentFolder, 0, iniFilePath));
            LoadIntSetting(searchInSubDirs,
                Win32.GetPrivateProfileInt(lpAppName, searchInSubDirs, 0, iniFilePath));
            LoadIntSetting(searchMenuCommands,
                Win32.GetPrivateProfileInt(lpAppName, searchMenuCommands, 0, iniFilePath));
            LoadIntSetting(highlightColorBackground,
                Win32.GetPrivateProfileInt(lpAppName, highlightColorBackground, -23296, iniFilePath));
            LoadIntSetting(gridSelectedRowBackground,
                Win32.GetPrivateProfileInt(lpAppName, gridSelectedRowBackground, -16746281, iniFilePath));
            LoadIntSetting(gridSelectedRowForeground,
                Win32.GetPrivateProfileInt(lpAppName, gridSelectedRowForeground, -16777216, iniFilePath));
            LoadIntSetting(gridTextColor,
                Win32.GetPrivateProfileInt(lpAppName, gridTextColor, -16777216, iniFilePath));
            LoadIntSetting(gridBackgroundColor,
                Win32.GetPrivateProfileInt(lpAppName, gridBackgroundColor, -5526613, iniFilePath));
            LoadIntSetting(minTypeCharLimit,
                Win32.GetPrivateProfileInt(lpAppName, minTypeCharLimit, 2, iniFilePath));
            LoadIntSetting(clearOnClose,
                Win32.GetPrivateProfileInt(lpAppName, clearOnClose, 0, iniFilePath));
            LoadIntSetting(selectFirstRowOnFilter,
                Win32.GetPrivateProfileInt(lpAppName, selectFirstRowOnFilter, 0, iniFilePath));
            LoadIntSetting(preferFilenameResults,
                Win32.GetPrivateProfileInt(lpAppName, preferFilenameResults, 0, iniFilePath));
            LoadIntSetting(sortAfterFilterBy,
                Win32.GetPrivateProfileInt(lpAppName, sortAfterFilterBy, -1, iniFilePath));
            LoadIntSetting(sortOrderAfterFilterBy,
                Win32.GetPrivateProfileInt(lpAppName, sortOrderAfterFilterBy, 0, iniFilePath));
            LoadIntSetting(fuzzySearch,
                Win32.GetPrivateProfileInt(lpAppName, fuzzySearch, 0, iniFilePath));
            LoadIntSetting(fuzzynessTolerance,
                Win32.GetPrivateProfileInt(lpAppName, fuzzynessTolerance, 1, iniFilePath));
            LoadIntSetting(rowBackgroundColor,
                Win32.GetPrivateProfileInt(lpAppName, rowBackgroundColor, -1, iniFilePath));
            LoadIntSetting(secondsBetweenDirectoryScans,
                Win32.GetPrivateProfileInt(lpAppName, secondsBetweenDirectoryScans, 5, iniFilePath));
            LoadIntSetting(maxResultsHighlightingEnabled,
                Win32.GetPrivateProfileInt(lpAppName, maxResultsHighlightingEnabled, 5000, iniFilePath));
            LoadIntSetting(searchDelayMs,
                Win32.GetPrivateProfileInt(lpAppName, searchDelayMs, 300, iniFilePath));
            // TODO: add customizable font size; changing font is tricky, so am not doing yet
            //LoadIntSetting(fontSize,
            //    Win32.GetPrivateProfileInt(lpAppName, fontSize, 8, iniFilePath));

        }

        public void SetColorSetting(String name, Color color)
        {
            int colorInt32 =
                BitConverter.ToInt32(
                    new[]
                    {
                        color.B, color.G, color.R, color.A
                    }, 0);
            SetIntSetting(name, colorInt32);
        }

        public Color GetColorSetting(String name)
        {
            return Color.FromArgb(GetIntSetting(name));
        }
    }
}