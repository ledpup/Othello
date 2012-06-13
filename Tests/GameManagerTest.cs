using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Othello.Model;

namespace Tests
{
    [TestClass]
    public class GameManagerTest
    {
        private List<short?> _tamenoriPlays = new List<short?> { 26, 20, 45, 44, 37, 34, 29, 46, 53, 19 };
        private string _tamenori = "c4,e3,f6,e6,f5,c5,f4,g6,f7,d3";

        [TestMethod]
        public void CreateNewGame()
        {
            var actual = new GameManager();
            Assert.AreEqual(4, actual.GameState.AllPieces.CountBits());
            Assert.AreEqual(1UL << 28 | 1UL << 35, actual.GameState.PlayerPieces);
            Assert.AreEqual(1UL << 27 | 1UL << 36, actual.GameState.OpponentPieces);
        }

        [TestMethod]
        public void FirstPlay()
        {
            var gameManager = new GameManager();

            ulong expected = 1UL << 19 | 1UL << 26 | 1UL << 37 | 1UL << 44;

            var playBoard = gameManager.GameState.PlayerPlays;
            var plays = gameManager.GameState.PlayerPlays.Indices().ToList();

            Assert.AreEqual(expected, playBoard);
            Assert.IsTrue(plays.Contains(19));
            Assert.IsTrue(plays.Contains(26));
            Assert.IsTrue(plays.Contains(37));
            Assert.IsTrue(plays.Contains(44));
        }

        [TestMethod]
        public void CreateGameFromPlays()
        {
            var actual = new GameManager(_tamenoriPlays);
            Assert.AreEqual(14, actual.GameState.AllPieces.CountBits());
            Assert.AreEqual(10, actual.Turn);
        }

        [TestMethod]
        public void MakePlay()
        {
            var gameManager = new GameManager();
            gameManager.PlacePiece(26);
            Assert.AreEqual(1, gameManager.Turn);
        }

        [TestMethod]
        public void UndoGame()
        {
            var gameManager = new GameManager(_tamenoriPlays);
            var actual = new GameManager(gameManager.Plays.Take(6).ToList());

            var plays = _tamenoriPlays.Take(6).ToList();
            var expected = new GameManager(plays);

            Assert.AreEqual(expected.GameState.AllPieces, actual.GameState.AllPieces);
            Assert.AreEqual(expected.GameState.PlayerPieces, actual.GameState.PlayerPieces);
            Assert.AreEqual(expected.GameState.OpponentPieces, actual.GameState.OpponentPieces);
            Assert.AreEqual(6, actual.Turn);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void InvalidPlay()
        {
            var gameManager = new GameManager();
            gameManager.PlacePiece(1);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NullPlays()
        {
            new GameManager(null);
        }

        [TestMethod]
        [ExpectedException((typeof(Exception)))]
        public void InvalidNullPlay()
        {
            var plays = GameManager.DeserialsePlays("c4,e3,,");
            new GameManager(plays);
        }

        [TestMethod]
        public void UndoGameThenPlacePiece()
        {
            var gameManager = new GameManager(_tamenoriPlays);
            gameManager = new GameManager(_tamenoriPlays.Take(6).ToList());

            Assert.AreEqual(6, gameManager.Turn);

            gameManager.PlacePiece(29);
            gameManager.NextTurn();

            Assert.AreEqual(7, gameManager.Turn);
        }

        [TestMethod]
        public void SaveAndLoadGame()
        {
            var gameManager = new GameManager(_tamenoriPlays);

            var algebraicNotation = GameManager.SerialsePlays(_tamenoriPlays);
            var plays = GameManager.DeserialsePlays(algebraicNotation);

            Assert.AreEqual(string.Join(",", plays), string.Join(",", plays));
            Assert.AreEqual(10, gameManager.Turn);
        }

        [TestMethod]
        public void LoadAndSaveGame()
        {
            var gameManager = new GameManager(new List<short?>());
            var plays = GameManager.DeserialsePlays(_tamenori);
            gameManager = new GameManager(plays);
            Assert.AreEqual(_tamenori, GameManager.SerialsePlays(gameManager.Plays));
            Assert.AreEqual(10, gameManager.Turn);
        }

        [TestMethod]
        public void MultipleNullPlays()
        {
            var plays = GameManager.DeserialsePlays("d3,c3,b3,b2,b1,a1,c4,c1,c2,d2,d1,e1,a2,a3,f5,e2,f1,g1,,f2,,e3,,b5,b4");

            Assert.AreEqual(25, plays.Count);
            
            var gameManager = new GameManager(plays);

            Assert.AreEqual(plays.Count, gameManager.Turn);
        }
    }
}
