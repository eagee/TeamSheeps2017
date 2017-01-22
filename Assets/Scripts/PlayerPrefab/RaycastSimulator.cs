using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
