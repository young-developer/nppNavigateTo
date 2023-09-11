// NPP plugin platform for .Net v0.94.00 by Kasper B. Graversen etc.

using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;

namespace Kbg.NppPluginNET.PluginInfrastructure
{
    public class Win32
    {
        /// <summary>
        /// Get the scroll information of a scroll bar or window with scroll bar
        /// @see https://msdn.microsoft.com/en-us/library/windows/desktop/bb787537(v=vs.85).aspx
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct ScrollInfo
        {
            /// <summary>
            /// Specifies the size, in bytes, of this structure. The caller must set this to sizeof(SCROLLINFO).
            /// </summary>
            public uint cbSize;

            /// <summary>
            /// Specifies the scroll bar parameters to set or retrieve.
            /// @see ScrollInfoMask
            /// </summary>
            public uint fMask;

            /// <summary>
            /// Specifies the minimum scrolling position.
            /// </summary>
            public int nMin;

            /// <summary>
            /// Specifies the maximum scrolling position.
            /// </summary>
            public int nMax;

            /// <summary>
            /// Specifies the page size, in device units. A scroll bar uses this value to determine the appropriate size of the proportional scroll box.
            /// </summary>
            public uint nPage;

            /// <summary>
            /// Specifies the position of the scroll box.
            /// </summary>
            public int nPos;

            /// <summary>
            /// Specifies the immediate position of a scroll box that the user is dragging. 
            /// An application can retrieve this value while processing the SB_THUMBTRACK request code. 
            /// An application cannot set the immediate scroll position; the SetScrollInfo function ignores this member.
            /// </summary>
            public int nTrackPos;
        }

        /// <summary>
        /// Used for the ScrollInfo fMask
        /// SIF_ALL             => Combination of SIF_PAGE, SIF_POS, SIF_RANGE, and SIF_TRACKPOS.
        /// SIF_DISABLENOSCROLL => This value is used only when setting a scroll bar's parameters. If the scroll bar's new parameters make the scroll bar unnecessary, disable the scroll bar instead of removing it.
        /// SIF_PAGE            => The nPage member contains the page size for a proportional scroll bar.
        /// SIF_POS             => The nPos member contains the scroll box position, which is not updated while the user drags the scroll box.
        /// SIF_RANGE           => The nMin and nMax members contain the minimum and maximum values for the scrolling range.
        /// SIF_TRACKPOS        => The nTrackPos member contains the current position of the scroll box while the user is dragging it.
        /// </summary>
        public enum ScrollInfoMask
        {
            SIF_RANGE = 0x1,
            SIF_PAGE = 0x2,
            SIF_POS = 0x4,
            SIF_DISABLENOSCROLL = 0x8,
            SIF_TRACKPOS = 0x10,
            SIF_ALL = SIF_RANGE + SIF_PAGE + SIF_POS + SIF_TRACKPOS
        }

        /// <summary>
        /// Used for the GetScrollInfo() nBar parameter
        /// </summary>
        public enum ScrollInfoBar
        {
            SB_HORZ = 0,
            SB_VERT = 1,
            SB_CTL = 2,
            SB_BOTH = 3
        }

        /// <summary>
        /// You should try to avoid calling this method in your plugin code. Rather use one of the gateways such as 
        /// <see cref="ScintillaGateway"/> or <see cref="NotepadPPGateway"/>.  
        /// If gateways are missing or incomplete, please help extend them and send your code to the project 
        /// at https://github.com/kbilsted/NotepadPlusPlusPluginPack.Net
        /// </summary>
        [DllImport("user32")]
        public static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, IntPtr wParam,
            [MarshalAs(UnmanagedType.LPWStr)] string lParam);

