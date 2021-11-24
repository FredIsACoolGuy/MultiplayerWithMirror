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

        lobbyMatchList = Callback<LobbyMatchList_t>.Create(OnLobbyMatchList);
        lobbyEntered = Callback<LobbyEnter_t>.Create(OnLobbyEntered);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void clicked()
    {   
        SteamMatchmaking.AddRequestLobbyListStringFilter("Key", "FredsGame", ELobbyComparison.k_ELobbyComparisonEqual);
        SteamMatchmaking.RequestLobbyList();
    }

    private void OnLobbyMatchList(LobbyMatchList_t callback)
    {

        if (callback.m_nLobbiesMatching == 0)
        {
            steamLobby.HostPublicLobby();
        }
        else
        {
            lobbyToCheck = 0;
            maxLobbyToCheck = (int)callback.m_nLobbiesMatching;
            tryJoinLobby();
        }
        for (int i = 0; i< callback.m_nLobbiesMatching; i++)
        {
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

            SteamMatchmaking.JoinLobby(lobbyID);
        }

        /* if (callback.m_nLobbiesMatching == 0)
         {
             text.text = "No LOBBIE";
         }*/

       // text.text = text.text+callback.m_nLobbiesMatching.ToString();
    }


    private void tryJoinLobby()
    {
        CSteamID lobbyID = SteamMatchmaking.GetLobbyByIndex(lobbyToCheck);
        SteamMatchmaking.JoinLobby(lobbyID);
    }


    private void OnLobbyEntered(LobbyEnter_t callback)
    {
       if(callback.m_EChatRoomEnterResponse == 5)
        {
            lobbyToCheck++;

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

        if (GameObject.Find("JoinPanel"))
        {
            GameObject.Find("JoinPanel").SetActive(false);
        }

        string hostAddress = SteamMatchmaking.GetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), hostAddressKey);

        networkManager.networkAddress = hostAddress;
        networkManager.StartClient();
    }

}
