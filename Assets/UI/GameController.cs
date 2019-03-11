using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using Othello.Model;
using UnityEngine.UI;
using System.Diagnostics;
using System.Text;
using Othello.Model.Evaluation;
using System.Threading;

public class GameController : MonoBehaviour
{
    public GameObject NewGameButton;
    public GameObject ButtonPrefab;
    public GameObject GameOptionsPanel;
    public GameObject ViewOptionsPanel;
    public GameObject GameoverPanel;
    public GameObject SkipTurnPanel;
    public GameObject ArchiveInfoPanel;
    public GameObject GamePlayHistoryPanel;
    public Toggle WhiteHuman;
    public Toggle BlackHuman;
    public Text SearchInfo;
    public Text PlayerTurn;
    public Text BlackAnalysis;
    public Text WhiteAnalysis;
    public Dropdown SearchDepthDropDown;
    public Button StartButton;
    public Button ReplayButton;

    private PlayerUiSettings _playerUiSettings;
    public Toggle ShowValidPlaysToggle, ShowBoardCoordinatesToggle, ShowArchiveStatsToggle;

    Dictionary<short, GameObject> _playHistory;

    public short _currentTurnIndex;

    public GameObject BoardTile;
    public GameObject Piece;
    public GameObject Text;

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
    float _animationSpeed = .35f;

    public List<string> GameArchive;
    private Dictionary<string, GameStateStats> _positionStats;

    public bool UseTranspositionTable
    {
        get { return _playerUiSettings.UseTranspositionTable; }
        set { _playerUiSettings.UseTranspositionTable = value; }
    }

    public bool UseOpeningBook
    {
        get { return _playerUiSettings.UseOpeningBook; }
        set { _playerUiSettings.UseOpeningBook = value; }
    }

    public int SearchMethod
    {
        get { return _playerUiSettings.SearchMethod; }
        set { _playerUiSettings.SearchMethod = value; }
    }

    public int SearchDepth
    {
        get { return _computerPlayer.BaseSearchDepth; }
        set { _computerPlayer.BaseSearchDepth = value; }
    }

    public bool IsReplaying;
    public bool IsViewingHistory;

    private ComputerPlayer _computerPlayer;
    short? _computerPlayIndex;
    bool _computerStarted;

    void Start()
	{
        GameoverPanel.SetActive(false);
        SkipTurnPanel.SetActive(false);

        _playerUiSettings = PlayerUiSettings.Load();





        BoardLocation = ((short)0).ToCartesianCoordinate();

        var archiveData = Application.streamingAssetsPath + "/ArchiveData.txt";

        GameArchive = File.Exists(archiveData)
                                ? File.ReadAllLines(archiveData).ToList()
                                : new List<string>();

        try
        {
            _gameManager = GameManager.LoadFromFile(Application.persistentDataPath + "/CurrentGame.txt");
        }
        catch (System.IO.FileNotFoundException)
        {
            _gameManager = new GameManager();
        }

        Plays = _gameManager.Plays;

        Messenger<short>.AddListener("Tile clicked", OnTileSelected);
        Messenger<short>.AddListener("Tile hover", OnTileHover);
        Messenger<float>.AddListener("Game speed changed", OnGameSpeedChanged);

        _computerPlayer = new ComputerPlayer(_playerUiSettings);

        _positionStats = new Dictionary<string, GameStateStats>();

        CreateBoard();
        CreatePieces();
        _boardCoordinates = new List<GameObject>();
        DrawBoardCoordinates();
        _tileInfo = new List<GameObject>();
        DrawStats();

        _random = new System.Random();

        var cameraX = _width / 2.0f * Spacing - (Spacing / 2);
        var cameraY = -_height / 2.0f * Spacing + (Spacing / 2);
        _cameraZ = -(_width + _height) / 2 - 1;

        transform.position = new Vector3(cameraX, cameraY, _cameraZ);

        _depthFirstSearch = new DepthFirstSearch();

        StopWatch = new Stopwatch();







        NewGameButton.GetComponent<Button>().onClick.AddListener(NewGame);

        WhiteHuman.onValueChanged.AddListener(delegate { WhiteIsHuman(WhiteHuman); });
        WhiteHuman.isOn = _playerUiSettings.WhiteIsHuman;

        BlackHuman.onValueChanged.AddListener(delegate { BlackIsHuman(BlackHuman); });
        BlackHuman.isOn = _playerUiSettings.BlackIsHuman;

        ShowValidPlaysToggle.onValueChanged.AddListener(delegate { SetShowValidPlays(ShowValidPlaysToggle); });
        ShowValidPlaysToggle.isOn = _playerUiSettings.ShowValidPlays;

        ShowBoardCoordinatesToggle.onValueChanged.AddListener(delegate { SetShowBoardCoordinates(ShowBoardCoordinatesToggle); });
        ShowBoardCoordinatesToggle.isOn = _playerUiSettings.ShowBoardCoordinates;

        ShowArchiveStatsToggle.onValueChanged.AddListener(delegate { SetShowArchiveStats(ShowArchiveStatsToggle); });
        ShowArchiveStatsToggle.isOn = _playerUiSettings.ShowArchiveStats;

        SkipTurnPanel.GetComponentInChildren<Button>().onClick.AddListener(delegate { SkipTurn(); });

        var searchDepth = SearchDepthDropDown.GetComponent<Dropdown>();
        searchDepth.value = _playerUiSettings.SearchDepth - 2;
        searchDepth.onValueChanged.AddListener(delegate { ChangeSearchDepth(); });

        ReplayButton.onClick.AddListener(delegate { ReplayGame(); });

        _playHistory = new Dictionary<short, GameObject>();

        PlayHistory();
    }

