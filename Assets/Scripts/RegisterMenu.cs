using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Text.RegularExpressions;

public class RegisterMenu : MonoBehaviour
{
    [SerializeField] private TMP_InputField usernameInputField;
    [SerializeField] private TMP_InputField passwordInputField;
    [SerializeField] private TMP_InputField playerNameInputField;
    [SerializeField] private TextMeshProUGUI noti;

    //private MongoDBManager dbManager;

    private MongoDBManager dbManager = new MongoDBManager();


    private void Start()
    {
        dbManager = new MongoDBManager();

        // Load error messages from CSV file
        CSVLoader.LoadErrorMessages("ErrorData.csv");
    }

    private void Update()
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

    //public void OnClick()
    //{
    //    string username = usernameInputField.text;
    //    string password = passwordInputField.text;
    //    string playerName = playerNameInputField.text;

    //    if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(playerName))
    //        noti.SetText("Both Username, Password and Player name\ncannot be empty!");
    //    else
    //        noti.SetText(dbManager.Register(username, password, playerName));
    //}
    public void OnClick()
    {
        string username = RemoveSpaces(usernameInputField.text.ToString());
        string password = passwordInputField.text;
        string playerName = RemoveSpaces(playerNameInputField.text);

        //if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(playerName))
        //{
        //    noti.SetText("Both Username, Password and Player name\ncannot be empty!");
        //}

        if (string.IsNullOrEmpty(username))
        {
            noti.text = CSVLoader.GetErrorMessage("VALIDATION_USERNAME_E001");
        }
        else if (string.IsNullOrEmpty(password))
        {
            noti.text = CSVLoader.GetErrorMessage("VALIDATION_PASSWORD_E001");
        }
        else if (string.IsNullOrEmpty(playerName))
        {
            noti.text = CSVLoader.GetErrorMessage("VALIDATION_PLAYERNAME_E001");
        }


        else if (username.Length > 15)
        {
            //noti.SetText("USER NAME\nmust not be longer than 15 characters!");
            noti.text = CSVLoader.GetErrorMessage("VALIDATION_USERNAME_E002");
        }
        else if (password.Length < 6)
        {
            //noti.SetText("USER NAME\nmust not be longer than 15 characters!");
            noti.text = CSVLoader.GetErrorMessage("VALIDATION_PASSWORD_E002");
        }
        else if (playerName.Length > 15)
        {
            //noti.SetText("PLAYER NAME\nmust not be longer than 15 characters!");
            noti.text = CSVLoader.GetErrorMessage("VALIDATION_PLAYERNAME_E002");
        }
        else
        {
            //noti.SetText(dbManager.Register(username, password, playerName));
            noti.text = CSVLoader.GetErrorMessage(dbManager.Register(username, password, playerName));
        }
    }

    private string RemoveSpaces(string input)
    {
        // Use regex to remove spaces from the input
        return Regex.Replace(input, @"\s", "");
    }
}
