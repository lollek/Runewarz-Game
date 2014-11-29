using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuneWarz.Game
{
    class Player
    {
        public const int MAX_PLAYERS = 4;
        public const int PLAYER_HUMAN = 0;

        public int Color = -1;
        public int NumTiles = 0;

        public Player(int Color)
        {
            this.Color = Color;
        }

        public void SetColor(int Color)
        {
            this.Color = Color;
        }
    }
}
