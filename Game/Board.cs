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
