using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Mirror;

public class NetworkRoomPlayerLobby : NetworkBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject lobbyUI = null;
    [SerializeField] private GameObject playerButtons = null;
    [SerializeField] private TMP_Text[] playerNameTexts = new TMP_Text[4];
    [SerializeField] private Image[] playerReadyImage = new Image[4];
    [SerializeField] private Button startGameButton = null;
    [SerializeField] private Button settingsButton = null;

    public Sprite[] readyImageSprites = new Sprite[4];

    //can only be updated on the server
    //when variables change these functions are called
    [SyncVar(hook = nameof(HandleDisplayNameChanged))]
    public string DisplayName = "Loading...";
    [SyncVar(hook = nameof(HandleReadyStatusChanged))]
    public bool IsReady = false;

    [Header("Customization")]
    [SerializeField] private CharacterLookScript[] characters = new CharacterLookScript[4];
    public int playerNum;
    [SyncVar(hook = nameof(HandleSkinStatusChanged))]
    public int skinNum = 0;
    [SyncVar(hook = nameof(HandleHatStatusChanged))]
    public int hatNum = 0;

    public bool isLeader;

    private const int maxSkinNum = 8;
    private const int maxHatNum = 11;
    public bool IsLeader
    {
        set
        {
            isLeader = value;
            startGameButton.gameObject.SetActive(value);
            settingsButton.gameObject.SetActive(value);
        }
    }

    private NetworkManagerOverride room;
    private NetworkManagerOverride Room
    {
        get
        {
            if (room != null)
            {
                return room;
            }
            return room = NetworkManager.singleton as NetworkManagerOverride;
        }
    }

    public override void OnStartAuthority()
    {
        CmdSetDisplayName(PlayerNameInput.DisplayName);

        lobbyUI.SetActive(true);
        //CmdSetPlayerNum(Room.RoomPlayers.Count);

        playerNum = Room.RoomPlayers.Count;

        PositionPlayerButtons();

        foreach(CharacterLookScript cls in characters)
        {
            cls.playerStart();
        }
    }

    private void PositionPlayerButtons()
    {
        playerButtons.GetComponent<RectTransform>().anchoredPosition = new Vector2((180 * playerNum) - 270f, -125f);       
        playerButtons.SetActive(true);

        playerReadyImage[playerNum].sprite = readyImageSprites[3];
    }

    public override void OnStartClient()
    {
        Room.RoomPlayers.Add(this);

        UpdateDisplay();
    }

    public override void OnStopClient()
    {
        Room.RoomPlayers.Remove(this);

        UpdateDisplay();
    }


    // whenever one of these values change the UpdateDisplay() function is called
    public void HandleDisplayNameChanged(string oldValue, string newValue) => UpdateDisplay();
    public void HandleReadyStatusChanged(bool oldValue, bool newValue) => UpdateDisplay();
    public void HandleSkinStatusChanged(int oldValue, int newValue) => UpdateDisplay();
    public void HandleHatStatusChanged(int oldValue, int newValue) => UpdateDisplay();


    private void UpdateDisplay()
    {

        CmdSetDisplayName(PlayerNameInput.DisplayName);

        if (!hasAuthority)
        {
            lobbyUI.SetActive(false);

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

        for (int i = 0; i < playerNameTexts.Length; i++)
        {
            playerNameTexts[i].text = "";
            playerReadyImage[i].sprite = readyImageSprites[0];
        }

        
        for (int i = 0; i < Room.RoomPlayers.Count; i++)
        {
            playerNameTexts[i].text = Room.RoomPlayers[i].DisplayName;
            if (Room.RoomPlayers[i].IsReady)
            {
                playerReadyImage[i].sprite = readyImageSprites[2];

            }
            else
            {
                playerReadyImage[i].sprite = readyImageSprites[1];

            }

        }

        for (int i = 0; i < 4; i++)
        {
            if (i < Room.RoomPlayers.Count)
            {
                characters[i].setVisable(true);
                characters[i].changeSkin(Room.RoomPlayers[i].skinNum);
                characters[i].changeHat(Room.RoomPlayers[i].hatNum);
            }
            else
            {
                characters[i].setVisable(false);
            }
        }

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

    //commands can be called by clients but are only ran on the server

    [Command]

    private void CmdSetDisplayName(string displayName)
    {
        DisplayName = displayName;
    }

    [Command]
    private void CmdSetPlayerNum(int pNum)
    {
        playerNum = pNum;
    }

    [Command]
    public void CmdIncSkinNum()
    {
        skinNum = (skinNum+1) % maxSkinNum;
    }

    [Command]
    public void CmdIncHatNum()
    {
        hatNum = (hatNum + 1) % maxHatNum;
    }

    [Command]
    public void CmdReadyUp()
    {
        IsReady = !IsReady;

        Room.NotifyPlayersOfReadyState();
    }

    [Command]

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

    [Command]

    public void CmdChangeSkin()
    {
        Room.ChangeSkin(playerNum);
    }

}
