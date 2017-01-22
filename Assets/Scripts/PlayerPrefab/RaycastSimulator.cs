using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
///  Uses a collider and a trigger to simulate a forward raycast since we have so much
///  trouble lining the raycast up with the camera (based on our current rotation)
/// </summary>
public class RaycastSimulator : MonoBehaviour {

    public GameObject parentGameObject;
    
    // When we collide with a non-player object, we'll trigger the raycast behavior in them.
    void OnTriggerEnter(Collider other)
    {
        if (parentGameObject.tag == "Player" && other.gameObject.tag == "Bot")
        {
            // TODO: The the type of face from the other object and modify our musical state accordingly...
            other.gameObject.SendMessage("OnRaycastHitBot", parentGameObject.transform.position, SendMessageOptions.DontRequireReceiver);
            other.gameObject.GetComponent<NavMeshAgent>().SetDestination(parentGameObject.transform.position);
            int faceType = other.gameObject.GetComponent<NetworkPlayerScript>().faceMaterialIndex;
            if (faceType == 0)
                parentGameObject.GetComponent<MusicManager>().State = "winning";
            else if (faceType == 1)
                parentGameObject.GetComponent<MusicManager>().State = "angry";
            else if (faceType == 2)
                parentGameObject.GetComponent<MusicManager>().State = "boredom";
            else if (faceType == 3)
                parentGameObject.GetComponent<MusicManager>().State = "sad";
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (parentGameObject.tag == "Player" && other.gameObject.tag == "Player")
        {
            other.gameObject.SendMessage("OnRaycastHitPlayer", other.transform.position, SendMessageOptions.DontRequireReceiver);
        }
    }
}
