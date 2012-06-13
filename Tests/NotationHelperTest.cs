using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Othello.Model;

namespace Tests
{
    [TestClass]
    public class NotationHelperTest
    {
        private List<short?> _tamenoriPlayList = new List<short?> { 26, 20, 45, 44, 37, 34, 29, 46, 53, 19 };
        private string _tamenori = "c4,e3,f6,e6,f5,c5,f4,g6,f7,d3";

        [TestMethod]
        public void AlgebraicToIndex()
        {
            var move = "c4".ToIndex();
            Assert.AreEqual((short)26, move);

            move = "f5".ToIndex();
            Assert.AreEqual((short)37, move);
            move = "f8".ToIndex();
            Assert.AreEqual((short)61, move);
            move = "f1".ToIndex();
            Assert.AreEqual((short)5, move);

            move = "a1".ToIndex();
            Assert.AreEqual((short)0, move);
            move = "a4".ToIndex();
            Assert.AreEqual((short)24, move);
            move = "a8".ToIndex();
            Assert.AreEqual((short)56, move);

            move = "h8".ToIndex();
            Assert.AreEqual((short)63, move);
            move = "h1".ToIndex();
            Assert.AreEqual((short)7, move);
        }

        [TestMethod]
        public void IndexToAlgebraic()
        {
            var move = ((short?)37).ToAlgebraicNotation();
            Assert.AreEqual("f5", move);
            move = ((short?)61).ToAlgebraicNotation();
            Assert.AreEqual("f8", move);
            move = ((short?)5).ToAlgebraicNotation();
            Assert.AreEqual("f1", move);

            move = ((short?)56).ToAlgebraicNotation();
            Assert.AreEqual("a8", move);
            move = ((short?)0).ToAlgebraicNotation();
            Assert.AreEqual("a1", move);

            move = ((short?)63).ToAlgebraicNotation();
            Assert.AreEqual("h8", move);
            move = ((short?)7).ToAlgebraicNotation();
            Assert.AreEqual("h1", move);
        }

        [TestMethod]
        public void Tamenori()
        {
            var tamenoriAlgebraic = new List<string>();
            _tamenoriPlayList.ForEach(m => tamenoriAlgebraic.Add(m.ToAlgebraicNotation()));

            var actual = string.Join(",", tamenoriAlgebraic);

            Assert.AreEqual(_tamenori, actual);
        }
    }
}
