using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// Responsible for handling player startup, rotation, and synchronization over the network
/// </summary>
public class NetworkPlayerScript : NetworkBehaviour
{
    public static List<NetworkPlayerScript> BotsAndPlayers = new List<NetworkPlayerScript>();

    [Header("Options")]
    public float smoothSpeed = 10f;

    [SyncVar]
    private Vector3 mostRecentPos;

    [SyncVar]
    private Quaternion mostRecentRotation;

    private Vector3 prevPos;
    private Quaternion prevRotation;

    NetworkAnimator anim;

    private float currentTransmitTime = 0.0f;
    private const float TRANSMIT_INTERVAL = 0.05f;

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
    }

    private void SetupLocalPlayerComponents()
    {
        print("Setting local player components");
        // Get the list of start positions, and randomly choose one
        StartPositionScript[] startPositions = FindObjectsOfType<StartPositionScript>();
        int positionIndex = (int)Random.Range(0, (startPositions.Length - 1));
        this.transform.position = startPositions[positionIndex].transform.position;

        // For now, disable rendering meshes for the local player (so they don't get in the way of the camera)
        MeshRenderer[] meshRenderers = GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer mesh in meshRenderers)
        {
            mesh.enabled = false;
        }
    }

    void Start()
    {
        anim = GetComponent<NetworkAnimator>();

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

    void LerpPositionAndRotation()
    {
        // Apply position to other players (mostRecentPos read from Server vis SyncVar)
        this.transform.position = Vector3.Lerp(transform.position, mostRecentPos, smoothSpeed * Time.deltaTime);
        //this.transform.rotation = Quaternion.Lerp(this.transform.rotation, mostRecentRotation, smoothSpeed * Time.deltaTime);
        //this.transform.rotation = Quaternion.Lerp(this.transform.rotation, mostRecentRotation, smoothSpeed * Time.deltaTime);
    }

    void TransmitPosition()
    {
        // If moved, send my data to server
        if (prevPos != transform.position || Mathf.Abs(prevRotation.eulerAngles.y - GetComponentInChildren<OVRCameraController>().transform.rotation.eulerAngles.y) > 0.10f)
        {
            // Pull Y rotation from the camera controller before sending so clients seem to turn with their camera.
            // Send position to server (function runs server-side)
            CmdSendDataToServer(transform.position, GetComponentInChildren<OVRCameraController>().transform.rotation);
            prevPos = transform.position;
            prevRotation = GetComponentInChildren<OVRCameraController>().transform.rotation;
        }
    }

    void FixedUpdate()
    {
        currentTransmitTime += Time.deltaTime;
        if(currentTransmitTime > TRANSMIT_INTERVAL)
        {
            currentTransmitTime = 0.0f;
            // We transmit position on a constant interval so that consistent updates are being made to the server.
            if (isLocalPlayer)
            {
                //TransmitPosition();
            }
        }
    }

    void Update()
    {
        // We lerp on update so that time.deltaTime isn't being executed on a fixed basis (since this will be different between different machines)
        if (!isLocalPlayer)
        {
           // LerpPositionAndRotation();
        }
        
        // Update our animations based on the player movement
        if (prevPos != transform.position)
        {
            anim.animator.SetBool("Walking", true);
        }
        else
        {
            anim.animator.SetBool("Walking", false);
        }
    }

    [Command]
    void CmdSendDataToServer(Vector3 pos, Quaternion rotation)
    {
        mostRecentPos = pos;
        mostRecentRotation = rotation;
    }

}