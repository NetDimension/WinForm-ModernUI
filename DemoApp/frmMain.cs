using NetDimension.WinForm;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DemoApp
{
	public partial class frmMain : ModernUIForm
	{
		[DllImport("user32.dll")]
		public static extern bool ReleaseCapture();
		[DllImport("user32.dll")]
		public static extern bool SendMessage(IntPtr hwnd, int wMsg, int wParam, int lParam);
		public const int WM_SYSCOMMAND = 0x0112;
		public const int SC_MOVE = 0xF010;
		public const int HTCAPTION = 0x0002;

		public frmMain()
		{
			InitializeComponent();
			//this.FormBorderStyle = FormBorderStyle.Sizable;
			//this.WindowState = FormWindowState.Maximized;
			//this.StartPosition = FormStartPosition.CenterScreen;
		}

		private void button1_Click(object sender, EventArgs e)
		{
			this.Close();
		}

		private void label1_MouseDown(object sender, MouseEventArgs e)
		{
			ReleaseCapture();
			SendMessage(this.Handle, WM_SYSCOMMAND, SC_MOVE + HTCAPTION, 0);
		}

		private void button3_Click(object sender, EventArgs e)
		{
			frmAbout AboutForm = new frmAbout();
			AboutForm.ShowDialog(this);

		}

		private void button2_Click(object sender, EventArgs e)
		{
            Process.Start("https://github.com/NetDimension/WinForm-ModernUI");

            //this.Scale(new SizeF(1.25f, 1.25f));
		}

        private void button4_Click(object sender, EventArgs e)
        {
            this.WindowState = (this.WindowState == FormWindowState.Normal ? FormWindowState.Maximized : FormWindowState.Normal);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;

        }

        private void button5_Click(object sender, EventArgs e)
        {
            this.Close();
        }


    }
}
