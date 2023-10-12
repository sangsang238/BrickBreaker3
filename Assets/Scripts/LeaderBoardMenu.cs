using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LeaderBoardMenu : MonoBehaviour
{
    //RealmConnectManager manager = new RealmConnectManager();
    //public static Player mdplayer = null;


    MongoDBManager manager = new MongoDBManager();
    public static ModelPlayer mdplayer = null;

    public TextMeshProUGUI textMeshPro;
    public TextMeshProUGUI txtScore;
    

    // Start is called before the first frame update
    void Start()
    {
        if (manager == null)
        {
            Debug.LogError("MongoDBManager not found in the scene.");
            return;
        }
        else if (mdplayer!=null)
        {
            int highestScore = manager.GetHighestScoreForPlayer(mdplayer.Username);
            //Debug.Log($"{mdplayer.PlayerName}'s highest score: {highestScore}");
            txtScore.text = $"{mdplayer.PlayerName}'s highest score: {highestScore}";
        }
        if (manager != null)
            textMeshPro.text = GetPlayerNamesAndHighScoresAsString();
    }

    public string GetPlayerNamesAndHighScoresAsString()
    {
        Dictionary<string, int> playerScores = manager.GetPlayerNamesAndHighScores();

        if (playerScores.Count == 0)
        {
            return "No players found.";
        }

        string result = "";
        int i = 1;
        foreach (var kvp in playerScores)
        {
            result += i.ToString()+".";
            result += $"{kvp.Key} «{kvp.Value}»\n\n\n";
            //result += "\n________________________\n\n";
            i++;
        }

        return result;
    }

}
