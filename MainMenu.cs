using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RuneWarz
{
    class MainMenu : TableLayoutPanel
    {
        public Button NewGameButton;
        public Button LoadGameButton;
        public Button QuitButton;

        public MainMenu() 
        {
            this.NewGameButton = new Button();
            this.LoadGameButton = new Button();
            this.QuitButton = new Button();
            this.SuspendLayout();

            System.Drawing.Color Orange = Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));

            // Set Layout
            this.Anchor = ((AnchorStyles)(((AnchorStyles.Bottom | AnchorStyles.Left) | AnchorStyles.Right)));
            this.ColumnCount = 3;
            this.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33333F));
            this.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33334F));
            this.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33333F));
            this.Controls.Add(this.NewGameButton, 1, 1);
            this.Controls.Add(this.LoadGameButton, 1, 2);
            this.Controls.Add(this.QuitButton, 1, 4);
            this.Location = new Point(0, 30);
            this.Name = "GUIPanel";
            this.ForeColor = Color.White;
            this.BackColor = Color.Black;
            this.RowCount = 6;
            this.RowStyles.Add(new RowStyle(SizeType.Percent, 33.33555F));
            this.RowStyles.Add(new RowStyle(SizeType.Percent, 10.00067F));
            this.RowStyles.Add(new RowStyle(SizeType.Percent, 10.00067F));
            this.RowStyles.Add(new RowStyle(SizeType.Percent, 10.00067F));
            this.RowStyles.Add(new RowStyle(SizeType.Percent, 10.00067F));
            this.RowStyles.Add(new RowStyle(SizeType.Percent, 26.66178F));
            this.Size = new Size(800, 600);
            this.TabIndex = 0;

            // Set NewGameButton
            this.NewGameButton.Anchor = ((AnchorStyles)(((AnchorStyles.Top | AnchorStyles.Left) | AnchorStyles.Right)));
            this.NewGameButton.FlatAppearance.MouseOverBackColor = Orange;
            this.NewGameButton.FlatStyle = FlatStyle.Flat;
            this.NewGameButton.Location = new Point(269, 203);
            this.NewGameButton.Name = "NewGameButton";
            this.NewGameButton.Size = new Size(260, 23);
            this.NewGameButton.TabIndex = 0;
            this.NewGameButton.Text = "New Game";
            this.NewGameButton.UseVisualStyleBackColor = true;

            // Set LoadGameButton
            this.NewGameButton.Anchor = ((AnchorStyles)(((AnchorStyles.Top | AnchorStyles.Left) | AnchorStyles.Right)));
            this.LoadGameButton.FlatAppearance.MouseOverBackColor = Orange;
            this.LoadGameButton.FlatStyle = FlatStyle.Flat;
            this.LoadGameButton.Location = new Point(269, 263);
            this.LoadGameButton.Name = "LoadGameButton";
            this.LoadGameButton.Size = new Size(260, 23);
            this.LoadGameButton.TabIndex = 1;
            this.LoadGameButton.Text = "Load Game";
            this.LoadGameButton.UseVisualStyleBackColor = true;

            // Set QuitButton
            this.QuitButton.Anchor = ((AnchorStyles)(((AnchorStyles.Top | AnchorStyles.Left) | AnchorStyles.Right)));
            this.QuitButton.FlatAppearance.MouseOverBackColor = Orange;
            this.QuitButton.FlatStyle = FlatStyle.Flat;
            this.QuitButton.Location = new Point(269, 383);
            this.QuitButton.Name = "QuitButton";
            this.QuitButton.Size = new Size(260, 23);
            this.QuitButton.TabIndex = 3;
            this.QuitButton.Text = "Quit";
            this.QuitButton.UseVisualStyleBackColor = true;
            
            // Done
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}
