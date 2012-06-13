using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using Othello.Model;

namespace Tests
{
    /// <summary>
    ///This is a test class for BitBoardTest and is intended
    ///to contain all BitBoardTest Unit Tests
    ///</summary>
    [TestClass()]
    public class BitBoardHelperTest
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
        public void CountTest()
        {
            var x = ulong.MaxValue;
            short expected = 64;
            var actual = x.CountBits();
            Assert.AreEqual(expected, actual);

            x = 255;
            expected = 8;
            actual = x.CountBits();
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void IndicesTest()
        {
            ulong x = 256;
            var expected = new List<short> {8};
            var actual = x.Indices();
            Assert.IsTrue(actual.Count() == 1);
            Assert.AreEqual(expected[0], actual.ToArray()[0]);

            x = 255;
            actual = x.Indices();
            expected = new List<short> { 0, 1, 2, 3, 4, 5, 6, 7 };
            actual.ToList().ForEach(o => Assert.IsTrue(expected.Contains(o)));

            x = 68853694464;
            actual = x.Indices();
            expected = new List<short> { 27, 36 };
            actual.ToList().ForEach(o => Assert.IsTrue(expected.Contains(o)));

            x = 1UL << 63;
            expected = new List<short> { 63 };
            actual = x.Indices();
            Assert.IsTrue(actual.Count() == 1);
            Assert.AreEqual(expected[0], actual.ToArray()[0]);
        }

        [TestMethod]
        public void BitScanForwardTest()
        {
            var index = BitBoardHelper.BitScanForward(1);
            Assert.AreEqual(0, index); 
            index = BitBoardHelper.BitScanForward(128);
            Assert.AreEqual(7, index);
            index = BitBoardHelper.BitScanForward((ulong)Math.Pow(2, 27));
            Assert.AreEqual(27, index);
            index = BitBoardHelper.BitScanForward((ulong)Math.Pow(2, 64));
            Assert.AreEqual(63, index);
        }

        [TestMethod]
        public void Rotate180Test()
        {
            var gameState1 = GameState.NewGame();
            var gameState2 = gameState1;

            var plays = gameState1.PlayerPlays.Indices().ToList();
            
            gameState1.Draw();
            gameState1.PlacePiece(plays[0]);
            gameState1.Draw();
            gameState1 = gameState1.Rotate180();
            gameState1.Draw();
            gameState2.PlacePiece(plays[3]);
            gameState2.Draw();

            Assert.AreEqual(gameState2.PlayerPieces, gameState1.PlayerPieces);
        }

        [TestMethod]
        public void FlipDiagA1H8Test()
        {
            var gameState1 = GameState.NewGame();
            var gameState2 = gameState1;

            var plays = gameState1.PlayerPlays.Indices().ToList();

            gameState1.Draw();
            gameState1.PlacePiece(plays[0]);
            gameState1.Draw();
            gameState1 = gameState1.FlipDiagA1H8();
            gameState1.Draw();
            gameState2.PlacePiece(plays[1]);
            gameState2.Draw();

            Assert.AreEqual(gameState2.PlayerPieces, gameState1.PlayerPieces);
        }

        [TestMethod]
        public void FlipDiagA8H1Test()
        {
            var gameState1 = GameState.NewGame();
            var gameState2 = gameState1;

            var plays = gameState1.PlayerPlays.Indices().ToList();

            gameState1.Draw();
            gameState1.PlacePiece(plays[0]);
            gameState1.Draw();
            gameState1 = gameState1.FlipDiagA8H1();
            gameState1.Draw();
            gameState2.PlacePiece(plays[2]);
            gameState2.Draw();

            Assert.AreEqual(gameState2.PlayerPieces, gameState1.PlayerPieces);
        }
    }

}
