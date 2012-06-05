using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reversi.Model.Evaluation
{
	public static class Patterns
	{
	    public static List<ulong> Corner = AllPermutations("a1".ToBitBoard());
        public static List<ulong> XSquare = AllPermutations("b2".ToBitBoard());
        public static List<ulong> CornerAndXSquare = AllPermutations("a1".ToBitBoard() | "b2".ToBitBoard());
        public static List<ulong> CSquare = AllPermutations("a2".ToBitBoard() | "b1".ToBitBoard());
        public static List<ulong> CornerAndCSquare = AllPermutations("a1".ToBitBoard() | "a2".ToBitBoard() | "b1".ToBitBoard());

        public static List<ulong> Edges = AllPermutations(255);

	    public static ulong Edge = CombinedSymmetries(255);

        public static int NumberOfEdgeSquares = Edge.CountBits();

	    public static List<ulong> AllEdges
        {
            get
            {
                if (_allEdgeBitBoards != null)
                    return _allEdgeBitBoards;

                _allEdgeBitBoards = new List<ulong>();

                for (ulong i = 1; i < 255; i++)
                {
                    var combos = AllPermutations(i);

                    combos.ForEach(x =>
                                       {
                                           if (!_allEdgeBitBoards.Contains(x))
                                               _allEdgeBitBoards.Add(x);
                                       });
                }
                return _allEdgeBitBoards;
            }
        }
        private static List<ulong> _allEdgeBitBoards;

        static List<ulong> AllPermutations(ulong bitboard)
        {
            return new List<ulong>
                             {
                                bitboard,
                                BitBoardHelper.Rotations[1](bitboard),
                                BitBoardHelper.Rotations[2](bitboard),
                                BitBoardHelper.Rotations[3](bitboard),
                                BitBoardHelper.Rotations[0](bitboard) | BitBoardHelper.Rotations[1](bitboard),
                                BitBoardHelper.Rotations[0](bitboard) | BitBoardHelper.Rotations[2](bitboard),
                                BitBoardHelper.Rotations[0](bitboard) | BitBoardHelper.Rotations[3](bitboard),
                                BitBoardHelper.Rotations[1](bitboard) | BitBoardHelper.Rotations[2](bitboard),
                                BitBoardHelper.Rotations[1](bitboard) | BitBoardHelper.Rotations[3](bitboard),
                                BitBoardHelper.Rotations[2](bitboard) | BitBoardHelper.Rotations[3](bitboard),
                                BitBoardHelper.Rotations[0](bitboard) | BitBoardHelper.Rotations[1](bitboard) | BitBoardHelper.Rotations[2](bitboard),
                                BitBoardHelper.Rotations[0](bitboard) | BitBoardHelper.Rotations[1](bitboard) | BitBoardHelper.Rotations[3](bitboard),
                                BitBoardHelper.Rotations[0](bitboard) | BitBoardHelper.Rotations[2](bitboard) | BitBoardHelper.Rotations[3](bitboard),
                                BitBoardHelper.Rotations[1](bitboard) | BitBoardHelper.Rotations[2](bitboard) | BitBoardHelper.Rotations[3](bitboard),
                                BitBoardHelper.Rotations[0](bitboard) | BitBoardHelper.Rotations[1](bitboard) | BitBoardHelper.Rotations[2](bitboard) | BitBoardHelper.Rotations[3](bitboard),
                             };
        }

        public static ulong CombinedSymmetries(ulong bitBoard)
        {
            return bitBoard | bitBoard.Rotate90Clockwise() | bitBoard.Rotate180() | bitBoard.Rotate90AntiClockwise();
        }

        public static List<ulong> Symmetries(ulong bitBoard)
        {
            return new List<ulong> { bitBoard, bitBoard.Rotate90Clockwise(), bitBoard.Rotate180(), bitBoard.Rotate90AntiClockwise(), };
        }
	}
}
