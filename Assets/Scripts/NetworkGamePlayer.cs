using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Mirror;

public class NetworkGamePlayer : NetworkBehaviour
{
    //can only be updated on the server
    //when variables change these functions are called
    [SyncVar]
    public string DisplayName = "Loading...";

    [SyncVar]
    public int skinNum = 0;
    [SyncVar]
    public int hatNum = 0;

    private List<GameObject> characters = new List<GameObject>();

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


    public override void OnStartClient()
    {
        DontDestroyOnLoad(this.gameObject);
        Room.GamePlayers.Add(this);
        UpdateLooks();
    }

    public override void OnStopClient()
    {
        Room.GamePlayers.Remove(this);
    }

    //only ever run on the server
    [Server]
    public void SetDisplayName(string displayName)
    {
        this.DisplayName = displayName;
    }

    [Server]
    public void SetSkinNum(int num)
    {
        this.skinNum = num;      
    }

    [Server]
    public void SetHatNum(int num)
    {
        this.hatNum = num;
    }

}
