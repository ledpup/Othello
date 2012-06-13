using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Othello.Model;
using Othello.Model.Evaluation;

namespace Tests
{
    [TestClass]
    public class ZobristHashTest
    {
        readonly List<short?> _plays = GameManager.DeserialsePlays("e6,f4,c3,c6,d6,c4,d3,f7,f6,f5,e3,f3,g4,h4,g5,e7,c5,b4,d7,c8,g6,e2,e8,c7,d2,f2,h3,h2,g3,h5,h6,h7,d8,f8,b5,a4,a3,c1,f1,e1,a5,b6,g7,b3,d1,h8,b1,g2,a6,c2,a2,b2,g8,,g1,h1,a1,,a7,b7");

        [TestMethod]
        public void AlwaysTheSameHashForSameStateTest()
        {
            new ZobristHash();

            var gameState = new GameState();
            var hash = ZobristHash.Hash(gameState);

            var gameState2 = new GameState();
            var hash2 = ZobristHash.Hash(gameState2);

            Assert.AreEqual(hash, hash2);
        }

        [TestMethod]
        public void DifferentStateTest()
        {
            new ZobristHash();

            var gameState1 = new GameState(1UL, 0UL);
            var hash1 = ZobristHash.Hash(gameState1);

            var gameState2 = new GameState(0UL, 1UL);
            var hash2 = ZobristHash.Hash(gameState2);

            Assert.AreNotEqual(hash1, hash2);
        }

        [TestMethod]
        public void WholeGameTest()
        {
            new ZobristHash();

            var gameManager = new GameManager();

            var hashSet = new HashSet<ulong>();

            _plays.ForEach(x =>
                               {
                                   gameManager.PlacePiece(x);
                                   gameManager.NextTurn();
                                   hashSet.Add(ZobristHash.Hash(gameManager.GameState));
                               });

            Assert.AreEqual(_plays.Count, hashSet.Count);
        }
    }
}
