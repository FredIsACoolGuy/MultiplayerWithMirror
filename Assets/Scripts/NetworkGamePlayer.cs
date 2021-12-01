using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Mirror;

public class NetworkGamePlayer : NetworkBehaviour
{
    #region Customization Values
    //can only be updated on the server
    //when variables change these functions are called
    [SyncVar]
    public string DisplayName = "Loading...";
    [SyncVar]
    public int skinNum = 0;
    [SyncVar]
    public int hatNum = 0;
    #endregion

    //stores the network manager override
    private NetworkManagerOverride room;
    private NetworkManagerOverride Room
    {
        get
        {
            //if room is currently null it finds it and assigns to room
            if (room != null)
            {
                return room;
            }
            return room = NetworkManager.singleton as NetworkManagerOverride;
        }
    }
    
    //updates all player looks so they match the custom options picked by the players
    public void UpdateLooks()
    {
        for (int i = 0; i < 4; i++)
        {
            if (i < Room.GamePlayers.Count)
            {
                Room.GamePlayers[i].GetComponent<CharacterLookScript>().playerStart();
            }
        }
    }

    //overrides the OnStartClient which is called when the client starts
    public override void OnStartClient()
    {
        //keep this gameobject from being destroyed between scenes
        DontDestroyOnLoad(this.gameObject);
        //add to the list of players in the actual game in the Network Manager Override
        Room.GamePlayers.Add(this);
        //updates the players appearence
        UpdateLooks();
    }

    //called when the client stops
    public override void OnStopClient()
    {
        //removes from list of player in game
        Room.GamePlayers.Remove(this);
    }

    #region Server Functions
    //only ever run on the server - these are called when the game starts to pass customization from lobby scene to game scene
    [Server] //updates display name
    public void SetDisplayName(string displayName)
    {
        this.DisplayName = displayName;
    }

    [Server] //updates skin num
    public void SetSkinNum(int num)
    {
        this.skinNum = num;      
    }

    [Server] //updates hat num
    public void SetHatNum(int num)
    {
        this.hatNum = num;
    }
    #endregion
}
