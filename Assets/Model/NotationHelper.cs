using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reversi.Model
{
	public static class NotationHelper
	{
        public static short? ToIndex(this string algebraicNotation)
        {
			if (string.IsNullOrEmpty(algebraicNotation))
				return null;
			
            var charArray = algebraicNotation.ToCharArray();
            var column = int.Parse(((char)(charArray[0] - 48)).ToString());
            var row = int.Parse(charArray[1].ToString());

            if (column < 1 || column > 8 || row < 1 || row > 8)
                throw new Exception();

            var x = row - 1;
            var y = column - 1;

            return (short)(x * 8 + y);
        }

        public static string ToAlgebraicNotation(this short? playIndex)
        {
			if (playIndex == null)
				return "";
				
            if (playIndex < 0 || playIndex > 63)
                throw new Exception();

            var column = playIndex / 8 + 1;
            var row = playIndex % 8 + 1;

            return ((char)(row + 96)).ToString() + column;
        }
		
		public static Point ToCartesianCoordinate(this short index)
		{
			var x = index % 8;
            var y = index / 8;
			return new Point(x, y);
		}

        public static string ToAlgebraicNotation(this byte play)
        {
            var chars = play.ToString().ToCharArray();
            return ((char)(chars[0] + 48)).ToString() + chars[1];
        }
		
		public static char ToChar(this short index)
		{
			return ((char)(index + 48));
		}

        public static short ToInt(this char character)
        {
            return (short)(Convert.ToInt16(character) - 48);
        }

        public static double RoundToSignificantDigits(this double d, int digits)
        {
            if (d == 0)
				return 0;
			if (d < 0)
                throw new Exception("Number cannot be less than zero.");

            var scale = Math.Pow(10, Math.Floor(Math.Log10(d)) + 1);
            return scale * Math.Round(d / scale, digits);
        }

        //public static void Print<T>(this ICollection<T> col)

        public static string ToChars(this ICollection<short?> plays)
        {
            if (plays.Count == 0)
                return "";

            var playsAsChar = plays.Where(p => p != null).Select(p => ((short)p).ToChar()).Select(x => x.ToString()).ToArray();
            var serialisedPlays = string.Join("", playsAsChar);

            return serialisedPlays;
        }
	}
}
