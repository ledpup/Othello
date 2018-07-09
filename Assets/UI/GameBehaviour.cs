using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Othello.Model.Evaluation;
using UnityEngine;
using Othello.Model;
using System;
using System.Threading;

public delegate void OnBoardChangeHandler(object sender);

public class GameBehaviour : MonoBehaviour
{
    public GameObject BoardTile;
    public GameObject Piece;
    public GameObject Text;
    public GUISkin GuiSkin;
    
    List<GameObject> _gamePieces;

    const int _width = 8;
    const int _height = 8;
    const float Spacing = 1.1f;

    GameObject[,] _gameBoard;
    GameManager _gameManager;
    
    float _cameraZ;
    public const float PieceStartingHeight = 10;

    public List<short?> Plays;

    List<GameObject> _boardCoordinates;
    List<GameObject> _tileInfo;
    public Point BoardLocation;

    System.Random _random;
    
    Stopwatch _stopwatch;
    float _animationSpeed = .5f;

    public List<string> GameArchive;
    private Dictionary<string, GameStateStats> _positionStats;
    
    public bool UseTranspositionTable
	{ 
		get { return PlayerUiSettings.UseTranspositionTable; } 
        set { PlayerUiSettings.UseTranspositionTable = value; } 
    }
	
    public bool UseOpeningBook 
	{ 
		get { return PlayerUiSettings.UseOpeningBook; } 
        set { PlayerUiSettings.UseOpeningBook = value; } 
    }
	
    public int SearchMethod
	{ 
		get { return PlayerUiSettings.SearchMethod; } 
        set { PlayerUiSettings.SearchMethod = value; } 
    }
	
    public int SearchDepth 
	{ 
		get { return _computerPlayer.BaseSearchDepth; } 
        set { _computerPlayer.BaseSearchDepth = value; } 
    }
	
    public bool IsReplaying;

    private ComputerPlayer _computerPlayer;
    short? _computerPlayIndex;
    bool _computerStarted;

    public bool ShowArchiveStats
	{
        get { return PlayerUiSettings.ShowArchiveStats; }
		set 
		{
            PlayerUiSettings.ShowArchiveStats = value; 
			GeneratePositionStats();
		}
	}
		
    public bool ShowValidPlays
    {
        get { return PlayerUiSettings.ShowValidPlays; }
        set
        {
            if (PlayerUiSettings.ShowValidPlays == value)
                return;
            PlayerUiSettings.ShowValidPlays = value; 
            CreatePieces();
        }
    }
    
    public bool ShowBoardCoordinates
    {
        get { return PlayerUiSettings.ShowBoardCoordinates; }
        set 
        {
            if (PlayerUiSettings.ShowBoardCoordinates == value)
                return;
            PlayerUiSettings.ShowBoardCoordinates = value;
            DrawBoardCoordinates();
        }
    }

    private List<EvaluationNode> _savedGameStateNodes;

    public static GameBehaviour CreateGameBehaviour(GameObject gameObject, GUISkin guiSkin, GameObject boardTile, GameObject piece, GameObject text, Point boardLocation, GameManager gameManager, List<string> gameArchive, PlayerUiSettings playerUiSettings)
    {
        var gameBehaviour = gameObject.AddComponent<GameBehaviour>();
        
        gameBehaviour.GuiSkin = guiSkin;
        gameBehaviour.BoardTile = boardTile;
        gameBehaviour.Piece = piece;
        gameBehaviour.Text = text;
        gameBehaviour.BoardLocation = boardLocation;
        gameBehaviour.GameArchive = gameArchive;
        gameBehaviour.PlayerUiSettings = playerUiSettings;
        
        gameBehaviour.StartGameBehavour(gameManager);
        
        return gameBehaviour;
    }
    
    public bool IsComputerTurn
    {
        get { return ((_gameManager.PlayerIndex == 0 && !PlayerUiSettings.BlackIsHuman) || (_gameManager.PlayerIndex == 1 && !PlayerUiSettings.WhiteIsHuman)); }
    }
    
    public void StartGameBehavour(GameManager gameManager)
    {
		Messenger<short>.AddListener("Tile clicked", OnTileSelected);
        Messenger<short>.AddListener("Tile hover", OnTileHover);
        Messenger<float>.AddListener("Game speed changed", OnGameSpeedChanged);
		
		_computerPlayer = new ComputerPlayer(PlayerUiSettings);
		
        _positionStats = new Dictionary<string, GameStateStats>();
        
        if (gameManager != null)
        {
            _gameManager = gameManager;
        }
        else
        {
            try
            {
                _gameManager = GameManager.LoadFromFile(GamesController.SavePath + "CurrentGame.txt");
            }
            catch (System.IO.FileNotFoundException)
            {
                _gameManager = new GameManager();
            }
        }

        Plays = _gameManager.Plays;
        
        CreateBoard();
		CreatePieces();
        _boardCoordinates = new List<GameObject>();
		DrawBoardCoordinates();
        _tileInfo = new List<GameObject>();
		DrawStats();
        
        _random = new System.Random();
        
        var cameraX = _width / 2.0f * Spacing - (Spacing / 2);
        var cameraY = - _height / 2.0f * Spacing + (Spacing / 2);
        _cameraZ = -(_width + _height) / 2 - 1;
        
        transform.position = new Vector3(cameraX, cameraY, _cameraZ);
        
        _depthFirstSearch = new DepthFirstSearch();
        
        StopWatch = new Stopwatch();
    }
    
