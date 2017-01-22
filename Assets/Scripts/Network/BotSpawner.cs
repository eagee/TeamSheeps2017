using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class BotSpawner : NetworkBehaviour
{
    [SerializeField]
    GameObject OVRNetworkPlayerPrefab;

    [ServerCallback]
    void Start()
    {
        Transform startPos = NetworkManager.singleton.GetStartPosition();
        GameObject obj = Instantiate(OVRNetworkPlayerPrefab,startPos.position, startPos.rotation);
        obj.GetComponent<NetworkIdentity>().localPlayerAuthority = false;
        obj.AddComponent<BotBehavior>();
        //obj.GetComponent<BotBehavior>().SetAwakeVector(this.gameObject.transform.position);
        obj.name = obj.name + this.gameObject.name;
        NetworkServer.Spawn(obj);
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawIcon(transform.position, "Bot.png", true);
    }
}
