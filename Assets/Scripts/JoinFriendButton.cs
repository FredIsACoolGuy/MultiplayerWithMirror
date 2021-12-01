using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using Steamworks;
using TMPro;

public class JoinFriendButton : MonoBehaviour
{
    //stored index of friend
    public int friendNum;
    //test to display friend name
    public TMP_Text tmp;
    //raw image for showing the friends steam avatar
    public RawImage avatar;
    //stores the friends steam id
    private ulong steamIdStored;
    //the network manager that deals with connections
    private NetworkManager networkManager;

    //callbacks will be called when Steam competes certain actions
    protected Callback<AvatarImageLoaded_t> avatarImageLoaded;


    private void Start()
    {
        //get the network manager
        networkManager = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();
        //setup for callback
        avatarImageLoaded = Callback<AvatarImageLoaded_t>.Create(OnAvatarImageLoaded);
    }

    //called when button clicked
    public void clicked()
    {
        //gets the friends info from steam using the index stored in friendNum
        FriendGameInfo_t friendInfo;
        CSteamID steamIdFriend = SteamFriends.GetFriendByIndex(friendNum, EFriendFlags.k_EFriendFlagImmediate);
        SteamFriends.GetFriendGamePlayed(steamIdFriend, out friendInfo);
        //joins the lobby your friend is in
        SteamMatchmaking.JoinLobby(friendInfo.m_steamIDLobby);
    }

    //sets the text to display the friends steam name
    public void setName(string name)
    {
        tmp.text = name;       
    }

    //displays the steam avatar
    public void setImage(CSteamID steamIdFriend)
    {
        //stores the friends steam ID for use later on
        steamIdStored = steamIdFriend.m_SteamID;

        //gets the friends avatar from steam
        int imageId = SteamFriends.GetMediumFriendAvatar(steamIdFriend);

        //if image hasnt loaded yet it returns -1
        if (imageId == -1)
        {
            return;
        }

        //sets the texture to be the steam avatar image
        avatar.texture = GetSteamImage(imageId);
    }

    private Texture2D GetSteamImage(int iImage)
    {
        //set up new texture
        Texture2D texture = null;

        //gets the size of the image and makes sure it is valid
        bool isValid = SteamUtils.GetImageSize(iImage, out uint width, out uint height);

        if (isValid)
        {
            //converts image into an array of bytes - 4 bytes for each pixel: RGBA
            byte[] image = new byte[width * height * 4];

            //checks if the image can be converted to a raw image
            isValid = SteamUtils.GetImageRGBA(iImage, image, (int)(width * height * 4));

            if (isValid)
            {
                //creates a new texture from the bytes
                texture = new Texture2D((int)(width), (int)(height), TextureFormat.RGBA32, false, true);
                texture.LoadRawTextureData(image);
                texture.Apply();
            }
        }

        //returns the new texture
        return texture;
    }

    //once the avatar image loads it calls GetSteamImage to display it
    private void OnAvatarImageLoaded(AvatarImageLoaded_t callback)
    {
        if(callback.m_steamID.m_SteamID != steamIdStored)
        {
            return;
        }

        avatar.texture = GetSteamImage(callback.m_iImage);
    }

}
