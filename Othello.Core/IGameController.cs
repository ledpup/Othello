using System;
using System.Collections.Generic;
using System.Text;

namespace Othello.Core
{
    public interface IGameController
    {
        int Transpositions { get; set; }
    }
}
