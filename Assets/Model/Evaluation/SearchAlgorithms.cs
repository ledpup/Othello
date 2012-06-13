using System;
using System.Linq;

namespace Othello.Model.Evaluation
{
    public static class SearchAlgorithms
    {
        public delegate float SearchMethod(INode node, SearchConfig searchConfig);
        private const float Minimum = -1000000;
        public const int InitialAlphaBeta = 10000;
        public static readonly float[] Sign = new[] { 1f, -1 };

        public static float NegaMax(INode node, SearchConfig config)
        {
            if (config.NodesSearched != null)
                config.NodesSearched.Add(node);

            if (node.IsGameOver || config.Depth == config.MaxDepth)
            {
                return Sign[config.Colour] * node.Value;
            }

            ProcessNode(node, config.Depth, config.MaxDepth);

            config.Colour = 1 - config.Colour;
            config.Depth++;

            var bestScore = Minimum;
            foreach (var child in node.Children)
            {
				var gameState = child.GameState;
				
                var score = GetScore(config.UseTranspositionTable, gameState) ?? -NegaMax(child, config);

                AddHashToTranspositionTable(config.UseTranspositionTable, gameState, score);

                if (score > bestScore)
                    bestScore = score;
            }

            return bestScore;
        }

        public static float AlphaBetaNegaMax(INode node, SearchConfig config)
        {
            return AlphaBetaNegaMax(node, config, -InitialAlphaBeta, InitialAlphaBeta);
        }

        static float AlphaBetaNegaMax(INode node, SearchConfig config, float alpha, float beta)
        {
            if (config.NodesSearched != null)
                config.NodesSearched.Add(node);

            if (node.IsGameOver || config.Depth == config.MaxDepth)
                return Sign[config.Colour] * node.Value;

            ProcessNode(node, config.Depth, config.MaxDepth);

            config.Colour = 1 - config.Colour;
            config.Depth++;

            var bestScore = Minimum;
            foreach (var child in node.Children)
            {
				var gameState = child.GameState;
				
                var score = GetScore(config.UseTranspositionTable, gameState) ?? -AlphaBetaNegaMax(child, config, -beta, -alpha);

                AddHashToTranspositionTable(config.UseTranspositionTable, gameState, score);

                if (score >= beta)
                    return score;

                if (score > bestScore)
                    bestScore = score;

                if (score > alpha)
                    alpha = score;
            }
            return bestScore;
        }
        
        private static float? GetScore(bool useTranspositionTable, GameState gameState)
        {
            if (useTranspositionTable)
            {
                if (ComputerPlayer.TranspositionTable.ContainsKey(gameState))
                {
                    GameBehaviour.Transpositions++;
                    return ComputerPlayer.TranspositionTable[gameState];
                }
                return null;
            }
            return null;
        }

        private static void AddHashToTranspositionTable(bool useTranspositionTable, GameState gameState, float score)
        {
            if (useTranspositionTable)
                if (!ComputerPlayer.TranspositionTable.ContainsKey(gameState))
                    ComputerPlayer.TranspositionTable.Add(gameState, score);
        }

        public static float NegaScout(INode node, SearchConfig config)
        {
            return NegaScout(node, config, -InitialAlphaBeta, InitialAlphaBeta);
        }

        static float NegaScout(INode node, SearchConfig config, float alpha, float beta)
        {
            if (config.NodesSearched != null)
                config.NodesSearched.Add(node);

            if (node.IsGameOver || config.Depth == config.MaxDepth)
                return Sign[config.Colour] * node.Value;

            ProcessNode(node, config.Depth, config.MaxDepth);

            // Order children so NegaScout can search effectively
			var orderedChildren = node.Children.OrderByDescending(x => x.Value);

            var b = beta;

            config.Colour = 1 - config.Colour;
            config.Depth++;

            var firstChildSearched = false;
            foreach (var child in orderedChildren)
            {
                var t = -NegaScout(child, config, -b, -alpha);
                if ((t > alpha) && (t < beta) && firstChildSearched)
                {
                    t = -NegaScout(child, config, -beta, -alpha);
                }

                alpha = Math.Max(alpha, t);

                if (alpha >= beta)
                    return alpha;

                b = alpha + 1;

                firstChildSearched = true;
            }
            return alpha;
        }


        private static void ProcessNode(INode node, int depth, int maxDepth)
        {
            if (depth > maxDepth)
                throw new ApplicationException(string.Format("Attempting to search below maximum depth (depth {0} > max depth {1})", depth, maxDepth));

            // If the current node doesn't have any children, we must need to skip a turn.
            if (!node.HasChildren)
            {
                node.NextTurn();
            }
        }
    }
}