    private void ChangeReplayButtonText()
    {
        if (IsReplaying)
        {
            ReplayButton.GetComponentInChildren<Text>().text = "STOP";

        }
        else
        {
            ReplayButton.GetComponentInChildren<Text>().text = "REPLAY";
        }
    }

    private void OnLastPlay(short tileIndex)
    {
        if (_playHistory == null)
        {
            return;
        }

        _currentTurnIndex = (short)Plays.IndexOf(tileIndex);

        AddPlayButton(_currentTurnIndex);
        ColourPlayHistoryButtons((short)Plays.Count);
    }

    private void ChangeSearchDepth()
    {
        _playerUiSettings.SearchDepth = SearchDepthDropDown.GetComponent<Dropdown>().value + 2;
        SearchDepth = _playerUiSettings.SearchDepth;
    }

    void NewGame()
    {
        RestartGame();
        GameoverPanel.SetActive(false);
        var plays = _playHistory.Keys.Count;
        for (short i = 0; i < plays; i++)
        {
            if (_playHistory.ContainsKey(i))
                Destroy(_playHistory[i]);
        }
        _playHistory = new Dictionary<short, GameObject>();
        Messenger<short>.Broadcast("Notify tile", -1);
    }

    void Quit()
    {
        Application.Quit();
    }

    private void SaveSettings()
    {
        if (_playerUiSettings != null)
        {
            _playerUiSettings.Save();
        }
    }

    void OnGUI()
	{
        if (IsReplaying)
            return;

        TurnInfoGui();

        InfoGui();
    }

    private void InfoGui()
    {
        SearchInfo.text = "Search time: " + Math.Round(StopWatch.ElapsedMilliseconds / 1000D, 1) + " secs\nNodes searched: " + string.Format("{0:n0}", NodesSearched) + "\nTranspositions: " + string.Format("{0:n0}", Transpositions);

        var results = AnalysisInfo();

        if (InfoPlayIndex != null)
        {
            BlackAnalysis.text = "BLACK\r\n" + (_currentTurnIndex % 2 == 0 ? results[0] : results[1]);
            WhiteAnalysis.text = "WHITE\r\n" + (_currentTurnIndex % 2 == 0 ? results[1] : results[0]);
        }
        else
        {
            BlackAnalysis.text = "BLACK\r\n" + (_currentTurnIndex % 2 == 0 ? results[1] : results[0]);
            WhiteAnalysis.text = "WHITE\r\n" + (_currentTurnIndex % 2 == 0 ? results[0] : results[1]);
        }

        if (!string.IsNullOrEmpty(ArchiveInfo()))
        {
            ArchiveInfoPanel.SetActive(true);
            ArchiveInfoPanel.GetComponentInChildren<Text>().text = ArchiveInfo();
        }
        else
        {
            ArchiveInfoPanel.SetActive(false);
        }
    }
	
    void BlackIsHuman(Toggle toggle)
    {
        _playerUiSettings.BlackIsHuman = toggle.isOn;
    }

    void WhiteIsHuman(Toggle toggle)
    {
        _playerUiSettings.WhiteIsHuman = toggle.isOn;
    }
    void SetShowValidPlays(Toggle toggle)
    {
        ShowValidPlays = toggle.isOn;
    }
    void SetShowBoardCoordinates(Toggle toggle)
    {
        ShowBoardCoordinates = toggle.isOn;
    }
    void SetShowArchiveStats(Toggle toggle)
    {
        ShowArchiveStats = toggle.isOn;
    }

