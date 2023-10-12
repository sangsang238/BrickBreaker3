using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelSelectionMenu : MonoBehaviour
{
    public List<Button> levelButtons;
    public Button dialog;
    //public List<TextMeshProUGUI> levelAppreciateTexts;

    private void Start()
    {
        //dialog.gameObject.SetActive(false);
        if (GameManager.mdplayer != null)
        {
            UpdateLevelSelectionMenu();
        }
    }

    public void Refresh()
    {
        MongoDBManager manager = new MongoDBManager();
        //RealmConnectManager manager = new RealmConnectManager();
        if (manager != null && GameManager.mdplayer != null)
        { 
            GameManager.mdplayer = manager.Login(GameManager.mdplayer.Username, GameManager.mdplayer.Password);
            UpdateLevelSelectionMenu();
            dialog.gameObject.SetActive(true);
        }
        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void UpdateLevelSelectionMenu()
    {
        MongoDBManager manager = new MongoDBManager();
        //RealmConnectManager manager = new RealmConnectManager();
        if (manager == null)
        {
            Debug.LogError("MongoDBManager not found in the scene.");
            return;
        }

        for (int level = 1; level <= levelButtons.Count; level++)
        {
            int appreciateValue = GetLevelAppreciateValue(level);
            //UpdateLevelAppreciateText(level - 1, level, appreciateValue);

            // Show stars only for levels with appreciateValue >= 1
            if (appreciateValue >= 1)
            {
                string starsInLevelName = $"StarsInLevel";
                GameObject starsInLevel = levelButtons[level - 1].transform.Find(starsInLevelName).gameObject;
                starsInLevel.SetActive(true);

                // Activate/Deactivate stars based on the appreciateValue
                for (int starIndex = 1; starIndex <= 3; starIndex++)
                {
                    string starName = $"star{starIndex}";
                    GameObject star = starsInLevel.transform.Find(starName).gameObject;
                    star.SetActive(starIndex <= appreciateValue);

                    // Debug statement
                    //Debug.Log($"Level {level}, Star {starName}, Active: {star.activeSelf}");
                }
            }
        }
    }

    private int GetLevelAppreciateValue(int levelNumber)
    {
        if (GameManager.mdplayer == null)
            return 0;

        foreach (var levelData in GameManager.mdplayer.Levels)
        {
            if (levelData.LevelNumber == levelNumber)
                return levelData.Appreciate;
        }

        return 0;
    }

    //private void UpdateLevelAppreciateText(int index, int levelNumber, int appreciateValue)
    //{
    //    levelAppreciateTexts[index].text = $"Level {levelNumber}: {appreciateValue} stars";
    //    levelAppreciateTexts[index].gameObject.SetActive(true);
    //}

    // Clicking on a level button
    public void OnButtonClick(int level)
    {
        GameManager.currentLevel = level;
        PauseMenu.currentLevel = level;
        SceneManager.LoadScene("GamePlayScene1");
    }
}