using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Reversi.Model;

namespace Tests
{
    [TestClass]
    public class StabilityTests
    {
        [TestMethod]
        public void StabilityTest()
        {
            var player = new List<string> { "a1", "b1", "c1", "e1", "a2", "b2", "d2", "a3", "c3" };
            var opponent = new List<string> {"d1", "c3", "b3", "a4"};

            var playerBoard = player.ToBitBoard();
            var opponentBoard = opponent.ToBitBoard();
            
            var stablePieces = Play.StablePieces(playerBoard, opponentBoard);

            var stablePositions = stablePieces.Indices().Select(x => ((short?)x).ToAlgebraicNotation()).ToList();

            Assert.IsTrue(stablePositions.Contains("a1"));
            Assert.IsTrue(stablePositions.Contains("b1"));
            Assert.IsTrue(stablePositions.Contains("c1"));
            Assert.IsTrue(stablePositions.Contains("a2"));
            Assert.IsTrue(stablePositions.Contains("b2"));
            Assert.IsTrue(stablePositions.Contains("a3"));
        }


    }
}
