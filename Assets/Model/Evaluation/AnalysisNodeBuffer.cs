namespace Othello.Model.Evaluation
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
