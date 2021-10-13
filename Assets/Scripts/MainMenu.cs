using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private NetworkManagerOverride networkManager = null;

    [SerializeField] private GameObject landingPage = null;

    //hide start screen when hosting
    public void HostLobby()
    {
        networkManager.StartHost();
        landingPage.SetActive(false);
    }
}
