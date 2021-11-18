using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using Steamworks;


public class SteamFindFriendsLobbies : MonoBehaviour
{

    public Text friendName;
    public RectTransform content;
    public GameObject listItem;

    private List<GameObject> listOfObjects = new List<GameObject>();

    void Start()
    {
        if (!SteamManager.Initialized)
        {
            return;
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void RefreshList()
    {
        int friendCount = SteamFriends.GetFriendCount(EFriendFlags.k_EFriendFlagImmediate);

        if (listOfObjects.Count > 0)
        {
            foreach(GameObject obj in listOfObjects)
            {
                Destroy(obj);
            }
            listOfObjects.Clear();
        }


        friendName.enabled = true;
        friendName.text = "no friends online";
        for (int i = 0; i < friendCount; i++)
        {
            FriendGameInfo_t friendInfo;
            CSteamID steamIdFriend = SteamFriends.GetFriendByIndex(i, EFriendFlags.k_EFriendFlagImmediate); 
            if(SteamFriends.GetFriendGamePlayed(steamIdFriend, out friendInfo) && friendInfo.m_steamIDLobby.IsValid())
            {
                friendName.enabled = false;
                GameObject newItem = Instantiate(listItem, content);
                listOfObjects.Add(newItem);
                newItem.GetComponent<JoinFriendButton>().friendNum = i;
                newItem.GetComponent<JoinFriendButton>().setName(SteamFriends.GetFriendPersonaName(steamIdFriend));
                newItem.GetComponent<JoinFriendButton>().setImage(steamIdFriend);
            }
        }
    }

}
