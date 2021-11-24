using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;


public class DisconnectFromGameButton : MonoBehaviour
{
    public int sceneNum;
    public bool isLeader = false;
    private GameObject networkManager;

    void Start()
    {
        networkManager = GameObject.Find("NetworkManager");
    }

    void Update()
    {
        
    }
   
    public void clicked()
    {
        if (isLeader)
        {
            networkManager.GetComponent<NetworkManagerOverride>().StopClient();
            networkManager.GetComponent<NetworkManagerOverride>().StopServer();
        }
        else
        {
            networkManager.GetComponent<NetworkManagerOverride>().StopClient();

        }

        if (networkManager.GetComponent<SteamLobby>() != null)
        {
            networkManager.GetComponent<SteamLobby>().ClientDisconnect();
        }

        Destroy(networkManager);

        SceneManager.LoadScene(sceneNum, LoadSceneMode.Single);
    }
}
