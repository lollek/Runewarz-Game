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

        public Tile[] GameTiles;
        public Player[] Players;
        public int NumPlayers;
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

        public void CaptureTiles(int Player, int Color)
        {
            // Change color of all Player tiles to Color
            for (int i = 0; i < BOARD_HEIGHT * BOARD_WIDTH; ++i)
                if (this.GameTiles[i] != null && this.GameTiles[i].Owner == Player)
                    this.GameTiles[i].Color = Color;

            // Change owner of capturable tiles to Player
            List<Tuple<int, int>> Tiles = FindCapturableTiles(Player, Color);
            for (int i = 0; i < Tiles.Count; ++i)
                this.GameTiles[Tiles[i].Item1 + Tiles[i].Item2 * this.BOARD_WIDTH].Owner = Player;
        }

        public List<Tuple<int,int>> FindCapturableTiles(int Player, int Color)
        {
            List<Tuple<int,int>> HoverTiles = new List<Tuple<int,int>>();
            for (int y = 0; y < this.BOARD_HEIGHT; ++y)
                for (int x = 0; x < this.BOARD_WIDTH; ++x)
                {
                    Game.Tile tile = this.GameTiles[x + y * this.BOARD_WIDTH];
                    if (tile != null && tile.Owner == Player)
                        AddNeighborsOfSameColor(HoverTiles, Color, x, y);
                }

            for (int i = 0; i < HoverTiles.Count; ++i)
                AddNeighborsOfSameColor(HoverTiles, Color, HoverTiles[i].Item1, HoverTiles[i].Item2);
            
            return HoverTiles;
        }
        
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

        void AddNewTile(int x, int y)
        {
            GameTiles[y * BOARD_WIDTH + x] = new Tile(RandomNumberGenerator.Next(Game.Tile.NUM_COLORS) + 1);
        }

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
