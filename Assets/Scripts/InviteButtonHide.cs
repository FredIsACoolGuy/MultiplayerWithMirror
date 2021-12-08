using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class InviteButtonHide : MonoBehaviour
{
    private GameObject networkManager;
    void Start()
    {
        //finds the network manager
        networkManager = GameObject.Find("NetworkManager");

        if (networkManager.GetComponent<TelepathyTransport>()!=null)
        {
            this.gameObject.SetActive(false);
        }
    }
}
