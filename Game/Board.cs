using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

        const string Filename = "RuneWarz.sync";

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

            Sync();
        }

        public static Board LoadFromState()
        {
            if (File.Exists(Filename))
                return new Board(true);
            else
                return new Board();
        }
        Board(bool sync) { if (sync) { Sync(); } }

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

        public void Sync()
        {
            // Load from state
            if (this.GameTiles == null)
            {
                using (FileStream FStream = File.OpenRead(Filename))
                using (StreamReader RStream = new StreamReader(FStream, Encoding.ASCII))
                {
                    Sync_StringToHeader(RStream.ReadLine());
                    Sync_StringToBoard(RStream.ReadLine());
                }
            }
            // Save to state
            else
            {
                // Write data
                using (FileStream FStream = File.Open(Filename, FileMode.Create))
                using (StreamWriter WStream = new StreamWriter(FStream, Encoding.ASCII))
                {
                    WStream.WriteLine(Sync_HeaderToString());
                    WStream.Write(Sync_BoardToString());
                }
            }
        }
        /// <summary>
        /// Creates a Header string "%dx%d %d+" where 
        /// %dx%d is the width and height of the board. e.g. 45x25
        /// %d+ is the color of existing players. e.g. 25   would be that there are two players, 
        ///   and player 1 has color 2, while player 2 has color 5
        /// </summary>
        /// <returns>String that can be saved to file</returns>
        string Sync_HeaderToString()
        {
            return string.Format("{0}x{1} {2}", this.Width, this.Height,
                     string.Concat(this.Players.Select((Player) =>
                            (char)(Player == null ? ' ' : '0' + Player.Color))));
        }
        void Sync_StringToHeader(string Header)
        {
            Match match = new Regex(@"^(\d+)x(\d+)\s(\d+)\s*$").Match(Header);
            this.Width = int.Parse(match.Groups[1].ToString());
            this.Height = int.Parse(match.Groups[2].ToString());
            this.NumPlayers = match.Groups[3].ToString().Length;
            this.Players = match.Groups[3].ToString()
                           .Where((num) => num != ' ')
                           .Select((num) => new Player(num - '0'))
                           .ToArray();
        }

        /// <summary>
        /// Creates a Board string, which is the board array converted to an ascii string which 1 char representing each tile
        /// A space means that there is nothing there (null, black in the game)
        /// A number means that it's not captured by a player, and the number represents its color
        /// A letter means that it's captured by a player, A = player 1, B = player2 and so on.
        /// </summary>
        /// <returns>String that can be saved to file</returns>
        string Sync_BoardToString()
        {
            return string.Concat(this.GameTiles.Select((Tile) => (char)(
                                        Tile == null ? ' '
                                 : (Tile.Owner == -1 ? '0' + Tile.Color
                                 : /* Has owner */     'A' + Tile.Owner))));
        }
        void Sync_StringToBoard(string Board)
        {
            this.GameTiles = 
                Board.Select((tile) =>
                    tile == ' ' ? null
                    :new Tile(('0' <= tile && tile <= '9') ? tile - '0' : this.Players[tile - 'A'].Color,
                              ('0' <= tile && tile <= '9') ? -1   : tile - 'A'))
                .ToArray();
        }
    }
}
