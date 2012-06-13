using System;
using System.Linq;
using System.Collections.Generic;
using Othello.Model.Evaluation;

namespace Othello.Model
{
    public struct GameState
    {
        public ulong PlayerPieces;
        public ulong OpponentPieces;
        public readonly short NumberOfOpponentPieces;
        public readonly short NumberOfPlayerPieces;
        public readonly ulong PlayerPlays;
        public readonly ulong OpponentPlays;

        static readonly Func<ulong, ulong> flipDiagA1H8 = x => x.FlipDiagA1H8();
        static readonly Func<ulong, ulong> flipDiagA8H1 = x => x.FlipDiagA8H1();
        static readonly Func<ulong, ulong> rotate180 = x => x.Rotate180();

        //public ulong Placement;
        //public ulong FlippedPieces;

        public static Dictionary<string, Rotation> RotateDictionary = new Dictionary<string, Rotation>
                                       { 
                                           {"FlipDiagA1H8", new Rotation(flipDiagA1H8, RotateIndices(flipDiagA1H8)) },
                                           {"FlipDiagA8H1", new Rotation(flipDiagA8H1, RotateIndices(flipDiagA8H1)) },
                                           {"Rotate180", new Rotation(rotate180, RotateIndices(rotate180)) },
                                       };

        public GameState(ulong playerPieces, ulong opponentPieces) : this()
        {
            PlayerPieces = playerPieces;
            OpponentPieces = opponentPieces;
            NumberOfPlayerPieces = PlayerPieces.CountBits();
            NumberOfOpponentPieces = OpponentPieces.CountBits();

            var emptySquares = AllPieces ^ ulong.MaxValue;
            PlayerPlays = Play.ValidPlays(PlayerPieces, OpponentPieces, emptySquares);
            OpponentPlays = Play.ValidPlays(OpponentPieces, PlayerPieces, emptySquares);
        }

        //public IEnumerable<INode> Children;
        //public int Evaluation { get { return Colour == 0 ? -Children.Sum(o => o.WhiteMobility) : Children.Sum(o => o.BlackMobility); } }
		
		public static GameState NewGame()
		{
			return new GameState(1UL << 28 | 1UL << 35, 1UL << 27 | 1UL << 36);
		}

        public List<short> PlacePiece(short index)
		{
            var placement = 1UL << index;

            var flippedPieces = Play.PlacePiece(placement, PlayerPieces, OpponentPieces);

            PlayerPieces |= flippedPieces | placement;
            OpponentPieces ^= flippedPieces;

            return flippedPieces.Indices().ToList();
		}

        public GameState NextTurn()
        {
            return new GameState(OpponentPieces, PlayerPieces);
        }

        public ulong AllPieces
        {
            get { return PlayerPieces | OpponentPieces; }
        }

        public ulong EmptySquares { get { return AllPieces ^ ulong.MaxValue; } }

        public bool IsGameOver { get { return PlayerPlays == 0 && OpponentPlays == 0; } }

        public bool PlayerWinning { get { return NumberOfPlayerPieces > NumberOfOpponentPieces; } }
        public bool OpponentWinning { get { return NumberOfPlayerPieces < NumberOfOpponentPieces; } }
        public bool IsDraw { get { return NumberOfPlayerPieces == NumberOfOpponentPieces; } }
		
        public bool HasPlays { get { return PlayerPlays > 0; } }

        public GameState Rotate180()
        {
            return new GameState(PlayerPieces.Rotate180(), OpponentPieces.Rotate180());
        }

        public GameState FlipDiagA1H8()
        {
            return new GameState(PlayerPieces.FlipDiagA1H8(), OpponentPieces.FlipDiagA1H8());
        }

        public GameState FlipDiagA8H1()
        {
            return new GameState(PlayerPieces.FlipDiagA8H1(), OpponentPieces.FlipDiagA8H1());
        }

        public void Draw()
        {
            for (var i = 0; i < 64; i++)
            {
                if (i % 8 == 0)
                    Console.WriteLine();

                var pos = 1UL << i;
                if ((PlayerPieces & pos) > 0)
                    Console.Write("P");
                else if ((PlayerPlays & pos) > 0)
                    Console.Write("x");
                else if ((OpponentPieces & pos) > 0)
                    Console.Write("O");
                else
                    Console.Write(".");
            }
            Console.WriteLine();
        }

        public override bool Equals(object obj)
        {
            var comparedGameState = (GameState)obj;
            return (PlayerPieces == comparedGameState.PlayerPieces) && (OpponentPieces == comparedGameState.OpponentPieces);
        }

        public override int GetHashCode()
        {
            return (PlayerPieces | OpponentPieces).GetHashCode();
        }

        public GameState Rotate(Func<ulong, ulong> rotateFunc)
        {
            return new GameState(rotateFunc(PlayerPieces), rotateFunc(OpponentPieces));
        }

        private static Dictionary<short, short> RotateIndices(Func<ulong, ulong> rotateFunction)
        {
            var roatatedIndices = new Dictionary<short, short>();
            for (var i = 0; i < 64; i++)
            {
                var original = 1UL << i;

                var newPosition = rotateFunction(original);

                roatatedIndices.Add(original.Indices().Single(), newPosition.Indices().Single());
            }
            return roatatedIndices;
        }


    }
}