	bool _firstUpdate = true;
	
    void Update()
    {
		// This is a horrible hack to get the last played piece to highlight!!!!
		// I should find a better way... one day.
		if (_firstUpdate)
		{
			_firstUpdate = false;
			if (Plays.Count > 0)
				Messenger<short>.Broadcast("Last play", (short)Plays.Last());
		}
		
        if (IsReplaying)
        {
            if (_stopwatch.ElapsedMilliseconds > 350 * (1 / _animationSpeed))
            {
                if (_gameManager.Turn < Plays.Count())
                {
                    PlacePiece(Plays[_gameManager.Turn]);
                    _stopwatch = System.Diagnostics.Stopwatch.StartNew();
                }
            }
        }
        else
        {
            DrawStats();
            ComputerPlay();
        }
    }
    
    void OnApplicationQuit()
    {
        _gameManager.Save(GamesController.SavePath + "CurrentGame.txt", Plays);
    }
    
    void OnTileSelected(short index)
    {
        if (IsComputerTurn)
            return;

        if (!_gameManager.CanPlay(index))
            return;
        
		Play(index);
    }
	
	void Play(short? index)
	{
		PlacePiece(index);
        Plays = _gameManager.Plays;
	}
    
    void PlacePiece(short? index)
    {
        _gameManager.PlacePiece(index);
        if (index != null)
		{
            PlaceAndFlipPieces();
			Messenger<short>.Broadcast("Last play", (short)index);
		}
        _gameManager.NextTurn();
        DisplayPlays();
    }

    private short? _infoPlayIndex;

    void OnTileHover(short index)
    {
        _infoPlayIndex = null;
        if (_gameManager.CanPlay(index))
        {
            _infoPlayIndex = index;
        }
    }

    bool _canDrawStats;
    
    void DrawStats()
    {
        if (!_canDrawStats)
            return;
        
		DeleteTileInfo();

        if (!PlayerUiSettings.ShowArchiveStats || !ShowValidPlays || IsReplaying)
            return;
		
        _canDrawStats = false;
        
        var position = _gameManager.Plays.ToChars();
        
        if (!_positionStats.ContainsKey(position))
            throw new Exception("This should never happen");
                    
        var playerPlays = _gameManager.PlayerPlays;
        
        playerPlays.ForEach(p => 
                        {
                            var coord = p.ToCartesianCoordinate();
                            
                            if (!_positionStats[position].PlayStats.ContainsKey(p)) 
                                return;

                            DrawTileInfo(coord.X, coord.Y, -.45f, -.45f, _positionStats[position].PlayStats[p].PercentageOfGames + "%");
							if (_gameManager.PlayerIsBlack)
                            	DrawTileInfo(coord.X, coord.Y, -.45f, .2f, _positionStats[position].PlayStats[p].PercentageOfWinsForBlack + "%");
							else
								DrawTileInfo(coord.X, coord.Y, -.45f, .2f, _positionStats[position].PlayStats[p].PercentageOfWinsForWhite + "%");
                        });
        
    }
    
    bool _statsAvailable;
    
    void GeneratePositionStats()
    {
        if (IsReplaying)
		{
            return;
		}

        var position = _gameManager.Plays.ToChars();
        
        if (!_positionStats.ContainsKey(position))
        {
            _statsAvailable = false;
            var gameStateStats = new GameStateStats();
            _positionStats.Add(position, gameStateStats);
            var thread = new Thread(() => gameStateStats.GenerateStats(_gameManager, GameArchive, ref _canDrawStats, ref _statsAvailable));
            thread.Start();
        }
        else
        {
            _canDrawStats = true;
        }
    }
    
	void DeleteTileInfo()
	{
		_tileInfo.ForEach(Destroy);
	}
	
    void DisplayPlays()
    {
		GeneratePositionStats();
		
        if (!ShowValidPlays || IsReplaying)
            return;

        var playerPlays = _gameManager.PlayerPlays;

        playerPlays.ForEach(p => CreatePiece(p, _gameManager.PlayerIndex, .3f, 0));
    }
    
