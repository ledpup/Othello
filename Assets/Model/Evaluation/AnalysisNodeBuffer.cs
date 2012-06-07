using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reversi.Model.Evaluation
{
    public class AnalysisNodeBuffer
    {
        public AnalysisNode[] Entries;
        public ushort LowestFreeSlot;
        public static int ArraySize = ushort.MaxValue;

        public AnalysisNodeBuffer()
        {
            Entries = new AnalysisNode[ArraySize];
        }
    }
}
