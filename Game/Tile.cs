using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuneWarz.Game
{
    class Tile
    {
        public const int NUM_COLORS = 6;

        public int Color;
        public int Owner;

        public Tile(int color)
        {
            this.Color = color;
            this.Owner = -1;
        }

        public Tile(int color, int owner)
        {
            this.Color = color;
            this.Owner = owner;
        }
    }
}
