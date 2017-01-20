using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtTarget : MonoBehaviour {

    public Transform target;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        Quaternion targetRotation = Quaternion.LookRotation(target.position - transform.position);
        transform.rotation = target.rotation;
        
	}
}
