using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reversi.Model.Evaluation
{
	public class GameStateNodeCollection
	{
        private readonly List<GameStateNodeBuffer> _gameStateNodeBuffers;

        public GameStateNodeCollection()
        {
            _gameStateNodeBuffers = new List<GameStateNodeBuffer>();
        }

        public void ClearBuffers()
        {
            _gameStateNodeBuffers.ForEach(x => x.LowestFreeSlot = 0);
        }

        public GameStateNodeReference AddGameStateNode(ref GameStateNode gameStateNode)
        {
            var buffer = GetBuffer();

            var bufferIndex = (ushort)_gameStateNodeBuffers.IndexOf(buffer);

            buffer.Entries[buffer.LowestFreeSlot] = gameStateNode;
            var reference =  new GameStateNodeReference { Buffer = bufferIndex, Index = buffer.LowestFreeSlot };
            buffer.LowestFreeSlot++;

            return reference;
        }

	    private GameStateNodeBuffer GetBuffer()
	    {
	        var buffer = _gameStateNodeBuffers.FirstOrDefault(x => x.LowestFreeSlot < x.Entries.Length);
	        if (buffer == null)
	        {
	            buffer = new GameStateNodeBuffer();
	            _gameStateNodeBuffers.Add(buffer);
	        }
	        return buffer;
	    }

	    public INode GetGameStateNode(GameStateNodeReference gameStateNodeReference)
        {
            var buffer = _gameStateNodeBuffers[gameStateNodeReference.Buffer];
            return buffer.Entries[gameStateNodeReference.Index];
        }

	    public int NumberOfNodes
	    {
            get
            {
                return (_gameStateNodeBuffers.Count - 1) * GameStateNodeBuffer.ArraySize +
                       _gameStateNodeBuffers[_gameStateNodeBuffers.Count - 1].Entries.Length;
            }
	    }
	}

    public struct GameStateNodeReference
    {
        public ushort Buffer;
        public ushort Index;
    }
}
