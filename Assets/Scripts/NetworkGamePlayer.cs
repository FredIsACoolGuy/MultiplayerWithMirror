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

    private GameObject[] characters = new GameObject[4];

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

    public override void OnStartClient()
    {
        DontDestroyOnLoad(this.gameObject);
        Room.GamePlayers.Add(this);
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

}
