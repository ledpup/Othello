using Othello.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Othello.UnitTests
{
    public class PlayerUiSettings : IPlayerUiSettings
    {
        public bool BlackIsHuman { get; set; }
        public bool WhiteIsHuman { get; set; }
        public bool ShowValidPlays { get; set; }
        public bool ShowBoardCoordinates { get; set; }
        public bool ShowArchiveStats { get; set; }
        public bool UseTranspositionTable { get; set; }
        public bool UseOpeningBook { get; set; }
        public int SearchMethod { get; set; }
        public int MaxSearchDepth { get; set; }
        public int MaxSearchTime { get; set; }

        public PlayerUiSettings()
        {
            BlackIsHuman = true;
            WhiteIsHuman = false;
            ShowBoardCoordinates = true;
            ShowValidPlays = true;
            ShowArchiveStats = true;
            UseTranspositionTable = true;
            UseOpeningBook = true;
            MaxSearchDepth = 5;
            SearchMethod = 1;
            MaxSearchTime = 300000;
        }
    }
}
