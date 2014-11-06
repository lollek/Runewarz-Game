using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RuneWarz
{
    public partial class Runewarz : Form
    {
        // See HandleMouseDown()
        [DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();

        public Runewarz()
        {
            InitializeComponent();
        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void HandleMouseEnter(object sender, EventArgs e)
        {
            ((Label)sender).BackColor = Color.Orange;
        }

        private void HandleMouseLeave(object sender, EventArgs e)
        {
            ((Label)sender).BackColor = default(Color);
        }

        private void HandleMouseClick(object sender, MouseEventArgs e)
        {
            if (sender.Equals(this.IconClose) || sender.Equals(this.QuitButton))
            {
                Application.Exit();
            }
            else if (sender.Equals(this.IconMinimize))
            {
                this.WindowState = FormWindowState.Minimized;
            }
        }

        private void HandleMouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                // Drag the window while holding down the left mouse button
                // ReleaseCapture() releases the Window's control of the mouse
                // SendMessage() Sends a custom event that we hand craft, 
                //  in this case it's a MouseDown event on the Title Bar (which is hidden)
                const int WM_NCLBUTTONDOWN = 0x00A1; // User MouseDown Event
                const int HT_CAPTION = 0x0002;       // Title Bar
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }
    }
}
