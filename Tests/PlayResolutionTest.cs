using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Othello.Model;

namespace Tests
{
    /// <summary>
    ///This is a test class for MoveResolutionTest and is intended
    ///to contain all MoveResolutionTest Unit Tests
    ///</summary>
    [TestClass()]
    public class PlayResolutionTest
    {
        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion

        [TestMethod]
        public void ResolveLeftTest()
        {
            ulong activePiece = 8;
            ulong activePieces = 9;
            ulong inactivePieces = 6;
            ulong expected = 6;
            var actual = Play.PlaceOneDirection(Play.Left, activePiece, activePieces, inactivePieces);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void ResolveFalseLeftTest()
        {
            ulong activePiece = 8;
            ulong activePieces = 12;
            ulong inactivePieces = 2;
            ulong expected = 0;
            var actual = Play.PlaceOneDirection(Play.Left, activePiece, activePieces, inactivePieces);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void ResolveLeftEmptyTest()
        {
            ulong activePiece = 8;
            ulong activePieces = 9;
            ulong inactivePieces = 4;
            ulong expected = 0;
            var actual = Play.PlaceOneDirection(Play.Left, activePiece, activePieces, inactivePieces);
            Assert.AreEqual(expected, actual);

            activePieces = 9;
            inactivePieces = 2;
            actual = Play.PlaceOneDirection(Play.Left, activePiece, activePieces, inactivePieces);
            Assert.AreEqual(expected, actual);

            activePieces = 8;
            inactivePieces = 4;
            actual = Play.PlaceOneDirection(Play.Left, activePiece, activePieces, inactivePieces);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void WhiteResolveTest()
        {
            var gameState = new GameState(0x00000028080c0000, 0x0000081034000000);

            var actual = Play.PlacePiece(512, gameState.OpponentPieces, gameState.PlayerPieces);
            ulong expected = 262144 + 134217728;
            Assert.AreEqual(expected, actual);

            actual = Play.PlacePiece(1024, gameState.OpponentPieces, gameState.PlayerPieces);
            expected = 262144 + 524288;
            Assert.AreEqual(expected, actual);

            actual = Play.PlacePiece(2048, gameState.OpponentPieces, gameState.PlayerPieces);
            expected = 524288 + 134217728 + 34359738368;
            Assert.AreEqual(expected, actual);

            actual = Play.PlacePiece(4096, gameState.OpponentPieces, gameState.PlayerPieces);
            expected = 524288;
            Assert.AreEqual(expected, actual);

            actual = Play.PlacePiece(274877906944, gameState.OpponentPieces, gameState.PlayerPieces);
            expected = 137438953472;
            Assert.AreEqual(expected, actual);

            actual = Play.PlacePiece(70368744177664, gameState.OpponentPieces, gameState.PlayerPieces);
            expected = 137438953472;
            Assert.AreEqual(expected, actual);

            actual = Play.PlacePiece(35184372088832, gameState.OpponentPieces, gameState.PlayerPieces);
            expected = 137438953472;
            Assert.AreEqual(expected, actual);

            actual = Play.PlacePiece(17592186044416, gameState.OpponentPieces, gameState.PlayerPieces);
            expected = 34359738368;
            Assert.AreEqual(expected, actual);

            actual = Play.PlacePiece(4398046511104, gameState.OpponentPieces, gameState.PlayerPieces);
            expected = 34359738368;
            Assert.AreEqual(expected, actual);

            actual = Play.PlacePiece(17179869184, gameState.OpponentPieces, gameState.PlayerPieces);
            expected = 34359738368;
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void BlackResolveTest()
        {
            var gameState = new GameState(0x00000028080c0000, 0x0000081034000000);
            //var gameState = GameState.NewGame();

            var actual = Play.PlacePiece(131072, gameState.PlayerPieces, gameState.OpponentPieces);
            ulong expected = 67108864;
            Assert.AreEqual(expected, actual);

            actual = Play.PlacePiece(33554432, gameState.PlayerPieces, gameState.OpponentPieces);
            expected = 67108864;
            Assert.AreEqual(expected, actual);

            actual = Play.PlacePiece(8589934592, gameState.PlayerPieces, gameState.OpponentPieces);
            expected = 67108864;
            Assert.AreEqual(expected, actual);

            actual = Play.PlacePiece(17179869184, gameState.PlayerPieces, gameState.OpponentPieces);
            expected = 67108864;
            Assert.AreEqual(expected, actual);

            actual = Play.PlacePiece(2251799813685250, gameState.PlayerPieces, gameState.OpponentPieces);
            expected = 8796093022208;
            Assert.AreEqual(expected, actual);

            actual = Play.PlacePiece(35184372088832, gameState.PlayerPieces, gameState.OpponentPieces);
            expected = 68719476736;
            Assert.AreEqual(expected, actual);

            actual = Play.PlacePiece(1073741824, gameState.PlayerPieces, gameState.OpponentPieces);
            expected = 268435456 + 536870912;
            Assert.AreEqual(expected, actual);

            actual = Play.PlacePiece(2097152, gameState.PlayerPieces, gameState.OpponentPieces);
            expected = 268435456 + 536870912;
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void PotentialMobility()
        {
            var gameState = GameState.NewGame();

            var actual = Play.PotentialMobilityOneDirection(Play.Left, gameState.PlayerPieces, gameState.EmptySquares);
            var expected = "c5";
            Assert.AreEqual(expected, ((short?)actual.Indices().Single()).ToAlgebraicNotation());

            actual = Play.PotentialMobilityOneDirection(Play.UpLeft, gameState.PlayerPieces, gameState.EmptySquares);
            Assert.AreEqual("d3", ((short?)(actual.Indices().ToList()[0])).ToAlgebraicNotation());
            Assert.AreEqual("c4", ((short?)(actual.Indices().ToList()[1])).ToAlgebraicNotation());

            actual = Play.PotentialMobilityOneDirection(Play.Up, gameState.PlayerPieces, gameState.EmptySquares);
            expected = "e3";
            Assert.AreEqual(expected, ((short?)actual.Indices().Single()).ToAlgebraicNotation());


            actual = Play.PotentialMobility(gameState.PlayerPieces, gameState.EmptySquares);
            var plays = actual.Indices().Select(x => ((short?)x).ToAlgebraicNotation()).ToList();
            Assert.AreEqual(10, plays.Count);
            Assert.IsTrue(plays.Contains("d3"));
            Assert.IsTrue(plays.Contains("e3"));
            Assert.IsTrue(plays.Contains("f3"));
            Assert.IsTrue(plays.Contains("f4"));
            Assert.IsTrue(plays.Contains("f5"));
            Assert.IsTrue(plays.Contains("e6"));
            Assert.IsTrue(plays.Contains("d6"));
            Assert.IsTrue(plays.Contains("c6"));
            Assert.IsTrue(plays.Contains("c5"));
            Assert.IsTrue(plays.Contains("c4"));
            Assert.IsTrue(plays.Contains("c4"));
        }
    }
}
