using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Othello.Model;
using Othello.Model.Evaluation;

namespace Tests
{
    [TestClass]
    public class PatternsTest
    {
        [TestMethod]
        public void CornerTest()
        {
            var corners = Patterns.CombinedSymmetries("a1".ToBitBoard());

            var positions = corners.ToAlgebraicNotationList();
            corners.Draw();
            Assert.IsTrue(positions.Contains("a1"));
            Assert.IsTrue(positions.Contains("h1"));
            Assert.IsTrue(positions.Contains("a8"));
            Assert.IsTrue(positions.Contains("h8"));
        }

        [TestMethod]
        public void FullEdgeTest()
        {
            var edges = Patterns.Symmetries(255);

            var positions = edges[2].ToAlgebraicNotationList();
            
            Assert.IsTrue(positions.Contains("a8"));
            Assert.IsTrue(positions.Contains("b8"));
            Assert.IsTrue(positions.Contains("c8"));
            Assert.IsTrue(positions.Contains("d8"));
            Assert.IsTrue(positions.Contains("e8"));
            Assert.IsTrue(positions.Contains("f8"));
            Assert.IsTrue(positions.Contains("g8"));
            Assert.IsTrue(positions.Contains("h8"));
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
