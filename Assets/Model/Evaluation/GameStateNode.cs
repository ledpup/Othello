using System;
using System.Collections.Generic;
using System.Linq;

namespace Reversi.Model.Evaluation
{
    public struct GameStateNode : INode
	{
        //public delegate float EvaluationMethod();
        //public static EvaluationMethod Evaluation { get; set; }
        GameState GameState { get; set; }
        public short PlayIndex { get; private set; }
        private readonly Dictionary<string, float> _weights;
        private static readonly float[] PositionValues = new [] {  1f,    0f,    0.2f,  0.1f,  0.1f,  0.2f,  0f,    1f,
                                                                   0f,    0f,    0.02f, 0.02f, 0.02f, 0.02f, 0f,    0f,
                                                                   0.2f,  0.02f, 0.1f,  0.04f, 0.04f, 0.1f,  0.02f, 0.2f,
                                                                   0.1f,  0.02f, 0.04f, 0.02f, 0.02f, 0.04f, 0.02f, 0.1f,
                                                                   0.1f,  0.02f, 0.04f, 0.02f, 0.02f, 0.04f, 0.02f, 0.1f,
                                                                   0.2f,  0.02f, 0.1f,  0.04f, 0.04f, 0.1f,  0.02f, 0.2f,
                                                                   0f,    0f,    0.01f, 0.01f, 0.01f, 0.01f, 0f,    0f,
                                                                   1f,    0f,    0.20f, 0.1f,  0.1f,  0.20f, 0f,    1f};

        public GameStateNode(GameState gameState, Dictionary<string, float> weights, short playIndex = (short)0) : this()
		{
            GameState = gameState;
            PlayIndex = playIndex;
            _children = null;
            //Evaluation = SimpleEval;

            _weights = weights;
		}

        public void NextTurn()
        {
            GameState = GameState.NextTurn();
            _children = null;
        }

        public float Value
        {
            get
            {
                // If the game is finished, we don't need an heuristic because we know who has won.
                if (IsGameOver)
                    return GameState.PlayerWinning ? 1 : 0;

                // This is the heuristic evaluation function.
                return 1 - (1 / Evaluation());
            }
        }

        float Evaluation()
		{
            return (Pieces + Mobility + PotentialMobility + Parity + Position);
		}
		
        public bool IsGameOver
        {
            get { return GameState.IsGameOver; }
        }

		private List<INode> _children;

		public IEnumerable<INode> Children 
		{
			get
			{
				if (_children != null)
					return _children;

				_children = new List<INode>();
				foreach (var play in PlayerPlays)
				{
				    var gameState = GameState;
                    gameState.PlacePiece(play);

				    var nextGameState = gameState.NextTurn();

                    var child = new GameStateNode(nextGameState, _weights, play);
                    _children.Add(child);
				}
				return _children;
			} 
		}

        IEnumerable<short> PlayerPlays
        {
            get { return GameState.PlayerPlays.Indices(); }
        }

        public float Pieces
        {
            get 
            {
                var pieces = GameState.NumberOfPlayerPieces - GameState.NumberOfOpponentPieces + 64;
                return pieces < 0 ? 0 : Standardise(pieces) * _weights["Pieces"];
            }
        }

        public float Mobility
        {
            get
            {
                var mobility = GameState.PlayerPlays.CountBits() - GameState.OpponentPlays.CountBits() + 64;
                return mobility == 0 ? 0 : Standardise(mobility) * _weights["Mobility"];
			}
        }

        public float PotentialMobility
        {
            get
            {
                var playerFrontier = Play.PotentialMobility(GameState.PlayerPieces, GameState.EmptySquares).CountBits();
                var opponentFrontier = Play.PotentialMobility(GameState.OpponentPieces, GameState.EmptySquares).CountBits();
                var potentialMobility = opponentFrontier - playerFrontier + 64;
                return potentialMobility == 0 ? 0 : Standardise(potentialMobility) * _weights["PotentialMobility"];
            }
        }

        public float Parity
        {
            get 
            { 
                var parity = GameState.AllPieces.CountBits() % 2 == 0 ? 0 : 1;
                return parity * _weights["Parity"];
            }
        }

        public float Position
        {
            get { return PositionValues[PlayIndex] * _weights["PositionValues"]; }
        }

        private static int Standardise(int value)
        {
            return (1 - 1 / value);
        }
    }
}
