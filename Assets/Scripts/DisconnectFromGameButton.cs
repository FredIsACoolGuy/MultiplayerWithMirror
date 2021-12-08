using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;


public class DisconnectFromGameButton : MonoBehaviour
{
    //number of scene to load
    public int sceneNum;

    //stores whether the client is also the host
    public bool isLeader = false;
    //the network manager
    private GameObject networkManager;

    void Start()
    {
        //finds the network manager
        networkManager = GameObject.Find("NetworkManager");
    }

    //called when the button is pressed
    public void clicked()
    {
        //if the client is also the host this shuts down the server aswell
        if (isLeader)
        {
            networkManager.GetComponent<NetworkManagerOverride>().StopClient();
            networkManager.GetComponent<NetworkManagerOverride>().StopServer();
        }
        else
        {
            networkManager.GetComponent<NetworkManagerOverride>().StopClient();
        }

        //if the network manager has the steam lobby component then the steam lobby has to be disconnected
        //this check is here because the game can still be played locally which doesnt use Steamworks
        if (networkManager.GetComponent<SteamLobby>() != null)
        {
            networkManager.GetComponent<SteamLobby>().ClientDisconnect();
        }



        //destroys this old network manager so a new one can be initialized
        Destroy(networkManager);
        //load the scene 
        SceneManager.LoadScene(sceneNum, LoadSceneMode.Single);
    }
}
