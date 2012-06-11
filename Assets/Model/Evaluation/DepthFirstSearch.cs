using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Model.Evaluation;

namespace Reversi.Model.Evaluation
{
    public class DepthFirstSearch
    {
        public static AnalysisNodeCollection AnalysisNodeCollection
        {
            get { return _analysisNodeCollection ?? (_analysisNodeCollection = new AnalysisNodeCollection()); }
        }
        private static AnalysisNodeCollection _analysisNodeCollection;

        public short GetPlay(GameManager gameManager, ComputerPlayer computerPlayer)
        {
            var node = new AnalysisNode(ref gameManager.GameState, computerPlayer.GetWeights(gameManager.TurnExcludingPasses));

            var indexesAndScores = new List<KeyValuePair<short, float>>();

            //node.ChildNodeReferences.ForEach(x =>
            //                                  {
            //                                      var child = AnalysisNodeCollection.GetAnalysisNode(x);
            //                                      var score = Search(ref child, gameManager.PlayerIndex);
            //                                      indexesAndScores.Add(new KeyValuePair<short, float>(child.PlayIndex, score));
            //                                  });

            var maxDepth = computerPlayer.GetSearchDepth(gameManager.TurnExcludingPasses);
			
            node.Children.ToList().ForEach(x =>
            {
                var score = computerPlayer.Search(x, new SearchConfig(gameManager.PlayerIndex, 0, maxDepth, computerPlayer.PlayerUiSettings.UseTranspositionTable));
                indexesAndScores.Add(new KeyValuePair<short, float>((short)x.PlayIndex, score));
            });

            var rankedScores = indexesAndScores.OrderByDescending(x => Math.Abs(x.Value));
            return rankedScores.First().Key;
        }

        public void GetPlay(GameManager gameManager, ref short? computerPlayIndex, ComputerPlayer computerPlayer)
        {
            computerPlayIndex = GetPlay(gameManager, computerPlayer);
        }

        public void GetPlayWithBook(GameManager gameManager, GameStateStats gameStateStats, ref short? computerPlayIndex, ref bool computerStarted, ComputerPlayer computerPlayer)
        {
            if (computerPlayer.PlayerUiSettings.UseOpeningBook)
            {
                computerPlayIndex = BestBookPlay(gameStateStats.PlayStats, gameManager.PlayerIndex);
                if (computerPlayIndex != null) // Found a play in the opening book
                {
                    return;
                }
            }

            computerPlayIndex = GetPlay(gameManager, computerPlayer);
        }

        public short? BestBookPlay(Dictionary<short, PlayStats> playStats, int playerIndex)
        {
            var bestPlay = playStats.Values
                .Where(x => x.PercentageOfGames > .001D)
                .OrderByDescending(x => x.PercentageOfWinsForBlack * SearchAlgorithms.Sign[playerIndex])
                .FirstOrDefault();

            if (bestPlay != null)
            {
                return playStats.First(x => x.Value == bestPlay).Key;
            }
            return null;
        }
    }
}
