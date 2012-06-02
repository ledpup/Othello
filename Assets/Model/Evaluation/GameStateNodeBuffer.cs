using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reversi.Model.Evaluation
{
    public class GameStateNodeBuffer
    {
        public GameStateNode[] Entries;
        int lowestFreeSlot;

        public GameStateNodeBuffer()
        {
            Entries = new GameStateNode[1000];
        }
    }
}
