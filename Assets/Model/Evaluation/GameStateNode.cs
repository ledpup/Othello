using System;
using System.Collections.Generic;
using System.Linq;

namespace Reversi.Model.Evaluation
{
    public struct GameStateNode : INode
	{
        private GameState _gameState { get; set; }
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

        public GameStateNode(ref GameState gameState, Dictionary<string, float> weights, short playIndex = (short)0) : this()
		{
            _gameState = gameState;
            _weights = weights;
            PlayIndex = playIndex;
            //_children = null;
		}

        public void NextTurn()
        {
            _gameState = _gameState.NextTurn();
            //_children = null;
        }

        public float Value
        {
            get
            {
                // If the game is finished, we don't need an heuristic because we know who has won.
                if (IsGameOver)
                    return _gameState.PlayerWinning ? 1 : 0;

                // This is the heuristic evaluation function.
                return Standardise(Evaluation);
            }
        }

        float Evaluation
		{
            get 
            {
                return (Pieces + Mobility + PotentialMobility + Parity + Position);
            }
		}
		
        public bool IsGameOver
        {
            get { return _gameState.IsGameOver; }
        }

        public List<GameStateNodeReference> ChildNodeReferences
        {
            get
            {
                if (_childNodeReferences != null)
                    return _childNodeReferences;

                _childNodeReferences = new List<GameStateNodeReference>();
                foreach (var play in PlayerPlays)
                {
                    var gameState = _gameState;
                    gameState.PlacePiece(play);

                    var nextGameState = gameState.NextTurn();

                    var child = new GameStateNode(ref nextGameState, _weights, play);

                    var reference = DepthFirstSearch.GameStateNodeCollection.AddGameStateNode(ref child);

                    _childNodeReferences.Add(reference);
                    
                }

                return _childNodeReferences;
            }
        }
        private List<GameStateNodeReference> _childNodeReferences;
        //public IEnumerable<INode> Children 
        //{
        //    get
        //    {
        //        if (_children != null)
        //            return _children;

        //        _children = new List<INode>();
        //        foreach (var play in PlayerPlays)
        //        {
        //            var gameState = _gameState;
        //            gameState.PlacePiece(play);

        //            var nextGameState = gameState.NextTurn();

        //            var child = new GameStateNode(nextGameState, _weights, play);
        //            _children.Add(child);
        //        }
        //        return _children;
        //    } 
        //}

        IEnumerable<short> PlayerPlays
        {
            get { return _gameState.PlayerPlays.Indices(); }
        }

        public float Pieces
        {
            get 
            {
                var pieces = _gameState.NumberOfPlayerPieces - _gameState.NumberOfOpponentPieces + 64;
                return pieces < 0 ? 0 : Standardise(pieces) * _weights["Pieces"];
            }
        }

        public float Mobility
        {
            get
            {
                var mobility = _gameState.PlayerPlays.CountBits() - _gameState.OpponentPlays.CountBits() + 64;
                return mobility == 0 ? 0 : Standardise(mobility) * _weights["Mobility"];
			}
        }

        public float PotentialMobility
        {
            get
            {
                var playerFrontier = Play.PotentialMobility(_gameState.PlayerPieces, _gameState.EmptySquares).CountBits();
                var opponentFrontier = Play.PotentialMobility(_gameState.OpponentPieces, _gameState.EmptySquares).CountBits();
                var potentialMobility = opponentFrontier - playerFrontier + 64;
                return potentialMobility == 0 ? 0 : Standardise(potentialMobility) * _weights["PotentialMobility"];
            }
        }

        public float Parity
        {
            get 
            { 
                var parity = _gameState.AllPieces.CountBits() % 2 == 0 ? 0 : 1;
                return parity * _weights["Parity"];
            }
        }

        public float Position
        {
            get { return PositionValues[PlayIndex] * _weights["PositionValues"]; }
        }

        private static float Standardise(float value)
        {
            return (1f - 1f / value);
        }
    }
}
