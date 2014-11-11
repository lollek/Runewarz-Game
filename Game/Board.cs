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
        public int Width;
        public int Height;
        public int NumPlayers;
        public Player[] Players;

        Tile[] GameTiles;
        DateTime LastSync;
        bool PlayersTurn;

        /// <summary>
        /// Make a game board from a string.
        /// # becomes playable tiles
        /// @ becomes players (played tiles)
        /// The rest becomes unplayable tiles, and are invisible
        /// </summary>
        /// <param name="LoadSyncState">If true, we will attempt to load a saved game instead of starting a new</param>
        public Board(bool LoadSyncState)
        {
            BoardTemplate DefaultBoard = new BoardTemplate();
            this.Width = DefaultBoard.BoardWidth;
            this.Height = DefaultBoard.BoardHeight;
            this.NumPlayers = 0;
            this.Players = new Player[Game.Player.MAX_PLAYERS];
            this.GameTiles = new Tile[Width * Height];
            this.PlayersTurn = false;

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
            for (int Player = 0; Player < this.NumPlayers; ++Player)
            {
                List<Tuple<int, int>> Tiles = FindCapturableTiles(Player, this.Players[Player].Color);
                CaptureTiles(Tiles, Player, this.Players[Player].Color);
            }
            this.PlayersTurn = true;
        }

        /// <summary>Get tile at position</summary>
        /// <param name="x">X coordinate on the board</param>
        /// <param name="y">Y coordinate on the board</param>
        /// <returns>The tile at the given position</returns>
        public Tile GetTile(int x, int y)
        {
            return this.GameTiles[x + y * Width];
        }

        public bool AnyPlayerHasColor(int Color)
        {
            return Players.Any(Player => Player != null && Player.Color == Color);
        }

        /// <summary>
        /// Make the human player capture all nearby tiles of a given color
        /// </summary>
        /// <param name="Player">The Player to capture tiles</param>
        /// <param name="Color">The color of the tiles to capture</param>
        public void PlayerCaptureTiles(int Color)
        {
            if (!this.PlayersTurn)
                return;
            this.PlayersTurn = false;

            List<Tuple<int, int>> Tiles = FindCapturableTiles(Game.Player.PLAYER_HUMAN, Color);
            if (Tiles.Count > 0)
                CaptureTiles(Tiles, Game.Player.PLAYER_HUMAN, Color);
            else
            {
                this.PlayersTurn = true;
                return;
            }
            Tiles.Clear();

            AITakeTurn();
            this.PlayersTurn = true;
        }

        /// <summary>
        /// Make all non-human players take a turn.
        /// Returns false if no AI player could move
        /// </summary>
        public bool AITakeTurn()
        {
            bool APlayerHasMoved = false;
            for (int Player = Game.Player.PLAYER_HUMAN + 1; Player < this.NumPlayers; ++Player)
            {
                int BestColor = 0;
                List<Tuple<int, int>> BestList = null;
                for (int Color = 0; Color < Game.Tile.NUM_COLORS; ++Color)
                {
                    if (AnyPlayerHasColor(Color))
                        continue;

                    List<Tuple<int, int>> List = FindCapturableTiles(Player, Color);
                    if (BestList == null || BestList.Count < List.Count)
                    {
                        BestList = List;
                        BestColor = Color;
                    }
                }
                if (BestList.Count > 0)
                {
                    CaptureTiles(BestList, Player, BestColor);
                    APlayerHasMoved = true;
                }
                    
            }
            Sync();
            return APlayerHasMoved;
        }

        public bool PlayerCanMove()
        {
            for (int Color = 0; Color < Game.Tile.NUM_COLORS; ++Color)
                if (AnyPlayerHasColor(Color))
                    continue;
                else if (FindCapturableTiles(Game.Player.PLAYER_HUMAN, Color).Count > 0)
                    return true;
            return false;
        }

        /// <summary>
        /// Return a list of all tiles a player can capture of a color
        /// </summary>
        /// <param name="Player">The player to capture tiles</param>
        /// <param name="Color">The color of the tiles to capture</param>
        /// <returns></returns>
        public List<Tuple<int,int>> FindCapturableTiles(int Player, int Color)
        {
            // Find all tiles adjacent to the player's
            List<Tuple<int,int>> HoverTiles = new List<Tuple<int,int>>();
            for (int y = 0; y < this.Height; ++y)
                for (int x = 0; x < this.Width; ++x)
                {
                    Game.Tile tile = GetTile(x, y);
                    if (tile != null && tile.Owner == Player)
                        AddNeighborsOfSameColor(HoverTiles, Color, x, y);
                }

            // Find all tiles adjacent to another adjacent tile
            for (int i = 0; i < HoverTiles.Count; ++i)
                AddNeighborsOfSameColor(HoverTiles, Color, HoverTiles[i].Item1, HoverTiles[i].Item2);
            
            return HoverTiles;
        }

        /// <summary>
        /// Make a player capture all given tiles of a given color
        /// </summary>
        /// <param name="List">List of the tiles to capture</param>
        /// <param name="Player">Which player to capture the tiles</param>
        /// <param name="Color">Color to change player to</param>
        void CaptureTiles(List<Tuple<int, int>> List, int Player, int Color)
        {
            // Change owner of capturable tiles to Player

            for (int i = 0; i < List.Count; ++i)
                this.GetTile(List[i].Item1, List[i].Item2).Owner = Player;

            // Change color of all Player tiles to Color
            this.Players[Player].Color = Color;
            for (int i = 0; i < Height * Width; ++i)
                if (this.GameTiles[i] != null && this.GameTiles[i].Owner == Player)
                    this.GameTiles[i].Color = Color;
        }

        /// <summary>
        /// Add tiles of a given color near the given coordinates to a list
        /// </summary>
        /// <param name="hoverTiles">List to add tiles to</param>
        /// <param name="Color">The color of the tiles to look for</param>
        /// <param name="x">X coordinate on the board</param>
        /// <param name="y">Y coordinate on the board</param>
        void AddNeighborsOfSameColor(List<Tuple<int, int>> hoverTiles, int Color, int x, int y)
        {
            if (x > 0)
                TryToAddTileToCapturables(hoverTiles, Color, x - 1, y);
            if (x < this.Width - 1)
                TryToAddTileToCapturables(hoverTiles, Color, x + 1, y);
            if (y > 0)
                TryToAddTileToCapturables(hoverTiles, Color, x, y - 1);
            if (y < this.Height - 1)
                TryToAddTileToCapturables(hoverTiles, Color, x, y + 1);
        }

        /// <summary>
        /// Add Tiles to list in case it's the given color and ownerless
        /// </summary>
        /// <param name="Tiles">List to add tiles to</param>
        /// <param name="Color">Color of tiles to include</param>
        /// <param name="x">X coordinate on the board</param>
        /// <param name="y">Y coordinate on the board</param>
        void TryToAddTileToCapturables(List<Tuple<int,int>> Tiles, int Color, int x, int y)
        {
            Game.Tile Tile = GetTile(x, y);
            if (Tile == null || Tile.Color != Color || Tile.Owner != -1)
                return;

            Tuple<int,int> Tuple = new Tuple<int,int>(x,y);
            if (Tiles.Contains(Tuple))
                return;

            Tiles.Add(Tuple);
        }

        /// <summary>
        /// Allocate a new tile on the given coordinate
        /// </summary>
        /// <param name="x">X coordinate on the board</param>
        /// <param name="y">Y coordinate on the board</param>
        void AddNewTile(Random RNG, int x, int y)
        {
            GameTiles[y * Width + x] = new Tile(RNG.Next(Game.Tile.NUM_COLORS));
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
