using Microsoft.VisualStudio.TestTools.UnitTesting;
using Othello.Model;

namespace Tests
{
    [TestClass()]
    public class ValidPlayTest
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
        public void UpTest()
        {
            ulong black = 1 << 16;
            ulong white = 1 << 8;

            ulong expected = 1;
            var actual = Play.ValidateOneDirection(Play.Up, black, white, BitBoardHelper.EmptySquares(black, white));
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void DownTest()
        {
            ulong black = 1;
            ulong white = 1 << 8;

            ulong expected = 1 << 16;
            var actual = Play.ValidateOneDirection(Play.Down, black, white, BitBoardHelper.EmptySquares(black, white));
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void RightTest()
        {
            ulong black = 1;
            ulong white = 1 << 1;

            ulong expected = 1 << 2;
            var actual = Play.ValidateOneDirection(Play.Right, black, white, BitBoardHelper.EmptySquares(black, white));
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void LeftTest()
        {
            ulong black = 1 << 2;
            ulong white = 1 << 1;

            ulong expected = 1;
            var actual = Play.ValidateOneDirection(Play.Left, black, white, BitBoardHelper.EmptySquares(black, white));
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void UpRightTest()
        {
            ulong black = 1 << 16;
            ulong white = 1 << 9;

            ulong expected = 1 << 2;
            var actual = Play.ValidateOneDirection(Play.UpRight, black, white, BitBoardHelper.EmptySquares(black, white));
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void DownRightTest()
        {
            ulong black = 1;
            ulong white = 1 << 9;

            ulong expected = 1 << 18;
            var actual = Play.ValidateOneDirection(Play.DownRight, black, white, BitBoardHelper.EmptySquares(black, white));
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void DownLeftTest()
        {
            ulong black = 1 << 2;
            ulong white = 1 << 9;

            ulong expected = 1 << 16;
            var actual = Play.ValidateOneDirection(Play.DownLeft, black, white, BitBoardHelper.EmptySquares(black, white));
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void UpLeftTest()
        {
            ulong black = 1 << 18;
            ulong white = 1 << 9;

            ulong expected = 1;
            var actual = Play.ValidateOneDirection(Play.UpLeft, black, white, BitBoardHelper.EmptySquares(black, white));
            Assert.AreEqual(expected, actual);
        }


        [TestMethod]
        public void ExampleBoardTest()
        {
            var gameState = new GameState(0x00000028080c0000, 0x0000081034000000);

            ulong expectedBlackPlay = 0x0008200642220000;
            
            Assert.AreEqual(expectedBlackPlay, gameState.PlayerPlays);
        }
    }
}
