using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using Steamworks;
using TMPro;

public class InviteFriendButton : MonoBehaviour
{
    //int to store the friends index
    public int friendNum;
    //text to show if invite sent successfully
    public TMP_Text tmp;
    //the raw image needed to show the friends steam avatar
    public RawImage avatar;
    //stores the friends steam id
    private ulong steamIdStored;

    //callbacks will be called when Steam competes certain actions
    protected Callback<AvatarImageLoaded_t> avatarImageLoaded;

    //setup for the callback
    private void Start()
    {
        avatarImageLoaded = Callback<AvatarImageLoaded_t>.Create(OnAvatarImageLoaded);
    }

    //called when button pressed
    public void clicked()
    {
        //finds the steamID for the friend using the index stored in friendNum
        CSteamID steamIdFriend = SteamFriends.GetFriendByIndex(friendNum, EFriendFlags.k_EFriendFlagAll);

        //gets the player info about this player using their steamID to get the current lobby ID
        FriendGameInfo_t playerInfo;
        SteamFriends.GetFriendGamePlayed(SteamUser.GetSteamID(), out playerInfo);
        
        //invites friend to lobby - this returns true if sent successfully and false if not sent
        if (SteamMatchmaking.InviteUserToLobby(playerInfo.m_steamIDLobby, steamIdFriend))
        {
            tmp.text="Sent";
        }
        else
        {
            tmp.text = "Not Sent";
        }
    }

    //displays the friends name
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
        if (callback.m_steamID.m_SteamID != steamIdStored)
        {
            return;
        }

        avatar.texture = GetSteamImage(callback.m_iImage);
    }
}
