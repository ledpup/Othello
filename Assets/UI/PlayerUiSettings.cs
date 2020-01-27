using System;
using System.IO;
using Othello.Model;
using Othello.Core;
using UnityEngine;

[Serializable]
public class PlayerUiSettings : IPlayerUiSettings
{
    public bool BlackIsHuman { get; set; }
    public bool WhiteIsHuman { get; set; }
    public bool ShowValidPlays { get; set; }
    public bool ShowBoardCoordinates { get; set; }
    public bool ShowArchiveStats { get; set; }
    public bool UseTranspositionTable { get; set; }
    public bool UseOpeningBook { get; set; }
    public int SearchMethod { get; set; }
    public int MaxSearchDepth { get; set; }
    public int MaxSearchTime { get; set; }

    public PlayerUiSettings()
    {
        BlackIsHuman = true;
        WhiteIsHuman = false;
        ShowBoardCoordinates = true;
        ShowValidPlays = true;
        ShowArchiveStats = true;
        UseTranspositionTable = true;
        UseOpeningBook = true;
        MaxSearchDepth = 5;
        SearchMethod = 1;
        MaxSearchTime = 300000;
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

