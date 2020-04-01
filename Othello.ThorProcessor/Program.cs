using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Othello.ThorProcessor
{
    class Program
    {
        static void Main(string[] args)
        {
            var players = ThorFileReader.ReadPlayersFile(@"DataFiles\WTHOR.JOU");
            var tournaments = ThorFileReader.ReadTournamentFile(@"DataFiles\WTHOR.TRN");

            var files = Directory.GetFiles(@"DataFiles", "*.wtb").OrderBy(x => x).ToList();
            if (!files.Any())
            {
                throw new Exception("No Thor DB files can be found.");
            }
            var games = new List<ThorGame>();

            files.ForEach(x => 
            {
                games.AddRange(ThorFileReader.ReadThorGameFile(x));
            });


            var serialisedGames = ThorFileReader.BuildOpeningBook(games, tournaments, players);
        }
    }
}
