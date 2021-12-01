using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Steamworks;

public class SteamLobby : MonoBehaviour
{
    //callbacks are called from steam when certain actions are achived
    protected Callback<LobbyCreated_t> lobbyCreated;
    protected Callback<LobbyDataUpdate_t> lobbyDataUpdate;
    protected Callback<GameLobbyJoinRequested_t> gameLobbyJoinRequested;
    protected Callback<LobbyEnter_t> lobbyEntered;

    //stores the network manager
    private NetworkManager networkManager;

    //stores the adress key for the hostAddress
    private const string hostAddressKey = "hostAddress";

    //stores the current lobby ID
    private CSteamID lobbyID;

    //called on start
    private void Start()
    {
        Debug.Log(this.name);
        networkManager = GetComponent<NetworkManager>();

        //if the steam manager is not initialized then return, otherwise continue
        if (!SteamManager.Initialized)
        {
            return;
        }

        //calls the OnLobbyCreated function when Steam succesfully creates a lobby
        lobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
        //calls the OnLobbyUpdate function when steam updates lobby data
        lobbyDataUpdate = Callback<LobbyDataUpdate_t>.Create(OnLobbyDataUpdate);
        //calls the OnGameLobbyJoinRequested when steam recieves a request to join a lobby
        gameLobbyJoinRequested = Callback<GameLobbyJoinRequested_t>.Create(OnGameLobbyJoinRequested);
        //calls the OnLobbyEntered when steam user enters a lobby
        lobbyEntered = Callback<LobbyEnter_t>.Create(OnLobbyEntered);
    }

    //called when button pressed to start hosting lobby
    public void HostLobby()
    {
        //disconnects in case client is connected to an old lobby
        ClientDisconnect();
        //create a new lobby for only friends to join and with a max of 4 connections
        SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypeFriendsOnly, networkManager.maxConnections);   
    }

    //called when starting a quick play lobby
    public void HostPublicLobby()
    {
        //disconnects in case client is connected to an old lobby
        ClientDisconnect();
        //create a new public lobby for anyone to join and with a max of 4 connections
        SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypePublic, networkManager.maxConnections);
    }

    //called when steam creates a lobby
    private void OnLobbyCreated(LobbyCreated_t callback)
    {
        //if the callback result is not ok then output error and return
        if(callback.m_eResult != EResult.k_EResultOK)
        {
            Debug.Log("failed to connect");
            return;
        }

        //otherwise start a host using Mirror
        networkManager.StartHost();
        //get lobby data from steam
        lobbyID = new CSteamID(callback.m_ulSteamIDLobby);
        //set metadata about the lobby using the lobby ID
        //host address is the value used for other players to join the Mirror lobby
        SteamMatchmaking.SetLobbyData(lobbyID, hostAddressKey, SteamUser.GetSteamID().ToString());     
        //This key is used to distinguish this game from any others
        SteamMatchmaking.SetLobbyData(lobbyID, "Key", "FredsGame");
        //hides the loading panel once it has completed loading
        if (GameObject.Find("LoadingPanel"))
        {
            GameObject.Find("LoadingPanel").SetActive(false);
        }
    }

    //called when steam receives a lobby join request
    private void OnGameLobbyJoinRequested(GameLobbyJoinRequested_t callback)
    {
        //joins the user to the steam lobby
        SteamMatchmaking.JoinLobby(callback.m_steamIDLobby);
    }

    //called when metadata about the lobby is updated
    private void OnLobbyDataUpdate(LobbyDataUpdate_t callback)
    {
        //used to check the Key data has been updated
        Debug.Log(SteamMatchmaking.GetLobbyData(lobbyID, "Key"));
    }


    //called when steam user enters a lobby
    private void OnLobbyEntered(LobbyEnter_t callback)
    {
        //if there is the network server running then return. otherwise continue
        if (NetworkServer.active)
        {
            return;
        }

        //disconnect from lobby incase still connected to an old lobby
        ClientDisconnect();

        //if the join panel is active then hide it
        if (GameObject.Find("JoinPanel"))
        {
            GameObject.Find("JoinPanel").SetActive(false);
        }

        //if the loading panel is active then hide it
        if (GameObject.Find("LoadingPanel"))
        {
            GameObject.Find("LoadingPanel").SetActive(false);
        }

        //get the host address from the steam lobby
        string hostAddress = SteamMatchmaking.GetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), hostAddressKey);

        //use host address to connect to the Mirror host
        networkManager.networkAddress = hostAddress;
        networkManager.StartClient();

        //if title is active then hide it
        if (GameObject.Find("Title"))
        {
            GameObject.Find("Title").SetActive(false);
        }
        //if Landing panel page is active then hide it
        if (GameObject.Find("LandingPagePanel"))
        {
            GameObject.Find("LandingPagePanel").SetActive(false);
        }
    }

    //used to disconnect client from steam lobby
    public void ClientDisconnect()
    {
        //gets player info from steam
        FriendGameInfo_t playerInfo;
        SteamFriends.GetFriendGamePlayed(SteamUser.GetSteamID(), out playerInfo);

        //leaves the current lobby
        SteamMatchmaking.LeaveLobby(playerInfo.m_steamIDLobby);
    }


}
