using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using Othello.Model;
using UnityEngine.UI;

public class GamesController : MonoBehaviour
{
    public GameObject NewGameButton;
	public GameObject Piece;
	public GameObject BoardTile;
	public GameObject Text;
    public GameObject ButtonPrefab;
    public GameObject TogglePrefab;
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
    public Text GameAnalysis;
    public Dropdown SearchDepthDropDown;
    public Button ReplayButton;

    private List<GameBehaviour> _games;
	GameBehaviour _activeGame;

    private List<string> _gameArchive;
	
	public static string SavePath = @"Save\";
    
    private GUIStyle listStyle = new GUIStyle();

    private PlayerUiSettings _playerUiSettings;
    public Toggle ShowValidPlaysToggle, ShowBoardCoordinatesToggle, ShowArchiveStatsToggle;

    Dictionary<short, GameObject> _playHistory;

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


        NewGameButton.GetComponent<Button>().onClick.AddListener(NewGame);

        WhiteHuman.onValueChanged.AddListener(delegate { WhiteIsHuman(WhiteHuman); });
        WhiteHuman.isOn = _playerUiSettings.WhiteIsHuman;

        BlackHuman.onValueChanged.AddListener(delegate { BlackIsHuman(BlackHuman); });
        BlackHuman.isOn = _playerUiSettings.BlackIsHuman;

        ShowValidPlaysToggle.onValueChanged.AddListener(delegate { ShowValidPlays(ShowValidPlaysToggle); });
        ShowValidPlaysToggle.isOn = _playerUiSettings.ShowValidPlays;

        ShowBoardCoordinatesToggle.onValueChanged.AddListener(delegate { ShowBoardCoordinates(ShowBoardCoordinatesToggle); });
        ShowBoardCoordinatesToggle.isOn = _playerUiSettings.ShowBoardCoordinates;

        ShowArchiveStatsToggle.onValueChanged.AddListener(delegate { ShowArchiveStats(ShowArchiveStatsToggle); });
        ShowArchiveStatsToggle.isOn = _playerUiSettings.ShowArchiveStats;

        SkipTurnPanel.GetComponentInChildren<Button>().onClick.AddListener(delegate { ShipTurn(); });

        var searchDepth = SearchDepthDropDown.GetComponent<Dropdown>();
        searchDepth.value = _playerUiSettings.SearchDepth - 2;
        searchDepth.onValueChanged.AddListener(delegate { ChangeSearchDepth(); });

        listStyle.normal.textColor = Color.white;
        listStyle.onHover.background = listStyle.hover.background = new Texture2D(2, 2);
        listStyle.padding.left = listStyle.padding.right = listStyle.padding.top = listStyle.padding.bottom = 4;

        ReplayButton.onClick.AddListener(delegate { ReplayGame(); });

        Messenger<short>.AddListener("Last play", OnLastPlay);
        _playHistory = new Dictionary<short, GameObject>();

        PlayHistory();

        Messenger.AddListener("Replay finished", ChangeReplayButtonText);
    }

    private void ChangeReplayButtonText()
    {
        if (_activeGame.IsReplaying)
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
        if (_activeGame == null || _playHistory == null)
        {
            return;
        }

        var index =  (short)_activeGame.Plays.IndexOf(tileIndex);

        AddPlayButton(index);
    }

    private void ChangeSearchDepth()
    {
        _playerUiSettings.SearchDepth = SearchDepthDropDown.GetComponent<Dropdown>().value + 2;
        _activeGame.SearchDepth = _playerUiSettings.SearchDepth;
    }

    void NewGame()
    {
        _activeGame.RestartGame();
        GameoverPanel.SetActive(false);
        PlayHistory();
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
	    if (_activeGame.IsReplaying)
            return;

	    TurnInfoGui();

	    InfoGui();
	    //GameSpeedGui();
	}

    private void InfoGui()
    {
        //if (!_searchComboBox.IsClickedComboButton && !_depthComboBox.IsClickedComboButton)
            SearchInfo.text = "Search time: " + Math.Round(_activeGame.StopWatch.ElapsedMilliseconds / 1000D, 1) + " secs\nNodes searched: " + string.Format("{0:n0}", _activeGame.NodesSearched) + "\nTranspositions: " + string.Format("{0:n0}", GameBehaviour.Transpositions);

        //if (!_depthComboBox.IsClickedComboButton)
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

    void PlayHistory()
	{		
		if (!_activeGame.Plays.Any())
            return;
		
	    for (short i = 0; i < _activeGame.Plays.Count; i++) 
	    {
            AddPlayButton(i);
        }
	}
	
    void AddPlayButton(short index)
    {
        if (index < 0)
            return;
        if (_activeGame.Plays.Count < index || _playHistory.ContainsKey(index))
            return;

        if (_activeGame.Plays[index] == null) // Is the turn skipped?
            return;

        var column = index % 2 == 0 ? 35 : 10;
        var row = (index / 2) * 14;



        var playButton = Instantiate(ButtonPrefab);
        playButton.transform.SetParent(GamePlayHistoryPanel.transform);

        playButton.GetComponent<RectTransform>().anchorMin = new Vector2(0.5f, 1);
        playButton.GetComponent<RectTransform>().anchorMax = new Vector2(0.5f, 1);
        playButton.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.5f);
        playButton.transform.localPosition = new Vector3(-column + 22.5f, -row + 200);

        playButton.GetComponentInChildren<Text>().text = _activeGame.Plays[index].ToAlgebraicNotation().ToUpper();
        var uniqueIndexReference = index; // https://answers.unity.com/questions/1121756/how-to-addlistener-from-code-featuring-an-argument.html
        playButton.GetComponent<Button>().onClick.AddListener(delegate { PlayTo(uniqueIndexReference); });
        _playHistory.Add(index, playButton);
    }

    void PlayTo(short index)
    {
        Debug.Log(index);
        GameoverPanel.SetActive(false);
        _activeGame.PlayTo(index);
    }

    void ReplayGame()
    {
        GameoverPanel.SetActive(false);
        _games.ForEach(x => x.Replay());
        ChangeReplayButtonText();
    }


}