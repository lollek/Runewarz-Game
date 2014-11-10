using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuneWarz.Game
{
    class Board
    {
        public int BOARD_WIDTH = 45;
        public int BOARD_HEIGHT = 25;
        string[] board1 = new string[] { 
             "            #####   #####   #####"
            ,"            #####   #####   #####"
            ," @##########################################"
            ," #            #       #       #            #"
            ," #            #       #       #            #"
            ," #            #       #       #            #"
            ,"#######    ####### ####### #######    #######"
            ,"################## ####### #######    #######"
            ,"#######    ####### ####### #######    #######"
            ,"#######    ####### ####### #######    #######"
            ,"#######    ####### ####### #######    #######"
            ,"#######    ####### ####### #######    #######"
            ,"#######    ####### ####### #######    #######"
            ,"#######    ####### ####### #######    #######"
            ,"#######    ####### ####### #######    #######"
            ,"#######    ####### ####### #######    #######"
            ,"#######    ####### ####### #######    #######"
            ,"#######    ####### ####### ##################"
            ,"#######    ####### ####### #######    #######"
            ," #            #       #       #            #"
            ," #            #       #       #            #"
            ," #            #       #       #            #"
            ," ##########################################@"
            ,"            #####   #####   #####"
            ,"            #####   #####   #####"};

        public Player[] Players;
        public int NumPlayers;

        Tile[] GameTiles;
        Random RandomNumberGenerator;

        /// <summary>
        /// Make a game board from a string.
        /// # becomes playable tiles
        /// @ becomes players (played tiles)
        /// The rest becomes unplayable tiles, and are invisible
        /// </summary>
        public Board()
        {
            this.RandomNumberGenerator = new Random();
            this.GameTiles = new Tile[BOARD_WIDTH * BOARD_HEIGHT];
            this.Players = new Player[Game.Player.MAX_PLAYERS];
            this.NumPlayers = 0;

            for (int y = 0; y < BOARD_HEIGHT; ++y)
                for (int x = 0; x < board1[y].Length; ++x)
                {
                    switch (board1[y][x])
                    {
                        case '#': AddNewTile(x, y); break;
                        case '@': AddNewPlayer(x, y); break;
                    }
                }
        }

        public Tile GetTile(int x, int y)
        {
            return this.GameTiles[x + y * BOARD_WIDTH];
        }

        /// <summary>
        /// Make a player capture all nearby tiles of a given color
        /// </summary>
        /// <param name="Player">The Player to capture tiles</param>
        /// <param name="Color">The color of the tiles to capture</param>
        public void CaptureTiles(int Player, int Color)
        {
            // Change color of all Player tiles to Color
            this.Players[Player].Color = Color;
            for (int i = 0; i < BOARD_HEIGHT * BOARD_WIDTH; ++i)
                if (this.GameTiles[i] != null && this.GameTiles[i].Owner == Player)
                    this.GameTiles[i].Color = Color;

            // Change owner of capturable tiles to Player
            List<Tuple<int, int>> Tiles = FindCapturableTiles(Player, Color);
            for (int i = 0; i < Tiles.Count; ++i)
                this.GameTiles[Tiles[i].Item1 + Tiles[i].Item2 * this.BOARD_WIDTH].Owner = Player;
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
            for (int y = 0; y < this.BOARD_HEIGHT; ++y)
                for (int x = 0; x < this.BOARD_WIDTH; ++x)
                {
                    Game.Tile tile = this.GameTiles[x + y * this.BOARD_WIDTH];
                    if (tile != null && tile.Owner == Player)
                        AddNeighborsOfSameColor(HoverTiles, Color, x, y);
                }

            // Find all tiles adjacent to another adjacent tile
            for (int i = 0; i < HoverTiles.Count; ++i)
                AddNeighborsOfSameColor(HoverTiles, Color, HoverTiles[i].Item1, HoverTiles[i].Item2);
            
            return HoverTiles;
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
            if (x < this.BOARD_WIDTH - 1)
                TryToAddTileToCapturables(hoverTiles, Color, x + 1, y);
            if (y > 0)
                TryToAddTileToCapturables(hoverTiles, Color, x, y - 1);
            if (y < this.BOARD_HEIGHT - 1)
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
            Game.Tile Tile = this.GameTiles[x + y * this.BOARD_WIDTH];
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
        void AddNewTile(int x, int y)
        {
            GameTiles[y * BOARD_WIDTH + x] = new Tile(RandomNumberGenerator.Next(Game.Tile.NUM_COLORS) + 1);
        }

        /// <summary>
        /// Allocate a new tile on the given board, and allocate a new player
        /// </summary>
        /// <param name="x">X coordinate on the board</param>
        /// <param name="y">Y coordinate on the board</param>
        void AddNewPlayer(int x, int y)
        {
            AddNewTile(x, y);
            Tile Tile = this.GameTiles[y * BOARD_WIDTH + x];

            this.Players[this.NumPlayers] = new Player(Tile.Color);
            Tile.Owner = this.NumPlayers;
            this.NumPlayers++;
        }


    }
}
