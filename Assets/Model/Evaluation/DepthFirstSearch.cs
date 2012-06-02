using System;
using System.Collections.Generic;
using System.Linq;

namespace Reversi.Model.Evaluation
{
    public static class DepthFirstSearch
    {
        public delegate float SearchMethod(INode node, int colour = 0, int depth = 0, IList<INode> nodesSearched = null /* Only used if we want to record which nodes were searched */);
        public static SearchMethod Search { get; set; }
        public static int MaxDepth = 7;

        private static readonly float[] Sign = new[] { 1f, -1 };

        const int InitialAlphaBeta = 10000;

        private const float Minimum = -1000000;

        public static float NegaMax(INode node, int colour, int depth, IList<INode> nodesSearched = null)
        {
            if (nodesSearched != null)
                nodesSearched.Add(node);

            if (node.IsGameOver || depth == MaxDepth)
            {
                return Sign[colour] * node.Value;
            }

            ProcessNode(node, depth);

            return node.Children.Aggregate(Minimum, (current, child) => Math.Max(current, -NegaMax(child, 1 - colour, depth + 1, nodesSearched)));
        }

        private static void ProcessNode(INode node, int depth)
        {
            if (depth > MaxDepth)
                throw new ApplicationException(string.Format("Attempting to search (depth {0}) below maximum depth ({1})", depth, MaxDepth));

            // If the current node doesn't have any children, we must need to skip a turn.
            if (!node.Children.Any())
            {
                node.NextTurn();
            }
        }

        public static float AlphaBetaNegaMax(INode node, int colour, int depth, IList<INode> nodesSearched = null)
        {
            return AlphaBetaNegaMax(node, colour, depth, -InitialAlphaBeta, InitialAlphaBeta, nodesSearched);
        }

        private static float AlphaBetaNegaMax(INode node, int colour, int depth, float alpha, float beta, IList<INode> nodesSearched = null)
        {
            if (nodesSearched != null)
                nodesSearched.Add(node);

            if (node.IsGameOver || depth == MaxDepth)
                return Sign[colour] * node.Value;

            ProcessNode(node, depth);

            var bestScore = Minimum;

            foreach (var child in node.Children)
            {
                var score = -AlphaBetaNegaMax(child, 1 - colour, depth + 1, - beta, -alpha, nodesSearched);

                if (score >= beta)
                    return score;

                if (score > bestScore)
                    bestScore = score;

                if (score > alpha)
                    alpha = score;
            }
            return bestScore;
        }

        public static float NegaScout(INode node, int colour, int depth, IList<INode> nodesSearched = null)
        {
            return NegaScout(node, colour, depth, -InitialAlphaBeta, InitialAlphaBeta, nodesSearched);
        }

        private static float NegaScout(INode node, int colour, int depth, float alpha, float beta, IList<INode> nodesSearched = null)
        {
            if (nodesSearched != null)
                nodesSearched.Add(node);

            if (node.IsGameOver || depth == MaxDepth)
                return Sign[colour] * node.Value;

            ProcessNode(node, depth);

            // Order children so NegaScout can search effectively
            var orderedChildren = node.Children.OrderByDescending(x => x.Value);

            var b = beta;

            var firstChildSearched = false;
            foreach (var child in orderedChildren)
            {
                var t = -NegaScout(child, 1 - colour, depth + 1, -b, -alpha, nodesSearched);
                if ((t > alpha) && (t < beta) && firstChildSearched)
                    t = -NegaScout(child, 1 - colour, depth + 1, -beta, -alpha, nodesSearched);
              
                alpha = Math.Max(alpha, t);
              
                if ( alpha >= beta )
                    return alpha;
                
                b = alpha + 1;

                firstChildSearched = true;
            }
            return alpha;
        }

        public static short GetPlay(GameManager gameManager, Dictionary<string, float> weights)
        {
            Search = AlphaBetaNegaMax;
            MaxDepth = 7;

            var node = new GameStateNode(gameManager.GameState, weights);

            var indexesAndScores = new List<KeyValuePair<short, float>>();

            node.Children.ToList().ForEach(x =>
            {
                var score = Search(x, gameManager.PlayerIndex);
                indexesAndScores.Add(new KeyValuePair<short, float>(x.PlayIndex, score));
            });

            var min = indexesAndScores.Min(x => x.Value);
            return indexesAndScores.First(x => x.Value == min).Key;
        }

        public static void GetPlay(GameManager gameManager, Dictionary<string, float> weights, ref short? computerPlayIndex)
        {
            computerPlayIndex = GetPlay(gameManager, weights);
        }

        public static void GetPlayWithBook(GameManager gameManager, GameStateStats gameStateStats, Dictionary<string, float> weights, ref short? computerPlayIndex, ref bool computerStarted)
        {
            computerPlayIndex = BestBookPlay(gameStateStats.PlayStats, gameManager.PlayerIndex);
            if (computerPlayIndex != null)
            {
                return;
            }

            computerPlayIndex = GetPlay(gameManager, weights);
        }

        public static short? BestBookPlay(Dictionary<short, PlayStats> playStats, int playerIndex)
        {
            var bestPlay = playStats.Values
                .Where(x => x.PercentageOfGames > .001D)
                .OrderByDescending(x => x.PercentageOfWinsForBlack * Sign[playerIndex])
                .FirstOrDefault();

            if (bestPlay != null)
            {
                return playStats.First(x => x.Value == bestPlay).Key;
            }
            return null;
        }
    }
}
