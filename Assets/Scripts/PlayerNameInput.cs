using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerNameInput : MonoBehaviour
{

    //UI elements 
    [SerializeField] private TMP_InputField nameInput = null;
    [SerializeField] private Button continueButton = null;

    //players name can only be set internally
    public static string DisplayName { get; private set; }

    //used to load from playerPrefs
    private const string PlayerPrefsKey = "PlayerName";

    

    void Start()
    {
        SetUpInputField();
    }


    private void SetUpInputField()
    {
        //if there is no name stored then return
        if (!PlayerPrefs.HasKey(PlayerPrefsKey))
        {
            return;
        }

        //otherwise get the player name from player prefs and display it
        string defaultName = PlayerPrefs.GetString(PlayerPrefsKey);

        nameInput.text = defaultName;

        SetPlayerName(defaultName);
        SavePlayerName();
    }

    //if name is not null or empty the continue button becomes active

    public void UpdatePlayerName()
    {
        SetPlayerName(nameInput.text);
    }

    public void SetPlayerName(string name)
    {
        continueButton.interactable = !string.IsNullOrEmpty(name);
        SavePlayerName();
    }

    //stores the name in the player prefs
    public void SavePlayerName()
    {
        DisplayName = nameInput.text;

        PlayerPrefs.SetString(PlayerPrefsKey, DisplayName);
    }
}
