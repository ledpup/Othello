using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Othello.Model;
using Othello.Model.Evaluation;

namespace WtbProcessor
{
    class Program
    {
        static void Main(string[] args)
        {
            var gameArchive = File.ReadAllLines("ArchiveData.txt").ToList();
            var gameStateStats = new GameStateStats();

            var computerPlayers = new[] { new ComputerPlayer(), new ComputerPlayer() };

            var random = new Random();

            var depthFirstSearch = new DepthFirstSearch();

            var stopWatch = new Stopwatch();
            stopWatch.Start();

            for (var i = 0; i < 500; i++)
            {
                var gameManager = new GameManager();

                Console.WriteLine("Game {0} ", i);

                var score = 0;
                for (var round = 0; round < 2; round++)
                {
                    Console.WriteLine("Round {0}", round);

                    PlayGame(gameManager, gameStateStats, gameArchive, depthFirstSearch, computerPlayers);

                    score += GetScore(round, gameManager);

                    Console.WriteLine("Score {0} ", score);
                    Console.WriteLine(gameManager.GameOverMessage);

                    SwapPlayers(computerPlayers);
                }

                short winner;
                if (score == 0)
                    winner = (short)random.Next(1);
                else if (score > 0)
                    winner = 1;
                else
                    winner = 0;

                Console.WriteLine("Time {0}", stopWatch.Elapsed);
                
                computerPlayers[winner].Draw();
                Console.WriteLine();

                ChangeWeightsForSelectedPlayer(winner, computerPlayers, random);

                SwapPlayers(computerPlayers);

                stopWatch.Restart();
            }
            
            Console.ReadKey();
        }

        private static int GetScore(int round, GameManager gameManager)
        {
            if (gameManager.IsDraw)
                return 0;

            var winner = gameManager.WinnerIndex;

            if (round == 0)
                return (int)SearchAlgorithms.Sign[winner];
            
            return (int)SearchAlgorithms.Sign[winner] * -1;
        }

        private static void PlayGame(GameManager gameManager, GameStateStats gameStateStats, List<string> gameArchive, DepthFirstSearch depthFirstSearch, ComputerPlayer[] computerPlayers)
        {
            while (!gameManager.IsGameOver)
            {
                if (!gameManager.HasPlays)
                {
                    gameManager.NextTurn();
                    continue;
                }

                short? computerPlayIndex = null;

                gameStateStats.GenerateStats(gameManager, gameArchive);

                DepthFirstSearch.AnalysisNodeCollection.ClearMemory();
                depthFirstSearch.GetPlayWithBook(gameManager, gameStateStats, computerPlayers[gameManager.PlayerIndex], ref computerPlayIndex);

                //Console.Write(gameManager.Turn);
                //gameManager.Draw();
                //Console.WriteLine("Play " + computerPlayIndex.ToAlgebraicNotation());

                gameManager.PlacePiece(computerPlayIndex);
                gameManager.NextTurn();
            }
        }

        private static void SwapPlayers(ComputerPlayer[] computerPlayers)
        {
            var temp = computerPlayers[0];
            computerPlayers[0] = computerPlayers[1];
            computerPlayers[1] = temp;
        }

        private static void ChangeWeightsForSelectedPlayer(short winner, ComputerPlayer[] computerPlayers, Random random)
        {
            var playerToChange = 1 - winner;

            var changingPlayer = computerPlayers[playerToChange];

            var winnerPlayer = computerPlayers[winner];

            for (short phase = 0; phase < changingPlayer.Weights.Length; phase++)
            {
                for (var weight = 0; weight < changingPlayer.Strategies.Count; weight++)
                {
                    var key = changingPlayer.Weights[phase].Keys.ElementAt(weight);
                    changingPlayer.Weights[phase][key] = winnerPlayer.Weights[phase][key];
                }
            }
            
            ChangeOneStrategy(changingPlayer, random);
        }

        private static void ChangeOneStrategy(ComputerPlayer changingPlayer, Random random)
        {
            var phase = random.Next(changingPlayer.NumberOfGamePhases);
            var weight = random.Next(changingPlayer.Strategies.Count);
            var key = changingPlayer.Weights[phase].Keys.ElementAt(weight);
            changingPlayer.Weights[phase][key] = (float)random.NextDouble();
        }

        private static void DrawBoard(IDictionary<short, short> dic)
        {
            for (short i = 0; i < 64; i++)
            {
                if (i % 8 == 0)
                    Console.WriteLine();

                Console.Write(dic[i] + " ");
            }
            Console.WriteLine();
        }
    }
}
