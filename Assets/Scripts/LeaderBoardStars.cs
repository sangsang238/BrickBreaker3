using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LeaderBoardStars : MonoBehaviour
{
    MongoDBManager manager = new MongoDBManager();
    public static ModelPlayer mdplayer = null;

    public TextMeshProUGUI textMeshPro;
    public TextMeshProUGUI txtTotalStars;

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
            int totalStars = manager.GetTotalStarsForPlayer(mdplayer.Username);
            //Debug.Log($"{mdplayer.PlayerName}'s highest score: {totalStars}");
            txtTotalStars.text = $"{mdplayer.PlayerName}'s total stars: {totalStars}";
        }
        if (manager != null)
            textMeshPro.text = GetPlayerNamesAndTotalStarsAsString();
    }

    public string GetPlayerNamesAndTotalStarsAsString()
    {
        Dictionary<string, int> playerStars = manager.GetPlayerNamesAndTotalAppreciate();

        if (playerStars.Count == 0)
        {
            return "No players found.";
        }

        string result = "";
        int i = 1;
        foreach (var kvp in playerStars)
        {
            result += i.ToString() + ".";
            result += $"{kvp.Key} «{kvp.Value}»\n\n\n";
            //result += "\n________________________\n\n";
            i++;
        }

        return result;
    }
}
