using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Othello.Model
{
    public static class BitBoardHelper
    {
        // From http://chessprogramming.wikispaces.com/Population+Count#SWAR-Popcount-The%20PopCount%20routine
        public static short CountBits(this ulong x)
        {
            const ulong k1 = 0x5555555555555555; /*  -1/3   */
            const ulong k2 = 0x3333333333333333; /*  -1/5   */
            const ulong k4 = 0x0f0f0f0f0f0f0f0f; /*  -1/17  */
            const ulong kf = 0x0101010101010101; /*  -1/255 */

            x = x - ((x >> 1) & k1);        /* put count of each 2 bits into those 2 bits */
            x = (x & k2) + ((x >> 2) & k2); /* put count of each 4 bits into those 4 bits */
            x = (x + (x >> 4)) & k4;        /* put count of each 8 bits into those 8 bits */
            x = (x * kf) >> 56;             /* returns 8 most significant bits of x + (x<<8) + (x<<16) + (x<<24) + ...  */

            return (short)x;
        }

        // Uses the BitScanForward method to get the indices of bits containing ones.
		// From http://chessprogramming.wikispaces.com/Bitboard+Serialization
        public static IEnumerable<short> Indices(this ulong x)
        {
            var indices = new List<short>();		
			if (x > 0)
			{
				do 
				{
					short index = BitScanForward(x); 	// square index from 0..63
					indices.Add(index);
				} while ((x &= x - 1) > 0); 			// reset LS1B
			}
            return indices;
        }

        public static ulong EmptySquares(ulong pieces1, ulong pieces2)
        {
            return pieces1 & pieces2 ^ UInt64.MaxValue;
        }

        // http://code.google.com/p/vajolet/source/browse/vajolet/chesslib/bitboard.cs?spec=svn47&r=47
        public static short BitScanForward(ulong bitBoard)
        {
            return Debruijn64Array[(((ulong)((long)bitBoard & -(long)bitBoard)) * Debruijn64) >> 58];
        }

        private static readonly short[] Debruijn64Array = new short[]{
                63,  0, 58,  1, 59, 47, 53,  2,
                60, 39, 48, 27, 54, 33, 42,  3,
                61, 51, 37, 40, 49, 18, 28, 20,
                55, 30, 34, 11, 43, 14, 22,  4,
                62, 57, 46, 52, 38, 26, 32, 41,
                50, 36, 17, 19, 29, 10, 13, 21,
                56, 45, 25, 31, 35, 16,  9, 12,
                44, 24, 15,  8, 23,  7,  6,  5,
        };

        const long Debruijn64 = 0x07EDD5E59A4E28C2;

        public static Func<ulong,ulong>[] Rotations = new Func<ulong, ulong>[] {x => x, x => x.Rotate90Clockwise(), x => x.Rotate180(), x => x.Rotate90AntiClockwise() };

        public static ulong Rotate90Clockwise(this ulong x)
        {
            return FlipVertical(FlipDiagA1H8(x));
        }

        public static ulong Rotate180(this ulong x)
        {
            return MirrorHorizontal(FlipVertical(x));
        }

        public static ulong Rotate90AntiClockwise(this ulong x)
        {
            return FlipDiagA1H8(FlipVertical(x));
        }

        static ulong MirrorHorizontal(this ulong x)
        {
            const ulong k1 = 0x5555555555555555;
            const ulong k2 = 0x3333333333333333;
            const ulong k4 = 0x0f0f0f0f0f0f0f0f;
            x = ((x >> 1) & k1) | ((x & k1) << 1);
            x = ((x >> 2) & k2) | ((x & k2) << 2);
            x = ((x >> 4) & k4) | ((x & k4) << 4);
            return x;
        }

        public static ulong FlipVertical(this ulong x)
        {
            const ulong k1 = 0x00FF00FF00FF00FF;
            const ulong k2 = 0x0000FFFF0000FFFF;
            x = ((x >> 8) & k1) | ((x & k1) << 8);
            x = ((x >> 16) & k2) | ((x & k2) << 16);
            x = (x >> 32) | (x << 32);
            return x;
        }

        public static ulong FlipDiagA1H8(this ulong x)
        {
            const ulong k1 = 0x5500550055005500;
            const ulong k2 = 0x3333000033330000;
            const ulong k4 = 0x0f0f0f0f00000000;
            var t = k4 & (x ^ (x << 28));
            x ^= t ^ (t >> 28);
            t = k2 & (x ^ (x << 14));
            x ^= t ^ (t >> 14);
            t = k1 & (x ^ (x << 7));
            x ^= t ^ (t >> 7);
            return x;
        }

        public static ulong FlipDiagA8H1(this ulong x)
        {
            const ulong k1 = 0xaa00aa00aa00aa00;
            const ulong k2 = 0xcccc0000cccc0000;
            const ulong k4 = 0xf0f0f0f00f0f0f0f;
            var t = x ^ (x << 36);
            x ^= k4 & (t ^ (x >> 36));
            t = k2 & (x ^ (x << 18));
            x ^= t ^ (t >> 18);
            t = k1 & (x ^ (x << 9));
            x ^= t ^ (t >> 9);
            return x;
        }

        public static void Draw(this ulong x)
        {
            for (var i = 0; i < 64; i++)
            {
                if (i % 8 == 0)
                    Console.WriteLine();

                var pos = 1UL << i;
                Console.Write((x & pos) > 0 ? "x" : ".");
            }
            Console.WriteLine();
        }

        //public static ulong PseudoRotate45Clockwise(ulong x)
        //{
        //    const ulong k1 = 0xAAAAAAAAAAAAAAAA;
        //    const ulong k2 = 0xCCCCCCCCCCCCCCCC;
        //    const ulong k4 = 0xF0F0F0F0F0F0F0F0;
        //    x ^= k1 & (x ^ RotateRight(x, 8));
        //    x ^= k2 & (x ^ RotateRight(x, 16));
        //    x ^= k4 & (x ^ RotateRight(x, 32));
        //    return x;
        //}

        //public static ulong PseudoRotate45AntiClockwise(ulong x)
        //{
        //    const ulong k1 = 0x5555555555555555;
        //    const ulong k2 = 0x3333333333333333;
        //    const ulong k4 = 0x0f0f0f0f0f0f0f0f;
        //    x ^= k1 & (x ^ RotateRight(x, 8));
        //    x ^= k2 & (x ^ RotateRight(x, 16));
        //    x ^= k4 & (x ^ RotateRight(x, 32));
        //    return x;
        //}

        //static ulong RotateRight(ulong x, int s)
        //{
        //    return (x >> s) | (x << (32 - s));
        //}

        //ulong RotateLeft(ulong x, int s)
        //{
        //    return (x << s) | (x >> (64 - s));
        //}

    }
}
