using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class JoinLobbyMenu : MonoBehaviour
{
    [SerializeField] private NetworkManagerOverride networkManager = null;

    [SerializeField] private GameObject landingPage = null;
    [SerializeField] private TMP_InputField ipAddressInput = null;
    [SerializeField] private Button joinButton = null;


    private void OnEnable()
    {
        NetworkManagerOverride.OnClientConnected += HandleClientConnected;
        NetworkManagerOverride.OnClientDisconnected += HandleClientDisconnected;
    }

    private void OnDisable()
    {
        NetworkManagerOverride.OnClientConnected -= HandleClientConnected;
        NetworkManagerOverride.OnClientDisconnected -= HandleClientDisconnected;
    }

    public void JoinLobby()
    {
        string ipAddress = ipAddressInput.text;

        networkManager.networkAddress = ipAddress;
        networkManager.StartClient();

        joinButton.interactable = false;
    }

    private void HandleClientConnected()
    {
        joinButton.interactable = true;

        gameObject.SetActive(false);
        landingPage.SetActive(false);
    }
    
    private void HandleClientDisconnected()
    {
        joinButton.interactable = true;
    }
}
