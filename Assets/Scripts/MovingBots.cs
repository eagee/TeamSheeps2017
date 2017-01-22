using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MovingBots: MonoBehaviour
{

    NavMeshAgent agent;
    public Transform target;

    bool isMoving;

    // Use this for initialization
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        //agent.SetDestination(target.position);
        StartMoving();
    }

    void StartMoving()
    {
        Transform newTarget = NavManager.instance.GetPlayer();
        while (newTarget == transform && newTarget != agent)
        {
            newTarget = NavManager.instance.GetPlayer();
        }
        agent.SetDestination(newTarget.position);
        isMoving = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isMoving)
            return;

        if (agent.stoppingDistance >= agent.remainingDistance)
            Invoke("StartMoving", 3f);
    }
}
