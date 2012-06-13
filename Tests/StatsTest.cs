using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Othello.Model;
using System.Diagnostics.Contracts;
using Othello.Model.Evaluation;

namespace Tests
{
    [TestClass]
    public class StatsTest
    {
        [TestMethod]
        public void Test()
        {
            var gameArchive = new List<string> {@"\MBJC[]ZUV^DRQbYcIOdkme<XiNn@HPAlj8aF_=GWf5:ogE;243>h`70?619,W",
                                                @"\MBJC[]ZUV^DRQbYcIOdkmeji<AW_nHgP:aflh2`X9o@031N8;4EG=5F?>76,B",
                                                @"\MBJC[]ZUV^DRQbYcIOe@HdmNjA<klPX`:ahif=WE;F8on4239g_0156?>7G,0",
                                                @"\MBJC[]ZUV^DRQbYcIWdkme<Xj@NiHPA8n9:a0l`ohGfFE=Og_215643;>7?,W",
                                                @"\MBJC[]ZUV^DRQbYceIdm_=AF<NEW;Xkgj43HP5621:Ofo@ln809?>7G`aih,B",
                                                @"\MBJC[]ZUV^DRQbc=eY;djkl<XOA3I@:25HiNF41E6GW>P`9_fgah?708onm,B",
                                                @"\MBJC[]ZUV^DRQbcY;OdlekXmAHfNWFEGI<543=6:@P29>7?1`hjon_gia80,B",};

            var positionStats = new GameStateStats();

            var playStats = positionStats.GenerateStats(@"\MBJC[]ZUV^DRQb", gameArchive);

            Assert.IsNotNull(playStats['Y'.ToIndex()]);
            Assert.AreEqual(5, playStats['Y'.ToIndex()].SubsetCount);
            Assert.AreEqual(2, playStats['Y'.ToIndex()].BlackWins);

            Assert.IsNotNull(playStats['c'.ToIndex()]);
            Assert.AreEqual(2, playStats['c'.ToIndex()].SubsetCount);
        }

        [TestMethod]
        public void FullySymmetryTest()
        {
            var gameArchive = new List<string> {@"\MBJC[]ZUV^DRQbYcIOdkme<XiNn@HPAlj8aF_=GWf5:ogE;243>h`70?619,W",
                                                @"\MBJC[]ZUV^DRQbYcIOdkmeji<AW_nHgP:aflh2`X9o@031N8;4EG=5F?>76,B",
                                                @"\MBJC[]ZUV^DRQbYcIOe@HdmNjA<klPX`:ahif=WE;F8on4239g_0156?>7G,0",
                                                @"\MBJC[]ZUV^DRQbYcIWdkme<Xj@NiHPA8n9:a0l`ohGfFE=Og_215643;>7?,W",
                                                @"\MBJC[]ZUV^DRQbYceIdm_=AF<NEW;Xkgj43HP5621:Ofo@ln809?>7G`aih,B",
                                                @"\MBJC[]ZUV^DRQbc=eY;djkl<XOA3I@:25HiNF41E6GW>P`9_fgah?708onm,B",
                                                @"\MBJC[]ZUV^DRQbcY;OdlekXmAHfNWFEGI<543=6:@P29>7?1`hjon_gia80,B",};

            var gameManager = new GameManager();

            var positionStats = new GameStateStats();

            var canDraw = false;
            var statsAvailable = false;

            positionStats.GenerateStats(gameManager, gameArchive, ref canDraw, ref statsAvailable);

            Assert.IsTrue(positionStats.PlayStats.ContainsKey((short)"e6".ToIndex()));
            Assert.IsTrue(positionStats.PlayStats.ContainsKey((short)"d3".ToIndex()));
            Assert.IsTrue(positionStats.PlayStats.ContainsKey((short)"c4".ToIndex()));
            Assert.IsTrue(positionStats.PlayStats.ContainsKey((short)"f5".ToIndex()));

        }

        [TestMethod]
        public void BestBookOpeningTest()
        {
            var subset = new List<string>
                             {
                                 @"\MB,B",
                                 @"\MB,B",
                                 @"\MB,W",
                                 @"\MB,W",
                                 @"\MC,W",
                                 @"\MC,W",
                                 @"\MD,B",
                                 @"\MD,B",
                                 @"\MD,W",
                             };

            var gameStateStats = new Dictionary<short, PlayStats>
                                     {
                                         { 0, new PlayStats(subset.Count, subset, @"\M", 'B') },
                                         { 1, new PlayStats(subset.Count, subset, @"\M", 'C') },
                                         { 2, new PlayStats(subset.Count, subset, @"\M", 'D') },
                                     };

            var depthFirstSearch = new DepthFirstSearch();

            var bestIndexBlack = depthFirstSearch.BestBookPlay(gameStateStats, 0);
            Assert.AreEqual((short)2, bestIndexBlack);

            var bestIndexWhite = depthFirstSearch.BestBookPlay(gameStateStats, 1);
            Assert.AreEqual((short)1, bestIndexWhite);
        }
    }
}
