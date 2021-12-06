using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using Steamworks;


public class SteamFindFriendsLobbies : MonoBehaviour
{
    //display text for friends name
    public Text friendName;
    //content box to hold all list items
    public RectTransform content;
    //prefab for all list items
    public GameObject listItem;

    //list holding all spawned list items
    private List<GameObject> listOfObjects = new List<GameObject>();

    void Start()
    {
        if (!SteamManager.Initialized)
        {
            return;
        }
    }

    //refreshes the list of friends lobbies
    public void RefreshList()
    {
        //gets the count of all player lobbies
        int friendCount = SteamFriends.GetFriendCount(EFriendFlags.k_EFriendFlagImmediate);

        //if there are more than 0 friends
        if (listOfObjects.Count > 0)
        {
            //destroys all gameobjects in list and clears the list
            foreach(GameObject obj in listOfObjects)
            {
                Destroy(obj);
            }
            listOfObjects.Clear();
        }

        // displayes no friends online if there are no 
        friendName.enabled = true;
        friendName.text = "no friends online";

        //loop through all friends in the list
        for (int i = 0; i < friendCount; i++)
        {
            //gets the steam info about the friend using their index number
            FriendGameInfo_t friendInfo;
            CSteamID steamIdFriend = SteamFriends.GetFriendByIndex(i, EFriendFlags.k_EFriendFlagImmediate); 
            
            //checks the friend is in a valid lobby
            if(SteamFriends.GetFriendGamePlayed(steamIdFriend, out friendInfo) && friendInfo.m_steamIDLobby.IsValid())
            {
                //disables the no friends online message
                friendName.enabled = false;
                //instantiates new item for in the list
                GameObject newItem = Instantiate(listItem, content);
                //adds new item to the list
                listOfObjects.Add(newItem);
                //passes data to the button
                JoinFriendButton button = newItem.GetComponent<JoinFriendButton>();
                button.friendNum = i;
                button.setName(SteamFriends.GetFriendPersonaName(steamIdFriend));
                button.setImage(steamIdFriend);
            }
        }
    }

}
