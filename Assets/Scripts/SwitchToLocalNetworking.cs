using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchToLocalNetworking : MonoBehaviour
{
    private GameObject networkMan;
    void Start()
    {
        networkMan = GameObject.Find("NetworkManager");
    }

    public void clicked()
    {
        
    }
}
