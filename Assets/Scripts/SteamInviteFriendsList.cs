using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using Steamworks;

public class SteamInviteFriendsList : MonoBehaviour
{
    //text used to indicate that the list is loading
    public Text loadingText;
    //the parent object for all list items
    public RectTransform content;
    //the base gameobject for the list items
    public GameObject listItem;

    //list which stores all currently instantiated items
    private List<GameObject> listOfObjects = new List<GameObject>();

    //loads all friends from steam into a list in game
    public void RefreshList()
    {
        //gets the length of the list of all players steam friends
        int friendCount = SteamFriends.GetFriendCount(EFriendFlags.k_EFriendFlagAll);


        //if the list is not empty it destroys all gameobjects and clears the list
        //this allows for a brand new list to be formed each time
        if (listOfObjects.Count > 0)
        {
            foreach (GameObject obj in listOfObjects)
            {
                Destroy(obj);
            }
            listOfObjects.Clear();
        }

        //shows text to say there are no friends
        //this gets hidden as soon as a single friend is found
        loadingText.enabled = true;
        loadingText.text = "no friends online";

        //loops through all friends in the players steam friends
        for (int i = 0; i < friendCount; i++)
        {
            //gets the friends steam ID
            CSteamID steamIdFriend = SteamFriends.GetFriendByIndex(i, EFriendFlags.k_EFriendFlagAll);
            
            loadingText.enabled = false;
            GameObject newItem = Instantiate(listItem, content);
            listOfObjects.Add(newItem);
            newItem.GetComponent<InviteFriendButton>().friendNum = i;
            newItem.GetComponent<InviteFriendButton>().setName(SteamFriends.GetFriendPersonaName(steamIdFriend));
            newItem.GetComponent<InviteFriendButton>().setImage(steamIdFriend);
            
        }
    }

}