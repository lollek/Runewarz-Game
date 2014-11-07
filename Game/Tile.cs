using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuneWarz.Game
{
    class Tile
    {
        const int NUM_COLORS = 6;
        int Color;

        public Tile()
        {
            this.Color = new System.Random().Next(NUM_COLORS);
        }
    }
}
