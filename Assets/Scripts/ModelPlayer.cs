using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MongoDB.Bson;
using MongoDB.Driver;
using System;

public class ModelPlayer
{
    public string Username { get; set; }
    public string Password { get; set; }
    public string PlayerName { get; set; }
    public List<int> Scores { get; set; }
    public List<LevelData> Levels { get; set; }
    public int Color { get; set; }
    public int playSession { get; set; }
    public int playTime { get; set; }
    public List<String> playMoments { get; set; }

    public class LevelData
    {
        public int LevelNumber { get; set; }
        public int Appreciate { get; set; }
    }
}