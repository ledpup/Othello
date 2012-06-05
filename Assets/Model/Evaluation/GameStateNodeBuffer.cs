using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reversi.Model.Evaluation
{
    public class GameStateNodeBuffer
    {
        public GameStateNode[] Entries;
        public ushort LowestFreeSlot;
        public static int ArraySize = ushort.MaxValue;

        public GameStateNodeBuffer()
        {
            Entries = new GameStateNode[ArraySize];
        }
    }
}
