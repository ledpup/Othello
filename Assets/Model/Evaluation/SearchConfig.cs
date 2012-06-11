using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Reversi.Model.Evaluation;

namespace Assets.Model.Evaluation
{
    public struct SearchConfig
    {
        public int Colour;
        public int Depth;
        public int MaxDepth;
        //public float Alpha;
        //public float Beta;
        public bool UseTranspositionTable;
        public IList<INode> NodesSearched; /* Only used if we want to record which nodes were searched */

        public SearchConfig(int colour = 0, int depth = 0, int maxDepth = 5, bool useTranspositionTable = false, IList<INode> nodesSearched = null)
        {
            Colour = colour;
            Depth = depth;
            MaxDepth = maxDepth;
            UseTranspositionTable = useTranspositionTable;
            NodesSearched = nodesSearched;
            //Alpha = -SearchAlgorithms.InitialAlphaBeta;
            //Beta = SearchAlgorithms.InitialAlphaBeta;
        }
    }
}
