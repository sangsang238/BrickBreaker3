using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;
using System;

public class GameWinMenu : MonoBehaviour
{
    public SpriteRenderer star1;
    public SpriteRenderer star2;
    public SpriteRenderer star3;

    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI txtLevel;
    int currentLevel = PauseMenu.currentLevel;
    int stars=1;
    MongoDBManager manager = new MongoDBManager();
    public static ModelPlayer mdplayer = null;


    //RealmConnectManager manager = new RealmConnectManager();
    //public static Player mdplayer = null;

    public void Start()
    {
        star1.enabled = false;
        star2.enabled = false;
        star3.enabled = false;
        txtLevel.SetText("Level " + currentLevel);
        UpdateScoreText();
        UpdateAppreciate();
        
        if (mdplayer != null)
        {
            Debug.Log(mdplayer.Username + "-"+ currentLevel +"-"+ stars);
            manager.RecordPlayerRating(mdplayer.Username, currentLevel, stars);
        }
        savePlayTime();
    }

    public void RestartGame()
    {
        GameManager.currentLevel = currentLevel;
        SceneManager.LoadScene("GamePlayScene1");
        Time.timeScale = 1f;
    }
    public void NextLevel()
    {
        PauseMenu.currentLevel++;
        GameManager.currentLevel = PauseMenu.currentLevel;
        SceneManager.LoadScene("GamePlayScene1");
        Time.timeScale = 1f;
    }
    public void BackToMenu()
    {
        PauseMenu.currentLevel = 0;
        //GameManager.currentLevel = PauseMenu.currentLevel;
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
    private void UpdateAppreciate()
    {
        // 3 first levels
        if (currentLevel>0 && currentLevel <= 3) {
            if (GameManager.score > 0 && GameManager.score <= 600)
                stars = 3;
            else if (GameManager.score>600 && GameManager.score <= 1000)
                stars = 2;
            else
                stars = 1;
        }

        // 3 second levels
        if (currentLevel >= 4 && currentLevel<=6)
        {
            if (GameManager.score > 0 && GameManager.score <= 1500)
                stars = 3;
            else if (GameManager.score > 1500 && GameManager.score <= 2000)
                stars = 2;
            else
                stars = 1;
        }

        // 3 third levels
        if (currentLevel >= 7 && currentLevel<= 12)
        {
            if (GameManager.score > 0 && GameManager.score <= 2500)
                stars = 3;
            else if (GameManager.score > 2500 && GameManager.score <= 3000)
                stars = 2;
            else
                stars = 1;
        }

        //Stars Appearance
        if (stars == 3)
        {
            star1.enabled = true;
            star2.enabled = true;
            star3.enabled = true;
        }
        else if (stars == 2)
        {
            star1.enabled = true;
            star2.enabled = true;
        }
        else
        {
            star1.enabled = true;
        }
    }

    private void savePlayTime()
    {
        if (GameManager.mdplayer != null)
        {
            //report time
            TimeSpan sessionDuration = DateTime.Now - GameManager.sessionStartTime;
            int sessionDurationInSeconds = (int)sessionDuration.TotalSeconds;
            //Debug.Log("\nTime: " + sessionDurationInSeconds.ToString());
            int a = manager.GetPlayTime(GameManager.mdplayer.Username);
            manager.SavePlayTime(GameManager.mdplayer.Username, a + sessionDurationInSeconds);
            Debug.Log("\nCurrTime: " + a.ToString());
        }
    }

}
