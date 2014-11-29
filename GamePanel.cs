using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RuneWarz
{
    class GamePanel : PictureBox
    {
        const int TILESIZE = 15;
        const int TILE_OWNERLESS = 0;
        const int TILE_PADLOCK = 1;
        const int TILE_PLAYER1 = 2;
        const int TILE_PLAYER2 = 3;
        const int TILE_PLAYER3 = 4;
        const int TILE_PLAYER4 = 5;

        Bitmap Tiles;
        Game.Game Game;

        int LastHoverColor = -1;
        int CurrentHoverColor = -1;
        int Offset_X = -1;
        int Offset_Y = -1;

        public GamePanel() {
            Assembly TempAssembly = Assembly.GetExecutingAssembly();
            Stream ImageStream = TempAssembly.GetManifestResourceStream("RuneWarz.tiles.png");
            this.Tiles = new Bitmap(ImageStream);
            this.Game = new Game.Game(this);

            this.Visible = false;
            this.Location = new System.Drawing.Point(0, 30);
            this.Size = new System.Drawing.Size(800, 600);
            this.BackColor = Color.Black;

            this.Enabled = true;
            this.MouseMove += GamePanel_MouseMove;
            this.MouseDown += GamePanel_MouseClick;
            this.Paint += Paint_GamePanel;
        }

        /// <summary>
        /// Reset the map to make it ready for a new game
        /// </summary>
        public void StartNewGame()
        {
            Game.NewGame();
            this.Offset_X = (800 - (this.Game.BoardWidth() * TILESIZE)) / 2;
            this.Offset_Y = (600 - (this.Game.BoardHeight() * TILESIZE)) / 2;
            this.Invalidate();
        }
        public void ResumeGame()
        {
            Game.ResumeGame();
            this.Offset_X = (800 - (this.Game.BoardWidth() * TILESIZE)) / 2;
            this.Offset_Y = (600 - (this.Game.BoardHeight() * TILESIZE)) / 2;
            this.Invalidate();
        }

        void GamePanel_MouseMove(object sender, MouseEventArgs e)
        {
            if (Offset_X == -1 || Offset_Y == -1)
                return;

            int Board_X = (e.X - Offset_X) / TILESIZE;
            int Board_Y = (e.Y - Offset_Y) / TILESIZE;
            if (0 < Board_X && Board_X < this.Game.BoardWidth() &&
                0 < Board_Y && Board_Y < this.Game.BoardHeight())
            {
                LastHoverColor = CurrentHoverColor;
                CurrentHoverColor = Game.GetColorOfTile(Board_X, Board_Y);
                if (LastHoverColor != CurrentHoverColor)
                    this.Invalidate();
            }
        }

        void GamePanel_MouseClick(object sender, MouseEventArgs e)
        {
            if (!this.Visible || CurrentHoverColor == -1)
                return;

            if (this.Game.TakeTurn(CurrentHoverColor))
                this.Invalidate();
        }
        
        /// <summary>
        /// Main function for redrawing graphics
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Paint_GamePanel(object sender, PaintEventArgs e)
        {
            Paint_OwnedTiles(e);
            Paint_Score(e);

            if (this.Game.GameIsOver)
                Paint_GameOver(e);
            else if (LastHoverColor != CurrentHoverColor)
                Paint_Hover(e);
        }

        void Paint_OwnedTiles(PaintEventArgs e)
        {
            for (int y = 0; y < this.Game.BoardHeight(); ++y)
                for (int x = 0; x < this.Game.BoardWidth(); ++x)
                {
                    int Color = Game.GetColorOfTile(x, y);
                    int Owner = Game.GetOwnerOfTile(x, y);
                    Paint_Tile(x, y, Owner == -1 ? TILE_OWNERLESS : TILE_PLAYER1 + Owner, Color, e);
                }
        }
        void Paint_Score(PaintEventArgs e)
        {
            int x = 50;
            int start_y = this.Height - 75;
            int NumPlayers = this.Game.GetNumPlayers();
            Font Font = new Font("Microsoft Sans Serif", 10, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));

            for (int player = 0; player < NumPlayers; ++player)
            {
                int Color = this.Game.GetColorOfPlayer(player);
                int NumTiles = this.Game.GetNumTilesOfPlayer(player);
                int imageType = TILE_PLAYER1 + player;
                int y = start_y + (player + 1)*TILESIZE;

                for (int xi = 0; xi < NumTiles; ++xi)
                {
                    Rectangle source = new Rectangle((Color + 1) * TILESIZE, imageType * TILESIZE, TILESIZE, TILESIZE);
                    Rectangle destination = new Rectangle(x + xi, y, TILESIZE, TILESIZE);
                    e.Graphics.DrawImage(this.Tiles, destination, source, GraphicsUnit.Pixel);
                }
                e.Graphics.DrawString(NumTiles.ToString(), Font, Brushes.White, new Point(x + NumTiles + TILESIZE, y));
                e.Graphics.DrawString((player + 1).ToString(), Font, Brushes.White, new Point(x - 20, y));
            }

            Font.Dispose();
        }
        void Paint_GameOver(PaintEventArgs e)
        {
            int winner = -1;
            int best = -1;
            for (int i = 0; i < this.Game.GetNumPlayers(); ++i)
            {
                int current = this.Game.GetNumTilesOfPlayer(i);
                if (current <= best)
                    continue;
                winner = i;
                best = current;
            }
            string WinString = String.Format("Player {0} has won!", winner + 1);

            Font Font = new Font("Microsoft Sans Serif", 16, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
            e.Graphics.DrawString(WinString, Font, Brushes.White, new Point(this.Width / 2 - WinString.Length*5, 20));
            Font.Dispose();
        }
        void Paint_Hover(PaintEventArgs e)
        {
            /* Second iteration - Print hover */
            int Print_Tile_Type = TILE_PLAYER1;
            if (this.Game.AnyPlayerHasColor(CurrentHoverColor))
                Print_Tile_Type = TILE_PADLOCK;

            foreach (var tile in this.Game.GetCapturableTiles(this.Game.HumanPlayer(), CurrentHoverColor))
            {
                int x = tile.Item1;
                int y = tile.Item2;
                Paint_Tile(x, y, Print_Tile_Type, this.Game.GetColorOfTile(x, y), e);
            }
        }

        /// <summary>
        /// Helper function to draw a single Tile
        /// </summary>
        /// <param name="tile">Tile to draw</param>
        /// <param name="x">X coordinate on the board</param>
        /// <param name="y">Y coordinate on the board</param>
        /// <param name="imageType">Kind of image to print</param>
        /// <param name="e">A PaintEventArgs</param>
        void Paint_Tile(int x, int y, int imageType, int Color, PaintEventArgs e)
        {
            Rectangle source = new Rectangle((Color + 1) * TILESIZE, imageType * TILESIZE, TILESIZE, TILESIZE);
            Rectangle destination = new Rectangle(Offset_X + x * TILESIZE, Offset_Y + y * TILESIZE, TILESIZE, TILESIZE);
            e.Graphics.DrawImage(this.Tiles, destination, source, GraphicsUnit.Pixel);
        }
    }
}