        [DllImport("user32")]
        public static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg,
            [MarshalAs(UnmanagedType.LPWStr)] string wParam,
            [MarshalAs(UnmanagedType.LPWStr)] string lParam);

        /// <summary>
        /// You should try to avoid calling this method in your plugin code. Rather use one of the gateways such as 
        /// <see cref="ScintillaGateway"/> or <see cref="NotepadPPGateway"/>.  
        /// If gateways are missing or incomplete, please help extend them and send your code to the project 
        /// at https://github.com/kbilsted/NotepadPlusPlusPluginPack.Net
        /// </summary>
        [DllImport("user32")]
        public static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, IntPtr wParam,
            [MarshalAs(UnmanagedType.LPWStr)] StringBuilder lParam);

        /// <summary>
        /// You should try to avoid calling this method in your plugin code. Rather use one of the gateways such as 
        /// <see cref="ScintillaGateway"/> or <see cref="NotepadPPGateway"/>.  
        /// If gateways are missing or incomplete, please help extend them and send your code to the project 
        /// at https://github.com/kbilsted/NotepadPlusPlusPluginPack.Net
        /// </summary>
        [DllImport("user32")]
        public static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, IntPtr wParam, IntPtr lParam);

        /// <summary>
        /// You should try to avoid calling this method in your plugin code. Rather use one of the gateways such as 
        /// <see cref="ScintillaGateway"/> or <see cref="NotepadPPGateway"/>.  
        /// If gateways are missing or incomplete, please help extend them and send your code to the project 
        /// at https://github.com/kbilsted/NotepadPlusPlusPluginPack.Net
        /// </summary>
        [DllImport("user32")]
        public static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, IntPtr wParam, out IntPtr lParam);

        /// <summary>
        /// You should try to avoid calling this method in your plugin code. Rather use one of the gateways such as 
        /// <see cref="ScintillaGateway"/> or <see cref="NotepadPPGateway"/>.  
        /// If gateways are missing or incomplete, please help extend them and send your code to the project 
        /// at https://github.com/kbilsted/NotepadPlusPlusPluginPack.Net
        /// </summary>
        public static IntPtr SendMessage(IntPtr hWnd, uint Msg, int wParam, NppMenuCmd lParam)
        {
            return SendMessage(hWnd, (UInt32)Msg, new IntPtr(wParam), new IntPtr((uint)lParam));
        }

        /// <summary>
        /// You should try to avoid calling this method in your plugin code. Rather use one of the gateways such as 
        /// <see cref="ScintillaGateway"/> or <see cref="NotepadPPGateway"/>.  
        /// If gateways are missing or incomplete, please help extend them and send your code to the project 
        /// at https://github.com/kbilsted/NotepadPlusPlusPluginPack.Net
        /// </summary>
        public static IntPtr SendMessage(IntPtr hWnd, uint Msg, int wParam, IntPtr lParam)
        {
            return SendMessage(hWnd, (UInt32)Msg, new IntPtr(wParam), lParam);
        }

        /// <summary>
        /// You should try to avoid calling this method in your plugin code. Rather use one of the gateways such as 
        /// <see cref="ScintillaGateway"/> or <see cref="NotepadPPGateway"/>.  
        /// If gateways are missing or incomplete, please help extend them and send your code to the project 
        /// at https://github.com/kbilsted/NotepadPlusPlusPluginPack.Net
        /// </summary>
        public static IntPtr SendMessage(IntPtr hWnd, uint Msg, int wParam, int lParam)
        {
            return SendMessage(hWnd, (UInt32)Msg, new IntPtr(wParam), new IntPtr(lParam));
        }

        /// <summary>
        /// You should try to avoid calling this method in your plugin code. Rather use one of the gateways such as 
        /// <see cref="ScintillaGateway"/> or <see cref="NotepadPPGateway"/>.  
        /// If gateways are missing or incomplete, please help extend them and send your code to the project 
        /// at https://github.com/kbilsted/NotepadPlusPlusPluginPack.Net
        /// </summary>
        public static IntPtr SendMessage(IntPtr hWnd, uint Msg, int wParam, out int lParam)
        {
            IntPtr outVal;
            IntPtr retval = SendMessage(hWnd, (UInt32)Msg, new IntPtr(wParam), out outVal);
            lParam = outVal.ToInt32();
            return retval;
        }

        /// <summary>
        /// You should try to avoid calling this method in your plugin code. Rather use one of the gateways such as 
        /// <see cref="ScintillaGateway"/> or <see cref="NotepadPPGateway"/>.  
        /// If gateways are missing or incomplete, please help extend them and send your code to the project 
        /// at https://github.com/kbilsted/NotepadPlusPlusPluginPack.Net
        /// </summary>
        public static IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, int lParam)
        {
            return SendMessage(hWnd, (UInt32)Msg, wParam, new IntPtr(lParam));
        }

        /// <summary>
        /// You should try to avoid calling this method in your plugin code. Rather use one of the gateways such as 
        /// <see cref="ScintillaGateway"/> or <see cref="NotepadPPGateway"/>.  
        /// If gateways are missing or incomplete, please help extend them and send your code to the project 
        /// at https://github.com/kbilsted/NotepadPlusPlusPluginPack.Net
        /// </summary>
        public static IntPtr SendMessage(IntPtr hWnd, uint Msg, int wParam,
            [MarshalAs(UnmanagedType.LPWStr)] StringBuilder lParam)
        {
            return SendMessage(hWnd, (UInt32)Msg, new IntPtr(wParam), lParam);
        }

        /// <summary>
        /// You should try to avoid calling this method in your plugin code. Rather use one of the gateways such as 
        /// <see cref="ScintillaGateway"/> or <see cref="NotepadPPGateway"/>.  
        /// If gateways are missing or incomplete, please help extend them and send your code to the project 
        /// at https://github.com/kbilsted/NotepadPlusPlusPluginPack.Net
        /// </summary>
        public static IntPtr SendMessage(IntPtr hWnd, uint Msg, int wParam,
            [MarshalAs(UnmanagedType.LPWStr)] string lParam)
        {
            return SendMessage(hWnd, (UInt32)Msg, new IntPtr(wParam), lParam);
        }

        /// <summary>
        /// You should try to avoid calling this method in your plugin code. Rather use one of the gateways such as 
        /// <see cref="ScintillaGateway"/> or <see cref="NotepadPPGateway"/>.  
        /// If gateways are missing or incomplete, please help extend them and send your code to the project 
        /// at https://github.com/kbilsted/NotepadPlusPlusPluginPack.Net
        /// </summary>
        public static IntPtr SendMessage(IntPtr hWnd, SciMsg Msg, IntPtr wParam, int lParam)
        {
            return SendMessage(hWnd, (UInt32)Msg, wParam, new IntPtr(lParam));
        }

        /// <summary>
        /// You should try to avoid calling this method in your plugin code. Rather use one of the gateways such as 
        /// <see cref="ScintillaGateway"/> or <see cref="NotepadPPGateway"/>.  
        /// If gateways are missing or incomplete, please help extend them and send your code to the project 
        /// at https://github.com/kbilsted/NotepadPlusPlusPluginPack.Net
        /// </summary>
        public static IntPtr SendMessage(IntPtr hWnd, SciMsg Msg, int wParam, IntPtr lParam)
        {
            return SendMessage(hWnd, (UInt32)Msg, new IntPtr(wParam), lParam);
        }

        /// <summary>
        /// You should try to avoid calling this method in your plugin code. Rather use one of the gateways such as 
        /// <see cref="ScintillaGateway"/> or <see cref="NotepadPPGateway"/>.  
        /// If gateways are missing or incomplete, please help extend them and send your code to the project 
        /// at https://github.com/kbilsted/NotepadPlusPlusPluginPack.Net
        /// </summary>
        public static IntPtr SendMessage(IntPtr hWnd, SciMsg Msg, int wParam, string lParam)
        {
            return SendMessage(hWnd, (UInt32)Msg, new IntPtr(wParam), lParam);
        }

        public static IntPtr SendMessage(IntPtr hWnd, SciMsg Msg, string wParam, string lParam)
        {
            return SendMessage(hWnd, (UInt32)Msg, wParam, lParam);
        }

        /// <summary>
        /// You should try to avoid calling this method in your plugin code. Rather use one of the gateways such as 
        /// <see cref="ScintillaGateway"/> or <see cref="NotepadPPGateway"/>.  
        /// If gateways are missing or incomplete, please help extend them and send your code to the project 
        /// at https://github.com/kbilsted/NotepadPlusPlusPluginPack.Net
        /// </summary>
        public static IntPtr SendMessage(IntPtr hWnd, SciMsg Msg, int wParam,
            [MarshalAs(UnmanagedType.LPStr)] StringBuilder lParam)
        {
            return SendMessage(hWnd, (UInt32)Msg, new IntPtr(wParam), lParam);
        }

        /// <summary>
        /// You should try to avoid calling this method in your plugin code. Rather use one of the gateways such as 
        /// <see cref="ScintillaGateway"/> or <see cref="NotepadPPGateway"/>.  
        /// If gateways are missing or incomplete, please help extend them and send your code to the project 
        /// at https://github.com/kbilsted/NotepadPlusPlusPluginPack.Net
        /// </summary>
        public static IntPtr SendMessage(IntPtr hWnd, SciMsg Msg, int wParam, int lParam)
        {
            return SendMessage(hWnd, (UInt32)Msg, new IntPtr(wParam), new IntPtr(lParam));
        }

        /// <summary>
        /// You should try to avoid calling this method in your plugin code. Rather use one of the gateways such as 
        /// <see cref="ScintillaGateway"/> or <see cref="NotepadPPGateway"/>.  
        /// If gateways are missing or incomplete, please help extend them and send your code to the project 
        /// at https://github.com/kbilsted/NotepadPlusPlusPluginPack.Net
        /// </summary>
        public static IntPtr SendMessage(IntPtr hWnd, SciMsg Msg, IntPtr wParam, IntPtr lParam)
        {
            return SendMessage(hWnd, (UInt32)Msg, wParam, lParam);
        }

        /// <summary>
        /// You should try to avoid calling this method in your plugin code. Rather use one of the gateways such as
        /// <see cref="ScintillaGateway"/> or <see cref="NotepadPPGateway"/>.
        /// If gateways are missing or incomplete, please help extend them and send your code to the project
        /// at https://github.com/kbilsted/NotepadPlusPlusPluginPack.Net
        /// </summary>
        public static IntPtr SendMessage(IntPtr hWnd, uint Msg, int wParam, ref LangType lParam)
        {
            IntPtr outVal;
            IntPtr retval = SendMessage(hWnd, (UInt32)Msg, new IntPtr(wParam), out outVal);
            lParam = (LangType)outVal;
            return retval;
        }

        public const int MAX_PATH = 2600;

        [DllImport("kernel32")]
        public static extern int GetPrivateProfileInt(string lpAppName, string lpKeyName, int nDefault,
            string lpFileName);

        [DllImport("kernel32")]
        public static extern int GetPrivateProfileString(string lpAppName, string lpKeyName, string lpDefault,
            StringBuilder lpReturnedString, int nSize, string lpFileName);

        [DllImport("kernel32")]
        public static extern bool WritePrivateProfileString(string lpAppName, string lpKeyName, string lpString,
            string lpFileName);

        public const int MF_BYCOMMAND = 0;
        public const int MF_CHECKED = 8;
        public const int MF_UNCHECKED = 0;

        [DllImport("user32")]
        public static extern IntPtr GetMenu(IntPtr hWnd);

        [DllImport("user32")]
        public static extern IntPtr SetWindowText(IntPtr hWnd, string windowName);

        [DllImport("user32")]
        public static extern int GetMenuItemCount(IntPtr hMenu);

        [DllImport("user32")]
        public static extern int CheckMenuItem(IntPtr hMenu, int uIDCheckItem, int uCheck);

        [DllImport("user32")]
        public static extern bool GetMenuItemInfoW(IntPtr hMenu, uint uItem, bool fByPosition, ref MENUITEMINFO lpmii);

        public const uint MIIM_FTYPE = 0x00000100;
        public const uint MIIM_ID = 0x00000002;
        public const uint MIIM_STRING = 0x00000040;
        public const uint MIIM_SUBMENU = 0x00000004;
        public const uint MFT_SEPARATOR = 0x00000800;

        [StructLayout(LayoutKind.Sequential)]
        public struct MENUITEMINFO
        {
            public uint cbSize;
            public uint fMask;
            public uint fType;
            public uint fState;
            public uint wID;
            public IntPtr hSubMenu;
            public IntPtr hbmpChecked;
            public IntPtr hbmpUnchecked;
            public IntPtr dwItemData;
            public IntPtr dwTypeData;
            public uint cch;
            public IntPtr hbmpItem;

            // return the size of the structure
            public static uint Size
            {
                get { return (uint)Marshal.SizeOf(typeof(MENUITEMINFO)); }
            }
        }

        public static string GetMenuItemText(IntPtr hMenu, uint uItem, bool fByPosition)
        {
            Win32.MENUITEMINFO info = new Win32.MENUITEMINFO();
            info.cbSize = Win32.MENUITEMINFO.Size;
            info.fMask = Win32.MIIM_FTYPE;
            Win32.GetMenuItemInfoW(hMenu, uItem, fByPosition, ref info);

            string s = null;
            if ((info.fType & Win32.MFT_SEPARATOR) == 0)
            {
                info.dwTypeData = IntPtr.Zero;
                info.cch = 0;
                info.fMask = Win32.MIIM_STRING;
                Win32.GetMenuItemInfoW(hMenu, uItem, fByPosition, ref info);

                int len = (int)info.cch;
                info.cch++;
                IntPtr sPtr = Marshal.AllocHGlobal((len + 1) * 2);
                info.dwTypeData = sPtr;
                info.fMask = Win32.MIIM_STRING | Win32.MIIM_FTYPE;
                Win32.GetMenuItemInfoW(hMenu, uItem, fByPosition, ref info);

                s = Marshal.PtrToStringUni(sPtr, len);
                Marshal.FreeHGlobal(sPtr);
            }

            return s;
        }

        [DllImport("user32")]
        public static extern bool ClientToScreen(IntPtr hWnd, ref Point lpPoint);

        [DllImport("kernel32")]
        public static extern void OutputDebugString(string lpOutputString);

        /// <summary>
        /// @see https://msdn.microsoft.com/en-us/library/windows/desktop/bb787583(v=vs.85).aspx
        /// </summary>
        /// <param name="hwnd"></param>
        /// <param name="nBar"></param>
        /// <param name="scrollInfo"></param>
        /// <returns></returns>
        [DllImport("user32")]
        public static extern int GetScrollInfo(IntPtr hwnd, int nBar, ref ScrollInfo scrollInfo);
    }
}