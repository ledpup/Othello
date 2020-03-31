using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Othello.Model;
using Othello.Model.Evaluation;

namespace Othello.UnitTests
{
    [TestClass]
    public class PatternsTest
    {
        [TestMethod]
        public void CornerTest()
        {
            var corners = Patterns.CombinedSymmetries("A1".ToBitBoard());

            var positions = corners.ToAlgebraicNotationList();
            corners.Draw();
            Assert.IsTrue(positions.Contains("A1"));
            Assert.IsTrue(positions.Contains("H1"));
            Assert.IsTrue(positions.Contains("A8"));
            Assert.IsTrue(positions.Contains("H8"));
        }

        [TestMethod]
        public void FullEdgeTest()
        {
            var edges = Patterns.Symmetries(255);

            var positions = edges[2].ToAlgebraicNotationList();
            
            Assert.IsTrue(positions.Contains("A8"));
            Assert.IsTrue(positions.Contains("B8"));
            Assert.IsTrue(positions.Contains("C8"));
            Assert.IsTrue(positions.Contains("D8"));
            Assert.IsTrue(positions.Contains("E8"));
            Assert.IsTrue(positions.Contains("F8"));
            Assert.IsTrue(positions.Contains("G8"));
            Assert.IsTrue(positions.Contains("H8"));
        }

        [TestMethod]
        public void AllCornerTest()
        {
            Patterns.Corners.ForEach(x => x.Draw());
            Patterns.Corners.ForEach(x => Assert.IsTrue(Patterns.Corners.Where(y => x == y).Count() == 1));
        }

        [TestMethod]
        public void AllEdgeTest()
        {
            Patterns.AllEdges.ForEach(x => x.Draw());
            Patterns.AllEdges.ForEach(x => Assert.IsTrue(Patterns.AllEdges.Where(y => x == y).Count() == 1));
        }

        
    }
}
