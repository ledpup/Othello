using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Othello.Model
{
    public abstract class SpacialObject
    {
        public Point Location;
        public int X { get { return Location.X; } }
        public int Y { get { return Location.Y; } }
		public short Index { get { return (short)(Location.X + Location.Y * 8); } }

        public SpacialObject(int x, int y)
            : this(new Point(x, y))
        {
        }

        public SpacialObject(Point location)
        {
            Location = location;
        }

        public override string ToString()
        {
            return string.Format("[{0}, {1}]", X, Y);
        }
    }
}