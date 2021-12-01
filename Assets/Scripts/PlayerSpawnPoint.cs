using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawnPoint : MonoBehaviour
{
    //Adds spawn point into player spawn system
    private void Awake()
    {
        PlayerSpawnSystem.AddSpawnPoint(transform);
    }

    //Rmeovers spawn point from player spawn system
    private void OnDestroy()
    {
        PlayerSpawnSystem.RemoveSpawnPoint(transform);
    }

    //draws gizmos to show position and rotation
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 0.5f);
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * 2f);

    }
}
