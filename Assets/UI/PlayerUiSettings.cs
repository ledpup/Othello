using System;
using System.IO;
using Othello.Model;

[Serializable]
public class PlayerUiSettings
{
    public bool BlackIsHuman;
    public bool WhiteIsHuman;
	public bool ShowValidPlays;
	public bool ShowBoardCoordinates;
    public bool ShowArchiveStats;
    public bool UseTranspositionTable;
    public bool UseOpeningBook;
    public int SearchMethod;
    public int SearchDepth;

    public PlayerUiSettings()
    {
        BlackIsHuman = true;
        WhiteIsHuman = false;
        ShowBoardCoordinates = true;
        ShowValidPlays = true;
        UseTranspositionTable = true;
        UseOpeningBook = true;
        SearchDepth = 5;
        SearchMethod = 1;
    }

    internal static PlayerUiSettings Load()
    {
        if (File.Exists(GamesController.SavePath + "PlayerUiSettings.cfg"))
            return (PlayerUiSettings) Serialiser.DeSerializeObject(GamesController.SavePath + "PlayerUiSettings.cfg");
        return new PlayerUiSettings();
    }

    internal void Save()
    {
        Serialiser.SerializeObject(GamesController.SavePath + "PlayerUiSettings.cfg", this);
    }

    
}

