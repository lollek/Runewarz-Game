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

        public void CaptureTile(int Player, int x, int y)
        {
            Tile Tile = this.GameTiles[x + y * this.BOARD_WIDTH];
            Tile.Owner = Player;
            Tile.Color = this.Players[Player].Color;
        }

        public HashSet<Tuple<int, int>> FindCapturableTiles(int Player, int Color)
        {
            HashSet<Tuple<int, int>> HoverTiles = new HashSet<Tuple<int, int>>();
            for (int y = 0; y < this.BOARD_HEIGHT; ++y)
                for (int x = 0; x < this.BOARD_WIDTH; ++x)
                {
                    Game.Tile tile = this.GameTiles[x + y * this.BOARD_WIDTH];
                    if (tile != null && tile.Owner == Player)
                        AddTileAndNeighbors(HoverTiles, Color, x, y);
                }
            return HoverTiles;
        }
        
        void AddTileAndNeighbors(HashSet<Tuple<int, int>> hoverTiles, int Color, int x, int y)
        {
            Tuple<int, int> xy = new Tuple<int,int>(x,y);
            if (hoverTiles.Contains(xy))
                return;
            if (this.GameTiles[x + y * this.BOARD_WIDTH].Owner == -1)
                hoverTiles.Add(xy);

            if (x > 0 && TileIsColor(x - 1, y, Color))
                AddTileAndNeighbors(hoverTiles, Color, x - 1, y);
            if (x < this.BOARD_WIDTH - 1 && TileIsColor(x + 1, y, Color))
                AddTileAndNeighbors(hoverTiles, Color, x + 1, y);
            if (y > 0 && TileIsColor(x, y - 1, Color))
              AddTileAndNeighbors(hoverTiles, Color, x, y - 1);
            if (y < this.BOARD_HEIGHT - 1 && TileIsColor(x, y + 1, Color))
                AddTileAndNeighbors(hoverTiles, Color, x, y + 1);
        }

        bool TileIsColor(int x, int y, int Color)
        {
            Game.Tile Tile = this.GameTiles[x + y * this.BOARD_WIDTH];
            return Tile != null && Tile.Color == Color;
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
