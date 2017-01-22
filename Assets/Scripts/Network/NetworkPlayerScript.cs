using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.AI;

/// <summary>
/// Responsible for handling player startup, rotation, and synchronization over the network
/// </summary>
public class NetworkPlayerScript : NetworkBehaviour
{
    public static List<NetworkPlayerScript> BotsAndPlayers = new List<NetworkPlayerScript>();

    public Material[] FaceMaterials;

    public MeshRenderer FaceMeshRenderer;

    [Header("Options")]
    public float smoothSpeed = 10f;

    [SyncVar]
    private Vector3 mostRecentPos;

    [SyncVar]
    private Quaternion mostRecentRotation;

    private Vector3 prevPos;
    private Quaternion prevRotation;

    NetworkAnimator anim;

    private bool m_faceNeedsChange = true;

    private bool m_raycastReceived = false;
    private float m_raycastLostTimer = 0.0f;

    const int SMILE_FACE = 0;
    const int ANGRY_FACE = 1;
    const int NEUTRAL_FACE = 2;
    const int SAD_FACE = 3;

    // Changes the face of the player object to a smile if it's a player, and a random face otherwise.
    public void HandleFaceChange()
    {
        if (this.tag == "Player")
        {
            FaceMeshRenderer.sharedMaterial = FaceMaterials[SMILE_FACE];
            return;
        }

        int faceNumber = (int)Random.Range(0, 2);

        if (faceNumber == 0)
        {
            FaceMeshRenderer.sharedMaterial = FaceMaterials[ANGRY_FACE];
        }
        else if (faceNumber == 1)
        {
            FaceMeshRenderer.sharedMaterial = FaceMaterials[NEUTRAL_FACE];
        }
        else 
        {
            FaceMeshRenderer.sharedMaterial = FaceMaterials[SAD_FACE];
        }
    }

    // Changes the mask of the player object to a random mask prefab
    public void HandleMaskChange()
    {

    }

    private void DisableLocalPlayerComponents()
    {
        // Disable all of the non-player VR controls
        GetComponent<CharacterController>().enabled = false;
        GetComponent<OVRPlayerController>().enabled = false;
        GetComponent<OVRGamepadController>().enabled = false;
        GetComponent<OVRMainMenu>().enabled = false;
        GetComponentInChildren<OVRCameraController>().enabled = false;
        GetComponentInChildren<OVRDevice>().enabled = false;
        GetComponentInChildren<OVRDistortionCamera>().enabled = false;
        GetComponentInChildren<AudioListener>().enabled = false;
        GetComponentInChildren<MusicManager>().enabled = false;
        // Disable all child camera support components
        Camera[] childCameras = GetComponentsInChildren<Camera>();
        foreach (var cam in childCameras)
        {
            cam.enabled = false;
        }
        OVRCamera[] ovrCams = GetComponentsInChildren<OVRCamera>();
        foreach (var ovrCam in ovrCams)
        {
            ovrCam.enabled = false;
        }
        OVRLensCorrection[] ovrLensCorrs = GetComponentsInChildren<OVRLensCorrection>();
        foreach (var ovrLens in ovrLensCorrs)
        {
            ovrLens.enabled = false;
        }
        FlareLayer[] flareLayers = GetComponentsInChildren<FlareLayer>();
        foreach (var flare in flareLayers)
        {
            flare.enabled = false;
        }

        AudioSource [] Sounds = GetComponentsInChildren<AudioSource>();
        foreach (var sound in Sounds)
        {
           sound.enabled = false;
        }
    }

    private void SetupLocalPlayerComponents()
    {
        print("Setting local player components");
        // Get the list of start positions, and randomly choose one
        StartPositionScript[] startPositions = FindObjectsOfType<StartPositionScript>();
        int positionIndex = (int)Random.Range(0, (startPositions.Length - 1));
        this.transform.position = startPositions[positionIndex].transform.position;

        this.gameObject.tag = "Player";

        // For now, disable rendering meshes for the local player (so they don't get in the way of the camera)
        MeshRenderer[] meshRenderers = GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer mesh in meshRenderers)
        {
            mesh.enabled = false;
        }
    }

    public List<NetworkPlayerScript> GetBotsAndPlayers()
    {
        return BotsAndPlayers;
    }

    void Start()
    {
        m_faceNeedsChange = true;

        m_raycastReceived = false;

        m_raycastLostTimer = 0.0f;

        anim = GetComponent<NetworkAnimator>();

        BotsAndPlayers.Add(this);

        // If we're setting up the local player, we want the camera to follow him/her so we set the "Player" tag
        // (all other players are tagged "OtherPlayer" by default, and enable the PlayerController object so that
        // it's represented over the network : - )
        if (isLocalPlayer)
        {
            SetupLocalPlayerComponents();
        }
        else
        {
            DisableLocalPlayerComponents();
        }
    }

    // Checks for proximity of another object within proximity, and if it's a player and
    // this is a bot, then turn toward it.
    void OnTriggerEnter(Collider other)
    {
        if (this.gameObject.tag != "Player" && other.gameObject.tag == "Player")
        {
            GetComponent<NavMeshAgent>().SetDestination(other.gameObject.transform.position);
        }
    }

    void OnRaycastHitBot()
    {
        m_raycastReceived = true;
        m_raycastLostTimer = 0.0f;
    }

    void Update()
    {
        // Update our animations based on the player movement
        if (prevPos != transform.position)
        {
            anim.animator.SetBool("Walking", true);
        }
        else
        {
            anim.animator.SetBool("Walking", false);
        }

        if(m_faceNeedsChange)
        {
            m_faceNeedsChange = false;
            HandleFaceChange();
        }

         
        if(m_raycastReceived)
        {
            anim.animator.SetBool("MaskRemoved", true);
            m_raycastLostTimer += Time.deltaTime;
            if(m_raycastLostTimer > 3.25f)
            {
                m_raycastReceived = false;
            }
        }
        else
        {
            if(m_raycastLostTimer >= 3.25)
            {
                anim.animator.SetBool("MaskRemoved", false);
            }
        }
        
    }

}