using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Othello.Model;

namespace Othello.Model.Thor
{
    public class ThorFileProcessor
    {
        private Dictionary<int, List<ThorGame>> _gameDatabase;

        public ThorFileProcessor()
        {
            var tournaments = ReadReferenceFile(@"Thor\WTHOR.TRN", 26);
            var players = ReadReferenceFile(@"Thor\WTHOR.JOU", 20);
            TournamentFiles(tournaments, players);

            //Console.WriteLine("Press any key to continue.");
            //Console.ReadKey();
        }

        private void TournamentFiles(IDictionary<int, string> tournaments, IDictionary<int, string> players)
        {
            _gameDatabase = new Dictionary<int, List<ThorGame>>();

            var files = Directory.GetFiles(@"Thor", "*.wtb").OrderBy(x => x).ToList();

            if (!files.Any())
            {
                throw new Exception("No Thor DB files can be found.");
            }

            files.ToList().ForEach(f => 
                {
                    var databaseYear = ReadThorDb(f);
                    _gameDatabase.Add(databaseYear.Key, databaseYear.Value);
                });

            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            var numberOfGames = 0;

            var games = new List<string>();

            _gameDatabase.Keys
                .ToList()
                .ForEach(year => _gameDatabase[year]
                    .ForEach(g =>
                                 {
                                     numberOfGames++;
                                     var game = ProcessGame(g, year, tournaments, players);
                                     if (!string.IsNullOrEmpty(game))
                                         games.Add(game);
                                 }));

            var orderedGames = games.ToArray();

            Array.Sort(orderedGames, StringComparer.Ordinal);

            //games.Sort();
            //var orderedGames = games.Sort();// games.OrderBy(x => x).ToList();

            orderedGames.ToList().ForEach(Console.WriteLine);

            stopwatch.Stop();

            //Console.WriteLine();
            //Console.WriteLine("Number of games = {0}", numberOfGames);
            //Console.WriteLine("Elapsed time = {0}", stopwatch.Elapsed);
            //Console.WriteLine("Games processed per second = {0}", numberOfGames / stopwatch.ElapsedMilliseconds * 1000);
        }

        private static string ProcessGame(ThorGame game, int year, IDictionary<int, string> tournaments, IDictionary<int, string> players)
        {
            //Console.WriteLine();
            //Console.WriteLine("{0} ({1})", tournaments[game.TournamentId], year);
            //Console.WriteLine("{0} vs {1}", players[game.BlackId], players[game.WhiteId]);
            //Console.WriteLine("Black Score: {0}", game.BlackScore);

            var plays = GameManager.DeserialsePlays(game.SerialisedPlays);

            var gameManager = new GameManager 
            { 
                BlackName = players[game.BlackId], 
                WhiteName = players[game.WhiteId],
            };

            plays.ForEach(play =>
            {
                if (!gameManager.CanPlay((short)play))
                {
                    gameManager.PlacePiece(null);
                    gameManager.NextTurn();
                }

                gameManager.PlacePiece(play);
                gameManager.NextTurn();
            });

            Console.WriteLine(GameManager.SerialsePlays(plays));

            if (gameManager.IsGameOver)
            {
                if (game.BlackScore != gameManager.BlackScore)
                    throw new Exception(string.Format("At gameover, calculated score ({0}) does not match recorded score ({1}).", gameManager.BlackScore, game.BlackScore));

                var stringBuilder = new StringBuilder(gameManager.Plays.ToChars());
                stringBuilder.Append(",");
                stringBuilder.Append(gameManager.Winner);

                return stringBuilder.ToString();
            }

            return null;
            
            

            //else if (game.BlackScore != 0 && game.BlackScore != gameManager.BlackPieces.CountBits())
            //    Console.WriteLine(string.Format("At resignation, number of black pieces ({0}) does not match recorded score ({1}).", gameManager.BlackPieces.CountBits(), game.BlackScore));
        }

        public static Dictionary<int, string> ReadReferenceFile(string f, int blockLength)
        {
            int headerLength = 16;
            int nameLength = blockLength - 1;

            var array = File.ReadAllBytes(f);

            var dictionary = new Dictionary<int, string>();

            var id = 0;
            for (var i = headerLength; i < array.Length; i += blockLength)
            {
                var characters = new char[nameLength];
                for (var j = 0; j < nameLength; j++)
                {
                    characters[j] = (char)array[i + j];
                }
                var name = (new string(characters)).Trim('\0');

                dictionary.Add(id, name);
                id++;
            }

            return dictionary;
        }

        public static KeyValuePair<int, List<ThorGame>> ReadThorDb(string f)
        {
            const int fileHeaderLength = 16;
            int gameHeaderLength = 8;
            int numberOfPlays = 60;
            int gameBlockLength = gameHeaderLength + numberOfPlays;

            var thorArray = File.ReadAllBytes(f);

            var year = int.Parse(f.Substring(9, 4));
            
            if (thorArray[12] != 8)
                throw new Exception("Thor processor only supports 8x8 boards");

            var gameDatabase = new List<ThorGame>();

            for (var i = fileHeaderLength; i < thorArray.Length; i += gameBlockLength)
            {
                var game = new ThorGame
                {
                    TournamentId = BitConverter.ToInt16(new [] { thorArray[i], thorArray[i + 1] }, 0),
                    BlackId = BitConverter.ToInt16(new[] { thorArray[i + 2], thorArray[i + 3] }, 0),
                    WhiteId = BitConverter.ToInt16(new[] { thorArray[i + 4], thorArray[i + 5] }, 0),
                    BlackScore = thorArray[i + 6],
                    TheoreticalScore = thorArray[i + 7],
                };

                for (var playIndex = 0; playIndex < numberOfPlays; playIndex++)
                {
                    var play = thorArray[i + gameHeaderLength + playIndex];

                    if (play > 0)
                    {
                        game.Plays.Add(play.ToAlgebraicNotation());
                    }
                }
                gameDatabase.Add(game);
            }
            return new KeyValuePair<int, List<ThorGame>>(year, gameDatabase);
        }
    }
}
