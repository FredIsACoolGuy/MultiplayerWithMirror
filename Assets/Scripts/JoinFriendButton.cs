using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using Steamworks;
using TMPro;

public class JoinFriendButton : MonoBehaviour
{
    public int friendNum;
    public TMP_Text tmp;
    public RawImage avatar;

    private ulong steamIdStored;
    private NetworkManager networkManager;

    protected Callback<AvatarImageLoaded_t> avatarImageLoaded;
   // protected Callback<LobbyEnter_t> lobbyEntered;
   // private const string hostAddressKey = "hostAddress";


    private void Start()
    {
        networkManager = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();
        Debug.Log(networkManager.name);
        avatarImageLoaded = Callback<AvatarImageLoaded_t>.Create(OnAvatarImageLoaded);
    }
    public void clicked()
    {
        FriendGameInfo_t friendInfo;
        CSteamID steamIdFriend = SteamFriends.GetFriendByIndex(friendNum, EFriendFlags.k_EFriendFlagImmediate);
        SteamFriends.GetFriendGamePlayed(steamIdFriend, out friendInfo);
        SteamMatchmaking.JoinLobby(friendInfo.m_steamIDLobby);
    }

    public void setName(string name)
    {
        tmp.text = name;       
    }

    public void setImage(CSteamID steamIdFriend)
    {
        steamIdStored = steamIdFriend.m_SteamID;

        int imageId = SteamFriends.GetMediumFriendAvatar(steamIdFriend);


        if (imageId == -1)
        {
            return;
        }

        avatar.texture = GetSteamImage(imageId);
    }

    private Texture2D GetSteamImage(int iImage)
    {
        Texture2D texture = null;

        bool isValid = SteamUtils.GetImageSize(iImage, out uint width, out uint height);

        if (isValid)
        {
            byte[] image = new byte[width * height * 4];

            isValid = SteamUtils.GetImageRGBA(iImage, image, (int)(width * height * 4));

            if (isValid)
            {
                texture = new Texture2D((int)(width), (int)(height), TextureFormat.RGBA32, false, true);
                texture.LoadRawTextureData(image);
                texture.Apply();
            }
        }

        return texture;
    }

    private void OnAvatarImageLoaded(AvatarImageLoaded_t callback)
    {
        if(callback.m_steamID.m_SteamID != steamIdStored)
        {
            return;
        }

        avatar.texture = GetSteamImage(callback.m_iImage);
    }

}
