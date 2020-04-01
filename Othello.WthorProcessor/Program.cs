using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Othello.WthorProcessor
{
    class Program
    {
        static void Main(string[] args)
        {
            var players = WthorFileLoader.ReadPlayersFile(@"DataFiles\WTHOR.JOU");
            var tournaments = WthorFileLoader.ReadTournamentFile(@"DataFiles\WTHOR.TRN");

            var files = Directory.GetFiles(@"DataFiles", "*.wtb").OrderBy(x => x).ToList();
            if (!files.Any())
            {
                throw new Exception("No Thor DB files can be found.");
            }
            var games = new List<WthorGame>();

            files.ForEach(x => 
            {
                games.AddRange(WthorFileLoader.ReadWthorGameFile(x));
            });

            var serialisedGames = WthorFileLoader.BuildOpeningBook(games, tournaments, players);
        }
    }
}
