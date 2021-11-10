using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System.Linq;

public class PlayerSpawnSystem : NetworkBehaviour
{
    [SerializeField] private GameObject playerPrefab = null;

    private static List<Transform> spawnPoints = new List<Transform>();

    private int nextIndex = 0;


    public static void AddSpawnPoint(Transform transform)
    {
        spawnPoints.Add(transform);

        spawnPoints = spawnPoints.OrderBy(x => x.GetSiblingIndex()).ToList();
    }

    public static void RemoveSpawnPoint(Transform transform)
    {
        spawnPoints.Remove(transform);
    }

    public override void OnStartServer()
    {
        NetworkManagerOverride.OnServerReadied += SpawnPlayer;
    }

    [ServerCallback]

    private void OnDestroy()
    {
        NetworkManagerOverride.OnServerReadied -= SpawnPlayer;
    }

    [Server]
    public void SpawnPlayer(NetworkConnection conn) 
    {
        Transform spawnPoint = spawnPoints.ElementAtOrDefault(nextIndex);

        if (spawnPoint == null)
        {
            Debug.LogError($"Missing spawn point for player{nextIndex}");
            return;
        }

        GameObject playerInstance = conn.identity.gameObject;

     
        CallLooks(playerInstance, nextIndex);
        //GameObject playerInstance = Instantiate(playerPrefab, spawnPoints[nextIndex].position, spawnPoints[nextIndex].rotation);
        //playerInstance.GetComponent<SkinnedMeshRenderer>().material = conn.identity.GetComponent<SkinnedMeshRenderer>().material;

        //Debug.Log(conn);

        //NetworkServer.Spawn(playerInstance, conn);
        
        //NetworkServer.AddPlayerForConnection(conn, playerInstance);
        nextIndex++;
    }

    [ClientRpc]
    public void CallLooks(GameObject playerInstance, int posNum)
    {
        playerInstance.transform.position = spawnPoints[posNum].position;
        playerInstance.transform.rotation = spawnPoints[posNum].rotation;
        playerInstance.GetComponent<CharacterLookScript>().playerStart();
        
    }
}
