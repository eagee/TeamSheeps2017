using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavManager : MonoBehaviour
{
    public static NavManager instance;

    public Transform[] players;
    // Use this for initialization

    private void Awake()
    {
        instance = this;
    }

    public Transform GetPlayer()
    {
        int index = Random.Range(0, players.Length);
        return players[index];
    }
}
