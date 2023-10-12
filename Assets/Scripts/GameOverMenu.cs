using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.SocialPlatforms.Impl;
using System;

public class GameOverMenu : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    int currentLevel = PauseMenu.currentLevel;
    private MongoDBManager manager = new MongoDBManager();
    public static ModelPlayer mdplayer = null;

    //private RealmConnectManager manager = new RealmConnectManager();
    //public static Player mdplayer = null;

    public void Start()
    {
        UpdateScoreText();
        savedata();
        savePlayTime();
    }

    // Use this for initialization
    public void RestartGame()
    {
        GameManager.currentLevel = currentLevel;
        SceneManager.LoadScene("GamePlayScene1");
        Time.timeScale = 1f;
    }
    public void BackToMenu()
    {
        PauseMenu.currentLevel = 0;
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenuScene1");
    }

    public void Quit()
    {
        #if UNITY_STANDALONE
                Application.Quit();
        #endif
        #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
    private void UpdateScoreText()
    {
        scoreText.text = GameManager.score.ToString();
    }
    private void savedata()
    {
        if (mdplayer != null && currentLevel == 0 && MainMenu.saveScore == true)
        {
            manager.SaveScoreInPlayMode(mdplayer.Username, GameManager.score);
            Debug.Log(mdplayer.PlayerName + "-" + currentLevel + "-" + MainMenu.saveScore+"-"+ GameManager.score.ToString());
        }
        //Debug.Log(mdplayer.PlayerName + "\n" + currentLevel + "\n" + MainMenu.saveScore);
    }
    private void savePlayTime()
    {
        if (mdplayer != null)
        {
            //report time
            TimeSpan sessionDuration = DateTime.Now - GameManager.sessionStartTime;
            int sessionDurationInSeconds = (int)sessionDuration.TotalSeconds;
            int currTimePlay = manager.GetPlayTime(mdplayer.Username);
            Debug.Log("\nCurrTime: " + currTimePlay);
            manager.SavePlayTime(mdplayer.Username, currTimePlay + sessionDurationInSeconds);
            Debug.Log("\nUpdatedTime: " + sessionDurationInSeconds.ToString());
        }
    }
}