    bool _displayedGameOver;
    void TurnInfoGui()
	{		
		if (IsGameOver)
		{
            if (!_displayedGameOver)
            {
                _displayedGameOver = true;
                GameoverPanel.SetActive(true);

                GameoverPanel.GetComponent<Image>().color = GameWinner == "White" ? Color.white : Color.black;

                var gameOverText = GameObject.Find("Gameover Text").GetComponent<Text>();
                gameOverText.color = GameWinner == "White" ? Color.black : Color.white;

                var winner = GameObject.Find("Winner").GetComponent<Text>();
                winner.color = GameWinner == "White" ? Color.black : Color.white;
                winner.text = GameWinner.ToUpper();

                var gameResult = GameObject.Find("Game Result").GetComponent<Text>();
                gameResult.color = GameWinner == "White" ? Color.black : Color.white;
                gameResult.text = GameResult;
            }
		}
		else if (!CanPlay)
		{
            _displayedGameOver = false;

            if (IsComputerTurn)
			{
				SkipTurn();
			}
			else 
			{
                SkipTurnPanel.SetActive(true);
                SkipTurnPanel.GetComponentInChildren<Text>().text = CannotPlayMessage;
            }
		}
		else
		{
            _displayedGameOver = false;

            if (!PlayerTurn.text.StartsWith(Player.ToUpper()))
            {
                PlayerTurn.text = Player.ToUpper();
                PlayerTurn.color = Player == "Black" ? Color.black : Color.white;
            }
        }
	}

    public void StartButtonDown()
    {
        PlayToStart();
        ColourPlayHistoryButtons(-1);
        Messenger<short>.Broadcast("Notify tile", -1);
    }

    private void SkipTurn()
    {
        _gameManager.PlacePiece(null);
        _gameManager.NextTurn();
        DisplayPlays();
        SkipTurnPanel.SetActive(false);
    }

    void PlayHistory()
	{		
		if (!Plays.Any())
            return;
		
	    for (short i = 0; i < Plays.Count; i++) 
	    {
            AddPlayButton(i);
        }
	}
	
    void AddPlayButton(short turnIndex)
    {
        if (turnIndex < 0)
            return;

        if (_playHistory.ContainsKey(turnIndex))
        {
            if (_playHistory[turnIndex].GetComponentInChildren<Text>().text != Plays[turnIndex].ToAlgebraicNotation().ToUpper())
            {
                var removeUpTo = _playHistory.Count;
                for (var i = turnIndex; i < removeUpTo; i ++)
                {
                    if (_playHistory.ContainsKey(i))
                    {
                        Destroy(_playHistory[i]);
                        _playHistory.Remove(i);
                    }
                }
            }
            else
            {
                return;
            }
        }

        if (Plays[turnIndex] == null) // Is the turn skipped?
            return;

        var column = turnIndex % 2 == 0 ? 35 : 10;
        var row = (turnIndex / 2) * 13;

        var playButton = Instantiate(ButtonPrefab);
        playButton.transform.SetParent(GamePlayHistoryPanel.transform);

        playButton.GetComponent<RectTransform>().anchorMin = new Vector2(0.5f, 1);
        playButton.GetComponent<RectTransform>().anchorMax = new Vector2(0.5f, 1);
        playButton.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.5f);
        playButton.transform.localPosition = new Vector3(-column + 22.5f, -row + 210);

