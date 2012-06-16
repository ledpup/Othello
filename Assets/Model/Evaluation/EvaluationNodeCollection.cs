using System.Collections.Generic;
using System.Linq;

namespace Othello.Model.Evaluation
{
	public class EvaluationNodeCollection
	{
        private readonly List<EvaluationNodeBuffer> _evaluationNodeBuffers;

        public EvaluationNodeCollection()
        {
            _evaluationNodeBuffers = new List<EvaluationNodeBuffer>();
        }

        public void ClearMemory()
        {
            _evaluationNodeBuffers.ForEach(x => x.LowestFreeSlot = 0);
            ComputerPlayer.TranspositionTable.Clear();
            GameBehaviour.Transpositions = 0;
        }

        public EvaluationNodeReference AddAnalysisNode(ref EvaluationNode evaluationNode)
        {
            var buffer = GetBuffer();

            var bufferIndex = (ushort)_evaluationNodeBuffers.IndexOf(buffer);

            buffer.Entries[buffer.LowestFreeSlot] = evaluationNode;
            var reference = new EvaluationNodeReference { Buffer = bufferIndex, Index = buffer.LowestFreeSlot };
            buffer.LowestFreeSlot++;

            return reference;
        }

	    private EvaluationNodeBuffer GetBuffer()
	    {
	        var buffer = _evaluationNodeBuffers.FirstOrDefault(x => x.LowestFreeSlot < x.Entries.Length);
	        if (buffer == null)
	        {
	            buffer = new EvaluationNodeBuffer();
	            _evaluationNodeBuffers.Add(buffer);
	        }
	        return buffer;
	    }

        public INode GetEvaluationNode(EvaluationNodeReference evaluationNodeReference)
        {
            var buffer = _evaluationNodeBuffers[evaluationNodeReference.Buffer];
            return buffer.Entries[evaluationNodeReference.Index];
        }

	    public int NumberOfNodes
	    {
            get
            {
                return _evaluationNodeBuffers.Sum(x => x.LowestFreeSlot);
            }
	    }
	}

    public struct EvaluationNodeReference
    {
        public ushort Buffer;
        public ushort Index;
    }
}
