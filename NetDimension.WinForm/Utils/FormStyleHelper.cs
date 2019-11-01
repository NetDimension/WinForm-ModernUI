using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NetDimension.WinForm
{
    public static class FormStyleHelper
    {
        public static bool IsWindows8OrLower
        {
            get { return (Environment.OSVersion.Version.Major < 6 || (Environment.OSVersion.Version.Major == 6 && Environment.Version.Minor < 3)); }
        }

        /// <summary>
        /// Gets the size of the borders requested by the real window.
        /// </summary>
        /// <param name="cp">Window style parameters.</param>
        /// <returns>Border sizing.</returns>
        public static Padding GetWindowBorders(CreateParams cp)
        {
            Win32.RECT rect = new Win32.RECT();

            // Start with a zero sized rectangle
            rect.left = 0;
            rect.right = 0;
            rect.top = 0;
            rect.bottom = 0;


            // Adjust rectangle to add on the borders required
            Win32.AdjustWindowRectEx(ref rect, cp.Style, false, cp.ExStyle);

            // Return the per side border values
            return new Padding(-rect.left, -rect.top, rect.right, rect.bottom);
        }

        public static Size ScaleSize(Size value, SizeF scaleFactor)
        {
            return new Size(
                (int)Math.Round(value.Width * scaleFactor.Width, MidpointRounding.AwayFromZero),
                (int)Math.Round(value.Height * scaleFactor.Height, MidpointRounding.AwayFromZero));
        }

        public static Padding ScalePadding(Padding value, SizeF scaleFactor)
        {
            return new Padding(
                (int)Math.Round(value.Left * scaleFactor.Width, MidpointRounding.AwayFromZero),
                (int)Math.Round(value.Top * scaleFactor.Height, MidpointRounding.AwayFromZero),
                (int)Math.Round(value.Right * scaleFactor.Width, MidpointRounding.AwayFromZero),
                (int)Math.Round(value.Bottom * scaleFactor.Height, MidpointRounding.AwayFromZero));
        }

        public static Padding GetWindowRealNCMargin(Form f, CreateParams cp)
        {
            Win32.RECT boundsRect = new Win32.RECT();

            Rectangle screenClient;

            if (f.IsHandleCreated)
            {
                // RectangleToScreen will force handle creation and we don't want this during Form.ScaleControl
                screenClient = f.RectangleToScreen(f.ClientRectangle);
            }
            else
            {
                screenClient = f.ClientRectangle;
                screenClient.Offset(-f.Location.X, -f.Location.Y);
            }

            boundsRect.left = screenClient.Left;
            boundsRect.top = screenClient.Top;
            boundsRect.right = screenClient.Right;
            boundsRect.bottom = screenClient.Bottom;
            var hasMenu = f.MainMenuStrip != null;

            Win32.AdjustWindowRectEx(ref boundsRect, cp.Style, hasMenu, cp.ExStyle);

            Padding result = Padding.Empty;

            result = new Padding(screenClient.Left - boundsRect.left, screenClient.Top - boundsRect.top, boundsRect.right - screenClient.Right, boundsRect.bottom - screenClient.Bottom);

            return result;


        }


        /// <summary>
        /// Discover if the provided Form is currently maximized.
        /// </summary>
        /// <param name="f">Form reference.</param>
        /// <returns>True if maximized; otherwise false.</returns>
        public static bool IsFormMaximized(Form f)
        {
            // Get the current window style (cannot use the 
            // WindowState property as it can be slightly out of date)
            uint style = Win32.GetWindowLong(f.Handle, Win32.GWL_STYLE);

            return ((style &= Win32.WS_MAXIMIZE) != 0);
        }


        /// <summary>
        /// Discover if the provided Form is currently minimized.
        /// </summary>
        /// <param name="f">Form reference.</param>
        /// <returns>True if minimized; otherwise false.</returns>
        public static bool IsFormMinimized(Form f)
        {
            // Get the current window style (cannot use the 
            // WindowState property as it can be slightly out of date)
            uint style = Win32.GetWindowLong(f.Handle, Win32.GWL_STYLE);

            return ((style &= Win32.WS_MINIMIZE) != 0);
        }

        /// <summary>
        /// Gets the real client rectangle of the list.
        /// </summary>
        /// <param name="handle">Window handle of the control.</param>
        public static Rectangle RealClientRectangle(IntPtr handle)
        {
            // Grab the actual current size of the window, this is more accurate than using
            // the 'this.Size' which is out of date when performing a resize of the window.
            Win32.RECT windowRect = new Win32.RECT();
            Win32.GetWindowRect(handle, ref windowRect);

            // Create rectangle that encloses the entire window
            return new Rectangle(0, 0,
                                 windowRect.right - windowRect.left,
                                 windowRect.bottom - windowRect.top);
        }


    }
}
