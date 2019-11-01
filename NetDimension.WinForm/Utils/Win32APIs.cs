// *****************************************************************************
// 
//  © Component Factory Pty Ltd 2017. All rights reserved.
//	The software and associated documentation supplied hereunder are the 
//  proprietary information of Component Factory Pty Ltd, 13 Swallows Close, 
//  Mornington, Vic 3931, Australia and are supplied subject to licence terms.
// 
//  Version 4.6.0.0 	www.ComponentFactory.com
// *****************************************************************************

using System;
using System.Text;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace NetDimension.WinForm
{
    internal delegate IntPtr WndProcHandler(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

    internal class Win32
    {
        internal static readonly IntPtr TRUE = new IntPtr(1);
        internal static readonly IntPtr FALSE = new IntPtr(0);

        internal static readonly IntPtr MESSAGE_HANDLED = new IntPtr(1);
        internal static readonly IntPtr MESSAGE_PROCESS = new IntPtr(0);

        public enum SetWindowPosFlags : uint
        {
            SWP_NOSIZE = 0x0001,
            SWP_NOMOVE = 0x0002,
            SWP_NOZORDER = 0x0004,
            SWP_NOREDRAW = 0x0008,
            SWP_NOACTIVATE = 0x0010,
            SWP_FRAMECHANGED = 0x0020,
            SWP_SHOWWINDOW = 0x0040,
            SWP_HIDEWINDOW = 0x0080,
            SWP_NOCOPYBITS = 0x0100,
            SWP_NOOWNERZORDER = 0x0200,
            SWP_NOSENDCHANGING = 0x0400,
            SWP_DRAWFRAME = 0x0020,
            SWP_NOREPOSITION = 0x0200,
            SWP_DEFERERASE = 0x2000,
            SWP_ASYNCWINDOWPOS = 0x4000
        }

        internal static void SendFrameChanged(IntPtr hWnd)
        {
            SetWindowPos(hWnd, IntPtr.Zero, 0, 0, 0, 0,
                (uint)(SetWindowPosFlags.SWP_FRAMECHANGED | SetWindowPosFlags.SWP_NOACTIVATE | SetWindowPosFlags.SWP_NOCOPYBITS |
                SetWindowPosFlags.SWP_NOMOVE | SetWindowPosFlags.SWP_NOOWNERZORDER | SetWindowPosFlags.SWP_NOREPOSITION |
                SetWindowPosFlags.SWP_NOSENDCHANGING | SetWindowPosFlags.SWP_NOSIZE | SetWindowPosFlags.SWP_NOZORDER));
        }
        internal static POINT GetPostionFromPtr(IntPtr lparam)
        {
            var scaledX = LOWORD(lparam);
            var scaledY = HIWORD(lparam);

            var x = scaledX;
            var y = scaledY;

            return new POINT(x, y);
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct BLENDFUNCTION
        {
            /// <summary>
            /// BlendOp field of structure
            /// </summary>
            public byte BlendOp;

            /// <summary>
            /// BlendFlags field of structure
            /// </summary>
            public byte BlendFlags;

            /// <summary>
            /// SourceConstantAlpha field of structure
            /// </summary>
            public byte SourceConstantAlpha;

            /// <summary>
            /// AlphaFormat field of structure
            /// </summary>
            public byte AlphaFormat;
        }
        public enum GetWindowLongFlags
        {
            /// <summary>
            /// Specified GWL_WNDPROC enumeration value.
            /// </summary>
            GWL_WNDPROC = -4,

            /// <summary>
            /// Specified GWL_HINSTANCE enumeration value.
            /// </summary>
            GWL_HINSTANCE = -6,

            /// <summary>
            /// Specified GWL_HWNDPARENT enumeration value.
            /// </summary>
            GWL_HWNDPARENT = -8,

            /// <summary>
            /// Specified GWL_STYLE enumeration value.
            /// </summary>
            GWL_STYLE = -16,

            /// <summary>
            /// Specified GWL_EXSTYLE enumeration value.
            /// </summary>
            GWL_EXSTYLE = -20,

            /// <summary>
            /// Specified GWL_USERDATA enumeration value.
            /// </summary>
            GWL_USERDATA = -21,

            /// <summary>
            /// Specified GWL_ID enumeration value.
            /// </summary>
            GWL_ID = -12
        }

        public enum IDC_STANDARD_CURSORS
        {
            IDC_ARROW = 32512,
            IDC_IBEAM = 32513,
            IDC_WAIT = 32514,
            IDC_CROSS = 32515,
            IDC_UPARROW = 32516,
            IDC_SIZE = 32640,
            IDC_ICON = 32641,
            IDC_SIZENWSE = 32642,
            IDC_SIZENESW = 32643,
            IDC_SIZEWE = 32644,
            IDC_SIZENS = 32645,
            IDC_SIZEALL = 32646,
            IDC_NO = 32648,
            IDC_HAND = 32649,
            IDC_APPSTARTING = 32650,
            IDC_HELP = 32651
        }


        [Flags]
        public enum WindowStyles : long
        {
            WS_OVERLAPPED = 0x00000000,
            WS_POPUP = 0x80000000,
            WS_CHILD = 0x40000000,
            WS_MINIMIZE = 0x20000000,
            WS_VISIBLE = 0x10000000,
            WS_DISABLED = 0x08000000,
            WS_CLIPSIBLINGS = 0x04000000,
            WS_CLIPCHILDREN = 0x02000000,
            WS_MAXIMIZE = 0x01000000,
            WS_CAPTION = 0x00C00000,
            WS_BORDER = 0x00800000,
            WS_DLGFRAME = 0x00400000,
            WS_VSCROLL = 0x00200000,
            WS_HSCROLL = 0x00100000,
            WS_SYSMENU = 0x00080000,
            WS_THICKFRAME = 0x00040000,
            WS_GROUP = 0x00020000,
            WS_TABSTOP = 0x00010000,
            WS_MINIMIZEBOX = 0x00020000,
            WS_MAXIMIZEBOX = 0x00010000,
            WS_TILED = 0x00000000,
            WS_ICONIC = 0x20000000,
            WS_SIZEBOX = 0x00040000,
            WS_POPUPWINDOW = 0x80880000,
            WS_OVERLAPPEDWINDOW = 0x00CF0000,
            WS_TILEDWINDOW = 0x00CF0000,
            WS_CHILDWINDOW = 0x40000000
        }
        [Flags]
        public enum WindowExStyles
        {
            /// <summary>
            /// Specified WS_EX_DLGMODALFRAME enumeration value.
            /// </summary>
            WS_EX_DLGMODALFRAME = 0x00000001,

            /// <summary>
            /// Specified WS_EX_NOPARENTNOTIFY enumeration value.
            /// </summary>
            WS_EX_NOPARENTNOTIFY = 0x00000004,
            /// <summary>
            /// Specified WS_EX_NOACTIVATE enumeration value.
            /// </summary>
            WS_EX_NOACTIVATE = 0x08000000,
            /// <summary>
            /// Specified WS_EX_TOPMOST enumeration value.
            /// </summary>
            WS_EX_TOPMOST = 0x00000008,

            /// <summary>
            /// Specified WS_EX_ACCEPTFILES enumeration value.
            /// </summary>
            WS_EX_ACCEPTFILES = 0x00000010,

            /// <summary>
            /// Specified WS_EX_TRANSPARENT enumeration value.
            /// </summary>
            WS_EX_TRANSPARENT = 0x00000020,

            /// <summary>
            /// Specified WS_EX_MDICHILD enumeration value.
            /// </summary>
            WS_EX_MDICHILD = 0x00000040,

            /// <summary>
            /// Specified WS_EX_TOOLWINDOW enumeration value.
            /// </summary>
            WS_EX_TOOLWINDOW = 0x00000080,

            /// <summary>
            /// Specified WS_EX_WINDOWEDGE enumeration value.
            /// </summary>
            WS_EX_WINDOWEDGE = 0x00000100,

            /// <summary>
            /// Specified WS_EX_CLIENTEDGE enumeration value.
            /// </summary>
            WS_EX_CLIENTEDGE = 0x00000200,

            /// <summary>
            /// Specified WS_EX_CONTEXTHELP enumeration value.
            /// </summary>
            WS_EX_CONTEXTHELP = 0x00000400,

            /// <summary>
            /// Specified WS_EX_RIGHT enumeration value.
            /// </summary>
            WS_EX_RIGHT = 0x00001000,

            /// <summary>
            /// Specified WS_EX_LEFT enumeration value.
            /// </summary>
            WS_EX_LEFT = 0x00000000,

            /// <summary>
            /// Specified WS_EX_RTLREADING enumeration value.
            /// </summary>
            WS_EX_RTLREADING = 0x00002000,

            /// <summary>
            /// Specified WS_EX_LTRREADING enumeration value.
            /// </summary>
            WS_EX_LTRREADING = 0x00000000,

            /// <summary>
            /// Specified WS_EX_LEFTSCROLLBAR enumeration value.
            /// </summary>
            WS_EX_LEFTSCROLLBAR = 0x00004000,

            /// <summary>
            /// Specified WS_EX_RIGHTSCROLLBAR enumeration value.
            /// </summary>
            WS_EX_RIGHTSCROLLBAR = 0x00000000,

            /// <summary>
            /// Specified WS_EX_CONTROLPARENT enumeration value.
            /// </summary>
            WS_EX_CONTROLPARENT = 0x00010000,

            /// <summary>
            /// Specified WS_EX_STATICEDGE enumeration value.
            /// </summary>
            WS_EX_STATICEDGE = 0x00020000,

            /// <summary>
            /// Specified WS_EX_APPWINDOW enumeration value.
            /// </summary>
            WS_EX_APPWINDOW = 0x00040000,

            /// <summary>
            /// Specified WS_EX_OVERLAPPEDWINDOW enumeration value.
            /// </summary>
            WS_EX_OVERLAPPEDWINDOW = 0x00000300,

            /// <summary>
            /// Specified WS_EX_PALETTEWINDOW enumeration value.
            /// </summary>
            WS_EX_PALETTEWINDOW = 0x00000188,

            /// <summary>
            /// Specified WS_EX_LAYERED enumeration value.
            /// </summary>
            WS_EX_LAYERED = 0x00080000
        }

        public enum HitTest : int
        {
            HTERROR = (-2),
            HTTRANSPARENT = (-1),
            HTNOWHERE = 0,
            HTCLIENT = 1,
            HTCAPTION = 2,
            HTSYSMENU = 3,
            HTGROWBOX = 4,
            HTSIZE = HTGROWBOX,
            HTMENU = 5,
            HTHSCROLL = 6,
            HTVSCROLL = 7,
            HTMINBUTTON = 8,
            HTMAXBUTTON = 9,
            HTLEFT = 10,
            HTRIGHT = 11,
            HTTOP = 12,
            HTTOPLEFT = 13,
            HTTOPRIGHT = 14,
            HTBOTTOM = 15,
            HTBOTTOMLEFT = 16,
            HTBOTTOMRIGHT = 17,
            HTBORDER = 18,
            HTREDUCE = HTMINBUTTON,
            HTZOOM = HTMAXBUTTON,
            HTSIZEFIRST = HTLEFT,
            HTSIZELAST = HTBOTTOMRIGHT,
            HTOBJECT = 19,
            HTCLOSE = 20,
            HTHELP = 21
        }

        #region Constants
        internal const uint WS_POPUP = 0x80000000;
        internal const uint WS_MINIMIZE = 0x20000000;
        internal const uint WS_MAXIMIZE = 0x01000000;
        internal const uint WS_VISIBLE = 0x10000000;
        internal const uint WS_BORDER = 0x00800000;
        internal const int PRF_CLIENT = 0x00000004;
        internal const int WS_EX_TOPMOST = 0x00000008;
        internal const int WS_EX_TOOLWINDOW = 0x00000080;
        internal const int WS_EX_LAYERED = 0x00080000;
        internal const int WS_EX_NOACTIVATE = 0x08000000;
        internal const int WS_EX_TRANSPARENT = 0x00000020;
        internal const int WS_EX_CLIENTEDGE = 0x00000200;
        internal const int SC_MINIMIZE = 0xF020;
        internal const int SC_MAXIMIZE = 0xF030;
        internal const int SC_CLOSE = 0xF060;
        internal const int SC_RESTORE = 0xF120;
        internal const int SW_SHOWNOACTIVATE = 4;
        internal const int WM_DESTROY = 0x0002;
        internal const int WM_NCDESTROY = 0x0082;
        internal const int WM_MOVE = 0x0003;
        internal const int WM_ENTERSIZEMOVE = 0x0231;
        internal const int WM_EXITSIZEMOVE = 0x232;
        internal const int WM_SIZE = 0x0005;
        internal const int WM_SIZING = 0x0214;

        internal const int WM_SETFOCUS = 0x0007;
        internal const int WM_KILLFOCUS = 0x0008;
        internal const int WM_SETREDRAW = 0x000B;
        internal const int WM_SETTEXT = 0x000C;
        internal const int WM_PAINT = 0x000F;
        internal const int WM_PRINTCLIENT = 0x0318;
        internal const int WM_CTLCOLOR = 0x0019;
        internal const int WM_ERASEBKGND = 0x0014;
        internal const int WM_ACTIVATEAPP = 0x001C;
        internal const int WM_MOUSEACTIVATE = 0x0021;
        internal const int WM_WINDOWPOSCHANGING = 0x0046;
        internal const int WM_WINDOWPOSCHANGED = 0x0047;
        internal const int WM_HELP = 0x0053;
        internal const int WM_NCCALCSIZE = 0x0083;
        internal const int WM_NCHITTEST = 0x0084;
        internal const int WM_NCPAINT = 0x0085;
        internal const int WM_NCACTIVATE = 0x0086;
        internal const int WM_NCMOUSEMOVE = 0x00A0;
        internal const int WM_NCLBUTTONDOWN = 0x00A1;
        internal const int WM_NCLBUTTONUP = 0x00A2;
        internal const int WM_NCLBUTTONDBLCLK = 0x00A3;
        internal const int WM_NCRBUTTONDOWN = 0x00A4;
        internal const int WM_NCMBUTTONDOWN = 0x00A7;
        internal const int WM_NCMBUTTONDBLCLK = 0x00A9;
        internal const int WM_SETCURSOR = 0x0020;
        internal const int WM_KEYDOWN = 0x0100;
        internal const int WM_KEYUP = 0x0101;
        internal const int WM_CHAR = 0x0102;
        internal const int WM_DEADCHAR = 0x0103;
        internal const int WM_SYSKEYDOWN = 0x0104;
        internal const int WM_SYSKEYUP = 0x0105;
        internal const int WM_SYSCHAR = 0x0106;
        internal const int WM_SYSDEADCHAR = 0x0107;
        internal const int WM_KEYLAST = 0x0108;
        internal const int WM_SYSCOMMAND = 0x0112;
        internal const int WM_HSCROLL = 0x0114;
        internal const int WM_VSCROLL = 0x0115;
        internal const int WM_INITMENU = 0x0116;
        internal const int WM_CTLCOLOREDIT = 0x0133;
        internal const int WM_MOUSEMOVE = 0x0200;
        internal const int WM_LBUTTONDOWN = 0x0201;
        internal const int WM_LBUTTONUP = 0x0202;
        internal const int WM_LBUTTONDBLCLK = 0x0203;
        internal const int WM_RBUTTONDOWN = 0x0204;
        internal const int WM_RBUTTONUP = 0x0205;
        internal const int WM_MBUTTONDOWN = 0x0207;
        internal const int WM_MBUTTONUP = 0x0208;
        internal const int WM_MOUSEWHEEL = 0x020A;
        internal const int WM_NCMOUSELEAVE = 0x02A2;
        internal const int WM_MOUSELEAVE = 0x02A3;
        internal const int WM_PRINT = 0x0317;
        internal const int WM_CONTEXTMENU = 0x007B;
        internal const int MA_NOACTIVATE = 0x03;
        internal const int EM_FORMATRANGE = 0x0439;
        internal const int SWP_NOSIZE = 0x0001;
        internal const int SWP_NOMOVE = 0x0002;
        internal const int SWP_NOZORDER = 0x0004;
        internal const int SWP_NOACTIVATE = 0x0010;
        internal const int SWP_FRAMECHANGED = 0x0020;
        internal const int SWP_NOOWNERZORDER = 0x0200;
        internal const int SWP_SHOWWINDOW = 0x0040;
        internal const int SWP_HIDEWINDOW = 0x0080;
        internal const int RDW_INVALIDATE = 0x0001;
        internal const int RDW_UPDATENOW = 0x0100;
        internal const int RDW_FRAME = 0x0400;
        internal const int DCX_WINDOW = 0x01;
        internal const int DCX_CACHE = 0x02;
        internal const int DCX_CLIPSIBLINGS = 0x10;
        internal const int DCX_INTERSECTRGN = 0x80;
        internal const int DCX_VALIDATE = 0x200000;
        internal const int TME_LEAVE = 0x0002;
        internal const int TME_NONCLIENT = 0x0010;
        internal const int HTNOWHERE = 0x00;
        internal const int HTCLIENT = 0x01;
        internal const int HTCAPTION = 0x02;
        internal const int HTSYSMENU = 0x03;
        internal const int HTGROWBOX = 0x04;
        internal const int HTSIZE = 0x04;
        internal const int HTMENU = 0x05;
        internal const int HTLEFT = 0x0A;
        internal const int HTRIGHT = 0x0B;
        internal const int HTTOP = 0x0C;
        internal const int HTTOPLEFT = 0x0D;
        internal const int HTTOPRIGHT = 0x0E;
        internal const int HTBOTTOM = 0x0F;
        internal const int HTBOTTOMLEFT = 0x10;
        internal const int HTBOTTOMRIGHT = 0x11;
        internal const int HTBORDER = 0x12;
        internal const int HTHELP = 0x15;
        internal const int HTIGNORE = 0xFF;
        internal const int HTTRANSPARENT = -1;
        internal const int ULW_ALPHA = 0x00000002;
        internal const int DEVICE_BITSPIXEL = 12;
        internal const int DEVICE_PLANES = 14;
        internal const int SRCCOPY = 0xCC0020;
        internal const int GWL_STYLE = -16;
        internal const int DTM_SETMCCOLOR = 0x1006;
        internal const int DTT_COMPOSITED = 8192;
        internal const int DTT_GLOWSIZE = 2048;
        internal const int DTT_TEXTCOLOR = 1;
        internal const int MCSC_BACKGROUND = 0;
        internal const int PLANES = 14;
        internal const int BITSPIXEL = 12;
        internal const byte AC_SRC_OVER = 0x00;
        internal const byte AC_SRC_ALPHA = 0x01;
        internal const uint GW_HWNDFIRST = 0;
        internal const uint GW_HWNDLAST = 1;
        internal const uint GW_HWNDNEXT = 2;
        internal const uint GW_HWNDPREV = 3;
        internal const uint GW_OWNER = 4;
        internal const uint GW_CHILD = 5;
        internal const uint GW_ENABLEDPOPUP = 6;
        #endregion

        #region Static Methods
        internal static int LOWORD(IntPtr value)
        {
            int int32 = ((int)value.ToInt64() & 0xFFFF);
            return (int32 > 32767) ? int32 - 65536 : int32;
        }

        internal static int HIWORD(IntPtr value)
        {
            int int32 = (((int)value.ToInt64() >> 0x10) & 0xFFFF);
            return (int32 > 32767) ? int32 - 65536 : int32;
        }

        internal static int LOWORD(int value)
        {
            return (value & 0xFFFF);
        }

        internal static int HIWORD(int value)
        {
            return ((value >> 0x10) & 0xFFFF);
        }

        internal static IntPtr MAKEPARAM(IntPtr l, IntPtr h)
        {
            return (IntPtr)((l.ToInt32() & 0xffff) | (h.ToInt32() << 0x10));
        }

        internal static int MAKELOWORD(int value)
        {
            return (value & 0xFFFF);
        }

        internal static int MAKEHIWORD(int value)
        {
            return ((value & 0xFFFF) << 0x10);
        }
        #endregion

        #region Static User32
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct WNDCLASS
        {
            public uint style;
            public IntPtr lpfnWndProc;
            public int cbClsExtra;
            public int cbWndExtra;
            public IntPtr hInstance;
            public IntPtr hIcon;
            public IntPtr hCursor;
            public IntPtr hbrBackground;
            [MarshalAs(UnmanagedType.LPWStr)]
            public string lpszMenuName;
            [MarshalAs(UnmanagedType.LPWStr)]
            public string lpszClassName;
        }

        [DllImport("user32.dll", SetLastError = true)]
        public static extern UInt16 RegisterClassW([In] ref WNDCLASS lpWndClass);
        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr CreateWindowExW(
            UInt32 dwExStyle,
            [MarshalAs(UnmanagedType.LPWStr)] string lpClassName,
            [MarshalAs(UnmanagedType.LPWStr)] string lpWindowName,
            UInt32 dwStyle,
            Int32 x,
            Int32 y,
            Int32 nWidth,
            Int32 nHeight,
            IntPtr hWndParent,
            IntPtr hMenu,
            IntPtr hInstance,
            IntPtr lpParam
            );
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        internal static extern bool PrintWindow(IntPtr hwnd, IntPtr hDC, uint nFlags);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        internal static extern short VkKeyScan(char ch);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        internal static extern IntPtr WindowFromPoint(Win32.POINT pt);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        internal static extern uint GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        internal static extern uint SetWindowLong(IntPtr hwnd, int nIndex, int nLong);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        internal static extern IntPtr GetActiveWindow();

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        internal static extern int ShowWindow(IntPtr hWnd, short cmdShow);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        internal static extern ushort GetKeyState(int virtKey);
        [DllImport("user32.dll")]
        public static extern IntPtr SetCapture(IntPtr hWnd);
        [DllImport("user32.dll")]
        public static extern bool InvalidateRect(IntPtr hWnd, IntPtr lpRect, bool bErase);
        [DllImport("User32.dll")]
        public static extern bool ReleaseCapture();

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        internal static extern uint SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);
        [DllImport("user32.dll", SetLastError = true)]
        internal static extern bool PostMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        internal static extern int SetWindowPos(IntPtr hWnd, IntPtr hWndAfter, int X, int Y, int Width, int Height, uint flags);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        internal static extern bool RedrawWindow(IntPtr hWnd, IntPtr rectUpdate, IntPtr hRgnUpdate, uint uFlags);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        internal static extern bool RedrawWindow(IntPtr hWnd, ref Win32.RECT rectUpdate, IntPtr hRgnUpdate, uint uFlags);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        internal static extern bool TrackMouseEvent(ref TRACKMOUSEEVENTS tme);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        internal static extern IntPtr GetDC(IntPtr hWnd);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        internal static extern IntPtr GetDCEx(IntPtr hWnd, IntPtr hRgnClip, uint fdwOptions);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        internal static extern IntPtr GetWindowDC(IntPtr hwnd);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        internal static extern bool GetWindowRect(IntPtr hWnd, ref RECT rect);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        internal static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);
        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr DefWindowProcW(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);


        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern bool UpdateLayeredWindow(IntPtr hwnd, IntPtr hdcDst, ref POINT pptDst, ref SIZE psize, IntPtr hdcSrc, ref POINT pprSrc, Int32 crKey, ref BLENDFUNCTION pblend, Int32 dwFlags);


        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        internal static extern void DisableProcessWindowsGhosting();

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        internal static extern void AdjustWindowRectEx(ref RECT rect, int dwStyle, bool hasMenu, int dwExSytle);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        internal static extern int MapWindowPoints(IntPtr hWndFrom, IntPtr hWndTo, [In, Out]Win32.POINTC pt, int cPoints);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        internal static extern bool TranslateMessage([In] ref MSG lpMsg);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        internal static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        internal static extern IntPtr BeginPaint(IntPtr hwnd, ref Win32.PAINTSTRUCT ps);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        internal static extern bool EndPaint(IntPtr hwnd, ref Win32.PAINTSTRUCT ps);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        internal static extern bool GetClientRect(IntPtr hWnd, out RECT lpRect);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        internal extern static int OffsetRect(ref RECT lpRect, int x, int y);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        internal static extern bool InflateRect(ref RECT lprc, int dx, int dy);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        internal static extern IntPtr GetWindow(IntPtr hWnd, uint uCmd);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        internal static extern uint RegisterWindowMessage(string lpString);
        [DllImport("user32.dll", SetLastError = true)]
        public static extern int CloseWindow(IntPtr hWnd);
        [DllImport("user32.dll")]
        public static extern int SetParent(int hWndChild, int hWndParent);
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DestroyWindow(IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern int GetCursorPos(ref POINT lpPoint);
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern bool ScreenToClient(IntPtr hWnd, ref POINT pt);

        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern bool ClientToScreen(IntPtr hWnd, ref POINT lpPoint);

        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr LoadCursor(IntPtr hInstance, uint cursor);

        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SetCursor(IntPtr hCursor);
        #endregion

        #region Static Gdi32
        public const int RGN_AND = 1, RGN_OR = 2, RGN_XOR = 3, RGN_DIFF = 4, RGN_COPY = 5;
        [DllImport("USER32.dll")]
        internal static extern int SetWindowRgn(IntPtr hWnd, IntPtr hRgn, bool redraw);

        [DllImport("gdi32.dll", CharSet = CharSet.Auto)]
        internal static extern int BitBlt(IntPtr hDestDC, int x, int y, int nWidth, int nHeight, IntPtr hSrcDC, int xSrc, int ySrc, int dwRop);

        [DllImport("gdi32.dll", CharSet = CharSet.Auto)]
        internal static extern IntPtr CreateCompatibleBitmap(IntPtr hDC, int nWidth, int nHeight);

        [DllImport("gdi32.dll", CharSet = CharSet.Auto)]
        internal static extern int ExcludeClipRect(IntPtr hDC, int x1, int y1, int x2, int y2);

        [DllImport("gdi32.dll", CharSet = CharSet.Auto)]
        internal static extern int IntersectClipRect(IntPtr hdc, int nLeftRect, int nTopRect, int nRightRect, int nBottomRect);

        [DllImport("gdi32.dll", CharSet = CharSet.Auto)]
        internal static extern int GetDeviceCaps(IntPtr hDC, int nIndex);

        [DllImport("gdi32.dll", CharSet = CharSet.Auto)]
        internal static extern IntPtr CreateDIBSection(IntPtr hDC, BITMAPINFO pBMI, uint iUsage, int ppvBits, IntPtr hSection, uint dwOffset);

        [DllImport("gdi32.dll", CharSet = CharSet.Auto)]
        internal static extern IntPtr CreateCompatibleDC(IntPtr hDC);

        [DllImport("gdi32.dll", CharSet = CharSet.Auto)]
        internal static extern IntPtr SelectObject(IntPtr hDC, IntPtr hObject);

        [DllImport("gdi32.dll", CharSet = CharSet.Auto)]
        internal static extern IntPtr DeleteObject(IntPtr hObject);

        [DllImport("gdi32.dll", CharSet = CharSet.Auto)]
        internal static extern bool DeleteDC(IntPtr hDC);

        [DllImport("gdi32.dll", EntryPoint = "SaveDC", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]
        internal static extern int IntSaveDC(HandleRef hDC);

        [DllImport("gdi32.dll", EntryPoint = "RestoreDC", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]
        internal static extern bool IntRestoreDC(HandleRef hDC, int nSavedDC);

        [DllImport("gdi32.dll", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]
        internal static extern bool GetViewportOrgEx(HandleRef hDC, [In, Out]POINTC point);

        [DllImport("gdi32.dll", EntryPoint = "CreateRectRgn", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]
        internal static extern IntPtr IntCreateRectRgn(int x1, int y1, int x2, int y2);

        [DllImport("gdi32.dll", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]
        internal static extern int GetClipRgn(HandleRef hDC, HandleRef hRgn);

        [DllImport("gdi32.dll", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]
        internal static extern bool SetViewportOrgEx(HandleRef hDC, int x, int y, [In, Out]POINTC point);

        [DllImport("gdi32.dll", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]
        internal static extern int GetRgnBox(HandleRef hRegion, ref RECT clipRect);

        [DllImport("gdi32.dll", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]
        internal static extern int CombineRgn(HandleRef hRgn, HandleRef hRgn1, HandleRef hRgn2, int nCombineMode);

        [DllImport("gdi32.dll", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]
        internal static extern int SelectClipRgn(HandleRef hDC, HandleRef hRgn);

        [DllImport("gdi32.dll", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]
        internal static extern int SelectClipRgn(IntPtr hDC, IntPtr hRgn);

        [DllImport("gdi32.dll", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]
        internal static extern uint SetTextColor(IntPtr hdc, int crColor);

        [DllImport("gdi32.dll", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]
        internal static extern uint SetBkColor(IntPtr hdc, int crColor);

        [DllImport("gdi32.dll", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]
        internal static extern IntPtr CreateSolidBrush(int crColor);
        [DllImport("gdi32.dll")]
        public static extern IntPtr CreateRectRgn(int nLeftRect, int nTopRect, int nRightRect, int nBottomRect);

        public static IntPtr CreateRectRgn(System.Drawing.Rectangle rect)
        {
            return CreateRectRgn(rect.Left, rect.Top, rect.Right, rect.Bottom);
        }

        [DllImport("GDI32.dll")]
        internal static extern int CombineRgn(IntPtr hrgnDest, IntPtr hrgnSrc1, IntPtr hrgnSrc2, int fnCombineMode);


        #endregion

        #region Static DwmApi
        [DllImport("dwmapi.dll", CharSet = CharSet.Auto)]
        internal static extern void DwmIsCompositionEnabled(ref bool enabled);

        [DllImport("dwmapi.dll", CharSet = CharSet.Auto)]
        internal static extern int DwmExtendFrameIntoClientArea(IntPtr hWnd, ref MARGINS pMarInset);

        [DllImport("dwmapi.dll", CharSet = CharSet.Auto)]
        internal static extern int DwmDefWindowProc(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam, out IntPtr result);
        #endregion

        #region Static Ole32
        [DllImport("ole32.dll", CharSet = CharSet.Auto)]
        internal static extern void CoCreateGuid(ref GUIDSTRUCT guid);
        #endregion

        #region Static Uxtheme
        [DllImport("uxtheme.dll", CharSet = CharSet.Auto)]
        internal static extern bool IsAppThemed();

        [DllImport("uxtheme.dll", CharSet = CharSet.Auto)]
        internal static extern bool IsThemeActive();

        [DllImport("uxtheme.dll", CharSet = CharSet.Auto)]
        internal static extern int SetWindowTheme(IntPtr hWnd, String subAppName, String subIdList);

        [DllImport("uxtheme.dll", CharSet = CharSet.Unicode)]
        internal static extern int DrawThemeTextEx(IntPtr hTheme, IntPtr hDC, int iPartId, int iStateId, string text, int iCharCount, int dwFlags, ref RECT pRect, ref DTTOPTS pOptions);
        #endregion

        #region Static Kernel32
        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        internal static extern short QueryPerformanceCounter(ref long var);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        internal static extern short QueryPerformanceFrequency(ref long var);
        #endregion

        #region Structures
        [StructLayout(LayoutKind.Sequential)]
        internal struct SIZE
        {
            public int cx;
            public int cy;

            public SIZE(Int32 cx, Int32 cy)
            {
                this.cx = cx;
                this.cy = cy;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct POINT
        {
            public int x;
            public int y;

            #region Constructors
            public POINT(int x, int y)
            {
                this.x = x;
                this.y = y;
            }

            public POINT(System.Drawing.Point point)
            {
                x = point.X;
                y = point.Y;
            }

            public override string ToString()
            {
                return $"x:{x} y:{y}";
            }
            #endregion
        }

        [StructLayout(LayoutKind.Sequential)]
        internal class POINTC
        {
            public int x;
            public int y;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct MARGINS
        {
            public int leftWidth;
            public int rightWidth;
            public int topHeight;
            public int bottomHeight;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct TRACKMOUSEEVENTS
        {
            public uint cbSize;
            public uint dwFlags;
            public IntPtr hWnd;
            public uint dwHoverTime;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct NCCALCSIZE_PARAMS
        {
            public RECT rectProposed;
            public RECT rectBeforeMove;
            public RECT rectClientBeforeMove;
            public int lpPos;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct WINDOWPOS
        {
            public IntPtr hwnd;
            public IntPtr hwndInsertAfter;
            public int x;
            public int y;
            public int cx;
            public int cy;
            public uint flags;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct GUIDSTRUCT
        {
            public ushort Data1;
            public ushort Data2;
            public ushort Data3;
            public ushort Data4;
            public ushort Data5;
            public ushort Data6;
            public ushort Data7;
            public ushort Data8;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct MSG
        {
            public IntPtr hwnd;
            public int message;
            public IntPtr wParam;
            public IntPtr lParam;
            public uint time;
            public POINT pt;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct DTTOPTS
        {
            public int dwSize;
            public int dwFlags;
            public int crText;
            public int crBorder;
            public int crShadow;
            public int iTextShadowType;
            public POINT ptShadowOffset;
            public int iBorderSize;
            public int iFontPropId;
            public int iColorPropId;
            public int iStateId;
            public bool fApplyOverlay;
            public int iGlowSize;
            public int pfnDrawTextCallback;
            public IntPtr lParam;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal class BITMAPINFO
        {
            public int biSize;
            public int biWidth;
            public int biHeight;
            public short biPlanes;
            public short biBitCount;
            public int biCompression;
            public int biSizeImage;
            public int biXPelsPerMeter;
            public int biYPelsPerMeter;
            public int biClrUsed;
            public int biClrImportant;
            public byte bmiColors_rgbBlue;
            public byte bmiColors_rgbGreen;
            public byte bmiColors_rgbRed;
            public byte bmiColors_rgbReserved;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct PAINTSTRUCT
        {
            private IntPtr hdc;
            public bool fErase;
            public RECT rcPaint;
            public bool fRestore;
            public bool fIncUpdate;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
            public byte[] rgbReserved;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct CHARRANGE
        {
            public int cpMin;
            public int cpMax;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct FORMATRANGE
        {
            public IntPtr hdc;
            public IntPtr hdcTarget;
            public RECT rc;
            public RECT rcPage;
            public CHARRANGE chrg;
        }
        #endregion
    }
}
