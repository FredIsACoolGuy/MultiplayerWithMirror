using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class CharacterLookScript : NetworkBehaviour
{
    private SkinnedMeshRenderer mr;

    public Material[] skins;

    public GameObject[] hats;


    public void playerStart()
    {
        mr = this.GetComponentInChildren<SkinnedMeshRenderer>();
        if (this.GetComponent<NetworkGamePlayer>() != null)
        {
            changeSkin(this.GetComponent<NetworkGamePlayer>().skinNum);
            changeHat(this.GetComponent<NetworkGamePlayer>().hatNum);
        }
    }

    public void setVisable(bool visable)
    {
        gameObject.SetActive(visable);
    }

    public void changeSkin(int skinNum)
    {
        mr.material = skins[skinNum];
    }

    public void changeHat(int hatNum)
    {
        for(int i=0; i<hats.Length; i++)
        {
            if (i == hatNum)
            {
                hats[i].SetActive(true);
            }
            else
            {
                hats[i].SetActive(false);
            }
        }
    }
}