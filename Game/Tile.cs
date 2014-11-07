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
        public const int TILE_SIZE = 15;
        public int Color;

        public Tile(int color)
        {
            this.Color = color;
        }
    }
}
