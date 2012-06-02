using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reversi.Model
{
    public struct Point
    {
        public int X, Y;

        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }
    }
}