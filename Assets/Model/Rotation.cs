using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Othello.Model
{
    public class Rotation
    {
        public Dictionary<short, short> IndicesMap;
        public Func<ulong, ulong> Function;

        public Rotation(Func<ulong, ulong> function, Dictionary<short, short> indicesMap)
        {
            Function = function;
            IndicesMap = indicesMap;
        }
    }
}
