using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;
using UnityEngine.Networking.Types;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Checks for match maker rooms that are active, if one isn't, then we start a server for that room.
/// errors gracefully.
/// </summary>
[RequireComponent(typeof(NetworkManager))]
public class MatchMakerHandler : MonoBehaviour {

    public bool edtUseMatchMaker = true;
    private bool m_useMatchMaker = true;
    private float m_ConnectionTimer = 0.0f;
    private const float CONNECTION_WAIT_TIME = 1.0f;

    private NetworkManager m_networkManager;


    /// <summary>
    /// Proivdes a single place where the match maker value is set
    /// </summary>
    public bool UseMatchMaker
    {
        get
        {
            return m_useMatchMaker;
        }
        set
        {
            //TODO: Add code here that will disconnect the active match maker client
            // if a user decides to switch into private mode.
            m_useMatchMaker = value;
        }
    }

    /// <summary>
    /// Used when object is initialized
    /// </summary>
    void Start()
    {
        UseMatchMaker = edtUseMatchMaker;
    }

    /// <summary>
    /// Awake is called after all other objects in the scene have been initialized, so we can grab other components safely.
    /// </summary>
    void Awake()
    {
        m_networkManager = GetComponent<NetworkManager>();
    }

    // Update is called once per frame
    void Update()
    {
        m_ConnectionTimer += Time.deltaTime;
        if (m_ConnectionTimer > CONNECTION_WAIT_TIME)
        {
            m_ConnectionTimer = 0.0f;

            // If this is a server, ensure that there is an active match being hosted via match maker;
            // otherwise attempt to connect to the server via match maker if the client is disconnected.
            if (!NetworkClient.active && !NetworkServer.active)
            {
                if (UseMatchMaker == true)
                {
                    HandleMatchmakerSetup();
                }
                else
                {
                    m_networkManager.StartHost();
                }
            }
        }// end check for connection timer.
    }

    /// <summary>
    /// Ensures that for a client or server matchmaker creates or connects to the necessary matches.
    /// </summary>
    private void HandleMatchmakerSetup()
    {
        if (m_networkManager.matchMaker == null)
        {
            Debug.Log("Starting Matchmaker!");
            m_networkManager.StartMatchMaker();
        }
        else if (m_networkManager.matchInfo == null)
        {
            if (m_networkManager.matches == null)
            {
                Debug.Log("Getting List of Matches!");
                m_networkManager.matchMaker.ListMatches(0, 20, m_networkManager.matchName, false, 0, 1, m_networkManager.OnMatchList);
            }
            else
            {
                Debug.Log("Match Count: " + m_networkManager.matches.Count.ToString());
                // If this is a client connection and we've got a populated list of matches, then go ahead and join the first one
                // (it's the only one we should have available for the show)
                if (m_networkManager.matches.Count > 0)
                {
                    Debug.Log("Joining TeamSheeps2017 Match!");
                    m_networkManager.matchName = m_networkManager.matches[0].name;
                    m_networkManager.matchSize = (uint)m_networkManager.matches[0].currentSize;
                    m_networkManager.matchMaker.JoinMatch(m_networkManager.matches[0].networkId, "", "", "", 0, 1, m_networkManager.OnMatchJoined);
                }
                else
                {
                    Debug.Log("No matches on Server, so we're starting the server!");
                    Debug.Log("Creating a Server Match!");
                    m_networkManager.matchMaker.CreateMatch(m_networkManager.matchName, m_networkManager.matchSize, true, "", "", "", 0, 1, m_networkManager.OnMatchCreate);
                }
            }
        }// end else if (m_networkManager.matchInfo == null)
        else
        {
            Debug.Log("Matchinfo is populated!");
        }
    }
}
