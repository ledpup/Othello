using System;
using System.Collections.Generic;
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

            


            var weights = new []
                           { 
                               new Dictionary<string, float>{
                                   { "Pieces", 1f },
                                   { "Mobility", 1f },
                                   { "PotentialMobility", 1f },
                                   { "Parity", 1f },
                                   { "PositionValues", 1f }
		                       },
                               new Dictionary<string, float>{
                                   { "Pieces", 1f },
                                   { "Mobility", 1f },
                                   { "PotentialMobility", 1f },
                                   { "Parity", 1f },
                                   { "PositionValues", 1f }
		                       },
                           };

            var random = new Random();

            for (var i = 0; i < 100; i++)
            {
                var gameManager = new GameManager();

                while (!gameManager.GameState.IsGameOver)
                {
                    if (!gameManager.HasPlays)
                    {
                        gameManager.NextTurn();
                        continue;
                    }

                    short? computerPlayIndex = null;

                    DepthFirstSearch.GetPlay(gameManager, weights[gameManager.PlayerIndex], ref computerPlayIndex);

                    Console.WriteLine("Play " + computerPlayIndex.ToAlgebraicNotation());

                    gameManager.PlacePiece(computerPlayIndex);
                    gameManager.NextTurn();
                }
                Console.WriteLine(gameManager.GameOverMessage);

                ChangeWeightsForSelectedPlayer(gameManager.WinnerIndex, weights, random);

                SwapPlayers(weights);
            }
            
            Console.ReadKey();
        }

        private static void SwapPlayers(Dictionary<string, float>[] weights)
        {
            var tempWeights = new Dictionary<string, float>();

            foreach (var weight in weights[0])
                tempWeights[weight.Key] = weight.Value;

            foreach (var key in weights[1].Keys.ToList())
            {
                weights[0][key] = weights[1][key];
                weights[1][key] = tempWeights[key];
            }
        }

        private static void ChangeWeightsForSelectedPlayer(short winner, IList<Dictionary<string, float>> weights, Random random)
        {
            if (winner == -1)
                winner = (short)random.Next(1);

            var playerToChange = 1 - winner;

            foreach (var weight in weights[winner])
                weights[playerToChange][weight.Key] = weight.Value;

            var weightToChange = random.Next(weights[playerToChange].Count);
            var keyToChange = weights[playerToChange].Keys.ElementAt(weightToChange);
            weights[playerToChange][keyToChange] = (float)random.NextDouble();
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
