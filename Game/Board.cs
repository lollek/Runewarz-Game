using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuneWarz.Game
{
    class Board
    {
        const int BOARD_WIDTH = 45;
        const int BOARD_HEIGHT = 25;
        const String board1 =
@"
            #####   #####   #####
            #####   #####   #####
 @##########################################
 #            #       #       #            #
 #            #       #       #            #
 #            #       #       #            #
#######    ####### ####### #######    #######
################## ####### #######    #######
#######    ####### ####### #######    #######
#######    ####### ####### #######    #######
#######    ####### ####### #######    #######
#######    ####### ####### #######    #######
#######    ####### ####### #######    #######
#######    ####### ####### #######    #######
#######    ####### ####### #######    #######
#######    ####### ####### #######    #######
#######    ####### ####### #######    #######
#######    ####### ####### ##################
#######    ####### ####### #######    #######
 #            #       #       #            #
 #            #       #       #            #
 #            #       #       #            #
 ##########################################@
            #####   #####   #####
            #####   #####   #####";
        Tile[] GameTiles;

        public Board()
        {
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

            for (int y = 0; y < BOARD_HEIGHT; ++y)
            {
                for (int x = 0; x < BOARD_WIDTH; ++x)
                {
                    bool GoToNextY = false;
                    switch(board1[y * BOARD_WIDTH + x])
                    {
                        case '#': 
                            GameTiles[y * BOARD_WIDTH + x] = new Tile();
                            break;
                        case '@': 
                            GameTiles[y * BOARD_WIDTH + x] = new Tile();
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
