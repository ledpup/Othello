using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;

namespace Reversi.Model
{
	public class GameManager
	{
		public GameState GameState;
		
		public List<short?> Plays;
		
		public GameManager() : this (new List<short?>())
        {
        }

        public GameManager(List<short?> plays)
		{
            if (plays == null)
                throw new ArgumentNullException("plays");

            GameState = GameState.NewGame();

            Plays = new List<short?>();

            plays.ForEach(x =>
            {
                PlacePiece(x);
                NextTurn();
            });

            BlackName = "Black";
            WhiteName = "White";
		}
		
		public bool HasPlays { get { return GameState.PlayerPlays.Indices().Any(); } }
		
		public bool CanPlay(short position)
		{
			return GameState.PlayerPlays.Indices().Any(m => m == position);
		}
				
		public void PlacePiece(short? index)
		{
			if (GameState.CanPlay && index == null)
				throw new Exception(string.Format("{0} can not pass.", Player));
					
			Plays.Add(index);

		    if (index == null)
				return;
			
			var play = (short)index;
			
			if (!CanPlay(play))
				throw new Exception(string.Format("Play to {0} (index {1}) is not a valid play.", index.ToAlgebraicNotation(), play));
			
			GameState.PlacePiece(play);
		}
		
		public void NextTurn()
		{
            GameState = GameState.NextTurn();
		}

		public static string SerialsePlays(List<short?> plays)
		{
            if (plays.Count == 0)
				return "";

            var playsAsAlgebraic = plays.Select(m => m.ToAlgebraicNotation()).ToArray();
			var serialisedPlays = string.Join(",", playsAsAlgebraic);

		    return serialisedPlays;
		}

		public static List<short?> DeserialsePlays(string data)
		{
		    data = data.ToLower();

			if (string.IsNullOrEmpty(data))
				return new List<short?>();

			return data.Split(',').Select(m => m.ToIndex()).ToList();
		}
		
		public void Save(string fileName, List<short?> plays = null)
		{
			if (string.IsNullOrEmpty(fileName))
				throw new ArgumentNullException("fileName");

            var playList = plays ?? Plays;
			
			var fileInfo = new FileInfo(fileName);
			if (!fileInfo.Directory.Exists)
                fileInfo.Directory.Create();
			
            File.WriteAllText(fileName, SerialsePlays(playList));
		}

        public static List<GameManager> LoadGamesFromFile(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                throw new ArgumentNullException("fileName");

            if (!File.Exists(fileName))
                throw new FileNotFoundException();

            var data = File.ReadAllLines(fileName);

            var games = new List<GameManager>();

            data.ToList().ForEach(x => games.Add(Load(x)));

            return games;
        }

		public static GameManager LoadFromFile(string fileName)
		{
			if (string.IsNullOrEmpty(fileName))
				throw new ArgumentNullException("fileName");
			
			if (!File.Exists(fileName))
				throw new FileNotFoundException();
			
			var data = File.ReadAllText(fileName);
			return Load(data);
		}

	    public static GameManager Load(string data)
        {
            var plays = DeserialsePlays(data);
            return new GameManager(plays);
        }
		
		public string GameOverMessage
		{
			get 
			{
				if (!GameState.IsGameOver)
					throw new Exception();
					
				string message;
	            if (GameState.IsDraw)
	            {
	                message = "Gameover.\r\nIt is a draw.";
	            }
	            else
	            {
	                var winner = GameState.PlayerWinning ? Player : Opponent;

	                message = string.Format("Gameover.\r\n{0} wins {1} to {2}.", winner, WinnersScore, LossersScore);
	            }
				return message;
			}
		}

        public char Winner
        {
            get
            {
                if (BlackScore == WhiteScore)
                    return '0';

                return BlackScore > WhiteScore ? 'B' : 'W';
            }
        }

        public short WinnerIndex
        {
            get
            {
                if (BlackScore == WhiteScore)
                    return -1;

                return BlackScore > WhiteScore ? (short)0 : (short)1;
            }
        }

	    public int WinnersScore
	    {
            get { return BlackScore > WhiteScore ? BlackScore : WhiteScore; }
	    }

        public int LossersScore
        {
            get { return BlackScore > WhiteScore ? WhiteScore : BlackScore; }
        }

	    public ulong BlackPieces
	    {
            get { return PlayerIndex == 0 ? GameState.PlayerPieces : GameState.OpponentPieces; }
	    }

        public ulong WhitePieces
        {
            get { return PlayerIndex == 1 ? GameState.PlayerPieces : GameState.OpponentPieces; }
        }

	    public int BlackScore
	    {
            get
            {
                var score = BlackPieces.CountBits();
                var emptySquares = GameState.IsDraw ? GameState.EmptySquares.CountBits() / (short)2 : score > WhitePieces.CountBits() ? GameState.EmptySquares.CountBits() : (short)0;
                return score + emptySquares;
            }
	    }

        public int WhiteScore
        {
            get
            {
                var score = WhitePieces.CountBits();
                var emptySquares = GameState.IsDraw ? GameState.EmptySquares.CountBits() / (short)2 : score > BlackPieces.CountBits() ? GameState.EmptySquares.CountBits() : (short)0;
                return score + emptySquares;
            }
        }

	    public string BlackName;
	    public string WhiteName;

	    public string Player
		{
            get { return PlayerIndex == 0 ? BlackName : WhiteName; }
		}

		public string Opponent
		{
            get { return PlayerIndex == 1 ? BlackName : WhiteName; }
		}
		
		public int PlayerIndex
		{
			get { return Turn % 2; }
		}

	    public int Turn
	    {
            get { return Plays.Count; }
	    }
	}
}