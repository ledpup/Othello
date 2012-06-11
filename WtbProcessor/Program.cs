using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using Assets.Model;
using Reversi.Model;
using Reversi.Model.Evaluation;

namespace WtbProcessor
{
    class Program
    {
        static void Main(string[] args)
        {
            //var gameArchive = File.ReadAllLines("ArchiveData.txt").ToList();

            //var positionStats = new GameStateStats(@"\]U", gameArchive);

            //positionStats = new GameStateStats(@"\MBJ", gameArchive);

            //positionStats = new GameStateStats(@"\MCJD", gameArchive);
            

            //new ThorFileProcessor();
            //var function = new Func<ulong, ulong>(x => x);


            //DrawBoard(RotateIndices(function));
            //DrawBoard(RotateIndices(flipDiagA1H8));
            //DrawBoard(RotateIndices(function2));
            //DrawBoard(RotateIndices(rotate180));

            var computerPlayers = new[] { new ComputerPlayer(false), new ComputerPlayer(false) };

            var random = new Random();

            var depthFirstSearch = new DepthFirstSearch();

            var stopWatch = new Stopwatch();
            stopWatch.Start();

            for (var i = 0; i < 500; i++)
            {
                var gameManager = new GameManager();

                while (!gameManager.IsGameOver)
                {
                    if (!gameManager.HasPlays)
                    {
                        gameManager.NextTurn();
                        continue;
                    }

                    short? computerPlayIndex = null;

                    DepthFirstSearch.AnalysisNodeCollection.ClearMemory();
                    depthFirstSearch.GetPlay(gameManager, ref computerPlayIndex, computerPlayers[gameManager.PlayerIndex]);

                    //Console.Write(gameManager.Turn);
                    //gameManager.Draw();
                    //Console.WriteLine("Play " + computerPlayIndex.ToAlgebraicNotation());

                    gameManager.PlacePiece(computerPlayIndex);
                    gameManager.NextTurn();
                }

                var winner = gameManager.WinnerIndex;

                if (gameManager.IsDraw)
                    winner = (short)random.Next(1);

                Console.WriteLine("Game {0}, Time {1}", i, stopWatch.Elapsed);
                Console.WriteLine(gameManager.GameOverMessage);
                computerPlayers[winner].Draw();
                Console.WriteLine();

                ChangeWeightsForSelectedPlayer(winner, computerPlayers, random);

                SwapPlayers(computerPlayers);

                stopWatch.Restart();
            }
            
            Console.ReadKey();
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
