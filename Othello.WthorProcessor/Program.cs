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
            var folder = args.Length > 0 ? args[0] : "DataFiles";
            var outputFile = args.Length > 1 ? args[1] : "WthorOpeningBook.txt";

            var players = WthorFileLoader.ReadPlayersFile($@"{folder}\WTHOR.JOU");
            var tournaments = WthorFileLoader.ReadTournamentFile($@"{folder}\WTHOR.TRN");

            var files = Directory.GetFiles(folder, "*.wtb").OrderBy(x => x).ToList();
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

            File.WriteAllLines(outputFile, serialisedGames);
        }
    }
}
