using System;

namespace Othello.Core
{
    public interface IPlayerUiSettings
    {
        bool BlackIsHuman { get; set; }
        bool WhiteIsHuman { get; set; }
        bool ShowValidPlays { get; set; }
        bool ShowBoardCoordinates { get; set; }
        bool ShowArchiveStats { get; set; }
        bool UseTranspositionTable { get; set; }
        bool UseOpeningBook { get; set; }
        int SearchMethod { get; set; }
        int MaxSearchDepth { get; set; }
        int MaxSearchTime { get; set; }
    }
}
