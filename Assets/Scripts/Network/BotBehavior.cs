using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.AI;

public class BotBehavior : NetworkBehaviour {

    NavMeshAgent agent;
    public Transform target;

    bool isMoving;
    private NetworkPlayerScript[] players;

    // Use this for initialization
    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.enabled = true;
    }

    [ServerCallback]    
    void Update()
    {
        
        if (GetComponent<NetworkPlayerScript>().GetBotsAndPlayers().Count >= 2)
        {
            if (isMoving == false)
            {
                StartMoving();
            }

            PathFindToOtherPlayers();
        }
    }

    [Server]
    void PathFindToOtherPlayers()
    {
        if (!isMoving)
            return;

        if (agent.stoppingDistance >= agent.remainingDistance)
            Invoke("StartMoving", 10f);
    }

    [Server]
    void StartMoving()
    {

        // Find furthest away
        // float distance = 0.0f;
        // Vector3 targetPosition = new Vector3(0f, 0f, this.transform.position.z);
        // foreach(var player in players)
        // {
        //     float lastDistance = distance;
        //     distance = Vector3.Distance(player.transform.position, this.transform.position);
        //     if (distance > lastDistance)
        //     {
        //         targetPosition = player.transform.position;
        //     }
        // }
        // agent.SetDestination(targetPosition);

        // Find Random
        int count = GetComponent<NetworkPlayerScript>().GetBotsAndPlayers().Count;
        Transform newTarget = GetComponent<NetworkPlayerScript>().GetBotsAndPlayers()[(int)Random.Range(0, count - 1)].gameObject.transform;
        while (newTarget == transform || newTarget == agent.transform)
        {
            newTarget = GetComponent<NetworkPlayerScript>().GetBotsAndPlayers()[(int)Random.Range(0, count-1)].gameObject.transform;
        }

        agent.SetDestination(newTarget.position);
        isMoving = true;
    }
}
