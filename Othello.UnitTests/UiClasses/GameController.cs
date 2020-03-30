using Othello.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Othello.UnitTests
{
    public class GameController : IGameController
    {
        public int Transpositions { get; set; }
    }
}
