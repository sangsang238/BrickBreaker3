using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class LoginMenu : MonoBehaviour
{

    MongoDBManager dbManager = new MongoDBManager();
    //public static ModelPlayer mdplayer = null;
    
    [SerializeField] private TMP_InputField usernameInputField;
    [SerializeField] private TMP_InputField passwordInputField;
    [SerializeField] private TextMeshProUGUI noti;

    //private MongoDBManager dbManager;
    private MainMenu mainMenu;

    public Button dialog;



    private void Start()
    {
        dbManager = new MongoDBManager();
        mainMenu = GameObject.FindObjectOfType<MainMenu>();

        // Load error messages from CSV file
        CSVLoader.LoadErrorMessages("ErrorData.csv");
    }

    void Update()
    {
        // Check for Tab key press
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            Selectable next = null;
            Selectable current = EventSystem.current.currentSelectedGameObject?.GetComponent<Selectable>();

            if (current != null)
            {
                next = current.FindSelectableOnDown();
                if (next == null)
                {
                    next = current.navigation.selectOnDown;
                }
            }

            if (next != null)
            {
                next.Select();
            }
        }

        // Check for Enter key press
        if (Input.GetKeyDown(KeyCode.Return))
        {
            OnClick();
        }
    }

    public void OnClick()
    {
        string username = usernameInputField.text;
        string password = passwordInputField.text;

        if (string.IsNullOrEmpty(username))
        {
            //noti.SetText("Username cannot be empty!");
            noti.text = CSVLoader.GetErrorMessage("VALIDATION_USERNAME_E001");
        }
        else if (string.IsNullOrEmpty(password))
        {
            //noti.SetText("Password cannot be empty!");
            noti.text = CSVLoader.GetErrorMessage("VALIDATION_PASSWORD_E001");
        }
        else
        {
            ModelPlayer a = dbManager.Login(username, password);
            if (a == null)
            {
                //noti.SetText("Sorry, your email or password is incorrect.\nPlease try again!");
                noti.text = CSVLoader.GetErrorMessage("LOGIN_E001");
            }
                
            else
            {
                //noti.color = new Color(80, 200, 120);
                //noti.SetText(a.PlayerName + " login success!");
                //noti.text = CSVLoader.GetErrorMessage("LOGIN_SUCCESS");

                dialog.gameObject.SetActive(true);

                GameManager.mdplayer = a;
                MainMenu.mdplayer = a;
                OptionsMenu.mdplayer = a;
                GameOverMenu.mdplayer = a;
                LeaderBoardMenu.mdplayer = a;
                LeaderBoardStars.mdplayer = a;
                HistoryMenu.mdplayer = a;
                GameWinMenu.mdplayer = a;
                Report_Personal.mdplayer = a;
                MainMenu.isLoggedIn = true;
                PlayerPrefs.SetString("PlayerName", a.PlayerName);

                //SceneManager.LoadScene("MainMenuScene1");
                StartCoroutine(DelayedSceneTransition(1.5f, "MainMenuScene1"));
            }
        }
    }
    IEnumerator DelayedSceneTransition(float delay, string sceneName)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(sceneName);
    }
}
