using System;
using System.Collections.Generic;
using Reversi.Model.Evaluation;

namespace Tests
{
    class TestNode : INode
    {
        public TestNode(IEnumerable<INode> children)
        {
            Children = children;
        }

        public float Value { get; set; }

        public IEnumerable<INode> Children { get; private set; }
        public bool IsGameOver { get { return false; } }
        public short PlayIndex { get; set; }

        public void NextTurn()
        {
            throw new NotImplementedException();
        }
    }
}
