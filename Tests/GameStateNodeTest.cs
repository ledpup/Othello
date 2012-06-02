using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Reversi.Model;
using Reversi.Model.Evaluation;

namespace Tests
{
    [TestClass]
    public class GameStateNodeTest
    {
        Dictionary<string, float> _weights = new Dictionary<string, float>
                           {
                               { "Pieces", .9f },
                               { "Mobility", .9f },
                               { "PotentialMobility", .9f },
                               { "Parity", 1f },
                               { "PositionValues", .8f },
		                   };

        [TestMethod]
        public void TestMethod1()
        {

            var gameManager = new GameManager();
            var gameStateNode = new GameStateNode(gameManager.GameState, _weights);

            ValidateNode(gameStateNode);
        }

        private void ValidateNode(GameStateNode gameStateNode)
        {
            Assert.IsTrue(gameStateNode.Pieces >= 0 && gameStateNode.Pieces <= 1);
            Assert.IsTrue(gameStateNode.Mobility >= 0 && gameStateNode.Mobility <= 1);
            Assert.IsTrue(gameStateNode.Parity >= 0 && gameStateNode.Parity <= 1);
            Assert.IsTrue(gameStateNode.Position >= 0 && gameStateNode.Position <= 1);
        }

        [TestMethod]
        public void FullGameTest()
        {
            var weights = new[]
                           { 
                               new Dictionary<string, float>{
                                   { "Pieces", 1f },
                                   { "Mobility", 1f },
                                   { "PotentialMobility", 1f },
                                   { "Parity", 1f },
                                   { "PositionValues", 1f },
		                       },
                               new Dictionary<string, float>{
                                   { "Pieces", 1f },
                                   { "Mobility", 1f },
                                   { "PotentialMobility", 1f },
                                   { "Parity", 1f },
                                   { "PositionValues", 1f },
		                       },
                           };

            var gameManager = new GameManager();

            while (!gameManager.GameState.IsGameOver)
            {
                if (!gameManager.HasPlays)
                {
                    gameManager.NextTurn();
                    continue;
                }

                short? computerPlayIndex = null;

                DepthFirstSearch.GetPlay(gameManager, weights[gameManager.PlayerIndex], ref computerPlayIndex);

                gameManager.PlacePiece(computerPlayIndex);
                gameManager.NextTurn();

                var gameStateNode = new GameStateNode(gameManager.GameState, _weights);
                ValidateNode(gameStateNode);
            }
        }
    }
}
