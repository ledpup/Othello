using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reversi.Model.Evaluation
{
	public class AnalysisNodeCollection
	{
        private readonly List<AnalysisNodeBuffer> _analysisNodeBuffers;

        public AnalysisNodeCollection()
        {
            _analysisNodeBuffers = new List<AnalysisNodeBuffer>();
        }

        public void ClearBuffers()
        {
            _analysisNodeBuffers.ForEach(x => x.LowestFreeSlot = 0);
        }

        public AnalysisNodeReference AddAnalysisNode(ref AnalysisNode gameStateNode)
        {
            var buffer = GetBuffer();

            var bufferIndex = (ushort)_analysisNodeBuffers.IndexOf(buffer);

            buffer.Entries[buffer.LowestFreeSlot] = gameStateNode;
            var reference = new AnalysisNodeReference { Buffer = bufferIndex, Index = buffer.LowestFreeSlot };
            buffer.LowestFreeSlot++;

            return reference;
        }

	    private AnalysisNodeBuffer GetBuffer()
	    {
	        var buffer = _analysisNodeBuffers.FirstOrDefault(x => x.LowestFreeSlot < x.Entries.Length);
	        if (buffer == null)
	        {
	            buffer = new AnalysisNodeBuffer();
	            _analysisNodeBuffers.Add(buffer);
	        }
	        return buffer;
	    }

        public INode GetAnalysisNode(AnalysisNodeReference gameStateNodeReference)
        {
            var buffer = _analysisNodeBuffers[gameStateNodeReference.Buffer];
            return buffer.Entries[gameStateNodeReference.Index];
        }

	    public int NumberOfNodes
	    {
            get
            {
                return _analysisNodeBuffers.Sum(x => x.LowestFreeSlot);
            }
	    }
	}

    public struct AnalysisNodeReference
    {
        public ushort Buffer;
        public ushort Index;
    }
}
