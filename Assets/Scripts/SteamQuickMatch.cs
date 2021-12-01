using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using Steamworks;

public class SteamQuickMatch : MonoBehaviour
{
    private NetworkManager networkManager;
    protected Callback<LobbyMatchList_t> lobbyMatchList;
    protected Callback<LobbyEnter_t> lobbyEntered;
    
    private const string hostAddressKey = "hostAddress";

    private int lobbyToCheck = 0;
    private int maxLobbyToCheck = 0;

    public SteamLobby steamLobby;
    // Start is called before the first frame update
    void Start()
    {
        networkManager = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();
        //set up callbacks
        lobbyMatchList = Callback<LobbyMatchList_t>.Create(OnLobbyMatchList);
        lobbyEntered = Callback<LobbyEnter_t>.Create(OnLobbyEntered);
    }

    //called when button clicked
    public void clicked()
    {   
        //adds a filter to the search to make sure it is only finding lobbies playing this game
        SteamMatchmaking.AddRequestLobbyListStringFilter("Key", "FredsGame", ELobbyComparison.k_ELobbyComparisonEqual);
        //requests a list of all lobbies from steam using the filter we just set above
        SteamMatchmaking.RequestLobbyList();
    }

    //called once steam retreives the lobby list
    private void OnLobbyMatchList(LobbyMatchList_t callback)
    {
        //if there are no lobbies then host a public lobby
        if (callback.m_nLobbiesMatching == 0)
        {
            steamLobby.HostPublicLobby();
        }
        else
        {
            //if there are lobbies then call tryJoinLobby
            lobbyToCheck = 0;
            maxLobbyToCheck = (int)callback.m_nLobbiesMatching;
            tryJoinLobby();
        }
        //loop through all lobbies in the list
        for (int i = 0; i< callback.m_nLobbiesMatching; i++)
        {
            //get the lobby ID 
            CSteamID lobbyID = SteamMatchmaking.GetLobbyByIndex(i);

            ///Debuging
            ///
            ///All for outputting data and debugging//
            ///
            ///text.text = text.text+SteamMatchmaking.GetLobbyData(lobbyID, "Key");
            ///
            ///string mKey = "poop";
            ///int keyBuffer = 10;
            ///string value = "poop";
            ///int valueBuffer = 10;
            ///bool lobbyDataRet = SteamMatchmaking.GetLobbyDataByIndex(lobbyID, 1, out mKey, keyBuffer, out value, valueBuffer);
            ///
            ///text.text = text.text + " , " + value;

            //join this lobby
            SteamMatchmaking.JoinLobby(lobbyID);
        }
    }

    //get the lobbyID and join it
    private void tryJoinLobby()
    {
        CSteamID lobbyID = SteamMatchmaking.GetLobbyByIndex(lobbyToCheck);
        SteamMatchmaking.JoinLobby(lobbyID);
    }


    //if lobby entered call back
    private void OnLobbyEntered(LobbyEnter_t callback)
    {
        //this checks if the lobby was joined or not
       if(callback.m_EChatRoomEnterResponse == 5)
        {
            //if the lobby was not entered then increase lobbyToCheck num
            lobbyToCheck++;

            //if all the lobbies have been checked and failed to join, host a new lobby
            if (lobbyToCheck >= maxLobbyToCheck)
            {
                steamLobby.HostPublicLobby();
                return;
            }

            tryJoinLobby();
        }


        if (NetworkServer.active)
        {
            return;
        }
        //hide unnessicarry UI
        if (GameObject.Find("JoinPanel"))
        {
            GameObject.Find("JoinPanel").SetActive(false);
        }
        //join the Mirror server
        string hostAddress = SteamMatchmaking.GetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), hostAddressKey);

        networkManager.networkAddress = hostAddress;
        networkManager.StartClient();
    }
}
