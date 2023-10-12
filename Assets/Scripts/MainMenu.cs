using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{

    [SerializeField] private Canvas loginMenu;
    [SerializeField] private TextMeshProUGUI playerNameText;
    [SerializeField] private Button btnLogOut;
    [SerializeField] private Button btnHistory;

    //public static ModelPlayer mdplayer = null;
    MongoDBManager manager = new MongoDBManager();
    public static ModelPlayer mdplayer = null;
    public static bool isLoggedIn = false;
    public static bool saveScore = false;
    string playerName = "";

    private void Start()
    {
        playerName = PlayerPrefs.GetString("PlayerName", "Guest");
        UpdatePlayerName(playerName);
    }


    // Call this method when the player's name needs to be updated on the main menu
    public void UpdatePlayerName(string name)
    {
        if (playerNameText != null)
        {
            if (isLoggedIn)
            {
                playerNameText.text = "Player Name: " + name;
                PlayerPrefs.SetString("PlayerName", name); // Save the player's name to PlayerPrefs
                btnLogOut.interactable = true;
                btnHistory.interactable = true;
            }
            else
            {
                playerNameText.text = "Guest";
                PlayerPrefs.SetString("PlayerName", "Guest");
                btnLogOut.interactable = false;
                btnHistory.interactable = false;
            }
            
        }
    }

    public void Play()
    {
        
        if (mdplayer == null)
        {
            loginMenu.gameObject.SetActive(true);
        }
        else
        {
            //// Get curr PlaySession and increase by 1
            //int a = manager.GetPlaySession(mdplayer.Username);
            //manager.SavePlaySession(mdplayer.Username, a+1);

            saveScore = true;
            SceneManager.LoadScene("GamePlayScene1");
            // fixed
            GameManager.currentLevel = 0;
        }

    }



    public void PlayAsGuest()
    {
        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        saveScore = false;
        SceneManager.LoadScene("GamePlayScene1");
        // fixed
        GameManager.currentLevel = 0;
    }

    public void Quit()
    {
        //#if UNITY_STANDALONE
        //                Application.Quit();
        //#endif
        //#if UNITY_EDITOR
        //                UnityEditor.EditorApplication.isPlaying = false;
        //#endif

        
        GameManager.mdplayer = null;
        mdplayer = null;
        OptionsMenu.mdplayer = null;
        GameOverMenu.mdplayer = null;
        LeaderBoardMenu.mdplayer = null;
        LeaderBoardStars.mdplayer = null;
        HistoryMenu.mdplayer = null;
        GameWinMenu.mdplayer = null;
        Report_Personal.mdplayer = null;
        isLoggedIn = false; // opt playerName display in main page
        UpdatePlayerName(playerName);
        SceneManager.LoadScene("MainMenuScene1"); // update list levels appreciated
    }
    public void Trophycup()
    {
        UnityEngine.Debug.Log("Trophycup button clicked!");
    }
}
