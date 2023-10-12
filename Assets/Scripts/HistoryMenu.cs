using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HistoryMenu : MonoBehaviour
{
    MongoDBManager manager = new MongoDBManager();
    public static ModelPlayer mdplayer = null;

    public TextMeshProUGUI textMeshPro;


    // Start is called before the first frame update
    void Start()
    {
        if (manager == null)
        {
            Debug.LogError("MongoDBManager not found in the scene.");
            return;
        }
        else if (mdplayer != null)
        {
            //int highestScore = manager.GetHighestScoreForPlayer(mdplayer.Username);
            ////Debug.Log($"{mdplayer.PlayerName}'s highest score: {highestScore}");
            //txtScore.text = $"{mdplayer.PlayerName}'s highest score: {highestScore}";


            List<int> allScores = manager.GetAllScoresForLoggedInPlayer(mdplayer.Username);
            string result = "";
            int i = 1;
            foreach (int score in allScores)
            {
                result += i.ToString()+$".";
                result += $" «{score}»\n\n\n";
                //result += "\n________________________\n\n";
                i++;
            }


            textMeshPro.text = result;

        }
        //if (manager != null)
        //    textMeshPro.text = GetPlayerNamesAndHighScoresAsString();
    }


    //public string GetPlayerNamesAndHighScoresAsString()
    //{
    //    Dictionary<string, int> playerScores = manager.GetPlayerNamesAndHighScores();
    //    List<int> allScores = manager.GetAllScoresForLoggedInPlayer(loggedInUsername);

    //    if (playerScores.Count == 0)
    //    {
    //        return "No players found.";
    //    }

    //    string result = "";
    //    int i = 1;
    //    foreach (var kvp in playerScores)
    //    {
    //        result += i.ToString() + ".";
    //        result += $"{kvp.Key} {kvp.Value}";
    //        result += "\n________________________\n\n";
    //        i++;
    //    }


    //    // Thực hiện lấy toàn bộ điểm số của người chơi đang đăng nhập
    //    List<int> allScores = GetAllScoresForLoggedInPlayer(loggedInUsername);
    //    Console.WriteLine($"All Scores for {loggedInUsername}:");
    //    foreach (int score in allScores)
    //    {
    //        Console.WriteLine(score);
    //    }




    //    return result;
    //}
}