    void DrawTileInfo(int x, int y, float xOffset, float yOffset, string text)
    {
        var textObject = (GameObject)Instantiate(Text);
        ((TextMesh)textObject.transform.GetComponent("TextMesh")).text = text;
        textObject.transform.localScale = new Vector3(.5f, .5f, .5f);
        textObject.GetComponent<Renderer>().material.color = Color.black;
        textObject.transform.position = GetWorldCoordinates(x + xOffset, y + yOffset, 0, BoardLocation);
        
        _tileInfo.Add(textObject);
    }

    private DepthFirstSearch _depthFirstSearch;
    public Stopwatch StopWatch;

    void ComputerPlay()
    {
        if (!IsComputerTurn)
        {
            return;
        }

        if (!_gameManager.HasPlays) 
            return;

        if (_computerPlayIndex == null && !_computerStarted && _statsAvailable)
        {
            _computerStarted = true;
            StopWatch.Reset();
            StopWatch.Start();

			DepthFirstSearch.AnalysisNodeCollection.ClearMemory();
            var thread = new Thread(() => _depthFirstSearch.GetPlayWithBook(_gameManager, _positionStats[_gameManager.Plays.ToChars()], _computerPlayer, ref _computerPlayIndex));
            thread.Start();
        }
        else if (_computerPlayIndex != null)
        {
            Play((short)_computerPlayIndex);
            _computerStarted = false;
            StopWatch.Stop();
            _computerPlayIndex = null;
        }        
    }

    private short RandomPlay()
    {
        var playerPlays = _gameManager.PlayerPlays;
        
        if (!_gameManager.HasPlays)
            throw new Exception();

        var index = _random.Next(0, playerPlays.Count);
        return playerPlays[index];
    }

    public void SkipTurn()
    {
        _gameManager.PlacePiece(null);
        _gameManager.NextTurn();
        DisplayPlays();
    }
    

    public void RestartGame()
    {
        Plays = new List<short?>();
        _gameManager = new GameManager();
        CreatePieces();
		Messenger<short>.Broadcast("Last play", -1);
    }
    
    void PlaceAndFlipPieces()
    {        
        var toDestroy = _gamePieces.Where(p => p.transform.localScale.x < .5f).ToList();
        toDestroy.ForEach(p => 
                          {
                            Destroy(p);
                            _gamePieces.Remove(p);
                          });
        
        var playerColour = 1 - _gameManager.PlayerIndex;

        CreatePiece(_gameManager.Placement, playerColour, .95f, -PieceStartingHeight);
        
        FlipPieces(_gameManager.FlippedPieces, _gameManager.Placement, playerColour);
    }
    
    void FlipPieces(List<short> flippedPieces, short placement, int playerColour)
    {
        flippedPieces.ForEach(fp => 
            {
                var gamePiece = _gamePieces.Single(p => ((PieceBehaviour)p.transform.GetComponent("PieceBehaviour")).Index == fp);
                ((PieceBehaviour)gamePiece.transform.GetComponent("PieceBehaviour")).Flip(playerColour, placement);
            });
    }
    
    public void CreatePieces()
    {
        if (_gamePieces == null)
            _gamePieces = new List<GameObject>();

        _gamePieces.ForEach(Destroy);        
        _gamePieces = new List<GameObject>();

        var opponentColour = _gameManager.PlayerIndex - 1;

        _gameManager.PlayerPieces.ForEach(p => CreatePiece(p, _gameManager.PlayerIndex, .95f, 0));
        _gameManager.OpponentPieces.ForEach(p => CreatePiece(p, opponentColour, .95f, 0));
        DisplayPlays();
    }

    GameObject CreatePiece(short pieceIndex, int colour, float scale, float z)
    {
        var gamePiece = (GameObject)Instantiate(Piece);
        var point = pieceIndex.ToCartesianCoordinate();
        gamePiece.transform.position = GetWorldCoordinates(point.X, point.Y, z, BoardLocation);
        gamePiece.transform.localScale = new Vector3(scale, scale, scale);
        
        var rotation = colour == 0 ? 0 : 180;
        var pieceTransform = gamePiece.transform;
        
        pieceTransform.Rotate(0, rotation, 0);
        var pb = ((PieceBehaviour)pieceTransform.GetComponent("PieceBehaviour"));
        pb.Index = pieceIndex;
        pb.OnGameSpeedChanged(_animationSpeed);
        pb.Drop(colour);
        
        _gamePieces.Add(gamePiece);
        
        return gamePiece;
    }

