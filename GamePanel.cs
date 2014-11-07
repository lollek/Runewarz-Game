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
        Game.Board GameMap;
        Bitmap Tiles;

        int Offset_X;
        int Offset_Y;

        public GamePanel() {
            Assembly TempAssembly = Assembly.GetExecutingAssembly();
            Stream ImageStream = TempAssembly.GetManifestResourceStream("RuneWarz.tiles.png");
            this.Tiles = new Bitmap(ImageStream);

            this.Visible = false;
            this.Location = new System.Drawing.Point(0, 30);
            this.Size = new System.Drawing.Size(800, 600);
            this.BackColor = Color.Black;
        }

        public void StartNewGame()
        {
            this.GameMap = new Game.Board();
            this.Offset_X = (800 - (this.GameMap.BOARD_WIDTH * Game.Tile.TILE_SIZE)) / 2;
            this.Offset_Y = (600 - (this.GameMap.BOARD_HEIGHT * Game.Tile.TILE_SIZE)) / 2;
            this.Paint += new PaintEventHandler(GamePanel_Paint);

        }
        
        public void GamePanel_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            for (int y = 0; y < this.GameMap.BOARD_HEIGHT; ++y)
            {
                for (int x = 0; x < this.GameMap.BOARD_WIDTH; ++x)
                {
                    Game.Tile tile = this.GameMap.GameTiles[x + y * this.GameMap.BOARD_WIDTH];
                    if (tile != null)
                    {
                        Rectangle source = new Rectangle(
                            tile.Color * Game.Tile.TILE_SIZE, 0, 
                            Game.Tile.TILE_SIZE, Game.Tile.TILE_SIZE);
                        Rectangle destination = new Rectangle(
                            Offset_X + x * Game.Tile.TILE_SIZE, Offset_Y + y * Game.Tile.TILE_SIZE, 
                            Game.Tile.TILE_SIZE, Game.Tile.TILE_SIZE);
                        e.Graphics.DrawImage(this.Tiles, destination, source, GraphicsUnit.Pixel);
                    }
                }
            }
        }
    }
}
