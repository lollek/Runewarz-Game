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

        int LastHoverColor = -1;
        int CurrentHoverColor = -1;
        int Offset_X = -1;
        int Offset_Y = -1;

        public GamePanel() {
            Assembly TempAssembly = Assembly.GetExecutingAssembly();
            Stream ImageStream = TempAssembly.GetManifestResourceStream("RuneWarz.tiles.png");
            this.Tiles = new Bitmap(ImageStream);

            this.Visible = false;
            this.Location = new System.Drawing.Point(0, 30);
            this.Size = new System.Drawing.Size(800, 600);
            this.BackColor = Color.Black;

            this.MouseMove += GamePanel_MouseMove;
            this.Paint += Paint_GamePanel;
        }

        public void StartNewGame()
        {
            this.GameMap = new Game.Board();
            this.Offset_X = (800 - (this.GameMap.BOARD_WIDTH * Game.Tile.TILE_SIZE)) / 2;
            this.Offset_Y = (600 - (this.GameMap.BOARD_HEIGHT * Game.Tile.TILE_SIZE)) / 2;

            // Each player automatically captures all nearby tiles of the game color
            for (int player = 0; player < this.GameMap.NumPlayers; ++player)
            {
                CurrentHoverColor = this.GameMap.Players[player].Color;
                HashSet<Tuple<int, int>> Capturables = FindCurrentHoverTiles(player);
                foreach (var tile in Capturables)
                    this.GameMap.CaptureTile(player, tile.Item1, tile.Item2);
            }
            CurrentHoverColor = -1;
            this.Invalidate();
        }

        void GamePanel_MouseMove(object sender, MouseEventArgs e)
        {
            if (Offset_X == -1 || Offset_Y == -1)
                return;
            int Board_X = (e.X - Offset_X) / Game.Tile.TILE_SIZE;
            int Board_Y = (e.Y - Offset_Y) / Game.Tile.TILE_SIZE;
            if (0 < Board_X && Board_X < this.GameMap.BOARD_WIDTH &&
                0 < Board_Y && Board_Y < this.GameMap.BOARD_HEIGHT)
            {
                Game.Tile Tile = this.GameMap.GameTiles[Board_X + Board_Y * this.GameMap.BOARD_WIDTH];
                LastHoverColor = CurrentHoverColor;
                CurrentHoverColor = Tile == null ? -1 : Tile.Color;
                if (LastHoverColor != CurrentHoverColor)
                    this.Invalidate();
            }
        }

        
        void Paint_GamePanel(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            /* First Iteration - Print ownership */
            for (int y = 0; y < this.GameMap.BOARD_HEIGHT; ++y)
                for (int x = 0; x < this.GameMap.BOARD_WIDTH; ++x)
                {
                    Game.Tile tile = this.GameMap.GameTiles[x + y * this.GameMap.BOARD_WIDTH];
                    if (tile == null)
                        continue;
                    else if (tile.Owner != -1)
                        Paint_Tile(tile, x, y, Game.Tile.TILE_TYPE_PLA1 + tile.Owner, e);
                    else
                        Paint_Tile(tile, x, y, Game.Tile.TILE_TYPE_NONE, e);
                }

            if (LastHoverColor == CurrentHoverColor)
                return;

            /* Second iteration - Print hover */
            HashSet<Tuple<int, int>> HoverTiles = FindCurrentHoverTiles(Game.Player.PLAYER_HUMAN);
            foreach (var tile in HoverTiles)
                Paint_Tile(this.GameMap.GameTiles[tile.Item1 + tile.Item2 * this.GameMap.BOARD_WIDTH], 
                           tile.Item1, tile.Item2, Game.Tile.TILE_TYPE_PLA1, e);
        }

        void Paint_Tile(Game.Tile tile, int x, int y, int imageType, PaintEventArgs e)
        {
            const int tile_size = Game.Tile.TILE_SIZE;
            Rectangle source = new Rectangle(tile.Color * tile_size, imageType * tile_size, tile_size, tile_size);
            Rectangle destination = new Rectangle(Offset_X + x * tile_size, Offset_Y + y * tile_size, tile_size, tile_size);
            e.Graphics.DrawImage(this.Tiles, destination, source, GraphicsUnit.Pixel);
        }

        HashSet<Tuple<int, int>> FindCurrentHoverTiles(int Player)
        {
            HashSet<Tuple<int, int>> HoverTiles = new HashSet<Tuple<int, int>>();
            for (int y = 0; y < this.GameMap.BOARD_HEIGHT; ++y)
                for (int x = 0; x < this.GameMap.BOARD_WIDTH; ++x)
                {
                    Game.Tile tile = this.GameMap.GameTiles[x + y * this.GameMap.BOARD_WIDTH];
                    if (tile == null)
                        continue;
                    else if (tile.Owner == Player)
                        AddHoverTiles(HoverTiles, x, y);
                }
            return HoverTiles;
        }

        void AddHoverTiles(HashSet<Tuple<int, int>> hoverTiles, int x, int y)
        {
            Tuple<int, int> xy = new Tuple<int,int>(x,y);
            if (hoverTiles.Contains(xy))
                return;
            hoverTiles.Add(xy);

            if (x > 0 && Tile_IsHoverColor(x - 1, y))
                AddHoverTiles(hoverTiles, x - 1, y);
            if (x < this.GameMap.BOARD_WIDTH - 1 && Tile_IsHoverColor(x + 1, y))
                AddHoverTiles(hoverTiles, x + 1, y);
            if (y > 0 && Tile_IsHoverColor(x, y - 1))
              AddHoverTiles(hoverTiles, x, y - 1);
            if (y < this.GameMap.BOARD_HEIGHT - 1 && Tile_IsHoverColor(x, y + 1))
                AddHoverTiles(hoverTiles, x, y + 1);
        }

        bool Tile_IsHoverColor(int x, int y)
        {
            Game.Tile Tile = this.GameMap.GameTiles[x + y * this.GameMap.BOARD_WIDTH];
            return Tile != null && Tile.Color == CurrentHoverColor;
        }
    }
}