    void CreateBoard()
    {
        _gameBoard = new GameObject[_width, _height];
        
        for (var x = 0; x < _width; x++)
        {			
            for (var y = 0; y < _height; y++)
            {
                if (_gameBoard[x, y] != null)
                    Destroy(_gameBoard[x, y]);
                
                var tile = (GameObject)Instantiate(BoardTile);
                _gameBoard[x, y] = tile;

                var tileTransform = tile.transform;
                tileTransform.position = GetWorldCoordinates(x, y, 0, BoardLocation);

                var tileBehaviour = (TileBehaviour)tileTransform.GetComponent("TileBehaviour");
                tileBehaviour.Tile = new Tile(x, y);
            }
        }
    }
    
    void DrawBoardCoordinates()
    {
        _boardCoordinates.ForEach(Destroy);
        
        if (!ShowBoardCoordinates || IsReplaying)
            return;
        
        for (var i = 0; i < _width; i++)
        {
            DrawCoordinate(i, 0, 0, 8, true);
            DrawCoordinate(0, i, -0.7f, .2f, false);
        }			
    }
    
    void DrawCoordinate(int x, int y, float xOffset, float yOffset, bool chararacter)
    {
        var text = (GameObject)Instantiate(Text);
        ((TextMesh)text.transform.GetComponent("TextMesh")).text = chararacter ? ((char)(x + 97)).ToString() : (y + 1).ToString();
        text.transform.position = GetWorldCoordinates(-.2f + x + xOffset, -.5f + y + yOffset, 0, BoardLocation);
        
        _boardCoordinates.Add(text);
    }
    
    static Vector3 GetWorldCoordinates(float x, float y, float z, Point boardLocation)
    {
        return new Vector3(x * Spacing + (_width * Spacing * boardLocation.X * 1.075f), -y * Spacing + (_height * Spacing * boardLocation.Y * 1.075f), z);
    }
    
	public void PlayToStart()
	{
		_gameManager = new GameManager();
        CreatePieces();
		Messenger<short>.Broadcast("Last play", -1);
	}
	
    public void PlayTo(short index)
    {
        var plays = Plays.GetRange(0, index).Select(x => (short?)x).ToList();
        _gameManager = new GameManager(plays);
        CreatePieces();
        PlacePiece(Plays[index]);
    }

    public void OnGameSpeedChanged(float gameSpeed)
    {
        _animationSpeed = gameSpeed;
    }
    
    public string Player
    {
        get { return _gameManager.Player; }
    }
    
    public bool CanPlay
    {
        get { return _gameManager.HasPlays; }
    }
    
    public bool IsGameOver
    {
        get { return _gameManager.IsGameOver; }
    }
    
    public string GameOverMessage
    {
        get { return _gameManager.GameOverMessage; }
    }
    
    public string CannotPlayMessage
    {
        get { return string.Format("{0} can not play.\n{1} to play instead.", _gameManager.Player, _gameManager.Opponent); }
    }

    internal void Replay()
    {
        IsReplaying = !IsReplaying;
    
		DeleteTileInfo();
        DrawBoardCoordinates();
        CreatePieces();

        if (!IsReplaying)
            return;

        if (Plays.Where(x => x != null).Last() == _gameManager.Plays.Where(x => x != null).Last())
        {
            PlayToStart();
        }
        _stopwatch = System.Diagnostics.Stopwatch.StartNew();
    }

    public int NodesSearched
    {
        get 
        { 
            if (_computerPlayIndex == null)
                _nodesSearched = DepthFirstSearch.AnalysisNodeCollection.NumberOfNodes;
            return _nodesSearched;
        }
    }
    private int _nodesSearched;
    public PlayerUiSettings PlayerUiSettings;
    
    internal string AnalysisInfo()
    {
        return _gameManager.AnalysisInfo(_infoPlayIndex, _computerPlayer);
    }

    public static int Transpositions { get; set; }

    internal string ArchiveInfo()
    {
        if (_infoPlayIndex ==  null)
            return null;
        if (!_statsAvailable)
            return null;
		
        var position = _gameManager.Plays.ToChars();
		
		if (!_positionStats[position].PlayStats.ContainsKey(((short)_infoPlayIndex)))
			return null;
        
        var stats = _positionStats[position].PlayStats[((short)_infoPlayIndex)];
		var stringBuilder = new StringBuilder();
        stringBuilder.AppendLine(string.Format("{0} of {1} games ({2}%) play {3}.", stats.SubsetCount, GameArchive.Count, stats.PercentageOfGames, _infoPlayIndex.ToAlgebraicNotation()));
        stringBuilder.AppendLine(string.Format("Black won {0} ({1}%)", stats.BlackWins, stats.PercentageOfWinsForBlack));
        stringBuilder.AppendLine(string.Format("White won {0} ({1}%)", stats.WhiteWins, stats.PercentageOfWinsForWhite));
        stringBuilder.AppendLine(string.Format("Draws = {0} ({1}%)", stats.Draws, stats.PercentageOfDraws));

        return stringBuilder.ToString();
    }
}