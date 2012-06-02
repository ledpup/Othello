using System.Collections.Generic;
using System.Linq;

namespace Reversi.Model
{
	public class PlayStats
	{
	    public string Position;
        public double PercentageOfGames;
        public double PercentageOfWinsForBlack;
	    public int SubsetCount;
	    public int BlackWins;

        public PlayStats(int numberOfGames, List<string> subset, string position, char play)
        {
            var nextPosition = position + play;

            Position = nextPosition;

            subset = GameStateStats.GetSubset(nextPosition, subset, subset.Count());

            SubsetCount = subset.Count();

            BlackWins = subset.Where(y => y.Last() == 'B').Count();

            PercentageOfGames = ((double)SubsetCount / numberOfGames * 100).RoundToSignificantDigits(2);
            PercentageOfWinsForBlack = ((double)BlackWins / SubsetCount * 100).RoundToSignificantDigits(2);
        }
	}
}
