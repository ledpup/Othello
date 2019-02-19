using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Othello.Assets.UI;
using UnityEngine;
using Othello.Model;
using UnityEngine.UI;

public class GamesController : MonoBehaviour
{
	public GameObject Piece;
	public GameObject BoardTile;
	public GameObject Text;
    public GameObject ButtonPrefab;
    public GameObject TogglePrefab;
    public GameObject Panel;
    public GameObject GameOptionsPanel;
    public GameObject ViewOptionsPanel;
    public GameObject GameoverPanel;
    public GameObject SkipTurnPanel;
    public GameObject ArchiveInfoPanel;
    public GameObject GamePlayHistoryPanel;
    public Text SearchInfo;
    public Text PlayerTurn;
    public Text GameAnalysis;

    private List<GameBehaviour> _games;
	GameBehaviour _activeGame;
	//float _globalAnimationSpeed = .15f;

    private List<string> _gameArchive;
	
	public static string SavePath = @"Save\";

    GUIContent[] _searchMethods, _searchDepths;
    private ComboBox _searchComboBox = new ComboBox();
    private ComboBox _depthComboBox = new ComboBox();
    private GUIStyle listStyle = new GUIStyle();

    private PlayerUiSettings _playerUiSettings;
    Toggle _blackIsHumanToggle, _whiteIsHumanToggle, _showValidPlaysToggle, _showBoardCoordinatesToggle, _showArchiveStatsToggle;

    void Start()
	{
        GameoverPanel.SetActive(false);
        SkipTurnPanel.SetActive(false);

        _gameArchive = File.ReadAllLines(SavePath + "ArchiveData.txt").ToList();

	    _games = new List<GameBehaviour>();

	    var gameManagers = GameManager.LoadGamesFromFile(SavePath + "CurrentGame.txt");
		
		_playerUiSettings = PlayerUiSettings.Load();
        _games.Add(GameBehaviour.CreateGameBehaviour(gameObject, BoardTile, Piece, Text, ((short)0).ToCartesianCoordinate(), gameManagers.First(), _gameArchive, _playerUiSettings));

		_activeGame = _games.First();


        var newGameButton = Instantiate(ButtonPrefab);
        newGameButton.transform.SetParent(Panel.transform);
        newGameButton.GetComponentInChildren<Text>().text = "New Game";
        newGameButton.GetComponent<Button>().onClick.AddListener(NewGame);

        var quitButton = Instantiate(ButtonPrefab);
        quitButton.transform.SetParent(Panel.transform);
        quitButton.GetComponentInChildren<Text>().text = "Quit";
        quitButton.GetComponent<Button>().onClick.AddListener(Quit);


        var toggle = Instantiate(TogglePrefab);
        toggle.transform.SetParent(GameOptionsPanel.transform);
        toggle.GetComponentInChildren<Text>().text = "Black is a human player";
        _blackIsHumanToggle = toggle.GetComponent<Toggle>();
        _blackIsHumanToggle.onValueChanged.AddListener(delegate { BlackIsHuman(_blackIsHumanToggle); });

        toggle = Instantiate(TogglePrefab);
        toggle.transform.SetParent(GameOptionsPanel.transform);
        toggle.GetComponentInChildren<Text>().text = "White is a human player";
        _whiteIsHumanToggle = toggle.GetComponent<Toggle>();
        _whiteIsHumanToggle.onValueChanged.AddListener(delegate { WhiteIsHuman(_whiteIsHumanToggle); });

        toggle = Instantiate(TogglePrefab);
        toggle.transform.SetParent(ViewOptionsPanel.transform);
        toggle.GetComponentInChildren<Text>().text = "Show valid plays";
        _showValidPlaysToggle = toggle.GetComponent<Toggle>();
        _showValidPlaysToggle.onValueChanged.AddListener(delegate { ShowValidPlays(_showValidPlaysToggle); });

        toggle = Instantiate(TogglePrefab);
        toggle.transform.SetParent(ViewOptionsPanel.transform);
        toggle.GetComponentInChildren<Text>().text = "Show board coordinates";
        _showBoardCoordinatesToggle = toggle.GetComponent<Toggle>();
        _showBoardCoordinatesToggle.onValueChanged.AddListener(delegate { ShowBoardCoordinates(_showBoardCoordinatesToggle); });

        toggle = Instantiate(TogglePrefab);
        toggle.transform.SetParent(ViewOptionsPanel.transform);
        toggle.GetComponentInChildren<Text>().text = "Show archive stats";
        _showArchiveStatsToggle = toggle.GetComponent<Toggle>();
        _showArchiveStatsToggle.onValueChanged.AddListener(delegate { ShowArchiveStats(_showArchiveStatsToggle); });

        SkipTurnPanel.GetComponentInChildren<Button>().onClick.AddListener(delegate { ShipTurn(); });

        //_activeGame.UseTranspositionTable = GUI.Toggle(new Rect(20, 150, 200, 20), _activeGame.UseTranspositionTable, "Use transposition table");
        //_activeGame.UseOpeningBook = GUI.Toggle(new Rect(20, 170, 200, 20), _activeGame.UseOpeningBook, "Use opening book");

        _searchMethods = new[] { new GUIContent("NegaMax"), new GUIContent("NegaMax w/ Alpha-Beta") };
        _searchDepths = new[] 
		{ 
			new GUIContent("Search Depth 0"), 
			new GUIContent("Search Depth 1"), 
			new GUIContent("Search Depth 2"), 
			new GUIContent("Search Depth 3"), 
			new GUIContent("Search Depth 4"), 
			new GUIContent("Search Depth 5"), 
			new GUIContent("Search Depth 6"),
            new GUIContent("Search Depth 7"),
		};
        _searchComboBox.SelectedItemIndex = _playerUiSettings.SearchMethod;
        _depthComboBox.SelectedItemIndex = _playerUiSettings.SearchDepth;

        listStyle.normal.textColor = Color.white;
        listStyle.onHover.background = listStyle.hover.background = new Texture2D(2, 2);
        listStyle.padding.left = listStyle.padding.right = listStyle.padding.top = listStyle.padding.bottom = 4;

        UndoRedoGui();
    }

