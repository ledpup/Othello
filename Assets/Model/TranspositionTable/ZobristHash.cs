using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Reversi.Model;

namespace Reversi.Model.TranspositionTable
{
    /// <summary>
    /// Zobrist hash to take the two ulong bitboard of the GameState and transform it to a single ulong.
    /// Used in a HashTable for a transposition table and other uses.
    /// http://en.wikipedia.org/wiki/Zobrist_hashing
    /// http://chessprogramming.wikispaces.com/Zobrist+Hashing
    /// http://chessprogramming.wikispaces.com/Transposition+Table
    /// </summary>
    public class ZobristHash
    {
        private const int BoardSize = 64;

        private Dictionary<short, ulong[]> _randomNumbers;
        private ulong blackRandom;

        public ZobristHash()
        {
            
            // Generate random numbers for each position on the board, for possible state (empty, player, opponent)
            if (_randomNumbers == null)
                _randomNumbers = new Dictionary<short, ulong[]>();

            var random = new Random(0); // Always use the same seed (so we can persist the hashes)

            for (var i = (short) 0; i < 3; i++)
            {
                var boardRandomNumbers = new ulong[BoardSize];
                for (var j = 0; j < BoardSize; j++)
                {
                    boardRandomNumbers[j] = random.NextUlong();
                }
                _randomNumbers.Add(i, boardRandomNumbers);
            }

            blackRandom = random.NextUlong();
        }

        
        public ulong Hash(GameState gameState, bool blackToPlay)
        {
            var pieces = new[]
                             {
                                 gameState.EmptySquares,
                                 gameState.PlayerPieces,
                                 gameState.OpponentPieces,
                             };

            var hash = 0UL;
            for (var boardIndex = 0; boardIndex < BoardSize; boardIndex++)
            {
                var position = 1UL << boardIndex;
                for (var pieceTypeIndex = (short)0; pieceTypeIndex < 3; pieceTypeIndex++)
                {
                    if ((pieces[pieceTypeIndex] & position) > 0)
                        hash ^= _randomNumbers[pieceTypeIndex][boardIndex];
                }
            }

            if (blackToPlay)
                hash ^= blackRandom;

            return hash;
        }
    }

    public static class RandomLong
    {
        public static long Nextlong(this Random random)
        {
            return (long)((random.NextDouble() * 2.0 - 1.0) * long.MaxValue);
        }

        public static ulong NextUlong(this Random random)
        {
            return (ulong)(random.NextDouble() * ulong.MaxValue);
        }
    }
}
