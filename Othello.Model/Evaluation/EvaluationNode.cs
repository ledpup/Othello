using System.Collections.Generic;
using System.Linq;

namespace Othello.Model.Evaluation
{
    public struct EvaluationNode : INode
	{
        public GameState GameState { get; private set; }
        public short? PlayIndex { get; private set; }
        private readonly Dictionary<string, float> _weights;

        public EvaluationNode(ref GameState gameState, Dictionary<string, float> weights, short? playIndex = null) : this()
		{
            GameState = gameState;
            _weights = weights;
            PlayIndex = playIndex;
		}

        public void NextTurn()
        {
            GameState = GameState.NextTurn();
        }

        public float Value
        {
            get
            {
                // If the game is finished, we don't need an heuristic because we know who has won.
                if (IsGameOver)
                    return GameState.PlayerWinning ? 1 : 0;

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
            get { return GameState.IsGameOver; }
        }

        private List<EvaluationNodeReference> ChildNodeReferences
        {
            get
            {
                if (_childNodeReferences != null)
                    return _childNodeReferences;

                _childNodeReferences = new List<EvaluationNodeReference>();
                foreach (var play in PlayerPlays)
                {
                    var gameState = GameState;
                    gameState.PlacePiece(play);

                    var nextGameState = gameState.NextTurn();

                    var child = new EvaluationNode(ref nextGameState, _weights, play);

                    var reference = DepthFirstSearch.AnalysisNodeCollection.AddAnalysisNode(ref child);

                    _childNodeReferences.Add(reference);
                }

                return _childNodeReferences;
            }
        }
        private List<EvaluationNodeReference> _childNodeReferences;
        
        public IEnumerable<INode> Children
        {
            get
            {
                var children = new List<INode>();
                ChildNodeReferences.ForEach(x => children.Add(DepthFirstSearch.AnalysisNodeCollection.GetEvaluationNode(x)));
                return children;
            }
        }

        public bool HasChildren
        {
            get { return ChildNodeReferences.Count > 0; }
        }
        
        IEnumerable<short> PlayerPlays
        {
            get { return GameState.PlayerPlays.Indices(); }
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
            get { return GameState.NumberOfOpponentPieces; }
        }

        public short PlayerPieces
        {
            get { return GameState.NumberOfPlayerPieces; }
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
            get { return GameState.OpponentPlays.CountBits(); }
        }

        public short PlayerPlayCount
        {
            get { return GameState.PlayerPlays.CountBits(); }
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
            get { return Play.PotentialMobility(GameState.PlayerPieces, GameState.EmptySquares).CountBits(); }
        }

        public short OpponentFrontier
        {
            get { return Play.PotentialMobility(GameState.OpponentPieces, GameState.EmptySquares).CountBits(); }
        }


        public float Parity
        {
            get 
            { 
                var parity = GameState.AllPieces.CountBits() % 2 == 0 ? 0 : 1;
                return parity * _weights["Parity"];
            }
        }

        public short PlayerCorners
        {
            get { return (Patterns.Corner & GameState.PlayerPieces).CountBits(); }
        }

        public short OpponentCorners
        {
            get { return (Patterns.Corner & GameState.OpponentPieces).CountBits(); }
        }

        public short PlayerXSquares
        {
            get { return (Patterns.XSquare & GameState.PlayerPieces).CountBits(); }
        }

        public short OpponentXSquares
        {
            get { return (Patterns.XSquare & GameState.OpponentPieces).CountBits(); }
        }

        public short PlayerCSquares
        {
            get { return (Patterns.CSquare & GameState.PlayerPieces).CountBits(); }
        }

        public short OpponentCSquares
        {
            get { return (Patterns.CSquare & GameState.OpponentPieces).CountBits(); }
        }

        public short PlayerEdges
        {
            get { return (Patterns.Edge & GameState.PlayerPieces).CountBits(); }
        }

        public short OpponentEdges
        {
            get { return (Patterns.Edge & GameState.OpponentPieces).CountBits(); }
        }

        public float Pattern
        {
            get
            {
                var corner = CompareBitboards(Patterns.Corners, GameState.PlayerPieces, GameState.OpponentPieces, 1);
                var xSquare = CompareBitboards(Patterns.XSquares, GameState.PlayerPieces, GameState.OpponentPieces, -1);
                var cornerAndXSquare = CompareBitboards(Patterns.CornerAndXSquare, GameState.PlayerPieces, GameState.OpponentPieces, 1);
                var cSquare = CompareBitboards(Patterns.CSquares, GameState.PlayerPieces, GameState.OpponentPieces, -1);
                var cornerAndCSquare = CompareBitboards(Patterns.CornerAndCSquare, GameState.PlayerPieces, GameState.OpponentPieces, 1);
                
                var edges = CompareBitboards(Patterns.Edges, GameState.PlayerPieces, GameState.OpponentPieces, 1);

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
