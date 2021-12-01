using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class CharacterLookScript : NetworkBehaviour
{
    private SkinnedMeshRenderer mr;
   
    [Header("Possible Looks")]
    public Material[] skins;
    public GameObject[] hats;

    //called when the player starts
    public void playerStart()
    {
        //find the SkinnedMeshRenderer 
        mr = this.GetComponentInChildren<SkinnedMeshRenderer>();
        //checks for Network Game Player, and then uses these stored numbers to update appearance
        if (this.GetComponent<NetworkGamePlayer>() != null)
        {
            changeSkin(this.GetComponent<NetworkGamePlayer>().skinNum);
            changeHat(this.GetComponent<NetworkGamePlayer>().hatNum);
        }
    }

    //sets the character to visable or not
    public void setVisable(bool visable)
    {
        gameObject.SetActive(visable);
    }

    //changes the material on the mesh renderer
    public void changeSkin(int skinNum)
    {
        mr.material = skins[skinNum];
    }

    //enables and disables hats to change the hat the character is wearing
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
