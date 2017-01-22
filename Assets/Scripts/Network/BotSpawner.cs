using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class BotSpawner : NetworkBehaviour
{
    [SerializeField]
    GameObject OVRNetworkPlayerPrefab;
    public int numberOfBotsToSpawn = 15;

    [ServerCallback]
    void Start()
    {
        for(int i = 0; i < numberOfBotsToSpawn; i++)
        { 
            NetworkStartPosition[] startPositions = FindObjectsOfType<NetworkStartPosition>();
            Transform startPos = startPositions[Random.Range(0, startPositions.Length - 1)].transform;
            GameObject obj = Instantiate(OVRNetworkPlayerPrefab,startPos.position, startPos.rotation);
            obj.GetComponent<NetworkIdentity>().localPlayerAuthority = false;
            obj.AddComponent<BotBehavior>();

            obj.tag = "Bot";
            obj.name = obj.name + this.gameObject.name;
            NetworkServer.Spawn(obj);
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawIcon(transform.position, "Bot.png", true);
    }
}
