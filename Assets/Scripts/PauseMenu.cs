using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenu;
    public GameObject btnPause;
    //public GameObject btnPassThisLevel;
    public bool isPaused;
    public static int currentLevel = 0;
    //void Awake()
    //{
    //    DontDestroyOnLoad(this.gameObject);
    //}
    public void Play()
    {
        pauseMenu.SetActive(false);
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                ResumeGame();
            else
                PauseGame();
        }    
    }


    //public void PassThisLevel()
    //{
        
    //}


    public void PauseGame()
    {
        pauseMenu.SetActive(true);
        btnPause.SetActive(false);
        Time.timeScale = 0f;
        isPaused = true;
    }
    public void ResumeGame()
    {
        pauseMenu.SetActive(false);
        btnPause.SetActive(true);
        Time.timeScale = 1f;
        isPaused = false;
    }
    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        GameManager.currentLevel = currentLevel;
        Time.timeScale = 1f;
        //SceneManager.LoadScene("GamePlayScene1");
    }

    public void BackToMenu()
    {
        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        currentLevel = 0;
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenuScene1");
    }

    public void Quit()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("GameOverScene1");
    }

}
