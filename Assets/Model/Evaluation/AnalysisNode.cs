using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reversi.Model.Evaluation
{
    public struct AnalysisNode : INode
	{
        private GameState _gameState { get; set; }
        public short? PlayIndex { get; private set; }
        private readonly Dictionary<string, float> _weights;

        public AnalysisNode(ref GameState gameState, Dictionary<string, float> weights, short? playIndex = null) : this()
		{
            _gameState = gameState;
            _weights = weights;
            PlayIndex = playIndex;
            _children = null;
		}

        public void NextTurn()
        {
            _gameState = _gameState.NextTurn();
            _children = null;
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
                return (Pieces + Mobility + PotentialMobility + Pattern);
            }
		}
		
        public bool IsGameOver
        {
            get { return _gameState.IsGameOver; }
        }

        private List<AnalysisNodeReference> ChildNodeReferences
        {
            get
            {
                if (_childNodeReferences != null)
                    return _childNodeReferences;

                _childNodeReferences = new List<AnalysisNodeReference>();
                foreach (var play in PlayerPlays)
                {
                    var gameState = _gameState;
                    gameState.PlacePiece(play);

                    var nextGameState = gameState.NextTurn();

                    var child = new AnalysisNode(ref nextGameState, _weights, play);

                    var reference = DepthFirstSearch.AnalysisNodeCollection.AddAnalysisNode(ref child);

                    _childNodeReferences.Add(reference);
                }

                return _childNodeReferences;
            }
        }
        private List<AnalysisNodeReference> _childNodeReferences;
        
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

        //            var child = new AnalysisNode(ref nextGameState, _weights, play);
        //            _children.Add(child);
        //        }
        //        return _children;
        //    }
        //}
        private List<INode> _children;

        public IEnumerable<INode> Children
        {
            get
            {
                var children = new List<INode>();
                ChildNodeReferences.ForEach(x => children.Add(DepthFirstSearch.AnalysisNodeCollection.GetAnalysisNode(x)));
                return children;
            }
        }

        public bool HasChildren
        {
            get { return ChildNodeReferences.Any(); }
        }
        
        IEnumerable<short> PlayerPlays
        {
            get { return _gameState.PlayerPlays.Indices(); }
        }

        public float Pieces
        {
            get 
            {
                var pieces = PlayerPieces - OpponentPieces + 64;
                return pieces < 0 ? 0 : Standardise(pieces) * _weights["Pieces"];
            }
        }

        public short OpponentPieces
        {
            get { return _gameState.NumberOfOpponentPieces; }
        }

        public short PlayerPieces
        {
            get { return _gameState.NumberOfPlayerPieces; }
        }

        public float Mobility
        {
            get
            {
                var mobility = PlayerPlayCount - OpponentPlayCount + 64;
                return mobility == 0 ? 0 : Standardise(mobility) * _weights["Mobility"];
			}
        }

        public short OpponentPlayCount
        {
            get { return _gameState.OpponentPlays.CountBits(); }
        }

        public short PlayerPlayCount
        {
            get { return _gameState.PlayerPlays.CountBits(); }
        }

        public float PotentialMobility
        {
            get
            {
                var potentialMobility = OpponentFrontier - PlayerFrontier + 64;
                return potentialMobility == 0 ? 0 : Standardise(potentialMobility) * _weights["PotentialMobility"];
            }
        }

        public short PlayerFrontier
        {
            get { return Play.PotentialMobility(_gameState.PlayerPieces, _gameState.EmptySquares).CountBits(); }
        }

        public short OpponentFrontier
        {
            get { return Play.PotentialMobility(_gameState.OpponentPieces, _gameState.EmptySquares).CountBits(); }
        }


        public float Parity
        {
            get 
            { 
                var parity = _gameState.AllPieces.CountBits() % 2 == 0 ? 0 : 1;
                return parity * _weights["Parity"];
            }
        }

        public short PlayerCorners
        {
            get { return (Patterns.Corner & _gameState.PlayerPieces).CountBits(); }
        }

        public short OpponentCorners
        {
            get { return (Patterns.Corner & _gameState.OpponentPieces).CountBits(); }
        }

        public short PlayerXSquares
        {
            get { return (Patterns.XSquare & _gameState.PlayerPieces).CountBits(); }
        }

        public short OpponentXSquares
        {
            get { return (Patterns.XSquare & _gameState.OpponentPieces).CountBits(); }
        }

        public short PlayerCSquares
        {
            get { return (Patterns.CSquare & _gameState.PlayerPieces).CountBits(); }
        }

        public short OpponentCSquares
        {
            get { return (Patterns.CSquare & _gameState.OpponentPieces).CountBits(); }
        }

        public short PlayerEdges
        {
            get { return (Patterns.Edge & _gameState.PlayerPieces).CountBits(); }
        }

        public short OpponentEdges
        {
            get { return (Patterns.Edge & _gameState.OpponentPieces).CountBits(); }
        }

        public float Pattern
        {
            get
            {
                var corner = CompareBitboards(Patterns.Corners, _gameState.PlayerPieces, _gameState.OpponentPieces, 1);
                var xSquare = CompareBitboards(Patterns.XSquares, _gameState.PlayerPieces, _gameState.OpponentPieces, -1);
                var cornerAndXSquare = CompareBitboards(Patterns.CornerAndXSquare, _gameState.PlayerPieces, _gameState.OpponentPieces, 1);
                var cSquare = CompareBitboards(Patterns.CSquares, _gameState.PlayerPieces, _gameState.OpponentPieces, -1);
                var cornerAndCSquare = CompareBitboards(Patterns.CornerAndCSquare, _gameState.PlayerPieces, _gameState.OpponentPieces, 1);
                
                var edges = CompareBitboards(Patterns.Edges, _gameState.PlayerPieces, _gameState.OpponentPieces, 1);

                return Standardise(corner + xSquare + cornerAndXSquare + (cSquare + cornerAndCSquare * .75f) + (edges * .5f)) * _weights["Pattern"];
            }
        }

        public float CompareBitboards(List<ulong> bitBoards, ulong player, ulong opponent, int direction)
        {
            var playersCount = bitBoards.Count(x => (x & player) > 0);
            var opponentsCount = bitBoards.Count(x => (x & opponent) > 0);

            return ((playersCount * direction) - (opponentsCount * direction) + bitBoards.Count) / (float)(bitBoards.Count * 2);
        }

        private static float Standardise(float value)
        {
            return (1f - 1f / value);
        }


    }
}
