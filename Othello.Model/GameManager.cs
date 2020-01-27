using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using Othello.Model.Evaluation;

namespace Othello.Model
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
		
		public bool HasPlays { get { return GameState.HasPlays; } }
		
		public bool CanPlay(short position)
		{
			return GameState.PlayerPlays.Indices().Any(m => m == position);
		}
				
		public void PlacePiece(short? tileIndex)
		{
			if (GameState.HasPlays && tileIndex == null)
				throw new Exception(string.Format("{0} can not pass.", Player));
					
			Plays.Add(tileIndex);

		    if (tileIndex == null)
				return;
			
			var play = (short)tileIndex;
			
			if (!CanPlay(play))
				throw new Exception(string.Format("Play to {0} (index {1}) is not a valid play.", tileIndex.ToAlgebraicNotation(), play));
			
			FlippedPieces = GameState.PlacePiece(play);
		    Placement = play;
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

            var data = File.Exists(fileName) ? File.ReadAllLines(fileName) : null;

            var games = new List<GameManager>();
			
			if (data == null || !data.Any())
				games.Add(new GameManager());
			else
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
		
        public string GameWinner
        {
            get
            {
                if (GameState.IsDraw)
                {
                    return "Draw";
                }
                return GameState.PlayerWinning ? Player : Opponent;
            }   
        }


		public string GameResult
		{
			get 
			{
				if (!GameState.IsGameOver)
					throw new Exception();

	            if (!GameState.IsDraw)
	            {
	                return string.Format("{0} to {1}", WinnersScore, LossersScore);
	            }
				return "";
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
            get { return PlayerIsBlack ? GameState.PlayerPieces : GameState.OpponentPieces; }
	    }

        public ulong WhitePieces
        {
            get { return PlayerIsBlack ? GameState.OpponentPieces : GameState.PlayerPieces; }
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

	    public bool PlayerIsBlack
	    {
	        get { return PlayerIndex == 0; }
	    }

	    public string Player
		{
            get { return PlayerIsBlack ? BlackName : WhiteName; }
		}

		public string Opponent
		{
            get { return PlayerIsBlack ? WhiteName : BlackName; }
		}
		
		public int PlayerIndex
		{
			get { return Turn % 2; }
		}

	    public short Turn
	    {
            get { return (short)Plays.Count; }
	    }

		public short TurnExcludingPasses
	    {
            get { return (short)Plays.Where(x => x != null).Count(); }
	    }
		
	    public List<short> PlayerPlays
	    {
	        get { return GameState.PlayerPlays.Indices().ToList(); }
	    }

        public void Draw()
        {
            for (var i = 0; i < 64; i++)
            {
                if (i % 8 == 0)
                    Console.WriteLine();

                var pos = 1UL << i;
                if ((BlackPieces & pos) > 0)
                    Console.Write("x");
                else if ((WhitePieces & pos) > 0)
                    Console.Write("o");
                else
                    Console.Write(" ");
            }
            Console.WriteLine();
        }

        public bool IsGameOver { get { return GameState.IsGameOver; } }

	    public List<short> PlayerPieces
	    {
	        get {return GameState.PlayerPieces.Indices().ToList(); }
	    }

        public List<short> OpponentPieces
        {
            get { return GameState.OpponentPieces.Indices().ToList(); }
        }

	    public short Placement;

	    public List<short> FlippedPieces;
        
        public bool IsDraw
        {
            get { return GameState.IsDraw; }
        }

        public string[] AnalysisInfo(short? infoPlayIndex, ComputerPlayer computerPlayer)
        {
            var gameState = GameState;

            var turn = Turn;

            if (infoPlayIndex != null)
            {
                gameState.PlacePiece((short)infoPlayIndex);
                gameState = gameState.NextTurn();
                turn++;
            }

            var analysisNode = new EvaluationNode(ref gameState, computerPlayer.GetWeights(turn));

            var results = new string[2];

            results[0] =               StringSpacing(analysisNode.PlayerPieces) 
                            + "\r\n" + StringSpacing(analysisNode.PlayerPlayCount)
                            + "\r\n" + StringSpacing(analysisNode.PlayerFrontier)
                            + "\r\n" + StringSpacing(analysisNode.PlayerCorners)
                            + "\r\n" + StringSpacing(analysisNode.PlayerXSquares)
                            + "\r\n" + StringSpacing(analysisNode.PlayerCSquares)
                            + "\r\n" + StringSpacing(analysisNode.PlayerEdges);

            results[1] =               StringSpacing(analysisNode.OpponentPieces)
                            + "\r\n" + StringSpacing(analysisNode.OpponentPlayCount)
                            + "\r\n" + StringSpacing(analysisNode.OpponentFrontier)
                            + "\r\n" + StringSpacing(analysisNode.OpponentCorners)
                            + "\r\n" + StringSpacing(analysisNode.OpponentXSquares)
                            + "\r\n" + StringSpacing(analysisNode.OpponentCSquares)
                            + "\r\n" + StringSpacing(analysisNode.OpponentEdges);

            return results;
        }

	    static bool IsBlacksTurn(short turn)
        {
            return turn % 2 == 0;
        }

        private static string BlackAndWhite(short p, short o, bool isBlacksTurn)
        {
            return isBlacksTurn ? FormatValues(p, o) : FormatValues(o, p);
        }

	    private static string FormatValues(short p, short o)
        {
            return StringSpacing(p) + "\t\t\t" + StringSpacing(o);
        }

        private static string StringSpacing(short value)
        {
            if (value.ToString().Length == 1)
                return "  " + value;
            return value.ToString();
        }

    }
}