using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Reversi.Model.Evaluation;

namespace Reversi.Model.TranspositionTable
{
    public class HashNode
    {
        public List<AnalysisNode?> AnalysisNodes;
	}
}
