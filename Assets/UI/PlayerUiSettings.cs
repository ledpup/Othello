using System;
using System.IO;
using Othello.Model;
using UnityEngine;

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
        if (File.Exists(Application.persistentDataPath + "/PlayerUiSettings.cfg"))
            return (PlayerUiSettings) Serialiser.DeSerializeObject(Application.persistentDataPath + "/PlayerUiSettings.cfg");
        return new PlayerUiSettings();
    }

    internal void Save()
    {
        Serialiser.SerializeObject(Application.persistentDataPath + "/PlayerUiSettings.cfg", this);
    }
}

