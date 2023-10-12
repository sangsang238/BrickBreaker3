using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Report_Personal : MonoBehaviour
{
    MongoDBManager manager = new MongoDBManager();
    public static ModelPlayer mdplayer = null;
    public TextMeshProUGUI txtname;
    public TextMeshProUGUI txtHighestScore;
    public TextMeshProUGUI txtTotalStars;
    public TextMeshProUGUI txtGames;
    public TextMeshProUGUI txtTime;
    // Start is called before the first frame update
    void Start()
    {
        if (mdplayer != null)
        {  
            int highestScore = manager.GetHighestScoreForPlayer(mdplayer.Username);
            int totalStars = manager.GetTotalStarsForPlayer(mdplayer.Username);
            int games = manager.GetPlaySession(mdplayer.Username);
            int time = manager.GetPlayTime(mdplayer.Username);
            string name = mdplayer.PlayerName.ToString();

            txtname.text = $"Player Name: {name}";
            txtHighestScore.text = $"Your highest score: {highestScore}";
            txtTotalStars.text = $"Your total stars: {totalStars}";
            txtGames.text = $"Your games: {games}";
            txtTime.text = $"Your time spent playing: {Report_List.FormatPlayTimeForCSV(time)}";

        }
    }
}
