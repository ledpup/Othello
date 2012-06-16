using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using Othello.Model;
using Othello.Model.Evaluation;

namespace Tests
{
    [TestClass]
    public class DepthFirstSearchTest
    {
        GameManager _game1 = new GameManager(GameManager.DeserialsePlays("e6,f4,c3,c6,d6,c4,d3,f7,f6,f5,e3,f3,g4,h4,g5,e7,c5,b4,d7,c8,g6,e2,e8,c7,d2,f2,h3,h2,g3,h5,h6,h7,d8,f8,b5,a4,a3,c1,f1,e1,a5,b6,g7,b3,d1,h8,b1,g2,a6,c2,a2,b2,g8,,g1,h1,a1,,a7,b7"));
        GameManager _game2 = new GameManager(GameManager.DeserialsePlays("e6,f4,c3,c6,d6,c4,d3,f7,f6,f5,e3,f3,g4,h4,g5,e7,c5,b4,d7,c8,g6,e2,e8,c7,d2,f2,h3,h2,g3,h5,h6,h7,d8,f8,b5,a4,a3,c1,f1,e1,a5,b6,g7,b3,d1,h8,b1,g2,a6,c2"));
        GameManager _game3 = new GameManager(GameManager.DeserialsePlays("e6,f6,f5,d6,c5,e3,d3,c4,c6,b5,b6,b4,c3,f4,a4,e2,a3,b3,c2,a5,a6,c1,f1,d2,d1,f2,b1,c7,e1,b2,c8,b8,a8,a7,b7,a2,d8,d7,a1,,e8,e7,g2,g1,h1,h2,h3,,f3,,f8,f7,g3"));

        Dictionary<string, float> _weights = new Dictionary<string, float>
                           {
                               { "Pieces", .9f },
                               { "Mobility", .9f },
                               { "PotentialMobility", .9f },
                               { "Parity", 1f },
                               { "Pattern", .8f },
		                   };

        static TestNode AlphaBetaTree
        {
            get
            {
                var root = new TestNode(new List<INode> 
                {
                    new TestNode(new List<INode>
                    {
                        new TestNode(new List<INode>
                        {
                            new TestNode(null) {Value = 2},
                            new TestNode(null) {Value = 3},
                        }), 
                        new TestNode(new List<INode>
                        {
                            new TestNode(null) { Value = 5 },
                            new TestNode(null) { Value = 10 },
                        })
                    }),
                    new TestNode(new List<INode> 
                    { 
                        new TestNode(new List<INode>
                        {
                            new TestNode(null) { Value = 0 },
                        }),
                        new TestNode(new List<INode>
                        {
                            new TestNode(null) { Value = 10 }, 
                            new TestNode(null) { Value = 10 },
                        })
                    }),
                    new TestNode(new List<INode> 
                    {
                        new TestNode(new List<INode>
                        {
                            new TestNode(null) { Value = 2 }, 
                            new TestNode(null) { Value = 1 },
                        }),
                        new TestNode(new List<INode>
                        {
                            new TestNode(null) { Value = 10 }, 
                            new TestNode(null) { Value = 10 },
                        }) 
                    }),
                }
                );

                return root;
            }
        }

        static TestNode NegaScoutTree
        {
            get
            {
                var root = new TestNode(new List<INode> 
                {
                    new TestNode(new List<INode>
                    {
                        new TestNode(new List<INode>
                        {
                            new TestNode(null) {Value = 5},
                            new TestNode(null) {Value = 4},
                        }), 
                        new TestNode(new List<INode>
                        {
                            new TestNode(null) { Value = 6 },
                            new TestNode(null) { Value = 10 },
                        })
                    }),
                    new TestNode(new List<INode> 
                    { 
                        new TestNode(new List<INode>
                        {
                            new TestNode(null) { Value = 7 },
                            new TestNode(null) { Value = 10 },
                        }),
                        new TestNode(new List<INode>
                        {
                            new TestNode(null) { Value = 4 }, 
                            new TestNode(null) { Value = 4 },
                        })
                    }),
                }
                );

                return root;
            }
        }

