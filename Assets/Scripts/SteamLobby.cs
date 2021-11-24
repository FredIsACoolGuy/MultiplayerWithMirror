using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Steamworks;

public class SteamLobby : MonoBehaviour
{
    protected Callback<LobbyCreated_t> lobbyCreated;
    protected Callback<LobbyDataUpdate_t> lobbyDataUpdate;
    protected Callback<GameLobbyJoinRequested_t> gameLobbyJoinRequested;
    protected Callback<LobbyEnter_t> lobbyEntered;

    private NetworkManager networkManager;

    private const string hostAddressKey = "hostAddress";

    private CSteamID lobbyID;
    private void Start()
    {
        Debug.Log(this.name);
        networkManager = GetComponent<NetworkManager>();

        if (!SteamManager.Initialized)
        {
            return;
        }

        lobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
        lobbyDataUpdate = Callback<LobbyDataUpdate_t>.Create(OnLobbyDataUpdate);
        gameLobbyJoinRequested = Callback<GameLobbyJoinRequested_t>.Create(OnGameLobbyJoinRequested);
        lobbyEntered = Callback<LobbyEnter_t>.Create(OnLobbyEntered);
    }
    public void HostLobby()
    {
        ClientDisconnect();

        SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypeFriendsOnly, networkManager.maxConnections);   
    }

    public void HostPublicLobby()
    {
        ClientDisconnect();

        SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypePublic, networkManager.maxConnections);
    }

    private void OnLobbyCreated(LobbyCreated_t callback)
    {
        if(callback.m_eResult != EResult.k_EResultOK)
        {
            Debug.Log("failed to connect");
            return;
        }

        networkManager.StartHost();
        lobbyID = new CSteamID(callback.m_ulSteamIDLobby);
        SteamMatchmaking.SetLobbyData(lobbyID, hostAddressKey, SteamUser.GetSteamID().ToString());
        
        SteamMatchmaking.SetLobbyData(lobbyID, "Key", "FredsGame");
        if (GameObject.Find("LoadingPanel"))
        {
            GameObject.Find("LoadingPanel").SetActive(false);
        }


    }
    private void OnGameLobbyJoinRequested(GameLobbyJoinRequested_t callback)
    {
        SteamMatchmaking.JoinLobby(callback.m_steamIDLobby);
    }

    private void OnLobbyDataUpdate(LobbyDataUpdate_t callback)
    {
        //Debug.Log(callback);
        //Debug.Log(callback.m_bSuccess);
        //Debug.Log(callback.m_ulSteamIDLobby);
        //Debug.Log(callback.m_ulSteamIDMember);

        Debug.Log(SteamMatchmaking.GetLobbyData(lobbyID, "Key"));

    }



    private void OnLobbyEntered(LobbyEnter_t callback)
    {
        if (NetworkServer.active)
        {
            return;
        }
        ClientDisconnect();

        if (GameObject.Find("JoinPanel"))
        {
            GameObject.Find("JoinPanel").SetActive(false);
        }

        if (GameObject.Find("LoadingPanel"))
        {
            GameObject.Find("LoadingPanel").SetActive(false);
        }

        string hostAddress = SteamMatchmaking.GetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), hostAddressKey);

        networkManager.networkAddress = hostAddress;
        networkManager.StartClient();

        if (GameObject.Find("Title"))
        {
            GameObject.Find("Title").SetActive(false);
        }
        if (GameObject.Find("LandingPagePanel"))
        {
            GameObject.Find("LandingPagePanel").SetActive(false);
        }
    }

    public void ClientDisconnect()
    {
        FriendGameInfo_t playerInfo;
        SteamFriends.GetFriendGamePlayed(SteamUser.GetSteamID(), out playerInfo);
        Debug.Log("PlayerLobbyBefore:"+playerInfo.m_steamIDLobby);
        SteamMatchmaking.LeaveLobby(playerInfo.m_steamIDLobby);

        SteamFriends.GetFriendGamePlayed(SteamUser.GetSteamID(), out playerInfo);

        Debug.Log("PlayerLobbyafter:" + playerInfo.m_steamIDLobby);

    }


}
