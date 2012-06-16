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
				
		public void PlacePiece(short? index)
		{
			if (GameState.HasPlays && index == null)
				throw new Exception(string.Format("{0} can not pass.", Player));
					
			Plays.Add(index);

		    if (index == null)
				return;
			
			var play = (short)index;
			
			if (!CanPlay(play))
				throw new Exception(string.Format("Play to {0} (index {1}) is not a valid play.", index.ToAlgebraicNotation(), play));
			
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

            if (!File.Exists(fileName))
                throw new FileNotFoundException();

            var data = File.ReadAllLines(fileName);

            var games = new List<GameManager>();
			
			if (!data.Any())
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
        //{
        //    get { return GameState.Placement.Indices().Single(); }
        //}

	    public List<short> FlippedPieces;
        //{
        //    get { return GameState.FlippedPieces.Indices().ToList(); }
        //}



        public bool IsDraw
        {
            get { return GameState.IsDraw; }
        }

        public string AnalysisInfo(short? infoPlayIndex, ComputerPlayer computerPlayer)
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

            var playerName = IsBlacksTurn(turn) ? "Black" : "White";

            var stringBulider = new StringBuilder();
            //stringBulider.AppendLine();

            //stringBulider.AppendLine(playerName + " Score\t\t\t" + analysisNode.Value * 100);
            //stringBulider.AppendLine();
            stringBulider.AppendLine("\t\t\t\t  Black\t  White");
            stringBulider.AppendLine("Pieces\t\t\t" + BlackAndWhite(analysisNode.PlayerPieces, analysisNode.OpponentPieces, IsBlacksTurn(turn)));
            stringBulider.AppendLine("Mobility\t\t\t" + BlackAndWhite(analysisNode.PlayerPlayCount, analysisNode.OpponentPlayCount, IsBlacksTurn(turn)));
            stringBulider.AppendLine("Frontier\t\t\t" + BlackAndWhite(analysisNode.PlayerFrontier, analysisNode.OpponentFrontier, IsBlacksTurn(turn)));
            stringBulider.AppendLine("Corner\t\t\t" + BlackAndWhite(analysisNode.PlayerCorners, analysisNode.OpponentCorners, IsBlacksTurn(turn)));
            stringBulider.AppendLine("X Square\t\t" + BlackAndWhite(analysisNode.PlayerXSquares, analysisNode.OpponentXSquares, IsBlacksTurn(turn)));
            stringBulider.AppendLine("C Square\t\t" + BlackAndWhite(analysisNode.PlayerCSquares, analysisNode.OpponentCSquares, IsBlacksTurn(turn)));
            stringBulider.AppendLine("Edge\t\t\t\t" + BlackAndWhite(analysisNode.PlayerEdges, analysisNode.OpponentEdges, IsBlacksTurn(turn)));
            return stringBulider.ToString();
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