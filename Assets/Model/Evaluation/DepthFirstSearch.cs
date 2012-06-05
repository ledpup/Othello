using System;
using System.Collections.Generic;
using System.Linq;

namespace Reversi.Model.Evaluation
{
    public class DepthFirstSearch
    {
        public delegate float SearchMethod(ref INode node, int colour = 0, int depth = 0, IList<INode> nodesSearched = null /* Only used if we want to record which nodes were searched */);
        public static SearchMethod Search { get; set; }

        public static GameStateNodeCollection GameStateNodeCollection;

        public ComputerPlayer ComputerPlayer;

        public DepthFirstSearch() : this(new ComputerPlayer(true))
        {
            
        }

        public DepthFirstSearch(ComputerPlayer computerPlayer)
        {
            ComputerPlayer = computerPlayer;
            Search = SearchAlgorithms.AlphaBetaNegaMax;
            GameStateNodeCollection = new GameStateNodeCollection();
        }

        public short GetPlay(GameManager gameManager)
        {
            SearchAlgorithms.MaxDepth = ComputerPlayer.GetSearchDepth(gameManager.Turn);

            var node = new GameStateNode(ref gameManager.GameState, ComputerPlayer.GetWeights(gameManager.Turn));

            var indexesAndScores = new List<KeyValuePair<short, float>>();

            node.ChildNodeReferences.ForEach(x =>
                                              {
                                                  var child = GameStateNodeCollection.GetGameStateNode(x);
                                                  var score = Search(ref child, gameManager.PlayerIndex);
                                                  indexesAndScores.Add(new KeyValuePair<short, float>(child.PlayIndex, score));
                                              });

            //node.Children.ToList().ForEach(x =>
            //{
            //    var score = Search(x, gameManager.PlayerIndex);
            //    indexesAndScores.Add(new KeyValuePair<short, float>(x.PlayIndex, score));
            //});

            var rankedScores = indexesAndScores.OrderByDescending(x => Math.Abs(x.Value));
            return rankedScores.First().Key;
        }

        public void GetPlay(GameManager gameManager, ref short? computerPlayIndex)
        {
            computerPlayIndex = GetPlay(gameManager);
        }

        public void GetPlayWithBook(GameManager gameManager, GameStateStats gameStateStats, ref short? computerPlayIndex, ref bool computerStarted)
        {
            if (ComputerPlayer.UseOpeningBook)
            {
                computerPlayIndex = BestBookPlay(gameStateStats.PlayStats, gameManager.PlayerIndex);
                if (computerPlayIndex != null) // Found a book play
                {
                    return;
                }
            }

            computerPlayIndex = GetPlay(gameManager);
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
