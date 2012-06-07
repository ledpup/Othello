using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Reversi.Model;
using Reversi.Model.TranspositionTable;

namespace Tests
{
    [TestClass]
    public class ZobristHashTest
    {
        [TestMethod]
        public void AlwaysTheSameHashForSameStateTest()
        {
            var zobristHash = new ZobristHash();
            var gameState = new GameState();
            var hash = zobristHash.Hash(gameState, false);

            var zobristHash2 = new ZobristHash();
            var gameState2 = new GameState();
            var hash2 = zobristHash2.Hash(gameState2, false);
            
            Assert.AreEqual(hash, hash2);
        }

        [TestMethod]
        public void DifferentStateTest()
        {
            var zobristHash = new ZobristHash();

            var gameState1 = new GameState(1UL, 0UL);
            var hash1 = zobristHash.Hash(gameState1, true);

            var gameState2 = new GameState(0UL, 1UL);
            var hash2 = zobristHash.Hash(gameState2, true);
            
            Assert.AreNotEqual(hash1, hash2);
        }

        [TestMethod]
        public void SameStateDifferentPlayerTest()
        {
            var zobristHash = new ZobristHash();

            var gameState1 = new GameState();
            var hash1 = zobristHash.Hash(gameState1, true);

            var gameState2 = new GameState();
            var hash2 = zobristHash.Hash(gameState2, false);

            Assert.AreNotEqual(hash1, hash2);
        }
    }
}