    void NewGame()
    {
        _activeGame.RestartGame();
        GameoverPanel.SetActive(false);
        UndoRedoGui();
    }

    void Quit()
    {
        Application.Quit();
    }

    void OnApplicationQuit()
	{
	    _playerUiSettings.Save();
	}
	
	void OnGUI()
	{
        Replay();
		
	    if (_activeGame.IsReplaying)
            return;

	    TurnInfoGui();

	    InfoGui();
	    //GameSpeedGui();
	}

    private void InfoGui()
    {
        if (!_searchComboBox.IsClickedComboButton && !_depthComboBox.IsClickedComboButton)
            SearchInfo.text = "Search time: " + Math.Round(_activeGame.StopWatch.ElapsedMilliseconds / 1000D, 1) + " secs\nNodes searched: " + _activeGame.NodesSearched + "\nTranspositions: " + GameBehaviour.Transpositions;

        if (!_depthComboBox.IsClickedComboButton)
            GameAnalysis.text = _activeGame.AnalysisInfo();
        if (!string.IsNullOrEmpty(_activeGame.ArchiveInfo()))
        {
            ArchiveInfoPanel.SetActive(true);
            ArchiveInfoPanel.GetComponentInChildren<Text>().text = _activeGame.ArchiveInfo();
        }
        else
        {
            ArchiveInfoPanel.SetActive(false);
        }
    }

    void OptionsGui()
    {


        _activeGame.SearchMethod = _searchComboBox.List(new Rect(20, 200, 150, 20), _searchMethods[_activeGame.SearchMethod].text, _searchMethods, listStyle);
        if (!_searchComboBox.IsClickedComboButton)
            _activeGame.SearchDepth = _depthComboBox.List(new Rect(20, 220, 150, 20), _searchDepths[_activeGame.SearchDepth].text, _searchDepths, listStyle);
    }
	
