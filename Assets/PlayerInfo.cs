using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlayerInfo
{
    public string name;
    public int score;
    public List<string> activeGames;
}

[Serializable]
public class GameData
{
    public string displayName;
    public string gameID;
    public int numberOfPlayers = 2;
    public int seed;
    public List<PlayerInfo> players;
}

[Serializable]
public class PlayerGameInfo
{
    public string displayName;
    public Vector3 position;
    public bool hidden;
}