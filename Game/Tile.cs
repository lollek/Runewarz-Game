using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuneWarz.Game
{
    class Tile
    {
        public const int TILE_TYPE_NONE = 0;
        public const int TILE_TYPE_LOCK = 1;
        public const int TILE_TYPE_PLA1 = 2;
        public const int TILE_TYPE_PLA2 = 3;
        public const int TILE_TYPE_PLA3 = 4;
        public const int TILE_TYPE_PLA4 = 5;
        public const int NUM_COLORS = 6;
        public const int TILE_SIZE = 15;
        public int Color;

        public Tile(int color)
        {
            this.Color = color;
        }
    }
}
