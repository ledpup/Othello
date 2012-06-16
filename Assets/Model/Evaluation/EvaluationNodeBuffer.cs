namespace Othello.Model.Evaluation
{
    public class EvaluationNodeBuffer
    {
        public EvaluationNode[] Entries;
        public ushort LowestFreeSlot;
        public static int ArraySize = ushort.MaxValue;

        public EvaluationNodeBuffer()
        {
            Entries = new EvaluationNode[ArraySize];
        }
    }
}
