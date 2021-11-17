using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using Steamworks;

public class SteamInviteFriendsList : MonoBehaviour
{
    public Text loadingText;
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
        int friendCount = SteamFriends.GetFriendCount(EFriendFlags.k_EFriendFlagAll);

        if (listOfObjects.Count > 0)
        {
            foreach (GameObject obj in listOfObjects)
            {
                Destroy(obj);
            }
            listOfObjects.Clear();
        }


        loadingText.enabled = true;
        loadingText.text = "no friends online";

        for (int i = 0; i < friendCount; i++)
        {
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
