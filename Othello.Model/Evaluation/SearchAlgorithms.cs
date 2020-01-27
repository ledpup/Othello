using Othello.Core;
using System;
using System.Diagnostics;
using System.Linq;

namespace Othello.Model.Evaluation
{
    public static class SearchAlgorithms
    {
        public delegate float SearchMethod(INode node, SearchConfig searchConfig, IGameController gameController, Stopwatch searchTime = null);
        private const float Minimum = -1000000;
        public const int InitialAlphaBeta = 10000;
        public static readonly float[] Sign = new[] { 1f, -1 };

        public static float NegaMax(INode node, SearchConfig config, IGameController gameController, Stopwatch searchTime = null)
        {
            if (config.NodesSearched != null)
                config.NodesSearched.Add(node);

            if (node.IsGameOver || config.Depth == config.MaxDepth || (searchTime != null && searchTime.ElapsedMilliseconds > config.MaxSearchTime))
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
				
                var score = GetScore(config.UseTranspositionTable, gameState, gameController) ?? -NegaMax(child, config, gameController, searchTime);

                AddHashToTranspositionTable(config.UseTranspositionTable, gameState, score);

                if (score > bestScore)
                    bestScore = score;
            }

            return bestScore;
        }

        public static float AlphaBetaNegaMax(INode node, SearchConfig config, IGameController gameController, Stopwatch searchTime = null)
        {
            return AlphaBetaNegaMax(node, config, -InitialAlphaBeta, InitialAlphaBeta, gameController, searchTime);
        }

        static float AlphaBetaNegaMax(INode node, SearchConfig config, float alpha, float beta, IGameController gameController, Stopwatch searchTime = null)
        {
            if (config.NodesSearched != null)
                config.NodesSearched.Add(node);

            if (node.IsGameOver || config.Depth == config.MaxDepth || (searchTime != null && searchTime.ElapsedMilliseconds > config.MaxSearchTime))
                return Sign[config.Colour] * node.Value;

            ProcessNode(node, config.Depth, config.MaxDepth);

            config.Colour = 1 - config.Colour;
            config.Depth++;

            var bestScore = Minimum;
            foreach (var child in node.Children)
            {
				var gameState = child.GameState;
				
                var score = GetScore(config.UseTranspositionTable, gameState, gameController) ?? -AlphaBetaNegaMax(child, config, -beta, -alpha, gameController, searchTime);

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
        
        private static float? GetScore(bool useTranspositionTable, GameState gameState, IGameController gameController)
        {
            if (useTranspositionTable)
            {
                if (ComputerPlayer.TranspositionTable.ContainsKey(gameState))
                {
                    gameController.Transpositions++;
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

        public static float NegaScout(INode node, SearchConfig config, IGameController gameController, Stopwatch searchTime = null)
        {
            return NegaScout(node, config, -InitialAlphaBeta, InitialAlphaBeta, searchTime);
        }

        static float NegaScout(INode node, SearchConfig config, float alpha, float beta, Stopwatch searchTime = null)
        {
            if (config.NodesSearched != null)
                config.NodesSearched.Add(node);

            if (node.IsGameOver || config.Depth == config.MaxDepth || (searchTime != null && searchTime.ElapsedMilliseconds > config.MaxSearchTime))
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
                var t = -NegaScout(child, config, -b, -alpha, searchTime);
                if ((t > alpha) && (t < beta) && firstChildSearched)
                {
                    t = -NegaScout(child, config, -beta, -alpha, searchTime);
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

            // If the current node doesn't have any children, skip a turn.
            if (!node.HasChildren)
            {
                node.NextTurn();
            }
        }
    }
}