        playButton.GetComponentInChildren<Text>().text = Plays[turnIndex].ToAlgebraicNotation().ToUpper();
        var uniqueTurnReference = turnIndex; // https://answers.unity.com/questions/1121756/how-to-addlistener-from-code-featuring-an-argument.html
        playButton.GetComponent<Button>().onClick.AddListener(delegate { PlayTo(uniqueTurnReference); });
        _playHistory.Add(turnIndex, playButton);
    }

    void PlayTo(short turnIndex)
    {
        IsViewingHistory = true;
        ColourPlayHistoryButtons(turnIndex);

        GameoverPanel.SetActive(false);
        SkipTurnPanel.SetActive(false);

        var plays = Plays.GetRange(0, turnIndex).ToList();
        _gameManager = new GameManager(plays);
        CreatePieces();
        PlacePiece(Plays[turnIndex]);

        _currentTurnIndex = turnIndex;
        Messenger<short>.Broadcast("Notify tile", (short)Plays[turnIndex]);
    }

    private void ColourPlayHistoryButtons(short index)
    {
        if (index == -1)
        {
            StartButton.GetComponent<Image>().color = new Color(0.625f, 1, 1);
            index = 0;
        }
        else
        {
            StartButton.GetComponent<Image>().color = new Color(1, 1, 1);
        }

        for (short i = 0; i < _playHistory.Keys.Count; i++)
        {
            if (_playHistory.ContainsKey(i))
            {
                if (i < index || _playHistory.Keys.Count - 1 == index)
                {
                    _playHistory[i].GetComponent<Image>().color = new Color(1, 1, 1);
                }
                else
                {
                    _playHistory[i].GetComponent<Image>().color = new Color(0.625f, 1, 1);
                }
            }
        }
    }

    void ReplayGame()
    {
        Messenger<short>.Broadcast("Notify tile", -1);
        GameoverPanel.SetActive(false);
        Replay();
        ChangeReplayButtonText();
    }

    public bool ShowArchiveStats
    {
        get { return _playerUiSettings.ShowArchiveStats; }
        set
        {
            _playerUiSettings.ShowArchiveStats = value;
            GeneratePositionStats();
        }
    }

    public bool ShowValidPlays
    {
        get { return _playerUiSettings.ShowValidPlays; }
        set
        {
            if (_playerUiSettings.ShowValidPlays == value)
                return;
            _playerUiSettings.ShowValidPlays = value;
            CreatePieces();
        }
    }

    public bool ShowBoardCoordinates
    {
        get { return _playerUiSettings.ShowBoardCoordinates; }
        set
        {
            if (_playerUiSettings.ShowBoardCoordinates == value)
                return;
            _playerUiSettings.ShowBoardCoordinates = value;
            DrawBoardCoordinates();
        }
    }

    private List<EvaluationNode> _savedGameStateNodes;

    public bool IsComputerTurn
    {
        get { return ((_gameManager.PlayerIndex == 0 && !_playerUiSettings.BlackIsHuman) || (_gameManager.PlayerIndex == 1 && !_playerUiSettings.WhiteIsHuman)); }
    }


    bool _firstUpdate = true;

    void Update()
    {
        if (_firstUpdate)
        {
            _firstUpdate = false;
            if (Plays.Count > 0)
                Messenger<short>.Broadcast("Notify tile", (short)Plays.Last());
        }

        if (IsReplaying)
        {
            if (_stopwatch.ElapsedMilliseconds > 350 * (1 / _animationSpeed))
            {
                if (_gameManager.Turn < Plays.Count())
                {
                    PlacePiece(Plays[_gameManager.Turn]);
                    Messenger<short>.Broadcast("Notify tile", (short)Plays[_gameManager.Turn - 1]);
                    _stopwatch = Stopwatch.StartNew();
                }
                else
                {
                    Replay();
                    ChangeReplayButtonText();
                }
            }
        }
        else if (IsViewingHistory)
        {
            DrawStats();
        }
        else
        {
            DrawStats();
            ComputerPlay();
        }
    }

    void OnApplicationQuit()
    {
        Save();
    }

    void Save()
    {
        SaveSettings();
        SaveGame();
    }
    private void SaveGame()
    {
        if (_gameManager != null && Plays != null)
        {
            _gameManager.Save(Application.persistentDataPath + "/CurrentGame.txt", Plays);
        }
    }

    void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus)
        {
            Save();
        }
    }
    void OnTileSelected(short index)
    {
        if (IsComputerTurn)
            return;

        if (!_gameManager.CanPlay(index))
            return;

        Play(index);
    }

    void Play(short? tileIndex)
    {
        IsViewingHistory = false;
        PlacePiece(tileIndex);
        Plays = _gameManager.Plays;
        OnLastPlay((short)tileIndex);
        Messenger<short>.Broadcast("Notify tile", (short)tileIndex);
    }

    void PlacePiece(short? tileIndex)
    {
        _gameManager.PlacePiece(tileIndex);
        if (tileIndex != null)
        {
            PlaceAndFlipPieces();
        }
        _gameManager.NextTurn();
        DisplayPlays();
    }

    public short? InfoPlayIndex;

    void OnTileHover(short index)
    {
        InfoPlayIndex = null;
        if (_gameManager.CanPlay(index))
        {
            InfoPlayIndex = index;
        }
    }

    bool _canDrawStats;

    void DrawStats()
    {
        if (!_canDrawStats)
            return;

        DeleteTileInfo();

        if (!_playerUiSettings.ShowArchiveStats || !ShowValidPlays || IsReplaying)
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

            DrawTileInfo(coord.X, coord.Y, -0.4f, -.465f + (coord.Y * .01f), _positionStats[position].PlayStats[p].PercentageOfGames + "%");

            if (_gameManager.PlayerIsBlack)
                DrawTileInfo(coord.X, coord.Y, -0.4f, .275f + (coord.Y * .01f), _positionStats[position].PlayStats[p].PercentageOfWinsForBlack + "%");
            else
                DrawTileInfo(coord.X, coord.Y, -0.4f, .275f + (coord.Y * .01f), _positionStats[position].PlayStats[p].PercentageOfWinsForWhite + "%");
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

        playerPlays.ForEach(p => CreatePiece(p, _gameManager.PlayerIndex, 2, PieceBehaviour.TileHeight));
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


    public void RestartGame()
    {
        Plays = new List<short?>();
        _gameManager = new GameManager();
        CreatePieces();
        OnLastPlay(-1);
    }

    void PlaceAndFlipPieces()
    {
        var toDestroy = _gamePieces.Where(p => p.transform.localScale.x == 2).ToList();
        toDestroy.ForEach(p =>
        {
            Destroy(p);
            _gamePieces.Remove(p);
        });

        var playerColour = 1 - _gameManager.PlayerIndex;

        CreatePiece(_gameManager.Placement, playerColour, 23, -PieceStartingHeight);

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

        _gameManager.PlayerPieces.ForEach(p => CreatePiece(p, _gameManager.PlayerIndex, 23, PieceBehaviour.TileHeight));
        _gameManager.OpponentPieces.ForEach(p => CreatePiece(p, opponentColour, 23, PieceBehaviour.TileHeight));
        DisplayPlays();
    }

    GameObject CreatePiece(short pieceIndex, int colour, float scale, float z)
    {
        var gamePiece = (GameObject)Instantiate(Piece);
        var point = pieceIndex.ToCartesianCoordinate();
        gamePiece.transform.position = GetWorldCoordinates(point.X, point.Y, z, BoardLocation);
        gamePiece.transform.localScale = new Vector3(scale, scale, scale * 0.2f);

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

        if (!ShowBoardCoordinates)
            return;

        for (var i = 0; i < _width; i++)
        {
            DrawCoordinate(i, 0, -0.05f, 7.9f, true);
            DrawCoordinate(0, i, -0.7f, .2f, false);
        }
    }

    void DrawCoordinate(int x, int y, float xOffset, float yOffset, bool chararacter)
    {
        var text = (GameObject)Instantiate(Text);
        ((TextMesh)text.transform.GetComponent("TextMesh")).text = chararacter ? ((char)(x + 65)).ToString() : (y + 1).ToString();
        text.transform.position = GetWorldCoordinates(-.05f + x + xOffset, -.35f + y + yOffset, 0, BoardLocation);

        _boardCoordinates.Add(text);
    }

    static Vector3 GetWorldCoordinates(float x, float y, float z, Point boardLocation)
    {
        return new Vector3(x * Spacing + (_width * Spacing * boardLocation.X * 1.075f) + 1f, -y * Spacing + (_height * Spacing * boardLocation.Y * 1.075f), z);
    }

    public void PlayToStart()
    {
        _gameManager = new GameManager();
        CreatePieces();
        OnLastPlay(-1);
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

    public string GameWinner
    {
        get { return _gameManager.GameWinner; }
    }

    public string GameResult
    {
        get { return _gameManager.GameResult; }
    }

    public string CannotPlayMessage
    {
        get { return string.Format("{0} can not play\n{1} to play instead", _gameManager.Player.ToUpper(), _gameManager.Opponent.ToUpper()); }
    }

    internal void Replay()
    {
        IsReplaying = !IsReplaying;

        DeleteTileInfo();
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

    internal string[] AnalysisInfo()
    {
        return _gameManager.AnalysisInfo(InfoPlayIndex, _computerPlayer);
    }

    public static int Transpositions { get; set; }

    internal string ArchiveInfo()
    {
        if (InfoPlayIndex == null)
            return null;
        if (!_statsAvailable)
            return null;

        var position = _gameManager.Plays.ToChars();

        if (!_positionStats[position].PlayStats.ContainsKey(((short)InfoPlayIndex)))
            return null;

        var stats = _positionStats[position].PlayStats[((short)InfoPlayIndex)];
        var stringBuilder = new StringBuilder();
        stringBuilder.AppendLine(string.Format("{0}% of games play {1}", stats.PercentageOfGames, InfoPlayIndex.ToAlgebraicNotation()));
        stringBuilder.AppendLine(string.Format("{0}% of games won by black", stats.PercentageOfWinsForBlack));
        stringBuilder.AppendLine(string.Format("{0}% of games won by white", stats.PercentageOfWinsForWhite));
        stringBuilder.AppendLine(string.Format("{0}% of games were draws", stats.PercentageOfDraws));

        return stringBuilder.ToString();
    }
}