    void BlackIsHuman(Toggle toggle)
    {
        _playerUiSettings.BlackIsHuman = toggle.isOn;
    }

    void WhiteIsHuman(Toggle toggle)
    {
        _playerUiSettings.WhiteIsHuman = toggle.isOn;
    }
    void ShowValidPlays(Toggle toggle)
    {
        _activeGame.ShowValidPlays = toggle.isOn;
    }
    void ShowBoardCoordinates(Toggle toggle)
    {
        _activeGame.ShowBoardCoordinates = toggle.isOn;
    }
    void ShowArchiveStats(Toggle toggle)
    {
        _activeGame.ShowArchiveStats = toggle.isOn;
    }

    bool _displayedGameOver;
    void TurnInfoGui()
	{
		var labelWidth = 200;
		var labelHeight = 80;
		
		var x = Screen.width / 2 - labelWidth / 2;
		var y = Screen.height / 2 - labelHeight / 2;
		
		if (_activeGame.IsGameOver)
		{
            if (!_displayedGameOver)
            {
                _displayedGameOver = true;
                GameoverPanel.SetActive(true);

                GameoverPanel.GetComponent<Image>().color = _activeGame.GameWinner == "White" ? Color.white : Color.black;

                var gameOverText = GameObject.Find("Gameover Text").GetComponent<Text>();
                gameOverText.color = _activeGame.GameWinner == "White" ? Color.black : Color.white;

                var winner = GameObject.Find("Winner").GetComponent<Text>();
                winner.color = _activeGame.GameWinner == "White" ? Color.black : Color.white;
                winner.text = _activeGame.GameWinner.ToUpper();

                var gameResult = GameObject.Find("Game Result").GetComponent<Text>();
                gameResult.color = _activeGame.GameWinner == "White" ? Color.black : Color.white;
                gameResult.text = _activeGame.GameResult;
            }
		}
		else if (!_activeGame.CanPlay)
		{
            _displayedGameOver = false;

            if (_activeGame.IsComputerTurn)
			{
				_activeGame.SkipTurn();
			}
			else 
			{
                SkipTurnPanel.SetActive(true);
                SkipTurnPanel.GetComponentInChildren<Text>().text = _activeGame.CannotPlayMessage;

            }
		}
		else
		{
            _displayedGameOver = false;

            if (!PlayerTurn.text.StartsWith(_activeGame.Player.ToUpper()))
            {
                PlayerTurn.text = _activeGame.Player.ToUpper();
                PlayerTurn.color = _activeGame.Player == "Black" ? Color.black : Color.white;
            }
        }
	}

    public void StartButtonDown()
    {
        _activeGame.PlayToStart();
    }

    private void ShipTurn()
    {
        _activeGame.SkipTurn();
        SkipTurnPanel.SetActive(false);
    }

    void UndoRedoGui()
	{		
		if (!_activeGame.Plays.Any())
            return;
		
	    for (short i = 0; i < _activeGame.Plays.Count; i++) 
	    { 
	        if (_activeGame.Plays[i] == null)
	            continue;

            var column = i % 2 == 0 ? 30 : 10;
            var row = (i / 2) * 12;
            var playButton = Instantiate(ButtonPrefab);
            playButton.transform.SetParent(GamePlayHistoryPanel.transform);
            playButton.transform.localPosition = new Vector3(column - 20, row - 50);


            playButton.GetComponentInChildren<Text>().text = _activeGame.Plays[i].ToAlgebraicNotation();
            playButton.GetComponent<Button>().onClick.AddListener(PlayTo);
            

        }
	}
	
    void PlayTo()
    {
        GameoverPanel.SetActive(false);
        _activeGame.PlayTo(5);
    }
	
	void Replay()
	{
        if (GUI.Button(new Rect(Screen.width - 200, 0, 80, 20), _activeGame.IsReplaying ? "Stop" : "Replay"))
		{
            GameoverPanel.SetActive(false);
            _games.ForEach(x => x.Replay());
		}
	}
	
	
}