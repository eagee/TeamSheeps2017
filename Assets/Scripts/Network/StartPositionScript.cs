using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartPositionScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnDrawGizmos()
    {
        Gizmos.DrawIcon(transform.position, "StartPosition.png", true);
    }
}
