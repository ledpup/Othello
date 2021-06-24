using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Othello.Model;

namespace Othello.UnitTests
{
    [TestClass]
    public class NotationHelperTest
    {
        private List<short?> _tamenoriPlayList = new List<short?> { 26, 20, 45, 44, 37, 34, 29, 46, 53, 19 };
        private string _tamenori = "C4,E3,F6,E6,F5,C5,F4,G6,F7,D3";

        [TestMethod]
        public void AlgebraicToIndex()
        {
            var move = "C4".ToIndex();
            Assert.AreEqual((short)26, move);

            move = "F5".ToIndex();
            Assert.AreEqual((short)37, move);
            move = "F8".ToIndex();
            Assert.AreEqual((short)61, move);
            move = "F1".ToIndex();
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
            Assert.AreEqual("F5", move);
            move = ((short?)61).ToAlgebraicNotation();
            Assert.AreEqual("F8", move);
            move = ((short?)5).ToAlgebraicNotation();
            Assert.AreEqual("F1", move);

            move = ((short?)56).ToAlgebraicNotation();
            Assert.AreEqual("A8", move);
            move = ((short?)0).ToAlgebraicNotation();
            Assert.AreEqual("A1", move);

            move = ((short?)63).ToAlgebraicNotation();
            Assert.AreEqual("H8", move);
            move = ((short?)7).ToAlgebraicNotation();
            Assert.AreEqual("H1", move);
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
