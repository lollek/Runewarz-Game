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
        Random RandomNumberGenerator;

        public Board()
        {
            RandomNumberGenerator = new Random();
            Contructor_MakeBoard();
        }

        /// <summary>
        /// Make a game board from a string.
        /// # becomes playable tiles
        /// @ becomes players (played tiles)
        /// The rest becomes unplayable tiles, and are invisible
        /// </summary>
        private void Contructor_MakeBoard()
        {
            this.GameTiles = new Tile[BOARD_WIDTH * BOARD_HEIGHT];
            System.Diagnostics.Debug.WriteLine(board1);

            for (int y = 0; y < BOARD_HEIGHT; ++y)
            {
                for (int x = 0; x < board1[y].Length; ++x)
                {
                    bool GoToNextY = false;
                    switch(board1[y][x])
                    {
                        case '#': 
                            GameTiles[y * BOARD_WIDTH + x] = new Tile(RandomNumberGenerator.Next(Game.Tile.NUM_COLORS) +1);
                            break;
                        case '@':
                            GameTiles[y * BOARD_WIDTH + x] = new Tile(RandomNumberGenerator.Next(Game.Tile.NUM_COLORS) + 1);
                            break;
                        case '\r': 
                        case '\n': 
                            GoToNextY = true;
                            break;
                    }
                    if (GoToNextY)
                    {
                        break;
                    }
                }
            }
        }
    }
}
