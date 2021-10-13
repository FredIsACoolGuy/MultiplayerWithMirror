using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Linq;
using Mirror;

public class NetworkManagerOverride : NetworkManager
{
    //name of menu scene
    [Scene][SerializeField] private string menuScene = string.Empty;

    [SerializeField] private int minPlayers = 2;

    [SerializeField] private NetworkRoomPlayerLobby roomPlayerPrefab = null;

    public static event Action OnClientConnected;
    public static event Action OnClientDisconnected;

    public List<NetworkRoomPlayerLobby> RoomPlayers { get; } = new List<NetworkRoomPlayerLobby>();

    //load all prefabs that can be spawned into the networked scene
    public override void OnStartServer() => spawnPrefabs = Resources.LoadAll<GameObject>("SpawnablePrefabs").ToList();

    public override void OnStartClient()
    {
        var spawnablePrefabs = Resources.LoadAll<GameObject>("SpawnablePrefabs");

        foreach(var prefab in spawnablePrefabs)
        {
            NetworkClient.RegisterPrefab(prefab);
        }
    }

    //called on client when connecting to the server
    public override void OnClientConnect(NetworkConnection conn)
    {
        base.OnClientConnect(conn);

        OnClientConnected?.Invoke();
    }

    //called on client when disconnecting from the server
    public override void OnClientDisconnect(NetworkConnection conn)
    {
        base.OnClientDisconnect(conn);

        OnClientDisconnected?.Invoke();
    }

    //called when a client connects to the server
    public override void OnServerConnect(NetworkConnection conn)
    {
        //if there are too many players then disconnect
        if (numPlayers >= maxConnections)
        {
            conn.Disconnect();
            return;
        }

        //if they arent shown the menu scene then disconnect
        //stops players joining mid-game
        if(SceneManager.GetActiveScene().path != menuScene)
        {
            conn.Disconnect();
            return;
        }
    }

    //spawns in player prefab and links the object to this connection
    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        if(SceneManager.GetActiveScene().path == menuScene)
        {
            bool isLeader = RoomPlayers.Count == 0;

            NetworkRoomPlayerLobby roomPlayerInstance = Instantiate(roomPlayerPrefab);

            roomPlayerInstance.IsLeader = isLeader;

            NetworkServer.AddPlayerForConnection(conn, roomPlayerInstance.gameObject);
        }
    }


    public override void OnServerDisconnect(NetworkConnection conn)
    {
        if(conn.identity != null)
        {
            var player = conn.identity.GetComponent<NetworkRoomPlayerLobby>();

            RoomPlayers.Remove(player);

            NotifyPlayersOfReadyState();

        }
        base.OnServerDisconnect(conn);
    }

    public override void OnStopServer()
    {
        RoomPlayers.Clear();
    }

    public void NotifyPlayersOfReadyState()
    {
        foreach(NetworkRoomPlayerLobby player in RoomPlayers)
        {
            player.HandleReadyToStart(IsReadyToStart());
        }
    }

    private bool IsReadyToStart()
    {
        Debug.Log(numPlayers);
        if (numPlayers < minPlayers)
        {
            return false;
        }

        foreach (NetworkRoomPlayerLobby player in RoomPlayers)
        {
            if (!player.IsReady)
            {
                return false;
            }
        }

        return true;
    }
}
