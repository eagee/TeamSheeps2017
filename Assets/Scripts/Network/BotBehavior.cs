using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.AI;

public class BotBehavior : NetworkBehaviour {

    NavMeshAgent agent;
    
    bool isMoving;
    private List<NetworkPlayerScript> players;
    private List<SpawnPointGizmo> spawnPoints;

    private int lastDiscoveryMethod = 0;

    private float nearTargetTime = 0.0f;

    // Use this for initialization
    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.enabled = true;
        players = GetComponent<NetworkPlayerScript>().GetBotsAndPlayers();
        spawnPoints = FindObjectOfType<SpawnPointGizmo>().GetSpawnPointList();
    }

    [ServerCallback]    
    void Update()
    {
        
        if (players.Count >= 2)
        {
            if (isMoving == false)
            {
                StartMoving();
            }

            PathFindToOtherPlayers();
        }
    }

    private void RotateTwoardTarget()
    {
        Vector3 direction = (agent.destination - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 1f);
    }

    [Server]
    void PathFindToOtherPlayers()
    {
        if (!isMoving)
            return;

        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            nearTargetTime += Time.deltaTime;
            if(nearTargetTime > 5.0f)
            {
                nearTargetTime = 0.0f;
                StartMoving();
            }
            else
            {
                RotateTwoardTarget();
            }
        }
            
    }

    [Server]
    void StartMoving()
    {
        lastDiscoveryMethod++;
        if (lastDiscoveryMethod > 1) lastDiscoveryMethod = 0;

        // int randomDiscoveryMethod = (int)Random.Range(0f, 3f);
        if (lastDiscoveryMethod == 0)
        {
            // Pick a random player to find
            float count = (float)players.Count;
            Transform newTarget = players[(int)Random.Range(0f, count)].gameObject.transform;
            while (newTarget.position == transform.position) // || newTarget.position == agent.transform.position
            {
                newTarget = players[(int)Random.Range(0f, count)].gameObject.transform;
            }
            agent.SetDestination(newTarget.position);
        }
        if (lastDiscoveryMethod == 1)
        {
            // Pick a random spawn point to return to
            float count = (float)spawnPoints.Count;
            Transform newTarget = spawnPoints[(int)Random.Range(0f, count)].gameObject.transform;
            agent.SetDestination(newTarget.position);
        }
        //else
        //{
        //    // Find furthest away player
        //    float distance = 0.0f;
        //    Vector3 targetPosition = new Vector3(0f, 0f, this.transform.position.z);
        //    foreach (var player in players)
        //    {
        //        float lastDistance = distance;
        //        distance = Vector3.Distance(player.transform.position, this.transform.position);
        //        if (distance > lastDistance)
        //        {
        //            targetPosition = player.transform.position;
        //        }
        //    }
        //    agent.SetDestination(targetPosition);
        //}

        // Find Random
        isMoving = true;
    }
}
