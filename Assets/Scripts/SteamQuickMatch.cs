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

    public Text text;

    // Start is called before the first frame update
    void Start()
    {
        networkManager = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();

        lobbyMatchList = Callback<LobbyMatchList_t>.Create(OnLobbyMatchList);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void clicked()
    {
        text.text = "loadin";
        SteamMatchmaking.RequestLobbyList();
    }

    private void OnLobbyMatchList(LobbyMatchList_t callback)
    {

       


        for (int i = 0; i< callback.m_nLobbiesMatching; i++)
        {
            CSteamID lobbyID = SteamMatchmaking.GetLobbyByIndex(i);

            text.text = text.text+SteamMatchmaking.GetLobbyData(lobbyID, "Key");
        }

       /* if (callback.m_nLobbiesMatching == 0)
        {
            text.text = "No LOBBIE";
        }*/

        text.text = text.text+callback.m_nLobbiesMatching.ToString();
    }

}
