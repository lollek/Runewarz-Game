using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuneWarz.Game
{
    class Board
    {
        public int Width { get; private set; }
        public int Height { get; private set; }
        public int NumPlayers { get; private set; }
        public Player[] Players { get; private set; }
        public Tile[] GameTiles { get; private set; }

        /// <summary>
        /// Make a new game board
        /// # becomes playable tiles
        /// @ becomes players (played tiles)
        /// The rest becomes unplayable tiles, and are invisible
        /// </summary>
        public Board()
        {
            BoardTemplate DefaultBoard = new BoardTemplate();
            this.Width = DefaultBoard.BoardWidth;
            this.Height = DefaultBoard.BoardHeight;
            this.NumPlayers = 0;
            this.Players = new Player[Player.MAX_PLAYERS];
            this.GameTiles = new Tile[Width * Height];

            Random RNG = new Random();
            for (int y = 0; y < Height; ++y)
                for (int x = 0; x < DefaultBoard.Board[y].Length; ++x)
                {
                    switch (DefaultBoard.Board[y][x])
                    {
                        case '#': AddNewTile(RNG, x, y); break;
                        case '@': AddNewPlayer(RNG, x, y); break;
                    }
                }

            // Each player automatically captures all nearby tiles of the game color
            for (int player = 0; player < this.NumPlayers; ++player)
                TryToCapture(player, this.Players[player].Color);
        }

        public Board LoadFromState()
        {
            Sync();
            return this;
        }

        /// <summary>Get tile at position</summary>
        /// <param name="x">X coordinate on the board</param>
        /// <param name="y">Y coordinate on the board</param>
        /// <returns>The tile at the given position</returns>
        public Tile GetTile(int x, int y)
        {
            return this.GameTiles[x + y * Width];
        }

        /// <summary>
        /// Check how many tiles a player can capture if it tries to capture all tiles of one given color
        /// </summary>
        /// <param name="Player">The player to capture tiles</param>
        /// <param name="Color">The color to capture tiles of</param>
        /// <returns>The number of capturable tiles</returns>
        public List<Tuple<int, int>> GetCapturableTiles(int Player, int Color)
        {
            List<Tuple<int, int>> CapturableTiles = new List<Tuple<int, int>>();
            for (int y = 0; y < this.Height; ++y)
                for (int x = 0; x < this.Width; ++x)
                {
                    Tile tile = this.GetTile(x, y);
                    if (tile != null && tile.Owner == Player)
                        GetCapturableTiles_AddNeighbors(CapturableTiles, Color, x, y);
                }

            // Find all tiles adjacent to another adjacent tile
            for (int i = 0; i < CapturableTiles.Count; ++i)
                GetCapturableTiles_AddNeighbors(CapturableTiles, Color, CapturableTiles[i].Item1, CapturableTiles[i].Item2);

            return CapturableTiles;
        }
        void GetCapturableTiles_AddNeighbors(List<Tuple<int, int>> List, int Color, int x, int y)
        {
            if (x > 0)
                GetCapturableTiles_TryToAddTile(List, Color, x - 1, y);
            if (x < this.Width - 1)
                GetCapturableTiles_TryToAddTile(List, Color, x + 1, y);
            if (y > 0)
                GetCapturableTiles_TryToAddTile(List, Color, x, y - 1);
            if (y < this.Height - 1)
                GetCapturableTiles_TryToAddTile(List, Color, x, y + 1);
        }
        void GetCapturableTiles_TryToAddTile(List<Tuple<int, int>> List, int Color, int x, int y)
        {
            Tile Tile = this.GetTile(x, y);
            if (Tile == null || Tile.Color != Color || Tile.Owner != -1)
                return;

            Tuple<int, int> Tuple = new Tuple<int, int>(x, y);
            if (!List.Contains(Tuple))
                List.Add(Tuple);
        }

        /// <summary>
        /// Attempt for a player to capture tiles of a given color.
        /// </summary>
        /// <param name="Player">Which player to capture the tiles</param>
        /// <param name="Color">Color of tiles to capture</param>
        /// <returns>True if any tiles were captured</returns>
        public bool TryToCapture(int Player, int Color)
        {
            List<Tuple<int, int>> List = GetCapturableTiles(Player, Color);
            if (List.Count == 0)
                return false;

            // Change owner of capturable tiles to Player
            foreach (var tile in List)
                this.GetTile(tile.Item1, tile.Item2).Owner = Player;

            // Change color of all Player tiles to Color
            this.Players[Player].Color = Color;
            int NumTiles = Height * Width;
            for (int i = 0; i < NumTiles; ++i)
                if (this.GameTiles[i] != null && this.GameTiles[i].Owner == Player)
                    this.GameTiles[i].Color = Color;
            return true;
        }


        /// <summary>
        /// Allocate a new tile on the given coordinate
        /// </summary>
        /// <param name="x">X coordinate on the board</param>
        /// <param name="y">Y coordinate on the board</param>
        void AddNewTile(Random RNG, int x, int y)
        {
            GameTiles[y * Width + x] = new Tile(RNG.Next(Tile.NUM_COLORS));
        }

        /// <summary>
        /// Allocate a new tile on the given board, and allocate a new player
        /// </summary>
        /// <param name="x">X coordinate on the board</param>
        /// <param name="y">Y coordinate on the board</param>
        void AddNewPlayer(Random RNG, int x, int y)
        {
            AddNewTile(RNG, x, y);
            Tile Tile = GetTile(x, y);

            this.Players[this.NumPlayers] = new Player(Tile.Color);
            Tile.Owner = this.NumPlayers;
            this.NumPlayers++;
        }

        void Sync()
        {
            const string Filename = ".runewarzsync";

            // Make data to write
            string Header = string.Format("{0}x{1} {2}", this.Width, this.Height, 
                                string.Concat(this.Players.Select((Player) => (char)(Player == null ? ' ' : '0' + Player.Color))));
            string Board = string.Concat(this.GameTiles.Select((Tile) => (char)(
                                Tile == null ?     ' '
                             : (Tile.Owner == -1 ? '0' + Tile.Color
                             : /* default */       'A' + Tile.Owner))));

            // Write data
            using (FileStream FStream = File.Open(Filename, FileMode.Create))
            using (StreamWriter WStream = new StreamWriter(FStream, Encoding.ASCII))
            {
                WStream.WriteLine(Header);
                WStream.Write(Board);
            }
        }
    }
}
