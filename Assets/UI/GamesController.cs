using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using Reversi.Model;

public enum DisplayText
{
	None,
	ArchiveStats,
	EvaluationData,
}

public class GamesController : MonoBehaviour
{
	public GameObject Piece;
	public GameObject BoardTile;
	public GameObject Text;
	public GUISkin GuiSkin;
	
	
	
	private List<GameBehaviour> _games;
	GameBehaviour _activeGame;
	float _globalGameSpeed = .5f;

    private List<string> _gameArchive;
	
	public static string SavePath = @"Save\";
	
	void Start()
	{
	    _gameArchive = File.ReadAllLines(SavePath + "ArchiveData.txt").ToList();

	    _games = new List<GameBehaviour>();

	    var gameManagers = GameManager.LoadGamesFromFile(SavePath + "Games Archive.txt");
		
        //gameManagers.ForEach(x => _games.Add(GameBehaviour.CreateGameBehaviour(gameObject, GuiSkin, BoardTile, Piece, Text, gameManagers.IndexOf(x).ToCartesianCoordinate(), x)));

        _games.Add(GameBehaviour.CreateGameBehaviour(gameObject, GuiSkin, BoardTile, Piece, Text, ((short)0).ToCartesianCoordinate(), gameManagers.First(), _gameArchive));
		
		//_games.Add(GameBehaviour.CreateGameBehaviour(gameObject, GuiSkin, BoardTile, Piece, Text, new Point(0, 0)));
		
		_activeGame = _games.First();
		
	}

    private void LoadTrieData()
    {
        
    }
	
	void OnApplicationQuit()
	{
	}
	
	void OnGUI()
	{
		GUI.skin = GuiSkin;
			
		Replay();
		if (!_activeGame.IsReplaying)
		{
			OptionsGui();
			GamePersistenceGui();
			TurnInfoGui();
			UndoRedoGui();
		}
		GameSpeedGui();
	}
	
	int _selection;
	
	void OptionsGui()
	{
		_activeGame.BlackIsHuman = GUI.Toggle (new Rect (20, 50, 200, 20), _activeGame.BlackIsHuman, "Black is a human player");
		_activeGame.WhiteIsHuman = GUI.Toggle (new Rect (20, 70, 200, 20), _activeGame.WhiteIsHuman, "White is a human player");
		_activeGame.ShowValidPlays = GUI.Toggle (new Rect (20, 90, 200, 20), _activeGame.ShowValidPlays, "Show valid plays");
		_activeGame.ShowBoardCoordinates = GUI.Toggle (new Rect (20, 110, 200, 20), _activeGame.ShowBoardCoordinates, "Show board coordinates");
		
		var blah = new GUIContent[1];
		
		_selection = GUI.SelectionGrid(new Rect (20, 150, 150, 80), _selection, new string[3] { "No Tile Info", "Show Archive Stats", "Show Evaluation Data" }, 1);
		
		_games.ForEach(x => x.DisplayText = (DisplayText)_selection);
	}
	
	void GamePersistenceGui()
	{
//		if (GUI.Button(new Rect(20, 20, 80, 20), "New Game"))
//		{
//			_activeGame.Plays = new List<int?>();
//			_activeGame.GameManager = new GameManager();
//        	_activeGame.CreatePieces();
//		}
//		else if (GUI.Button(new Rect(100, 20, 80, 20), "Load"))
//		{
//			_gameManager.Load(@"Save\CurrentGame.txt");
//			CreatePieces();
//		}
//		else if (GUI.Button(new Rect(180, 20, 80, 20), "Save"))
//			_gameManager.Save(@"Save\CurrentGame.txt");
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
		if (GUI.Button(new Rect(Screen.width - 160, 0, 80, 20), "Start"))
		{
			_activeGame.RestartGame();
		}
		
		if (!_activeGame.Plays.Any())
            return;
		
	    for (var i = 0; i < _activeGame.Plays.Count; i++) 
	    { 
	        if (_activeGame.Plays[i] == null)
	            continue;

	        var column = i % 2 == 0 ? 60 : 20;
	        var row = (i / 2) * 20;
	        if (GUI.Button(new Rect(Screen.width - 100 - column, 20 + row, 40, 20), _activeGame.Plays[i].ToAlgebraicNotation()))
	        {
				_activeGame.PlayTo(i);
	        }
	    }
	}
	
	void GameSpeedGui()
	{
		var oldGameSpeed = _globalGameSpeed;
		_globalGameSpeed = GUI.HorizontalSlider (new Rect (25, 250, 100, 30), _globalGameSpeed, 0.0f, 1.0f);
		if (oldGameSpeed != _globalGameSpeed)
		{
			Messenger<float>.Broadcast("Game speed changed", _globalGameSpeed);
		}
	}
	
	void Replay()
	{
        if (GUI.Button(new Rect(Screen.width - 260, 0, 80, 20), _activeGame.IsReplaying ? "Stop" : "Replay"))
		{
		    //_activeGame.Replay();
			_games.ForEach(x => x.Replay());
		}
	}
	
	
}