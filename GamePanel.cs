using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RuneWarz
{
    class GamePanel : Panel
    {
        Game.Board GameMap;

        public GamePanel() {
            this.Visible = false;
            this.Location = new System.Drawing.Point(0, 20);
            this.Size = new System.Drawing.Size(800, 600);
        }

        public void StartNewGame()
        {
            this.GameMap = new Game.Board();

        }
    }
}
