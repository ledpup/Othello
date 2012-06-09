using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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
	float _globalAnimationSpeed = .5f;

    private List<string> _gameArchive;
	
	public static string SavePath = @"Save\";
	
	void Start()
	{	
	    _gameArchive = File.ReadAllLines(SavePath + "ArchiveData.txt").ToList();

	    _games = new List<GameBehaviour>();

	    var gameManagers = GameManager.LoadGamesFromFile(SavePath + "CurrentGame.txt");
		
        //gameManagers.ForEach(x => _games.Add(GameBehaviour.CreateGameBehaviour(gameObject, GuiSkin, BoardTile, Piece, Text, gameManagers.IndexOf(x).ToCartesianCoordinate(), x)));

        _games.Add(GameBehaviour.CreateGameBehaviour(gameObject, GuiSkin, BoardTile, Piece, Text, ((short)0).ToCartesianCoordinate(), gameManagers.First(), _gameArchive));
		
		//_games.Add(GameBehaviour.CreateGameBehaviour(gameObject, GuiSkin, BoardTile, Piece, Text, new Point(0, 0)));
		
		_activeGame = _games.First();
		
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
		    GUI.TextArea(new Rect(20, 300, 180, 40), "Nodes searched: " + _activeGame.NodesSearched + "\nTranspositions: " + GameBehaviour.Transpositions);



		    GuiSkin.textArea.alignment = TextAnchor.UpperLeft;

		    //var style = new GUIStyle {alignment = TextAnchor.UpperLeft, };

            GUI.TextArea(new Rect(20, 340, 180, 100), _activeGame.ArchiveInfo());
		    GUI.TextArea(new Rect(20, 440, 180, 130), _activeGame.AnalysisInfo());
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
				
		_selection = GUI.SelectionGrid(new Rect (20, 150, 150, 60), _selection, new [] { "No Tile Info", "Show Archive Stats", "Show Evaluation Data" }, 1);
		
		_games.ForEach(x => x.DisplayText = (DisplayText)_selection);
	}
	
	void GamePersistenceGui()
	{
        if (GUI.Button(new Rect(20, 20, 80, 20), "New Game"))
        {
            _activeGame.Plays = new List<short?>();
            _activeGame.StartGameBehavour(new GameManager());
        }
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
		if (GUI.Button(new Rect(Screen.width - 140, 0, 80, 20), "Start"))
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
	        var row = (i / 2) * 18;
	        if (GUI.Button(new Rect(Screen.width - 80 - column, 20 + row, 40, 18), _activeGame.Plays[i].ToAlgebraicNotation()))
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
        if (GUI.Button(new Rect(Screen.width - 240, 0, 80, 20), _activeGame.IsReplaying ? "Stop" : "Replay"))
		{
		    //_activeGame.Replay();
			_games.ForEach(x => x.Replay());
		}
	}
	
	
}