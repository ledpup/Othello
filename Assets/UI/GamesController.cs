using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Othello.Assets.UI;
using UnityEngine;
using Othello.Model;

public class GamesController : MonoBehaviour
{
	public GameObject Piece;
	public GameObject BoardTile;
	public GameObject Text;
	public GUISkin GuiSkin;
	
	private List<GameBehaviour> _games;
	GameBehaviour _activeGame;
	float _globalAnimationSpeed = .5f;

    private List<string> _gameArchive;
	
	public static string SavePath = @"Save\";

    GUIContent[] _searchMethods, _searchDepths;
    private ComboBox _searchComboBox = new ComboBox();
    private ComboBox _depthComboBox = new ComboBox();
    private GUIStyle listStyle = new GUIStyle();

    private PlayerUiSettings _playerUiSettings;

	void Start()
	{
        _gameArchive = File.ReadAllLines(SavePath + "ArchiveData.txt").ToList();

	    _games = new List<GameBehaviour>();

	    var gameManagers = GameManager.LoadGamesFromFile(SavePath + "CurrentGame.txt");
		
		_playerUiSettings = PlayerUiSettings.Load();
        _games.Add(GameBehaviour.CreateGameBehaviour(gameObject, GuiSkin, BoardTile, Piece, Text, ((short)0).ToCartesianCoordinate(), gameManagers.First(), _gameArchive, _playerUiSettings));

		_activeGame = _games.First();

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
	}
	
	void OnApplicationQuit()
	{
	    _playerUiSettings.Save();
	}
	
	void OnGUI()
	{
		GUI.skin = GuiSkin;
			
		Replay();
		
	    if (_activeGame.IsReplaying)
            return;

	    OptionsGui();
	    GameGui();
	    TurnInfoGui();
	    UndoRedoGui();
	    InfoGui();
	    //GameSpeedGui();
	}

    private void InfoGui()
    {
        if (!_searchComboBox.IsClickedComboButton && !_depthComboBox.IsClickedComboButton)
            GUI.TextArea(new Rect(20, 260, 180, 50), "Search time: " + Math.Round(_activeGame.StopWatch.ElapsedMilliseconds / 1000D, 1) + " secs\nNodes searched: " + _activeGame.NodesSearched + "\nTranspositions: " + GameBehaviour.Transpositions);

        GuiSkin.textArea.alignment = TextAnchor.UpperLeft;

        if (!_depthComboBox.IsClickedComboButton)
            GUI.TextArea(new Rect(20, 310, 180, 130), _activeGame.AnalysisInfo());
        if (!string.IsNullOrEmpty(_activeGame.ArchiveInfo()))
            GUI.TextArea(new Rect(20, 440, 180, 80), _activeGame.ArchiveInfo());
    }
	
	void OptionsGui()
	{
        _playerUiSettings.BlackIsHuman = GUI.Toggle(new Rect(20, 50, 200, 20), _playerUiSettings.BlackIsHuman, "Black is a human player");
        _playerUiSettings.WhiteIsHuman = GUI.Toggle(new Rect(20, 70, 200, 20), _playerUiSettings.WhiteIsHuman, "White is a human player");
        _activeGame.ShowValidPlays = GUI.Toggle(new Rect(20, 90, 200, 20), _activeGame.ShowValidPlays, "Show valid plays");
        _activeGame.ShowBoardCoordinates = GUI.Toggle(new Rect(20, 110, 200, 20), _activeGame.ShowBoardCoordinates, "Show board coordinates");
        _activeGame.ShowArchiveStats = GUI.Toggle(new Rect(20, 130, 200, 20), _activeGame.ShowArchiveStats, "Show archive stats");
        _activeGame.UseTranspositionTable = GUI.Toggle(new Rect(20, 150, 200, 20), _activeGame.UseTranspositionTable, "Use transposition table");
        _activeGame.UseOpeningBook = GUI.Toggle(new Rect(20, 170, 200, 20), _activeGame.UseOpeningBook, "Use opening book");

        _activeGame.SearchMethod = _searchComboBox.List(new Rect(20, 200, 150, 20), _searchMethods[_activeGame.SearchMethod].text, _searchMethods, listStyle);
        if (!_searchComboBox.IsClickedComboButton)
            _activeGame.SearchDepth = _depthComboBox.List(new Rect(20, 220, 150, 20), _searchDepths[_activeGame.SearchDepth].text, _searchDepths, listStyle);
	}
	
	void GameGui()
	{
        if (GUI.Button(new Rect(20, 20, 80, 20), "New Game"))
        {
            _activeGame.RestartGame();
        }
		if (GUI.Button(new Rect(120, 20, 80, 20), "Quit"))
        {
            Application.Quit();
        }
	}
	
	void TurnInfoGui()
	{
		var labelWidth = 200;
		var labelHeight = 80;
		
		var x = Screen.width / 2 - labelWidth / 2;
		var y = Screen.height / 2 - labelHeight / 2;
		
		if (_activeGame.IsGameOver)
		{
			var message = _activeGame.GameOverMessage;
		    
            GUI.Label(new Rect(x, y, labelWidth, labelHeight), message);
		}
		else if (!_activeGame.CanPlay)
		{
			if (_activeGame.IsComputerTurn)
			{
				_activeGame.SkipTurn();
			}
			else 
			{
				GUI.Label (new Rect(x, y, labelWidth, labelHeight), _activeGame.CannotPlayMessage);
				if (GUI.Button(new Rect(x, Screen.height / 2 + labelHeight / 2, labelWidth, 20), "Okay"))
				{
					_activeGame.SkipTurn();
				}
			}
		}
		else
		{
			GUI.Label (new Rect(Screen.width / 2 - labelWidth / 2, 20, labelWidth, 20), string.Format("{0} to play.", _activeGame.Player));
		}
	}

	void UndoRedoGui()
	{
		if (GUI.Button(new Rect(Screen.width - 100, 0, 80, 20), "Start"))
		{
			_activeGame.PlayToStart();
		}
		
		if (!_activeGame.Plays.Any())
            return;
		
	    for (short i = 0; i < _activeGame.Plays.Count; i++) 
	    { 
	        if (_activeGame.Plays[i] == null)
	            continue;

	        var column = i % 2 == 0 ? 60 : 20;
	        var row = (i / 2) * 18;
	        if (GUI.Button(new Rect(Screen.width - 40 - column, 20 + row, 40, 18), _activeGame.Plays[i].ToAlgebraicNotation()))
	        {
				_activeGame.PlayTo(i);
	        }
	    }
	}
	
	void GameSpeedGui()
	{
		var oldGameSpeed = _globalAnimationSpeed;
        GUI.TextField(new Rect(20, 240, 130, 25), "Animation Speed");
		_globalAnimationSpeed = GUI.HorizontalSlider (new Rect (20, 270, 130, 30), _globalAnimationSpeed, 0.0f, 1.0f);
		if (oldGameSpeed != _globalAnimationSpeed)
		{
			Messenger<float>.Broadcast("Game speed changed", _globalAnimationSpeed);
		}
	}
	
	void Replay()
	{
        if (GUI.Button(new Rect(Screen.width - 200, 0, 80, 20), _activeGame.IsReplaying ? "Stop" : "Replay"))
		{
			_games.ForEach(x => x.Replay());
		}
	}
	
	
}