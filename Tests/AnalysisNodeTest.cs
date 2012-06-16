using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Othello.Model;
using Othello.Model.Evaluation;

namespace Tests
{
    [TestClass]
    public class AnalysisNodeTest
    {
        Dictionary<string, float> _weights = new Dictionary<string, float>
                           {
                               { "Pieces", .9f },
                               { "Mobility", .9f },
                               { "PotentialMobility", .9f },
                               { "Parity", 1f },
                               { "Pattern", .8f },
		                   };

        [TestMethod]
        public void TestMethod1()
        {

            var gameManager = new GameManager();
            var gameStateNode = new EvaluationNode(ref gameManager.GameState, _weights);

            ValidateNode(gameStateNode);
        }

        private void ValidateNode(EvaluationNode gameStateNode)
        {
            Assert.IsTrue(gameStateNode.Pieces >= 0 && gameStateNode.Pieces <= 1);
            Assert.IsTrue(gameStateNode.Mobility >= 0 && gameStateNode.Mobility <= 1);
            Assert.IsTrue(gameStateNode.Parity >= 0 && gameStateNode.Parity <= 1);
            Assert.IsTrue(gameStateNode.Pattern >= 0 && gameStateNode.Pattern <= 1);
        }

        [TestMethod]
        public void FullGameTest()
        {
            var gameManager = new GameManager();

            while (!gameManager.GameState.IsGameOver)
            {
                if (!gameManager.HasPlays)
                {
                    gameManager.NextTurn();
                    continue;
                }

                var depthFirstSearch = new DepthFirstSearch();
                var computerPlayIndex = depthFirstSearch.GetPlay(gameManager, new ComputerPlayer());

                gameManager.PlacePiece(computerPlayIndex);
                gameManager.NextTurn();

                var gameStateNode = new EvaluationNode(ref gameManager.GameState, _weights);
                ValidateNode(gameStateNode);
            }
        }
    }
}
