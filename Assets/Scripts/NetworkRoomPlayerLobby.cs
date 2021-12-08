using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Mirror;

public class NetworkRoomPlayerLobby : NetworkBehaviour
{
    #region UI Variables

    [Header("UI")]
    [SerializeField] private GameObject lobbyUI = null;
    [SerializeField] private GameObject playerButtons = null;
    [SerializeField] private TMP_Text[] playerNameTexts = new TMP_Text[4];
    [SerializeField] private Image[] playerReadyImage = new Image[4];
    [SerializeField] private Button startGameButton = null;
    [SerializeField] private Button settingsButton = null;
    [SerializeField] private Button inviteButton = null;
    [SerializeField] private Button exitButton = null;
    public Sprite[] readyImageSprites = new Sprite[4];

    #endregion

    #region UI Variables
    [Header("Customization")]
    [SerializeField] private CharacterLookScript[] characters = new CharacterLookScript[4];
    public int playerNum;
    private const int maxSkinNum = 8;
    private const int maxHatNum = 11;

    //can only be updated on the server
    //when variables change these functions are called
    [SyncVar(hook = nameof(HandleDisplayNameChanged))]
    public string DisplayName = "Loading...";
    [SyncVar(hook = nameof(HandleReadyStatusChanged))]
    public bool IsReady = false;
    [SyncVar(hook = nameof(HandleSkinStatusChanged))]
    public int skinNum = 0;
    [SyncVar(hook = nameof(HandleHatStatusChanged))]
    public int hatNum = 0;

    #endregion

    //bool to store if this client is also the host
    public bool isLeader;
    public bool IsLeader
    {
        set
        {
            //sets up UI to only be enabled for the leader
            isLeader = value;
            startGameButton.gameObject.SetActive(value);
            settingsButton.gameObject.SetActive(value);
            inviteButton.gameObject.SetActive(value);
            exitButton.GetComponent<DisconnectFromGameButton>().isLeader = value;
        }
    }

    //the network manager override script 
    private NetworkManagerOverride room;
    private NetworkManagerOverride Room
    {
        get
        {
            //if not assigned then set room to the Network manager override
            if (room != null)
            {
                return room;
            }
            return room = NetworkManager.singleton as NetworkManagerOverride;
        }
    }

    //called on start only on objects this client has authority over
    public override void OnStartAuthority()
    {
        //update the display name to be that of the input box
        CmdSetDisplayName(PlayerNameInput.DisplayName);
        //turn on the lobby UI
        lobbyUI.SetActive(true);

        //gets the player number by finding how many other players are currently in the lobby
        playerNum = Room.RoomPlayers.Count;
        //move player buttons to align with the character
        PositionPlayerButtons();
        //loop through all characters and call playerStart which sets up the character for customization
        foreach(CharacterLookScript cls in characters)
        {
            cls.playerStart();
        }
    }

    //move player buttons to align with character
    private void PositionPlayerButtons()
    {
        //position player buttons
        playerButtons.GetComponent<RectTransform>().anchoredPosition = new Vector2((180 * playerNum) - 270f, -125f);       
        //set buttons to be active
        playerButtons.SetActive(true);
        //set background to plain image
        playerReadyImage[playerNum].sprite = readyImageSprites[3];
    }

    //Called at start on client
    public override void OnStartClient()
    {
        //add to the list of room players - the list of all clients in the lobby
        Room.RoomPlayers.Add(this);
        //call update display to show new client
        UpdateDisplay();
    }

    //Called when client stops
    public override void OnStopClient()
    {
        //removes from the list of room players - the list of all clients in the lobby
        Room.RoomPlayers.Remove(this);
        //call update display to stop showing character
        UpdateDisplay();
    }


    // whenever one of these values change the UpdateDisplay() function is called
    public void HandleDisplayNameChanged(string oldValue, string newValue) => UpdateDisplay();
    public void HandleSkinStatusChanged(int oldValue, int newValue) => UpdateDisplay();
    public void HandleHatStatusChanged(int oldValue, int newValue) => UpdateDisplay();

    //calls UpdateDisplayName so the name is updated when the player readies - this in turn causes the update display to be called
    public void HandleReadyStatusChanged(bool oldValue, bool newValue) => UpdateDisplayName();

    //checks the room is active and then calls the command to change display name
    private void UpdateDisplayName()
    {       
        if (Room.isNetworkActive)
        {
            CmdSetDisplayName(PlayerNameInput.DisplayName);
        }
    }


    //update display refreshes all the visuals for each character
    private void UpdateDisplay()
    {
        //checks if the client has authority to run this code
        if (!hasAuthority)
        {
            //hides UI for other clients so players can only see their own UI
            lobbyUI.SetActive(false);


            //updates the player number incase some players have left since last update
            int p = 0;
            foreach (var player in Room.RoomPlayers)
            {
                if (player.hasAuthority)
                {
                    player.playerNum = p;
                    player.UpdateDisplay();
                    break;
                }
                p++;
            }

            return;      
        }

        //sets all characters to blank - this resets all the characters so they are ready to update
        for (int i = 0; i < playerNameTexts.Length; i++)
        {
            playerNameTexts[i].text = "";
            playerReadyImage[i].sprite = readyImageSprites[0];
        }

        //updates the display for the clients in the lobby
        for (int i = 0; i < Room.RoomPlayers.Count; i++)
        {
            //updates the text UI for the player name
            playerNameTexts[i].text = Room.RoomPlayers[i].DisplayName;
            //upates weather or not the player is ready
            if (Room.RoomPlayers[i].IsReady)
            {
                playerReadyImage[i].sprite = readyImageSprites[2];
            }
            else
            {
                playerReadyImage[i].sprite = readyImageSprites[1];
            }

        }

        //loop through all character models and if there is a client for each model it is set visible, otherwise it is hidden
        for (int i = 0; i < 4; i++)
        {
            if (i < Room.RoomPlayers.Count)
            {
                characters[i].setVisable(true);
                //updates character skin and hat
                characters[i].changeSkin(Room.RoomPlayers[i].skinNum);
                characters[i].changeHat(Room.RoomPlayers[i].hatNum);
            }
            else
            {
                characters[i].setVisable(false);
            }
        }
        //repositions player buttons incase a client has disconnected and so characters have to move
        PositionPlayerButtons();
    }

    //sets start button active for host when all players are ready
    public void HandleReadyToStart(bool readyToStart)
    {
        if (!isLeader)
        {
            return;
        }

        startGameButton.interactable = readyToStart;
    }

    #region Commands
    //commands can be called by clients but are only ran on the server

    [Command] //update player display name
    private void CmdSetDisplayName(string displayName)
    {
            DisplayName = displayName;
    }

    [Command] //update player num
    private void CmdSetPlayerNum(int pNum)
    {
        playerNum = pNum;
    }

    [Command] //update player skin num
    public void CmdIncSkinNum()
    {
        skinNum = (skinNum+1) % maxSkinNum;
    }

    [Command] //update player hat num
    public void CmdIncHatNum()
    {
        hatNum = (hatNum + 1) % maxHatNum;
    }

    [Command] //update player ready
    public void CmdReadyUp()
    {
        IsReady = !IsReady;

        Room.NotifyPlayersOfReadyState();
    }

    [Command] //Starts the actual game
    public void CmdStartGame()
    {
        //check that the player is the host
        if (Room.RoomPlayers[0].connectionToClient != connectionToClient)
        {
            return;
        }

        //start game
        Room.StartGame();
    }

    [Command] //change player skin
    public void CmdChangeSkin()
    {
        Room.ChangeSkin(playerNum);
    }
    #endregion

}
