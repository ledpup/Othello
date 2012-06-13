using System;
using System.Collections.Generic;
using System.Linq;

namespace Othello.Model
{
	public class GameStateStats
	{
	    public Dictionary<short, PlayStats> PlayStats;
				
		public GameStateStats()
		{
			PlayStats = new Dictionary<short, PlayStats>();
		}

        public void GenerateStats(GameManager gameManager, List<string> games)
        {
            var canDrawStats = false;
            var statsAvailable = false;
            GenerateStats(gameManager, games, ref canDrawStats, ref statsAvailable);
        }

        public void GenerateStats(GameManager gameManager, List<string> games, ref bool canDrawStats, ref bool statsAvailable)
        {
            if (gameManager == null)
                throw new ArgumentNullException("gameManager");
			
			if (games == null)
				throw new ArgumentNullException("games");

            var position = gameManager.Plays.ToChars();
            PlayStats = GenerateStats(position, games);

            BoardSymmetries(gameManager, games, "FlipDiagA1H8");
            BoardSymmetries(gameManager, games, "FlipDiagA8H1");
            BoardSymmetries(gameManager, games, "Rotate180");

            statsAvailable = canDrawStats = true;
        }

	    private void BoardSymmetries(GameManager gameManager, List<string> games, string rotation)
	    {
	        var newPlayList = new List<short?>();
	        gameManager.Plays.ForEach(x =>
	                                      {
	                                          if (x == null)
	                                              return;

	                                          newPlayList.Add(GameState.RotateDictionary[rotation].IndicesMap[(short)x]);
	                                      });

	        var tempStats = GenerateStats(newPlayList.ToChars(), games);

	        foreach (var blah in tempStats)
	        {
	            var realKey = GameState.RotateDictionary[rotation].IndicesMap[blah.Key];
	            PlayStats.Add(realKey, blah.Value);
	        }
	    }

	    public Dictionary<short, PlayStats> GenerateStats(string position, List<string> games)
        {
            if (games == null)
                throw new ArgumentNullException("games");

            var numberOfGames = games.Count;

            var subset = games;
            int positionLength;

            if (string.IsNullOrEmpty(position))
            {
                positionLength = 0;
            }
            else
            {
                subset = GetSubset(position, games, numberOfGames);
                positionLength = position.Length;
            }

            var plays = subset.Select(x => char.Parse(x.Substring(positionLength, 1))).Distinct().ToList();

            var playStats = new Dictionary<short, PlayStats>();

            plays.ForEach(x => playStats.Add(x.ToIndex(), new PlayStats(numberOfGames, subset, position, x)));

            return playStats;
        }

	    public static List<string> GetSubset(string position, List<string> games, int numberOfGames)
	    {
	        var startIndex = ~games.BinarySearch(position, StringComparer.Ordinal);

	        var subset = new List<string>();

	        for (var i = startIndex; i < numberOfGames; i++)
	        {
	            var game = games[i];

	            if (game.StartsWith(position))
	                subset.Add(game);
	            else
	                break;
	        }

	        return subset;
	    }
	}
}
