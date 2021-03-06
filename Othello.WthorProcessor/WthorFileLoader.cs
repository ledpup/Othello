﻿using Othello.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Othello.WthorProcessor
{
    public static class WthorFileLoader
    {
        public static Dictionary<int, string> ReadTournamentFile(string fileName)
        {
            var fileInfo = new FileInfo(fileName);
            if (!fileInfo.Exists)
            {
                throw new FileNotFoundException($"Tournament file {fileName} not found", fileName);
            }
            if (fileInfo.Extension.ToUpper() != ".TRN")
            {
                throw new Exception($"File extension is {fileInfo.Extension}. WTHOR tournament file is a .TRN file extension.");
            }    

            var array = File.ReadAllBytes(fileName);

            var tournaments = ReadReferenceFile(array, 26);
            return tournaments;
        }

        public static Dictionary<int, string> ReadPlayersFile(string fileName)
        {
            var fileInfo = new FileInfo(fileName);
            if (!fileInfo.Exists)
            {
                throw new FileNotFoundException($"Players file {fileName} not found", fileName);
            }
            if (fileInfo.Extension.ToUpper() != ".JOU")
            {
                throw new Exception($"File extension is {fileInfo.Extension}. WTHOR players file is a .JOU file extension.");
            }

            var array = File.ReadAllBytes(fileName);

            var players = ReadReferenceFile(array, 20);
            return players;
        }

        public static List<WthorGame> ReadWthorGameFile(string fileName)
        {
            var fileInfo = new FileInfo(fileName);
            if (!fileInfo.Exists)
            {
                throw new FileNotFoundException($"WTHOR DB file {fileName} not found", fileName);
            }
            if (fileInfo.Extension.ToUpper() != ".WTB")
            {
                throw new Exception($"File extension is {fileInfo.Extension}. WTHOR game database file is a .WTB file extension.");
            }

            var wthorGames = new List<WthorGame>();

            var data = File.ReadAllBytes(fileName);
            wthorGames.AddRange(GetWthorGamesFromDbFile(data));

            return wthorGames;
        }

        public static string[] BuildOpeningBook(List<WthorGame> games, IDictionary<int, string> tournaments, IDictionary<int, string> players)
        {
            var serialisedGames = new List<string>();
            games.ForEach(x => 
            {
                var gameManager = ConvertWthorGameToGameManager(x, players);
                if (gameManager != null)
                {
                    serialisedGames.Add(SerialiseGameToOpeningBookFormat(gameManager));
                }
                else
                {
                    // throw new Exception($"Game between {players[x.BlackId]} and {players[x.WhiteId]} in tournament {tournaments[x.TournamentId]} is invalid as it was not finished. Game state is {x.SerialisedPlays}.");
                }   
            });

            var array = serialisedGames.ToArray();

            Array.Sort(array, StringComparer.Ordinal);

            return array;
        }


        static string SerialiseGameToOpeningBookFormat(GameManager gameManager)
        {
            var stringBuilder = new StringBuilder(gameManager.Plays.ToChars());
            stringBuilder.Append(",");
            stringBuilder.Append(gameManager.Winner);

            return stringBuilder.ToString();
        }

        public static GameManager ConvertWthorGameToGameManager(WthorGame game, IDictionary<int, string> players)
        {
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

            if (gameManager.IsGameOver)
            {
                if (game.BlackScore != gameManager.BlackScore)
                {
                    throw new Exception(string.Format("Invalid game because the calculated score ({0}) does not match the recorded score ({1}).", gameManager.BlackScore, game.BlackScore));
                }
                return gameManager;
            }
            // Game not finished
            return null;
        }

        private static Dictionary<int, string> ReadReferenceFile(byte[] fileContent, int blockLength)
        {
            int headerLength = 16;
            int nameLength = blockLength - 1;

            var dictionary = new Dictionary<int, string>();

            var id = 0;
            for (var i = headerLength; i < fileContent.Length; i += blockLength)
            {
                var characters = new char[nameLength];
                for (var j = 0; j < nameLength; j++)
                {
                    characters[j] = (char)fileContent[i + j];
                }
                var name = (new string(characters)).Trim('\0');

                dictionary.Add(id, name);
                id++;
            }

            return dictionary;
        }

        public static List<WthorGame> GetWthorGamesFromDbFile(byte[] fileContent)
        {
            const int fileHeaderLength = 16;
            const int gameHeaderLength = 8;
            const int numberOfPlays = 60;
            const int gameBlockLength = gameHeaderLength + numberOfPlays;

            if (fileContent[12] != 8)
                throw new Exception("WThor processor only supports 8x8 boards");

            var games = new List<WthorGame>();

            for (var i = fileHeaderLength; i < fileContent.Length; i += gameBlockLength)
            {
                var game = new WthorGame
                {
                    TournamentId = BitConverter.ToInt16(new[] { fileContent[i], fileContent[i + 1] }, 0),
                    BlackId = BitConverter.ToInt16(new[] { fileContent[i + 2], fileContent[i + 3] }, 0),
                    WhiteId = BitConverter.ToInt16(new[] { fileContent[i + 4], fileContent[i + 5] }, 0),
                    BlackScore = fileContent[i + 6],
                    TheoreticalScore = fileContent[i + 7],
                };

                for (var playIndex = 0; playIndex < numberOfPlays; playIndex++)
                {
                    var play = fileContent[i + gameHeaderLength + playIndex];

                    if (play > 0)
                    {
                        game.Plays.Add(play.ToAlgebraicNotation());
                    }
                }
                games.Add(game);
            }
            return games;
        }
    }
}
