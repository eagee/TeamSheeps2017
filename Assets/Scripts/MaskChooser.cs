using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaskChooser : MonoBehaviour {

    public GameObject mask1;
    public GameObject mask2;

	// Use this for initialization
	void Start () {
        int randomInt = (int)Random.Range(1f, 500f);
        if (randomInt % 2 == 0) GameObject.Destroy(mask1);
        else GameObject.Destroy(mask1);
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
