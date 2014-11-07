using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RuneWarz
{
    class TitleBar : Panel
    {
        private Form Parent;
        private Label Title;
        private Label IconClose;
        private Label IconMinimize;

        public TitleBar(Form parent) 
        {
            this.Parent = parent;
            this.Title = new Label();
            this.IconClose = new Label();
            this.IconMinimize = new Label();
            SuspendLayout();

            this.Anchor = ((AnchorStyles)((AnchorStyles.Top)));
            this.Location = new Point(0, 0);
            this.Name = "TitleBar";
            this.ForeColor = Color.White;
            this.BackColor = Color.Black;
            this.Size = new System.Drawing.Size(800, 30);
            this.Controls.Add(Title);
            this.Controls.Add(IconClose);
            this.Controls.Add(IconMinimize);

            // Title
            this.Title.Anchor = AnchorStyles.Top;
            this.Title.AutoSize = true;
            this.Title.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
            this.Title.Location = new Point(346, 4);
            this.Title.Name = "Title";
            this.Title.Size = new Size(81, 20);
            this.Title.TabIndex = 4;
            this.Title.Text = "Runewarz";

            // Close
            this.IconClose.AutoSize = true;
            this.IconClose.Location = new Point(776, 4);
            this.IconClose.Name = "IconClose";
            this.IconClose.Size = new Size(12, 13);
            this.IconClose.TabIndex = 1;
            this.IconClose.Text = "x";
            this.IconClose.MouseClick += new MouseEventHandler(HandleMouseClick);
            this.IconClose.MouseEnter += new EventHandler(MouseOverHoverStart);
            this.IconClose.MouseLeave += new EventHandler(MouseOverHoverStop);

            // Minimize
            this.IconMinimize.AutoSize = true;
            this.IconMinimize.Location = new Point(757, 4);
            this.IconMinimize.Name = "IconMinimize";
            this.IconMinimize.Size = new Size(13, 13);
            this.IconMinimize.TabIndex = 2;
            this.IconMinimize.Text = "_";
            this.IconMinimize.MouseClick += new MouseEventHandler(HandleMouseClick);
            this.IconMinimize.MouseEnter += new EventHandler(MouseOverHoverStart);
            this.IconMinimize.MouseLeave += new EventHandler(MouseOverHoverStop);

            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private void HandleMouseClick(object sender, MouseEventArgs e)
        {
            if (sender.Equals(this.IconClose))
            {
                Application.Exit();
            }
            else if (sender.Equals(this.IconMinimize))
            {
                Parent.WindowState = FormWindowState.Minimized;
            }
        }

        private void MouseOverHoverStart(object sender, EventArgs e)
        {
            ((Label)sender).BackColor = Color.Orange;
        }

        private void MouseOverHoverStop(object sender, EventArgs e)
        {
            ((Label)sender).BackColor = default(Color);
        }

    }
}