        [TestMethod]
        public void NegaMaxTest()
        {
            var root = AlphaBetaTree;

            var computerPlayer = new ComputerPlayer {Search = SearchAlgorithms.NegaMax};

            var nodesSearched = new List<INode>();

            var score = computerPlayer.Search(root, new SearchConfig(0, 0, 3, false, nodesSearched));

            Assert.AreEqual(3, score);
            Assert.AreEqual(21, nodesSearched.Count());
        }

        [TestMethod]
        public void NegaMaxTest2()
        {
            var root = NegaScoutTree;

            var computerPlayer = new ComputerPlayer { Search = SearchAlgorithms.NegaMax };

            var nodesSearched = new List<INode>();

            var score = computerPlayer.Search(root, new SearchConfig(0, 0, 3, false, nodesSearched));

            Assert.AreEqual(5, score);
            Assert.AreEqual(15, nodesSearched.Count());
        }

        [TestMethod]
        public void AlphaBetaTest()
        {
            var root = AlphaBetaTree;

            var computerPlayer = new ComputerPlayer { Search = SearchAlgorithms.AlphaBetaNegaMax };

            var score = computerPlayer.Search(root, new SearchConfig(0, 0, 3));

            Assert.AreEqual(3, score);
        }

        [TestMethod]
        public void AlphaBetaTest2()
        {
            var root = NegaScoutTree;

            var computerPlayer = new ComputerPlayer { Search = SearchAlgorithms.AlphaBetaNegaMax };

            var nodesSearched = new List<INode>();

            var score = computerPlayer.Search(root, new SearchConfig(0, 0, 3, false, nodesSearched));

            Assert.AreEqual(5, score);
            Assert.AreEqual(14, nodesSearched.Count);
        }

        [TestMethod]
        public void NegaScoutTest()
        {
            var root = NegaScoutTree;

            var computerPlayer = new ComputerPlayer { Search = SearchAlgorithms.NegaScout };

            var nodesSearched = new List<INode>();

            var score = computerPlayer.Search(root, new SearchConfig(0, 0, 3, false, nodesSearched));

            Assert.AreEqual(5, score);
            Assert.AreEqual(13, nodesSearched.Count);
        }

        [TestMethod]
        public void NegaMaxReversiTest()
        {
            var computerPlayer = new ComputerPlayer { Search = SearchAlgorithms.NegaMax };

            var node = new EvaluationNode(ref _game1.GameState, _weights);

            var score = computerPlayer.Search(node, new SearchConfig(0, 0, 3));

            Assert.AreEqual(1, score);
        }

        [TestMethod]
        public void AlphaBetaReversiTest()
        {
            new DepthFirstSearch();

            var computerPlayer = new ComputerPlayer { Search = SearchAlgorithms.AlphaBetaNegaMax };

            var node = new EvaluationNode(ref _game1.GameState, _weights);

            var score = computerPlayer.Search(node, new SearchConfig(0, 0, 3));

            Assert.AreEqual(1, score);
        }

        [TestMethod]
        public void NegaScoutReversiTest()
        {
            var computerPlayer = new ComputerPlayer { Search = SearchAlgorithms.NegaScout };

            var node = new EvaluationNode(ref _game1.GameState, _weights);

            var score = computerPlayer.Search(node, new SearchConfig(0, 0, 3));

            Assert.AreEqual(1, score);
        }

        [TestMethod]
        public void NegaScoutReversiGame2Test()
        {
            var computerPlayer = new ComputerPlayer { Search = SearchAlgorithms.NegaScout };

            var node = new EvaluationNode(ref _game3.GameState, _weights);

            var score = computerPlayer.Search(node, new SearchConfig(0, 0, 10));

            Assert.AreEqual(0, score);
        }

        [TestMethod]
        public void SelectResultTest()
        {
            var depthFirstSearch = new DepthFirstSearch();

            var result = depthFirstSearch.GetPlay(_game3, new ComputerPlayer());

            Assert.AreEqual(30, result);
            //var index = DepthFirstSearch.GetMoveIndex(node, score);
        }


    }
}
