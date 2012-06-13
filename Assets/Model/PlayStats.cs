using System.Collections.Generic;
using System.Linq;

namespace Othello.Model
{
	public class PlayStats
	{
	    public string Position;
        public double PercentageOfGames;
        public double PercentageOfWinsForBlack;
	    public int SubsetCount;
	    public int BlackWins;
	    public int WhiteWins;
	    public int Draws;

        public PlayStats(int numberOfGames, List<string> subset, string position, char play)
        {
            var nextPosition = position + play;

            Position = nextPosition;

            subset = GameStateStats.GetSubset(nextPosition, subset, subset.Count());

            SubsetCount = subset.Count();

            BlackWins = subset.Where(y => y.Last() == 'B').Count();
            WhiteWins = subset.Where(y => y.Last() == 'W').Count();
            Draws = SubsetCount - BlackWins - WhiteWins;

            PercentageOfGames = ((double)SubsetCount / numberOfGames * 100).RoundToSignificantDigits(2);
            PercentageOfWinsForBlack = ((double)BlackWins / SubsetCount * 100).RoundToSignificantDigits(2);
        }

	    public double PercentageOfWinsForWhite
	    {
            get { return ((double)WhiteWins / SubsetCount * 100).RoundToSignificantDigits(2); }
	    }

        public double PercentageOfDraws
        {
            get { return ((double)Draws / SubsetCount * 100).RoundToSignificantDigits(2); }
        }
	}
}
